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
    public partial class KPIWcWgReportView : BaseUserControl
    {
        public KPIWcWgReportView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new KPIWCWGReportViewModel();
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

        KPIWCWGReportViewModel Model = null;

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
            else if (e.PropertyName == "SelectedItem1" || e.PropertyName == "SelectedItem2" || e.PropertyName == "SelectedItem3")
            {
                if (canReloadData)
                {
                    if (!_loadReportDataWorker.IsBusy)
                    {
                        Model.IsBusy = true;
                        _loadReportDataWorker.RunWorkerAsync();
                    }
                }
            }

        }


        public void ReloadData(object obj)
        {
            ReloadData1(obj);
            ReloadData2(obj);
            ReloadData3(obj);
        }

        public void ReloadData1(object obj)
        {
            // grdData.Rebind();

            chartKPI1.DefaultView.ChartArea.AxisX.Step = 1;
            chartKPI1.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
            chartKPI1.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;


            if (Model.ReportData1 != null)
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

                if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWCs)
                {
                    WorkCenter wc1 = Model.SelectedItem1 as WorkCenter;
                    if (wc1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkCenterHeader"));
                        sb.Append(wc1.Value);
                    }
                }
                else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWGs)
                {
                    WorkGroup wg1 = Model.SelectedItem1 as WorkGroup;
                    if (wg1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkGroupHeader"));
                        sb.Append(wg1.Value);
                    }
                }
                else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.SomeWCs)
                {
                    WorkCenter wc1 = Model.SelectedItem1 as WorkCenter;
                    if (wc1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkCenterHeader"));
                        sb.Append(wc1.Value);
                    }
                }


                nodates = sb.ToString();

                sb = new StringBuilder();
                if (!string.IsNullOrEmpty(nodates))
                {
                    sb.Append(dates);
                    if (!string.IsNullOrEmpty(dates))
                        sb.Append(" - ");
                    sb.Append(nodates);
                }
                else
                {
                    sb.Append(dates);
                }

                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    mainChartTitle1.Height = double.NaN;
                    tblMainChartTitle1.Text = sb.ToString();
                }
                else
                    mainChartTitle1.Height = 0;


                mainSerie1.SeriesDefinition.Appearance.Fill = Brushes.Transparent;
                mainSerie1.SeriesDefinition.Appearance.Foreground = Brushes.Black;
                mainSerie1.SeriesDefinition.Appearance.Stroke = Brushes.Blue;
                mainSerie1.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Blue;


                planSerie1.SeriesDefinition.Appearance.Fill = Brushes.Red;
                planSerie1.SeriesDefinition.Appearance.Stroke = Brushes.Red;
                planSerie1.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Transparent;
                planSerie1.SeriesDefinition.Appearance.PointMark.StrokeThickness = 0;
                planSerie1.SeriesDefinition.Appearance.PointMark.Fill = Brushes.Transparent;


                mainChartTitle1.Background = Brushes.White;
                tblMainChartTitle1.Foreground = Brushes.Black;
                tblMainChartTitle1.FontWeight = FontWeights.Bold;
                mainChartTitle1.BorderThickness = new Thickness(0);

                chartKPI1.DefaultView.ChartArea.AxisY.Title = Model.DisplayMembers[Model.DisplayMember].ToString();
                chartKPI1.DefaultView.ChartArea.AxisX.Title = Model.ShowBys[Model.SelectedShowBy];


                //if (Model.SelectedShowBy == ShowBy.Day)
                //{
                //if (Model.ReportData != null && Model.ReportData.Count > 0)
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = (int)(chartKPI.DefaultView.ChartArea.AxisX.CalculateNumberOfSteps() / Model.ReportData.Count);

                chartKPI1.DefaultView.ChartArea.AxisX.LabelRotationAngle = 30;

                if (Model.SelectedShowBy == ShowBy.All)
                    chartKPI1.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                else
                    chartKPI1.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "";

                //}
                //else
                //{
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = 1;
                //    chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
                //    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;
                //}




                mainItemMapingY1.FieldName = Model.DisplayValuesMember[Model.DisplayMember];

                mainItemMapingX1.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];
                planItemMapingX1.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];

                mainSerie1.LegendLabel = Model.DisplayMembers[Model.DisplayMember].ToString();


                double minval = 0;
                double maxval = 0;

                foreach (var itm in Model.ReportData1)
                    itm.ActualCode = Model.DisplayValuesMember[Model.DisplayMember].ToString();

                if (Model.ReportData1.Count > 0)
                {
                    switch (Model.DisplayMember)
                    {
                        case 1:
                            minval = (double)Model.ReportData1.Min(a => a.KpiOEE);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiOEE);

                            break;
                        case 2:
                            minval = (double)Model.ReportData1.Min(a => a.KpiProductivity);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiProductivity);
                            break;
                        case 3:
                            minval = (double)Model.ReportData1.Min(a => a.KpiAvailability);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiAvailability);
                            break;
                        case 4:
                            minval = (double)Model.ReportData1.Min(a => a.KpiQuality);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiQuality);
                            break;
                        case 5:
                            minval = (double)Model.ReportData1.Min(a => a.KpiPerformance);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiPerformance);
                            break;
                        case 6:
                            minval = (double)Model.ReportData1.Min(a => a.KpiProducedPcs);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiProducedPcs);
                            break;
                        case 7:
                            minval = (double)Model.ReportData1.Min(a => a.KpiActualTime);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiActualTime);
                            break;
                        case 8:
                            minval = (double)Model.ReportData1.Min(a => a.KpiActualPcsPerHeadhour);
                            maxval = (double)Model.ReportData1.Max(a => a.KpiActualPcsPerHeadhour);
                            break;
                    }

                    double minTarget = (double)Model.ReportData1.Min(a => a.TargetPlan);
                    if (minTarget < minval)
                        minval = minTarget;

                    double maxTarget = (double)Model.ReportData1.Max(a => a.TargetPlan);
                    if (maxTarget > maxval)
                        maxval = maxTarget;

                    minval -= 10;
                    maxval += 10;


                    mainaxisY1.AutoRange = false;
                    if (maxval - minval < 5)
                    {
                        mainaxisY1.MinValue = minval - 5;
                        mainaxisY1.MaxValue = maxval + 5;
                    }
                    else
                    {
                        mainaxisY1.MinValue = (int)(minval - ((maxval - minval) / 10));
                        mainaxisY1.MaxValue = (int)(maxval + ((maxval - minval) / 10));
                    }
                    mainaxisY1.Step = (int)((mainaxisY1.MaxValue - mainaxisY1.MinValue) / 5);

                    secondaxis1.AutoRange = false;
                    secondaxis1.MinValue = Math.Round(mainaxisY1.ActualMinValue);
                    secondaxis1.MaxValue = mainaxisY1.ActualMaxValue;
                    secondaxis1.Step = mainaxisY1.ActualStep;
                    secondaxis1.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            chartKPI1.ItemsSource = null;
            chartKPI1.ItemsSource = Model.ReportData1;
            chartKPI1.Rebind();

        }

        public void ReloadData2(object obj)
        {
            // grdData.Rebind();

            chartKPI2.DefaultView.ChartArea.AxisX.Step = 1;
            chartKPI2.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
            chartKPI2.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;


            if (Model.ReportData2 != null)
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

                if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWCs)
                {
                    WorkCenter wc1 = Model.SelectedItem2 as WorkCenter;
                    if (wc1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkCenterHeader"));
                        sb.Append(wc1.Value);
                    }
                }
                else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWGs)
                {
                    WorkGroup wg1 = Model.SelectedItem2 as WorkGroup;
                    if (wg1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkGroupHeader"));
                        sb.Append(wg1.Value);
                    }
                }
                else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.SomeWCs)
                {
                    WorkCenter wc1 = Model.SelectedItem2 as WorkCenter;
                    if (wc1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkCenterHeader"));
                        sb.Append(wc1.Value);
                    }
                }


                nodates = sb.ToString();

                sb = new StringBuilder();
                if (!string.IsNullOrEmpty(nodates))
                {
                    sb.Append(dates);
                    if (!string.IsNullOrEmpty(dates))
                        sb.Append(" - ");
                    sb.Append(nodates);
                }
                else
                {
                    sb.Append(dates);
                }

                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    mainChartTitle2.Height = double.NaN;
                    tblMainChartTitle2.Text = sb.ToString();
                }
                else
                    mainChartTitle2.Height = 0;


                mainSerie2.SeriesDefinition.Appearance.Fill = Brushes.Transparent;
                mainSerie2.SeriesDefinition.Appearance.Foreground = Brushes.Black;
                mainSerie2.SeriesDefinition.Appearance.Stroke = Brushes.Blue;
                mainSerie2.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Blue;


                planSerie2.SeriesDefinition.Appearance.Fill = Brushes.Red;
                planSerie2.SeriesDefinition.Appearance.Stroke = Brushes.Red;
                planSerie2.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Transparent;
                planSerie2.SeriesDefinition.Appearance.PointMark.StrokeThickness = 0;
                planSerie2.SeriesDefinition.Appearance.PointMark.Fill = Brushes.Transparent;


                mainChartTitle2.Background = Brushes.White;
                tblMainChartTitle2.Foreground = Brushes.Black;
                tblMainChartTitle2.FontWeight = FontWeights.Bold;
                mainChartTitle2.BorderThickness = new Thickness(0);

                chartKPI2.DefaultView.ChartArea.AxisY.Title = Model.DisplayMembers[Model.DisplayMember].ToString();
                chartKPI2.DefaultView.ChartArea.AxisX.Title = Model.ShowBys[Model.SelectedShowBy];


                //if (Model.SelectedShowBy == ShowBy.Day)
                //{
                //if (Model.ReportData != null && Model.ReportData.Count > 0)
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = (int)(chartKPI.DefaultView.ChartArea.AxisX.CalculateNumberOfSteps() / Model.ReportData.Count);

                chartKPI2.DefaultView.ChartArea.AxisX.LabelRotationAngle = 30;

                if (Model.SelectedShowBy == ShowBy.All)
                    chartKPI2.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                else
                    chartKPI2.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "";

                //}
                //else
                //{
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = 1;
                //    chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
                //    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;
                //}




                mainItemMapingY2.FieldName = Model.DisplayValuesMember[Model.DisplayMember];

                mainItemMapingX2.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];
                planItemMapingX2.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];

                mainSerie2.LegendLabel = Model.DisplayMembers[Model.DisplayMember].ToString();


                double minval = 0;
                double maxval = 0;

                foreach (var itm in Model.ReportData2)
                    itm.ActualCode = Model.DisplayValuesMember[Model.DisplayMember].ToString();

                if (Model.ReportData2.Count > 0)
                {
                    switch (Model.DisplayMember)
                    {
                        case 1:
                            minval = (double)Model.ReportData2.Min(a => a.KpiOEE);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiOEE);

                            break;
                        case 2:
                            minval = (double)Model.ReportData2.Min(a => a.KpiProductivity);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiProductivity);
                            break;
                        case 3:
                            minval = (double)Model.ReportData2.Min(a => a.KpiAvailability);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiAvailability);
                            break;
                        case 4:
                            minval = (double)Model.ReportData2.Min(a => a.KpiQuality);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiQuality);
                            break;
                        case 5:
                            minval = (double)Model.ReportData2.Min(a => a.KpiPerformance);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiPerformance);
                            break;
                        case 6:
                            minval = (double)Model.ReportData2.Min(a => a.KpiProducedPcs);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiProducedPcs);
                            break;
                        case 7:
                            minval = (double)Model.ReportData2.Min(a => a.KpiActualTime);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiActualTime);
                            break;
                        case 8:
                            minval = (double)Model.ReportData2.Min(a => a.KpiActualPcsPerHeadhour);
                            maxval = (double)Model.ReportData2.Max(a => a.KpiActualPcsPerHeadhour);
                            break;
                    }

                    double minTarget = (double)Model.ReportData2.Min(a => a.TargetPlan);
                    if (minTarget < minval)
                        minval = minTarget;

                    double maxTarget = (double)Model.ReportData2.Max(a => a.TargetPlan);
                    if (maxTarget > maxval)
                        maxval = maxTarget;

                    minval -= 10;
                    maxval += 10;


                    mainaxisY2.AutoRange = false;
                    if (maxval - minval < 5)
                    {
                        mainaxisY2.MinValue = minval - 5;
                        mainaxisY2.MaxValue = maxval + 5;
                    }
                    else
                    {
                        mainaxisY2.MinValue = (int)(minval - ((maxval - minval) / 10));
                        mainaxisY2.MaxValue = (int)(maxval + ((maxval - minval) / 10));
                    }
                    mainaxisY2.Step = (int)((mainaxisY2.MaxValue - mainaxisY2.MinValue) / 5);

                    secondaxis2.AutoRange = false;
                    secondaxis2.MinValue = Math.Round(mainaxisY2.ActualMinValue);
                    secondaxis2.MaxValue = mainaxisY2.ActualMaxValue;
                    secondaxis2.Step = mainaxisY2.ActualStep;
                    secondaxis2.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            chartKPI2.ItemsSource = null;
            chartKPI2.ItemsSource = Model.ReportData2;
            chartKPI2.Rebind();

        }

        public void ReloadData3(object obj)
        {
            // grdData.Rebind();

            chartKPI3.DefaultView.ChartArea.AxisX.Step = 1;
            chartKPI3.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
            chartKPI3.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;


            if (Model.ReportData3 != null)
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

                if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWCs)
                {
                    WorkCenter wc1 = Model.SelectedItem3 as WorkCenter;
                    if (wc1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkCenterHeader"));
                        sb.Append(wc1.Value);
                    }
                }
                else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWGs)
                {
                    WorkGroup wg1 = Model.SelectedItem3 as WorkGroup;
                    if (wg1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkGroupHeader"));
                        sb.Append(wg1.Value);
                    }
                }
                else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.SomeWCs)
                {
                    WorkCenter wc1 = Model.SelectedItem3 as WorkCenter;
                    if (wc1 != null)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(COS.Resources.ResourceHelper.GetResource<string>("rep_WorkCenterHeader"));
                        sb.Append(wc1.Value);
                    }
                }


                nodates = sb.ToString();

                sb = new StringBuilder();
                if (!string.IsNullOrEmpty(nodates))
                {
                    sb.Append(dates);
                    if (!string.IsNullOrEmpty(dates))
                        sb.Append(" - ");
                    sb.Append(nodates);
                }
                else
                {
                    sb.Append(dates);
                }

                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    mainChartTitle3.Height = double.NaN;
                    tblMainChartTitle3.Text = sb.ToString();
                }
                else
                    mainChartTitle3.Height = 0;


                mainSerie3.SeriesDefinition.Appearance.Fill = Brushes.Transparent;
                mainSerie3.SeriesDefinition.Appearance.Foreground = Brushes.Black;
                mainSerie3.SeriesDefinition.Appearance.Stroke = Brushes.Blue;
                mainSerie3.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Blue;


                planSerie3.SeriesDefinition.Appearance.Fill = Brushes.Red;
                planSerie3.SeriesDefinition.Appearance.Stroke = Brushes.Red;
                planSerie3.SeriesDefinition.Appearance.PointMark.Stroke = Brushes.Transparent;
                planSerie3.SeriesDefinition.Appearance.PointMark.StrokeThickness = 0;
                planSerie3.SeriesDefinition.Appearance.PointMark.Fill = Brushes.Transparent;


                mainChartTitle3.Background = Brushes.White;
                tblMainChartTitle3.Foreground = Brushes.Black;
                tblMainChartTitle3.FontWeight = FontWeights.Bold;
                mainChartTitle3.BorderThickness = new Thickness(0);

                chartKPI3.DefaultView.ChartArea.AxisY.Title = Model.DisplayMembers[Model.DisplayMember].ToString();
                chartKPI3.DefaultView.ChartArea.AxisX.Title = Model.ShowBys[Model.SelectedShowBy];


                //if (Model.SelectedShowBy == ShowBy.Day)
                //{
                //if (Model.ReportData != null && Model.ReportData.Count > 0)
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = (int)(chartKPI.DefaultView.ChartArea.AxisX.CalculateNumberOfSteps() / Model.ReportData.Count);

                chartKPI3.DefaultView.ChartArea.AxisX.LabelRotationAngle = 30;

                if (Model.SelectedShowBy == ShowBy.All)
                    chartKPI3.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                else
                    chartKPI3.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "";

                //}
                //else
                //{
                //    chartKPI.DefaultView.ChartArea.AxisX.Step = 1;
                //    chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
                //    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;
                //}




                mainItemMapingY3.FieldName = Model.DisplayValuesMember[Model.DisplayMember];

                mainItemMapingX3.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];
                planItemMapingX3.FieldName = "ShowByValue";// Model.ShowByValues[Model.SelectedShowBy];

                mainSerie3.LegendLabel = Model.DisplayMembers[Model.DisplayMember].ToString();


                double minval = 0;
                double maxval = 0;

                foreach (var itm in Model.ReportData3)
                    itm.ActualCode = Model.DisplayValuesMember[Model.DisplayMember].ToString();

                if (Model.ReportData3.Count > 0)
                {
                    switch (Model.DisplayMember)
                    {
                        case 1:
                            minval = (double)Model.ReportData3.Min(a => a.KpiOEE);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiOEE);

                            break;
                        case 2:
                            minval = (double)Model.ReportData3.Min(a => a.KpiProductivity);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiProductivity);
                            break;
                        case 3:
                            minval = (double)Model.ReportData3.Min(a => a.KpiAvailability);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiAvailability);
                            break;
                        case 4:
                            minval = (double)Model.ReportData3.Min(a => a.KpiQuality);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiQuality);
                            break;
                        case 5:
                            minval = (double)Model.ReportData3.Min(a => a.KpiPerformance);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiPerformance);
                            break;
                        case 6:
                            minval = (double)Model.ReportData3.Min(a => a.KpiProducedPcs);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiProducedPcs);
                            break;
                        case 7:
                            minval = (double)Model.ReportData3.Min(a => a.KpiActualTime);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiActualTime);
                            break;
                        case 8:
                            minval = (double)Model.ReportData3.Min(a => a.KpiActualPcsPerHeadhour);
                            maxval = (double)Model.ReportData3.Max(a => a.KpiActualPcsPerHeadhour);
                            break;
                    }

                    double minTarget = (double)Model.ReportData3.Min(a => a.TargetPlan);
                    if (minTarget < minval)
                        minval = minTarget;

                    double maxTarget = (double)Model.ReportData3.Max(a => a.TargetPlan);
                    if (maxTarget > maxval)
                        maxval = maxTarget;

                    minval -= 10;
                    maxval += 10;


                    mainaxisY3.AutoRange = false;
                    if (maxval - minval < 5)
                    {
                        mainaxisY3.MinValue = minval - 5;
                        mainaxisY3.MaxValue = maxval + 5;
                    }
                    else
                    {
                        mainaxisY3.MinValue = (int)(minval - ((maxval - minval) / 10));
                        mainaxisY3.MaxValue = (int)(maxval + ((maxval - minval) / 10));
                    }
                    mainaxisY3.Step = (int)((mainaxisY3.MaxValue - mainaxisY3.MinValue) / 5);

                    secondaxis3.AutoRange = false;
                    secondaxis3.MinValue = Math.Round(mainaxisY3.ActualMinValue);
                    secondaxis3.MaxValue = mainaxisY3.ActualMaxValue;
                    secondaxis3.Step = mainaxisY3.ActualStep;
                    secondaxis3.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            chartKPI3.ItemsSource = null;
            chartKPI3.ItemsSource = Model.ReportData3;
            chartKPI3.Rebind();

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

        bool canReloadData = true;
        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            canReloadData = false;
            KpiReportFilterWindow wnd = sender as KpiReportFilterWindow;
            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
            {
                if (!_loadReportDataWorker.IsBusy)
                {
                    if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWCs)
                    {
                        var wcList = Model.CurrentContext.WorkCenters.OrderBy(a => a.Value).ToList();
                        if (Model.SelectedDivision != null)
                            wcList = wcList.Where(a => a.ID_Division == Model.SelectedDivision.ID).ToList();

                        Model.ItemsSource1 = wcList.ToList<object>();
                        Model.ItemsSource2 = wcList.ToList<object>();
                        Model.ItemsSource3 = wcList.ToList<object>();

                        listBoxSource1.ItemsSource = null;
                        listBoxSource1.ItemsSource = Model.ItemsSource1;
                        Model.SelectedItem1 = Model.ItemsSource1.FirstOrDefault();

                        listBoxSource2.ItemsSource = null;
                        listBoxSource2.ItemsSource = Model.ItemsSource2;
                        if (Model.ItemsSource2.Count > 1)
                            Model.SelectedItem2 = Model.ItemsSource2[1];
                        else
                            Model.SelectedItem2 = Model.ItemsSource2.FirstOrDefault();

                        listBoxSource3.ItemsSource = null;
                        listBoxSource3.ItemsSource = Model.ItemsSource3;
                        if (Model.ItemsSource3.Count > 2)
                            Model.SelectedItem3 = Model.ItemsSource3[2];
                        else
                            Model.SelectedItem3 = Model.ItemsSource3.FirstOrDefault();


                    }
                    else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.AllWGs)
                    {
                        var wgList = Model.CurrentContext.WorkGroups.OrderBy(a => a.Value).ToList();
                        if (Model.SelectedDivision != null)
                            wgList = wgList.Where(a => a.ID_Division == Model.SelectedDivision.ID).ToList();

                        Model.ItemsSource1 = wgList.ToList<object>();
                        Model.ItemsSource2 = wgList.ToList<object>();
                        Model.ItemsSource3 = wgList.ToList<object>();

                        listBoxSource1.ItemsSource = null;
                        listBoxSource1.ItemsSource = Model.ItemsSource1;
                        Model.SelectedItem1 = Model.ItemsSource1.FirstOrDefault();

                        listBoxSource2.ItemsSource = null;
                        listBoxSource2.ItemsSource = Model.ItemsSource2;
                        if (Model.ItemsSource2.Count > 1)
                            Model.SelectedItem2 = Model.ItemsSource2[1];
                        else
                            Model.SelectedItem2 = Model.ItemsSource2.FirstOrDefault();

                        listBoxSource3.ItemsSource = null;
                        listBoxSource3.ItemsSource = Model.ItemsSource3;
                        if (Model.ItemsSource3.Count > 2)
                            Model.SelectedItem3 = Model.ItemsSource3[2];
                        else
                            Model.SelectedItem3 = Model.ItemsSource3.FirstOrDefault();

                    }
                    else if (Model.ReportingMode == KPIWCWGReportViewModel.ReportMode.SomeWCs)
                    {
                        var wcList = Model.CurrentContext.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == Model.SelectedWorkGroup.ID).Select(a => a.WorkCenter).OrderBy(a => a.Value).ToList();
                        if (Model.SelectedDivision != null)
                            wcList = wcList.Where(a => a.ID_Division == Model.SelectedDivision.ID).ToList();

                        Model.ItemsSource1 = wcList.ToList<object>();
                        Model.ItemsSource2 = wcList.ToList<object>();
                        Model.ItemsSource3 = wcList.ToList<object>();

                        listBoxSource1.ItemsSource = null;
                        listBoxSource1.ItemsSource = Model.ItemsSource1;
                        Model.SelectedItem1 = Model.ItemsSource1.FirstOrDefault();

                        listBoxSource2.ItemsSource = null;
                        listBoxSource2.ItemsSource = Model.ItemsSource2;
                        if (Model.ItemsSource2.Count > 1)
                            Model.SelectedItem2 = Model.ItemsSource2[1];
                        else
                            Model.SelectedItem2 = Model.ItemsSource2.FirstOrDefault();

                        listBoxSource3.ItemsSource = null;
                        listBoxSource3.ItemsSource = Model.ItemsSource3;
                        if (Model.ItemsSource3.Count > 2)
                            Model.SelectedItem3 = Model.ItemsSource3[2];
                        else
                            Model.SelectedItem3 = Model.ItemsSource3.FirstOrDefault();
                    }

                    Model.IsBusy = true;
                    _loadReportDataWorker.RunWorkerAsync();
                }
            }
            canReloadData = true;
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
