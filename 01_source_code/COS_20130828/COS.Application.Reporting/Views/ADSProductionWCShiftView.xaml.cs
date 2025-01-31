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
    public partial class ADSProductionWCShiftView : BaseUserControl
    {
        public ADSProductionWCShiftView()
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
                Model = new ADSProductionWCShiftModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
                this.Loaded += new RoutedEventHandler(ReportViewerView_Loaded);
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FormattingSaved") 
            {
                if (wndFormatt != null)
                    wndFormatt.Close();
            }
        }

        void ReportViewerView_Loaded(object sender, RoutedEventArgs e)
        {
            viewerMain.TextResources = new COS.Resources.Localization.ReportViewerResources();

            cmbDivisions.ItemsSource = Model.LocalDivisions;
            cmbShiftTypes.ItemsSource = Model.LocalShiftTypes;
        }

        public ADSProductionWCShiftModel Model = null;

        ////novější verze
        public Telerik.Reporting.InstanceReportSource ReportSource { set; get; }

        private void btnShowClick(object sender, RoutedEventArgs e)
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
                Model.GenerateData();

                //novější verze
                ReportSource.ReportDocument = new Reports.ADSProductionWCShift(Model);

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


            if (Model.SelectedDivision == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000008");
                err += Environment.NewLine;
            }

            if (Model.SelectedShiftType == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000009");
                err += Environment.NewLine;
            }

            if (Model.SelectedDate == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000010");
                err += Environment.NewLine;
            }


            return err;
        }

        RadWindow wndFormatt = null;
        private void btnFormatting_click(object sender, RoutedEventArgs e)
        {
            wndFormatt = new RadWindow();

            wndFormatt.Content = new ADSProductionConditionalFormatView(Model);
            wndFormatt.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            wndFormatt.ResizeMode = ResizeMode.CanMinimize;
            wndFormatt.Header = COS.Resources.ResourceHelper.GetResource<string>("rep_CondFormatting");
            wndFormatt.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            StyleManager.SetTheme(wndFormatt, new Expression_DarkTheme());

            wndFormatt.ShowDialog();
        }

        


    }
}
