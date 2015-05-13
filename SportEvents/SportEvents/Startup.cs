using Hangfire;
using Microsoft.Owin;
using Hangfire.SqlServer;
using Owin;
using System;

[assembly: OwinStartup(typeof(SportEvents.Startup))]

namespace SportEvents
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.UseHangfire(config =>
            {
                // Basic setup required to process background jobs.
                config.UseSqlServerStorage("dedekDB");
                config.UseServer();
                config.UseAuthorizationFilters();
                config.UseDashboardPath("/jobs");
            });
        }
    }
}