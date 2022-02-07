using System.ComponentModel;

namespace BugTrackerDemo.Models
{
    public class Company
    {
        public int Id { get; set; } 

        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }    



        //navigation properties

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<BTUser> Members { get; set; }
    }
}
