using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.View_Models
{
    public class UserTasksInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int CountCompletedTasks { get; set; }
        public int CountActiveTasks { get; set; }
    }
}
