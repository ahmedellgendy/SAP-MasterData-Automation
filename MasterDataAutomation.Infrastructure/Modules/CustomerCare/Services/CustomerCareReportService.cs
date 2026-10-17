
using MasterDataAutomation.Application.Modules.CustomerCare.Dtos.Reports;
using MasterDataAutomation.Application.Modules.CustomerCare.Interfaces;
using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;
using MasterDataAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Services;

public sealed class CustomerCareReportService : ICustomerCareReportService
{
    private readonly ApplicationDbContext _context;

    public CustomerCareReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerCareReportDto> GetReportAsync(
        CustomerCareReportFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        if (filter.FromDate.HasValue &&
            filter.ToDate.HasValue &&
            filter.FromDate.Value.Date > filter.ToDate.Value.Date)
        {
            throw new ArgumentException(
                "FromDate cannot be greater than ToDate.");
        }

        var now = DateTime.UtcNow;

        // All reporting dates are interpreted as UTC.
        var query = _context.CustomerCareTickets
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        // Date range: inclusive calendar dates
        if (filter.FromDate.HasValue)
        {
            var from = filter.FromDate.Value.Date;
            query = query.Where(x => x.CreatedAt >= from);
        }

        if (filter.ToDate.HasValue)
        {
            var exclusiveTo = filter.ToDate.Value.Date.AddDays(1);
            query = query.Where(x => x.CreatedAt < exclusiveTo);
        }

        

        if (filter.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == filter.CategoryId.Value);

        if (filter.SubCategoryId.HasValue)
            query = query.Where(x => x.SubCategoryId == filter.SubCategoryId.Value);

        if (filter.DepartmentId.HasValue)
            query = query.Where(
                x => x.AssignedDepartmentId == filter.DepartmentId.Value);

        if (filter.SalesRepId.HasValue)
            query = query.Where(x => x.SalesRepId == filter.SalesRepId.Value);

        if (filter.CreatedByUserId.HasValue)
            query = query.Where(
                x => x.CreatedByUserId == filter.CreatedByUserId.Value);

        if (filter.CriticalOnly)
            query = query.Where(x =>
                x.IsCritical ||
                x.Priority == CustomerCareTicketPriority.Critical);

        if (filter.OverdueOnly)
        {
            query = query.Where(x =>
                x.Status != CustomerCareTicketStatus.Resolved &&
                x.Status != CustomerCareTicketStatus.Closed &&
                x.Status != CustomerCareTicketStatus.Cancelled &&
                x.Status != CustomerCareTicketStatus.Rejected &&
                x.ResolutionDueAt.HasValue &&
                x.ResolutionDueAt.Value < now);
        }

        // Select only fields needed for reporting.
        var tickets = await query
            .Select(x => new ReportTicket
            {
                Id = x.Id,
                BranchId = x.BranchId,
                CategoryId = x.CategoryId,
                DepartmentId = x.AssignedDepartmentId,
                CreatedAt = x.CreatedAt,
                FirstResponseDueAt = x.FirstResponseDueAt,
                FirstRespondedAt = x.FirstRespondedAt,
                ResolutionDueAt = x.ResolutionDueAt,
                ResolvedAt = x.ResolvedAt,
                Status = x.Status,
                Priority = x.Priority,
                Source = x.Source,
                IsCritical = x.IsCritical,
                IsFirstResponseBreached = x.IsFirstResponseBreached,
                IsResolutionBreached = x.IsResolutionBreached,
                ReopenCount = x.ReopenCount
            })
            .ToListAsync(cancellationToken);

        // Category and department names
        var categoryNames = await _context.CustomerCareCategories
            .AsNoTracking()
            .Select(x => new { x.Id, x.Name })
            .ToDictionaryAsync(
                x => x.Id,
                x => x.Name,
                cancellationToken);

        var departmentNames = await _context.CustomerCareDepartments
            .AsNoTracking()
            .Select(x => new { x.Id, x.Name })
            .ToDictionaryAsync(
                x => x.Id,
                x => x.Name,
                cancellationToken);

        return new CustomerCareReportDto
        {
            Filter = filter,
            Summary = BuildSummary(tickets, now),

           
            ByCategory = BuildBreakdown(
                tickets,
                x => x.CategoryId,
                id => ResolveName(categoryNames, id),
                now),

            ByDepartment = BuildBreakdown(
                tickets,
                x => x.DepartmentId,
                id => ResolveName(departmentNames, id),
                now),

            ByStatus = BuildBreakdown(
                tickets,
                x => (int?)x.Status,
                id => id.HasValue
                    ? ((CustomerCareTicketStatus)id.Value).ToString()
                    : "غير محدد",
                now),

            ByPriority = BuildBreakdown(
                tickets,
                x => (int?)x.Priority,
                id => id.HasValue
                    ? ((CustomerCareTicketPriority)id.Value).ToString()
                    : "غير محدد",
                now),

            BySource = BuildBreakdown(
                tickets,
                x => (int?)x.Source,
                id => id.HasValue
                    ? ((CustomerCareTicketSource)id.Value).ToString()
                    : "غير محدد",
                now)
        };
    }

