using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Repositories;
using System.Text.Json;

namespace MasterDataAutomation.Infrastructure.Persistence;

public class JsonCustomerDraftRepository : ICustomerDraftRepository
{

    private readonly string _filePath;

    public JsonCustomerDraftRepository()
    {
        var dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");

        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        _filePath = Path.Combine(dataFolder, "DraftCustomers.json");
    }
    public List<CustomerImportDto> GetAll()
    {
        if (!File.Exists(_filePath))
            return new List<CustomerImportDto>();

        var json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<CustomerImportDto>();

        return JsonSerializer.Deserialize<List<CustomerImportDto>>(json)
               ?? new List<CustomerImportDto>();
    }

    public void Save(List<CustomerImportDto> customers)
    {
        var json = JsonSerializer.Serialize(
            customers,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, json);
    }

    public void Clear()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }

    public void RemoveAt(int index)
    {
        var customers = GetAll();

        if (index < 0 || index >= customers.Count)
            return;

        customers.RemoveAt(index);

        Save(customers);
    }

    public CustomerImportDto? GetByIndex(int index)
    {
        var customers = GetAll();

        if (index < 0 || index >= customers.Count)
            return null;

        return customers[index];
    }

    public void Update(int index, CustomerImportDto customer)
    {
        var customers = GetAll();

        if (index < 0 || index >= customers.Count)
            return;

        customers[index] = customer;

        Save(customers);
    }

    public void SubmitDraft()
    {
    }

    public int GetSubmittedCount()
    {
        return 0;
    }

    public List<CustomerImportDto> GetSubmitted()
    {
        return new List<CustomerImportDto>();
    }

    public void ApproveSubmitted()
    {
    }

    public void RejectSubmitted()
    {
    }

    public List<CustomerImportDto> GetApproved()
    {
        throw new NotImplementedException();
    }

    public void ClearApproved()
    {
    }
    public int GetApprovedCount()
    {
        return 0;
    }

    public int GetRejectedCount()
    {
        return 0;
    }

    public List<CustomerImportDto> GetRejected()
    {
        return new List<CustomerImportDto>();
    }

    public List<BranchStatusSummaryDto> GetBranchStatusSummary()
    {
        return new List<BranchStatusSummaryDto>();
    }
    public Dictionary<string, int> GetBranchSummary()
    {
        return new Dictionary<string, int>();
    }

    public void MoveRejectedToDraft()
    {
    }

    public List<CustomerReviewDto> GetSubmittedForReview()
    {
        return new List<CustomerReviewDto>();
    }

    public void ApproveById(int id)
    {
    }

    public void RejectById(int id, string rejectionReason)
    {
    }

    public void DeleteApprovedById(int id)
    {
    }

    public List<CustomerReviewDto> GetApprovedForReview()
    {
        return new List<CustomerReviewDto>();
    }

    public List<CustomerReviewDto> GetRejectedForReview()
    {
        return new List<CustomerReviewDto>();
    }


}   