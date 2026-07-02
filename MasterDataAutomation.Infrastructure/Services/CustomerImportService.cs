using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Interfaces.Validators;
using MasterDataAutomation.Application.Validators;
using MasterDataAutomation.Infrastructure.Excel;
using Microsoft.AspNetCore.Http;

namespace MasterDataAutomation.Infrastructure.Services
{
    public class CustomerImportService : ICustomerImportService
    {
        private readonly IExcelReaderService _reader;
        private readonly ICustomerToSapMapper _mapper;
        private readonly IExcelWriterService _writer;
        private readonly IExcelErrorReportService _errorReportService;
        private readonly IEnumerable<ICustomerImportValidator> _validators;
        private readonly ISettingsService _settingsService;
        private readonly IHistoryService _historyService;
        private readonly ICustomerDraftRepository _customerDraftRepository;
        private readonly IExcelReaderService _excelReaderService;
        public CustomerImportService(
            IExcelReaderService reader,
            ICustomerToSapMapper mapper,
            IExcelWriterService writer,
            IExcelErrorReportService errorReportService,
            IEnumerable<ICustomerImportValidator> validators,
            ISettingsService settingsService,
            IHistoryService historyService,
            ICustomerDraftRepository customerDraftRepository
,
            IExcelReaderService excelReaderService)
        {
            _reader = reader;
            _mapper = mapper;
            _writer = writer;
            _errorReportService = errorReportService;
            _validators = validators;
            _settingsService = settingsService;
            _historyService = historyService;
            _customerDraftRepository = customerDraftRepository;
            _excelReaderService = excelReaderService;
        }
        public async Task<GenerateResultDto> GenerateSapFileAsync(IFormFile file, int lastBpCode)
        {
            using var stream = file.OpenReadStream();

            var readResult = await _reader.ReadCustomersAsync(stream);

            foreach (var validator in _validators)
            {
                readResult.Errors.AddRange(
                    validator.Validate(readResult.Customers));
            }

            Console.WriteLine($"Customers = {readResult.Customers.Count}");
            Console.WriteLine($"Errors = {readResult.Errors.Count}");

            if (readResult.Errors.Any())
            {
                return new GenerateResultDto
                {
                    Errors = readResult.Errors,
                    File = await _errorReportService.GenerateAsync(readResult.Errors),
                    FileName = "Error_Report.xlsx"
                };
            }

            var sapCustomers = new List<SapCustomerDto>();

            int bpCode = lastBpCode + 1;
            foreach (var customer in readResult.Customers)
            {
                sapCustomers.Add(_mapper.Map(customer, bpCode));
                bpCode++;
            }
            _settingsService.SaveLastBpCode(bpCode - 1);

            _historyService.Add(new GenerationHistoryDto
            {
                Date = DateTime.Now,
                FileName = file.FileName,
                CustomersCount = sapCustomers.Count,
                FirstBpCode = lastBpCode + 1,
                LastBpCode = bpCode - 1,
                Status = "Success"
            });

            return new GenerateResultDto
            {
                File = await _writer.GenerateSapFileAsync(sapCustomers),
                FileName = "SAP_Upload.xlsx"
            };
        }

        public async Task<List<CustomerImportDto>> ReadCustomersAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var readResult = await _reader.ReadCustomersAsync(stream);

            foreach (var validator in _validators)
            {
                readResult.Errors.AddRange(
                    validator.Validate(readResult.Customers));
            }

            if (readResult.Errors.Any())
            {
                throw new Exception(
                    string.Join(Environment.NewLine,
                        readResult.Errors.Select(x => x.Message)));
            }

            return readResult.Customers;
        }

        public async Task<GenerateResultDto> GenerateFromDraftAsync()
        {
            var customers = _customerDraftRepository.GetAll();

            if (!customers.Any())
            {
                return new GenerateResultDto
                {
                    Errors = new List<ExcelErrorDto>
            {
                new ExcelErrorDto
                {
                    Message = "Draft is empty."
                }
            }
                };
            }

            var firstBpCode = _settingsService.GetLastBpCode() + 1;
            var currentBpCode = firstBpCode;

            var sapCustomers = new List<SapCustomerDto>();

            foreach (var customer in customers)
            {
                sapCustomers.Add(_mapper.Map(customer, currentBpCode));
                currentBpCode++;
            }

            var lastBpCode = currentBpCode - 1;

            var fileName = $"SAP_Upload_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            var file = await _writer.GenerateSapFileAsync(sapCustomers);

            var generatedFolder = Path.Combine(AppContext.BaseDirectory, "GeneratedFiles");

            if (!Directory.Exists(generatedFolder))
            {
                Directory.CreateDirectory(generatedFolder);
            }

            var savedFilePath = Path.Combine(generatedFolder, fileName);

            await File.WriteAllBytesAsync(savedFilePath, file);

            _settingsService.SaveLastBpCode(lastBpCode);

            _historyService.Add(new GenerationHistoryDto
            {
                Date = DateTime.Now,
                FileName = fileName,
                CustomersCount = customers.Count,
                FirstBpCode = firstBpCode,
                LastBpCode = lastBpCode,
                Status = "Success"
            });

            _customerDraftRepository.Clear();

            return new GenerateResultDto
            {
                File = file,
                FileName = fileName
            };
        }

        public async Task<ExcelReadResult> ReadCustomersWithErrorsAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var result = await _excelReaderService.ReadCustomersAsync(stream);

            return result;
        }

        public async Task<GenerateResultDto> GenerateFromApprovedAsync()
        {
            var customers = _customerDraftRepository.GetApproved();

            if (!customers.Any())
            {
                return new GenerateResultDto
                {
                    Errors = new List<ExcelErrorDto>
            {
                new ExcelErrorDto
                {
                    Message = "No approved customers found."
                }
            }
                };
            }

            var firstBpCode = _settingsService.GetLastBpCode() + 1;
            var currentBpCode = firstBpCode;

            var sapCustomers = new List<SapCustomerDto>();

            foreach (var customer in customers)
            {
                sapCustomers.Add(_mapper.Map(customer, currentBpCode));
                currentBpCode++;
            }

            var lastBpCode = currentBpCode - 1;

            var fileName = $"SAP_Approved_Upload_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            var file = await _writer.GenerateSapFileAsync(sapCustomers);

            var generatedFolder = Path.Combine(AppContext.BaseDirectory, "GeneratedFiles");

            if (!Directory.Exists(generatedFolder))
            {
                Directory.CreateDirectory(generatedFolder);
            }

            var savedFilePath = Path.Combine(generatedFolder, fileName);

            await File.WriteAllBytesAsync(savedFilePath, file);

            _settingsService.SaveLastBpCode(lastBpCode);

            _historyService.Add(new GenerationHistoryDto
            {
                Date = DateTime.Now,
                FileName = fileName,
                CustomersCount = customers.Count,
                FirstBpCode = firstBpCode,
                LastBpCode = lastBpCode,
                Status = "Success"
            });

            _customerDraftRepository.ClearApproved();

            return new GenerateResultDto
            {
                File = file,
                FileName = fileName
            };
        }
    }
}
