using MasterDataAutomation.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataAutomation.Application.Interfaces.Services
{
    public interface ISapCustomerMapper
    {
        List<SapCustomerDto> Map(List<CustomerImportDto> customers);
    }
}
