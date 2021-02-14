using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IGeneratorToken
    {
        Task<string> GenerateToken(User user);
    }
}
