using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;
using MasterDataAutomation.Application.Modules.ProductMaster.Enums;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.Products;

namespace MasterDataAutomation.Infrastructure.Modules.ProductMaster.Persistence;

public class ProductRequestRepository : IProductRequestRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<ProductRequestDto> GetDrafts()
    {
        return _context.ProductRequests
            .Where(x => x.Status == ProductRequestStatus.Draft)
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => MapToDto(x))
            .ToList();
    }

    public List<ProductRequestDto> GetSubmittedToAccountsManager()
    {
        return _context.ProductRequests
            .Where(x => x.Status == ProductRequestStatus.SubmittedToAccountsManager)
            .OrderByDescending(x => x.SubmittedAt)
            .Take(5)
            .Select(x => MapToDto(x))
            .ToList();
    }

    public ProductRequestDto? GetById(int id)
    {
        return _context.ProductRequests
            .Where(x => x.Id == id)
            .Select(x => MapToDto(x))
            .FirstOrDefault();
    }

    public void Create(CreateProductRequestDto dto)
    {
        var entity = new ProductRequestEntity
        {
            ItemCode = dto.ItemCode.Trim(),
            ItemName = dto.ItemName.Trim(),
            Notes = dto.Notes?.Trim(),

            PurchasePrice = dto.PurchasePrice,
            PiecesCount = dto.PiecesCount,
            PiecePrice = Math.Round(dto.PurchasePrice / dto.PiecesCount, 2),

            HasBonus = dto.HasBonus,
            BonusQuantity = dto.HasBonus ? dto.BonusQuantity : null,

            Currency = string.IsNullOrWhiteSpace(dto.Currency)
                ? "EGP"
                : dto.Currency.Trim(),

            Status = ProductRequestStatus.Draft,
            CreatedAt = DateTime.Now
        };

        _context.ProductRequests.Add(entity);
        _context.SaveChanges();
    }

    public void SubmitDraft(int id)
    {
        var request = _context.ProductRequests
            .FirstOrDefault(x => x.Id == id && x.Status == ProductRequestStatus.Draft);

        if (request == null)
            return;

        request.Status = ProductRequestStatus.SubmittedToAccountsManager;
        request.SubmittedAt = DateTime.Now;
        request.RejectionReason = null;

        _context.SaveChanges();
    }

    private static ProductRequestDto MapToDto(ProductRequestEntity x)
    {
        return new ProductRequestDto
        {
            Id = x.Id,
            ItemCode = x.ItemCode,
            ItemName = x.ItemName,
            Notes = x.Notes,

            PurchasePrice = x.PurchasePrice,
            PiecesCount = x.PiecesCount,
            PiecePrice = x.PiecePrice,

            HasBonus = x.HasBonus,
            BonusQuantity = x.BonusQuantity,

            OutletSellingPrice = x.OutletSellingPrice,
            RetailSellingPrice = x.RetailSellingPrice,

            Currency = x.Currency,
            Status = x.Status,
            RejectionReason = x.RejectionReason,

            CreatedAt = x.CreatedAt,
            SubmittedAt = x.SubmittedAt
        };
    }

    public void UpdateAccountsData(ProductRequestDto dto)
    {
        var request = _context.ProductRequests
            .FirstOrDefault(x => x.Id == dto.Id &&
                                 x.Status == ProductRequestStatus.SubmittedToAccountsManager);

        if (request == null)
            return;

        request.ItemName = dto.ItemName.Trim();
        request.Notes = dto.Notes?.Trim();

        request.PurchasePrice = dto.PurchasePrice;
        request.PiecesCount = dto.PiecesCount;
        request.PiecePrice = Math.Round(dto.PurchasePrice / dto.PiecesCount, 2);

        request.HasBonus = dto.HasBonus;
        request.BonusQuantity = dto.HasBonus ? dto.BonusQuantity : null;

        request.Currency = string.IsNullOrWhiteSpace(dto.Currency)
            ? "EGP"
            : dto.Currency.Trim();

        _context.SaveChanges();
    }

    public void ApproveByAccountsManager(int id)
    {
        var request = _context.ProductRequests
            .FirstOrDefault(x => x.Id == id &&
                                 x.Status == ProductRequestStatus.SubmittedToAccountsManager);

        if (request == null)
            return;

        request.Status = ProductRequestStatus.SubmittedToExecutiveManager;
        request.AccountsManagerReviewedAt = DateTime.Now;
        request.RejectionReason = null;

        _context.SaveChanges();
    }

    public void RejectByAccountsManager(int id, string rejectionReason)
    {
        var request = _context.ProductRequests
            .FirstOrDefault(x => x.Id == id &&
                                 x.Status == ProductRequestStatus.SubmittedToAccountsManager);

        if (request == null)
            return;

        request.Status = ProductRequestStatus.RejectedByAccountsManager;
        request.AccountsManagerReviewedAt = DateTime.Now;
        request.RejectionReason = rejectionReason;

        _context.SaveChanges();
    }

    public List<ProductRequestDto> GetSubmittedToExecutiveManager()
    {
        return _context.ProductRequests
            .Where(x => x.Status == ProductRequestStatus.SubmittedToExecutiveManager)
            .OrderByDescending(x => x.AccountsManagerReviewedAt)
            .Select(x => MapToDto(x))
            .ToList();
    }

    public bool FinalApproveByExecutiveManager(int id, decimal OutletSellingPrice, decimal retailSellingPrice)
    {
        var request = _context.ProductRequests
            .FirstOrDefault(x => x.Id == id &&
                                 x.Status == ProductRequestStatus.SubmittedToExecutiveManager);

        if (request == null)
            return false;

        var productExists = _context.Products
            .Any(x => x.ItemCode == request.ItemCode);

        if (productExists)
            return false;

        request.OutletSellingPrice = OutletSellingPrice;
        request.RetailSellingPrice = retailSellingPrice;
        request.ExecutiveManagerReviewedAt = DateTime.Now;
        request.CreatedAsProductAt = DateTime.Now;
        request.Status = ProductRequestStatus.CreatedAsProduct;
        request.RejectionReason = null;

        var product = new ProductEntity
        {
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,

            OutletSellingPrice = OutletSellingPrice,
            RetailSellingPrice = retailSellingPrice,

            Currency = string.IsNullOrWhiteSpace(request.Currency)
                ? "EGP"
                : request.Currency,

            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        return true;
    }

    public void RejectByExecutiveManager(int id, string rejectionReason)
    {
        var request = _context.ProductRequests
            .FirstOrDefault(x => x.Id == id &&
                                 x.Status == ProductRequestStatus.SubmittedToExecutiveManager);

        if (request == null)
            return;

        request.Status = ProductRequestStatus.RejectedByExecutiveManager;
        request.ExecutiveManagerReviewedAt = DateTime.Now;
        request.RejectionReason = rejectionReason;

        _context.SaveChanges();
    }
}