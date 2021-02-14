using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Models.DB_Models;
using Microsoft.AspNetCore.Mvc;
using Server.Interfaces;
using Server.Models.View_Models;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IDbHelper dbHelper;
        private readonly IGeneratorToken generatorToken;

        public AuthenticationController(IDbHelper dbHelper, IGeneratorToken generatorToken)
        {
            this.dbHelper = dbHelper;
            this.generatorToken = generatorToken;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserAuthenticationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<User> users = await dbHelper.GetData("select * from users where (username = @UserName or email = @UserName) and password = @Password",
                        new User { UserName = model.Login, Password=model.Password }, new List<string> { "UserName", "Password" });
                    if(users.Count == 1)
                    {
                        var token = await generatorToken.GenerateToken(users[0]);
                        return Ok(new { token = token });
                    }
                    return Unauthorized();
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
