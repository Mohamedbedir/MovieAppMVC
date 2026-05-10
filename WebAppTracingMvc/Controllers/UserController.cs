using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tracing.DAL.Entities;
using WebAppTracingMvc.Helpers;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.Controllers
{
    [Authorize]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? search)
        {
            var users = await userManager.Users.ToListAsync();

            // SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                users = users
                    .Where(u => u.Email.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            List<UserViewModel> result = new();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    ProfilePicture = user.ProfilePicture,
                    Roles = roles.ToList()
                });
            }

            ViewBag.Search = search;

            return View(result);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await userManager.GetRolesAsync(user);

            var result = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                Roles = roles.ToList()
            };

            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = roleManager.Roles
                .Select(r => r.Name)
                .ToList();

            var userRoles = await userManager.GetRolesAsync(user);

            var result = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,

                RoleName = userRoles.FirstOrDefault(),
                Roles = roles // 👈 مهم جدًا
            };

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            // ================= IMAGE =================
            if (model.ProfileImage != null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    DocumentSettings.DeleteImage(user.ProfilePicture, "Images");
                }

                user.ProfilePicture = DocumentSettings.UploadImage(model.ProfileImage, "Images");
            }

            // ================= BASIC INFO =================
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName; 

            await userManager.SetUserNameAsync(user, model.UserName);

            // ================= ROLE UPDATE =================
            if (User.IsInRole("Admin"))
            {
                var oldRoles = await userManager.GetRolesAsync(user);

                if (oldRoles.Any())
                    await userManager.RemoveFromRolesAsync(user, oldRoles);

                if (!string.IsNullOrEmpty(model.RoleName))
                    await userManager.AddToRoleAsync(user, model.RoleName);
            }

            await userManager.UpdateAsync(user);

            TempData["SuccessUpdate"] = $"User {user.UserName} Updated Successfully";

            return RedirectToAction(nameof(Details), new { id = user.Id });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            string folderName = "Images";

            // ================= DELETE IMAGE =================
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                DocumentSettings.DeleteImage(user.ProfilePicture, folderName);
            }

            // ================= DELETE USER =================
            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error deleting user");
                return View(user);
            }

            TempData["SuccessDelete"] = $"User {user.UserName} Deleted Successfully";

            return RedirectToAction(nameof(Index));
        }

    }
}
