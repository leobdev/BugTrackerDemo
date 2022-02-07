﻿using System.ComponentModel;

namespace BugTrackerDemo.Models
{
    public class TicketStatus
    {

        public int Id { get; set; } 

        [DisplayName("Status Name")]
        public string? Name { get; set; }   
    }
}
