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

        public async Task<object> ExecuteQuery<T>(string queryString, T objForParameters, List<string> parameterNames) where T: class
        {
            await con.OpenAsync();
            using(SqliteCommand command = AddParameters<T>(queryString, objForParameters, parameterNames, con))
            {
                return await command.ExecuteScalarAsync();
            }
        }

        public async Task<List<T>> GetData<T>(string queryString, T objForParameters, List<string> parameterNames) where T: class
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            await con.OpenAsync();
            using (SqliteCommand command = AddParameters(queryString, objForParameters, parameterNames, con))
            {
                SqliteDataReader reader = command.ExecuteReaderAsync().Result;
                while (await reader.ReadAsync())
                {
                    T instance = Activator.CreateInstance<T>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetValue(i).GetType() == typeof(Int64))
                            type.GetProperty(reader.GetName(i)).SetValue(instance, Convert.ToInt32(reader.GetValue(i)));
                        else
                            type.GetProperty(reader.GetName(i)).SetValue(instance, reader.GetValue(i));
                    }
                    list.Add(instance);
                }
            }
            return list;
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
