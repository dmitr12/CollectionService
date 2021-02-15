using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Server.Interfaces;
using Server.Managers;
using Server.Models.DB_Models;
using Server.Models.View_Models;
using Server.Utils;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly UserManager userManager;

        public RegistrationController(UserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public IActionResult RegisterUser(UserRegistrationModel model)
        {
            if (ModelState.IsValid)
                return Ok(new { msg = userManager.RegisterUser(model) });
            return BadRequest();
        }
    }
}
