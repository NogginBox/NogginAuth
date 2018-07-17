﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Noggin.NetCoreAuth.Config;
using Noggin.SampleSite.Data;
using System;

namespace Noggin.SampleSite
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNogginNetCoreAuth<SampleLoginHandler>(Configuration);
            services.AddScoped<ISimpleDbContext, SampleSimpleDbContext>();

            services
                .AddMemoryCache()
                .AddSession(options =>
                {
                    // Set a short timeout for easy testing.
                    options.IdleTimeout = TimeSpan.FromMinutes(5);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = ".Font.Wtf.Session";
                });

            // Add framework services.
            services.AddMvc();
            services.AddDbContext<SampleSimpleDbContext>();

            //services.AddAuthorization();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "NogginSampleCookieScheme";
                    options.DefaultChallengeScheme = "NogginSampleCookieScheme";
                    options.DefaultAuthenticateScheme = "NogginSampleCookieScheme";
                })
                .AddCookie("NogginSampleCookieScheme", options => {
                    options.AccessDeniedPath = "/";
                    options.LoginPath = "/";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            /*}
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }*/

            app.UseStaticFiles();
            app.UseSession();

            app.UseAuthentication();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapNogginNetAuthRoutes(app.ApplicationServices);
            });
        }
    }
}
