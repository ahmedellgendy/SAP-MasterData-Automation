using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

namespace MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;

public interface IProductRepository
{
    List<ProductDto> GetAll();
    List<ProductDto> Search(string? searchTerm);

    ProductDto? GetById(int id);
    ProductDto? GetByItemCode(string itemCode);

    void Add(ProductDto product);
    void Update(ProductDto product);
    void Delete(int id);

}