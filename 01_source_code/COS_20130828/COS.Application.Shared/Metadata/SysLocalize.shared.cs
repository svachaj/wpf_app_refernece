using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class SysLocalize
    {
        public static SysLocalize CreateNewLocalize()
        {
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysLocalizes);
            
            SysLocalize tempLocalize = new SysLocalize();
            tempLocalize.ID = COSContext.Current.SysLocalizes.Max(a => a.ID) + 1;
            COSContext.Current.SysLocalizes.AddObject(tempLocalize);
            
            return tempLocalize;
        }
    }
}
