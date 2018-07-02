using System;
using System.Collections.Generic;
using System.Text;

namespace ImageGallery.Models
{
    public class ImageDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public string Name { get; set; }

        public DateTime Uploaded { get; set; }


    }
}
