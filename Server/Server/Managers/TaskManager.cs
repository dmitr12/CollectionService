using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using Server.Converters;
using Server.DI;
using Server.Interfaces;
using Server.Models.Api;
using Server.Models.Api.JokeApi;
using Server.Models.Api.NumbersApi;
using Server.Models.Api.WeatherApi;
using Server.Models.DB_Models;
using Server.Models.Mail;
using Server.Models.View_Models;
using Server.Quartz;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Managers
{
    public class TaskManager
    {
        private readonly IConfiguration config;
        private readonly UserManager userManager;
        private readonly ConverterCsv converterCsv;
        private readonly TaskRepository taskRepository;
        private readonly BaseApiManager baseApiManager;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public TaskManager(IConfiguration config, ConverterCsv converterCsv, UserManager userManager,
            TaskRepository taskRepository, BaseApiManager baseApiManager)
        {
            this.config = config;
            this.converterCsv = converterCsv;
            this.userManager = userManager;
            this.taskRepository = taskRepository;
            this.baseApiManager = baseApiManager;
        }

        public Job GetTaskById(int idTask)
        {
            return taskRepository.GetItemById(idTask).Result;
        }

        public List<Job> GetTasksByUserId(int userId)
        {
            return taskRepository.GetTasksByUserId(userId).ToList();
        }

        public List<UserTasksInfo> GetStatistics()
        {
            return userManager.GetAllUsers().Select(user => new UserTasksInfo
            {
                UserId = user.UserId,
                UserName = user.UserName,
                CountCompletedTasks = user.CountCompletedTasks,
                CountActiveTasks = GetTasksByUserId(user.UserId).Count
            }).ToList();
        }

        public void UpdateTaskInDb(Job task)
        {
            taskRepository.UpdateItem(task).Wait();
        }

        public IActionResult UpdateTask(TaskModel taskModel, User user)
        {
            try
            {
                var apiManager = baseApiManager.GetApiManager(taskModel.ApiId);
                Api apiInfo = apiManager.GetApiInfo();
                ApiBase dataFromApi = apiManager.GetFilteredData(apiInfo.BaseUrl, apiInfo.FilterColumn, taskModel.FilterText);
                if (dataFromApi == null)
                    return new StatusCodeResult(400);
                UpdateTaskInDb(new Job
                {
                    TaskId = taskModel.TaskId, ApiId = taskModel.ApiId, Description = taskModel.Description, FilterText = taskModel.FilterText,
                    LastExecution = "", Name = taskModel.Name, Periodicity = taskModel.Periodicity, StartTask = taskModel.StartTask, UserId = user.UserId
                });
                JobScheduler.DeleteJob(taskModel.TaskId.ToString());
                JobScheduler.StartJob(new MailClass
                {
                    FromMail = config.GetSection("Mail").Value,
                    FromMailPassword = config.GetSection("MailPassword").Value,
                    ToMail = user.Email,
                    Subject = "Запрос данных",
                    Body = $"Обновленные данные"
                }, taskModel, taskModel.TaskId, apiInfo);
                return new StatusCodeResult(200);
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
                return new StatusCodeResult(500);
            }
        }

        public IActionResult AddTask(TaskModel taskModel, User user)
        {
            int addedTaskId = -1;
            try
            {
                var apiManager = baseApiManager.GetApiManager(taskModel.ApiId);
                Api apiInfo = apiManager.GetApiInfo();
                ApiBase dataFromApi = apiManager.GetFilteredData(apiInfo.BaseUrl, apiInfo.FilterColumn, taskModel.FilterText);
                if (dataFromApi == null)
                    return new StatusCodeResult(400);
                addedTaskId = (int)taskRepository.AddItem(new Job { ApiId = taskModel.ApiId, Description = taskModel.Description, 
                    FilterText = taskModel.FilterText, LastExecution = "", Name = taskModel.Name,
                    Periodicity = taskModel.Periodicity, StartTask = taskModel.StartTask, UserId = user.UserId }).Result;
                JobScheduler.StartJob(new MailClass
                {
                    FromMail = config.GetSection("Mail").Value,
                    FromMailPassword = config.GetSection("MailPassword").Value,
                    ToMail = user.Email,
                    Subject = "Запрос данных",
                    Body = $"Обновленные данные"
                }, taskModel, addedTaskId, apiInfo);
                return new StatusCodeResult(200);
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
                return new StatusCodeResult(500);
            }
        }

        public void DeleteTask(int taskId)
        {
            taskRepository.DeleteItem(taskId).Wait();
            JobScheduler.DeleteJob(taskId.ToString());
        }

        public StringBuilder GetStringForCsv(ApiBase obj)
        {
            return converterCsv.ConvertToCsv(obj);
        }

        public void StartAllTasks()
        {
            try
            {
                List<Job> tasks = taskRepository.GetAllItems().Result.ToList();
                foreach (Job task in tasks)
                {
                    var apiManager = baseApiManager.GetApiManager(task.ApiId);
                    Api apiInfo = apiManager.GetApiInfo();
                    User user = userManager.GetUserById(task.UserId);
                    MailClass mailClass = new MailClass
                    {
                        FromMail = config.GetSection("Mail").Value,
                        FromMailPassword = config.GetSection("MailPassword").Value,
                        ToMail = user.Email,
                        Subject = "Запрос данных",
                        Body = $"Обновленные данные"
                    };
                    TaskModel taskModel = new TaskModel
                    {
                        ApiId = task.ApiId,
                        Description = task.Description,
                        FilterText = task.FilterText,
                        Name = task.Name,
                        Periodicity = task.Periodicity,
                        StartTask = task.StartTask
                    };
                    JobScheduler.StartJob(mailClass, taskModel, task.TaskId, apiInfo);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
