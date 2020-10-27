using LPAR19.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LPAR19.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //string path = AppContext.BaseDirectory;
            //ProcessCaptured processCaptured = new ProcessCaptured();
            //////string path = server.Map
            ////var _ocr = new Tesseract(path, "eng", OcrEngineMode.Default);
            ////Image<Bgr, Byte> imgg = new Image<Bgr, byte>(path + "\\Untitled.png");
            //Image<Bgr, Byte> imgg = new Image<Bgr, byte>(path + "\\eucar.jpg");
            //processCaptured.ProcessImage(imgg);
            ////_ocr.SetImage(imgg);
            ////_ocr.Recognize();
            ////var result = _ocr.GetCharacters();
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
