using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Resources;

namespace COS.Application.Shared
{
    public partial class ViewEmployee
    {

        public string IsActive
        {
            get
            {

                string result = ResourceHelper.GetResource<string>("hr_IsActive");

                if (this.LeaveDate.HasValue)
                    result = ResourceHelper.GetResource<string>("hr_IsNotActive");

                return result;
            }
        }

    }
}
