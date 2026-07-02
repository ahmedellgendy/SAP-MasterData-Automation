namespace MasterDataAutomation.Application.Common.Constants
{
    public static class ExcelHeaders
    {
        public const string Line = "الخط";
        public const string Market = "الماركت";
        public const string Branch = "الفرع";
        public const string SalesDistrict = "SalesDistrict";
        public const string CustomerType = "النوع";

        public static readonly string[] RequiredHeaders =
        {
        Line,
        Market,
        Branch,
        SalesDistrict,
        CustomerType
        };
    }
}
