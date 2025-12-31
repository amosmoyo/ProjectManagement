using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extentions
{
    public static class InfrastructureExtention
    {
        public static void ExtendedInfra(this IServiceCollection services, IConfiguration configuration)
        {
            string _dbConn = configuration.GetConnectionString("DefaultConnection")!;

            services.AddDbContext<Data.ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_dbConn).EnableSensitiveDataLogging(false).EnableDetailedErrors();
            });
        }
    }
}
