using Emgu.CV;
using Emgu.CV.Structure;
using LPAR19.LPARCode;
using LPAR19.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;

namespace LPAR19.Controllers
{
    public class GaussController : Controller
    {
        public IActionResult Index()
        {
            GaussData GaussData = new GaussData();
            return View(GaussData);
        }

        public IActionResult Gauss(GaussData GaussData)
        {
            Image<Bgra, Byte> img = null;
            Mat GaussImg = new Mat();
            Mat GrayImg = new Mat();
            Mat SobalImg = new Mat();
            Mat ThresholdImg = new Mat();
            Mat MorpImg = new Mat();
            GaussData gd = new GaussData();
            using (MemoryStream ms = new MemoryStream())
            {
                if (ms != null)
                {
                    ProcessCaptured capture = new ProcessCaptured();
                    GaussData.FormFile.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    img = capture.GetImageFromStream(ms);
                    Size size = new Size(3, 3);
                    CvInvoke.GaussianBlur(img, GaussImg, size, 0);
                    CvInvoke.CvtColor(GaussImg, GrayImg, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                    CvInvoke.Sobel(GrayImg, SobalImg, Emgu.CV.CvEnum.DepthType.Cv8U, 1, 0, 3);
                    CvInvoke.Threshold(SobalImg, ThresholdImg, 0, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                    var gi = ThresholdImg.ToImage<Bgr, Byte>();
                    Size Ksize = new Size(17, 3);
                    Point points = new Point(-1, -1);
                    var element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, Ksize, points);
                    MCvScalar scalar = new MCvScalar(1);
                    CvInvoke.MorphologyEx(ThresholdImg, MorpImg, Emgu.CV.CvEnum.MorphOp.Close, element, points, 3, Emgu.CV.CvEnum.BorderType.Default, scalar);
                    var tempImg = MorpImg.ToImage<Bgr, Byte>();
                    var temp = tempImg.ToJpegData();
                    string data = "data:image/jpg;base64," + Convert.ToBase64String(temp, 0, temp.Length);
                    gd.Images.Add(new Images { Data = data, ImageName = "Gauss" });

                }

            }
            return View("Index", gd);
        }
    }


}
