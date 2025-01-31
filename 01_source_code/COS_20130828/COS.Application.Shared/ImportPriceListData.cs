using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;

namespace COS.Application.Shared
{
    public class ImportPriceListData : ValidationViewModelBase
    {
        public ImportPriceListData()
            : base()
        {

        }

        private string _forwarder = "";
        public string Forwarder
        {
            set
            {
                _forwarder = value;
                OnPropertyChanged("Forwarder");
            }
            get
            {
                return _forwarder;
            }
        }


        private string _country ="";
        public string Country
        {
            set
            {
                _country = value;
                OnPropertyChanged("Country");
            }
            get
            {
                return _country;
            }
        }

        private string _zone = "";
        public string Zone
        {
            set
            {
                _zone = value;
                OnPropertyChanged("Zone");
            }
            get
            {
                return _zone;
            }
        }

        private double _volume;
        public double Volume
        {
            set
            {
                _volume = value;
                OnPropertyChanged("Volume");
            }
            get
            {
                return _volume;
            }
        }

        private decimal? _price;
        public decimal? Price
        {
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
            get
            {
                return _price;
            }
        }

        public bool IsNew { set; get; }

        public PriceList ExistsPriceList { set; get; }

    }

}
