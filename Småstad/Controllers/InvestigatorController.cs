using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Småstad.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Småstad.Controllers
{
    [Authorize(Roles = "Investigator")]
    public class InvestigatorController : Controller
    {
        private ISmastadRepo repository;
        private IHostingEnvironment enviroment;

        public InvestigatorController(ISmastadRepo repo, IHostingEnvironment env)
        {
            repository = repo;
            enviroment = env;
        }

        public ViewResult StartInvestigator()
        {
            return View(repository);
        }

        /// <summary>
        /// Returns a view with a current errand id in the ViewBag.
        /// </summary>
        /// <param name="id"> Int of the current errand id. </param>
        public ViewResult CrimeInvestigator(int id)
        {
            ViewBag.ID = id;
            TempData["TempId"] = id;
            ViewBag.StatusList = repository.ErrandStatus;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errand"></param>
        /// <param name="picture"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateInvestigation(Errand errand, IFormFile picture, IFormFile sample)
        {
            int TempId = int.Parse(TempData["TempId"].ToString());
            
            Errand CurrentErrand = repository.GetErrand(TempId);

            // If a new status is chosen and it doesn't match the already selected one.
            if (errand.StatusId != CurrentErrand.StatusId && errand.StatusId != "Välj")
            {
                CurrentErrand.StatusId = errand.StatusId;

                // Both picture and sample need to implement saving of multiple pictures/samples. 
                // TODO - get ICollection -> add(newthingy) -> save updated list/collection.
                if (picture != null)
                {
                    string path = UploadFile(picture);
                    List<Picture> newPic = new List<Picture>();
                    newPic.Add(new Picture { PictureName = path, ErrandId = CurrentErrand.ErrandId });
                    repository.SavePic(newPic);
                    CurrentErrand.Pictures = newPic;
                }
                if (sample != null)
                {
                    string path = UploadFile(sample);
                    List<Sample> newSamp = new List<Sample>();
                    newSamp.Add(new Sample { SampleName = path, ErrandId = CurrentErrand.ErrandId });
                    repository.SaveSamp(newSamp);
                    CurrentErrand.Samples = newSamp;
                }

                if (errand.InvestigatorInfo != null)
                {
                    CurrentErrand.InvestigatorInfo += errand.InvestigatorInfo;
                }
                if (errand.InvestigatorAction != null)
                {
                    CurrentErrand.InvestigatorAction += errand.InvestigatorAction;
                }

            }

            repository.SaveErrand(CurrentErrand);

            return RedirectToAction("CrimeInvestigator", new { id = TempId });

        }
        
        /// <summary>
        /// Saves a file to the folder "Uploads".
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private string UploadFile(IFormFile document)
        {
            var tempPath = Path.GetTempFileName();
            var stream = new FileStream(tempPath, FileMode.Create);
            document.CopyToAsync(stream);
            stream.Close();

            var newPath = Path.Combine(enviroment.WebRootPath, "Uploads", document.FileName);
            System.IO.File.Move(tempPath, newPath);
            return document.FileName;
        }

    }
}
