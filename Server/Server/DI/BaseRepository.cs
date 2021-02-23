using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DI
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly SqliteConnection con;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public BaseRepository(IConfiguration config)
        {
            con = new SqliteConnection(config.GetConnectionString("SQLiteConnetion"));
        }


        public async Task<long> AddItem(T item)
        {
            long addedId = -1;
            try
            {
                await con.OpenAsync();
                using(var command = con.CreateCommand())
                {
                    AddItemParameters(item, command);
                    object result = await command.ExecuteScalarAsync();
                    addedId = (long)result;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                await con.CloseAsync();
            }
            return addedId;
        }

        public async Task DeleteItem(int id)
        {
            try
            {
                await con.OpenAsync();
                using(var command = con.CreateCommand())
                {
                    DeleteItemParameters(id, command);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllItems()
        {
            try
            {
                await con.OpenAsync();
                using(var command = con.CreateCommand())
                {
                    SelectItemsParameteres(command);
                    using(var reader=await command.ExecuteReaderAsync())
                    {
                        return GetListItems(reader).Result;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        public async Task UpdateItem(T item)
        {
            try
            {
                await con.OpenAsync();
                using (var command = con.CreateCommand())
                {

                    UpdateItemParameters(item, command);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        public async Task<T> GetItemById(int id)
        {
            try
            {
                await con.OpenAsync();
                using (var command = con.CreateCommand())
                {
                    SelectItemParameteres(command, id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return GetItem(reader).Result ;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
            finally
            {
                await con.CloseAsync();
            }
        }

        protected virtual void AddItemParameters(T obj, SqliteCommand sqliteCommand) { }
        protected virtual void DeleteItemParameters(int id, SqliteCommand sqliteCommand) { }
        protected virtual void SelectItemsParameteres(SqliteCommand sqliteCommand) { }
        protected virtual void UpdateItemParameters(T obj, SqliteCommand sqliteCommand) { }
        protected virtual void SelectItemParameteres(SqliteCommand sqliteCommand, int id) { }
        protected virtual Task<IEnumerable<T>> GetListItems(SqliteDataReader reader) { return null; }
        protected virtual Task<T> GetItem(SqliteDataReader reader) { return null; }
    }
}
