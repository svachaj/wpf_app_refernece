using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class SysAccountType
    {
        public string Description
        {
            get
            {
                string result = null;

                //var langitem = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == ID_Localization_description);
                //if (langitem != null)
                result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not Translated (" + ID_Localization_description.ToString() + ")";

                return result;
            }
        }


        public SysLocalize Localization
        {
            set
            {
                SysLocalize loc = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == this.ID_Localization_description);

                if (loc != null)
                {
                    loc.cs_Czech = value.cs_Czech;
                    loc.en_English = value.en_English;
                }

                OnPropertyChanged("Localization");
            }
            get
            {
                SysLocalize loc = COSContext.Current.SysLocalizes.FirstOrDefault(a => a.ID == this.ID_Localization_description);

                return loc;
            }
        }
    }
}
