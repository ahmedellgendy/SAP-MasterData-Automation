namespace MasterDataAutomation.Application.Dtos;

public class GenerateResultDto
{
    public bool HasErrors => Errors.Any();

    public List<ExcelErrorDto> Errors { get; set; } = new();

    public byte[] File { get; set; } = Array.Empty<byte>();

    public string FileName { get; set; } = string.Empty;
}