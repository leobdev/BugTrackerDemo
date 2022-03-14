﻿using BugTrackerDemo.Extensions;
using BugTrackerDemo.Models;
using BugTrackerDemo.Models.ViewModels;
using BugTrackerDemo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackerDemo.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService,
                                   IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            //Add an instance of the ViewModel as a list (model)
            List<ManageUserRolesViewModel> model = new List<ManageUserRolesViewModel>();

            //Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Get all company users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            //Loop over the users to populate the ViewModel
            // - instantiate ViewModel
            // - use _rolesService
            // - Create multi-selects
            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();

                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            //Get the Company Id
            int companyId = User.Identity.GetCompanyId().Value;

            //Instantiate the BTUser
            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);


            //Get Roles for the user
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);

            //Grab the selected role
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {

                //Remove user from their roles
                if (await _rolesService.RemoveUserFromRolesAsync(btUser, roles))
                {
                    //Add user to the new role
                    await _rolesService.AddUserRoleAsync(btUser, userRole);

                }
            }

            //Navigate back to the view
            return RedirectToAction(nameof(ManageUserRoles));

        }

    }
}
