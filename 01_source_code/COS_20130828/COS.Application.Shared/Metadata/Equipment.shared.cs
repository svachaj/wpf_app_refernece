using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class Equipment
    {
        public string DisplayName
        {
            get
            {
                return this.EquipmentName + " - " + this.EquipmentNumber;
            }
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
            get
            {
                return _isChecked;
            }
        }

        public bool IsPlanned
        {
            get
            {
                bool result = false;
                
                result = this.Plans.Count > 0 ? true : false;
                
                return result;
            }
        }

    }
}
