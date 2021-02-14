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

        public async Task<List<T>> GetData<T>(string queryString, T objForParameters, List<string> parameterNames) where T: class
        {
            if (typeof(T) == typeof(User))
            {
                List<User> users = await GetUsers(queryString, objForParameters as User, parameterNames);
                return users as List<T>;
            }
            if(typeof(T) == typeof(Role))
            {
                List<Role> roles = await GetRoles(queryString, objForParameters as Role, parameterNames);
                return roles as List<T>;
            }
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

        private async Task<List<User>> GetUsers(string queryString, User user, List<string> parameterNames)
        {
            List<User> users = new List<User>();
            await con.OpenAsync();
            using (SqliteCommand command = AddParameters(queryString, user, parameterNames, con))
            {
                SqliteDataReader reader = command.ExecuteReaderAsync().Result;
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        UserId = reader.GetInt32(0),
                        UserName = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3),
                        RoleId = reader.GetInt32(4)
                    });
                }
            }
            return users;
        }

        private async Task<List<Role>> GetRoles(string queryString, Role user, List<string> parameterNames)
        {
            List<Role> roles = new List<Role>();
            await con.OpenAsync();
            using (SqliteCommand command = AddParameters(queryString, user, parameterNames, con))
            {
                SqliteDataReader reader = command.ExecuteReaderAsync().Result;
                while (await reader.ReadAsync())
                {
                    roles.Add(new Role
                    {
                        RoleId = reader.GetInt32(0),
                        RoleName = reader.GetString(1),
                    });
                }
            }
            return roles;
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
