using Server.Models.DB_Models;
using Server.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IPersonRepository
    {
        bool UserExists(string userName, string email);
        User GetUserByNameOrEmail(UserAuthenticationModel model);

        Task IncerementCountCompletedTasks(int userId);
    }
}
