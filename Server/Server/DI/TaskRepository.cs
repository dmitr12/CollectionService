using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Server.Interfaces;
using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DI
{
    public class TaskRepository:BaseRepository<Job>, ITaskRepository
    {
        public TaskRepository(IConfiguration config) : base(config) { }

        protected override void AddItemParameters(Job obj, SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText= "insert into Tasks(Name, Description, StartTask, PeriodicityMin, LastExecution, FilterText, UserId, ApiId, CountExecutions) " +
                    "values(@Name, @Description, @StartTask, @PeriodicityMin, @LastExecution, @FilterText, @UserId, @ApiId, @CountExecutions); select last_insert_rowid()";
            sqliteCommand.Parameters.AddWithValue("Name", obj.Name);
            sqliteCommand.Parameters.AddWithValue("Description", obj.Description);
            sqliteCommand.Parameters.AddWithValue("StartTask", obj.StartTask);
            sqliteCommand.Parameters.AddWithValue("PeriodicityMin", Convert.ToInt64(obj.PeriodicityMin));
            sqliteCommand.Parameters.AddWithValue("LastExecution", obj.LastExecution);
            sqliteCommand.Parameters.AddWithValue("FilterText", obj.FilterText);
            sqliteCommand.Parameters.AddWithValue("UserId", Convert.ToInt64(obj.UserId));
            sqliteCommand.Parameters.AddWithValue("ApiId", Convert.ToInt64(obj.ApiId));
            sqliteCommand.Parameters.AddWithValue("CountExecutions", Convert.ToInt64(obj.CountExecutions));
        }

        protected override void DeleteItemParameters(int id, SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "delete from tasks where TaskId=@TaskId";
            sqliteCommand.Parameters.AddWithValue("@TaskId", id);
        }

        protected override void UpdateItemParameters(Job obj, SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "update Tasks set Name=@Name, Description=@Description, StartTask=@StartTask, PeriodicityMin=@PeriodicityMin, LastExecution=@LastExecution, FilterText=@FilterText," +
                " UserId=@UserId, ApiId=@ApiId, CountExecutions=@CountExecutions where TaskId=@TaskId";
            sqliteCommand.Parameters.AddWithValue("TaskId", obj.TaskId);
            sqliteCommand.Parameters.AddWithValue("Name", obj.Name);
            sqliteCommand.Parameters.AddWithValue("Description", obj.Description);
            sqliteCommand.Parameters.AddWithValue("StartTask", obj.StartTask);
            sqliteCommand.Parameters.AddWithValue("PeriodicityMin", Convert.ToInt64(obj.PeriodicityMin));
            sqliteCommand.Parameters.AddWithValue("LastExecution", obj.LastExecution);
            sqliteCommand.Parameters.AddWithValue("FilterText", obj.FilterText);
            sqliteCommand.Parameters.AddWithValue("UserId", Convert.ToInt64(obj.UserId));
            sqliteCommand.Parameters.AddWithValue("ApiId", Convert.ToInt64(obj.ApiId));
            sqliteCommand.Parameters.AddWithValue("CountExecutions", Convert.ToInt64(obj.CountExecutions));
        }

        protected override void SelectItemParameteres(SqliteCommand sqliteCommand, int id)
        {
            sqliteCommand.CommandText = "select TaskId, Name, Description, StartTask, PeriodicityMin, LastExecution, " +
                "FilterText, UserId, ApiId, CountExecutions from tasks where TaskId=@TaskId";
            sqliteCommand.Parameters.AddWithValue("TaskId", Convert.ToInt64(id));
        }

        protected override async Task<Job> GetItem(SqliteDataReader reader)
        {
            Job task = null;
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    task = new Job
                    {
                        TaskId = Convert.ToInt32(reader["TaskId"].ToString()),
                        Name = reader["Name"].ToString(),
                        Description=reader["Description"].ToString(),
                        StartTask = reader["StartTask"].ToString(),
                        PeriodicityMin = Convert.ToInt32(reader["PeriodicityMin"].ToString()),
                        LastExecution = reader["LastExecution"].ToString(),
                        FilterText = reader["FilterText"].ToString(),
                        UserId = Convert.ToInt32(reader["UserId"].ToString()),
                        ApiId = Convert.ToInt32(reader["ApiId"].ToString()),
                        CountExecutions = Convert.ToInt32(reader["CountExecutions"].ToString())
                    };
                }
            }
            return task;
        }

        protected override void SelectItemsParameteres(SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "select TaskId, Name, Description, StartTask, PeriodicityMin, LastExecution, " +
                "FilterText, UserId, ApiId, CountExecutions from tasks";
        }

        protected override async Task<IEnumerable<Job>> GetListItems(SqliteDataReader reader)
        {
            List<Job> tasks = new List<Job>();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    tasks.Add(new Job
                    {
                        TaskId = Convert.ToInt32(reader["TaskId"].ToString()),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        StartTask = reader["StartTask"].ToString(),
                        PeriodicityMin = Convert.ToInt32(reader["PeriodicityMin"].ToString()),
                        LastExecution = reader["LastExecution"].ToString(),
                        FilterText = reader["FilterText"].ToString(),
                        UserId = Convert.ToInt32(reader["UserId"].ToString()),
                        ApiId = Convert.ToInt32(reader["ApiId"].ToString()),
                        CountExecutions = Convert.ToInt32(reader["CountExecutions"].ToString())
                    });
                }
            }
            return tasks;
        }

        public IEnumerable<Job> GetTasksByUserId(int userId)
        {
            return GetAllItems().Result.Where(t => t.UserId == userId);
        }
    }
}
