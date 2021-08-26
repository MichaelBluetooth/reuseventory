using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Services.SeedData
{
    public class SeedDataStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" || Environment.GetEnvironmentVariable("SEED_DATA") == "true")
                {
                    using (var scope = builder.ApplicationServices.CreateScope())
                    {
                        ReuseventoryDbContext db = scope.ServiceProvider.GetService<ReuseventoryDbContext>();
                        SeedDataHelper.seed(db);
                    };
                }
                next(builder);
            };
        }
    }
}