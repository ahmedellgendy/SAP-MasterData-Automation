namespace MasterDataAutomation.Application.Interfaces.Services;

public interface ISettingsService
{
    int GetLastBpCode();

    void SaveLastBpCode(int bpCode);
}