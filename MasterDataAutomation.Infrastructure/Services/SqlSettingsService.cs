using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities;

namespace MasterDataAutomation.Infrastructure.Services;

public class SqlSettingsService : ISettingsService
{
    private readonly ApplicationDbContext _context;

    public SqlSettingsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public int GetLastBpCode()
    {
        var setting = _context.SystemSettings
            .FirstOrDefault(x => x.Key == "LastBpCode");

        if (setting == null)
        {
            _context.SystemSettings.Add(new SystemSettingEntity
            {
                Key = "LastBpCode",
                Value = "100000"
            });

            _context.SaveChanges();

            return 100000;
        }

        return int.TryParse(setting.Value, out var value)
            ? value
            : 100000;
    }

    public void SaveLastBpCode(int lastBpCode)
    {
        var setting = _context.SystemSettings
            .FirstOrDefault(x => x.Key == "LastBpCode");

        if (setting == null)
        {
            _context.SystemSettings.Add(new SystemSettingEntity
            {
                Key = "LastBpCode",
                Value = lastBpCode.ToString()
            });
        }
        else
        {
            setting.Value = lastBpCode.ToString();
        }

        _context.SaveChanges();
    }
}