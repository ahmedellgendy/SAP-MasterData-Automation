namespace MasterDataAutomation.Application.Dtos;

public class SapCustomerDto
{
    // Dynamic
    public string BP_Group { get; set; } = string.Empty;
    public string BP_Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SearchTerm1 { get; set; } = string.Empty;
    public string Name1 { get; set; } = string.Empty;
    public string Name2 { get; set; } = string.Empty;
    public string NielsenId { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode1 { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string TransportZone { get; set; } = string.Empty;
    // Company
    public string CustAccGroup { get; set; } = string.Empty;
    public string CompanyCode { get; set; } = string.Empty;
    public string SalesOrg { get; set; } = string.Empty;
    public string DistChannel { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    // Sales
    public string SalesDistrict { get; set; } = string.Empty;
    public string CustomerGroup { get; set; } = string.Empty;
    public string SalesOffice { get; set; } = string.Empty;
    public string PriceList { get; set; } = string.Empty;
    public string CustomerCurrency { get; set; } = string.Empty;
    public string CustPrcGroup { get; set; } = string.Empty;
    public string CustPrcProcedure { get; set; } = string.Empty;
    public string ShippingCond { get; set; } = string.Empty;
    public string Incoterms { get; set; } = string.Empty;
    public string PaymentTerms { get; set; } = string.Empty;
    public string CustGrp1 { get; set; } = string.Empty;
    // Finance
    public string DeliveringPlant { get; set; } = string.Empty;
    public string ReconcilAccount { get; set; } = string.Empty;
}