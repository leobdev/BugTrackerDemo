using System.ComponentModel;

namespace BugTrackerDemo.Models
{
    public class Invite
    {

        public int Id { get; set; }

        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; }

        [DisplayName("Join Datet")]
        public DateTimeOffset JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string InviteeId { get; set; }

        [DisplayName("Invitee Email")]
        public string InviteeEmail { get; set; }

        [DisplayName("Invitee First Name")]
        public string InviteeFirstName { get; set; }

        [DisplayName("Invitee Kast Name")]
        public string InviteeLastName { get; set; }

        public bool IsValid { get; set; }


        //Navigation Properties
        public virtual Company Company { get; set; }

        public virtual BTUser Invitor { get; set; }

        public virtual BTUser Invitee { get; set; }

        public virtual Project Project{ get; set; }

    }
}
