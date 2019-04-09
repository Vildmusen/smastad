using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Småstad.Models;

namespace Småstad.Components
{
    public class ShowErrandViewComponent : ViewComponent
    {
        private ISmastadRepo repository;

        public ShowErrandViewComponent(ISmastadRepo repo)
        {
            repository = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            Errand errand = await repository.getErrand(id);
            return View(errand);
        }
    }
}