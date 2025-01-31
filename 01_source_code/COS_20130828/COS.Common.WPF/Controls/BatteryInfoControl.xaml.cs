using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Forms;

namespace COS.Common.WPF.Controls
{
    /// <summary>
    /// Interaction logic for BatteryInfoControl.xaml
    /// </summary>
    public partial class BatteryInfoControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public BatteryInfoControl()
        {
            InitializeComponent();

            this.DataContext = this;

            BatteryTimer.Tick += new EventHandler(BatteryTimer_Tick);
            BatteryTimer.Interval = new TimeSpan(0, 0, 1);
            BatteryTimer.Start();
        }


        void BatteryTimer_Tick(object sender, EventArgs e)
        {
            BatteryValue = SystemInformation.PowerStatus.BatteryLifePercent;
        }

        public DispatcherTimer BatteryTimer = new DispatcherTimer();

        private double _batteryValue = 0;
        public double BatteryValue
        {
            set
            {
                if (_batteryValue != value)
                {
                    _batteryValue = value * 100;
                    OnPropertyChanged("BatteryValue");
                    OnPropertyChanged("BatteryValueString");
                }
            }
            get
            {
                return _batteryValue;
            }
        }

        public string BatteryValueString
        {
            get 
            {
                return "Zbývá " + ((int)BatteryValue).ToString() + " %";
            }
        }


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
