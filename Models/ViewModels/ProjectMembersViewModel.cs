using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackerDemo.Models.ViewModels
{
    public class ProjectMembersViewModel
    {
        public Project Project { get; set; }

        public MultiSelectList Users { get; set; }

        public List<string> SelectedUsers { get; set; }
    }
}
