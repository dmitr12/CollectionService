using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using Quartz;
using Quartz.Impl;
using Server.Managers;
using Server.Models.DB_Models;
using Server.Models.Mail;
using Server.Models.View_Models;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Quartz
{
    public class JobScheduler
    {
        private static IScheduler scheduler = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string jobName = "MailJob";

        public static async void StartJob(MailClass mailClass, TaskModel model, int idTask, Api api)
        {
            try
            {
                TriggerBuilder triggerBuilder = TriggerBuilder.Create().ForJob(jobName).WithIdentity(idTask.ToString()).WithCronSchedule(model.Periodicity);
                if (DateTime.Parse(model.StartTask) > DateTime.Now)
                    triggerBuilder.StartAt(DateTimeOffset.Parse(model.StartTask));
                else
                    triggerBuilder.StartNow();
                ITrigger trigger = triggerBuilder.Build();
                trigger.JobDataMap["MailClass"] = mailClass;
                trigger.JobDataMap["TaskId"] = idTask;
                trigger.JobDataMap["Api"] = api;
                await scheduler.ScheduleJob(trigger);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void DeleteJob(string triggerKey)
        {
            scheduler.UnscheduleJob(new TriggerKey(triggerKey));
        }

        public static async void StartAllTasks(TaskManager taskManager, IOptions<ThreadCountConfiguration> options, JobFactory jobFactory)
        {
            NameValueCollection coutnThread = new NameValueCollection { { $"{options.Value.parameterName}", $"{options.Value.countThreads}" } };
            var schedulerFactory = new StdSchedulerFactory(coutnThread);
            scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.JobFactory = jobFactory;
            await scheduler.Start();
            var mailJob = JobBuilder.Create<MailJob>().StoreDurably().WithIdentity(jobName).Build();
            await scheduler.AddJob(mailJob, true);
            taskManager.StartAllTasks();
        }
    }
}
