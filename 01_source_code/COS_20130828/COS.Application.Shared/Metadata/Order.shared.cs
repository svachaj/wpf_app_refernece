using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class Order
    {
        public bool IsApproved
        {
            get
            {
                return this.ApproveByID.HasValue ? true : false;
            }          
        }

    }
}
