using MasterDataAutomation.Application.Modules.CustomerModification.Dtos;
using MasterDataAutomation.Application.Modules.CustomerModification.Enums;
using MasterDataAutomation.Application.Modules.CustomerModification.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.Customers;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerModification.Persistence
{
    public class CustomerModificationRequestRepository : ICustomerModificationRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerModificationRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CustomerModificationRequestDto> GetDrafts()
        {
            return _context.CustomerModificationRequests
                .AsNoTracking()
                .Where(x => x.Status == CustomerModificationStatus.Draft)
                .OrderByDescending(x => x.CreatedAt)
                .Take(50)
                .Select(x => new CustomerModificationRequestDto
                {
                    Id = x.Id,
                    BranchId = x.BranchId,
                    BranchName = x.BranchName,
                    MarketCode = x.MarketCode,
                    MarketName = x.MarketName,
                    CurrentCustomerType = x.CurrentCustomerType,
                    ModificationType = x.ModificationType,
                    NewMarketName = x.NewMarketName,
                    Notes = x.Notes,
                    Status = x.Status,
                    RejectionReason = x.RejectionReason,
                    CreatedAt = x.CreatedAt,
                    SubmittedAt = x.SubmittedAt,
                    ReviewedAt = x.ReviewedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();
        }

        public List<CustomerModificationRequestDto> GetSubmitted()
        {
            return _context.CustomerModificationRequests
                .AsNoTracking()
                .Where(x => x.Status == CustomerModificationStatus.Submitted)
                .OrderByDescending(x => x.SubmittedAt)
                .Take(50)
                .Select(x => new CustomerModificationRequestDto
                {
                    Id = x.Id,
                    BranchId = x.BranchId,
                    BranchName = x.BranchName,
                    MarketCode = x.MarketCode,
                    MarketName = x.MarketName,
                    CurrentCustomerType = x.CurrentCustomerType,
                    ModificationType = x.ModificationType,
                    NewMarketName = x.NewMarketName,
                    Notes = x.Notes,
                    Status = x.Status,
                    RejectionReason = x.RejectionReason,
                    CreatedAt = x.CreatedAt,
                    SubmittedAt = x.SubmittedAt,
                    ReviewedAt = x.ReviewedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();
        }

        public List<CustomerModificationRequestDto> GetRejected()
        {
            return _context.CustomerModificationRequests
                .AsNoTracking()
                .Where(x => x.Status == CustomerModificationStatus.Rejected)
                .OrderByDescending(x => x.ReviewedAt)
                .Take(50)
                .Select(x => new CustomerModificationRequestDto
                {
                    Id = x.Id,
                    BranchId = x.BranchId,
                    BranchName = x.BranchName,
                    MarketCode = x.MarketCode,
                    MarketName = x.MarketName,
                    CurrentCustomerType = x.CurrentCustomerType,
                    ModificationType = x.ModificationType,
                    NewMarketName = x.NewMarketName,
                    Notes = x.Notes,
                    Status = x.Status,
                    RejectionReason = x.RejectionReason,
                    CreatedAt = x.CreatedAt,
                    SubmittedAt = x.SubmittedAt,
                    ReviewedAt = x.ReviewedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();
        }
        public CustomerModificationRequestDto? GetById(int id)
        {
            return _context.CustomerModificationRequests
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CustomerModificationRequestDto
                {
                    Id = x.Id,
                    BranchId = x.BranchId,
                    BranchName = x.BranchName,
                    MarketCode = x.MarketCode,
                    MarketName = x.MarketName,
                    CurrentCustomerType = x.CurrentCustomerType,
                    ModificationType = x.ModificationType,
                    NewMarketName = x.NewMarketName,
                    Notes = x.Notes,
                    Status = x.Status,
                    RejectionReason = x.RejectionReason,
                    CreatedAt = x.CreatedAt,
                    SubmittedAt = x.SubmittedAt,
                    ReviewedAt = x.ReviewedAt,
                    CreatedBy = x.CreatedBy
                })
                .FirstOrDefault();
        }

        public void Create(CreateCustomerModificationRequestDto dto, string? createdBy)
        {
            if (!dto.BranchId.HasValue)
                return;

            if (!dto.CurrentCustomerType.HasValue)
                return;

            if (!dto.ModificationType.HasValue)
                return;

            var entity = new CustomerModificationRequestEntity
            {
                BranchId = dto.BranchId.Value,
                BranchName = dto.BranchName.Trim(),

                MarketCode = dto.MarketCode.Trim(),
                MarketName = dto.MarketName.Trim(),

                CurrentCustomerType = dto.CurrentCustomerType.Value,
                ModificationType = dto.ModificationType.Value,

                NewMarketName = dto.ModificationType.Value == CustomerModificationType.ChangeName
                    ? dto.NewMarketName?.Trim()
                    : null,

                Notes = dto.Notes?.Trim(),

                Status = CustomerModificationStatus.Draft,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            _context.CustomerModificationRequests.Add(entity);
            _context.SaveChanges();
        }

        public void SubmitDraft(int id)
        {
            var request = _context.CustomerModificationRequests
                .FirstOrDefault(x => x.Id == id &&
                                     x.Status == CustomerModificationStatus.Draft);

            if (request == null)
                return;

            request.Status = CustomerModificationStatus.Submitted;
            request.SubmittedAt = DateTime.Now;
            request.RejectionReason = null;

            _context.SaveChanges();
        }

        public bool Approve(int id)
        {
            var request = _context.CustomerModificationRequests
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.Status == CustomerModificationStatus.Submitted);

            if (request == null)
                return false;

            request.Status = CustomerModificationStatus.Approved;
            request.ReviewedAt = DateTime.Now;
            request.RejectionReason = null;

            _context.SaveChanges();

            return true;
        }

        public List<CustomerModificationRequestDto> GetApproved()
        {
            return _context.CustomerModificationRequests
                .AsNoTracking()
                .Where(x => x.Status == CustomerModificationStatus.Approved)
                .OrderByDescending(x => x.ReviewedAt)
                .Take(50)
                .Select(x => new CustomerModificationRequestDto
                {
                    Id = x.Id,
                    BranchId = x.BranchId,
                    BranchName = x.BranchName,
                    MarketCode = x.MarketCode,
                    MarketName = x.MarketName,
                    CurrentCustomerType = x.CurrentCustomerType,
                    ModificationType = x.ModificationType,
                    NewMarketName = x.NewMarketName,
                    Notes = x.Notes,
                    Status = x.Status,
                    RejectionReason = x.RejectionReason,
                    CreatedAt = x.CreatedAt,
                    SubmittedAt = x.SubmittedAt,
                    ReviewedAt = x.ReviewedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();
        }
        public bool Reject(int id, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
                return false;

            var request = _context.CustomerModificationRequests
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.Status == CustomerModificationStatus.Submitted);

            if (request == null)
                return false;

            request.Status = CustomerModificationStatus.Rejected;
            request.ReviewedAt = DateTime.Now;
            request.RejectionReason = rejectionReason.Trim();

            _context.SaveChanges();

            return true;
        }
        public bool UpdateRejectedAndResubmit(int id, CreateCustomerModificationRequestDto dto)
        {
            if (!dto.BranchId.HasValue)
                return false;

            if (!dto.CurrentCustomerType.HasValue)
                return false;

            if (!dto.ModificationType.HasValue)
                return false;

            var request = _context.CustomerModificationRequests
                .FirstOrDefault(x => x.Id == id &&
                                     x.Status == CustomerModificationStatus.Rejected);

            if (request == null)
                return false;

            request.BranchId = dto.BranchId.Value;
            request.BranchName = dto.BranchName.Trim();

            request.MarketCode = dto.MarketCode.Trim();
            request.MarketName = dto.MarketName.Trim();

            request.CurrentCustomerType = dto.CurrentCustomerType.Value;
            request.ModificationType = dto.ModificationType.Value;

            request.NewMarketName = dto.ModificationType.Value == CustomerModificationType.ChangeName
                ? dto.NewMarketName?.Trim()
                : null;

            request.Notes = dto.Notes?.Trim();

            request.Status = CustomerModificationStatus.Submitted;
            request.SubmittedAt = DateTime.Now;
            request.ReviewedAt = null;
            request.RejectionReason = null;

            _context.SaveChanges();

            return true;
        }

        public void DeleteApprovedByIds(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return;

            var requests = _context.CustomerModificationRequests
                .Where(x => ids.Contains(x.Id) &&
                            x.Status == CustomerModificationStatus.Approved)
                .ToList();

            if (!requests.Any())
                return;

            _context.CustomerModificationRequests.RemoveRange(requests);
            _context.SaveChanges();
        }
    }
}