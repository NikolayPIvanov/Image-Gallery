using ImageGallery.API.Helpers;
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
        private readonly IPropertyMappingService propertyMappingService;

        public GalleryRepository(GalleryContext context,IPropertyMappingService propertyMappingService)
        {
            this.context = context;
            this.propertyMappingService = propertyMappingService;
        }

        public async Task<bool> ImageExists(Guid id)
        {
            return await context.Images.AnyAsync(i => i.Id == id);
        }

        public async Task<Image> GetImage(Guid id)
        {
            return await this.context.Images.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<PagedList<Image>> GetImages(ImagesResourceParameters imagesResourceParameters)
        {
            //var collection = this.context.Images
            //    .OrderBy(i => i.Title)
            //   .AsQueryable();
            
            var collection = this.context.Images
               .ApplySort(imagesResourceParameters.OrderBy,
               propertyMappingService.GetPropertyMapping<ImageDto, Image>());

            if (!string.IsNullOrEmpty(imagesResourceParameters.Title))
            {
                var titleForWhereClause = imagesResourceParameters.Title.Trim().ToLowerInvariant();
                collection = collection.Where(x => x.Title.ToLowerInvariant() == titleForWhereClause);
            }

            return await PagedList<Image>.Create(collection,
                imagesResourceParameters.PageNumber,
                imagesResourceParameters.PageSize);
        }

        public async Task AddImage(Image image)
        {
            await this.context.Images.AddAsync(image);
        }

        public void DeleteImage(Image image)
        {
            context.Images.Remove(image);

        }

        public async Task<bool> Save()
        {
            return (await this.context.SaveChangesAsync() >= 0);
        }


    }
}
