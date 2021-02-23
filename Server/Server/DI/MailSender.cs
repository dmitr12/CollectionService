using Microsoft.Extensions.Options;
using NLog;
using Server.Interfaces;
using Server.Models.Mail;
using Server.Utils;
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

        private readonly IOptions<SmtpClientParameters> smtpClientParameters;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public MailSender(IOptions<SmtpClientParameters> options)
        {
            smtpClientParameters = options;
        }

        public async Task SendMail(MailClass mailClass)
        {
            try
            {
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(mailClass.FromMail);
                    message.To.Add(new MailAddress(mailClass.ToMail));
                    message.Subject = mailClass.Subject;
                    message.Body = mailClass.Body;
                    if (mailClass.Attachment != null)
                    {
                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(mailClass.Attachment.ToString());
                        writer.Flush();
                        stream.Position = 0;
                        Attachment att = new Attachment(stream, "data.csv");
                        message.Attachments.Add(att);
                    }
                    using (SmtpClient smtp = new SmtpClient(smtpClientParameters.Value.Host, smtpClientParameters.Value.Port))
                    {
                        smtp.Credentials = new NetworkCredential(mailClass.FromMail, mailClass.FromMailPassword);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }         
        }
    }
}
