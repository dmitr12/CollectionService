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
    public class SQLiteDBHelper : IDbHelper
    {
        private readonly SqliteConnection con;

        public SQLiteDBHelper(IConfiguration config)
        {
            con = new SqliteConnection(config.GetConnectionString("SQLiteConnetion"));
        }

        public async Task Close()
        {
            await con.CloseAsync();
        }

        public async Task<int> ExecuteQuery<T>(string queryString, T objForParameters, List<string> parameterNames) where T: class
        {
            await con.OpenAsync();
            using(SqliteCommand command = AddParameters<T>(queryString, objForParameters, parameterNames, con))
            {
                int count = command.ExecuteNonQueryAsync().Result;
                return count;
            }
        }

        public async Task<List<T>> GetData<T>(string queryString, ObjectType objectType) where T: class
        {
            if (objectType == ObjectType.User)
            {
                List<User> users = await GetUsers(queryString);
                return users as List<T>;
            }
            //
            //Если objectType==Записи, то повторить считывание для типа Записи
            //
            return null;
        }

        public async Task<bool> HasRows<T>(string queryString, T objForParameters, List<string> parameterNames) where T: class
        {
            await con.OpenAsync();
            using(SqliteCommand command = AddParameters<T>(queryString, objForParameters, parameterNames, con))
            {
                var reader = command.ExecuteReaderAsync().Result;
                if (reader.HasRows)
                    return true;
                return false;
            }
        }

        private async Task<List<User>> GetUsers(string queryString)
        {
            List<User> users = new List<User>();
            await con.OpenAsync();
            using (SqliteCommand command = new SqliteCommand(queryString, con))
            {
                SqliteDataReader reader = command.ExecuteReaderAsync().Result;
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        UserId = reader.GetInt32(0),
                        UserName = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3)
                    });
                }
            }
            return users;
        }

        private SqliteCommand AddParameters<T>(string queryString, T obj, List<string> parameterNames, SqliteConnection connection)
        {
            Type type = typeof(T);
            SqliteCommand command = new SqliteCommand(queryString, connection);
            if (parameterNames != null)
            {
                for (int i = 0; i < parameterNames.Count; i++)
                {
                    command.Parameters.AddWithValue($"@{parameterNames[i]}", type.GetProperty($"{parameterNames[i]}").GetValue(obj));
                }
            }  
            return command;
        }
    }
}
