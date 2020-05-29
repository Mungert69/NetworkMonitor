using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetworkMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] newArgs = new string[args.Length + 1];

            var config = new ConfigurationBuilder()
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
                    // Add EF context here.
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
