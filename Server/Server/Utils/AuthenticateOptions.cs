using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    public class AuthenticateOptions
    {
        public string Secret { get; set; } 
        public int TokenLifeTime { get; set; } 
        public SymmetricSecurityKey GetSymmetricSecurityKey() 
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
    }
}
