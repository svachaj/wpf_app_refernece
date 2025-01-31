using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class DomesticExportDetail
    {

        public string TrpSosString
        {
            get
            {
                string result = "";

                foreach (var itm in this.TrpSos)
                {
                    result += itm.SO;
                    if (itm != this.TrpSos.LastOrDefault())
                        result += "; ";

                }

                return result;
            }
        }


    }
}
