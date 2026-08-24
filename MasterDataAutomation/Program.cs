using MasterDataAutomation.Application.Configuration;
using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Interfaces.Validators;
using MasterDataAutomation.Application.Modules.CustomerModification.Interfaces;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Application.Validators;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.DependencyInjection;
using MasterDataAutomation.Infrastructure.Excel;
using MasterDataAutomation.Infrastructure.Mapping;
using MasterDataAutomation.Infrastructure.Modules.CustomerModification.Persistence;
using MasterDataAutomation.Infrastructure.Modules.ProductMaster.Persistence;
using MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;
using MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Services;
using MasterDataAutomation.Infrastructure.Persistence;
using MasterDataAutomation.Infrastructure.Services;
using MasterDataAutomation.Infrastructure.Settings;
using MasterDataAutomation.Infrastructure.Validators;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
>>>>>>> feature/sales-analytics

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder =
                WebApplication.CreateBuilder(args);

<<<<<<< HEAD
            // =====================================================
            // Database
            // =====================================================

            builder.Services.AddDbContext<ApplicationDbContext>(
                options =>
                    options.UseSqlServer(
                        builder.Configuration
                            .GetConnectionString(
                                "DefaultConnection")));

            // =====================================================
            // MVC
            // =====================================================
=======
            var dataProtectionKeysPath =
           Path.Combine(
               builder.Environment.ContentRootPath,
               "App_Data",
               "DataProtectionKeys");

            Directory.CreateDirectory(
                dataProtectionKeysPath);

            builder.Services
                .AddDataProtection()
                .PersistKeysToFileSystem(
                    new DirectoryInfo(
                        dataProtectionKeysPath))
                .SetApplicationName("FridayOps");

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 200 * 1024 * 1024; // 200 MB
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 200 * 1024 * 1024; // 200 MB
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });


            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
>>>>>>> feature/sales-analytics

            builder.Services.AddControllersWithViews();

<<<<<<< HEAD
            // =====================================================
            // SAP Settings
            // =====================================================

            builder.Services.Configure<SapSettings>(
                builder.Configuration
                    .GetSection("SapSettings"));

            // =====================================================
            // Data Protection
            //
            // Important for shared hosting / application recycle.
            // Keeps authentication cookies valid after app restart.
            // =====================================================

            var dataProtectionKeysPath =
                Path.Combine(
                    builder.Environment.ContentRootPath,
                    "App_Data",
                    "DataProtectionKeys");

            Directory.CreateDirectory(
                dataProtectionKeysPath);

            builder.Services
                .AddDataProtection()
                .PersistKeysToFileSystem(
                    new DirectoryInfo(
                        dataProtectionKeysPath))
                .SetApplicationName(
                    "FridayOps");

            // =====================================================
            // Authentication
            // =====================================================

            builder.Services
                .AddAuthentication(
                    CookieAuthenticationDefaults
                        .AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath =
                        "/Account/Login";

                    options.AccessDeniedPath =
                        "/Account/AccessDenied";

                    options.ExpireTimeSpan =
                        TimeSpan.FromHours(8);

                    options.SlidingExpiration =
                        true;

                    options.Cookie.HttpOnly =
                        true;

                    options.Cookie.SameSite =
                        SameSiteMode.Lax;

                    options.Cookie.SecurePolicy =
                        CookieSecurePolicy.SameAsRequest;
                });
=======
            builder.Services
                  .AddAuthentication(
                      CookieAuthenticationDefaults.AuthenticationScheme)
                  .AddCookie(options =>
                  {
                      options.LoginPath =
                          "/Account/Login";

                      options.AccessDeniedPath =
                          "/Account/AccessDenied";

                      options.ExpireTimeSpan =
                          TimeSpan.FromHours(8);

                      options.SlidingExpiration =
                          true;
                  });
>>>>>>> feature/sales-analytics

            builder.Services.AddAuthorization();

            // =====================================================
            // Infrastructure
            // =====================================================

            builder.Services.AddInfrastructure();

            // =====================================================
            // Application Services
            // =====================================================

            builder.Services.AddScoped<
                IExcelReaderService,
                ExcelReaderService>();

            builder.Services.AddScoped<
                ICustomerToSapMapper,
                CustomerToSapMapper>();

            builder.Services.AddScoped<
                IExcelWriterService,
                ExcelWriterService>();

            builder.Services.AddScoped<
                ICustomerImportService,
                CustomerImportService>();

            builder.Services.AddScoped<
                ICustomerValidator,
                CustomerValidator>();

            builder.Services.AddScoped<
                IExcelErrorReportService,
                ExcelErrorReportService>();

            builder.Services.AddScoped<
                ICustomerImportValidator,
                DuplicateCustomerValidator>();

            builder.Services.AddScoped<
                ISettingsService,
                SqlSettingsService>();

            builder.Services.AddScoped<
                IHistoryService,
                SqlHistoryService>();

            builder.Services.AddScoped<
                ICustomerDraftRepository,
                SqlCustomerDraftRepository>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductRequestRepository, ProductRequestRepository>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
            builder.Services.AddScoped<ICustomerModificationRequestRepository, CustomerModificationRequestRepository>();
            builder.Services.AddScoped<ISalesAnalyticsMasterDataRepository, SalesAnalyticsMasterDataRepository>();
            builder.Services.AddScoped<ISalesAnalyticsMasterDataImportService, SalesAnalyticsMasterDataImportService>();
            builder.Services.AddScoped<ISalesAnalyticsDashboardRepository, SalesAnalyticsDashboardRepository>();
            builder.Services.AddScoped<ISalesDistrictMonthlyTargetRepository, SalesDistrictMonthlyTargetRepository>();
            builder.Services.AddScoped<ISalesAnalyticsDataQualityRepository, SalesAnalyticsDataQualityRepository>();


            // =====================================================
            // Session
            // =====================================================

            builder.Services
                .AddDistributedMemoryCache();

            builder.Services
                .AddSession(options =>
                {
                    options.IdleTimeout =
                        TimeSpan.FromHours(8);

                    options.Cookie.HttpOnly =
                        true;

                    options.Cookie.IsEssential =
                        true;
                });

            // =====================================================
            // Build
            // =====================================================

            var app =
                builder.Build();

            // =====================================================
            // Production Pipeline
            // =====================================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(
                    "/Home/Error");

                app.UseHsts();
            }

            // =====================================================
            // HTTP Pipeline
            // =====================================================

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            // =====================================================
            // Static Assets
            // =====================================================

            app.MapStaticAssets();

            // =====================================================
            // Routes
            // =====================================================

            app.MapControllerRoute(
                    name: "default",
                    pattern:
                        "{controller=SalesDashboard}/{action=Index}/{id?}")
                .WithStaticAssets();

            // =====================================================
            // Run
            // =====================================================

            app.Run();
        }
    }
}