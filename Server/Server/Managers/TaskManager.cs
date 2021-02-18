using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
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
        private readonly IDbHelper dbHelper;
        private readonly IMailSender mailSender;
        private readonly IConfiguration config;
        private readonly UserManager userManager;
        private readonly ConverterCsv converterCsv;

        public TaskManager(IDbHelper dbHelper, IMailSender mailSender, IConfiguration config, ConverterCsv converterCsv, UserManager userManager)
        {
            this.dbHelper = dbHelper;
            this.mailSender = mailSender;
            this.config = config;
            this.converterCsv = converterCsv;
            this.userManager = userManager;
        }

        public List<Api> GetListApi()
        {
            try
            {
                return dbHelper.GetData<Api>("select * from apies", null, null).Result;
            }
            finally
            {
                dbHelper.Close();
            }
        }

        private Api GetApiInfo(int apiId)
        {
            try
            {
               return dbHelper.GetData("select * from apies where apiId = @ApiId", new Api { ApiId = apiId }, new List<string> { "ApiId" }).Result[0];
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public void DeleteTaskFromBd(int idTask)
        {
            try
            {
                dbHelper.ExecuteQuery("delete from Tasks where TaskId = @TaskId", new Job { TaskId = idTask }, new List<string> { "TaskId" });
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public Job GetTaskById(int idTask)
        {
            try
            {
                return dbHelper.GetData("select * from Tasks where TaskId = @TaskId", new Job { TaskId = idTask }, new List<string> { "TaskId" }).Result[0];
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public void DeleteTaskFromDb(int idTask)
        {
            try
            {
                dbHelper.ExecuteQuery("delete from Tasks where TaskId=@TaskId", new Job { TaskId = idTask }, new List<string> { "TaskId" });
                JobScheduler.DeleteJob(idTask.ToString());
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public List<Job> GetTasksByUserId(int userId)
        {
            try
            {
                return dbHelper.GetData("select * from tasks where UserId=@UserId", new Job { UserId = userId }, new List<string> { "UserId" }).Result;
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public long AddTaskToDb(TaskModel taskViewModel, int userId)
        {
            try
            {
                string queryString = "insert into Tasks(Name, Description, StartTask, PeriodicityMin, LastExecution, FilterText, UserId, ApiId, CountExecutions) " +
                    "values(@Name, @Description, @StartTask, @PeriodicityMin, @LastExecution, @FilterText, @UserId, @ApiId, @CountExecutions); select last_insert_rowid()";
                Job task = new Job { Name = taskViewModel.Name, Description = taskViewModel.Description, StartTask = taskViewModel.StartTask, PeriodicityMin = taskViewModel.PeriodicityMin,
                    LastExecution = "", FilterText = taskViewModel.FilterText, UserId = userId, ApiId = taskViewModel.ApiId, CountExecutions = 0 };
                List<string> paramNames = new List<string>();
                paramNames.AddRange(new string[] { "Name", "Description", "StartTask", "PeriodicityMin", "LastExecution", "FilterText", "UserId", "ApiId", "CountExecutions" });
                return (long)dbHelper.ExecuteQuery(queryString, task, paramNames).Result;
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public void UpdateTaskInDb(Job task)
        {
            try
            {
                string queryString = "update Tasks set Name=@Name, Description=@Description, StartTask=@StartTask, PeriodicityMin=@PeriodicityMin, LastExecution=@LastExecution, FilterText=@FilterText," +
                " UserId=@UserId, ApiId=@ApiId, CountExecutions=@CountExecutions where TaskId=@TaskId";
                List<string> paramNames = new List<string>();
                paramNames.AddRange(new string[] { "Name", "Description", "StartTask", "PeriodicityMin", "LastExecution", "FilterText", "UserId", "ApiId", "CountExecutions", "TaskId" });
                dbHelper.ExecuteQuery(queryString, task, paramNames);
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public string UpdateTask<T>(TaskModel taskModel, User user)where T : class
        {
            try
            {
                Api apiInfo = GetApiInfo(taskModel.ApiId);
                T dataFromApi = GetFilteredData<T>(apiInfo.BaseUrl, apiInfo.FilterColumn, taskModel.FilterText);
                if (dataFromApi == null)
                    return "Неверный параметр фильтра";
                UpdateTaskInDb(new Job
                {
                    TaskId = taskModel.TaskId,
                    ApiId = taskModel.ApiId,
                    CountExecutions = 0,
                    Description = taskModel.Description,
                    FilterText = taskModel.FilterText,
                    LastExecution = "",
                    Name = taskModel.Name,
                    PeriodicityMin = taskModel.PeriodicityMin,
                    StartTask = taskModel.StartTask,
                    UserId = user.UserId
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

        public string AddTask<T>(TaskModel taskModel, User user) where T: class
        {
            int addedTaskId = -1;
            try
            {
                Api apiInfo = GetApiInfo(taskModel.ApiId);
                T dataFromApi = GetFilteredData<T>(apiInfo.BaseUrl, apiInfo.FilterColumn, taskModel.FilterText);
                if (dataFromApi == null)
                    return "Неверный параметр фильтра";
                addedTaskId = (int)AddTaskToDb(taskModel, user.UserId);
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
                DeleteTaskFromBd(addedTaskId);
                JobScheduler.DeleteJob(addedTaskId.ToString());
                return "Возникла ошибка при добавлении задачи";
            }
        }

        public void DeleteTask(int taskId)
        {
            DeleteTaskFromBd(taskId);
        }

        public T GetFilteredData<T>(string queryString, string filterColumn, string filterParameterValue) where T: class
        {
            T data = null;
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
                data = JsonConvert.DeserializeObject<T>(response.Content);
            return data;
        }

        public StringBuilder GetStringForCsv<T>(T obj)
        {
            return converterCsv.ConvertToCsv(obj);
        }

        public void StartAllJobs()
        {
            try
            {
                List<Job> tasks = dbHelper.GetData<Job>("select * from tasks", null, null).Result;
                foreach(Job task in tasks)
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
                        PeriodicityMin = task.PeriodicityMin,
                        StartTask = task.StartTask
                    };
                    if(apiInfo.ApiId == 2)
                        JobScheduler.StartJob<WeatherInfo>(mailSender, mailClass, taskModel, task.TaskId, this, apiInfo);
                    if(apiInfo.ApiId == 3)
                        JobScheduler.StartJob<NumbersInfo>(mailSender, mailClass, taskModel, task.TaskId, this, apiInfo);
                    if (apiInfo.ApiId == 4)
                        JobScheduler.StartJob<JokeInfo>(mailSender, mailClass, taskModel, task.TaskId, this, apiInfo);
                }
            }
            finally
            {
                dbHelper.Close();
            }
        }
    }
}
