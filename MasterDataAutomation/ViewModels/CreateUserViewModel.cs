using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MasterDataAutomation.Web.ViewModels;

public class CreateUserViewModel
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Sales";

    public List<SelectListItem> Roles { get; set; } = new();
}