using ImageGallery.API.Services.Contracts;
using ImageGallery.Data;
using ImageGallery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Services
{
    public class GalleryRepository : IGalleryRepository
    {
        GalleryContext context;

        public GalleryRepository(GalleryContext context)
        {
            this.context = context;
        }

        public async Task<bool> ImageExists(Guid id)
        {
            return await context.Images.AnyAsync(i => i.Id == id);
        }

        public async Task<Image> GetImage(Guid id)
        {
            return await this.context.Images.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Image>> GetImages()   
        {
            return await this.context.Images.ToListAsync();
        }

        public async Task AddImage(Image image)
        {
            await this.context.Images.AddAsync(image);
        }

        public void DeleteImage(Image image)
        {
            context.Images.Remove(image);

            // Note: in a real-life scenario, the image itself should also 
            // be removed from disk.  
        }

        public async Task<bool> Save()
        {
            return (await this.context.SaveChangesAsync() >= 0);
        }

    }
}
