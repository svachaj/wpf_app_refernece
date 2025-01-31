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
using COS.Application.WorkSafety.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;
using Telerik.Windows.Controls.GridView;

namespace COS.Application.WorkSafety.Views
{
    /// <summary>
    /// Interaction logic for TransportOrdesCoView.xaml
    /// </summary>
    public partial class AccidentView : BaseUserControl
    {
        public AccidentView(COSContext dataContext)
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

                model = new AccidentViewModel(this.dataContext);

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
                grvAccidents.ItemsSource = null;
                grvAccidents.ItemsSource = model.LocalAccidents;
            }
            else if (e.PropertyName == "AddAccident")
            {
                OpenNewAcc(null);
            }
            else if (e.PropertyName == "AddNearMiss")
            {
                OpenNewNearMiss(null);
            }
            else if (e.PropertyName == "DeleteAccident")
            {

            }
        }

        public AccidentViewModel model;
        public RadWindow DetailWindow = null;
        private AccidentDetailView detailView = null;
        private NearMissDetailView detailViewNearMiss;

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                model.ReloadDataAsync();
            }
            else
            {
                dataContext.RejectChanges();
            }
        }

        private void OpenNewAcc(AccidentLog accident)
        {
            DetailWindow = new RadWindow();

            DetailWindow.ResizeMode = ResizeMode.CanResize;
            DetailWindow.MinWidth = 850;
            DetailWindow.MinHeight = 550;
            DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
            DetailWindow.Header = ResourceHelper.GetResource<string>("log_TrpOrderWeekly");// this.Resources["Divisions"].ToString();
            DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            detailView = new AccidentDetailView(dataContext);
            detailView.RaiseWindow = DetailWindow;
            DetailWindow.Content = detailView;

            StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());

            if (accident == null)
                detailView.model.CreateNewAccident();
            else
                detailView.model.SelectedItem = accident;

            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindow.ShowDialog();
        }

        private void OpenNewNearMiss(AccidentLog nearMiss)
        {
            DetailWindow = new RadWindow();

            DetailWindow.ResizeMode = ResizeMode.CanResize;
            DetailWindow.MinWidth = 850;
            DetailWindow.MinHeight = 550;
            DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
            DetailWindow.Header = ResourceHelper.GetResource<string>("log_TrpOrderWeekly");// this.Resources["Divisions"].ToString();
            DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            detailViewNearMiss = new NearMissDetailView(dataContext);
            detailViewNearMiss.RaiseWindow = DetailWindow;
            DetailWindow.Content = detailViewNearMiss;

            StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());

            if (nearMiss == null)
                detailViewNearMiss.model.CreateNewNearMiss();
            else
                detailViewNearMiss.model.SelectedItem = nearMiss;

            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindow.ShowDialog();
        }

        private void grvAccidents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (grvAccidents.SelectedItem != null)
            {
                AccidentLog trcl = grvAccidents.SelectedItem as AccidentLog;

                if (trcl.TypeOfAccident.Code == "A") 
                {
                    OpenNewAcc(trcl);
                }
                else if (trcl.TypeOfAccident.Code == "N") 
                {
                    OpenNewNearMiss(trcl);
                }

                //DetailWindow = new RadWindow();

                //DetailWindow.ResizeMode = ResizeMode.CanResize;
                //DetailWindow.MinWidth = 850;
                //DetailWindow.MinHeight = 550;
                //DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                //DetailWindow.Header = ResourceHelper.GetResource<string>("log_TrpOrderWeekly");// this.Resources["Divisions"].ToString();
                //DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //detailView = new AccidentDetailView(dataContext);
                //detailView.RaiseWindow = DetailWindow;
                //DetailWindow.Content = detailView;

                //StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());

                //detailView.model.SelectedItem = trcl;

                //DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                //DetailWindow.ShowDialog();
            }
        }

        private void filter_click(object sender, RoutedEventArgs e)
        {
            COS.Application.WorkSafety.Views.AccidentFilterWindow fwnd = new AccidentFilterWindow(model);

            fwnd.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            fwnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            fwnd.Closed += new EventHandler<WindowClosedEventArgs>(fwnd_Closed);

            fwnd.ShowDialog();


        }

        void fwnd_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                model.ReloadDataAsync();
            }
        }

    }



}
