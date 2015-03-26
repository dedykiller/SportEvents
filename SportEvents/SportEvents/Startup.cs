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
            

            /*
             * Mel si to blbe buzno.
             * Uz sem to spravil.
             * 
             * PM> Install-Package Hangfire - mel si neco jineho
             * Konfigurace cajk, akorat si mel spatny namespace
             */


            //app.UseHangfire(config =>
            //{
            //    // Basic setup required to process background jobs.
            //    config.UseSqlServerStorage("dedekDB");
            //    config.UseServer();

            //    var options = new SqlServerStorageOptions
            //    {
            //        QueuePollInterval = TimeSpan.FromSeconds(15) // Default value
            //    };

            //    var storage = new SqlServerStorage("dedekDB", options);
            //});
        }
    }
}