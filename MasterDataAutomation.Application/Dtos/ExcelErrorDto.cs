using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataAutomation.Application.Dtos
{
    public class ExcelErrorDto
    {
        public int RowNumber { get; set; }

        public string Column { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
