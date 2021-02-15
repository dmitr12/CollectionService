using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IGeneratorToken
    {
        string GenerateToken(User user);
    }
}
