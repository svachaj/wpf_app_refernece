using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;

namespace COS.Application.Shared
{
    public class ImportStandardData : ValidationViewModelBase
    {
        public ImportStandardData()
            : base()
        {

        }

        private string _itemNumberText ="";
        public string ItemNumberText
        {
            set
            {
                _itemNumberText = value;
                OnPropertyChanged("ItemNumberText");
            }
            get
            {
                return _itemNumberText;
            }
        }

        private string _isConfigText = "";
        public string IsConfigText
        {
            set
            {
                _isConfigText = value;
                OnPropertyChanged("IsConfigText");
            }
            get
            {
                return _isConfigText;
            }
        }

        private string _descriptionText = "";
        public string DescriptionText
        {
            set
            {
                _descriptionText = value;
                OnPropertyChanged("DescriptionText");
            }
            get
            {
                return _descriptionText;
            }
        }

        private string _laboursText = "";
        public string LaboursText
        {
            set
            {
                _laboursText = value;
                OnPropertyChanged("LaboursText");
            }
            get
            {
                return _laboursText;
            }
        }

        private string _workGroupText = "";
        public string WorkGroupText
        {
            set
            {
                _workGroupText = value;
                OnPropertyChanged("WorkGroupText");
            }
            get
            {
                return _workGroupText;
            }
        }

        private string _workCenterText = "";
        public string WorkCenterText
        {
            set
            {
                _workCenterText = value;
                OnPropertyChanged("WorkCenterText");
            }
            get
            {
                return _workCenterText;
            }
        }

        private string _itemGroupsText = "";
        public string ItemGroupsText
        {
            set
            {
                _itemGroupsText = value;
                OnPropertyChanged("ItemGroupsText");
            }
            get
            {
                return _itemGroupsText;
            }
        }

        private string _weighText = "";
        public string WeighText
        {
            set
            {
                _weighText = value;
                OnPropertyChanged("WeighText");
            }
            get
            {
                return _weighText;
            }
        }

        private string _setupTimeText = "";
        public string SetupTimeText
        {
            set
            {
                _setupTimeText = value;
                OnPropertyChanged("SetupTimeText");
            }
            get
            {
                return _setupTimeText;
            }
        }

        private string _pcsPerMinText = "";
        public string PcsPerMinText
        {
            set
            {
                _pcsPerMinText = value;
                OnPropertyChanged("PcsPerMinText");
            }
            get
            {
                return _pcsPerMinText;
            }
        }

        private string _pcsPerHourText = "";
        public string PcsPerHourText
        {
            set
            {
                _pcsPerHourText = value;
                OnPropertyChanged("PcsPerHourText");
            }
            get
            {
                return _pcsPerHourText;
            }
        }

        private Standard _newStandard = null;
        public Standard NewStandard
        {
            set
            {
                _newStandard = value;
                OnPropertyChanged("NewStandard");
            }
            get
            {
                return _newStandard;
            }
        }


        private Standard _existingStandard = null;
        public Standard ExistingStandard
        {
            set
            {
                _existingStandard = value;
                OnPropertyChanged("ExistingStandard");
            }
            get
            {
                return _existingStandard;
            }
        }
        
      
    }

}
