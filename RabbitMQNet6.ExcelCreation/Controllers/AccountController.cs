using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQNet6.ExcelCreation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var hasUser = await _userManager.FindByEmailAsync(Email);

            if (hasUser != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(hasUser, Password, true, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
