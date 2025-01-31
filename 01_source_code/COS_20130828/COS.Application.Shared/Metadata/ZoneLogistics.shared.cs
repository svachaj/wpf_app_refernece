using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class ZoneLogistics
    {
        public string DisplayName 
        {
            get
            {
                string result = null;

                result = this.DestinationName;
                
                return result;
            }
        }

        public string CustNumbers
        {
            get
            {

                string cns = "";

                foreach (var cn in this.t_log_foreignExport_Zone_CustomerNumber.Select(a => a.cNumber))
                {
                    string cnv = cn.Replace("Z", "");
                    cnv = cnv.TrimStart('0');

                    cns += cnv;
                    cns += "/";
                }

                if (!string.IsNullOrEmpty(cns))
                {
                    cns = cns.Remove(cns.Length - 1, 1);
                }


                return cns;
            }
        }
    }

      
}
