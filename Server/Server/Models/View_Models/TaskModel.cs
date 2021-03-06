﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.View_Models
{
    public class TaskModel
    {
        public int TaskId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string StartTask { get; set; }

        [Required]
        public string Periodicity { get; set; }

        [Required]
        public string FilterText { get; set; }

        [Required]
        public int ApiId { get; set; }
    }
}
