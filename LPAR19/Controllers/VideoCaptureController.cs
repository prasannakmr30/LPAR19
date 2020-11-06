using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Timers;
namespace LPAR19.Controllers
{
    public class VideoCaptureController : Controller
    {
        VideoCapture _capture=null;
        Timer _timer = null;
        int fps = 30;
        public IActionResult Index()
        {
            this._capture = new VideoCapture(@"\video\v3.mp4");
            FileInfo file = new FileInfo(@"\video\v3.mp4");
            var size = file.Length;
            this._timer = new Timer(1000/30);
            this._timer.Elapsed += _timer_Elapsed;
            this._timer.Start();
             

            return View();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Mat frame = this._capture.QueryFrame();
        }
    }
}
