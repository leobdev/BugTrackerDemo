using System.ComponentModel;

namespace BugTrackerDemo.Models
{
    public class TicketType
    {
        public int Id { get; set; }


        [DisplayName("Type Name")]
        public string Name { get; set; }


    }
}
