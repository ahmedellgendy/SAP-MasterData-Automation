using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

namespace MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;

public interface IProductRequestRepository
{
    List<ProductRequestDto> GetDrafts();
    List<ProductRequestDto> GetSubmittedToAccountsManager();
    List<ProductRequestDto> GetSubmittedToExecutiveManager();

    ProductRequestDto? GetById(int id);

    void Create(CreateProductRequestDto dto);
    void SubmitDraft(int id);

    void UpdateAccountsData(ProductRequestDto dto);
    void ApproveByAccountsManager(int id);
    void RejectByAccountsManager(int id, string rejectionReason);

    bool FinalApproveByExecutiveManager(int id, decimal OutletSellingPrice, decimal retailSellingPrice);
    void RejectByExecutiveManager(int id, string rejectionReason);
}