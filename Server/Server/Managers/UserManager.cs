using Server.Interfaces;
using Server.Models.DB_Models;
using Server.Models.View_Models;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Managers
{
    public class UserManager
    {
        private readonly IDbHelper dbHelper;
        private readonly IGeneratorToken generatorToken;

        public UserManager(IDbHelper dbHelper, IGeneratorToken generatorToken)
        {
            this.dbHelper = dbHelper;
            this.generatorToken = generatorToken;
        }

        public string RegisterUser(UserRegistrationModel model)
        {
            try
            {
                if (dbHelper.HasRows($"select * from users where username = @UserName or email = @Email", model, new List<string> {
                        "UserName", "Email"}).Result)
                {
                    return "Пользователь с таким Login или Email уже есть";
                }
                dbHelper.ExecuteQuery($"insert into users(username, email, password, roleid, countcompletedtasks) values(@UserName, @Email, @Password, @RoleId, @CountCompletedTasks)",
                    new User { UserName = model.UserName, Email = model.Email, Password = HashClass.GetHash(model.Password), RoleId = 1 , CountCompletedTasks=0},
                    new List<string> { "UserName", "Email", "Password", "RoleId", "CountCompletedTasks"});
                return null;
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public string GetToken(UserAuthenticationModel model)
        {
            try
            {
                List<User> users = dbHelper.GetData("select * from users where (username = @UserName or email = @UserName) and password = @Password",
                    new User { UserName = model.Login, Password = HashClass.GetHash(model.Password) }, new List<string> { "UserName", "Password" }).Result;
                if (users.Count == 1)
                {
                    var token = generatorToken.GenerateToken(users[0]);
                    return token;
                }
                return null;
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public void InceremntUserCompletedTasks(int userId)
        {
            try
            {
                dbHelper.ExecuteQuery("update users set CountCompletedTasks=CountCompletedTasks+1 where userId=@UserId", new User { UserId = userId },
                    new List<string> { "UserId" });
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return dbHelper.GetData<User>("select * from users where RoleId=1", null, null).Result;
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
               return dbHelper.GetData("select * from users where UserId = @UserId", new User { UserId = userId }, new List<string> { "UserId" }).Result[0];
            }
            finally
            {
                dbHelper.Close();
            }
        }
    }
}
