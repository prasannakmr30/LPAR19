using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using LPAR19.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LPAR19.LPARCode
{
    public class ProcessCaptured
    {
        public UploadFile ProcessImage(IInputOutputArray image)
        {
            string path = AppContext.BaseDirectory;
            string base64string = string.Empty;
            UploadFile uploadFile = new UploadFile();
            LicensePlateDetector _licensePlateDetector = new LicensePlateDetector(path);
            List<IInputOutputArray> licensePlateImagesList = new List<IInputOutputArray>();
            List<IInputOutputArray> filteredLicensePlateImagesList = new List<IInputOutputArray>();
            List<RotatedRect> licenseBoxList = new List<RotatedRect>();
            List<string> words = _licensePlateDetector.DetectLicensePlate(
                image,
                licensePlateImagesList,
                filteredLicensePlateImagesList,
                licenseBoxList);
            Point startPoint = new Point(10, 10);
            for (int i = 0; i < words.Count; i++)
            {
                Mat dest = new Mat();
                CvInvoke.VConcat(licensePlateImagesList[i], filteredLicensePlateImagesList[i], dest);
                PointF[] verticesF = licenseBoxList[i].GetVertices();
                Point[] vertices = Array.ConvertAll(verticesF, Point.Round);
                using (VectorOfPoint pts = new VectorOfPoint(vertices))
                    CvInvoke.Polylines(image, pts, true, new Bgr(Color.Red).MCvScalar, 2);
                UMat mat = (UMat)filteredLicensePlateImagesList[i];
                Image<Bgr, byte> LpImage = mat.ToImage<Bgr, byte>();
                byte[] LpBytes = LpImage.ToJpegData();
                uploadFile.RawData.Add(new RawData { RawImage = "data:image/jpg;base64," + Convert.ToBase64String(LpBytes, 0, LpBytes.Length), Number = words[i] });
            }
            using (Image<Bgr, byte> img = (Image<Bgr, byte>)image)
            {
                Byte[] bytes = img.ToJpegData();
                base64string = "data:image/jpg;base64," + Convert.ToBase64String(bytes, 0, bytes.Length);
                uploadFile.OutputFile = base64string;
            }
            return uploadFile;
        }

        public Image<Bgr, byte> GetImageFromStream(MemoryStream ms)
        {
            int stride = 0;
            Image<Bgr, byte> cvImage = null;
            Bitmap bmp = new Bitmap(ms);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            System.Drawing.Imaging.PixelFormat pf = bmp.PixelFormat;
            if (pf == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                stride = bmp.Width * 4;
            }
            else
            {
                stride = bmp.Width * 3;
            }
            cvImage = new Image<Bgr, byte>(bmp.Width, bmp.Height, stride, (IntPtr)bmpData.Scan0);
            bmp.UnlockBits(bmpData);
            return cvImage;
        }


    }
}
