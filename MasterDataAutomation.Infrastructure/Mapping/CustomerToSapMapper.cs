using MasterDataAutomation.Application.Common.BusinessRules;
using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Configuration;
using Microsoft.Extensions.Options;
namespace MasterDataAutomation.Infrastructure.Mapping;

public class CustomerToSapMapper : ICustomerToSapMapper
{
    private readonly SapSettings _settings;

    public CustomerToSapMapper(IOptions<SapSettings> options)
    {
        _settings = options.Value;
    }
    public SapCustomerDto Map(CustomerImportDto customer, int bpCode)
    {
        var branch = _settings.Branches[customer.Branch];
        var customerType = _settings.CustomerTypes[customer.CustomerType];

        return new SapCustomerDto
        {
            // Dynamic
            BP_Code = bpCode.ToString(),
            Name1 = $"{customer.Market} ({customer.CustomerType})",
            Name2 = "",
            City = customer.Branch,
            Street = customer.Line,
            SalesOffice = branch.SalesOffice,
            SalesDistrict = customer.SalesDistrict,
            TransportZone = customer.SalesDistrict,

            // Business Rules
            Region = branch.Region,
            ShippingCond = branch.ShippingCond,
            Incoterms = customerType.Incoterms,
            CustGrp1 = customerType.CustGrp1,

            // Constants
            BP_Group = "6000",
            Title = "0003",
            SearchTerm1 = "Retail",
            NielsenId = "P",
            Country = "EG",
            PostalCode1 = "1234",
            Language = "E",
            CustAccGroup = "6000",
            CompanyCode = "2000",
            SalesOrg = "2000",
            DistChannel = "20",
            Division = "10",
            CustomerGroup = "01",
            PriceList = "01",
            CustomerCurrency = "EGP",
            CustPrcGroup = "07",
            CustPrcProcedure = "Z1",
            PaymentTerms = "C001",
            DeliveringPlant = "2000",
            ReconcilAccount = "1206000001"
        };
    }
}