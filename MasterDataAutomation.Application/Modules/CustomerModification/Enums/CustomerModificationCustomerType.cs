using System.ComponentModel.DataAnnotations;

namespace MasterDataAutomation.Application.Modules.CustomerModification.Enums
{
    public enum CustomerModificationCustomerType
    {
        [Display(Name = "خاص")]
        Private = 1,

        [Display(Name = "ثلاجة")]
        Fridge = 2
    }
}