    // =========================================================
    // Summary KPIs
    // =========================================================

    private static CustomerCareReportSummaryDto BuildSummary(
        List<ReportTicket> tickets,
        DateTime now)
    {
        var total = tickets.Count;

        var completed = tickets.Where(IsCompleted).ToList();

        var resolvedDurations = completed
            .Where(x =>
                x.ResolvedAt.HasValue &&
                x.ResolvedAt.Value >= x.CreatedAt)
            .Select(x =>
                (x.ResolvedAt!.Value - x.CreatedAt).TotalHours)
            .ToList();

        // SLA denominators contain only tickets that have
        // received a response or reached the response deadline.
        var firstResponseEligible = tickets
            .Where(x =>
                x.FirstResponseDueAt.HasValue &&
                (x.FirstRespondedAt.HasValue ||
                 x.FirstResponseDueAt.Value < now))
            .ToList();

        // Resolution SLA: completed tickets or passed deadlines.
        var resolutionEligible = tickets
            .Where(x =>
                x.ResolutionDueAt.HasValue &&
                (IsCompleted(x) ||
                 x.ResolutionDueAt.Value < now))
            .ToList();

        var firstResponseBreaches = firstResponseEligible
            .Count(x => IsFirstResponseBreached(x, now));

        var resolutionBreaches = resolutionEligible
            .Count(x => IsResolutionBreached(x, now));

        return new CustomerCareReportSummaryDto
        {
            TotalTickets = total,

            NewTickets = tickets.Count(x =>
                x.Status == CustomerCareTicketStatus.New),

            AssignedTickets = tickets.Count(x =>
                x.Status == CustomerCareTicketStatus.Assigned),

            InProgressTickets = tickets.Count(x =>
                x.Status == CustomerCareTicketStatus.InProgress),

            ResolvedTickets = tickets.Count(x =>
                x.Status == CustomerCareTicketStatus.Resolved),

            ClosedTickets = tickets.Count(x =>
                x.Status == CustomerCareTicketStatus.Closed),

            OpenTickets = tickets.Count(IsOpen),

            CompletedTickets = completed.Count,

            OverdueTickets = tickets.Count(x =>
                IsCurrentlyOverdue(x, now)),

            CriticalTickets = tickets.Count(x =>
                x.IsCritical ||
                x.Priority == CustomerCareTicketPriority.Critical),

            ReopenedTickets = tickets.Count(x =>
                x.ReopenCount > 0 ||
                x.Status == CustomerCareTicketStatus.Reopened),

            ResolutionRate = Percentage(completed.Count, total),

            ClosureRate = Percentage(
                tickets.Count(x =>
                    x.Status == CustomerCareTicketStatus.Closed),
                total),

            AverageResolutionHours = resolvedDurations.Count > 0
                ? Math.Round(resolvedDurations.Average(), 2)
                : 0,

            FirstResponseBreachedTickets = tickets.Count(x =>
                IsFirstResponseBreached(x, now)),

            ResolutionBreachedTickets = tickets.Count(x =>
                IsResolutionBreached(x, now)),

            FirstResponseComplianceRate =
                firstResponseEligible.Count == 0
                    ? 0
                    : Percentage(
                        firstResponseEligible.Count - firstResponseBreaches,
                        firstResponseEligible.Count),

            ResolutionComplianceRate =
                resolutionEligible.Count == 0
                    ? 0
                    : Percentage(
                        resolutionEligible.Count - resolutionBreaches,
                        resolutionEligible.Count)
        };
    }

