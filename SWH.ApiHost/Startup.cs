using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWH.ApiHost.Code;
using SWH.ApiHost.ServiceBus;

namespace SWH.ApiHost
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
            services.AddAuthentication();
            
            services.AddMvc();

            services.AddSingleton<ISendMessages, ServiceBusMessenger>();
            //services.AddSingleton<ISmartSensorMaster, SmartSensorMaster>();
            services.AddSingleton<ISmartSensorMaster, MockSensorMaster>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
           
            app.UseMvcWithDefaultRoute();
        }
    }
}
