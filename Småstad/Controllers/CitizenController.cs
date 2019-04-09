using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Småstad.Models;
using Småstad.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Småstad.Controllers
{
    public class CitizenController : Controller
    {
        private ISmastadRepo repository;

        public CitizenController(ISmastadRepo repo)
        {
            repository = repo;
        }

        public ViewResult Contact()
        {
            return View();
        }

        public ViewResult Faq()
        {
            return View();
        }

        public ViewResult Services()
        {
            return View();
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
        /// Returns a view with the current errand entered by the user. Sets the session "CurrentErrand" and
        /// fills it with the Errand object.
        /// </summary>
        /// <param name="errand"></param>
        public ViewResult Validate(Errand errand)
        {
            HttpContext.Session.SetJson("CurrentErrand", errand);
            return View(errand);
        }
    }
}
