using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Services.Contracts
{
    public interface IPropertyMappingService
    {
        Dictionary<string,PropertyMappingValue> GetPropertyMapping<TSource,TDestination>();

        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}
