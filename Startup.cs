using ASPNETCoreScheduler.Scheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetworkMonitor.Data;
using NetworkMonitor.Services;

namespace NetworkMonitor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, PingScheduleTask>();
            services.AddSingleton<IHostedService, SaveScheduleTask>();
            services.AddSingleton<IHostedService, AlertScheduleTask>();
            services.AddSingleton<IMonitorPingService, MonitorPingService>();
            services.AddSingleton<INetStatsService, NetStatsService>();
            services.AddSingleton<IMessageService, MailMessageService>();
            services.AddDbContext<MonitorContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin());
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseRouting();

            app.UseAuthorization();
            app.UseCors("AllowAnyOrigin");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
