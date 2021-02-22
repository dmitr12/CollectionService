using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface ITaskRepository
    {
        IEnumerable<Job> GetTasksByUserId(int userId);
    }
}
