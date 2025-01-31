using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class TpmCheckList_CheckListItem
    {
        private string _isInPlanExe = "";
        public string IsInPlanExe
        {
            set 
            {
                _isInPlanExe = value;
                OnPropertyChanged("IsInPlanExe");
            }
            get 
            {
                return _isInPlanExe;
            }
        }
    }
}
