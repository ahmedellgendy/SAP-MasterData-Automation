public static class BranchConfigurations
{
    public static readonly List<BranchConfiguration> Branches = new()
    {
        new()
        {
            SalesOffice = "1000",
            Region = "01",
            ShippingCondition = "10"
        },

        new()
        {
            SalesOffice = "2000",
            Region = "01",
            ShippingCondition = "20"
        },

        new()
        {
            SalesOffice = "3000",
            Region = "02",
            ShippingCondition = "30"
        },

        new()
        {
            SalesOffice = "4000",
            Region = "03",
            ShippingCondition = "40"
        }
    };
}