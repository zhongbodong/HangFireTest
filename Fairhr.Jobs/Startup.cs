using System;
using System.Threading.Tasks;
using Fairhr.Logs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.HttpJob;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddFairhrLogs(options =>
            {
                options.Key = Configuration["logKey"];
                options.ServerUrl = Configuration["logUrl"];
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();

            // 管理员面板
            app.UseHangfireDashboard("/jobs", adminOptions);

            //只读面板，只能读取不能操作
            app.UseHangfireDashboard("/read", readOptions);

            app.UseFairhrLogs();
            app.Run(async (context) =>
           {
               await Task.CompletedTask;
               context.Response.Redirect("/jobs");
           });
        }

        #region Hangfire配置 https://github.com/yuzd/Hangfire.HttpJob/wiki

        private DashboardOptions adminOptions = new DashboardOptions()
        {
            DisplayStorageConnectionString = false,
            IsReadOnlyFunc = context => false,
            IgnoreAntiforgeryToken = true,
            Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions() {
            RequireSsl=false,
            SslRedirect=false,
            LoginCaseSensitive=true,
            Users=new []{
                 new BasicAuthAuthorizationUser
                 {
                    Login="admin",
                    PasswordClear="123"
                 },
                 new BasicAuthAuthorizationUser
                 {
                    Login="fanyou",
                    PasswordClear="123"
                 }
            }
            })}
        };

        private DashboardOptions readOptions = new DashboardOptions()
        {
            IgnoreAntiforgeryToken = true,
            DisplayStorageConnectionString = false,
            IsReadOnlyFunc = context => true
        };
        private void ConfigurationHangfire(IGlobalConfiguration globalConfiguration)
        {
            globalConfiguration.UseStorage(
                new MySqlStorage(Configuration["jobdb"], new MySqlStorageOptions()
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = false,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(2),
                    TablesPrefix = "Fairhr_"
                }))
                .UseConsole()
                .UseHangfireHttpJob(new HangfireHttpJobOptions()
                {
                    DashboardName = "泛亚统一任务调度平台",
                    DashboardTitle = "调度平台",
                    DashboardFooter = string.Empty,
                    MailOption = new MailOption()
                    {
                        Server = "smtp.qq.com",
                        Port = 465,
                        UseSsl = true,
                        User = "510423039@qq.com",
                        Password = "vkskogjacsqabjgd"
                    }
                });
        }
        #endregion
    }
}
