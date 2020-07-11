using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.HttpJob.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HangfireClientTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            //var serverUrl = "http://111.229.198.31:5000/job";
            var serverUrl = "http://localhost:8000/jobs";
            //下面用的是同步的方式，也可以使用异步： await HangfireJobClient.AddBackgroundJobAsync
            //var result = HangfireJobClient.AddBackgroundJob(serverUrl, new BackgroundJob
            //{
            //    JobName = "测试api",
            //    Method = "Get",
            //    Url = "http://localhost:5000/testaaa",
            //    SendSuccess = true,
            //    DelayFromMinutes = 1 //这里是延迟多长时间
            //}, new HangfireServerPostOption
            //{
            //    BasicUserName = "test",//这里是hangfire设置的basicauth
            //    BasicPassword = "123456"//这里是hangfire设置的basicauth
            //});

            var result = HangfireJobClient.AddRecurringJob(serverUrl, new RecurringJob()
            {
                JobName = "测试13",
                Method = "Post",
                Data = new { name = "aaa", age = 10 },
                Url = "http://localhost:5000/WeatherForecast/testpost",
         Cron = "40 17 * * *"
            }, new HangfireServerPostOption
            {
                BasicUserName = "admin",//这里是hangfire设置的basicauth
                BasicPassword = "123"//这里是hangfire设置的basicauth
            });
            return result.IsSuccess.ToString();

        }


        [HttpPost("testpost")]
        public void Post()
        {
            Console.WriteLine("testpost");

        }
    }
}