    // =========================================================
    // Breakdowns
    // =========================================================

    private static List<CustomerCareReportBreakdownDto> BuildBreakdown(
        List<ReportTicket> tickets,
        Func<ReportTicket, int?> groupSelector,
        Func<int?, string> nameResolver,
        DateTime now)
    {
        return tickets
            .GroupBy(groupSelector)
            .Select(group =>
            {
                var groupTickets = group.ToList();
                var total = groupTickets.Count;
                var completed = groupTickets.Count(IsCompleted);

                return new CustomerCareReportBreakdownDto
                {
                    Id = group.Key,
                    Name = nameResolver(group.Key),
                    TotalTickets = total,
                    OpenTickets = groupTickets.Count(IsOpen),
                    CompletedTickets = completed,
                    OverdueTickets = groupTickets.Count(x =>
                        IsCurrentlyOverdue(x, now)),
                    ResolutionRate = Percentage(completed, total)
                };
            })
            .OrderByDescending(x => x.TotalTickets)
            .ThenBy(x => x.Name)
            .ToList();
    }

    // =========================================================
    // Status Rules
    // =========================================================

    private static bool IsCompleted(ReportTicket x)
    {
        return x.Status is
            CustomerCareTicketStatus.Resolved or
            CustomerCareTicketStatus.Closed;
    }

    private static bool IsOpen(ReportTicket x)
    {
        return x.Status is
            CustomerCareTicketStatus.New or
            CustomerCareTicketStatus.Assigned or
            CustomerCareTicketStatus.InProgress or
            CustomerCareTicketStatus.WaitingCustomer or
            CustomerCareTicketStatus.WaitingDepartment or
            CustomerCareTicketStatus.WaitingBranch or
            CustomerCareTicketStatus.Reopened;
    }

    private static bool IsCurrentlyOverdue(
        ReportTicket x,
        DateTime now)
    {
        return IsOpen(x) &&
               x.ResolutionDueAt.HasValue &&
               x.ResolutionDueAt.Value < now;
    }

    // =========================================================
    // SLA Rules
    // =========================================================

    private static bool IsFirstResponseBreached(
        ReportTicket x,
        DateTime now)
    {
        if (x.IsFirstResponseBreached)
            return true;

        if (!x.FirstResponseDueAt.HasValue)
            return false;

        if (x.FirstRespondedAt.HasValue)
            return x.FirstRespondedAt.Value >
                   x.FirstResponseDueAt.Value;

        return x.FirstResponseDueAt.Value < now;
    }

    private static bool IsResolutionBreached(
        ReportTicket x,
        DateTime now)
    {
        if (x.IsResolutionBreached)
            return true;

        if (!x.ResolutionDueAt.HasValue)
            return false;

        if (IsCompleted(x))
        {
            return x.ResolvedAt.HasValue &&
                   x.ResolvedAt.Value > x.ResolutionDueAt.Value;
        }

        if (!IsOpen(x))
            return false;

        return x.ResolutionDueAt.Value < now;
    }

    // =========================================================
    // Helpers
    // =========================================================

    private static decimal Percentage(int value, int total)
    {
        if (total == 0)
            return 0;

        return Math.Round(
            (decimal)value * 100m / total,
            2);
    }

    private static string ResolveName(
        Dictionary<int, string> names,
        int? id)
    {
        if (!id.HasValue)
            return "غير محدد";

        return names.TryGetValue(id.Value, out var name)
            ? name
            : $"غير معروف ({id.Value})";
    }

    // Lightweight database projection
    private sealed class ReportTicket
    {
        public int Id { get; set; }
        public int? BranchId { get; set; }
        public int? CategoryId { get; set; }
        public int? DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? FirstResponseDueAt { get; set; }
        public DateTime? FirstRespondedAt { get; set; }
        public DateTime? ResolutionDueAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public CustomerCareTicketStatus Status { get; set; }
        public CustomerCareTicketPriority Priority { get; set; }
        public CustomerCareTicketSource Source { get; set; }

        public bool IsCritical { get; set; }
        public bool IsFirstResponseBreached { get; set; }
        public bool IsResolutionBreached { get; set; }

        public int ReopenCount { get; set; }
    }
}
