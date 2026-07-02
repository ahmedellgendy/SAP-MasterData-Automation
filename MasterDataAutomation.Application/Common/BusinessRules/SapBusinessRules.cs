
namespace MasterDataAutomation.Application.Common.BusinessRules;

public static class SapBusinessRules
{
    public static BranchConfiguration GetBranch(string salesOffice)
    {
        return BranchConfigurations.Branches.First(x => x.SalesOffice == salesOffice);
    }

    public static string GetIncoterms(string customerType)
    {
        return customerType == "ثلاجة" ? "01" : "02";
    }

    public static string GetCustGrp1(string customerType)
    {
        return customerType == "ثلاجة" ? "01" : "02";
    }
}