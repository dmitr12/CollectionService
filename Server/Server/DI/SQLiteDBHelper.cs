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
            var list = new List<T>();
            var type = typeof(T);
            await con.OpenAsync();
            await using var command = AddParameters(queryString, objForParameters, parameterNames, con);
            var reader = command.ExecuteReaderAsync().Result;
            while (await reader.ReadAsync())
            {
                var instance = Activator.CreateInstance<T>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    type.GetProperty(reader.GetName(i))?.SetValue(instance,
                        reader.GetValue(i) is long ? Convert.ToInt32(reader.GetValue(i)) : reader.GetValue(i));
                }
                list.Add(instance);
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
            var type = typeof(T);
            var command = new SqliteCommand(queryString, connection);
            if (parameterNames != null)
            {
                foreach (var t in parameterNames)
                {
                    command.Parameters.AddWithValue($"@{t}", type.GetProperty($"{t}")?.GetValue(obj));
                }
            }
            return command;
        }
    }
}
