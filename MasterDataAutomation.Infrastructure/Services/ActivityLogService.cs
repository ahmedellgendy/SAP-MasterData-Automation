using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MasterDataAutomation.Infrastructure.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityLogService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Log(
        string action,
        string module,
        string entityName,
        int? entityId,
        string? description)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var userName = user?.Identity?.Name
            ?? user?.FindFirst("FullName")?.Value
            ?? "System";

        var userRole = user?.FindFirst(ClaimTypes.Role)?.Value;

        var log = new ActivityLogEntity
        {
            UserName = userName,
            UserRole = userRole,
            Action = action,
            Module = module,
            EntityName = entityName,
            EntityId = entityId,
            Description = description,
            CreatedAt = DateTime.Now
        };

        _context.ActivityLogs.Add(log);
        _context.SaveChanges();
    }
}