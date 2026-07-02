using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Interfaces.Repositories;

public interface ICustomerDraftRepository
{
    List<CustomerImportDto> GetAll();

    void Save(List<CustomerImportDto> customers);

    void Clear();
    void RemoveAt(int index);

    CustomerImportDto? GetByIndex(int index);

    void Update(int index, CustomerImportDto customer);

    void SubmitDraft();
    int GetSubmittedCount();

    List<CustomerImportDto> GetSubmitted();
    void ApproveSubmitted();
    void RejectSubmitted();

    List<CustomerImportDto> GetApproved();
    void ClearApproved();

    int GetApprovedCount();
    int GetRejectedCount();

    List<CustomerImportDto> GetRejected();

    Dictionary<string, int> GetBranchSummary();

    List<BranchStatusSummaryDto> GetBranchStatusSummary();

    void MoveRejectedToDraft();

    List<CustomerReviewDto> GetSubmittedForReview();
    void ApproveById(int id);

    void RejectById(int id, string rejectionReason);
    void DeleteApprovedById(int id);
    List<CustomerReviewDto> GetApprovedForReview();
    List<CustomerReviewDto> GetRejectedForReview();
}