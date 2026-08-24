using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;

public interface ISalesDistrictMonthlyTargetRepository
{
    List<SalesDistrictMonthlyTargetDto> GetTargets(int year, int month);

    List<SalesDistrictOptionDto> GetSalesDistrictOptions();

    void SaveTarget(SaveSalesDistrictMonthlyTargetDto dto, string? userName);

    bool Delete(int id);
}