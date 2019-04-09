using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Småstad.Models;
using Småstad.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Småstad.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        public HomeController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
        }

        /// <summary>
        /// If the modelState is valid, sends the user to the next view. Otherwise the same is loaded.
        /// </summary>
        /// <param name="errand"></param>
        /// <returns> The view Validate or Index. </returns>
        [HttpPost]
        [AllowAnonymous]
        public ViewResult Index(Errand errand)
        {

            if (ModelState.IsValid)
            {
                return View("Citizen/Validate", errand);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Checks if there is a session with an errand in it.
        /// </summary>
        /// <returns> The view with the errand if it exists, otherwise the view without anything. </returns>
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var CurrentErrand = HttpContext.Session.GetJson<Errand>("CurrentErrand");
            if(CurrentErrand == null)
            {
                return View();
            } else
            {
                return View(CurrentErrand);
            }
        }
        
        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            return View( new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user =
                    await userManager.FindByNameAsync(loginModel.Name);

                if(user != null)
                {
                    await signInManager.SignOutAsync();

                    if((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
                    {
                        if(await userManager.IsInRoleAsync(user, "Coordinator"))
                        {
                            return Redirect(loginModel.ReturnUrl = "/Coordinator/startCoordinator");
                        }
                        else if (await userManager.IsInRoleAsync(user, "Manager"))
                        {
                            return Redirect(loginModel.ReturnUrl = "/Manager/startManager");
                        }
                        else if (await userManager.IsInRoleAsync(user, "Investigator"))
                        {
                            return Redirect(loginModel.ReturnUrl = "/Investigator/startInvestigator");
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Ogiltigt namn eller lösenord");
            return View(loginModel);
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        //[AllowAnonymous]
        //public IActionResult AccessDenied()
        //{
        //    return View();
        //}
    }
}
