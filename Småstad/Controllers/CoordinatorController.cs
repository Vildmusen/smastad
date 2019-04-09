using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Småstad.Models;
using Småstad.Infrastructure;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Småstad.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private ISmastadRepo repository;

        public CoordinatorController(ISmastadRepo repo)
        {
            repository = repo;
        }

        public ViewResult StartCoordinator()
        {
            return View(repository);
        }

        /// <summary>
        /// Returns a view with a current errand id in the ViewBag.
        /// </summary>
        /// <param name="id"> Int of the current errand id. </param>
        public ViewResult CrimeCoordinator(int id)
        {
            ViewBag.ID = id;
            TempData["TempId"] = id;
            ViewBag.DepartmentList = repository.Department;
            return View();
        }
        
        /// <summary>
        /// Saves the correct information given in the form on CrimeCoordinator.cshtml
        /// </summary>
        /// <param name="department"></param>
        /// <returns> The view CrimeCoordinator. </returns>
        [HttpPost]
        public IActionResult UpdateDepartment(Department department)
        {
            int TempId = int.Parse(TempData["TempId"].ToString());
            string DepId = department.DepartmentId;
            
            if (ModelState.IsValid && DepId != "Välj")
            {
                // Select the last 2 characters of the department id, which should be an interger.
                string SubDepId = DepId.Substring(DepId.Length - 2);
                int DepIdInt = int.Parse(SubDepId);

                // Checking if > 0 since all departments with an id above 0 is eligible.
                if (DepIdInt > 0)
                { 
                    Errand CurrentErrand = repository.GetErrand(TempId);
                    CurrentErrand.DepartmentId = department.DepartmentId;
                    repository.SaveErrand(CurrentErrand);
                }
            }
            
            return RedirectToAction("CrimeCoordinator", new { id = TempId });

        }

        /// <summary>
        /// Returns a view with the current errand entered by the user. Sets the session "CurrentErrand", and
        /// fills it with the Errand object.
        /// </summary>
        /// <param name="errand"></param>
        public ViewResult Validate(Errand errand)
        {
            HttpContext.Session.SetJson("CurrentErrand", errand);
            return View(errand);
        }

        /// <summary>
        /// Retrieves the current errand from the session "CurrentErrand" and saves it.
        /// </summary>
        /// <returns></returns>
        public ViewResult Thanks()
        {
            Errand errand = HttpContext.Session.GetJson<Errand>("CurrentErrand");
            repository.SaveErrand(errand);
            return View();
        }

        /// <summary>
        /// If the modelState is valid, sends the user to the next view. Otherwise the same is loaded.
        /// </summary>
        /// <param name="errand"></param>
        /// <returns> the view Validate or ReportCrime. </returns>
        [HttpPost]
        public ViewResult ReportCrime(Errand errand)
        {
            if (ModelState.IsValid)
            {
                return View("Validate", errand);
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
        public ViewResult ReportCrime()
        {
            var CurrentErrand = HttpContext.Session.GetJson<Errand>("CurrentErrand");
            if (CurrentErrand == null)
            {
                return View();
            }
            else
            {
                return View(CurrentErrand);
            }
        }


    }
}
