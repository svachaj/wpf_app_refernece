using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class Employer
    {
        public string Description
        {
            get
            {
                string result = "";
                //var langitem = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == ID_Localization_description);
                //if (langitem != null)
                result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                return result;
            }
        }
    }
}
