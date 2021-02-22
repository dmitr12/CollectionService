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
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobDataMap = context.Trigger.JobDataMap;
            IMailSender mailSender = (IMailSender)jobDataMap.Get("MailSender");
            MailClass mailClass = (MailClass)jobDataMap.Get("MailClass");
            TaskManager taskManager = (TaskManager)jobDataMap.Get("TaskManager");
            var api = (Server.Models.DB_Models.Api)jobDataMap.Get("Api");
            int taskId = jobDataMap.GetInt("TaskId");
            Job task = taskManager.GetTaskById(taskId);
            var obj = jobDataMap.Get("ObjForApi");
            DateTime dt = DateTime.Now;
            mailClass.Body = $"Данные на {dt}";
            if(obj.GetType() == typeof(WeatherInfo))
                mailClass.Attachment = taskManager.GetStringForCsv(taskManager.GetFilteredData<WeatherInfo>(api.BaseUrl, api.FilterColumn, task.FilterText));
            else if(obj.GetType()==typeof(NumbersInfo))
                mailClass.Attachment = taskManager.GetStringForCsv(taskManager.GetFilteredData<NumbersInfo>(api.BaseUrl, api.FilterColumn, task.FilterText));
            else if (obj.GetType() == typeof(JokeInfo))
                mailClass.Attachment = taskManager.GetStringForCsv(taskManager.GetFilteredData<JokeInfo>(api.BaseUrl, api.FilterColumn, task.FilterText));
            await mailSender.SendMail(mailClass);
            task.LastExecution = dt.ToString();
            task.CountExecutions++;
            await taskManager.UpdateTaskInDb(task);
            await taskManager.UpdateCountCompletedUserTasks(task.UserId);
        }
    }
}
