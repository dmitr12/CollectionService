using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Interfaces;
using Server.Models.DB_Models;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.DI
{
    public class GeneratorToken : IGeneratorToken
    {
        private readonly IOptions<AuthenticateOptions> authenticateOptions;
        private const string claimRole = "role";
        public GeneratorToken(IOptions<AuthenticateOptions> options)
        {
            authenticateOptions = options;
        }

        public string GenerateToken(User user)
        {
            var authenticateParameters = authenticateOptions.Value;
            var securityKey = authenticateParameters.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(claimRole, user.RoleId.ToString())
            };
            var token = new JwtSecurityToken(claims: claims, notBefore: DateTime.Now, expires: DateTime.Now.AddSeconds(authenticateParameters.TokenLifeTime),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
