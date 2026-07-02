using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MasterDataAutomation.Web.ViewModels;

public class CreateCustomerViewModel
{
    public List<SelectListItem> Branches { get; set; } = new();

    public List<SelectListItem> CustomerTypes { get; set; } = new();
    public List<LineOptionViewModel> Lines { get; set; } = new();

    [Required]
    public string Line { get; set; } = string.Empty;

    [Required]
    public string Market { get; set; } = string.Empty;

    [Required]
    public string Branch { get; set; } = string.Empty;

    [Required]
    public string SalesDistrict { get; set; } = string.Empty;

    [Required]
    public string CustomerType { get; set; } = string.Empty;

    public int? Index { get; set; }
}