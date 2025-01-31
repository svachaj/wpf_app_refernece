using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace COS.Application.Shared
{
    public partial class ProductionHRResource
    {
        public Employee Employee
        {
            get
            {
                return COSContext.Current.Employees.FirstOrDefault(a => a.HR_ID == this.ID_HR);
            }
        }

        public string HR_ID_HELP { set; get; }

    }
}
