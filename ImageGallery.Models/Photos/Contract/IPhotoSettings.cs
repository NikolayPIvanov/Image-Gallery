using System;
using System.Collections.Generic;
using System.Text;

namespace ImageGallery.Models.Photos.Contract
{
    public interface IPhotoSettings
    {
        int MaxBytes { get; set; }
        string[] AcceptedFileTypes { get; set; }
        bool IsSupported(string fileName);
    }
}
