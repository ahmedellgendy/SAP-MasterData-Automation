using System.Text.Json;
using MasterDataAutomation.Application.Interfaces.Services;

namespace MasterDataAutomation.Infrastructure.Settings;

public class SettingsService : ISettingsService
{
    private readonly string _filePath;

    public SettingsService()
    {
        _filePath = Path.Combine(
            AppContext.BaseDirectory,
            "settings.json");
    }

    public int GetLastBpCode()
    {
        if (!File.Exists(_filePath))
            return 60015785;

        var json = File.ReadAllText(_filePath);

        var settings = JsonSerializer.Deserialize<AppSettings>(json);

        return settings?.LastBpCode ?? 60015785;
    }

    public void SaveLastBpCode(int bpCode)
    {
        var settings = new AppSettings
        {
            LastBpCode = bpCode
        };

        var json = JsonSerializer.Serialize(settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        Console.WriteLine($"Saving to: {_filePath}");

        File.WriteAllText(_filePath, json);

        Console.WriteLine("Settings saved.");

    }

    private class AppSettings
    {
        public int LastBpCode { get; set; }
    }
}