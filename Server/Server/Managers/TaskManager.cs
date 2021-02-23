using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Server.DI;
using Server.Interfaces;
using Server.Models.Api.JokeApi;
using Server.Models.Api.NumbersApi;
using Server.Models.Api.WeatherApi;
using Server.Models.DB_Models;
using Server.Models.Mail;
using Server.Models.Quartz;
using Server.Models.View_Models;
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
        private readonly IMailSender mailSender;
        private readonly IConfiguration config;
        private readonly UserManager userManager;
        private readonly ConverterCsv converterCsv;
        private readonly TaskRepository taskRepository;
        private readonly ApiRepository apiRepository;

        public TaskManager(IMailSender mailSender, IConfiguration config, ConverterCsv converterCsv, UserManager userManager,
            TaskRepository taskRepository, ApiRepository apiRepository)
        {
            this.mailSender = mailSender;
            this.config = config;
            this.converterCsv = converterCsv;
            this.userManager = userManager;
            this.taskRepository = taskRepository;
            this.apiRepository = apiRepository;
        }

        public List<Api> GetListApi()
        {
            return apiRepository.GetAllItems().Result.ToList();
        }

        private Api GetApiInfo(int apiId)
        {
            return apiRepository.GetItemById(apiId).Result;
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

        public async Task UpdateTaskInDb(Job task)
        {
            await taskRepository.UpdateItem(task);
        }

        public async Task UpdateCountCompletedUserTasks(int userId)
        {
            await userManager.InceremntUserCompletedTasks(userId);
        }

        public async Task<string> UpdateTask<T>(TaskModel taskModel, User user)where T : class
        {
            try
            {
                Api apiInfo = GetApiInfo(taskModel.ApiId);
                object dataFromApi = GetFilteredData(apiInfo.BaseUrl, apiInfo.FilterColumn, taskModel.FilterText, typeof(T));
                if (dataFromApi == null)
                    return "Неверный параметр фильтра";
                await UpdateTaskInDb(new Job
                {
                    TaskId = taskModel.TaskId, ApiId = taskModel.ApiId, CountExecutions = 0, Description = taskModel.Description, FilterText = taskModel.FilterText,
                    LastExecution = "", Name = taskModel.Name, Periodicity = taskModel.Periodicity, StartTask = taskModel.StartTask, UserId = user.UserId
                });
                JobScheduler.DeleteJob(taskModel.TaskId.ToString());
                JobScheduler.StartJob<T>(mailSender, new MailClass
                {
                    FromMail = config.GetSection("Mail").Value,
                    FromMailPassword = config.GetSection("MailPassword").Value,
                    ToMail = user.Email,
                    Subject = "Запрос данных",
                    Body = $"Обновленные данные"
                }, taskModel, taskModel.TaskId, this, apiInfo);
                return null;
            }
            catch
            {
                return "Возникла ошибка при изменении задачи";
            }
        }

        public async Task<string> AddTask<T>(TaskModel taskModel, User user) where T: class
        {
            int addedTaskId = -1;
            try
            {
                Api apiInfo = GetApiInfo(taskModel.ApiId);
                object dataFromApi = GetFilteredData(apiInfo.BaseUrl, apiInfo.FilterColumn, taskModel.FilterText, typeof(T));
                if (dataFromApi == null)
                    return "Неверный параметр фильтра";
                addedTaskId = (int)taskRepository.AddItem(new Job { ApiId = taskModel.ApiId, CountExecutions = 0, Description = taskModel.Description, 
                    FilterText = taskModel.FilterText, LastExecution = "", Name = taskModel.Name,
                    Periodicity = taskModel.Periodicity, StartTask = taskModel.StartTask, UserId = user.UserId }).Result;
                JobScheduler.StartJob<T>(mailSender, new MailClass
                {
                    FromMail = config.GetSection("Mail").Value,
                    FromMailPassword = config.GetSection("MailPassword").Value,
                    ToMail = user.Email,
                    Subject = "Запрос данных",
                    Body = $"Обновленные данные"
                }, taskModel, addedTaskId, this, apiInfo);
                return null;
            }
            catch
            {
                await taskRepository.DeleteItem(addedTaskId);
                JobScheduler.DeleteJob(addedTaskId.ToString());
                return "Возникла ошибка при добавлении задачи";
            }
        }

        public async Task DeleteTask(int taskId)
        {
            await taskRepository.DeleteItem(taskId);
            JobScheduler.DeleteJob(taskId.ToString());
        }

        public object GetFilteredData(string queryString, string filterColumn, string filterParameterValue, Type type)
        {
            StringBuilder sb = new StringBuilder(queryString);
            if (filterColumn == "/")
            {
                if (queryString.Contains('?'))
                {
                    int index = queryString.IndexOf('?');
                    sb.Insert(index, $"{filterColumn}{filterParameterValue}");
                }
                else
                    sb.Append(filterParameterValue);
            }
            else
                sb.Append($"&{filterColumn}={filterParameterValue}");
            var client = new RestClient(sb.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type","application/json; charset=utf-8");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                switch (type.Name)
                {
                    case "WeatherInfo": return JsonConvert.DeserializeObject<WeatherInfo>(response.Content);
                    case "NumbersInfo": return JsonConvert.DeserializeObject<NumbersInfo>(response.Content);
                    case "JokeInfo": return JsonConvert.DeserializeObject<JokeInfo>(response.Content);
                }
            }
            return null;
        }

        public StringBuilder GetStringForCsv<T>(T obj)
        {
            return converterCsv.ConvertToCsv(obj);
        }

        public void StartAllJobs()
        {
            List<Job> tasks = taskRepository.GetAllItems().Result.ToList();
            foreach (Job task in tasks)
            {
                Api apiInfo = GetApiInfo(task.ApiId);
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
                if (apiInfo.ApiId == (int)ApiesId.ApiWeather)
                    JobScheduler.StartJob<WeatherInfo>(mailSender, mailClass, taskModel, task.TaskId, this, apiInfo);
                if (apiInfo.ApiId == (int)ApiesId.ApiNumber)
                    JobScheduler.StartJob<NumbersInfo>(mailSender, mailClass, taskModel, task.TaskId, this, apiInfo);
                if (apiInfo.ApiId == (int)ApiesId.ApiJoke)
                    JobScheduler.StartJob<JokeInfo>(mailSender, mailClass, taskModel, task.TaskId, this, apiInfo);
            }
        }


    }
}
