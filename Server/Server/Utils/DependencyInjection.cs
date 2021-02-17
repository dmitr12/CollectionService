using Microsoft.Extensions.DependencyInjection;
using Server.DI;
using Server.Interfaces;
using Server.Managers;
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
            services.AddScoped<IDbHelper, SQLiteDBHelper>();
            services.AddScoped<IGeneratorToken, GeneratorToken>();
            services.AddScoped<UserManager>();
            services.AddScoped<TaskManager>();
            services.AddScoped<ConverterCsv>();
            services.AddScoped<IMailSender, MailSender>();
        }
    }
}
