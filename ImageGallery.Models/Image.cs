using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        public string FileName { get; set; }

        public DateTime DateUploaded { get; set; }

        public Image(string title, string fileName)
        {
            this.Title = title;
            this.FileName = fileName;
            this.DateUploaded = DateTime.Now;
        }

        public Image() { }
    }
}
