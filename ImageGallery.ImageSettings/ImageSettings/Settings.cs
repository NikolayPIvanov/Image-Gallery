
using ImageGallery.Settings;
using ImageGallery.Settings.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
    
namespace ImageGallery.ImageSettings
{
    public class Settings : IPhotoSettings
    {
        public int MaxBytes { get; set; }
        public string[] AcceptedFileTypes { get; set; }

        public bool IsSupported(string fileName)
        {
            return Enum.IsDefined(typeof(AllowedExtensionEnum),Path.GetExtension(fileName).ToLower());
        }
    }
}
