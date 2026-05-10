using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Tracing.DAL.Entities;
using WebAppTracingMvc.Helpers;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid) 
            {
                ApplicationUser user = new()
                {
                    UserName = registerViewModel.UserName,
                    Email = registerViewModel.Email,
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,

                };
                var res=await userManager.CreateAsync(user,registerViewModel.Password);
                if (res.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(registerViewModel);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Search User By Email
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email Or Password");

                return View(model);
            }

            // Login
            var result = await signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Movie");
            }

            ModelState.AddModelError("", "Invalid Email Or Password");

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Movie");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);

            // 🔐 Security: نفس النتيجة حتى لو المستخدم مش موجود
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email Address");

                return View(model);
            }

            // 🔑 Generate Reset Token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // 🔗 Create Reset Link
            var resetLink = Url.Action(
                action: "ResetPassword",
                controller: "Account",
                values: new { token, email = model.Email },
                protocol: Request.Scheme
            );

            // 📧 Email Body
            var email = new Email
            {
                From = "My App <yourEmail@gmail.com>",
                To = model.Email,
                Subject = "Reset Your Password",
                Body = $@"
            <div style='font-family:Arial;padding:20px'>
                
                <h2 style='color:#1877f2;'>Reset Your Password</h2>

                <p>Hello <b>{user.UserName}</b>,</p>

                <p>We received a request to reset your password.</p>

                <!-- Button Link -->
                <a href='{resetLink}'
                   style='display:inline-block;
                   background:#1877f2;
                   color:#fff;
                   padding:12px 20px;
                   text-decoration:none;
                   border-radius:6px;
                   margin-top:10px;'>
                   Click here to reset password
                </a>

                <p style='margin-top:20px;'>Or copy this link:</p>

                <p style='word-break:break-all;color:#555;'>
                    {resetLink}
                </p>

                <hr />

                <p style='color:red;font-size:12px;'>
                    If you did not request this, ignore this email.
                </p>

            </div>
        "
            };

            // 📩 Send Email
            await EmailSettings.SendEmailAsync(email);

            // 📦 Pass link to confirmation page (for UI box)
            //TempData["ResetLink"] = resetLink;

            return RedirectToAction(nameof(ForgetPasswordConfirmation));
        }
        [HttpGet]
        public IActionResult ForgetPasswordConfirmation()
        {
            ViewBag.ResetLink = TempData["ResetLink"];
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
                return BadRequest();

            TempData["email"] = email;
            TempData["token"] = token;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep();

                return View(model);
            }

            string email = TempData["email"] as string;
            string token = TempData["token"] as string;

            if (email == null || token == null)
                return RedirectToAction(nameof(Login));

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return RedirectToAction(nameof(Login));

            var result = await userManager.ResetPasswordAsync(
                user,
                token,
                model.Password
            );

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // 🔥 عشان TempData متتمسحش
            TempData["email"] = email;
            TempData["token"] = token;

            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
