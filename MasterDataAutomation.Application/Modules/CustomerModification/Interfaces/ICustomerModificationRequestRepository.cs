using MasterDataAutomation.Application.Modules.CustomerModification.Dtos;

namespace MasterDataAutomation.Application.Modules.CustomerModification.Interfaces
{
    public interface ICustomerModificationRequestRepository
    {
        List<CustomerModificationRequestDto> GetDrafts();
        List<CustomerModificationRequestDto> GetSubmitted();
        List<CustomerModificationRequestDto> GetRejected();

        CustomerModificationRequestDto? GetById(int id);

        void Create(CreateCustomerModificationRequestDto dto, string? createdBy);
        void SubmitDraft(int id);

        bool Approve(int id);
        bool Reject(int id, string rejectionReason);
    }
}
