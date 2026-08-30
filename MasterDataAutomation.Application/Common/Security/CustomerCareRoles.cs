namespace MasterDataAutomation.Application.Common.Security;

public static class CustomerCareRoles
{
    public const string Agent = "ComplaintAgent";

    public const string Supervisor = "ComplaintSupervisor";

    public const string Manager = "ComplaintManager";

    public const string Admin = "Admin";

    public const string All =
        Agent + "," +
        Supervisor + "," +
        Manager + "," +
        Admin;

    public const string SupervisorsAndAbove =
        Supervisor + "," +
        Manager + "," +
        Admin;

    public const string ManagersAndAdmin =
        Manager + "," +
        Admin;
}