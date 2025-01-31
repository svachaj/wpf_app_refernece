using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace COS.Application.Shared
{
    public partial class VA4H
    {
        partial void OnSO_NumberChanging(string value)
        {
            //if (value.IsNullOrEmptyString())
            //    throw new ValidationException("SO number je povinné!");
        }

        public void RefreshAllValidations() 
        {
            //OnSO_NumberChanging(null);
        }
    }
}
