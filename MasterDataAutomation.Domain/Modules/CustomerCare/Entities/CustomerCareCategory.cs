namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareCategory
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsQualityCategory { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CustomerCareSubCategory> SubCategories { get; set; }
        = new List<CustomerCareSubCategory>();
}