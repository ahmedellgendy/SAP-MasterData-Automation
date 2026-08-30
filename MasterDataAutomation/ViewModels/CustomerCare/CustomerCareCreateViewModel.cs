using System.ComponentModel.DataAnnotations;
using MasterDataAutomation.Application.Modules.CustomerCare.Dtos;
using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
namespace MasterDataAutomation.Web.ViewModels.CustomerCare;

public class CustomerCareCreateViewModel
{
    // =========================================================
    // Customer
    // =========================================================

    public int? CustomerId { get; set; }

    [Required]
    [Display(Name = "اسم العميل")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "رقم الموبايل")]
    public string? CustomerPhone { get; set; }

    [Display(Name = "العنوان")]
    public string? CustomerAddress { get; set; }

    // =========================================================
    // Ticket
    // =========================================================

    [Required]
    [Display(Name = "مصدر التواصل")]
    public CustomerCareTicketSource Source { get; set; }

    [Required]
    [Display(Name = "نوع الحالة")]
    public CustomerCareTicketType Type { get; set; }

    [Display(Name = "التصنيف")]
    public int? CategoryId { get; set; }

    [Display(Name = "التصنيف الفرعي")]
    public int? SubCategoryId { get; set; }

    [Required]
    [Display(Name = "تفاصيل الحالة")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "الأولوية")]
    public CustomerCareTicketPriority Priority { get; set; }
        = CustomerCareTicketPriority.Normal;

    // =========================================================
    // Assignment
    // =========================================================

    [Display(Name = "القسم المسؤول")]
    public int? AssignedDepartmentId { get; set; }

    [Display(Name = "متابعة في")]
    public DateTime? NextFollowUpAt { get; set; }

    // =========================================================
    // Quality
    // =========================================================

    public bool IsQuality { get; set; }

    public int? ProductId { get; set; }

    [Display(Name = "اسم المنتج")]
    public string? ProductName { get; set; }

    [Display(Name = "رقم التشغيلة")]
    public string? BatchNumber { get; set; }

    [Display(Name = "تاريخ الإنتاج")]
    public DateTime? ProductionDate { get; set; }

    [Display(Name = "تاريخ الصلاحية")]
    public DateTime? ExpiryDate { get; set; }

    [Display(Name = "نوع مشكلة الجودة")]
    public string? QualityIssueType { get; set; }

    [Display(Name = "تفاصيل الجودة")]
    public string? QualityIssueDetails { get; set; }

    public bool SampleRequired { get; set; }

    public bool HasHealthRisk { get; set; }

    public bool HasLegalRisk { get; set; }

    public string? RiskNotes { get; set; }

    // =========================================================
    // Dropdowns
    // =========================================================

    public List<CustomerCareLookupDto> Categories { get; set; } = new();

    public List<CustomerCareLookupDto> SubCategories { get; set; } = new();

    public List<CustomerCareLookupDto> Departments { get; set; } = new();

    public IFormFile? QualityImage { get; set; }

    public string? QualityImageDescription { get; set; }
}