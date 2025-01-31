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
using COS.Application.Reporting.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
using System.Threading;

namespace COS.Application.Reporting.Views
{
    /// <summary>
    /// Interaction logic for LogisticBaseReportView.xaml
    /// </summary>
    public partial class LogisticBaseReport2View : BaseUserControl
    {
        public LogisticBaseReport2View()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new LogisticBaseReport2ViewModel();
                this.DataContext = Model;

                Loaded += new RoutedEventHandler(KPIReportView_Loaded);

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                _loadReportDataWorker = new BackgroundWorker();
                _loadReportDataWorker.DoWork += new DoWorkEventHandler(_loadReportDataWorker_DoWork);
                _loadReportDataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loadReportDataWorker_RunWorkerCompleted);

                grdMainData.SelectionUnit = GridViewSelectionUnit.Cell;

            }
        }

        void _loadReportDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Model.IsBusy = false;
            if (!string.IsNullOrEmpty(localerror))
            {
                RadWindow.Alert(localerror);
            }
            else
            {
                ReloadData(null);
            }
        }


        delegate void mujdelegat(object obj);

        void _loadReportDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            localerror = null;
            try
            {
                Model.ShowReportInternal();

            }
            catch (Exception exc)
            {
                localerror = COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000001");
            }
        }

        string localerror = null;

        void KPIReportView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        LogisticBaseReport2ViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {

            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReportDataLoaded")
            {
                ReloadData(null);
            }

        }

        public void ReloadData(object obj)
        {

            if (Model.ReportData != null)
            {
                string dates = "";
                string nodates = "";
                StringBuilder sb = new StringBuilder();

                if (Model.SelectedDateFrom.HasValue)
                {
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_FromHeader"));
                    sb.Append(Model.SelectedDateFrom.Value.ToShortDateString());
                }

                if (Model.SelectedDateTo.HasValue)
                {
                    if (sb.Length > 0)
                        sb.Append("   ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_ToHeader"));
                    sb.Append(Model.SelectedDateTo.Value.ToShortDateString());
                }

                dates = sb.ToString();

                sb = new StringBuilder();

                if (Model.SelectedForwarder != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_Forwarder"));
                    sb.Append(Model.SelectedForwarder.Name);
                }

                if (Model.SelectedCountry != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_Country"));
                    sb.Append(": ");
                    sb.Append(Model.SelectedCountry.Code);
                }

                if (Model.SelectedZone != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_Destination"));
                    sb.Append(": ");
                    sb.Append(Model.SelectedZone.DisplayName);
                }

                if (Model.SelectedOrderedBy != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_OrderedBy"));
                    sb.Append(Model.SelectedOrderedBy.CustomerName);
                }

                if (Model.SelectedTransportPayment != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_TransportPayment"));
                    sb.Append(Model.SelectedTransportPayment.Description);
                }

                nodates = sb.ToString();

                sb = new StringBuilder();
                if (!string.IsNullOrEmpty(nodates))
                {
                    sb.Append(dates);
                    if (!string.IsNullOrEmpty(dates))
                        sb.Append(Environment.NewLine);
                    sb.Append(nodates);
                }
                else
                {
                    sb.Append(dates);
                }

                tblDetailInfo.Text = COS.Resources.ResourceHelper.GetResource<string>("rep_LogisticReport") + " " + sb.ToString();

                grdMainData.ItemsSource = Model.ReportData;
                grdMainData.Rebind();


            }

        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            LogisticsReportFilterWindow wnd = new LogisticsReportFilterWindow(Model);
            wnd.Owner = this;
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
           
        }

       
        BackgroundWorker _loadReportDataWorker = null;

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            LogisticsReportFilterWindow wnd = sender as LogisticsReportFilterWindow;
            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                if (!_loadReportDataWorker.IsBusy)
                {
                    if (Model.ReportData != null)
                        Model.ReportData.Clear();


                    Model.IsBusy = true;
                    _loadReportDataWorker.RunWorkerAsync();
                }

                //Model.ShowReportInternal();
                //ReloadData(null);
            }
        }



        private void cmbDisplayMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadData(null);
        }

        private void btnClearDateSelection(object sender, RoutedEventArgs e)
        {
            Model.SelectedDateFrom = null;
            Model.SelectedDateTo = null;

        }

        private void RadTimeBar_SelectionChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (!_loadReportDataWorker.IsBusy)
            {
                Model.IsBusy = true;
                _loadReportDataWorker.RunWorkerAsync();
            }
        }

        private void grdMainData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ForeignExport item = grdMainData.SelectedItem as ForeignExport;

            if (item != null)
            {
                COSContext.Current.ForeignExportToNavigateID = item.ID;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!_loadReportDataWorker.IsBusy)
            {
                if (Model.ReportData != null)
                    Model.ReportData.Clear();


                Model.IsBusy = true;
                _loadReportDataWorker.RunWorkerAsync();
            }
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            grdMainData.ClearAllColumnFilters();
        }




    }
}
