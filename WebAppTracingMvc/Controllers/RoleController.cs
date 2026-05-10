using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebAppTracingMvc.ViewModels;

public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        this.roleManager = roleManager;
    }

    // ================= INDEX =================
    public IActionResult Index()
    {
        var roles = roleManager.Roles
            .Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToList();

        return View(roles);
    }

    // ================= CREATE GET =================
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // ================= CREATE POST =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var role = new IdentityRole
        {
            Name = model.Name
        };

        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    // ================= EDIT GET =================
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
            return NotFound();

        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        var model = new RoleViewModel
        {
            Id = role.Id,
            Name = role.Name
        };

        return View(model);
    }

    // ================= EDIT POST =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var role = await roleManager.FindByIdAsync(model.Id);

        if (role == null)
            return NotFound();

        role.Name = model.Name;

        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    // ================= DELETE =================
    public async Task<IActionResult> Delete(string id)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        await roleManager.DeleteAsync(role);

        return RedirectToAction(nameof(Index));
    }
}