using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fairhr.Jobs.Filter;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.HttpJob;
using Hangfire.MySql.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fairhr.Jobs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(ConfigurationHangfire);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();
            //app.UseHangfireDashboard("/jobs", new DashboardOptions()
            //{
            //    Authorization = new[] { new FairhrJobsAuthorizationFilter() }
            //});
            app.UseHangfireDashboard("/jobs");

            app.Run(async (context) =>
           {
               await Task.CompletedTask;
               context.Response.Redirect("/jobs");
           });
        }

        #region Hangfire配置 https://www.bookstack.cn/read/Hangfire-zh-official/3.md
        private void ConfigurationHangfire(IGlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseStorage(
                new MySqlStorage(Configuration["jobdb"]))
                .UseConsole()
                .UseHangfireHttpJob(new HangfireHttpJobOptions()
                {
                    DashboardName = "泛亚统一任务调度平台",
                    DashboardTitle = "调度平台",
                    DashboardFooter = string.Empty
                });
        }
        #endregion
    }
}
