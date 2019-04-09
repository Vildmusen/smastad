using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Småstad.Models;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Småstad.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private ISmastadRepo repository;

        public ManagerController(ISmastadRepo repo)
        {
            repository = repo;
        }

        public ViewResult StartManager()
        { 
            return View(repository);
        }

        /// <summary>
        /// Returns a view with a current errand id in the ViewBag.
        /// </summary>
        /// <param name="id"> Int of the current errand id. </param>
        public ViewResult CrimeManager(int id)
        {
            ViewBag.ID = id;
            TempData["TempId"] = id;
            ViewBag.EmployeeList = repository.Employee;
            return View();
        }

        /// <summary>
        /// Saves the correct information given in the form from CrimeManager.cshtml
        /// </summary>
        /// <param name="errand"></param>
        /// <returns> Returns the view CrimeManager </returns>
        [HttpPost]
        public IActionResult AssignErrand(Errand errand)
        {
            int TempId = int.Parse(TempData["TempId"].ToString());
            
            Errand CurrentErrand = repository.GetErrand(TempId);

            // If the checkbox is checked.
            if (errand.InvestigatorAction == "true")
            {
                CurrentErrand.StatusId = "S_B";
                CurrentErrand.InvestigatorInfo = errand.InvestigatorInfo;
            }
            // If the checkbox isn't checked, an employee is chosen and there is a department assigned to the errand.
            else if (errand.EmployeeId != "Välj" && CurrentErrand.DepartmentId != null)
            {
                CurrentErrand.EmployeeId = errand.EmployeeId;
                CurrentErrand.StatusId = "S_C";
            }

            repository.SaveErrand(CurrentErrand);

            return RedirectToAction("CrimeManager", new { id = TempId });

        }

    }
}
