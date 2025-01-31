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
using System.ComponentModel;
using COS.Application.Logistics.Models;
using COS.Application.Shared;
using COS.Common;

namespace COS.Application.Logistics.Views.Foreign
{
    /// <summary>
    /// Interaction logic for TimeWindowEditWindow.xaml
    /// </summary>
    public partial class TimeWindowEditWindow : INotifyPropertyChanged
    {
        public TimeWindowEditWindow(ForeignViewExportClass exportClass, COSContext cosDataContext)
        {
            InitializeComponent();
            dataContext = cosDataContext;
            ExportClass = exportClass;

            SelectedItem = dataContext.ZoneLogistics.FirstOrDefault(a => a.ID == exportClass.ID_Destination);
            
            this.DataContext = this;
        }

        COSContext dataContext;

        ForeignViewExportClass ExportClass { set; get; }

        private void btnOK_click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportClass.TimeWindow = SelectedItem.ProdTimeWindow.HasValue ? SelectedItem.ProdTimeWindow.Value.ToShortTime() : ExportClass.TimeWindow;
                dataContext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


        private ZoneLogistics _selectedItem = null;
        public ZoneLogistics SelectedItem
        {
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
