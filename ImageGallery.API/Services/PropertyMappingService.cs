using ImageGallery.API.Services.Contracts;
using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> imagePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id" })},
                {"Title", new PropertyMappingValue(new List<string>() {"Title" })},
                {"Name", new PropertyMappingValue(new List<string>() {"FileName" },true)},
                {"Uploaded", new PropertyMappingValue(new List<string>() {"DateUploaded" },true)},

            };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<ImageDto, Image>(imagePropertyMapping));
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matching = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matching.Count() == 1)
            {
                var reslt = matching.First().MappingDictonary;
                return reslt;
            }

            throw new Exception($"Cannot map extact prop instance for {typeof(TSource)}, to {typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }

            }
            return true;

        }
    }

}
