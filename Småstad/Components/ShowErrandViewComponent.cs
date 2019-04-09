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

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            List<Picture> pictures = repository.GetPictures(id);
            List<Sample> samples = repository.GetSamples(id);
            var objectOfErrand = await repository.GetErrandTask(id);
            objectOfErrand.Samples = samples;
            objectOfErrand.Pictures = pictures;
            return View(objectOfErrand);
        }
    }
}
