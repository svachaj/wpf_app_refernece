using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class DomesticComposition
    {

        public string Description
        {
            get
            {
                string result = null;

                result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }
}
