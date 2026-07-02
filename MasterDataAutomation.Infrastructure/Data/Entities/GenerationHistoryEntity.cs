namespace MasterDataAutomation.Infrastructure.Data.Entities;

public class GenerationHistoryEntity
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string FileName { get; set; } = string.Empty;

    public int CustomersCount { get; set; }

    public int FirstBpCode { get; set; }

    public int LastBpCode { get; set; }

    public string Status { get; set; } = string.Empty;
}