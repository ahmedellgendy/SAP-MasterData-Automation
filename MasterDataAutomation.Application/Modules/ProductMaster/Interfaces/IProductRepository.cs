using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

namespace MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;

public interface IProductRepository
{
    List<ProductDto> GetAll();
    List<ProductDto> Search(string? searchTerm);

    ProductDto? GetById(int id);
    ProductDto? GetByItemCode(string itemCode);
    ProductDto? GetByItemName(string itemName);

    bool ExistsByItemName(string itemName, int? excludeId = null);

    void Add(ProductDto product);
    void Update(ProductDto product);
    void Delete(int id);

}