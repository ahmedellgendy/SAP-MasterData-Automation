namespace MasterDataAutomation.Application.Dtos
{
    public class CustomerImportDto
    {

        public string Line { get; set; } = string.Empty;

        public string Market { get; set; } = string.Empty;

        public string Branch { get; set; } = string.Empty;

        public string SalesDistrict { get; set; } = string.Empty;

        public string CustomerType { get; set; } = string.Empty;

        public string DuplicateKey => $"{Market.Trim().ToUpper()}|{Line.Trim().ToUpper()}";
    }
}
