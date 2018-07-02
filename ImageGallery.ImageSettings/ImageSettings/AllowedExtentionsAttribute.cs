using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ImageGallery.Settings

{
    public class AllowedExtentionsAttribute : ValidationAttribute
    {
        //
        // Summary:
        //     Determines whether the specified value of the object is valid.
        //
        // Parameters:
        //   value:
        //     The value of the object to validate.
        //
        // Returns:
        //     true if the specified value is valid; otherwise, false.
        public override bool IsValid(object value)
        {
            string extension = value as string;
            if (Enum.IsDefined(typeof(AllowedExtensionEnum),extension))
            {
                return true;
            }

            return false;
        }

    }
}
