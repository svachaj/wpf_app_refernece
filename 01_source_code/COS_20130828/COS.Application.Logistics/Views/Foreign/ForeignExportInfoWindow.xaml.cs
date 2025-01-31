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
    public partial class ForeignExportInfoWindow : INotifyPropertyChanged
    {
        public ForeignExportInfoWindow(ZoneLogistics zone, COSContext dataContext)
        {
            InitializeComponent();

            this.dataContext = dataContext;
            this.DataContext = this;

            LocalZone = zone;

            foreach (var itm in zone.Infos)
            {
                LocalInfos.Add(itm);
            }

            LocalInfos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LocalInfos_CollectionChanged);
        }

        COSContext dataContext = null;

        void LocalInfos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                LocalZone.Infos.Add((ForeignExportInfo)e.NewItems[0]);
                dataContext.ForeignExportInfoes.AddObject((ForeignExportInfo)e.NewItems[0]);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                LocalZone.Infos.Remove((ForeignExportInfo)e.OldItems[0]);
                dataContext.ForeignExportInfoes.DeleteObject((ForeignExportInfo)e.OldItems[0]);
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

        private ForeignExportInfo _SelectedItem = null;
        public ForeignExportInfo SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }


        private ObservableCollection<ForeignExportInfo> _LocalInfos = new ObservableCollection<ForeignExportInfo>();
        public ObservableCollection<ForeignExportInfo> LocalInfos
        {
            get
            {
                return _LocalInfos;
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
            this.DialogResult = true;
            this.Close();
        }

    }
}
