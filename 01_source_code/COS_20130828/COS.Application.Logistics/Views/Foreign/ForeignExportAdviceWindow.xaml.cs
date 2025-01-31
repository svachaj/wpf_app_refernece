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
    public partial class ForeignExportAdviceWindow : INotifyPropertyChanged
    {
        public ForeignExportAdviceWindow(ZoneLogistics zone, COSContext dataContext)
        {
            InitializeComponent();

            this.dataContext = dataContext;
            this.DataContext = this;

            LocalZone = zone;

            foreach (var itm in zone.Advices)
            {
                LocalAdvices.Add(itm);
            }

            LocalAdvices.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LocalAdvices_CollectionChanged);
        }

        COSContext dataContext = null;

        void LocalAdvices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                LocalZone.Advices.Add((ForeignExportAdvice)e.NewItems[0]);
                dataContext.ForeignExportAdvices.AddObject((ForeignExportAdvice)e.NewItems[0]);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                LocalZone.Advices.Remove((ForeignExportAdvice)e.OldItems[0]);
                dataContext.ForeignExportAdvices.DeleteObject((ForeignExportAdvice)e.OldItems[0]);
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

        private ForeignExportAdvice _SelectedItem = null;
        public ForeignExportAdvice SelectedItem
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


        private ObservableCollection<ForeignExportAdvice> _LocalAdvices = new ObservableCollection<ForeignExportAdvice>();
        public ObservableCollection<ForeignExportAdvice> LocalAdvices
        {
            get
            {
                return _LocalAdvices;
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
