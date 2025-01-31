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
using COS.Common.WPF;
using System.ComponentModel;
using COS.Application.Shared;
using COS.Application.Production.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;

namespace COS.Application.Production.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class HourlyProductionRecalculationView : BaseUserControl
    {
        public HourlyProductionRecalculationView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new HourlyProductionRecalculationViewModel();

                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);


            }
        }

        HourlyProductionRecalculationViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {

            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LocalError")
            {
                if (!string.IsNullOrEmpty(Model.LocalError))
                {
                    RadWindow.Alert(new DialogParameters() { Content = Model.LocalError, Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
        

        }

        private void btnFilter_click(object sender, RoutedEventArgs e)
        {
            HPFilterWindow wnd = new HPFilterWindow(Model);
            wnd.Owner = this;
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
        }

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            HPFilterWindow wnd = sender as HPFilterWindow;
            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                Model.ShowDataInternal();
                grdHourlyProductions.Rebind();
                //if (!_loadReportDataWorker.IsBusy)
                //{
                //    Model.IsBusy = true;
                //    _loadReportDataWorker.RunWorkerAsync();
                //}
            }
        }




    }
}