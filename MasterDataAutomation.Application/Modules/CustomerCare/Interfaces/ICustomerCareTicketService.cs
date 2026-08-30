using MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Interfaces;

public interface ICustomerCareTicketService
{
    Task<CustomerCareTicketDto> CreateAsync(
        CreateCustomerCareTicketDto dto,
        int createdByUserId,
        string createdByUserName,
        CancellationToken cancellationToken = default);

    Task<CustomerCareTicketDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CustomerCareTicketDto?> GetByTicketNumberAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default);

    Task<List<CustomerCareTicketListItemDto>> GetQueueAsync(
    CustomerCareTicketFilterDto filter,
    CancellationToken cancellationToken = default);

    Task<List<CustomerCareTicketListItemDto>> GetCustomerHistoryAsync(
        string phone,
        CancellationToken cancellationToken = default);

    Task<CustomerCareDuplicateCustomerDto?> CheckDuplicateCustomerAsync(
        string phone,
        CancellationToken cancellationToken = default);

    Task AssignAsync(
        int ticketId,
        AssignCustomerCareTicketDto dto,
        int changedByUserId,
        string changedByUserName,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        int ticketId,
        ChangeCustomerCareTicketStatusDto dto,
        int changedByUserId,
        string changedByUserName,
        CancellationToken cancellationToken = default);

    Task AddActionAsync(
        int ticketId,
        AddCustomerCareTicketActionDto dto,
        int createdByUserId,
        string createdByUserName,
        CancellationToken cancellationToken = default);

    Task ResolveAsync(
        int ticketId,
        ResolveCustomerCareTicketDto dto,
        int changedByUserId,
        string changedByUserName,
        CancellationToken cancellationToken = default);

    Task ReopenAsync(
        int ticketId,
        string reason,
        int changedByUserId,
        string changedByUserName,
        CancellationToken cancellationToken = default);

    Task<List<CustomerCareLookupDto>> GetCategoriesAsync(
    CancellationToken cancellationToken = default);

    Task<List<CustomerCareLookupDto>> GetSubCategoriesAsync(
        int categoryId,
        CancellationToken cancellationToken = default);

    Task<List<CustomerCareLookupDto>> GetDepartmentsAsync(
        CancellationToken cancellationToken = default);

    Task<CustomerCareTicketDetailsDto?> GetDetailsAsync(
    int id,
    CancellationToken cancellationToken = default);

    Task CloseAsync(
    int ticketId,
    string reason,
    int changedByUserId,
    string changedByUserName,
    CancellationToken cancellationToken = default);

    Task<CustomerCareDashboardDto> GetDashboardAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    CancellationToken cancellationToken = default);


    Task AddGiftAsync(
    int ticketId,
    AddCustomerCareTicketGiftDto dto,
    int createdByUserId,
    string createdByUserName,
    CancellationToken cancellationToken = default);

    Task DeleteGiftAsync(
        int ticketId,
        int giftId,
        CancellationToken cancellationToken = default);

    Task AddAttachmentAsync(
    int ticketId,
    AddCustomerCareAttachmentDto dto,
    int uploadedByUserId,
    string uploadedByUserName,
    CancellationToken cancellationToken = default);

    Task<string> DeleteAttachmentAsync(
        int ticketId,
        int attachmentId,
        CancellationToken cancellationToken = default);
}