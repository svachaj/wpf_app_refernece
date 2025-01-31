using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class SysClass
    {
        public string Description
        {
            get
            {
                string result = null;

                var langitem = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == ID_Localization_description);
                if (langitem != null)
                    result = COSContext.Current.Language == "cs-CZ" ? langitem.cs_Czech : langitem.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + ID_Localization_description.ToString() + ")";

                return result;
            }
        }
    }
}
