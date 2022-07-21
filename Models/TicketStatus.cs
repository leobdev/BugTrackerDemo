using System.ComponentModel;

namespace BugTrackerDemo.Models
{
    public class TicketStatus
    {

        public int Id { get; set; } 

        [DisplayName("Status")]
        public string Name { get; set; }   
    }
}
