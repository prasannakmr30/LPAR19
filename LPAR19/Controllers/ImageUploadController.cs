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
    public class ImageUploadController : Controller
    {
        public IActionResult Index()
        {
            UploadFile uploadFile = new UploadFile();
            return View(uploadFile);
        }

        public IActionResult FileUpload(UploadFile uploadFile)
        {
            UploadFile updata = new UploadFile();
            using (MemoryStream ms = new MemoryStream())
            {
                uploadFile.FormFile.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
                ProcessCaptured processCaptured = new ProcessCaptured();
                var image = Image.FromStream(ms);
                Image<Bgr, Byte> imgg = processCaptured.GetImageFromStream(ms);
                updata = processCaptured.ProcessImage(imgg);
            }
            return View("Index", updata);
        }


    }
}
