using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.Products;
using Microsoft.EntityFrameworkCore;

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
            .AsNoTracking()
            .OrderBy(x => x.ItemCode)
            .Take(1000)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                OutletSellingPrice = x.OutletSellingPrice,
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

    public List<ProductDto> Search(string? searchTerm)
    {
        var query = _context.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            query = query.Where(x =>
                x.ItemCode.Contains(searchTerm) ||
                x.ItemName.Contains(searchTerm));
        }

        return query
            .OrderBy(x => x.ItemCode)
            .Take(500)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                OutletSellingPrice = x.OutletSellingPrice,
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
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                OutletSellingPrice = x.OutletSellingPrice,
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
        itemCode = itemCode.Trim();

        return _context.Products
            .AsNoTracking()
            .Where(x => x.ItemCode == itemCode)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                OutletSellingPrice = x.OutletSellingPrice,
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

    public void Add(ProductDto product)
    {
        var entity = new ProductEntity
        {
            ItemCode = product.ItemCode.Trim(),
            ItemName = product.ItemName.Trim(),

            OutletSellingPrice = product.OutletSellingPrice,
            RetailSellingPrice = product.RetailSellingPrice,

            Currency = string.IsNullOrWhiteSpace(product.Currency)
                ? "EGP"
                : product.Currency.Trim(),

            MarketValidFrom = product.MarketValidFrom,
            MarketValidTo = product.MarketValidTo,

            RetailValidFrom = product.RetailValidFrom,
            RetailValidTo = product.RetailValidTo,

            IsActive = product.IsActive,
            CreatedAt = DateTime.Now
        };

        _context.Products.Add(entity);
        _context.SaveChanges();
    }

    public void Update(ProductDto product)
    {
        var entity = _context.Products.FirstOrDefault(x => x.Id == product.Id);

        if (entity == null)
            return;

        entity.ItemName = product.ItemName.Trim();

        entity.OutletSellingPrice = product.OutletSellingPrice;
        entity.RetailSellingPrice = product.RetailSellingPrice;

        entity.Currency = string.IsNullOrWhiteSpace(product.Currency)
            ? "EGP"
            : product.Currency.Trim();

        entity.MarketValidFrom = product.MarketValidFrom;
        entity.MarketValidTo = product.MarketValidTo;

        entity.RetailValidFrom = product.RetailValidFrom;
        entity.RetailValidTo = product.RetailValidTo;

        entity.IsActive = product.IsActive;
        entity.UpdatedAt = DateTime.Now;

        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(x => x.Id == id);

        if (product == null)
            return;

        _context.Products.Remove(product);

        _context.SaveChanges();
    }
}