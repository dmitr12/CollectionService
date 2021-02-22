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
using Server.Models.Api.JokeApi;
using Server.Models.Api.NumbersApi;
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

        private int UserId => int.Parse(User.Claims.Single(cl => cl.Type == ClaimTypes.NameIdentifier).Value);

        public TaskController(TaskManager taskManager, UserManager userManager)
        {
            this.taskManager = taskManager;
            this.userManager = userManager;
        }

        [HttpGet("GetTasksByUserId/{userId}")]
        public IActionResult GetTasksByUserId(int userId)
        {
            return Ok(taskManager.GetTasksByUserId(userId));
        }

        [HttpGet("GetTaskById/{taskId}")]
        public IActionResult GetTaskById(int taskId)
        {
            return Ok(taskManager.GetTaskById(taskId));
        }


        [HttpGet("GetStatistics")]
        [Authorize(Roles = "2")] //Admin
        public IActionResult GetStatistics()
        {
            return Ok(taskManager.GetStatistics());
        }


        [HttpDelete("DeleteTask/{idTask}")]
        [Authorize(Roles = "1")] //User
        public async Task<IActionResult> DeleteTask(int idTask)
        {
            await taskManager.DeleteTask(idTask);
            return Ok();
        }

        [HttpGet("Apies")]
        public IActionResult GetAllApies()
        {
            return Ok(taskManager.GetListApi());
        }

        [HttpPost("AddTask")]
        [Authorize(Roles = "1")] //User
        public IActionResult AddTask(TaskModel model)
        {
            User user = userManager.GetUserById(UserId);
            switch (model.ApiId)
            {
                case (int)ApiesId.ApiWeather: return Ok(taskManager.AddTask<WeatherInfo>(model, user));
                case (int)ApiesId.ApiNumber: return Ok(taskManager.AddTask<NumbersInfo>(model, user));
                case (int)ApiesId.ApiJoke: return Ok(taskManager.AddTask<JokeInfo>(model, user));
            }
            return BadRequest();
        }

        [HttpPut("UpdateTask")]
        [Authorize(Roles = "1")] //User
        public IActionResult UpdateTask(TaskModel model)
        {
            User user = userManager.GetUserById(UserId);
            switch (model.ApiId)
            {
                case (int)ApiesId.ApiWeather: return Ok(taskManager.UpdateTask<WeatherInfo>(model, user));
                case (int)ApiesId.ApiNumber: return Ok(taskManager.UpdateTask<NumbersInfo>(model, user));
                case (int)ApiesId.ApiJoke: return Ok(taskManager.UpdateTask<JokeInfo>(model, user));
            }
            return BadRequest();
        }
    }
}
