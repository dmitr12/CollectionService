using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Mail
{
    public class MailClass
    {
        public string FromMail { get; set; }
        public string FromMailPassword { get; set; }
        public string ToMail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public StringBuilder Attachment { get; set; }
    }
}
