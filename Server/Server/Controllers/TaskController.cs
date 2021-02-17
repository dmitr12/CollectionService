using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
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
        private readonly UserManager userManager;

        static int count=0;

        private int UserId => int.Parse(User.Claims.Single(cl => cl.Type == ClaimTypes.NameIdentifier).Value);

        public TaskController(TaskManager taskManager, UserManager userManager)
        {
            this.taskManager = taskManager;
            this.userManager = userManager;
            count++;
        }

        [HttpGet("GetTasksByUserId/{userId}")]
        public IActionResult GetTasksByUserId(int userId)
        {
            return Ok(taskManager.GetTasksByUserId(userId));
        }

        [HttpDelete("DeleteTask/{idTask}")]
        //[Authorize(Roles = "1")]
        public IActionResult DeleteTask(int idTask)
        {
            taskManager.DeleteTask(idTask);
            return Ok();
        }

        [HttpGet("Apies")]
        public IActionResult GetAllApies()
        {
            return Ok(taskManager.GetListApi());
        }

        [HttpPost("AddTask")]
        [Authorize(Roles = "1")]
        public IActionResult AddTask(TaskModel model)
        {
            User user = userManager.GetUserById(UserId);
            return Ok(new { msg = taskManager.AddTask<WeatherInfo>(model, user) });
        }
    }
}
