using Quartz;
using Server.Interfaces;
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
            await mailSender.SendMail(mailClass);
        }
    }
}
