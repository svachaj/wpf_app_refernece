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
    public partial class DowntimeDetailRvView : BaseUserControl
    {
        public DowntimeDetailRvView()
            : base()
        {
            InitializeComponent();

            if (ReportSource == null)
            {
                ReportSource = new Telerik.Reporting.InstanceReportSource();
                viewerMain.ReportSource = ReportSource;
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new DowntimeDetailModel();
                this.DataContext = Model;

                this.Loaded += new RoutedEventHandler(ReportViewerView_Loaded);
            }
        }

        void ReportViewerView_Loaded(object sender, RoutedEventArgs e)
        {
            viewerMain.TextResources = new COS.Resources.Localization.ReportViewerResources();

            cmbWorkCenters.ItemsSource = Model.LocalWorkCenters;
            cmbDowntimes.ItemsSource = Model.LocalDowntimes;
        }

        public DowntimeDetailModel Model = null;

        public Telerik.Reporting.InstanceReportSource ReportSource { set; get; }

        private void btnShowClick(object sender, RoutedEventArgs e)
        {
            if (ReportSource == null)
            {
                ReportSource = new Telerik.Reporting.InstanceReportSource();
                viewerMain.ReportSource = ReportSource;
            }

            string err = ValidInput();

            if (string.IsNullOrEmpty(err))
            {
                Model.GenerateData();

                ReportSource.ReportDocument = new Reports.DowntimeDetail(Model);

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


            if (Model.SelectedWorkCenter == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000002");
                err += Environment.NewLine;
            }

            if (Model.SelectedDowntime == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000003");
                err += Environment.NewLine;
            }

            if (Model.SelectedDateFrom == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000004");
                err += Environment.NewLine;
            }

            if (Model.SelectedDateTo == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000005");
                err += Environment.NewLine;
            }

            return err;
        }




    }
}
