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
using COS.Resources;

namespace COS.Application.Reporting.Views
{
    /// <summary>
    /// Interaction logic for DowntimeDetailRvView.xaml
    /// </summary>
    public partial class ProductionEmployeesView : BaseUserControl
    {
        public ProductionEmployeesView()
            : base()
        {
            InitializeComponent();

            viewerMain.ReportSource = new Telerik.Reporting.InstanceReportSource();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new ProductionEmployeesModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
                this.Loaded += new RoutedEventHandler(ReportViewerView_Loaded);

                _loadReportDataWorker = new BackgroundWorker();
                _loadReportDataWorker.DoWork += new DoWorkEventHandler(_loadReportDataWorker_DoWork);
                _loadReportDataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loadReportDataWorker_RunWorkerCompleted);
            }
        }


        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

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
                Model.GenerateData();

            }
            catch (Exception exc)
            {
                localerror = COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000001");
            }
        }

        string localerror = null;

        void ReportViewerView_Loaded(object sender, RoutedEventArgs e)
        {
            viewerMain.TextResources = new COS.Resources.Localization.ReportViewerResources();
        }

        public ProductionEmployeesModel Model = null;

        ////novější verze
        public Telerik.Reporting.InstanceReportSource ReportSource { set; get; }

        public void ReloadData(object param)
        {
            //novější verze
            if (ReportSource == null)
            {
                ReportSource = new Telerik.Reporting.InstanceReportSource();
                viewerMain.ReportSource = ReportSource;
            }

            string err = ValidInput();

            if (string.IsNullOrEmpty(err))
            {
                //Model.GenerateData();
               
                ReportSource.ReportDocument = new Reports.ProductionEmployees(Model);

                viewerMain.RefreshReport();

            }
            else
            {
                MessageBox.Show(err);
            }
        }

        private string ValidInput()
        {
            string err = "";


            if (Model.SelectedBonusGroup == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000011");
                err += Environment.NewLine;
            }

            //if (Model.SelectedShiftType == null)
            //{
            //    err += "Vyberte typ směny";//COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000003");
            //    err += Environment.NewLine;
            //}

            //if (Model.SelectedDate == null)
            //{
            //    err += "Vyberte datum";// COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000004");
            //    err += Environment.NewLine;
            //}


            return err;
        }


        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            ProductionEmployeesFilterWindow wnd = new ProductionEmployeesFilterWindow(Model);
            wnd.Owner = this;
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
        }

        BackgroundWorker _loadReportDataWorker = null;

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            ProductionEmployeesFilterWindow wnd = sender as ProductionEmployeesFilterWindow;
            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                if (!_loadReportDataWorker.IsBusy)
                {
                    Model.IsBusy = true;
                    _loadReportDataWorker.RunWorkerAsync();
                }
            }
        }

    }
}
