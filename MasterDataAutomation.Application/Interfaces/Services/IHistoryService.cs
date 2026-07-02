using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Interfaces.Services;

public interface IHistoryService
{
    List<GenerationHistoryDto> GetAll();

    void Add(GenerationHistoryDto history);
}