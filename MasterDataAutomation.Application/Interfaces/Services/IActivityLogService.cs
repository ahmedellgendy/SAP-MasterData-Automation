namespace MasterDataAutomation.Application.Interfaces.Services;

public interface IActivityLogService
{
    void Log(
        string action,
        string module,
        string entityName,
        int? entityId,
        string? description);
}