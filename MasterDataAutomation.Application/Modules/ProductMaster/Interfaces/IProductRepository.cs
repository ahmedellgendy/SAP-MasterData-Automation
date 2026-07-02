using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

namespace MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;

public interface IProductRepository
{
    List<ProductDto> GetAll();
    ProductDto? GetById(int id);
    ProductDto? GetByItemCode(string itemCode);
}