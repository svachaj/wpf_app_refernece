using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;

namespace COS.Application.Shared
{
    public class ImportMatrixConfiguratorData : ValidationViewModelBase
    {
        public ImportMatrixConfiguratorData()
            : base()
        {

        }

        private string _valueX = "";
        public string ValueX
        {
            set
            {
                _valueX = value;
                OnPropertyChanged("ValueX");
            }
            get
            {
                return _valueX;
            }
        }


        private string _valueY = "";
        public string ValueY
        {
            set
            {
                _valueY = value;
                OnPropertyChanged("ValueY");
            }
            get
            {
                return _valueY;
            }
        }

        private decimal _ValuePcs = 0;
        public decimal ValuePcs
        {
            set
            {
                _ValuePcs = value;
                OnPropertyChanged("ValuePcs");
            }
            get
            {
                return _ValuePcs;
            }
        }

        private int _labours = 0;
        public int Labours
        {
            set
            {
                _labours = value;
                OnPropertyChanged("Labours");
            }
            get
            {
                return _labours;
            }
        }

        private int _setupTime = 0;
        public int SetupTime
        {
            set
            {
                _setupTime = value;
                OnPropertyChanged("SetupTime");
            }
            get
            {
                return _setupTime;
            }
        }

        public int RowIndex { set; get; }
        public int ColumnIndex { set; get; }
        public bool IsHeader { set; get; }
        public string HeaderText { set; get; }
    }

}
