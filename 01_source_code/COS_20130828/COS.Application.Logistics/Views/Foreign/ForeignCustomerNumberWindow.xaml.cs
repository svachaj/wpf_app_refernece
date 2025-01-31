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
using System.Windows.Shapes;

using Telerik.Windows.Controls;
using COS.Application.Shared;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for ForeignCustomerNumberWindow.xaml
    /// </summary>
    public partial class ForeignCustomerNumberWindow : INotifyPropertyChanged
    {
        public ForeignCustomerNumberWindow(ZoneLogistics zone)
        {
            InitializeComponent();

            this.DataContext = this;

            LocalZone = zone;

            foreach (var itm in zone.t_log_foreignExport_Zone_CustomerNumber)
            {
                LocalCustomerNumbers.Add(itm);
            }

            LocalCustomerNumbers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LocalCustomerNumbers_CollectionChanged);
        }

        void LocalCustomerNumbers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                LocalZone.t_log_foreignExport_Zone_CustomerNumber.Add((ForeignExportZoneCustomerNumber)e.NewItems[0]);
                COSContext.Current.ForeignExportZoneCustomerNumbers.AddObject((ForeignExportZoneCustomerNumber)e.NewItems[0]);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                LocalZone.t_log_foreignExport_Zone_CustomerNumber.Remove((ForeignExportZoneCustomerNumber)e.OldItems[0]);
                COSContext.Current.ForeignExportZoneCustomerNumbers.DeleteObject((ForeignExportZoneCustomerNumber)e.OldItems[0]);
            }
        }

        private ZoneLogistics _localZone = null;
        public ZoneLogistics LocalZone
        {
            get
            {
                return _localZone;
            }
            set
            {
                _localZone = value;
                OnPropertyChanged("LocalZone");
            }
        }

        private ObservableCollection<ForeignExportZoneCustomerNumber> _localCustomerNumbers = new ObservableCollection<ForeignExportZoneCustomerNumber>();
        public ObservableCollection<ForeignExportZoneCustomerNumber> LocalCustomerNumbers
        {
            get
            {
                return _localCustomerNumbers;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWithoutEventsAndAnimations();
        }

    }
}
