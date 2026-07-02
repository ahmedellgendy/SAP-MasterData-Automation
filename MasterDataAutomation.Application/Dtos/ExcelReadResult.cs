using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataAutomation.Application.Dtos
{
    public class ExcelReadResult
    {
        public List<CustomerImportDto> Customers { get; set; } = new();

        public List<ExcelErrorDto> Errors { get; set; } = new();

        public bool IsSuccess => Errors.Count == 0;
    }
}
