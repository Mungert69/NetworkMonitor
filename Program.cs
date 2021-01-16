using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetworkMonitor.Data;
using System;

namespace NetworkMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] newArgs = new string[args.Length + 1];

            IConfigurationRoot config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();
            string urlString = config.GetSection("APIUrl").Value;
            newArgs[0] = urlString;                                // set the prepended value
            Array.Copy(args, 0, newArgs, 1, args.Length);
            IWebHost host = CreateWebHostBuilder(newArgs).Build();
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                try
                {
                    MonitorContext context = services.GetRequiredService<MonitorContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            host.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>


        WebHost.CreateDefaultBuilder(args).UseUrls(args[0])
                .UseStartup<Startup>();
    }
}
