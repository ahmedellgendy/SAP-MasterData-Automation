namespace MasterDataAutomation.Application.Configuration;

public class SapSettings
{
    public DefaultValues DefaultValues { get; set; } = new();

    public Dictionary<string, BranchSettings> Branches { get; set; } = new();

    public Dictionary<string, CustomerTypeSettings> CustomerTypes { get; set; } = new();
}

public class DefaultValues
{
    public string BP_Group { get; set; } = "";
    public string Title { get; set; } = "";
    public string SearchTerm1 { get; set; } = "";
    public string NielsenId { get; set; } = "";
    public string Country { get; set; } = "";
    public string PostalCode1 { get; set; } = "";
    public string Language { get; set; } = "";
    public string CustAccGroup { get; set; } = "";
    public string CompanyCode { get; set; } = "";
    public string SalesOrg { get; set; } = "";
    public string DistChannel { get; set; } = "";
    public string Division { get; set; } = "";
    public string CustomerGroup { get; set; } = "";
    public string PriceList { get; set; } = "";
    public string CustomerCurrency { get; set; } = "";
    public string CustPrcGroup { get; set; } = "";
    public string CustPrcProcedure { get; set; } = "";
    public string PaymentTerms { get; set; } = "";
    public string DeliveringPlant { get; set; } = "";
    public string ReconcilAccount { get; set; } = "";
}

public class BranchSettings
{
    public string SalesOffice { get; set; } = "";
    public string Region { get; set; } = "";
    public string ShippingCond { get; set; } = "";
}

public class CustomerTypeSettings
{
    public string Incoterms { get; set; } = "";
    public string CustGrp1 { get; set; } = "";
}