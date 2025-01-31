using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class OrderStatus
    {
        public string Description
        {
            get
            {
                string result = null;

                var langitem = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == this.ID_Localization);
                if (langitem != null)
                {
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                    if (string.IsNullOrEmpty(result))
                        result = "Not translated (" + this.ID_Localization.ToString() + ")";
                }
                return result;
            }
        }

        public override string ToString()
        {
            return this.Description;
        }
    }
}
