using MasterDataAutomation.Application.Modules.CustomerModification.Dtos;

namespace MasterDataAutomation.Application.Modules.CustomerModification.Interfaces
{
    public interface ICustomerModificationRequestRepository
    {
        List<CustomerModificationRequestDto> GetDrafts();
        List<CustomerModificationRequestDto> GetSubmitted();

        CustomerModificationRequestDto? GetById(int id);

        void Create(CreateCustomerModificationRequestDto dto, string? createdBy);
        void SubmitDraft(int id);
    }
}
