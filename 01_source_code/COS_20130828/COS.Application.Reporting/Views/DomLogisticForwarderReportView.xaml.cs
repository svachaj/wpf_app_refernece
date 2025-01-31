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
using COS.Common;

namespace COS.Application.Reporting.Views
{

    /// <summary>
    /// LogisticDomesticLoadsReportView
    /// </summary>
    public partial class DomLogisticForwarderReportView : BaseUserControl
    {
        public DomLogisticForwarderReportView(COSContext dataContext)
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

                Model = new DomLogisticForwarderModel(this.dataContext);

                if (ReportSource == null)
                {
                    ReportSource = new Telerik.Reporting.InstanceReportSource();
                    viewerMain.ReportSource = ReportSource;
                }

                this.DataContext = Model;

                Loaded += new RoutedEventHandler(KPIReportView_Loaded);

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                _loadReportDataWorker = new BackgroundWorker();
                _loadReportDataWorker.DoWork += new DoWorkEventHandler(_loadReportDataWorker_DoWork);
                _loadReportDataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loadReportDataWorker_RunWorkerCompleted);
            }
        }

        COSContext dataContext = null;

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

        DomLogisticForwarderModel Model = null;

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
                    sb.Append(" ");
                    sb.Append(Model.SelectedForwarder.Name);
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

                tblDetailInfo.Text = COS.Resources.ResourceHelper.GetResource<string>("submod_btn_LogisticForwarderExport") + " " + sb.ToString();

                ReportSource.ReportDocument = new Reports.DomLogisticForwarder(Model);

                viewerMain.RefreshReport();

            }
        }

        public Telerik.Reporting.InstanceReportSource ReportSource { set; get; }


        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            LogisticDomesticReportFilterWindow wnd = new LogisticDomesticReportFilterWindow(Model);
            wnd.Owner = this;
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
        }

        BackgroundWorker _loadReportDataWorker = null;

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            LogisticDomesticReportFilterWindow wnd = sender as LogisticDomesticReportFilterWindow;
            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                if (!_loadReportDataWorker.IsBusy)
                {
                    if (Model.ReportData != null)
                        Model.ReportData.Clear();


                    RunReport();
                }

            }
        }

        public void RunReport() 
        {
            string err = "";
            var valid = Model.Validate(out err);

            if (valid)
            {
                if (!_loadReportDataWorker.IsBusy)
                {
                    Model.IsBusy = true;
                    _loadReportDataWorker.RunWorkerAsync();
                }
            }
            else 
            {
                RadWindow.Alert(err);
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
            RunReport();
        }

        

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RunReport();
        }
               

       


    }
}
