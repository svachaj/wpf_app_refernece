using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Resources;

namespace COS.Application.Shared
{
    public class SynchronizeItem : COS.Common.NotifyBase
    {
        public string DisplayAction
        {
            get
            {
                string result = "";

                switch (Action)
                {
                    case SynchroAction.New:
                        result = ResourceHelper.GetResource<string>("hr_NewSynchronize");
                        break;
                    case SynchroAction.Delete:
                        result = ResourceHelper.GetResource<string>("hr_DeleteSynchronize");
                        break;
                    case SynchroAction.Update:
                        result = ResourceHelper.GetResource<string>("hr_UpdateSynchronize");
                        break;
                }

                return result;
            }
        }

        public SynchroAction Action { set; get; }

        private bool _isSelected = true;

        public bool IsSelected
        {
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
            get
            {
                return _isSelected;
            }
        }

        public string HR_ID { set; get; }

        public string HR_NameSurname { set; get; }

        public string DisplayName
        {
            get
            {
                return HR_NameSurname + " (" + HR_ID + ")";
            }
        }

        public Employee COSEmployee { set; get; }

        public RonEmployee RONEmployee { set; get; }

    }

    public enum SynchroAction
    {
        New,
        Delete,
        Update
    }
}
