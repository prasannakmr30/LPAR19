using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using LPAR19.LPARCode;
using LPAR19.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LPAR19.Controllers
{
    public class PlateStepsController : Controller
    {
        public IActionResult Index()
        {
            StepData StepData = new StepData();
            return View(StepData);
        }

        public IActionResult Steps(StepData uploadFile)
        {
            StepData stepData = new StepData();
            ProcessCaptured processCaptured = new ProcessCaptured();
            Image<Bgr, Byte> imgg = null;
            Mat finalImg = new Mat();
            using (MemoryStream ms = new MemoryStream())
            {
                if (ms != null)
                {
                    uploadFile.FormFile.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    imgg = processCaptured.GetImageFromStream(ms);
                    Mat im = imgg.Mat;
                    finalImg = imgg.Mat;
                    stepData = AddData(stepData, im, "Orginal");

                    //Mat blur = new Mat();
                    //blur = Blur(im);
                    //stepData = AddData(stepData, blur, "Blur");

                    //Mat gray = new Mat();
                    //gray = Gray(blur);
                    //stepData = AddData(stepData, gray, "Gray");

                    Mat threshold = new Mat();
                    threshold = Threshold(im);
                    stepData = AddData(stepData, threshold, "Threshold");

                    Mat gray = new Mat();
                    gray = Gray(threshold);
                    stepData = AddData(stepData, gray, "Gray");

                    Mat canny = new Mat();
                    canny = Canny(gray);
                    stepData = AddData(stepData, canny, "Canny");





                    List<RotatedRect> rect = new List<RotatedRect>();
                    rect = Contours(canny);

                    Mat rectImg = new Mat();

                    rectImg = DrawRect(finalImg, rect);
                    stepData = AddData(stepData, rectImg, "With Rectangle");


                }
            }
            #region Old
            //Image<Bgr, Byte> imgg = null;
            //Image<Bgr, byte> tempImg = null;
            //Image<Gray, Byte> GrayImg = null;
            //Image<Gray, Byte> CannyImgTemp = null;
            //Image<Bgr, Byte> CannyImg = null;
            //Image<Bgr, byte> temp = null;
            //int[,] hierachy = null;
            //List<VectorOfPoint> box = new List<VectorOfPoint>();
            //List<RotatedRect> boxList = new List<RotatedRect>();
            //CvInvoke.UseOpenCL = true;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    if (ms != null)
            //    {
            //        uploadFile.FormFile.CopyTo(ms);
            //        byte[] fileBytes = ms.ToArray();
            //        imgg = processCaptured.GetImageFromStream(ms);
            //        Mat im = imgg.Mat;
            //        stepData = AddData(stepData, im, "Orginal");
            //        tempImg = imgg.Copy();
            //    }
            //}
            //using (Mat blur = new Mat())
            //{
            //    using (Mat gray = new Mat())
            //    {
            //        using (Mat canny = new Mat())
            //        {
            //            Size kSize = new Size(3, 3);
            //           // CvInvoke.GaussianBlur(imgg, blur, kSize, 0);
            //            CvInvoke.Threshold(imgg, blur, 50, 255, ThresholdType.Binary);
            //            //CvInvoke.BilateralFilter(imgg, blur, -1, -1, 100);
            //            stepData = AddData(stepData, blur, "Gray");

            //            CvInvoke.Threshold(imgg, gray,50, 255, ThresholdType.Binary);
            //            //CvInvoke.CvtColor(blur, gray, ColorConversion.Bgr2Gray);

            //            stepData = AddData(stepData, gray, "Threshold");



            //            CvInvoke.Canny(imgg, canny, 100, 50, 3, false);
            //            CannyImgTemp = canny.ToImage<Gray, byte>();
            //            stepData = AddData(stepData, gray, "canny");

            //            //Find the Rectangle

            //            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            //            {
            //                using (VectorOfPoint approxContour = new VectorOfPoint())
            //                {
            //                    //hierachy = CvInvoke.FindContourTree(canny, contours, ChainApproxMethod.ChainApproxSimple);
            //                    CvInvoke.FindContours(canny, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            //                    //for (int i = 0; i < hierachy.GetLength(0); i++)
            //                    int count = contours.Size;
            //                    for (int i = 0; i < count; i++)
            //                    {
            //                        CvInvoke.ApproxPolyDP(contours[i], approxContour, CvInvoke.ArcLength(contours[i], true) * 0.05, true);
            //                        if (CvInvoke.ContourArea(approxContour, false) > 250)
            //                        {
            //                        //    if (approxContour.Size <=6) //The contour has 4 vertices.
            //                        //    {
            //                                //#region determine if all the angles in the contour are within [80, 100] degree
            //                                //bool isRectangle = true;
            //                                //Point[] pts = approxContour.ToArray();
            //                                //LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

            //                                //for (int j = 0; j < edges.Length; j++)
            //                                //{
            //                                //    double angle = Math.Abs(
            //                                //        edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
            //                                //    if (angle < 80 || angle > 100)
            //                                //    {
            //                                //        isRectangle = false;
            //                                //        break;
            //                                //    }
            //                                //}

            //                                //#endregion
            //                                //if (isRectangle) 
            //                                boxList.Add(CvInvoke.MinAreaRect(approxContour));
            //                           // }
            //                        }
            //                    }

            //                }
            //            }
            //        }
            //    }
            //}
            //foreach (RotatedRect boxr in boxList)
            //{
            //    CvInvoke.Polylines(tempImg, Array.ConvertAll(boxr.GetVertices(), Point.Round), true,
            //        new Bgr(Color.DarkOrange).MCvScalar, 2);
            //}
            //Mat pI = tempImg.Mat;
            //stepData = AddData(stepData, pI, "Poly");
            #endregion
            return View("Index", stepData);
        }


        private StepData AddData(StepData step,Mat image,string Name)
        {
            Image<Bgr, byte> img = image.ToImage<Bgr, byte>();
            Byte[] Tbytes = img.ToJpegData();
            step.Images.Add(new Images { Data = "data:image/jpg;base64," + Convert.ToBase64String(Tbytes, 0, Tbytes.Length),ImageName=Name });
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
            CvInvoke.Canny(img, output,100,50,3);
            return output;
        }

        private Mat Dilate(Mat img)
        {
            Mat output = new Mat();
            ScalarArray elem = new ScalarArray(0);
            CvInvoke.Dilate(img, output, elem, new Point(-1,-1),1,BorderType.Default,new MCvScalar(255,255,255));
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
                    //hierachy = CvInvoke.FindContourTree(canny, contours, ChainApproxMethod.ChainApproxSimple);
                    CvInvoke.FindContours(img, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    //for (int i = 0; i < hierachy.GetLength(0); i++)
                    int count = contours.Size;
                   
                    for (int i = 0; i < count; i++)
                    {
                        CvInvoke.ApproxPolyDP(contours[i], approxContour, CvInvoke.ArcLength(contours[i], true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 100)
                        {
                            //    if (approxContour.Size <=6) //The contour has 4 vertices.
                            //    {
                            //#region determine if all the angles in the contour are within [80, 100] degree
                            //bool isRectangle = true;
                            //Point[] pts = approxContour.ToArray();
                            //LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                            //for (int j = 0; j < edges.Length; j++)
                            //{
                            //    double angle = Math.Abs(
                            //        edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                            //    if (angle < 80 || angle > 100)
                            //    {
                            //        isRectangle = false;
                            //        break;
                            //    }
                            //}

                            //#endregion
                            //if (isRectangle) 
                            boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            // }
                        }
                    }

                }
               
            }
            return boxList;
        }

        private Mat DrawRect(Mat img,List<RotatedRect> rect)
        {
            if(rect !=null && rect.Count>0)
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
            CvInvoke.Threshold(img, output, 100, 255, ThresholdType.Binary);
            return output;
        }
    }
}
