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
using COS.Direction;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class ForeignVolumeControlView : BaseUserControl
    {
        public ForeignVolumeControlView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new ForeignVolumeControlViewModel();
                this.DataContext = model;
                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
                this.Loaded += new RoutedEventHandler(ForeignVolumeControlView_Loaded);
            }
        }

        void ForeignVolumeControlView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        public override void RefreshData()
        {
            model.InitExports(null);

            List<IColumnFilterDescriptor> filters = new List<IColumnFilterDescriptor>();
            foreach (var itm in grvForeigns.Columns)
            {
                filters.Add(itm.ColumnFilterDescriptor);
            }

            grvForeigns.ItemsSource = null;
            grvForeigns.ItemsSource = model.LocalForeignExports;


            foreach (var flt in filters)
            {
                flt.Refresh();               
            }



            grvForeigns.Rebind();
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsCompletedChanged")
            {
                grvForeigns.Rebind();
            }
            else if (e.PropertyName == "RefreshData")
            {
                RefreshData();
            }
        }

        public ForeignVolumeControlViewModel model;

        private void grvForeigns_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            ForeignExportVolumeControl export = e.DataElement as ForeignExportVolumeControl;

            if (export != null)
            {
                if (export.IsCompleted)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(178, 161, 199));
                    e.Row.Foreground = Brushes.Black;
                }
                else
                {
                    e.Row.Background = Brushes.Transparent;
                    e.Row.Foreground = Brushes.White;
                }
            }

        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            grvForeigns.ClearAllColumnFilters();
        }

        RadWindow DetailWindow;
        ForeignVolumeControlWindowEditView detailView;
        private void grvForeigns_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DomesticDestination item = new DomesticDestination();

            if (DetailWindow == null)
            {
                DetailWindow = new RadWindow();

                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                DetailWindow.Header = "Detail";// COS.Resources.ResourceHelper.GetResource<string>("log_cbDomesticDestination");
                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                detailView = new ForeignVolumeControlWindowEditView(model);
                detailView.RaiseWindow = DetailWindow;
                DetailWindow.Content = detailView;

                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
            }
            
            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindow.ShowDialog();
        }

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                RefreshData();
            }
            else
            {
                COSContext.Current.RejectChanges();
            }
        }


    }


}
