using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Persistence;

public class SqlCustomerDraftRepository : ICustomerDraftRepository
{
    private readonly ApplicationDbContext _context;

    public SqlCustomerDraftRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<CustomerImportDto> GetAll()
    {
        return _context.DraftCustomers
            .Where(x => x.Status == "Draft")
            .OrderBy(x => x.Id)
            .Select(x => new CustomerImportDto
            {
                Line = x.Line,
                Market = x.Market,
                Branch = x.Branch,
                SalesDistrict = x.SalesDistrict,
                CustomerType = x.CustomerType
            })
            .ToList();
    }

    public void Save(List<CustomerImportDto> customers)
    {
        _context.DraftCustomers.RemoveRange(
            _context.DraftCustomers.Where(x => x.Status == "Draft"));

        foreach (var customer in customers)
        {
            _context.DraftCustomers.Add(new DraftCustomerEntity
            {
                Line = customer.Line,
                Market = customer.Market,
                Branch = customer.Branch,
                SalesDistrict = customer.SalesDistrict,
                CustomerType = customer.CustomerType,
                Status = "Draft",
                CreatedBy = customer.CreatedBy,
                CreatedAt = DateTime.Now
            });
        }

        _context.SaveChanges();
    }

    public void Clear()
    {
        var customers = _context.DraftCustomers
            .Where(x => x.Status == "Draft")
            .ToList();

        _context.DraftCustomers.RemoveRange(customers);

        _context.SaveChanges();
    }

    public void RemoveAt(int index)
    {
        var customer = _context.DraftCustomers
            .Where(x => x.Status == "Draft")
            .OrderBy(x => x.Id)
            .Skip(index)
            .FirstOrDefault();

        if (customer == null)
            return;

        _context.DraftCustomers.Remove(customer);

        _context.SaveChanges();
    }

    public CustomerImportDto? GetByIndex(int index)
    {
        var customer = _context.DraftCustomers
            .Where(x => x.Status == "Draft")
            .OrderBy(x => x.Id)
            .Skip(index)
            .FirstOrDefault();

        if (customer == null)
            return null;

        return new CustomerImportDto
        {
            Line = customer.Line,
            Market = customer.Market,
            Branch = customer.Branch,
            SalesDistrict = customer.SalesDistrict,
            CustomerType = customer.CustomerType
        };
    }

    public void Update(int index, CustomerImportDto customer)
    {
        var entity = _context.DraftCustomers
            .Where(x => x.Status == "Draft")
            .OrderBy(x => x.Id)
            .Skip(index)
            .FirstOrDefault();

        if (entity == null)
            return;

        entity.Line = customer.Line;
        entity.Market = customer.Market;
        entity.Branch = customer.Branch;
        entity.SalesDistrict = customer.SalesDistrict;
        entity.CustomerType = customer.CustomerType;

        _context.SaveChanges();
    }

    public void SubmitDraft()
    {
        var customers = _context.DraftCustomers
            .Where(x => x.Status == "Draft")
            .ToList();

        foreach (var customer in customers)
        {
            customer.Status = "Submitted";
        }

        _context.SaveChanges();
    }

    public int GetSubmittedCount()
    {
        return _context.DraftCustomers
            .Count(x => x.Status == "Submitted");
    }

    public List<CustomerImportDto> GetSubmitted()
    {
        return _context.DraftCustomers
            .Where(x => x.Status == "Submitted")
            .OrderBy(x => x.Id)
            .Select(x => new CustomerImportDto
            {
                Line = x.Line,
                Market = x.Market,
                Branch = x.Branch,
                SalesDistrict = x.SalesDistrict,
                CustomerType = x.CustomerType
            })
            .ToList();
    }

    public void ApproveSubmitted()
    {
        var customers = _context.DraftCustomers
            .Where(x => x.Status == "Submitted")
            .ToList();

        foreach (var customer in customers)
        {
            customer.Status = "Approved";
            customer.RejectionReason = null;
        }

        _context.SaveChanges();
    }

    public void RejectSubmitted()
    {
        var customers = _context.DraftCustomers
            .Where(x => x.Status == "Submitted")
            .ToList();

        foreach (var customer in customers)
        {
            customer.Status = "Rejected";
        }

        _context.SaveChanges();
    }

    public List<CustomerImportDto> GetApproved()
    {
        return _context.DraftCustomers
            .Where(x => x.Status == "Approved")
            .OrderBy(x => x.Id)
            .Select(x => new CustomerImportDto
            {
                Line = x.Line,
                Market = x.Market,
                Branch = x.Branch,
                SalesDistrict = x.SalesDistrict,
                CustomerType = x.CustomerType
            })
            .ToList();
    }

