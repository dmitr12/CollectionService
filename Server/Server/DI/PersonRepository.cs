using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Server.Interfaces;
using Server.Models.DB_Models;
using Server.Models.View_Models;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DI
{
    public class PersonRepository : BaseRepository<User>, IPersonRepository
    {

        public PersonRepository(IConfiguration config) : base(config) { }

        protected override void AddItemParameters(User user, SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "insert into users(username, email, password, roleid, countcompletedtasks) " +
                "values(@UserName, @Email, @Password, @RoleId, @CountCompletedTasks); select last_insert_rowid()";
            sqliteCommand.Parameters.AddWithValue("UserName", user.UserName);
            sqliteCommand.Parameters.AddWithValue("Email", user.Email);
            sqliteCommand.Parameters.AddWithValue("Password", user.Password);
            sqliteCommand.Parameters.AddWithValue("RoleId", Convert.ToInt64(user.RoleId));
            sqliteCommand.Parameters.AddWithValue("CountCompletedTasks", Convert.ToInt64(user.CountCompletedTasks));
        }

        protected override void DeleteItemParameters(int id, SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "delete from users where UserId=@UserId";
            sqliteCommand.Parameters.AddWithValue("@UserId", id);
        }

        protected override void UpdateItemParameters(User user, SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "update users set UserName=@UserName, Email=@Email, Password=@Password," +
                " RoleId=@RoleId, CountCompletedTasks=@CountCompletedTasks where userId=@UserId";
            sqliteCommand.Parameters.AddWithValue("UserId",Convert.ToInt64(user.UserId));
            sqliteCommand.Parameters.AddWithValue("UserName", user.UserName);
            sqliteCommand.Parameters.AddWithValue("Email", user.Email);
            sqliteCommand.Parameters.AddWithValue("Password", user.Password);
            sqliteCommand.Parameters.AddWithValue("RoleId", Convert.ToInt64(user.RoleId));
            sqliteCommand.Parameters.AddWithValue("CountCompletedTasks", Convert.ToInt64(user.CountCompletedTasks));
        }

        protected override async Task<IEnumerable<User>> GetListItems(SqliteDataReader reader)
        {
            List<User> users = new List<User>();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"].ToString()),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        RoleId = Convert.ToInt32(reader["RoleId"].ToString()),
                        CountCompletedTasks = Convert.ToInt32(reader["CountCompletedTasks"].ToString())
                    });
                }
            }
            return users;
        }

        protected override void SelectItemsParameteres(SqliteCommand sqliteCommand)
        {
            sqliteCommand.CommandText = "select UserId, UserName, Email, Password, RoleId, CountCompletedTasks from users";
        }

        protected override void SelectItemParameteres(SqliteCommand sqliteCommand, int id)
        {
            sqliteCommand.CommandText = "select UserId, UserName, Email, Password, RoleId, CountCompletedTasks from users where UserId=@UserId";
            sqliteCommand.Parameters.AddWithValue("UserId", Convert.ToInt64(id));
        }

        protected override async Task<User> GetItem(SqliteDataReader reader)
        {
            User user = null;
            if (reader.HasRows)
            {
                while(await reader.ReadAsync())
                {
                    user = new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"].ToString()),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        RoleId = Convert.ToInt32(reader["RoleId"].ToString()),
                        CountCompletedTasks = Convert.ToInt32(reader["CountCompletedTasks"].ToString())
                    };
                }
            }
            return user;
        }

        public bool UserExists(string userName, string email)
        {
            if (GetAllItems().Result.Where(u => u.UserName == userName || u.Email == email).Count() > 0)
                return true;
            return false;
        }

        public User GetUserByNameOrEmail(UserAuthenticationModel model)
        {
            return GetAllItems().Result.Where(u => (u.UserName == model.Login || u.Email == model.Login) && u.Password==HashClass.GetHash(model.Password)).FirstOrDefault();
        }

        public async Task IncerementCountCompletedTasks(int userId)
        {
            User user = GetItemById(userId).Result;
            user.CountCompletedTasks++;
            await UpdateItem(user);
        }
    }
}
