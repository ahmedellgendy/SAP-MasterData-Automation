using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Infrastructure.Excel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataAutomation.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IExcelReaderService, ExcelReaderService>();

            return services;
        }
    }
}
