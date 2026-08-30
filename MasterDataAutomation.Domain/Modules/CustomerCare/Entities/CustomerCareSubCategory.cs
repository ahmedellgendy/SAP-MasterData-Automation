namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareSubCategory
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CustomerCareCategory Category { get; set; } = null!;
}