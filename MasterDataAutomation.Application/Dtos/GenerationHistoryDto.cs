namespace MasterDataAutomation.Application.Dtos;

public class GenerationHistoryDto
{
    public DateTime Date { get; set; }

    public string FileName { get; set; } = string.Empty;

    public int CustomersCount { get; set; }

    public int FirstBpCode { get; set; }

    public int LastBpCode { get; set; }

    public string Status { get; set; } = string.Empty;
}