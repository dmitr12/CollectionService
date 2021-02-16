using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Server.DI;
using Server.Interfaces;
using Server.Managers;
using Server.Models.Api.WeatherApi;
using Server.Models.DB_Models;
using Server.Models.Mail;
using Server.Models.Quartz;
using Server.Models.View_Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskManager taskManager;
        private readonly IConfiguration config;
        private readonly IMailSender mailSender;
        public TaskController(TaskManager taskManager, IConfiguration config, IMailSender mailSender)
        {
            this.taskManager = taskManager;
            this.config = config;
            this.mailSender = mailSender;
        }

        [HttpGet("Apies")]
        public IActionResult GetAllApies()
        {
            return Ok(taskManager.GetListApi());
        }

        [HttpPost("AddTask")]
        //[Authorize(Roles = "1")]
        public IActionResult AddTask(/*TaskModel model*/)
        {
            JobScheduler.StartJob(mailSender, new MailClass
            {
                FromMail = config.GetSection("Mail").Value,
                FromMailPassword = config.GetSection("MailPassword").Value,
                ToMail = "dyrda.dmitrij@mail.ru",
                Subject = "asdas",
                Body = "11111"
            });
            return Ok();
        }
    }
}
