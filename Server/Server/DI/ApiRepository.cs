using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DI
{
    public class ApiRepository: BaseRepository<Api>
    {
        public ApiRepository(IConfiguration config) : base(config) { }

        protected override void SelectItemParameteres(SqliteCommand sqliteCommand, int id)
        {
            sqliteCommand.CommandText = "select ApiId, ApiName, BaseUrl, FilterColumn from apies where ApiId=@ApiId";
            sqliteCommand.Parameters.AddWithValue("ApiId", Convert.ToInt64(id));
        }

        protected override async Task<Api> GetItem(SqliteDataReader reader)
        {
            Api api = null;
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    api = new Api
                    {
                        ApiId = Convert.ToInt32(reader["ApiId"].ToString()),
                        ApiName = reader["ApiName"].ToString(),
                        BaseUrl = reader["BaseUrl"].ToString(),
                        FilterColumn = reader["FilterColumn"].ToString()
                    };
                }
            }
            return api;
        }

        protected override void SelectItemsParameteres(SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "select ApiId, ApiName, BaseUrl, FilterColumn from apies";
        }

        protected override async Task<IEnumerable<Api>> GetListItems(SqliteDataReader reader)
        {
            List<Api> apies = new List<Api>();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    apies.Add(new Api
                    {
                        ApiId = Convert.ToInt32(reader["ApiId"].ToString()),
                        ApiName = reader["ApiName"].ToString(),
                        BaseUrl = reader["BaseUrl"].ToString(),
                        FilterColumn = reader["FilterColumn"].ToString()
                    });
                }
            }
            return apies;
        }
    }
}
