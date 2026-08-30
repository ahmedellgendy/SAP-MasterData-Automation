using MasterDataAutomation.Application.Modules.CustomerCare.Dtos;
using MasterDataAutomation.Application.Modules.CustomerCare.Interfaces;
using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;
using MasterDataAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Services;

public class CustomerCareTicketService : ICustomerCareTicketService
{
    private readonly ApplicationDbContext _context;

    public CustomerCareTicketService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // Create
    // =========================================================

    public async Task<CustomerCareTicketDto> CreateAsync(
        CreateCustomerCareTicketDto dto,
        int createdByUserId,
        string createdByUserName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName))
            throw new InvalidOperationException(
                "Customer name is required.");

        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new InvalidOperationException(
                "Ticket description is required.");

        if (createdByUserId <= 0)
            throw new InvalidOperationException(
                "Invalid ticket creator.");

        var now = DateTime.UtcNow;

        // =====================================================
        // Category
        // =====================================================

        CustomerCareCategory? category = null;

        if (dto.CategoryId.HasValue)
        {
            category = await _context.CustomerCareCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == dto.CategoryId.Value &&
                        x.IsActive,
                    cancellationToken);

            if (category == null)
                throw new InvalidOperationException(
                    "Selected category was not found or is inactive.");
        }

        // =====================================================
        // SubCategory
        // =====================================================

        if (dto.SubCategoryId.HasValue)
        {
            if (!dto.CategoryId.HasValue)
                throw new InvalidOperationException(
                    "A category must be selected before selecting a subcategory.");

            var subCategoryExists =
                await _context.CustomerCareSubCategories
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == dto.SubCategoryId.Value &&
                            x.CategoryId == dto.CategoryId.Value &&
                            x.IsActive,
                        cancellationToken);

            if (!subCategoryExists)
                throw new InvalidOperationException(
                    "Selected subcategory does not belong to the selected category or is inactive.");
        }

        // =====================================================
        // Department
        // =====================================================

        if (dto.AssignedDepartmentId.HasValue)
        {
            var departmentExists =
                await _context.CustomerCareDepartments
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == dto.AssignedDepartmentId.Value &&
                            x.IsActive,
                        cancellationToken);

            if (!departmentExists)
                throw new InvalidOperationException(
                    "Selected department was not found or is inactive.");
        }

        // =====================================================
        // Quality Validation
        // =====================================================

        var isQualityTicket =
            category?.IsQualityCategory == true;

        if (isQualityTicket &&
            dto.QualityDetail == null)
        {
            throw new InvalidOperationException(
                "Quality details are required for quality complaints.");
        }

        if (!isQualityTicket &&
            dto.QualityDetail != null)
        {
            throw new InvalidOperationException(
                "Quality details can only be added to a quality ticket.");
        }

        // =====================================================
        // Priority / Critical
        // =====================================================

        var priority = dto.Priority;

        var isCritical =
            priority == CustomerCareTicketPriority.Critical;

        if (dto.QualityDetail != null &&
            (dto.QualityDetail.HasHealthRisk ||
             dto.QualityDetail.HasLegalRisk))
        {
            priority = CustomerCareTicketPriority.Critical;
            isCritical = true;
        }

        // =====================================================
        // SLA Policy
        // =====================================================

        var slaPolicy =
            await FindSlaPolicyAsync(
                dto.Type,
                dto.CategoryId,
                dto.SubCategoryId,
                priority,
                cancellationToken);

        DateTime? firstResponseDueAt = null;
        DateTime? resolutionDueAt = null;
        DateTime? escalationDueAt = null;

        if (slaPolicy != null)
        {
            firstResponseDueAt =
                now.AddMinutes(
                    slaPolicy.FirstResponseMinutes);

            resolutionDueAt =
                now.AddMinutes(
                    slaPolicy.ResolutionMinutes);

            if (slaPolicy.EscalationMinutes.HasValue)
            {
                escalationDueAt =
                    now.AddMinutes(
                        slaPolicy.EscalationMinutes.Value);
            }
        }

        // =====================================================
        // Initial Status
        // =====================================================

        var hasAssignment =
            dto.AssignedToUserId.HasValue ||
            dto.AssignedDepartmentId.HasValue;

        var initialStatus =
            hasAssignment
                ? CustomerCareTicketStatus.Assigned
                : CustomerCareTicketStatus.New;

        // =====================================================
        // Ticket
        // =====================================================

        var ticket = new CustomerCareTicket
        {
            TicketNumber = GenerateTicketNumber(),

            CreatedAt = now,
            CreatedByUserId = createdByUserId,
            LastActivityAt = now,

            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName.Trim(),
            CustomerPhone = NormalizePhone(dto.CustomerPhone),
            CustomerAddress = Clean(dto.CustomerAddress),

            Source = dto.Source,
            Type = dto.Type,

            CategoryId = dto.CategoryId,
            SubCategoryId = dto.SubCategoryId,

            Description = dto.Description.Trim(),

            Priority = priority,
            Status = initialStatus,

            AssignedToUserId = dto.AssignedToUserId,
            AssignedDepartmentId = dto.AssignedDepartmentId,

            AssignedAt =
                hasAssignment
                    ? now
                    : null,

            BranchId = dto.BranchId,
            SalesRepId = dto.SalesRepId,
            SupervisorId = dto.SupervisorId,

            SlaPolicyId = slaPolicy?.Id,

            FirstResponseDueAt = firstResponseDueAt,
            ResolutionDueAt = resolutionDueAt,
            EscalationDueAt = escalationDueAt,

            NextFollowUpAt = dto.NextFollowUpAt,

            IsCritical = isCritical,
            IsActive = true
        };

        // =====================================================
        // Quality Detail
        // =====================================================

        if (dto.QualityDetail != null)
        {
            ticket.QualityDetail =
                new CustomerCareQualityDetail
                {
                    ProductId =
                        dto.QualityDetail.ProductId,

                    ProductName =
                        dto.QualityDetail.ProductName
                            ?.Trim()
                        ?? string.Empty,

                    BatchNumber =
                        Clean(
                            dto.QualityDetail.BatchNumber),

                    ProductionDate =
                        dto.QualityDetail.ProductionDate,

                    ExpiryDate =
                        dto.QualityDetail.ExpiryDate,

                    QualityIssueType =
                        Clean(
                            dto.QualityDetail.QualityIssueType),

                    QualityIssueDetails =
                        Clean(
                            dto.QualityDetail.QualityIssueDetails),

                    SampleRequired =
                        dto.QualityDetail.SampleRequired,

                    HasHealthRisk =
                        dto.QualityDetail.HasHealthRisk,

                    HasLegalRisk =
                        dto.QualityDetail.HasLegalRisk,

                    RiskNotes =
                        Clean(
                            dto.QualityDetail.RiskNotes),

                    CreatedAt = now
                };
        }

        // =====================================================
        // Initial Timeline
        // =====================================================

        ticket.Actions.Add(
            new CustomerCareTicketAction
            {
                ActionType = "Created",

                Description =
                    "Ticket created.",

                CreatedByUserId =
                    createdByUserId,

                CreatedByUserName =
                    createdByUserName,

                CreatedAt = now
            });

        // =====================================================
        // Initial Status History
        // =====================================================

        ticket.StatusHistory.Add(
            new CustomerCareStatusHistory
            {
                FromStatus = null,

                ToStatus = initialStatus,

                Reason =
                    hasAssignment
                        ? "Ticket created and assigned."
                        : "Ticket created.",

                ChangedByUserId =
                    createdByUserId,

                ChangedByUserName =
                    createdByUserName,

                ChangedAt = now
            });

        // =====================================================
        // Initial Assignment History
        // =====================================================

        if (hasAssignment)
        {
            ticket.AssignmentHistory.Add(
                new CustomerCareAssignmentHistory
                {
                    FromUserId = null,

                    ToUserId =
                        dto.AssignedToUserId,

                    FromDepartmentId = null,

                    ToDepartmentId =
                        dto.AssignedDepartmentId,

                    Reason =
                        "Initial ticket assignment.",

                    ChangedByUserId =
                        createdByUserId,

                    ChangedByUserName =
                        createdByUserName,

                    ChangedAt = now
                });
        }

        // =====================================================
        // Save
        // =====================================================

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync(
                    cancellationToken);

        try
        {
            _context.CustomerCareTickets.Add(ticket);

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }

        return await GetRequiredDtoAsync(
            ticket.Id,
            cancellationToken);
    }

    // =========================================================
    // Get By Id
    // =========================================================

    public async Task<CustomerCareTicketDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return null;

        return await BuildTicketQuery()
            .Where(x => x.Id == id)
            .Select(x => MapToDto(x))
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    // =========================================================
    // Get By Number
    // =========================================================

    public async Task<CustomerCareTicketDto?> GetByTicketNumberAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber))
            return null;

        var normalizedNumber =
            ticketNumber.Trim();

        return await BuildTicketQuery()
            .Where(
                x =>
                    x.TicketNumber ==
                    normalizedNumber)
            .Select(x => MapToDto(x))
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    // =========================================================
    // SLA Policy Selection
    // =========================================================

    private async Task<CustomerCareSlaPolicy?> FindSlaPolicyAsync(
        CustomerCareTicketType ticketType,
        int? categoryId,
        int? subCategoryId,
        CustomerCareTicketPriority priority,
        CancellationToken cancellationToken)
    {
        return await _context.CustomerCareSlaPolicies
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Where(
                x =>
                    !x.TicketType.HasValue ||
                    x.TicketType == ticketType)
            .Where(
                x =>
                    !x.CategoryId.HasValue ||
                    x.CategoryId == categoryId)
            .Where(
                x =>
                    !x.SubCategoryId.HasValue ||
                    x.SubCategoryId == subCategoryId)
            .Where(
                x =>
                    !x.Priority.HasValue ||
                    x.Priority == priority)
            .OrderByDescending(
                x =>
                    (x.TicketType.HasValue ? 1 : 0) +
                    (x.CategoryId.HasValue ? 1 : 0) +
                    (x.SubCategoryId.HasValue ? 1 : 0) +
                    (x.Priority.HasValue ? 1 : 0))
            .ThenBy(x => x.SortOrder)
            .FirstOrDefaultAsync(
                cancellationToken);
    }


    public async Task<List<CustomerCareTicketListItemDto>> GetQueueAsync(
    CustomerCareTicketFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = _context.CustomerCareTickets
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.SubCategory)
            .Include(x => x.AssignedDepartment)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();

            query = query.Where(x =>
                x.TicketNumber.Contains(search) ||
                x.CustomerName.Contains(search) ||
                (x.CustomerPhone != null &&
                 x.CustomerPhone.Contains(search)));
        }

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        if (filter.Priority.HasValue)
            query = query.Where(x => x.Priority == filter.Priority.Value);

        if (filter.Source.HasValue)
            query = query.Where(x => x.Source == filter.Source.Value);

        if (filter.Type.HasValue)
            query = query.Where(x => x.Type == filter.Type.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == filter.CategoryId.Value);

        if (filter.SubCategoryId.HasValue)
            query = query.Where(x => x.SubCategoryId == filter.SubCategoryId.Value);

        if (filter.AssignedToUserId.HasValue)
            query = query.Where(x => x.AssignedToUserId == filter.AssignedToUserId.Value);

        if (filter.AssignedDepartmentId.HasValue)
            query = query.Where(x => x.AssignedDepartmentId == filter.AssignedDepartmentId.Value);

        if (filter.BranchId.HasValue)
            query = query.Where(x => x.BranchId == filter.BranchId.Value);

        if (filter.CreatedByUserId.HasValue)
        {
            query = query.Where(
                x => x.CreatedByUserId ==
                     filter.CreatedByUserId.Value);
        }
        if (filter.IsCritical.HasValue)
            query = query.Where(x => x.IsCritical == filter.IsCritical.Value);

        if (filter.OverdueOnly)
        {
            query = query.Where(x =>
                x.ResolutionDueAt.HasValue &&
                x.ResolutionDueAt.Value < now &&
                x.Status != CustomerCareTicketStatus.Resolved &&
                x.Status != CustomerCareTicketStatus.Closed &&
                x.Status != CustomerCareTicketStatus.Cancelled &&
                x.Status != CustomerCareTicketStatus.Rejected);
        }

        if (filter.FollowUpOnly)
        {
            query = query.Where(x =>
                x.NextFollowUpAt.HasValue &&
                x.NextFollowUpAt.Value <= now);
        }

        if (filter.FromDate.HasValue)
            query = query.Where(x => x.CreatedAt >= filter.FromDate.Value.Date);

        if (filter.ToDate.HasValue)
        {
            var toExclusive =
                filter.ToDate.Value.Date.AddDays(1);

            query = query.Where(x => x.CreatedAt < toExclusive);
        }

        return await query
            .OrderByDescending(x => x.IsCritical)
            .ThenBy(x => x.ResolutionDueAt)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new CustomerCareTicketListItemDto
            {
                Id = x.Id,
                TicketNumber = x.TicketNumber,
                CreatedAt = x.CreatedAt,
                CustomerName = x.CustomerName,
                CustomerPhone = x.CustomerPhone,
                Source = x.Source,
                Type = x.Type,
                CategoryName = x.Category != null ? x.Category.Name : null,
                SubCategoryName = x.SubCategory != null ? x.SubCategory.Name : null,
                Priority = x.Priority,
                Status = x.Status,
                AssignedToUserId = x.AssignedToUserId,
                AssignedDepartmentId = x.AssignedDepartmentId,
                AssignedDepartmentName =
                    x.AssignedDepartment != null
                        ? x.AssignedDepartment.Name
                        : null,
                ResolutionDueAt = x.ResolutionDueAt,
                NextFollowUpAt = x.NextFollowUpAt,
                IsCritical = x.IsCritical,

                IsOverdue =
                    x.ResolutionDueAt.HasValue &&
                    x.ResolutionDueAt.Value < now &&
                    x.Status != CustomerCareTicketStatus.Resolved &&
                    x.Status != CustomerCareTicketStatus.Closed &&
                    x.Status != CustomerCareTicketStatus.Cancelled &&
                    x.Status != CustomerCareTicketStatus.Rejected
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<CustomerCareDuplicateCustomerDto?>
    CheckDuplicateCustomerAsync(
        string phone,
        CancellationToken cancellationToken = default)
    {
        var normalizedPhone = NormalizePhone(phone);

        if (string.IsNullOrWhiteSpace(normalizedPhone))
            return null;

        var fromDate =
            DateTime.UtcNow.AddDays(-30);

        var tickets =
            await _context.CustomerCareTickets
                .AsNoTracking()
                .Where(x =>
                    x.CustomerPhone == normalizedPhone)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

        if (tickets.Count == 0)
            return null;

        var openStatuses =
            new[]
            {
            CustomerCareTicketStatus.New,
            CustomerCareTicketStatus.Assigned,
            CustomerCareTicketStatus.InProgress,
            CustomerCareTicketStatus.WaitingCustomer,
            CustomerCareTicketStatus.WaitingDepartment,
            CustomerCareTicketStatus.WaitingBranch,
            CustomerCareTicketStatus.Reopened
            };

        return new CustomerCareDuplicateCustomerDto
        {
            CustomerPhone = normalizedPhone,

            TotalTickets = tickets.Count,

            OpenTickets =
                tickets.Count(x =>
                    openStatuses.Contains(x.Status)),

            TicketsLast30Days =
                tickets.Count(x =>
                    x.CreatedAt >= fromDate),

            RecentTickets =
                tickets
                    .Take(5)
                    .Select(x =>
                        new CustomerCareTicketListItemDto
                        {
                            Id = x.Id,
                            TicketNumber = x.TicketNumber,
                            CreatedAt = x.CreatedAt,
                            CustomerName = x.CustomerName,
                            CustomerPhone = x.CustomerPhone,
                            Source = x.Source,
                            Type = x.Type,
                            Priority = x.Priority,
                            Status = x.Status,
                            ResolutionDueAt = x.ResolutionDueAt,
                            NextFollowUpAt = x.NextFollowUpAt,
                            IsCritical = x.IsCritical
                        })
                    .ToList()
        };
    }


    public async Task<List<CustomerCareTicketListItemDto>>
    GetCustomerHistoryAsync(
        string phone,
        CancellationToken cancellationToken = default)
    {
        var normalizedPhone = NormalizePhone(phone);

        if (string.IsNullOrWhiteSpace(normalizedPhone))
            return new List<CustomerCareTicketListItemDto>();

        return await _context.CustomerCareTickets
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.SubCategory)
            .Where(x => x.CustomerPhone == normalizedPhone)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x =>
                new CustomerCareTicketListItemDto
                {
                    Id = x.Id,
                    TicketNumber = x.TicketNumber,
                    CreatedAt = x.CreatedAt,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,
                    Source = x.Source,
                    Type = x.Type,
                    CategoryName =
                        x.Category != null
                            ? x.Category.Name
                            : null,
                    SubCategoryName =
                        x.SubCategory != null
                            ? x.SubCategory.Name
                            : null,
                    Priority = x.Priority,
                    Status = x.Status,
                    ResolutionDueAt = x.ResolutionDueAt,
                    NextFollowUpAt = x.NextFollowUpAt,
                    IsCritical = x.IsCritical
                })
            .ToListAsync(cancellationToken);
    }

    public async Task AssignAsync(
    int ticketId,
    AssignCustomerCareTicketDto dto,
    int changedByUserId,
    string changedByUserName,
    CancellationToken cancellationToken = default)
    {
        var ticket =
            await _context.CustomerCareTickets
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "Ticket was not found.");

        var now = DateTime.UtcNow;

        if (dto.AssignedDepartmentId.HasValue)
        {
            var departmentExists =
                await _context.CustomerCareDepartments
                    .AnyAsync(
                        x =>
                            x.Id == dto.AssignedDepartmentId.Value &&
                            x.IsActive,
                        cancellationToken);

            if (!departmentExists)
                throw new InvalidOperationException(
                    "Selected department was not found or is inactive.");
        }

        var oldUserId = ticket.AssignedToUserId;
        var oldDepartmentId = ticket.AssignedDepartmentId;

        ticket.AssignedToUserId = dto.AssignedToUserId;
        ticket.AssignedDepartmentId = dto.AssignedDepartmentId;
        ticket.AssignedAt = now;
        ticket.LastActivityAt = now;

        if (ticket.Status == CustomerCareTicketStatus.New)
            ticket.Status = CustomerCareTicketStatus.Assigned;

        ticket.AssignmentHistory.Add(
            new CustomerCareAssignmentHistory
            {
                FromUserId = oldUserId,
                ToUserId = dto.AssignedToUserId,
                FromDepartmentId = oldDepartmentId,
                ToDepartmentId = dto.AssignedDepartmentId,
                Reason = Clean(dto.Reason),
                ChangedByUserId = changedByUserId,
                ChangedByUserName = changedByUserName,
                ChangedAt = now
            });

        ticket.Actions.Add(
            new CustomerCareTicketAction
            {
                ActionType = "Assigned",
                Description = Clean(dto.Reason) ?? "Ticket assignment updated.",
                CreatedByUserId = changedByUserId,
                CreatedByUserName = changedByUserName,
                CreatedAt = now
            });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(
    int ticketId,
    ChangeCustomerCareTicketStatusDto dto,
    int changedByUserId,
    string changedByUserName,
    CancellationToken cancellationToken = default)
    {
        var ticket =
            await _context.CustomerCareTickets
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "Ticket was not found.");

        if (ticket.Status == dto.Status)
            return;

        var oldStatus = ticket.Status;
        var now = DateTime.UtcNow;

        ticket.Status = dto.Status;
        ticket.LastActivityAt = now;

        ticket.StatusHistory.Add(
            new CustomerCareStatusHistory
            {
                FromStatus = oldStatus,
                ToStatus = dto.Status,
                Reason = Clean(dto.Reason),
                ChangedByUserId = changedByUserId,
                ChangedByUserName = changedByUserName,
                ChangedAt = now
            });

        ticket.Actions.Add(
            new CustomerCareTicketAction
            {
                ActionType = "StatusChanged",
                Description =
                    $"{oldStatus} → {dto.Status}" +
                    (string.IsNullOrWhiteSpace(dto.Reason)
                        ? string.Empty
                        : $" - {dto.Reason.Trim()}"),
                CreatedByUserId = changedByUserId,
                CreatedByUserName = changedByUserName,
                CreatedAt = now
            });

        await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task AddActionAsync(
    int ticketId,
    AddCustomerCareTicketActionDto dto,
    int createdByUserId,
    string createdByUserName,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.ActionType))
            throw new InvalidOperationException(
                "Action type is required.");

        var ticket =
            await _context.CustomerCareTickets
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "Ticket was not found.");

        var now = DateTime.UtcNow;

        ticket.Actions.Add(
            new CustomerCareTicketAction
            {
                ActionType = dto.ActionType.Trim(),
                Description = Clean(dto.Description),
                IsInternalNote = dto.IsInternalNote,
                FollowUpAt = dto.FollowUpAt,
                CreatedByUserId = createdByUserId,
                CreatedByUserName = createdByUserName,
                CreatedAt = now
            });

        ticket.LastActivityAt = now;

        if (dto.FollowUpAt.HasValue)
            ticket.NextFollowUpAt = dto.FollowUpAt;

        if (!ticket.FirstRespondedAt.HasValue &&
            !dto.IsInternalNote)
        {
            ticket.FirstRespondedAt = now;

            if (ticket.FirstResponseDueAt.HasValue &&
                now > ticket.FirstResponseDueAt.Value)
            {
                ticket.IsFirstResponseBreached = true;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ResolveAsync(
    int ticketId,
    ResolveCustomerCareTicketDto dto,
    int changedByUserId,
    string changedByUserName,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.ResolutionSummary))
            throw new InvalidOperationException(
                "Resolution summary is required.");

        var ticket =
            await _context.CustomerCareTickets
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "Ticket was not found.");

        var now = DateTime.UtcNow;
        var oldStatus = ticket.Status;

        ticket.Status =
            CustomerCareTicketStatus.Resolved;

        ticket.ResolutionSummary =
            dto.ResolutionSummary.Trim();

        ticket.ResolvedAt = now;
        ticket.LastActivityAt = now;

        ticket.IsResolutionBreached =
            ticket.ResolutionDueAt.HasValue &&
            now > ticket.ResolutionDueAt.Value;

        ticket.StatusHistory.Add(
            new CustomerCareStatusHistory
            {
                FromStatus = oldStatus,
                ToStatus = CustomerCareTicketStatus.Resolved,
                Reason = dto.ResolutionSummary.Trim(),
                ChangedByUserId = changedByUserId,
                ChangedByUserName = changedByUserName,
                ChangedAt = now
            });

        ticket.Actions.Add(
            new CustomerCareTicketAction
            {
                ActionType = "Resolved",
                Description = dto.ResolutionSummary.Trim(),
                CreatedByUserId = changedByUserId,
                CreatedByUserName = changedByUserName,
                CreatedAt = now
            });

        await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task ReopenAsync(
    int ticketId,
    string reason,
    int changedByUserId,
    string changedByUserName,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException(
                "Reopen reason is required.");

        var ticket =
            await _context.CustomerCareTickets
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "Ticket was not found.");

        if (ticket.Status != CustomerCareTicketStatus.Resolved &&
            ticket.Status != CustomerCareTicketStatus.Closed)
        {
            throw new InvalidOperationException(
                "Only resolved or closed tickets can be reopened.");
        }

        var now = DateTime.UtcNow;
        var oldStatus = ticket.Status;

        ticket.Status =
            CustomerCareTicketStatus.Reopened;

        ticket.ResolvedAt = null;
        ticket.ClosedAt = null;

        ticket.ReopenCount++;
        ticket.LastActivityAt = now;

        ticket.StatusHistory.Add(
            new CustomerCareStatusHistory
            {
                FromStatus = oldStatus,
                ToStatus = CustomerCareTicketStatus.Reopened,
                Reason = reason.Trim(),
                ChangedByUserId = changedByUserId,
                ChangedByUserName = changedByUserName,
                ChangedAt = now
            });

        ticket.Actions.Add(
            new CustomerCareTicketAction
            {
                ActionType = "Reopened",
                Description = reason.Trim(),
                CreatedByUserId = changedByUserId,
                CreatedByUserName = changedByUserName,
                CreatedAt = now
            });

        await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task<List<CustomerCareLookupDto>> GetCategoriesAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.CustomerCareCategories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CustomerCareLookupDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CustomerCareLookupDto>> GetSubCategoriesAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CustomerCareSubCategories
            .AsNoTracking()
            .Where(x =>
                x.CategoryId == categoryId &&
                x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CustomerCareLookupDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CustomerCareLookupDto>> GetDepartmentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.CustomerCareDepartments
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CustomerCareLookupDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<CustomerCareTicketDetailsDto?> GetDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return null;

        var ticket =
            await _context.CustomerCareTickets
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CustomerCareTicketDetailsDto
                {
                    Id = x.Id,

                    TicketNumber = x.TicketNumber,

                    CreatedAt = x.CreatedAt,

                    LastActivityAt = x.LastActivityAt,

                    CustomerId = x.CustomerId,

                    CustomerName = x.CustomerName,

                    CustomerPhone = x.CustomerPhone,

                    CustomerAddress = x.CustomerAddress,

                    Source = x.Source,

                    Type = x.Type,

                    CategoryId = x.CategoryId,

                    CategoryName =
                        x.Category != null
                            ? x.Category.Name
                            : null,

                    SubCategoryId = x.SubCategoryId,

                    SubCategoryName =
                        x.SubCategory != null
                            ? x.SubCategory.Name
                            : null,

                    Description = x.Description,

                    Priority = x.Priority,

                    Status = x.Status,

                    IsCritical = x.IsCritical,

                    AssignedToUserId = x.AssignedToUserId,

                    AssignedDepartmentId = x.AssignedDepartmentId,

                    AssignedDepartmentName =
                        x.AssignedDepartment != null
                            ? x.AssignedDepartment.Name
                            : null,

                    AssignedAt = x.AssignedAt,

                    FirstResponseDueAt = x.FirstResponseDueAt,

                    FirstRespondedAt = x.FirstRespondedAt,

                    ResolutionDueAt = x.ResolutionDueAt,

                    EscalationDueAt = x.EscalationDueAt,

                    IsFirstResponseBreached = x.IsFirstResponseBreached,

                    IsResolutionBreached = x.IsResolutionBreached,

                    NextFollowUpAt = x.NextFollowUpAt,

                    ResolutionSummary = x.ResolutionSummary,

                    ResolvedAt = x.ResolvedAt,

                    ClosedAt = x.ClosedAt,
                    CreatedByUserId = x.CreatedByUserId,

                    ReopenCount = x.ReopenCount,

                    Gifts = x.Gifts
                     .OrderByDescending(g => g.CreatedAt)
                     .Select(g =>
        new CustomerCareTicketGiftDto
        {
            Id = g.Id,
            ProductName = g.ProductName,
            Quantity = g.Quantity,
            RecipientName = g.RecipientName,
            RecipientPhone = g.RecipientPhone,
            Notes = g.Notes,
            CreatedAt = g.CreatedAt,
            CreatedByUserId = g.CreatedByUserId,
            CreatedByUserName = g.CreatedByUserName
        })
                     .ToList(),

                    Attachments = x.Attachments
                       .Where(a =>
                           a.AttachmentType == "QualityEvidence")
                       .OrderByDescending(a => a.UploadedAt)
                       .Select(a =>
        new CustomerCareAttachmentDto
        {
            Id = a.Id,

            FileName = a.FileName,

            FilePath = a.FilePath,

            ContentType = a.ContentType,

            FileSize = a.FileSize,

            AttachmentType =
                a.AttachmentType,

            Description =
                a.Description,

            UploadedByUserId =
                a.UploadedByUserId,

            UploadedByUserName =
                a.UploadedByUserName,

            UploadedAt =
                a.UploadedAt
        })
                       .ToList(),

                    QualityDetail =
                        x.QualityDetail == null
                            ? null
                            : new CustomerCareQualityDetailDto
                            {
                                ProductId =
                                    x.QualityDetail.ProductId,

                                ProductName =
                                    x.QualityDetail.ProductName,

                                BatchNumber =
                                    x.QualityDetail.BatchNumber,

                                ProductionDate =
                                    x.QualityDetail.ProductionDate,

                                ExpiryDate =
                                    x.QualityDetail.ExpiryDate,

                                QualityIssueType =
                                    x.QualityDetail.QualityIssueType,

                                QualityIssueDetails =
                                    x.QualityDetail.QualityIssueDetails,

                                SampleRequired =
                                    x.QualityDetail.SampleRequired,

                                SampleCollected =
                                    x.QualityDetail.SampleCollected,

                                SampleCollectedAt =
                                    x.QualityDetail.SampleCollectedAt,

                                QualityDecision =
                                    x.QualityDetail.QualityDecision,

                                CompensationType =
                                    x.QualityDetail.CompensationType,

                                CompensationQuantity =
                                    x.QualityDetail.CompensationQuantity,

                                CompensationNotes =
                                    x.QualityDetail.CompensationNotes,

                                HasHealthRisk =
                                    x.QualityDetail.HasHealthRisk,

                                HasLegalRisk =
                                    x.QualityDetail.HasLegalRisk,

                                RiskNotes =
                                    x.QualityDetail.RiskNotes
                            },

                    Actions =
                        x.Actions
                            .OrderByDescending(a => a.CreatedAt)
                            .Select(a =>
                                new CustomerCareTicketActionDto
                                {
                                    Id = a.Id,

                                    ActionType =
                                        a.ActionType,

                                    Description =
                                        a.Description,

                                    IsInternalNote =
                                        a.IsInternalNote,

                                    FollowUpAt =
                                        a.FollowUpAt,

                                    CreatedByUserId =
                                        a.CreatedByUserId,

                                    CreatedByUserName =
                                        a.CreatedByUserName,

                                    CreatedAt =
                                        a.CreatedAt
                                })
                            .ToList()
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (ticket == null)
            return null;

        if (!string.IsNullOrWhiteSpace(ticket.CustomerPhone))
        {
            ticket.CustomerHistory =
                await GetCustomerHistoryAsync(
                    ticket.CustomerPhone,
                    cancellationToken);
        }

        return ticket;
    }

    public async Task CloseAsync(
    int ticketId,
    string reason,
    int changedByUserId,
    string changedByUserName,
    CancellationToken cancellationToken = default)
    {
        var ticket =
            await _context.CustomerCareTickets
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "Ticket was not found.");

        if (ticket.Status != CustomerCareTicketStatus.Resolved)
        {
            throw new InvalidOperationException(
                "Only resolved tickets can be closed.");
        }

        var now = DateTime.UtcNow;

        ticket.Status =
            CustomerCareTicketStatus.Closed;

        ticket.ClosedAt = now;
        ticket.LastActivityAt = now;

        ticket.StatusHistory.Add(
            new CustomerCareStatusHistory
            {
                FromStatus =
                    CustomerCareTicketStatus.Resolved,

                ToStatus =
                    CustomerCareTicketStatus.Closed,

                Reason =
                    string.IsNullOrWhiteSpace(reason)
                        ? "Ticket closed."
                        : reason.Trim(),

                ChangedByUserId =
                    changedByUserId,

                ChangedByUserName =
                    changedByUserName,

                ChangedAt = now
            });

        ticket.Actions.Add(
            new CustomerCareTicketAction
            {
                ActionType = "Closed",

                Description =
                    string.IsNullOrWhiteSpace(reason)
                        ? "Ticket closed."
                        : reason.Trim(),

                CreatedByUserId =
                    changedByUserId,

                CreatedByUserName =
                    changedByUserName,

                CreatedAt = now
            });

        await _context.SaveChangesAsync(
            cancellationToken);
    }


    public async Task<CustomerCareDashboardDto> GetDashboardAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = _context.CustomerCareTickets
            .AsNoTracking()
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(
                x => x.CreatedAt >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var toExclusive =
                toDate.Value.Date.AddDays(1);

            query = query.Where(
                x => x.CreatedAt < toExclusive);
        }

        var totalTickets =
            await query.CountAsync(cancellationToken);

        var openStatuses =
            new[]
            {
            CustomerCareTicketStatus.New,
            CustomerCareTicketStatus.Assigned,
            CustomerCareTicketStatus.InProgress,
            CustomerCareTicketStatus.WaitingCustomer,
            CustomerCareTicketStatus.WaitingDepartment,
            CustomerCareTicketStatus.WaitingBranch,
            CustomerCareTicketStatus.Reopened
            };

        var openTickets =
            await query.CountAsync(
                x => openStatuses.Contains(x.Status),
                cancellationToken);

        var newTickets =
            await query.CountAsync(
                x => x.Status == CustomerCareTicketStatus.New,
                cancellationToken);

        var inProgressTickets =
            await query.CountAsync(
                x => x.Status == CustomerCareTicketStatus.InProgress,
                cancellationToken);

        var resolvedTickets =
            await query.CountAsync(
                x => x.Status == CustomerCareTicketStatus.Resolved,
                cancellationToken);

        var closedTickets =
            await query.CountAsync(
                x => x.Status == CustomerCareTicketStatus.Closed,
                cancellationToken);

        var criticalTickets =
            await query.CountAsync(
                x =>
                    x.IsCritical &&
                    openStatuses.Contains(x.Status),
                cancellationToken);

        var reopenedTickets =
            await query.CountAsync(
                x => x.ReopenCount > 0,
                cancellationToken);

        var overdueTickets =
            await query.CountAsync(
                x =>
                    x.ResolutionDueAt.HasValue &&
                    x.ResolutionDueAt.Value < now &&
                    openStatuses.Contains(x.Status),
                cancellationToken);

        var followUpDueTickets =
            await query.CountAsync(
                x =>
                    x.NextFollowUpAt.HasValue &&
                    x.NextFollowUpAt.Value <= now &&
                    openStatuses.Contains(x.Status),
                cancellationToken);

        // =====================================================
        // Average Resolution Time
        // =====================================================

        var resolvedDurations =
            await query
                .Where(
                    x =>
                        x.ResolvedAt.HasValue)
                .Select(
                    x =>
                        EF.Functions.DateDiffMinute(
                            x.CreatedAt,
                            x.ResolvedAt!.Value))
                .ToListAsync(
                    cancellationToken);

        decimal averageResolutionHours = 0;

        if (resolvedDurations.Count > 0)
        {
            averageResolutionHours =
                Math.Round(
                    (decimal)resolvedDurations.Average() / 60m,
                    2);
        }

        // =====================================================
        // Resolution SLA
        // =====================================================

        var ticketsWithResolutionSla =
            await query.CountAsync(
                x => x.ResolutionDueAt.HasValue,
                cancellationToken);

        var resolutionBreached =
            await query.CountAsync(
                x =>
                    x.ResolutionDueAt.HasValue &&
                    x.IsResolutionBreached,
                cancellationToken);

        decimal slaCompliance = 0;

        if (ticketsWithResolutionSla > 0)
        {
            slaCompliance =
                Math.Round(
                    ((decimal)(
                        ticketsWithResolutionSla -
                        resolutionBreached)
                     / ticketsWithResolutionSla)
                    * 100m,
                    2);
        }

        // =====================================================
        // First Response SLA
        // =====================================================

        var ticketsWithFirstResponseSla =
            await query.CountAsync(
                x => x.FirstResponseDueAt.HasValue,
                cancellationToken);

        var firstResponseBreached =
            await query.CountAsync(
                x =>
                    x.FirstResponseDueAt.HasValue &&
                    x.IsFirstResponseBreached,
                cancellationToken);

        decimal firstResponseCompliance = 0;

        if (ticketsWithFirstResponseSla > 0)
        {
            firstResponseCompliance =
                Math.Round(
                    ((decimal)(
                        ticketsWithFirstResponseSla -
                        firstResponseBreached)
                     / ticketsWithFirstResponseSla)
                    * 100m,
                    2);
        }

        // =====================================================
        // Category Breakdown
        // =====================================================

        var byCategory =
            await query
                .GroupBy(
                    x =>
                        x.Category != null
                            ? x.Category.Name
                            : "Uncategorized")
                .Select(
                    g =>
                        new
                        {
                            Name = g.Key,
                            Count = g.Count()
                        })
                .OrderByDescending(x => x.Count)
                .ToListAsync(
                    cancellationToken);

        var categoryBreakdown =
            byCategory
                .Select(
                    x =>
                        new CustomerCareDashboardBreakdownDto
                        {
                            Name = x.Name,
                            Count = x.Count,

                            Percentage =
                                totalTickets == 0
                                    ? 0
                                    : Math.Round(
                                        (decimal)x.Count /
                                        totalTickets *
                                        100m,
                                        2)
                        })
                .ToList();

        // =====================================================
        // Source Breakdown
        // =====================================================

        var bySource =
            await query
                .GroupBy(x => x.Source)
                .Select(
                    g =>
                        new
                        {
                            Source = g.Key,
                            Count = g.Count()
                        })
                .OrderByDescending(x => x.Count)
                .ToListAsync(
                    cancellationToken);

        var sourceBreakdown =
            bySource
                .Select(
                    x =>
                        new CustomerCareDashboardBreakdownDto
                        {
                            Name = x.Source.ToString(),
                            Count = x.Count,

                            Percentage =
                                totalTickets == 0
                                    ? 0
                                    : Math.Round(
                                        (decimal)x.Count /
                                        totalTickets *
                                        100m,
                                        2)
                        })
                .ToList();

        // =====================================================
        // Department Breakdown
        // =====================================================

        var byDepartment =
            await query
                .GroupBy(
                    x =>
                        x.AssignedDepartment != null
                            ? x.AssignedDepartment.Name
                            : "Unassigned")
                .Select(
                    g =>
                        new
                        {
                            Name = g.Key,
                            Count = g.Count()
                        })
                .OrderByDescending(x => x.Count)
                .ToListAsync(
                    cancellationToken);

        var departmentBreakdown =
            byDepartment
                .Select(
                    x =>
                        new CustomerCareDashboardBreakdownDto
                        {
                            Name = x.Name,
                            Count = x.Count,

                            Percentage =
                                totalTickets == 0
                                    ? 0
                                    : Math.Round(
                                        (decimal)x.Count /
                                        totalTickets *
                                        100m,
                                        2)
                        })
                .ToList();

        // =====================================================
        // Critical Open
        // =====================================================

        var criticalOpenTickets =
            await query
                .Where(
                    x =>
                        x.IsCritical &&
                        openStatuses.Contains(x.Status))
                .OrderBy(x => x.ResolutionDueAt)
                .Take(10)
                .Select(
                    x =>
                        new CustomerCareTicketListItemDto
                        {
                            Id = x.Id,
                            TicketNumber = x.TicketNumber,
                            CreatedAt = x.CreatedAt,
                            CustomerName = x.CustomerName,
                            CustomerPhone = x.CustomerPhone,
                            Source = x.Source,
                            Type = x.Type,

                            CategoryName =
                                x.Category != null
                                    ? x.Category.Name
                                    : null,

                            SubCategoryName =
                                x.SubCategory != null
                                    ? x.SubCategory.Name
                                    : null,

                            Priority = x.Priority,
                            Status = x.Status,

                            AssignedDepartmentName =
                                x.AssignedDepartment != null
                                    ? x.AssignedDepartment.Name
                                    : null,

                            ResolutionDueAt = x.ResolutionDueAt,

                            IsCritical = x.IsCritical,

                            IsOverdue =
                                x.ResolutionDueAt.HasValue &&
                                x.ResolutionDueAt.Value < now
                        })
                .ToListAsync(
                    cancellationToken);

        // =====================================================
        // Overdue
        // =====================================================

        var overdueItems =
            await query
                .Where(
                    x =>
                        x.ResolutionDueAt.HasValue &&
                        x.ResolutionDueAt.Value < now &&
                        openStatuses.Contains(x.Status))
                .OrderBy(x => x.ResolutionDueAt)
                .Take(10)
                .Select(
                    x =>
                        new CustomerCareTicketListItemDto
                        {
                            Id = x.Id,
                            TicketNumber = x.TicketNumber,
                            CreatedAt = x.CreatedAt,
                            CustomerName = x.CustomerName,
                            CustomerPhone = x.CustomerPhone,
                            Source = x.Source,
                            Type = x.Type,

                            CategoryName =
                                x.Category != null
                                    ? x.Category.Name
                                    : null,

                            SubCategoryName =
                                x.SubCategory != null
                                    ? x.SubCategory.Name
                                    : null,

                            Priority = x.Priority,
                            Status = x.Status,

                            AssignedDepartmentName =
                                x.AssignedDepartment != null
                                    ? x.AssignedDepartment.Name
                                    : null,

                            ResolutionDueAt = x.ResolutionDueAt,

                            IsCritical = x.IsCritical,
                            IsOverdue = true
                        })
                .ToListAsync(
                    cancellationToken);

        // =====================================================
        // Gifts
        // =====================================================

        var giftsQuery =
            _context.CustomerCareTicketGifts
                .AsNoTracking()
                .Where(x => query.Select(t => t.Id).Contains(x.TicketId));

        var totalGiftQuantity =
            await giftsQuery
                .SumAsync(
                    x => (int?)x.Quantity,
                    cancellationToken)
            ?? 0;

        var ticketsWithGifts =
            await giftsQuery
                .Select(x => x.TicketId)
                .Distinct()
                .CountAsync(cancellationToken);

        var topGiftProductsRaw =
            await giftsQuery
                .GroupBy(x => x.ProductName)
                .Select(g =>
                    new
                    {
                        Name = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToListAsync(cancellationToken);

        var topGiftProducts =
            topGiftProductsRaw
                .Select(x =>
                    new CustomerCareDashboardBreakdownDto
                    {
                        Name = x.Name,

                        Count = x.Quantity,

                        Percentage =
                            totalGiftQuantity == 0
                                ? 0
                                : Math.Round(
                                    (decimal)x.Quantity /
                                    totalGiftQuantity *
                                    100m,
                                    2)
                    })
                .ToList();

        var recentGifts =
            await giftsQuery
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .Select(x =>
                    new CustomerCareDashboardGiftDto
                    {
                        TicketId = x.TicketId,

                        TicketNumber =
                            x.Ticket.TicketNumber,

                        CustomerName =
                            x.Ticket.CustomerName,

                        ProductName =
                            x.ProductName,

                        Quantity =
                            x.Quantity,

                        RecipientName =
                            x.RecipientName,

                        RecipientPhone =
                            x.RecipientPhone,

                        CreatedByUserName =
                            x.CreatedByUserName,

                        CreatedAt =
                            x.CreatedAt
                    })
                .ToListAsync(cancellationToken);

        return new CustomerCareDashboardDto
        {
            TotalTickets = totalTickets,

            OpenTickets = openTickets,

            NewTickets = newTickets,

            InProgressTickets = inProgressTickets,

            OverdueTickets = overdueTickets,

            ResolvedTickets = resolvedTickets,

            ClosedTickets = closedTickets,

            CriticalTickets = criticalTickets,

            ReopenedTickets = reopenedTickets,

            FollowUpDueTickets = followUpDueTickets,

            AverageResolutionHours =
                averageResolutionHours,

            SlaCompliancePercentage =
                slaCompliance,

            FirstResponseCompliancePercentage =
                firstResponseCompliance,

            ByCategory =
                categoryBreakdown,

            BySource =
                sourceBreakdown,

            ByDepartment =
                departmentBreakdown,

            CriticalOpenTickets =
                criticalOpenTickets,

            OverdueItems =
                overdueItems,

            TotalGiftQuantity =
                 totalGiftQuantity,

            TicketsWithGifts =
                 ticketsWithGifts,

            TopGiftProducts =
                 topGiftProducts,

            RecentGifts =
                 recentGifts
        };
    }



    public async Task AddGiftAsync(
    int ticketId,
    AddCustomerCareTicketGiftDto dto,
    int createdByUserId,
    string createdByUserName,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.ProductName))
            throw new InvalidOperationException(
                "يجب إدخال اسم المنتج.");

        if (dto.Quantity <= 0)
            throw new InvalidOperationException(
                "كمية الهدية يجب أن تكون أكبر من صفر.");

        var ticket =
            await _context.CustomerCareTickets
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "التذكرة غير موجودة.");

        var gift =
            new CustomerCareTicketGift
            {
                TicketId = ticketId,

                ProductName =
                    dto.ProductName.Trim(),

                Quantity =
                    dto.Quantity,

                RecipientName =
                    string.IsNullOrWhiteSpace(dto.RecipientName)
                        ? ticket.CustomerName
                        : dto.RecipientName.Trim(),

                RecipientPhone =
                    string.IsNullOrWhiteSpace(dto.RecipientPhone)
                        ? ticket.CustomerPhone
                        : dto.RecipientPhone.Trim(),

                Notes =
                    string.IsNullOrWhiteSpace(dto.Notes)
                        ? null
                        : dto.Notes.Trim(),

                CreatedAt =
                    DateTime.UtcNow,

                CreatedByUserId =
                    createdByUserId,

                CreatedByUserName =
                    createdByUserName
            };

        _context.CustomerCareTicketGifts.Add(gift);

        _context.CustomerCareTicketActions.Add(
            new CustomerCareTicketAction
            {
                TicketId = ticketId,

                ActionType = "GiftAdded",

                Description =
                    $"تم تسجيل هدية: {gift.ProductName} × {gift.Quantity}",

                IsInternalNote = true,

                CreatedByUserId =
                    createdByUserId,

                CreatedByUserName =
                    createdByUserName,

                CreatedAt =
                    DateTime.UtcNow
            });

        ticket.LastActivityAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteGiftAsync(
    int ticketId,
    int giftId,
    CancellationToken cancellationToken = default)
    {
        var gift =
            await _context.CustomerCareTicketGifts
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == giftId &&
                        x.TicketId == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "الهدية غير موجودة.");

        _context.CustomerCareTicketGifts.Remove(gift);

        await _context.SaveChangesAsync(
            cancellationToken);
    }


    public async Task AddAttachmentAsync(
    int ticketId,
    AddCustomerCareAttachmentDto dto,
    int uploadedByUserId,
    string uploadedByUserName,
    CancellationToken cancellationToken = default)
    {
        var ticket =
            await _context.CustomerCareTickets
                .Include(x => x.Category)
                .FirstOrDefaultAsync(
                    x => x.Id == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "التذكرة غير موجودة.");

        if (ticket.Category == null ||
            !ticket.Category.IsQualityCategory)
        {
            throw new InvalidOperationException(
                "رفع صور الجودة متاح فقط لشكاوى الجودة.");
        }

        if (string.IsNullOrWhiteSpace(dto.FileName) ||
            string.IsNullOrWhiteSpace(dto.StoredFileName) ||
            string.IsNullOrWhiteSpace(dto.FilePath))
        {
            throw new InvalidOperationException(
                "بيانات الملف غير مكتملة.");
        }

        var now = DateTime.UtcNow;

        var attachment =
            new CustomerCareAttachment
            {
                TicketId = ticketId,

                FileName = dto.FileName,

                StoredFileName = dto.StoredFileName,

                FilePath = dto.FilePath,

                ContentType = dto.ContentType,

                FileSize = dto.FileSize,

                AttachmentType = "QualityEvidence",

                Description =
                    string.IsNullOrWhiteSpace(dto.Description)
                        ? null
                        : dto.Description.Trim(),

                UploadedByUserId = uploadedByUserId,

                UploadedByUserName = uploadedByUserName,

                UploadedAt = now
            };

        _context.CustomerCareAttachments.Add(
            attachment);

        _context.CustomerCareTicketActions.Add(
            new CustomerCareTicketAction
            {
                TicketId = ticketId,

                ActionType = "QualityImageUploaded",

                Description =
                    $"تم رفع صورة لشكوى الجودة: {dto.FileName}",

                IsInternalNote = true,

                CreatedByUserId =
                    uploadedByUserId,

                CreatedByUserName =
                    uploadedByUserName,

                CreatedAt = now
            });

        ticket.LastActivityAt = now;

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<string> DeleteAttachmentAsync(
    int ticketId,
    int attachmentId,
    CancellationToken cancellationToken = default)
    {
        var attachment =
            await _context.CustomerCareAttachments
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == attachmentId &&
                        x.TicketId == ticketId,
                    cancellationToken)
            ?? throw new KeyNotFoundException(
                "الصورة المرفقة غير موجودة.");

        var filePath =
            attachment.FilePath;

        _context.CustomerCareAttachments.Remove(
            attachment);

        await _context.SaveChangesAsync(
            cancellationToken);

        return filePath;
    }

    // =========================================================
    // Query
    // =========================================================

    private IQueryable<CustomerCareTicket> BuildTicketQuery()
    {
        return _context.CustomerCareTickets
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.SubCategory)
            .Include(x => x.AssignedDepartment);
    }

    // =========================================================
    // Required DTO
    // =========================================================

    private async Task<CustomerCareTicketDto> GetRequiredDtoAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await GetByIdAsync(
                   id,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   "Ticket was created but could not be retrieved.");
    }

    // =========================================================
    // Mapping
    // =========================================================

    private static CustomerCareTicketDto MapToDto(
        CustomerCareTicket ticket)
    {
        return new CustomerCareTicketDto
        {
            Id = ticket.Id,

            TicketNumber =
                ticket.TicketNumber,

            CreatedAt =
                ticket.CreatedAt,

            CustomerName =
                ticket.CustomerName,

            CustomerPhone =
                ticket.CustomerPhone,

            CustomerAddress =
                ticket.CustomerAddress,

            Source =
                ticket.Source,

            Type =
                ticket.Type,

            CategoryId =
                ticket.CategoryId,

            CategoryName =
                ticket.Category?.Name,

            SubCategoryId =
                ticket.SubCategoryId,

            SubCategoryName =
                ticket.SubCategory?.Name,

            Description =
                ticket.Description,

            Priority =
                ticket.Priority,

            Status =
                ticket.Status,

            AssignedToUserId =
                ticket.AssignedToUserId,

            AssignedDepartmentId =
                ticket.AssignedDepartmentId,

            AssignedDepartmentName =
                ticket.AssignedDepartment?.Name,

            BranchId =
                ticket.BranchId,

            SalesRepId =
                ticket.SalesRepId,

            SupervisorId =
                ticket.SupervisorId,

            FirstResponseDueAt =
                ticket.FirstResponseDueAt,

            ResolutionDueAt =
                ticket.ResolutionDueAt,

            NextFollowUpAt =
                ticket.NextFollowUpAt,

            IsFirstResponseBreached =
                ticket.IsFirstResponseBreached,

            IsResolutionBreached =
                ticket.IsResolutionBreached,

            IsCritical =
                ticket.IsCritical,

            ResolutionSummary =
                ticket.ResolutionSummary,

            ResolvedAt =
                ticket.ResolvedAt,

            ClosedAt =
                ticket.ClosedAt,

            ReopenCount =
                ticket.ReopenCount
        };
    }

    // =========================================================
    // Ticket Number
    // =========================================================

    private static string GenerateTicketNumber()
    {
        var suffix =
            Guid.NewGuid()
                .ToString("N")[..8]
                .ToUpperInvariant();

        return
            $"CC-{DateTime.UtcNow:yyyy}-{suffix}";
    }

    // =========================================================
    // Helpers
    // =========================================================

    private static string? Clean(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static string? NormalizePhone(
        string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        var digits =
            new string(
                phone
                    .Where(char.IsDigit)
                    .ToArray());

        if (digits.StartsWith("20") &&
            digits.Length == 12)
        {
            digits =
                "0" + digits[2..];
        }

        return digits;
    }
}