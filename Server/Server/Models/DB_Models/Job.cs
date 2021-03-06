﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.DB_Models
{
    public class Job
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartTask { get; set; }
        public string Periodicity { get; set; }
        public string LastExecution { get; set; }
        public string FilterText { get; set; }
        public int UserId { get; set; }
        public int ApiId { get; set; }
    }
}
