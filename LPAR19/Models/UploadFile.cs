using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LPAR19.Models
{
    public class UploadFile
    {
        public UploadFile()
        {
            RawData = new List<RawData>();
        }
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
        public string OutputFile { get; set; }

        public ICollection<RawData> RawData { get; set; }

    }

    public class RawData
    {
        public string RawImage { get; set; }
        public string Number { get; set; }
    }


    public class StepData
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
        public ICollection<Images> Images { get; set; }
        public StepData()
        {
            Images = new List<Images>();
        }
    }

    public class GaussData
    {
        public GaussData()
        {
            Images = new List<Images>();
        }
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
        public ICollection<Images> Images { get; set; }
    }
    public class Images
    {
        public string Data { get; set; }
        public string ImageName { get; set; }
        public string Text { get; set; }
    }
}
