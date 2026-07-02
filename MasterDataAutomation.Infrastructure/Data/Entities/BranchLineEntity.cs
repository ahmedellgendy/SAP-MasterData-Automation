namespace MasterDataAutomation.Infrastructure.Data.Entities;

public class BranchLineEntity
{
    public int Id { get; set; }

    public string Branch { get; set; } = string.Empty;

    public string Line { get; set; } = string.Empty;

    public string SalesDistrict { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}