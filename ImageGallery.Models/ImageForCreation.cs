using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ImageGallery.Models
{
    public class ImageForCreation
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
