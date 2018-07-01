using AutoMapper;
using ImageGallery.API.Services.Contracts;
using ImageGallery.Models;
using ImageGallery.Models.Photos;
using ImageGallery.Models.Photos.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPhotoSettings photoSettings;

        public ImagesController(IGalleryRepository galleryRepository,
                                IHostingEnvironment hostingEnvironment,
                                IPhotoSettings photoSettings)
        {
            _galleryRepository = galleryRepository;
            _hostingEnvironment = hostingEnvironment;
            this.photoSettings = photoSettings;
        }

        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var imagesFromRepo = await _galleryRepository.GetImages();

            var imagesToReturn = Mapper.Map<IEnumerable<Image>>(imagesFromRepo);

            return Ok(imagesToReturn);
        }

        [HttpGet("{id}", Name = "GetImage")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var imageFromRepo = await _galleryRepository.GetImage(id);

            if (imageFromRepo == null)
            {
                return NotFound();
            }

            var imageToReturn = Mapper.Map<Image>(imageFromRepo);

            return Ok(imageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageForCreation imageForCreation)
        {
            var file = imageForCreation.File;

            if (file == null)
            {
                return BadRequest("Null file");
            }

            if (file.Length == 0)
            {
                return BadRequest("Empty file");
            }

           /* if (file.Length > photoSettings.MaxBytes)
            {
                return BadRequest("Max file size exceeded");
            }
            if (!photoSettings.IsSupported(file.FileName))
            {
                return BadRequest("Invalid file type.");
            }
            */
            var uploadFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageForCreation.File.FileName);

            var filePath = Path.Combine(uploadFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageForCreation.File.CopyToAsync(stream);
            }

            var image = new Image(imageForCreation.Title, fileName);

            await _galleryRepository.AddImage(image);

            if (!(await _galleryRepository.Save()))
            {
                return BadRequest("An error ocurred while saving the data into the database.");
            }

            return CreatedAtRoute("GetImage",
               new { id = image.Id },
               image);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteImage(Guid id)
        {
            return Ok();

        }

        [HttpPut("{id}")]
        public IActionResult UpdateImage(Guid id,
            [FromBody] ImageForUpdate imageForUpdate)
        {
            return Ok();
        }

    }
}
