using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class ForeignExportTransportOrderCo
    {

        public bool IsNew
        {
            get
            {
                return this.ID == 0;
            }
        }
    }
}
