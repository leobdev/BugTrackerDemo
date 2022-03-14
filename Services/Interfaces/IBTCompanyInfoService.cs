using BugTrackerDemo.Models;

namespace BugTrackerDemo.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {

        public Task<Company> GetCompanyInfoByAdAsync(int? company);

        public Task<List<BTUser>> GetAllMembersAsync(int companyId);

        public Task<List<Project>> GetAllProjectsAsync(int companyId);

        public Task<List<Ticket>> GetAllTicketsAsync(int companyId);

    }
}
