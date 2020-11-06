using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using LPAR19.LPARCode;
using LPAR19.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
using Emgu.CV.Ocl;

namespace LPAR19.Controllers
{
    public class NewmethodController : Controller
    {
        public IActionResult Index()
        {
            StepData StepData = new StepData();
            return View(StepData);
        }
        public StepData StepsVideo(string uploadFile)
        {
            StepData stepData = new StepData();
            ProcessCaptured processCaptured = new ProcessCaptured();
            Image<Bgra, Byte> imgg = null;
            if (!string.IsNullOrWhiteSpace(uploadFile))
            {
                var element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(22, 3),new Point(1,1));
                byte[] fileBytes = Convert.FromBase64String(uploadFile);
                using (MemoryStream ms = new MemoryStream(fileBytes))
                {
                    imgg = processCaptured.GetImageFromStream(ms);
                    Mat finalImg = new Mat();
                    using (Mat im = imgg.Mat.Clone())
                    {
                        using (Mat blur=new Mat())
                        {
                            using (Mat gray = new Mat())
                            {
                                using (Mat sobal = new Mat())
                                {
                                    using (Mat threshold = new Mat())
                                    {
                                        using (Mat morpology = new Mat())
                                        {
                                            using (Mat canny = new Mat())
                                            {
                                                Size size = new Size(7, 7);
                                                CvInvoke.Threshold(im, threshold, 100, 255, ThresholdType.Binary);
                                                //CvInvoke.GaussianBlur(im, blur, size, 0);
                                                CvInvoke.CvtColor(im, gray, ColorConversion.Bgr2Gray);
                                                //CvInvoke.Sobel(gray, sobal, DepthType.Cv8U, 1, 0, 3);
                                                //CvInvoke.Threshold(sobal, threshold, 0, 255, ThresholdType.Otsu);
                                                //CvInvoke.MorphologyEx(gray, morpology, MorphOp.Close, element, new Point(0, 0), 1, BorderType.Default, new MCvScalar());
                                                CvInvoke.Canny(gray, canny, 100, 50, 7);
                                                finalImg = im.Clone();
                                                var box = Contours(canny);
                                                //Mat mat=DrawRect(finalImg, box);
                                                List<Mat> roi = RoI(box, finalImg);
                                                string path = AppContext.BaseDirectory;
                                                using (Tesseract _ocr = new Tesseract(path, "eng", OcrEngineMode.TesseractLstmCombined))
                                                {
                                                    _ocr.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
                                                    foreach (Mat m in roi)
                                                    {
                                                    using(Mat mThresh=new Mat())
                                                    {
                                                        CvInvoke.CvtColor(m.Clone(),mThresh,ColorConversion.Bgr2Gray);
                                                       
                                                            _ocr.SetImage(mThresh);
                                                            _ocr.Recognize();
                                                            Tesseract.Character[] words = _ocr.GetCharacters();
                                                            //string wor=words.
                                                            StringBuilder sb = new StringBuilder();
                                                            foreach (var c in words)
                                                            {
                                                                sb.Append(c.Text);
                                                            }
                                                            if (sb.ToString().Length > 3 && sb.Length <= 10)
                                                            stepData = AddData(stepData, m, "", sb.ToString());
                                                        }
                                                    }
                                               
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }              
            }
            imgg = null;
            return stepData;
        }
        private StepData AddData(StepData step, Mat image, string Name, string text = null)
        {
            Byte[] Tbytes = null;
            if (!image.Size.IsEmpty)
            {
                Image<Bgr, byte> img = image.ToImage<Bgr, byte>();
                Tbytes = img.ToJpegData();
                var exist = step.Images.Where(c => c.Text == text).ToList();
                if (exist.Count <= 0)
                {
                    step.Images.Add(new Images { Data = "data:image/jpg;base64," + Convert.ToBase64String(Tbytes, 0, Tbytes.Length), ImageName = Name, Text = text });
                    //step.Images.Add(new Images { Text = text });
                }
            }
            return step;
        }
        private StepData AddGrayData(StepData step, Mat image, string Name)
        {
            //if (!image.IsEmpty)
            //{
            Byte[] Tbytes = null;
            Image<Gray, byte> img = image.ToImage<Gray, byte>();
            Tbytes = img.ToJpegData();
            step.Images.Add(new Images { Data = "data:image/jpg;base64," + Convert.ToBase64String(Tbytes, 0, Tbytes.Length), ImageName = Name });

            //}
            return step;
        }

        private Mat Blur(Mat img)
        {
            Mat output = new Mat();
            CvInvoke.GaussianBlur(img, output, new Size(7, 7), 1);
            return output;
        }
        private Mat Gray(Mat img)
        {
            Mat output = new Mat();
            CvInvoke.CvtColor(img, output, ColorConversion.Bgr2Gray);
            return output;
        }
        private Mat Canny(Mat img)
        {
            Mat output = new Mat();
            CvInvoke.Canny(img, output, 100, 50, 3);
            return output;
        }

        private Mat Dilate(Mat img)
        {
            Mat output = new Mat();
            ScalarArray elem = new ScalarArray(0);
            CvInvoke.Dilate(img, output, elem, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(255, 255, 255));
            return output;
        }

        private List<RotatedRect> Contours(Mat img)
        {
            List<RotatedRect> boxList = new List<RotatedRect>();
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                using (VectorOfPoint approxContour = new VectorOfPoint())
                {
                    Mat output = new Mat();
                    CvInvoke.FindContours(img, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    int count = contours.Size;
                    for (int i = 0; i < count; i++)
                    {
                        CvInvoke.ApproxPolyDP(contours[i], approxContour, CvInvoke.ArcLength(contours[i], true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 200)
                        {
                            if (approxContour.Size ==4) //The contour has 4 vertices.
                            {
                                #region determine if all the angles in the contour are within [80, 100] degree
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                        edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 70 || angle > 110)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }

                                #endregion
                                if (isRectangle)
                                    boxList.Add(CvInvoke.MinAreaRect(approxContour));
                           }
                        }
                    }

                }

            }
            return boxList;
        }


        private List<RotatedRect> PlateContours(Mat img)
        {
            List<RotatedRect> boxList = new List<RotatedRect>();
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                using (VectorOfPoint approxContour = new VectorOfPoint())
                {
                    Mat output = new Mat();
                    CvInvoke.FindContours(img, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                    int count = contours.Size;
                    for (int i = 0; i < count; i++)
                    {
                        CvInvoke.ApproxPolyDP(contours[i], approxContour, CvInvoke.ArcLength(contours[i], true) * 0.05, true);
                        //if (CvInvoke.ContourArea(approxContour, false) > 200)
                        //{
                            //if (approxContour.Size == 4) //The contour has 4 vertices.
                            //{
                            //    #region determine if all the angles in the contour are within [80, 100] degree
                            //    bool isRectangle = true;
                            //    Point[] pts = approxContour.ToArray();
                            //    LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                            //    for (int j = 0; j < edges.Length; j++)
                            //    {
                            //        double angle = Math.Abs(
                            //            edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                            //        if (angle < 70 || angle > 110)
                            //        {
                            //            isRectangle = false;
                            //            break;
                            //        }
                            //    }

                             //   #endregion
                            //    if (isRectangle)
                                    boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            //}
                        //}
                    }

                }

            }
            return boxList;
        }

        private Mat DrawRect(Mat img, List<RotatedRect> rect)
        {
            if (rect != null && rect.Count > 0)
                foreach (RotatedRect boxr in rect)
                {
                    CvInvoke.Polylines(img, Array.ConvertAll(boxr.GetVertices(), Point.Round), true,
                        new Bgr(Color.DarkOrange).MCvScalar, 2);
                }
            return img;
        }

        private Mat Threshold(Mat img)
        {
            Mat output = new Mat();
            CvInvoke.Threshold(img, output, 100, 255, ThresholdType.BinaryInv);
            return output;
        }

        private List<Mat> RoI(List<RotatedRect> rect, Mat img)
        {
            List<Mat> mat = new List<Mat>();

            foreach (var rr in rect)
            {
                RotatedRect box = rr;
                if (box.Angle < -45.0)
                {
                    float tmp = box.Size.Width;
                    box.Size.Width = box.Size.Height;
                    box.Size.Height = tmp;
                    box.Angle += 90.0f;
                }
                else if (box.Angle > 45.0)
                {
                    float tmp = box.Size.Width;
                    box.Size.Width = box.Size.Height;
                    box.Size.Height = tmp;
                    box.Angle -= 90.0f;
                }
                //double whRatio = (double)box.Size.Width / box.Size.Height;
                //if (!(2.0 < whRatio && whRatio < 8.0))
                //{
                using (Mat thresh = new Mat())
                using (Mat tmp1 = new Mat())
                using (Mat tmp2 = new Mat())
                    using(Mat tmp3 =new Mat())
                    using(Mat tmp4=new Mat())
                {
                    PointF[] srcCorners = box.GetVertices();
                    PointF[] destCorners = new PointF[] { new PointF(0, box.Size.Height - 1), new PointF(0, 0), new PointF(box.Size.Width - 1, 0), new PointF(box.Size.Width - 1, box.Size.Height - 1) };

                    using (Mat rot = CvInvoke.GetAffineTransform(srcCorners, destCorners))
                    {
                        CvInvoke.WarpAffine(img, tmp1, rot, Size.Round(box.Size));
                    }
                    
                    //CvInvoke.CvtColor(tmp1, tmp2, ColorConversion.Bgr2Gray);

                    //Size approxSize = new Size(500, 500);
                    //double scale = Math.Min(approxSize.Width / box.Size.Width, approxSize.Height / box.Size.Height);
                    //Size newSize = new Size((int)Math.Round(box.Size.Width * scale), (int)Math.Round(box.Size.Height * scale));
                    //CvInvoke.Resize(tmp1, tmp3, newSize, 0, 0, Inter.Cubic);

                    //CvInvoke.AdaptiveThreshold(tmp3, thresh,255,AdaptiveThresholdType.GaussianC,ThresholdType.Binary,11,2);
                    //CvInvoke.BitwiseNot(thresh, tmp4);
                    
                    mat.Add(tmp1.Clone());
                }
                //}
            }
            return mat;
        }

    }
}
