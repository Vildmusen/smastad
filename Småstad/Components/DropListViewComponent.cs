using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Småstad.Models;

namespace Småstad.Components
{
    public class DropListViewComponent : ViewComponent
    {
        private ISmastadRepo repository;

        public DropListViewComponent(ISmastadRepo repo)
        {
            repository = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync(string type)
        {
            List<string> list = await repository.getList(type);
            return View(list);
        }
    }
}
