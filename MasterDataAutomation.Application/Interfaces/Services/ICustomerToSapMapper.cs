using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Interfaces.Services;

public interface ICustomerToSapMapper
{
    SapCustomerDto Map(CustomerImportDto customer, int bpCode);
}