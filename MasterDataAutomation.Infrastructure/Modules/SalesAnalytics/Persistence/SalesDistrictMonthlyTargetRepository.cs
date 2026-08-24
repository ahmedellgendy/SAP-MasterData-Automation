using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;

public class SalesDistrictMonthlyTargetRepository : ISalesDistrictMonthlyTargetRepository
{
    private readonly ApplicationDbContext _context;

    public SalesDistrictMonthlyTargetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<SalesDistrictMonthlyTargetDto> GetTargets(int year, int month)
    {
        return _context.SalesDistrictMonthlyTargets
            .AsNoTracking()
            .Where(x => x.Year == year && x.Month == month)
            .OrderBy(x => x.BranchName)
            .ThenBy(x => x.SalesDistrictName)
            .Select(x => new SalesDistrictMonthlyTargetDto
            {
                Id = x.Id,
                Year = x.Year,
                Month = x.Month,
                BranchCode = x.BranchCode,
                BranchName = x.BranchName,
                SalesDistrictCode = x.SalesDistrictCode,
                SalesDistrictName = x.SalesDistrictName,
                MonthlySalesTarget = x.MonthlySalesTarget,
                PlannedVisits = x.PlannedVisits
            })
            .ToList();
    }

    public List<SalesDistrictOptionDto> GetSalesDistrictOptions()
    {
        var rows = _context.SalesAnalyticsCustomers
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .Select(x => new
            {
                x.SalesDistrictCode,
                x.SalesDistrictName,
                x.BranchCode,
                x.BranchName
            })
            .ToList();

        return rows
            .GroupBy(x => x.SalesDistrictCode)
            .Select(g =>
            {
                var first = g.First();

                return new SalesDistrictOptionDto
                {
                    SalesDistrictCode = first.SalesDistrictCode ?? string.Empty,
                    SalesDistrictName = first.SalesDistrictName ?? string.Empty,
                    BranchCode = first.BranchCode,
                    BranchName = first.BranchName
                };
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .OrderBy(x => x.BranchName)
            .ThenBy(x => x.SalesDistrictName)
            .ToList();
    }

    public void SaveTarget(SaveSalesDistrictMonthlyTargetDto dto, string? userName)
    {
        var target = _context.SalesDistrictMonthlyTargets
            .FirstOrDefault(x =>
                x.Year == dto.Year &&
                x.Month == dto.Month &&
                x.SalesDistrictCode == dto.SalesDistrictCode);

        if (target == null)
        {
            target = new SalesDistrictMonthlyTargetEntity
            {
                Year = dto.Year,
                Month = dto.Month,

                BranchCode = dto.BranchCode?.Trim(),
                BranchName = dto.BranchName?.Trim(),

                SalesDistrictCode = dto.SalesDistrictCode.Trim(),
                SalesDistrictName = dto.SalesDistrictName.Trim(),

                MonthlySalesTarget = dto.MonthlySalesTarget,
                PlannedVisits = dto.PlannedVisits,

                CreatedAt = DateTime.Now,
                CreatedBy = userName
            };

            _context.SalesDistrictMonthlyTargets.Add(target);
        }
        else
        {
            target.BranchCode = dto.BranchCode?.Trim();
            target.BranchName = dto.BranchName?.Trim();

            target.SalesDistrictName = dto.SalesDistrictName.Trim();

            target.MonthlySalesTarget = dto.MonthlySalesTarget;
            target.PlannedVisits = dto.PlannedVisits;

            target.UpdatedAt = DateTime.Now;
            target.UpdatedBy = userName;
        }

        _context.SaveChanges();
    }

    public bool Delete(int id)
    {
        var target = _context.SalesDistrictMonthlyTargets
            .FirstOrDefault(x => x.Id == id);

        if (target == null)
            return false;

        _context.SalesDistrictMonthlyTargets.Remove(target);
        _context.SaveChanges();

        return true;
    }
}