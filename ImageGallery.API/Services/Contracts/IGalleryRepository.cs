﻿using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Services.Contracts
{
    public interface IGalleryRepository
    {
        Task<IEnumerable<Image>> GetImages();   
        Task<Image> GetImage(Guid id);
        Task<bool> ImageExists(Guid id);
        Task AddImage(Image image);
        void DeleteImage(Image image);
        Task<bool> Save();
    }
}
