using MasterDataAutomation.Application.Common.Constants;

namespace MasterDataAutomation.Infrastructure.Excel.Validators;

public class ExcelHeaderValidator
{
    public List<string> Validate(Dictionary<string, int> headers)
    {
        var errors = new List<string>();

        foreach (var requiredHeader in ExcelHeaders.RequiredHeaders)
        {
            if (!headers.ContainsKey(requiredHeader))
            {
                errors.Add($"Missing Header: {requiredHeader}");
            }
        }

        return errors;
    }
}