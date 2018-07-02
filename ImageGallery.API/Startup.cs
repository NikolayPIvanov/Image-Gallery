using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.API.Services;
using ImageGallery.API.Services.Contracts;
using ImageGallery.Data;
using ImageGallery.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ImageGallery.Settings.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace ImageGallery.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GalleryContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddTransient<IGalleryRepository, GalleryRepository>();
            services.AddTransient<IPhotoSettings, ImageSettings.Settings>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
             {
                 var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                 return new UrlHelper(actionContext);
             });
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            AutoMapper.Mapper.Initialize(configure =>
            {
                configure.CreateMap<Image, Image>().ReverseMap();

                configure.CreateMap<ImageForCreation, Image>()
                    .ForMember(m => m.FileName, options => options.Ignore())
                    .ForMember(m => m.Id, options => options.Ignore());

                configure.CreateMap<ImageForUpdate, Image>()
                    .ForMember(m => m.FileName, options => options.Ignore())
                    .ForMember(m => m.Id, options => options.Ignore());

                configure.CreateMap<ImageDto,Image>()
                .ForMember(m => m.Id, options => options.MapFrom(src => src.Id))
                .ForMember(m => m.Title, options => options.MapFrom(src => src.Title))
                .ForMember(m => m.FileName, options => options.MapFrom(src => src.Name))
                .ForMember(m => m.DateUploaded, options => options.MapFrom(src => src.Uploaded));

                configure.CreateMap<Image, ImageDto>()
                .ForMember(m => m.Id, options => options.MapFrom(src => src.Id))
                .ForMember(m => m.Title, options => options.MapFrom(src => src.Title))
                .ForMember(m => m.Name, options => options.MapFrom(src => src.FileName))
                .ForMember(m => m.Uploaded, options => options.MapFrom(src => src.DateUploaded));

            });


            app.UseMvc();
        }
    }
}
