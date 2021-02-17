using Server.Interfaces;
using Server.Models.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Server.DI
{
    public class MailSender : IMailSender
    {
        public async Task SendMail(MailClass mailClass)
        {
            using(MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(mailClass.FromMail);
                message.To.Add(new MailAddress(mailClass.ToMail));
                message.Subject = mailClass.Subject;
                message.Body = mailClass.Body;
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(mailClass.Attachment.ToString());
                writer.Flush();
                stream.Position = 0;
                Attachment att = new Attachment(stream, "data.csv");
                message.Attachments.Add(att);
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(mailClass.FromMail, mailClass.FromMailPassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }
            }
        }
    }
}
