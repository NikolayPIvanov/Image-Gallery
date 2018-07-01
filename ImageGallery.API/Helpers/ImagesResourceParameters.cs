using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Helpers
{
    public class ImagesResourceParameters
    {
        private const int MaxPageSize = 48;
        private int pageSize = 3;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => this.pageSize;
            set
            {
                pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

    }
}
