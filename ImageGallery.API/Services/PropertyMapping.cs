using ImageGallery.API.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappingDictonary { get; private set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictornary)
        {   
            this.MappingDictonary = mappingDictornary;
        }
    }
}
