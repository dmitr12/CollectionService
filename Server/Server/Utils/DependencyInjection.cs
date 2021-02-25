using Microsoft.Extensions.DependencyInjection;
using Server.DI;
using Server.Interfaces;
using Server.Managers;
using Server.Models.Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Utils
{
    public static class DependencyInjection
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<IGeneratorToken, GeneratorToken>();
            services.AddScoped<UserManager>();
            services.AddScoped<TaskManager>();
            services.AddScoped<BaseApiManager>();
            services.AddScoped<ConverterCsv>();
            services.AddScoped<PersonRepository>();
            services.AddScoped<ApiRepository>();
            services.AddScoped<TaskRepository>();
            services.AddScoped<JobFactory>();
            services.AddScoped<MailJob>();
            services.AddScoped<IMailSender, MailSender>();
        }
    }
}
