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
using Server.Utils;
using Server.Managers;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager userManager;

        public AuthenticationController(UserManager userManager)
        {
            this.userManager = userManager;
        }


        [HttpGet]
        public IActionResult TestApi()
        {
            return Ok(new { msg = "hello", number = 1 });
        }

        [HttpPost]
        public IActionResult Login(UserAuthenticationModel model)
        {
            if (ModelState.IsValid)
            {
                string token = userManager.GetToken(model);
                if(token!=null)
                    return Ok(new { token = token });
            }
            return Unauthorized();
        }
    }
}
