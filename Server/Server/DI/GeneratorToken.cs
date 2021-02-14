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
        private readonly IDbHelper dbHelper;

        public GeneratorToken(IOptions<AuthenticateOptions> options, IDbHelper dbHelper)
        {
            authenticateOptions = options;
            this.dbHelper = dbHelper;
        }

        public async Task<string> GenerateToken(User user)
        {
            try
            {
                var authenticateParameters = authenticateOptions.Value;
                var securityKey = authenticateParameters.GetSymmetricSecurityKey();
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };
                claims.Add(new Claim("login", user.UserName));
                claims.Add(new Claim("role", dbHelper.GetData("select * from roles where roleid = @RoleId", new Role {RoleId=user.RoleId},
                    new List<string> { "RoleId"}).Result[0].RoleName));
                var token = new JwtSecurityToken(claims: claims, notBefore: DateTime.Now, expires: DateTime.Now.AddSeconds(authenticateParameters.TokenLifeTime),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch
            {
                throw;
            }
            finally
            {
               await dbHelper.Close();
            }
        }
    }
}
