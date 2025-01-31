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
using System.Reflection.Emit;
using System.Reflection;
using System.Resources;

namespace COS.Application.Reporting.Views
{
    /// <summary>
    /// Interaction logic for KPIReportView.xaml
    /// </summary>
    public partial class KPIWcsReportView : BaseUserControl
    {
        public KPIWcsReportView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new KPIWcsReportViewModel();
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
                localerror = COS.Resources.ResourceHelper.GetResource<string>("m_Body_E00000001");
            }
        }

        string localerror = null;

        void KPIReportView_Loaded(object sender, RoutedEventArgs e)
        {
            //mainTimeBar.VisiblePeriodStart = DateTime.Now.Date;
            //mainTimeBar.VisiblePeriodEnd = DateTime.Now.Date.AddMonths(1);
        }

        KPIWcsReportViewModel Model = null;

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

            chartKPI.DefaultView.ChartArea.AxisX.Step = 1;
            chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
            chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;


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


                //mainSerie.SeriesDefinition.Appearance.Fill = Brushes.Blue;
                //mainSerie.SeriesDefinition.Appearance.Stroke = Brushes.Blue;

                //planSerie.SeriesDefinition.Appearance.Fill = Brushes.Red;
                //planSerie.SeriesDefinition.Appearance.Stroke = Brushes.Red;

                mainChartTitle.Background = Brushes.White;
                tblMainChartTitle.Foreground = Brushes.Black;
                tblMainChartTitle.FontWeight = FontWeights.Bold;
                mainChartTitle.BorderThickness = new Thickness(0);

                chartKPI.DefaultView.ChartArea.AxisY.Title = Model.DisplayMembers[Model.DisplayMember].ToString();
                chartKPI.DefaultView.ChartArea.AxisX.Title = Model.ShowBys[Model.SelectedShowBy];


                if (Model.SelectedShowBy == ShowBy.Day)
                {
                    if (Model.ReportData != null && Model.ReportData.Count > 0)
                        chartKPI.DefaultView.ChartArea.AxisX.Step = (int)(chartKPI.DefaultView.ChartArea.AxisX.CalculateNumberOfSteps() / Model.ReportData.Count);

                    chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 90;
                    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                }
                else
                {
                    chartKPI.DefaultView.ChartArea.AxisX.Step = 1;
                    chartKPI.DefaultView.ChartArea.AxisX.LabelRotationAngle = 0;
                    chartKPI.DefaultView.ChartArea.AxisX.DefaultLabelFormat = null;
                }



                double minval = 0;
                double maxval = 0;

                foreach (var itm in Model.ReportData)
                    itm.ActualCode = Model.DisplayValuesMember[Model.DisplayMember].ToString();


            }

            chartKPI.SeriesMappings.Clear();
            SeriesMapping serie = null;
            if (Model.ReportData != null)
            {
                if (Model.ReportData.Count > 0)
                {
                    foreach (var itm in Model.ReportData.First().KpiActualTimeDict.Keys)
                    {
                        serie = new SeriesMapping();

                        serie.ChartArea = mainArea;
                        serie.LegendLabel = itm;
                        serie.SeriesDefinition = new LineSeriesDefinition();
                        serie.ItemMappings.Add(new ItemMapping("[" + Model.DisplayValuesMember[Model.DisplayMember] + itm + "]", DataPointMember.YValue));
                        serie.ItemMappings.Add(new ItemMapping("ShowByValue", DataPointMember.XCategory));

                        chartKPI.SeriesMappings.Add(serie);
                    }
                }
            }

            //SeriesMapping serie = new SeriesMapping();

            //serie.LegendLabel = "WG100";
            //serie.SeriesDefinition = new LineSeriesDefinition();
            //serie.ChartArea = mainArea;

            //serie.ItemMappings.Add(new ItemMapping("[WC100Value]", DataPointMember.YValue));

            //serie.ItemMappings.Add(new ItemMapping("[Date]", DataPointMember.XCategory));

            //chartKPI.SeriesMappings.Add(serie);





            //Dynamic dyn = new Dynamic();

            //dyn.Add<decimal>("WC100Value", 40);
            //dyn.Add<decimal>("WC400Value", 70);
            //dyn.Add<DateTime>("Date", DateTime.Now.Date);

            //List<Dynamic> dynams = new List<Dynamic>();
            //dynams.Add(dyn);

            chartKPI.ItemsSource = null;
            chartKPI.ItemsSource = Model.ReportData;
            //chartKPI.ItemsSource = dynams;

            chartKPI.Rebind();

        }


        public class Dynamic
        {
            public Dynamic Add<T>(string key, T value)
            {
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Dynamic.dll");
                TypeBuilder typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString());
                typeBuilder.SetParent(this.GetType());
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(key, PropertyAttributes.None, typeof(T), Type.EmptyTypes);

                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + key, MethodAttributes.Public, CallingConventions.HasThis, typeof(T), Type.EmptyTypes);
                ILGenerator getter = getMethodBuilder.GetILGenerator();
                getter.Emit(OpCodes.Ldarg_0);
                getter.Emit(OpCodes.Ldstr, key);
                getter.Emit(OpCodes.Callvirt, typeof(Dynamic).GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof(T)));
                getter.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(getMethodBuilder);

                Type type = typeBuilder.CreateType();

                Dynamic child = (Dynamic)Activator.CreateInstance(type);
                child.dictionary = this.dictionary;
                dictionary.Add(key, value);
                return child;
            }

            protected T Get<T>(string key)
            {
                return (T)dictionary[key];
            }

            public object this[string key]
            {
                get { return dictionary.ContainsKey(key) ? dictionary[key] : null; }

                set
                {
                    if (dictionary.ContainsKey(key))
                    {
                        dictionary[key] = value;
                    }
                    else
                    {
                        dictionary.Add(key, value);
                    }
                }
            }

            private Dictionary<string, object> dictionary = new Dictionary<string, object>();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            KpiReportFilterWindow wnd = new KpiReportFilterWindow(Model);
            wnd.Owner = this;
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
                if (Model.SelectedWorkGroup == null)
                {
                    RadWindow.Alert(new DialogParameters() { Content = COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000007"), Header = COS.Resources.ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
                else
                {
                    if (!_loadReportDataWorker.IsBusy)
                    {
                        Model.IsBusy = true;
                        _loadReportDataWorker.RunWorkerAsync();
                    }
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




    }
}
