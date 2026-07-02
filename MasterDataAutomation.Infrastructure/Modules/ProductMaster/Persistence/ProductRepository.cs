using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Infrastructure.Data;

namespace MasterDataAutomation.Infrastructure.Modules.ProductMaster.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<ProductDto> GetAll()
    {
        return _context.Products
            .OrderBy(x => x.ItemCode)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                MarketSellingPrice = x.MarketSellingPrice,
                RetailSellingPrice = x.RetailSellingPrice,
                Currency = x.Currency,
                MarketValidFrom = x.MarketValidFrom,
                MarketValidTo = x.MarketValidTo,
                RetailValidFrom = x.RetailValidFrom,
                RetailValidTo = x.RetailValidTo,
                IsActive = x.IsActive
            })
            .ToList();
    }

    public ProductDto? GetById(int id)
    {
        return _context.Products
            .Where(x => x.Id == id)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                MarketSellingPrice = x.MarketSellingPrice,
                RetailSellingPrice = x.RetailSellingPrice,
                Currency = x.Currency,
                MarketValidFrom = x.MarketValidFrom,
                MarketValidTo = x.MarketValidTo,
                RetailValidFrom = x.RetailValidFrom,
                RetailValidTo = x.RetailValidTo,
                IsActive = x.IsActive
            })
            .FirstOrDefault();
    }

    public ProductDto? GetByItemCode(string itemCode)
    {
        return _context.Products
            .Where(x => x.ItemCode == itemCode)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                MarketSellingPrice = x.MarketSellingPrice,
                RetailSellingPrice = x.RetailSellingPrice,
                Currency = x.Currency,
                MarketValidFrom = x.MarketValidFrom,
                MarketValidTo = x.MarketValidTo,
                RetailValidFrom = x.RetailValidFrom,
                RetailValidTo = x.RetailValidTo,
                IsActive = x.IsActive
            })
            .FirstOrDefault();
    }
}