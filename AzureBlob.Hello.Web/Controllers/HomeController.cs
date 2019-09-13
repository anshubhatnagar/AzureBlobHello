using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzureBlob.Hello.Web.Models;
using Microsoft.AspNetCore.Http;
using AzureBlob.Hello.Web.Services;
using Microsoft.Extensions.Configuration;

namespace AzureBlob.Hello.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public HomeController(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetValue<string>("AzureStorage:ConnectionString");
        }

        public async Task<IActionResult> Index()
        {
            var blobService = new BlobService();
            List<string> imageList = await blobService.GetBlobList(_connectionString);

            return View(imageList);
        }

        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length <= 0)
                {
                    continue;
                }

                using (var stream = file.OpenReadStream())
                {
                    var blobService = new BlobService();
                    await blobService.UploadBlob(file.FileName, stream, _connectionString);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
