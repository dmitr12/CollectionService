using Microsoft.Extensions.DependencyInjection;
using NLog;
using Quartz;
using Server.Interfaces;
using Server.Managers;
using Server.Models.Api.JokeApi;
using Server.Models.Api.NumbersApi;
using Server.Models.Api.WeatherApi;
using Server.Models.DB_Models;
using Server.Models.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Quartz
{
    public class MailJob : IJob
    {

        private Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceScopeFactory serviceScopeFactory;
        private IMailSender mailSender;
        private TaskManager taskManager;
        private BaseApiManager apiManager;

        public MailJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobDataMap = context.Trigger.JobDataMap;
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    mailSender = scope.ServiceProvider.GetService<IMailSender>();
                    taskManager = scope.ServiceProvider.GetService<TaskManager>();
                    apiManager = scope.ServiceProvider.GetService<BaseApiManager>();
                }
                MailClass mailClass = (MailClass)jobDataMap.Get("MailClass");
                var api = (Server.Models.DB_Models.Api)jobDataMap.Get("Api");
                int taskId = jobDataMap.GetInt("TaskId");
                Job task = taskManager.GetTaskById(taskId);
                DateTime dt = DateTime.Now;
                mailClass.Body = $"Данные на {dt}";
                mailClass.Attachment = taskManager.GetStringForCsv(apiManager.GetApiManager(api.ApiId).GetFilteredData(api.BaseUrl, api.FilterColumn, task.FilterText));
                await mailSender.SendMail(mailClass);
                task.LastExecution = dt.ToString();
                task.CountExecutions++;
                taskManager.UpdateTaskInDb(task);
                taskManager.UpdateCountCompletedUserTasks(task.UserId);
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
