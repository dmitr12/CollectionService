using Quartz;
using Quartz.Impl;
using Server.Interfaces;
using Server.Models.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Quartz
{
    public class JobScheduler
    {
        public static async void StartJob(IMailSender mailSender, MailClass mailClass)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            IJobDetail jobDetail = JobBuilder.Create<MailJob>().WithIdentity(new JobKey("1")).Build();
            jobDetail.JobDataMap["MailSender"] = mailSender;
            jobDetail.JobDataMap["MailClass"] = mailClass;
            ITrigger trigger = TriggerBuilder.Create().StartNow().WithSimpleSchedule(opt => opt.WithIntervalInMinutes(2)
            .RepeatForever()).Build();
            await scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
