using System.Text.Json;
using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Services;

namespace MasterDataAutomation.Infrastructure.Settings;

public class HistoryService : IHistoryService
{
    private readonly string _filePath;

    public HistoryService()
    {
        _filePath = Path.Combine(
            AppContext.BaseDirectory,
            "history.json");
    }

    public List<GenerationHistoryDto> GetAll()
    {
        if (!File.Exists(_filePath))
            return new List<GenerationHistoryDto>();

        var json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<List<GenerationHistoryDto>>(json)
               ?? new List<GenerationHistoryDto>();
    }

    public void Add(GenerationHistoryDto history)
    {
        var historyList = GetAll();

        historyList.Add(history);

        var json = JsonSerializer.Serialize(
            historyList,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, json);
    }
}