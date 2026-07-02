using System.ComponentModel.DataAnnotations;

namespace MasterDataAutomation.Web.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        public IFormFile File { get; set; } = default!;

        [Required]
        public int LastBpCode { get; set; }

    }
}
