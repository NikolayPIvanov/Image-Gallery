using ImageGallery.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Data
{
    public class GalleryContext : DbContext
    {
        public GalleryContext(DbContextOptions<GalleryContext> options) : base(options)
        {

        }

        public DbSet<Image> Images { get; set; }
        
    }
}
