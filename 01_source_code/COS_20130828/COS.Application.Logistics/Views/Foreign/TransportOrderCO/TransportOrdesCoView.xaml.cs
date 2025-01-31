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
using COS.Application.Shared;
using System.ComponentModel;
using COS.Application.Logistics.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;
using Telerik.Windows.Controls.GridView;
using COS.Logistics;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for TransportOrdesCoView.xaml
    /// </summary>
    public partial class TransportOrdesCoView : BaseUserControl
    {
        public TransportOrdesCoView(COSContext dataContext)
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (dataContext == null)
                {
                    var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                    string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                    this.dataContext = new COSContext(decryptString);
                }
                else
                    this.dataContext = dataContext;

                model = new TransportOrdesCoViewModel(this.dataContext);

                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ForeignsView_Loaded);
            }
        }

        COSContext dataContext = null;

        void ForeignsView_Loaded(object sender, RoutedEventArgs e)
        {
            model.ReloadDataAsync();
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataReloaded")
            {
                grvTransports.ItemsSource = null;
                grvTransports.ItemsSource = model.LocalTransports;
            }
        }

        public TransportOrdesCoViewModel model;
        public RadWindow DetailWindow = null;
        private TransportOrdesCoDetailView detailView = null;

        private void grvTransports_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnOrder_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            TransportOrderForeignExportClass trcl = btn.DataContext as TransportOrderForeignExportClass;

            //if (DetailWindow == null)
            //{
            DetailWindow = new RadWindow();

            DetailWindow.ResizeMode = ResizeMode.CanResize;
            DetailWindow.MinWidth = 850;
            DetailWindow.MinHeight = 550;
            DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
            DetailWindow.Header = ResourceHelper.GetResource<string>("log_TrpOrderWeekly");// this.Resources["Divisions"].ToString();
            DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            detailView = new TransportOrdesCoDetailView(dataContext);
            detailView.RaiseWindow = DetailWindow;
            DetailWindow.Content = detailView;

            StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
            // }

            detailView.Model.SelectedWeek = model.SelectedWeek;
            detailView.Model.SelectedYear = model.SelectedYear;
            detailView.Model.SelectedItem = trcl;
            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindow.ShowDialog();
        }

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                model.ReloadData();
            }
            else
            {
                dataContext.RejectChanges();

            }
        }

        private void grvTransports_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (grvTransports.SelectedItem != null)
                model.SetForwarderCounts(((TransportOrderForeignExportClass)grvTransports.SelectedItem).Forwarder);
        }

    }


    public class TransportOrdersToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IEnumerable<ForeignExportTransportOrderCo> transports = value as IEnumerable<ForeignExportTransportOrderCo>;

            if (transports != null && transports.Count() > 0)
            {
                ResourceDictionary resource = new ResourceDictionary
                {
                    Source = new Uri("/COSResources;component/COSBaseResources.xaml", UriKind.RelativeOrAbsolute)
                };

                return (Style)resource["IsOrderedImage"];
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
