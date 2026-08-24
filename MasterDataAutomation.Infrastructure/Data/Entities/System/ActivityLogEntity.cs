namespace MasterDataAutomation.Infrastructure.Data.Entities.System;

public class ActivityLogEntity
{
    public int Id { get; set; }

    public string? UserName { get; set; }
    public string? UserRole { get; set; }

    public string Action { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;

    public int? EntityId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}