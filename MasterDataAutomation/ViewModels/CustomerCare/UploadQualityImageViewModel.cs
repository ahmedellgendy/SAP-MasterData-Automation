using Microsoft.AspNetCore.Http;

namespace MasterDataAutomation.Web.ViewModels.CustomerCare;

public class UploadQualityImageViewModel
{
    public IFormFile Image { get; set; } = null!;

    public string? Description { get; set; }
}