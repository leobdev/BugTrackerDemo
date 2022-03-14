using BugTrackerDemo.Data;
using BugTrackerDemo.Models;
using BugTrackerDemo.Models.Enums;
using BugTrackerDemo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackerDemo.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }

        //CRUD - Create
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentPM = await GetProjectManagerAsync(projectId);

            if (currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"***ERROR*** Error removing PM. Error --> {ex.Message}");

                    return false;
                }
            }

            //Add the new PM

            try
            {
                await AddUserToProjectAsync(userId, projectId);

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"***ERROR*** Error adding PM. Error --> {ex.Message}");

                return false;
            }


        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);

                        await _context.SaveChangesAsync();

                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        //CRUD - Delete (Archive)
        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<BTUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = new();

            projects = await _context.Projects.Where(x => x.CompanyId == companyId && x.Archived == false)
                                            .Include(x => x.Members)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.Comments)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.TicketStatus)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.TicketPriority)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.TicketType)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.Histories)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.DeveloperUser)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.OwnerUser)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(t => t.Notifications)
                                            .Include(x => x.ProjectPriority)
                                            .ToListAsync();

            return projects;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            int priorityId = await LookupProjectPriorityId(priorityName);

            return projects.Where(x => x.ProjectPriorityId == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            return projects.Where(x => x.Archived == true).ToList();

        }

        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        //CRUD - Read
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects
                .Include(x => x.Tickets)
                .Include(x => x.Members)
                .Include(x => x.ProjectPriority)
                .FirstOrDefaultAsync(x => x.Id == projectId && x.CompanyId == companyId);

            return project;

        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                                .Include(x => x.Members)
                                .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach(BTUser member in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }
            return null;
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects.Include(x => x.Members)
                                    .FirstOrDefaultAsync(t => t.Id == projectId);

            List<BTUser> members = new();

            foreach (var user in project.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }

            return members;

        }

        public async Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Company)
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Members)
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Tickets)
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Tickets)
                                                    .ThenInclude(u => u.DeveloperUser)
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Tickets)
                                                    .ThenInclude(u => u.OwnerUser)
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Tickets)
                                                    .ThenInclude(u => u.TicketPriority)
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Tickets)
                                                    .ThenInclude(u => u.TicketStatus)
                                            .Include(x => x.Projects)
                                                .ThenInclude(t => t.Tickets)
                                                    .ThenInclude(u => u.TicketType)
                                            .FirstOrDefaultAsync(x => x.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"****ERROR***** - Error getting user projects list ---> {ex.Message}");

                throw;

            }
        }

        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BTUser> users = await _context.Users.Where(x => x.Projects.All(p => p.Id != projectId)).ToListAsync();

            return users.Where(x => x.CompanyId == companyId).ToList();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects
                                    .Include(x => x.Members)
                                    .FirstOrDefaultAsync(x => x.Id == projectId);

            bool result = false;

            if (project != null)
            {
                result = project.Members.Any(x => x.Id == userId);
            }

            return result;
        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(x => x.Name == priorityName)).Id;

            return priorityId;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                                .Include(x => x.Members)
                                .FirstOrDefaultAsync(p => p.Id == projectId);
            try
            {
                foreach(BTUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                        
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);

                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception)
                {
                    throw;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"*****Error****** Error removing user from project. ---> {ex.Message}");
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);

                Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

                foreach(BTUser btUser in members)
                {
                    try
                    {
                        project.Members.Remove(btUser);
                        await _context.SaveChangesAsync();
                    }
                    catch(Exception)
                    {
                        throw;
                    }                 
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"****ERROR**** - Error removing users from project. ---> {ex.Message}");

                throw;
            }
        }

        //CRUD - Update
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
