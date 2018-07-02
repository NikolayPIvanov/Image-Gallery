using AutoMapper;
using ImageGallery.API.Services.Contracts;
using ImageGallery.Settings;
using ImageGallery.Settings.Contract;
using ImageGallery.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.API.Helpers;
using System.Security.AccessControl;

namespace ImageGallery.API.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {

        private readonly IGalleryRepository galleryRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IPhotoSettings photoSettings;
        private readonly IUrlHelper urlHelper;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly ITypeHelperService typeHelperService;

        public ImagesController(IGalleryRepository galleryRepository,
                                IHostingEnvironment hostingEnvironment,
                                IPhotoSettings photoSettings,
                                IUrlHelper urlHelper,
                                IPropertyMappingService propertyMappingService,
                                ITypeHelperService typeHelperService)
        {
            this.galleryRepository = galleryRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.photoSettings = photoSettings;
            this.urlHelper = urlHelper;
            this.propertyMappingService = propertyMappingService;
            this.typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetImages")]
        public async Task<IActionResult> GetImages(ImagesResourceParameters imagesResourceParameters)
        {
            if (!this.propertyMappingService.ValidMappingExistsFor<ImageDto,Image>(imagesResourceParameters.OrderBy))
            {   
                return BadRequest();
            }

            if (!this.typeHelperService.TypeHasProperties<ImageDto>(imagesResourceParameters.Fields))
            {
                return BadRequest();
            }
            var imagesFromRepo = await galleryRepository.GetImages(imagesResourceParameters);

            var previousPageLink = imagesFromRepo.HasPrevious ? CreateImagesResourceUri(imagesResourceParameters,
                ResourceUriType.PreviousPage) : null;
            
            var nextPageLink = imagesFromRepo.HasNext ? CreateImagesResourceUri(imagesResourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = imagesFromRepo.TotalCount,
                pageSize = imagesFromRepo.PageSize,
                currentPage = imagesFromRepo.CurrentPage,
                totalPages = imagesFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination", 
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var imagesToReturn = Mapper.Map<IEnumerable<Image>>(imagesFromRepo);

            return Ok(imagesToReturn.ShapeData(imagesResourceParameters.Fields));
        }

        [HttpGet("{id}", Name = "GetImage")]
        public async Task<IActionResult> GetImage(Guid id, [FromQuery] string fields)
        {
            if (!this.typeHelperService.TypeHasProperties<ImageDto>(fields))
            {
                return BadRequest();
            }
            var imageFromRepo = await galleryRepository.GetImage(id);   

            if (imageFromRepo == null)
            {
                return NotFound();
            }

            var imageToReturn = Mapper.Map<Image>(imageFromRepo);

            return Ok(imageToReturn.ShapeData(fields));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ImageForCreation imageForCreation)
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
            var uploadFolderPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads");

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

            await galleryRepository.AddImage(image);

            if (!(await galleryRepository.Save()))
            {
                return BadRequest("An error ocurred while saving the data into the database.");
            }

            return CreatedAtRoute("GetImage",
               new { id = image.Id },
               image);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var image = await galleryRepository.GetImage(id);

            if (image == null)
            {
                return NotFound(id);
            }

            galleryRepository.DeleteImage(image);

            if (!(await galleryRepository.Save()))
            {
                throw new Exception($"Deleting image with {id} failed on save.");
            }

            return NoContent();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(Guid id,
            [FromBody] ImageForUpdate imageForUpdate)
        {
            if (imageForUpdate == null)
            {
                return BadRequest("Image is null");
            }

            if (!ModelState.IsValid)
            {
                // return 422 - Unprocessable Entity when validation fails
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            var imageFromRepo = await galleryRepository.GetImage(id);

            if (imageFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(imageForUpdate, imageFromRepo);


            if (!(await galleryRepository.Save()))
            {
                throw new Exception($"Updating image with {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}/extensions")]
        public async Task<IActionResult> UpdateExtension(Guid id, [FromBody] ImageAllowedExtensions allowedExtensions)
        {

            var imageFromRepo = await galleryRepository.GetImage(id);

            if (imageFromRepo == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // return 422 - Unprocessable Entity when validation fails
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }


            var fileName = Path.GetFileNameWithoutExtension(imageFromRepo.FileName);

            imageFromRepo.FileName = fileName + '.' + allowedExtensions.Extension;

            if (!(await galleryRepository.Save()))
            {
                throw new Exception($"Updating image with {id} failed on save.");
            }

            var imageFromRepo2 = await galleryRepository.GetImage(id);


            return Ok(imageFromRepo2);

        }

        private string CreateImagesResourceUri(ImagesResourceParameters imagesResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return this.urlHelper.Link("GetImages",
                        new
                        {
                            fields = imagesResourceParameters.Fields,
                            orderBy = imagesResourceParameters.OrderBy,
                            title = imagesResourceParameters.Title,
                            pageNumber = imagesResourceParameters.PageNumber - 1,
                            pageSize = imagesResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return this.urlHelper.Link("GetImages",
                        new
                        {
                            fields = imagesResourceParameters.Fields,
                            orderBy = imagesResourceParameters.OrderBy,
                            title = imagesResourceParameters.Title,
                            pageNumber = imagesResourceParameters.PageNumber + 1,
                            pageSize = imagesResourceParameters.PageSize
                        });
                default:
                    return this.urlHelper.Link("GetAuthors",
                        new
                        {
                            fields = imagesResourceParameters.Fields,
                            orderBy = imagesResourceParameters.OrderBy,
                            title = imagesResourceParameters.Title,
                            pageNumber = imagesResourceParameters.PageNumber,
                            pageSize = imagesResourceParameters.PageSize
                        });

            }
        }
    }
}
