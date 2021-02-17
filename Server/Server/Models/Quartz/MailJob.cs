using Quartz;
using Server.Interfaces;
using Server.Managers;
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
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            IMailSender mailSender = (IMailSender)jobDataMap.Get("MailSender");
            MailClass mailClass = (MailClass)jobDataMap.Get("MailClass");
            TaskManager taskManager = (TaskManager)jobDataMap.Get("TaskManager");
            var api = (Server.Models.DB_Models.Api)jobDataMap.Get("Api");
            int taskId = jobDataMap.GetInt("TaskId");
            Job task = taskManager.GetTaskById(taskId);
            var obj = jobDataMap.Get("ObjForApi");
            mailClass.Body = $"Данные на {DateTime.Now}";
            if(obj.GetType() == typeof(WeatherInfo))
                mailClass.Attachment = taskManager.GetStringForCsv(taskManager.GetFilteredData<WeatherInfo>(api.BaseUrl, api.FilterColumn, task.FilterText));
            await mailSender.SendMail(mailClass);
            task.CountExecutions++;
            task.LastExecution = DateTime.Now.ToString();
            taskManager.UpdateTaskInDb(task);
        }
    }
}
