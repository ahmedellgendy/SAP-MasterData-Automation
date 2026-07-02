using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities;

namespace MasterDataAutomation.Infrastructure.Services;

public class SqlHistoryService : IHistoryService
{
    private readonly ApplicationDbContext _context;

    public SqlHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<GenerationHistoryDto> GetAll()
    {
        return _context.GenerationHistories
            .OrderByDescending(x => x.Date)
            .Select(x => new GenerationHistoryDto
            {
                Date = x.Date,
                FileName = x.FileName,
                CustomersCount = x.CustomersCount,
                FirstBpCode = x.FirstBpCode,
                LastBpCode = x.LastBpCode,
                Status = x.Status
            })
            .ToList();
    }

    public void Add(GenerationHistoryDto history)
    {
        var entity = new GenerationHistoryEntity
        {
            Date = history.Date,
            FileName = history.FileName,
            CustomersCount = history.CustomersCount,
            FirstBpCode = history.FirstBpCode,
            LastBpCode = history.LastBpCode,
            Status = history.Status
        };

        _context.GenerationHistories.Add(entity);
        _context.SaveChanges();
    }
}