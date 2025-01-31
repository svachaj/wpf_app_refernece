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
    /// Interaction logic for KPIReportView.xaml
    /// </summary>
    public partial class KPIReportView : BaseUserControl
    {
        public KPIReportView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new KPIReportViewModel();
                this.DataContext = Model;

                Loaded += new RoutedEventHandler(KPIReportView_Loaded);

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                Model.CurrentContext.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                _loadReportDataWorker = new BackgroundWorker();
                _loadReportDataWorker.DoWork += new DoWorkEventHandler(_loadReportDataWorker_DoWork);
                _loadReportDataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loadReportDataWorker_RunWorkerCompleted);
            }
        }

        public void Dispose()
        {
            if (Model != null)
                Model.Dispose();
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
                localerror = "Při načítání reportu došlo k chybě.";
            }
        }

        string localerror = null;

        void KPIReportView_Loaded(object sender, RoutedEventArgs e)
        {
            //mainTimeBar.VisiblePeriodStart = DateTime.Now.Date;
            //mainTimeBar.VisiblePeriodEnd = DateTime.Now.Date.AddMonths(1);
        }

        KPIReportViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {

            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "LocalError")
            //{
            //    if (!string.IsNullOrEmpty(Model.LocalError))
            //    {
            //        RadWindow.Alert(new DialogParameters() { Content = Model.LocalError, Owner = this });
            //    }
            //}
            //else if (e.PropertyName == "AddNewDowntime")
            //{
            //    grdDowntimes.BeginInsert();
            //}
            //else if (e.PropertyName == "AddNewOperator")
            //{
            //    grdOperators.BeginInsert();
            //}
            if (e.PropertyName == "ReportDataLoaded")
            {
                ReloadData(null);
            }

        }

        public void ReloadData(object obj)
        {
            // grdData.Rebind();

            chartKPI.DefaultView.ChartArea.AxisX.Step = 1;
            chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
            chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;

            var bind = tbxTotalOEE.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();

            bind = tbxTotalPerformance.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();

            bind = tbxTotalAvailability.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();

            bind = tbxTotalProductivity.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();

            bind = tbxTotalQuality.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();

            bind = tbxKpiProducedPcs.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();

            bind = tbxKpiActualTime.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();

            bind = tbxKpiActualPcsPerHeadhour.GetBindingExpression(TextBox.TextProperty);
            bind.UpdateTarget();


            if (Model.ReportData != null)
            {
                string dates = "";
                string nodates = "";
                StringBuilder sb = new StringBuilder();

                if (Model.IsWeekSelected)
                {
                    sb.Append("W" + Model.WeekFrom.ToString() + "  -  W" + Model.WeekTo.ToString()); 
                }
                else
                {
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
                }

                dates = sb.ToString();

                sb = new StringBuilder();

                if (Model.SelectedDivision != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_DivisionHeader"));
                    sb.Append(Model.SelectedDivision.Value);
                }

                if (Model.SelectedShiftType != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_ShiftTypeHeader"));
                    sb.Append(Model.SelectedShiftType.Description);
                }

                if (Model.SelectedShift != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_ShiftHeader"));
                    sb.Append(Model.SelectedShift.Description);
                }

                if (Model.SelectedWorkGroup != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkGroupHeader"));
                    sb.Append(Model.SelectedWorkGroup.Value);
                }

                if (Model.SelectedWorkCenter != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkCenterHeader"));
                    sb.Append(Model.SelectedWorkCenter.Value);
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

                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    mainChartTitle.Height = double.NaN;
                    tblMainChartTitle.Text = sb.ToString();
                }
                else
                    mainChartTitle.Height = 0;


                mainSerie.SeriesDefinition.Appearance.Fill = Brushes.Transparent;
                mainSerie.SeriesDefinition.Appearance.Foreground = Brushes.Black;
                mainSerie.SeriesDefinition.Appearance.Stroke = Brushes.Blue;
                mainSerie.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Blue;
                

                planSerie.SeriesDefinition.Appearance.Fill = Brushes.Red;
                planSerie.SeriesDefinition.Appearance.Stroke = Brushes.Red;
                planSerie.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Transparent;
                planSerie.SeriesDefinition.Appearance.PointMark.StrokeThickness = 0;
                planSerie.SeriesDefinition.Appearance.PointMark.Fill = Brushes.Transparent;
                

                mainChartTitle.Background = Brushes.White;
                tblMainChartTitle.Foreground = Brushes.Black;
                tblMainChartTitle.FontWeight = FontWeights.Bold;
                mainChartTitle.BorderThickness = new Thickness(0);

                chartKPI.DefaultView.ChartArea.AxisY.Title = Model.DisplayMembers[Model.DisplayMember].ToString();
                chartKPI.DefaultView.ChartArea.AxisX.Title = Model.ShowBys[Model.SelectedShowBy];


                //if (Model.SelectedShowBy == ShowBy.Day)
                //{
                //if (Model.ReportData != null && Model.ReportData.Count > 0)
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = (int)(chartKPI.DefaultView.ChartArea.AxisX.CalculateNumberOfSteps() / Model.ReportData.Count);

                chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 90;

                if (Model.SelectedShowBy == ShowBy.All)
                    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                else
                    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "";

                //}
                //else
                //{
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = 1;
                //    chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
                //    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;
                //}




                mainItemMapingY.FieldName = Model.DisplayValuesMember[Model.DisplayMember];

                mainItemMapingX.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];
                planItemMapingX.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];

                mainSerie.LegendLabel = Model.DisplayMembers[Model.DisplayMember].ToString();


                double minval = 0;
                double maxval = 0;

                foreach (var itm in Model.ReportData)
                    itm.ActualCode = Model.DisplayValuesMember[Model.DisplayMember].ToString();

                if (Model.ReportData.Count > 0)
                {
                    switch (Model.DisplayMember)
                    {
                        case 1:
                            minval = (double)Model.ReportData.Min(a => a.KpiOEE);
                            maxval = (double)Model.ReportData.Max(a => a.KpiOEE);

                            break;
                        case 2:
                            minval = (double)Model.ReportData.Min(a => a.KpiProductivity);
                            maxval = (double)Model.ReportData.Max(a => a.KpiProductivity);
                            break;
                        case 3:
                            minval = (double)Model.ReportData.Min(a => a.KpiAvailability);
                            maxval = (double)Model.ReportData.Max(a => a.KpiAvailability);
                            break;
                        case 4:
                            minval = (double)Model.ReportData.Min(a => a.KpiQuality);
                            maxval = (double)Model.ReportData.Max(a => a.KpiQuality);
                            break;
                        case 5:
                            minval = (double)Model.ReportData.Min(a => a.KpiPerformance);
                            maxval = (double)Model.ReportData.Max(a => a.KpiPerformance);
                            break;
                        case 6:
                            minval = (double)Model.ReportData.Min(a => a.KpiProducedPcs);
                            maxval = (double)Model.ReportData.Max(a => a.KpiProducedPcs);
                            break;
                        case 7:
                            minval = (double)Model.ReportData.Min(a => a.KpiActualTime);
                            maxval = (double)Model.ReportData.Max(a => a.KpiActualTime);
                            break;
                        case 8:
                            minval = (double)Model.ReportData.Min(a => a.KpiActualPcsPerHeadhour);
                            maxval = (double)Model.ReportData.Max(a => a.KpiActualPcsPerHeadhour);
                            break;
                    }

                    double minTarget = (double)Model.ReportData.Min(a => a.TargetPlan);
                    if (minTarget < minval)
                        minval = minTarget;

                    double maxTarget = (double)Model.ReportData.Max(a => a.TargetPlan);
                    if (maxTarget > maxval)
                        maxval = maxTarget;

                    minval -= 10;
                    maxval += 10;


                    mainaxisY.AutoRange = false;
                    if (maxval - minval < 5)
                    {
                        mainaxisY.MinValue = minval - 5;
                        mainaxisY.MaxValue = maxval + 5;
                    }
                    else
                    {
                        mainaxisY.MinValue = (int)(minval - ((maxval - minval) / 10));
                        mainaxisY.MaxValue = (int)(maxval + ((maxval - minval) / 10));
                    }
                    mainaxisY.Step = (int)((mainaxisY.MaxValue - mainaxisY.MinValue) / 5);

                    secondaxis.AutoRange = false;
                    secondaxis.MinValue = Math.Round(mainaxisY.ActualMinValue);
                    secondaxis.MaxValue = mainaxisY.ActualMaxValue;
                    secondaxis.Step = mainaxisY.ActualStep;
                    secondaxis.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            chartKPI.ItemsSource = null;
            chartKPI.ItemsSource = Model.ReportData;
            chartKPI.Rebind();



            GenerateColumns();

        }

        private void GenerateColumns()
        {
            int i = 0;

            if (Model.ReportData != null)
            {
                grdDataDynamic.Columns.Clear();

                List<decimal> valuesOEE = new List<decimal>();
                List<decimal> valuesPerformance = new List<decimal>();
                List<decimal> valuesProductivity = new List<decimal>();
                List<decimal> valuesAvailability = new List<decimal>();
                List<decimal> valuesQuality = new List<decimal>();
                List<decimal> valuesProducedPcs = new List<decimal>();
                List<decimal> valuesActualTime = new List<decimal>();
                List<decimal> valuesActPcsHeadHour = new List<decimal>();

                GridViewDataColumn col = null;

                foreach (var itm in Model.ReportData)
                {
                    col = new GridViewDataColumn();
                    col.Header = itm.ShowByValue;
                    Binding bind = new Binding("[" + i + "]");


                    col.DataMemberBinding = bind;
                    col.IsSortable = false;
                    col.IsReorderable = false;
                    col.IsFilterable = false;

                    i++;

                    grdDataDynamic.Columns.Add(col);

                    valuesOEE.Add(itm.KpiOEE);
                    valuesPerformance.Add(itm.KpiPerformance);
                    valuesProductivity.Add(itm.KpiProductivity);
                    valuesAvailability.Add(itm.KpiAvailability);
                    valuesQuality.Add(itm.KpiQuality);
                    valuesProducedPcs.Add(itm.KpiProducedPcs);
                    valuesActualTime.Add(itm.KpiActualTime);
                    valuesActPcsHeadHour.Add(itm.KpiActualPcsPerHeadhour);

                }

                List<List<decimal>> superData = new List<List<decimal>>();
                superData.Add(valuesOEE);
                superData.Add(valuesPerformance);
                superData.Add(valuesProductivity);
                superData.Add(valuesAvailability);
                superData.Add(valuesQuality);
                superData.Add(valuesProducedPcs);
                superData.Add(valuesActualTime);
                superData.Add(valuesActPcsHeadHour);

                grdDataDynamic.ItemsSource = superData;
                grdDataDynamic.Rebind();
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            KpiReportFilterWindow wnd = new KpiReportFilterWindow(Model);
            wnd.Owner = this;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
        }

        BackgroundWorker _loadReportDataWorker = null;

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            KpiReportFilterWindow wnd = sender as KpiReportFilterWindow;
            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                if (!_loadReportDataWorker.IsBusy)
                {
                    Model.IsBusy = true;
                    _loadReportDataWorker.RunWorkerAsync();
                }
            }
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            PrintDialog print = new PrintDialog();

            bool? result = print.ShowDialog();

            if (result.HasValue && result.Value)
            {
                print.PrintVisual(gridForPrint, "Report KPI");
            }
            else
                return;
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

        int igr = 0;

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!_loadReportDataWorker.IsBusy)
            {
                Model.IsBusy = true;
                _loadReportDataWorker.RunWorkerAsync();
            }
        }

       
        private void grdDataDynamic_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            //if (igr == 7 || igr == 8)
            //{
            //    foreach (var cell in e.Row.Cells)
            //    {
            //        //cell.ContentStringFormat = "n";                    
            //    }
            //}

            //igr++;
        }





    }
}
