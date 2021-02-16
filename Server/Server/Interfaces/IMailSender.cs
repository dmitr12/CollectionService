using Server.Models.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IMailSender
    {
        Task SendMail(MailClass mailClass);
    }
}
