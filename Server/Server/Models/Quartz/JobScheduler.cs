using Quartz;
using Quartz.Impl;
using Server.Interfaces;
using Server.Managers;
using Server.Models.DB_Models;
using Server.Models.Mail;
using Server.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Quartz
{
    public class JobScheduler
    {

        private static IScheduler scheduler = null;

        public static async void StartJob<T>(IMailSender mailSender, MailClass mailClass, TaskModel model, int idTask, TaskManager taskManager, Server.Models.DB_Models.Api api) where T:class
        {
            T obj = Activator.CreateInstance<T>();
            DateTime dt = Convert.ToDateTime(model.StartTask);
            DateTimeOffset dto;
            DateTimeOffset.TryParse(dt.ToString(), out dto);
            IScheduler scheduler = GetIntanceScheduler();
            IJobDetail jobDetail = null;
            if(!scheduler.IsStarted)
            await scheduler.Start();
            TriggerBuilder triggerBuilder = TriggerBuilder.Create().ForJob(new JobKey("MailJob")).WithIdentity(idTask.ToString()).WithSimpleSchedule(opt =>
             opt.WithIntervalInMinutes(model.PeriodicityMin).RepeatForever().WithMisfireHandlingInstructionFireNow());
            if (dt.AddMinutes(10) > DateTime.Now && dt > DateTime.Now)
                triggerBuilder.StartAt(dto);
            else
                triggerBuilder.StartNow();
            ITrigger trigger = triggerBuilder.Build();
            trigger.JobDataMap["MailSender"] = mailSender;
            trigger.JobDataMap["MailClass"] = mailClass;
            trigger.JobDataMap["TaskId"] = idTask;
            trigger.JobDataMap["TaskManager"] = taskManager;
            trigger.JobDataMap["ObjForApi"] = obj;
            trigger.JobDataMap["Api"] = api;
            jobDetail = scheduler.GetJobDetail(new JobKey("MailJob")).Result;
            if (jobDetail==null)
            {
                jobDetail = JobBuilder.Create<MailJob>().WithIdentity("MailJob").Build();
                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            else
            {
                await scheduler.ScheduleJob(trigger);
            }
        }

        public static void DeleteJob(string jobKey)
        {
            scheduler = GetIntanceScheduler();
            scheduler.UnscheduleJob(new TriggerKey(jobKey));
        }

        private static IScheduler GetIntanceScheduler()
        {
            if (scheduler == null)
            {
                NameValueCollection coutnThread = new NameValueCollection { { "quartz.threadPool.threadCount", "15" } };
                var schedulerFactory = new StdSchedulerFactory(coutnThread);
                scheduler = schedulerFactory.GetScheduler().Result;
                return scheduler;
            }
            return scheduler;
        }

        public static void StartAllTasks(TaskManager taskManager)
        {
            taskManager.StartAllJobs();
        }
    }
}
