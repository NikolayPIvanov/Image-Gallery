using ImageGallery.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace ImageGallery.API.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source,
            string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictonary)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (mappingDictonary == null)
            {
                throw new ArgumentNullException("mapping Dictonary");
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {

                return source;
            }

            var orderByAfterSplit = orderBy.Split(',');

            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                var trimmedOrderByClause = orderByClause.Trim();
                var orderDesc = trimmedOrderByClause.EndsWith(" desc");
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);


                if (!mappingDictonary.ContainsKey(propertyName))
                {
                    throw new ArgumentException();
                }

                var propertyMappingValue = mappingDictonary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentException();
                }

                

                foreach (var destinationProperty in propertyMappingValue.DestinationProperty.Reverse())
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDesc = !orderDesc;
                    }
                    source = source.OrderBy(destinationProperty + (orderDesc ? " descending" : " ascending"));
                }
            }
            return source;
        }
    }
}
