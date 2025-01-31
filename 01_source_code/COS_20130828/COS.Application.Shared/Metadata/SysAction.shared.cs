using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class SysAction
    {
        public string Description
        {
            get
            {
                string result = null;

                var langitem = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == ID_localization_description);
                if (langitem != null)
                    result = COSContext.Current.Language == "cs-CZ" ? langitem.cs_Czech : langitem.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + ID_localization_description.ToString() + ")";

                return result;
            }
        }

        public string Name
        {
            get
            {
                string result = null;

                var langitem = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == ID_localization_name);
                if (langitem != null)
                    result = COSContext.Current.Language == "cs-CZ" ? langitem.cs_Czech : langitem.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + ID_localization_name.ToString() + ")";

                return result;
            }
        }
    }
}
