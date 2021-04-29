using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.AspNetCore.SignalR;
using PersonalSite.Hubs;
using PersonalSite.MQTT;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Data;

namespace PersonalSite
{
    public class Startup
    {
        MQTTBroker mqttBroker;
        MQTTListener mqttListener;
        
        DebugLoggerProvider loggerProvider;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            mqttBroker = new MQTTBroker(1883);
            MQTTProducer.init();

            //Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSignalR();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });

            services.AddDbContext<PersonalSiteContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("PersonalSiteContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            mqttListener = new MQTTListener(app.ApplicationServices.GetService<IHubContext<ChatHub>>());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
