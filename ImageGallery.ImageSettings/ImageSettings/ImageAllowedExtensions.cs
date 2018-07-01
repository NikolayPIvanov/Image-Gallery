
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ImageGallery.Settings

{
    public class ImageAllowedExtensions
    {
        [Required]
        [AllowedExtentions]
        public string Extension { get; set; }
    }
}
