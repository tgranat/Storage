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
using Microsoft.EntityFrameworkCore;
using Storage.Data;

namespace Storage
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
            services.AddControllersWithViews();
            // StorageContext injected. Set to use sql database with connection string from appsettings.json
            services.AddDbContext<StorageContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("StorageContext")).EnableSensitiveDataLogging());
            // EnableSensitiveDataLogging() to enable database logging. 
            // added to LogLevel in appsettings: "Microsoft.EntityFrameWorkCore.Database.Command" : "Information"
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-3.1
                // ASP.NET Core apps can mix the use of conventional routing and attribute routing. It's typical to use conventional routes for 
                // controllers serving HTML pages for browsers, and attribute routing for controllers serving REST APIs.
                //Actions are either conventionally routed or attribute routed.Placing a route on the controller or the action makes it attribute
                // routed. Actions that define attribute routes cannot be reached through the conventional routes and vice-versa.Any route attribute 
                //on the controller makes all actions in the controller attribute routed.

                //endpoints.MapControllers();  // To map attribute routed controllers.

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Products}/{action=Index}/{id?}");
            });
        }
    }
}
