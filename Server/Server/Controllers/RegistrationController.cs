using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Server.Interfaces;
using Server.Models.DB_Models;
using Server.Models.View_Models;
using Server.Utils;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IDbHelper dbHelper;

        public RegistrationController(IConfiguration config, IDbHelper dbHelper)
        {
            this.config = config;
            this.dbHelper = dbHelper;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await dbHelper.HasRows($"select * from users where username = @UserName or email = @Email", model, new List<string> {
                        "UserName", "Email"}))
                        return Ok(new { msg = "Пользователь с таким Login или Email уже есть" });
                    await dbHelper.ExecuteQuery($"insert into users(username, email, password, roleid) values(@UserName, @Email, @Password, @RoleId)",
                        new User {UserName = model.UserName, Email = model.Email, Password = HashClass.GetHash(model.Password), RoleId = 2 },
                        new List<string> { "UserName", "Email", "Password", "RoleId" });
                    return Ok();
                }
                catch
                {
                    return StatusCode(500);
                }
                finally
                {
                    await dbHelper.Close();
                }
            }
            return BadRequest();
        }
    }
}
