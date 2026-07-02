using System.ComponentModel.DataAnnotations;

namespace MasterDataAutomation.Web.ViewModels;

public class SystemSettingsViewModel
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Last BP Code must be greater than zero.")]
    public int LastBpCode { get; set; }
}