    public void ClearApproved()
    {
        var customers = _context.DraftCustomers
            .Where(x => x.Status == "Approved")
            .ToList();

        _context.DraftCustomers.RemoveRange(customers);

        _context.SaveChanges();
    }

    public int GetApprovedCount()
    {
        return _context.DraftCustomers
            .Count(x => x.Status == "Approved");
    }

    public int GetRejectedCount()
    {
        return _context.DraftCustomers
            .Count(x => x.Status == "Rejected");
    }

    public List<CustomerImportDto> GetRejected()
    {
        return _context.DraftCustomers
            .Where(x => x.Status == "Rejected")
            .OrderBy(x => x.Id)
            .Select(x => new CustomerImportDto
            {
                Line = x.Line,
                Market = x.Market,
                Branch = x.Branch,
                SalesDistrict = x.SalesDistrict,
                CustomerType = x.CustomerType
            })
            .ToList();
    }

    public List<BranchStatusSummaryDto> GetBranchStatusSummary()
    {
        return _context.DraftCustomers
            .GroupBy(x => x.Branch)
            .Select(g => new BranchStatusSummaryDto
            {
                Branch = g.Key,
                DraftCount = g.Count(x => x.Status == "Draft"),
                SubmittedCount = g.Count(x => x.Status == "Submitted"),
                ApprovedCount = g.Count(x => x.Status == "Approved"),
                RejectedCount = g.Count(x => x.Status == "Rejected"),
                TotalCount = g.Count()
            })
            .OrderBy(x => x.Branch)
            .ToList();
    }
    public Dictionary<string, int> GetBranchSummary()
    {
        return _context.DraftCustomers
            .GroupBy(x => x.Branch)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public void MoveRejectedToDraft()
    {
        var customers = _context.DraftCustomers
            .Where(x => x.Status == "Rejected")
            .ToList();

        foreach (var customer in customers)
        {
            customer.Status = "Draft";
            customer.RejectionReason = null;

        }

        _context.SaveChanges();
    }


    public List<CustomerReviewDto> GetSubmittedForReview()
    {
        return _context.DraftCustomers
            .Where(x => x.Status == "Submitted")
            .OrderBy(x => x.Id)
            .Select(x => new CustomerReviewDto
            {
                Id = x.Id,
                Line = x.Line,
                Market = x.Market,
                Branch = x.Branch,
                SalesDistrict = x.SalesDistrict,
                CustomerType = x.CustomerType,
                Status = x.Status,
                CreatedBy = x.CreatedBy
            })
            .ToList();
    }

   

    public void ApproveById(int id)
    {
        var customer = _context.DraftCustomers
            .FirstOrDefault(x => x.Id == id && x.Status == "Submitted");

        if (customer == null)
            return;

        customer.Status = "Approved";
        customer.RejectionReason = null;

        _context.SaveChanges();
    }

    public void RejectById(int id, string rejectionReason)
    {
        var customer = _context.DraftCustomers
            .FirstOrDefault(x => x.Id == id && x.Status == "Submitted");

        if (customer == null)
            return;

        customer.Status = "Rejected";
        customer.RejectionReason = rejectionReason;

        _context.SaveChanges();
    }

    public void DeleteApprovedById(int id)
    {
        var customer = _context.DraftCustomers
            .FirstOrDefault(x => x.Id == id && x.Status == "Approved");

        if (customer == null)
            return;

        _context.DraftCustomers.Remove(customer);

        _context.SaveChanges();
    }

    public List<CustomerReviewDto> GetRejectedForReview()
    {
        return _context.DraftCustomers
            .Where(x => x.Status == "Rejected")
            .OrderBy(x => x.Id)
            .Select(x => new CustomerReviewDto
            {
                Id = x.Id,
                Line = x.Line,
                Market = x.Market,
                Branch = x.Branch,
                SalesDistrict = x.SalesDistrict,
                CustomerType = x.CustomerType,
                Status = x.Status,
                RejectionReason = x.RejectionReason
            })
            .ToList();
    }

    public List<CustomerReviewDto> GetApprovedForReview()
    {
        return _context.DraftCustomers
            .Where(x => x.Status == "Approved")
            .OrderBy(x => x.Id)
            .Select(x => new CustomerReviewDto
            {
                Id = x.Id,
                Line = x.Line,
                Market = x.Market,
                Branch = x.Branch,
                SalesDistrict = x.SalesDistrict,
                CustomerType = x.CustomerType,
                Status = x.Status,
                RejectionReason = x.RejectionReason
            })
            .ToList();
    }
}