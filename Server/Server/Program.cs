using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Models.Quartz;
using Server.Managers;
using Microsoft.Extensions.Options;
using Server.Utils;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            JobScheduler.SetIntanceScheduler(host.Services.CreateScope().ServiceProvider.GetService<IOptions<ThreadCountConfiguration>>());
            JobScheduler.StartAllTasks(host.Services.CreateScope().ServiceProvider.GetService<TaskManager>());
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
