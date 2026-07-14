using System.ComponentModel.DataAnnotations;

namespace MasterDataAutomation.Application.Modules.CustomerModification.Enums
{
    public enum CustomerModificationType
    {
        [Display(Name = "رفع الثلاجة")]
        RemoveFridge = 1,

        [Display(Name = "تعديل اسم")]
        ChangeName = 2,

        [Display(Name = "تحويل إلى خاص")]
        ChangeToPrivate = 3,

        [Display(Name = "تحويل إلى ثلاجة")]
        ChangeToFridge = 4
    }
}
