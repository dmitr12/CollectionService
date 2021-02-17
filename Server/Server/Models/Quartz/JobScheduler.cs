using Quartz;
using Quartz.Impl;
using Server.Interfaces;
using Server.Managers;
using Server.Models.DB_Models;
using Server.Models.Mail;
using Server.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Quartz
{
    public class JobScheduler
    {
        public static async void StartJob<T>(IMailSender mailSender, MailClass mailClass, TaskModel model, int idTask, TaskManager taskManager, Server.Models.DB_Models.Api api) where T:class
        {
            T obj = Activator.CreateInstance<T>();
            DateTime dt = Convert.ToDateTime(model.StartTask);
            DateTimeOffset dto;
            DateTimeOffset.TryParse(dt.ToString(), out dto);
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            IJobDetail jobDetail = JobBuilder.Create<MailJob>().WithIdentity(new JobKey(idTask.ToString())).Build();
            jobDetail.JobDataMap["MailSender"] = mailSender;
            jobDetail.JobDataMap["MailClass"] = mailClass;
            jobDetail.JobDataMap["TaskId"] = idTask;
            jobDetail.JobDataMap["TaskManager"] = taskManager;
            jobDetail.JobDataMap["ObjForApi"] = obj;
            jobDetail.JobDataMap["Api"] = api;
            TriggerBuilder triggerBuilder = TriggerBuilder.Create().WithSimpleSchedule(opt => opt.WithIntervalInMinutes(model.PeriodicityMin).RepeatForever());
            if (dt.AddMinutes(10) > DateTime.Now && dt>DateTime.Now)
                triggerBuilder.StartAt(dto);
            else
                triggerBuilder.StartNow();
            await scheduler.ScheduleJob(jobDetail, triggerBuilder.Build());
        }

        public static async void DeleteJob(string jobKey)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.DeleteJob(new JobKey(jobKey));
        }
    }
}
