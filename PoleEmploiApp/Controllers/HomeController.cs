using Microsoft.AspNetCore.Mvc;
using PoleEmploiApp.Models;
using PoleEmploiApp.Services.Interfaces;
using PoleEmploiApp.Services.Models;
using System.Diagnostics;

namespace PoleEmploiApp.Controllers
{
    public class HomeController : Controller
    {
        private IJobOfferService _jobOfferService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IJobOfferService jobOfferService)
        {
            _logger = logger;
            _jobOfferService = jobOfferService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RefreshJobOffers()
        {
            return Json(_jobOfferService.RefreshJobOffers());
        }


        public IActionResult DowloadExcelReport()
        {

            FileProcessResult result = _jobOfferService.GetExcelReportData();
            if (result.FileBytes != null)
            {
                return File(result.FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, result.FileName);
            }
            return Content("Error during the creation of the file");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}