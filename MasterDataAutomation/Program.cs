using MasterDataAutomation.Application.Configuration;
using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Interfaces.Validators;
using MasterDataAutomation.Application.Validators;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.DependencyInjection;
using MasterDataAutomation.Infrastructure.Excel;
using MasterDataAutomation.Infrastructure.Mapping;
using MasterDataAutomation.Infrastructure.Persistence;
using MasterDataAutomation.Infrastructure.Services;
using MasterDataAutomation.Infrastructure.Settings;
using MasterDataAutomation.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Infrastructure.Modules.ProductMaster.Persistence;


namespace MasterDataAutomation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.Configure<SapSettings>(
            builder.Configuration.GetSection("SapSettings"));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

            builder.Services.AddAuthorization();

            builder.Services.AddInfrastructure();

            builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
            builder.Services.AddScoped<ICustomerToSapMapper, CustomerToSapMapper>();
            builder.Services.AddScoped<IExcelWriterService, ExcelWriterService>();
            builder.Services.AddScoped<ICustomerImportService, CustomerImportService>();
            builder.Services.AddScoped<ICustomerValidator, CustomerValidator>();
            builder.Services.AddScoped<IExcelErrorReportService, ExcelErrorReportService>();
            builder.Services.AddScoped<ICustomerImportValidator, DuplicateCustomerValidator>();
            builder.Services.AddScoped<ISettingsService, SqlSettingsService>();
            builder.Services.AddScoped<IHistoryService, SqlHistoryService>();
            builder.Services.AddScoped<ICustomerDraftRepository, SqlCustomerDraftRepository>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductRequestRepository, ProductRequestRepository>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IActivityLogService, ActivityLogService>();



            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=SalesDashboard}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
