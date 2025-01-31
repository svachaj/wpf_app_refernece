using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COS.Application
{
    public class AppStartClass : INotifyPropertyChanged
    {

        public AppStartClass() 
        {

        }

        private int _resultStartCode = 0;
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public int ResultStartCode
        {
            set
            {
                if (_resultStartCode != value)
                {
                    _resultStartCode = value;
                    OnPropertyChanged("ResultStartCode");
                }
            }
            get
            {
                return _resultStartCode;
            }
        }


        public static void Init()
        {
            Current = new AppStartClass();

        }


        public static AppStartClass Current { private set; get; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
