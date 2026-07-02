namespace MasterDataAutomation.Infrastructure.Data.Entities;

public class AppUserEntity
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Sales";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}