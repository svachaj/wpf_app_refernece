using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using COS.Application.Reporting.Views;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using System.Globalization;

namespace COS.Application.Reporting.Models
{
    public partial class DowntimeParetoModel : ValidationViewModelBase
    {
        public DowntimeParetoModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(KPIReportViewModel_PropertyChanged);

            SelectedCountFilter = SelectionMini.First();


        }

        void KPIReportViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDivision")
            {
                if (SelectedDivision != null)
                {
                    LocalWorkGroups = COSContext.Current.WorkGroups.Where(a => a.ID_Division == SelectedDivision.ID).OrderBy(a => a.Value).ToList();
                }
            }
            else if (e.PropertyName == "SelectedWorkGroup")
            {
                if (SelectedWorkGroup != null)
                    LocalWorkCenters = COSContext.Current.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID).Select(a => a.WorkCenter).OrderBy(a => a.Value).ToList();
            }
        }


        public ICommand ShowReport
        {
            get
            {
                return new RelayCommand(param => this.ShowReportInternal());
            }
        }

        public ICommand ClearDivisionCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearDivision());
            }
        }

        private void ClearDivision()
        {
            SelectedDivision = null;
        }

        public ICommand ClearShiftTypeCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearShiftType());
            }
        }

        private void ClearShiftType()
        {
            SelectedShiftType = null;
        }

        public ICommand ClearShiftCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearShift());
            }
        }

        private void ClearShift()
        {
            SelectedShift = null;
        }

        public ICommand ClearWorkGroupCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearWorkGroup());
            }
        }

        private void ClearWorkGroup()
        {
            SelectedWorkGroup = null;
        }

        public ICommand ClearWorkCenterCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearWorkCenter());
            }
        }



        public ICommand UseWorkGroupsCommand
        {
            get
            {
                return new RelayCommand(param => this.UseWorkGroups());
            }
        }

        private void UseWorkGroups()
        {
            if (IsUsedWorkGroups)
                UsedWorkGroupsText = ResourceHelper.GetResource<string>("rep_FiltrUnUse");
            else
                UsedWorkGroupsText = ResourceHelper.GetResource<string>("rep_FiltrUse");
        }

        public ICommand UseWorkCentersCommand
        {
            get
            {
                return new RelayCommand(param => this.UseWorkCenters());
            }
        }

        private void UseWorkCenters()
        {
            if (IsUsedWorkCenters)
                UsedWorkCentersText = ResourceHelper.GetResource<string>("rep_FiltrUnUse");
            else
                UsedWorkCentersText = ResourceHelper.GetResource<string>("rep_FiltrUse");
        }



        private void ClearWorkCenter()
        {
            SelectedWorkCenter = null;
        }

        public void ShowReportInternal()
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.HourlyProductions);

            var productions = COSContext.Current.HourlyProductions.AsQueryable();

            if (SelectedWorkGroup != null)
                productions = productions.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID);

            if (SelectedWorkCenter != null)
                productions = productions.Where(a => a.ID_WorkCenter == SelectedWorkCenter.ID);

            if (SelectedDivision != null)
                productions = productions.Where(a => a.ID_Division == SelectedDivision.ID);

            if (SelectedShift != null)
                productions = productions.Where(a => a.ID_Shift == SelectedShift.ID);

            if (SelectedShiftType != null)
                productions = productions.Where(a => a.ID_ShiftType == SelectedShiftType.ID);


            if (IsMonthSelected)
            {
                if (YearOfMonth.HasValue && MonthFrom.HasValue)
                    SelectedDateFrom = new DateTime((int)YearOfMonth, (int)MonthFrom, 1);
                else
                    SelectedDateFrom = null;

                if (YearOfMonth.HasValue && MonthTo.HasValue)
                    SelectedDateTo = new DateTime((int)YearOfMonth, (int)MonthTo, 1).AddMonths(1).AddDays(-1);
                else
                    SelectedDateTo = null;
            }
            else if (IsYearSelected)
            {
                if (YearFrom.HasValue)
                    SelectedDateFrom = new DateTime((int)YearFrom, 1, 1);
                else
                    SelectedDateFrom = null;

                if (YearTo.HasValue)
                    SelectedDateTo = new DateTime((int)YearTo, 12, 31);
                else
                    SelectedDateTo = null;
            }
            else if (IsWeekSelected)
            {
                if (WeekFrom.HasValue && YearOfWeek.HasValue)
                {
                    CultureInfo myCI = new CultureInfo("cs-CZ");
                    Calendar myCal = myCI.Calendar;

                    SelectedDateFrom = myCal.AddWeeks(new DateTime((int)YearOfWeek, 1, 2), (int)WeekFrom).AddDays(-7);

                }
                else
                    SelectedDateFrom = null;

                if (WeekTo.HasValue && YearOfWeek.HasValue)
                {
                    CultureInfo myCI = new CultureInfo("cs-CZ");
                    Calendar myCal = myCI.Calendar;

                    SelectedDateTo = myCal.AddWeeks(new DateTime((int)YearOfWeek, 1, 2), (int)WeekTo);
                }
                else
                    SelectedDateTo = null;
            }


            if (IsWeekSelected)
            {
                if (WeekFrom.HasValue && YearOfWeek.HasValue)
                    productions = productions.Where(a => a.Week >= WeekFrom && a.Date.Year == YearOfWeek);

                if (WeekTo.HasValue && YearOfWeek.HasValue)
                    productions = productions.Where(a => a.Week <= WeekTo && a.Date.Year == YearOfWeek);
            }
            else
            {
                if (SelectedDateFrom.HasValue)
                    productions = productions.Where(a => a.Date >= SelectedDateFrom);

                if (SelectedDateTo.HasValue)
                    productions = productions.Where(a => a.Date <= SelectedDateTo);
            }

            productions = productions.OrderBy(a => a.Date);

            List<HourlyProduction> prods = productions.ToList();

            if (IsUsedWorkCenters)
            {
                if (UsedWorkCenters == null)
                {
                    var wgs = SelectedDivision != null ? COSContext.Current.WorkCenters.Where(a => a.ID_Division == SelectedDivision.ID).ToList() : COSContext.Current.WorkCenters.ToList();
                    var sets = COSContext.Current.UserProperties.FirstOrDefault(a => a.ID_user == null && a.Key == "KPIReportFilterUsedWCS");

                    UsedWorkCenters = new List<WorkCenter>();

                    if (sets != null)
                    {
                        foreach (var id in sets.Value.Split(';'))
                        {
                            if (!string.IsNullOrEmpty(id))
                                UsedWorkCenters.Add(wgs.FirstOrDefault(a => a.ID == int.Parse(id)));
                        }
                    }
                }

                prods = prods.ToList().Where(a => UsedWorkCenters.Contains(a.WorkCenter)).ToList();
            }

            if (IsUsedWorkGroups)
            {
                if (UsedWorkGroups == null)
                {
                    var wgs = SelectedDivision != null ? COSContext.Current.WorkGroups.Where(a => a.ID_Division == SelectedDivision.ID).ToList() : COSContext.Current.WorkGroups.ToList();
                    var sets = COSContext.Current.UserProperties.FirstOrDefault(a => a.ID_user == null && a.Key == "KPIReportFilterUsedWGS");

                    UsedWorkGroups = new List<WorkGroup>();

                    if (sets != null)
                    {
                        foreach (var id in sets.Value.Split(';'))
                        {
                            if (!string.IsNullOrEmpty(id))
                                UsedWorkGroups.Add(wgs.FirstOrDefault(a => a.ID == int.Parse(id)));
                        }
                    }
                }

                prods = prods.ToList().Where(a => UsedWorkGroups.Contains(a.WorkGroup)).ToList();
            }

            //System.Threading.Thread.Sleep(500);

            // ReportData = prods;

            KpiActualTime = 0;
            KpiProducedPcs = 0;
            ScrapPcs = 0;
            HlpPerformance = 0;
            HlpProductivity = 0;
            OperationalTime = 0;
            ActualPcsPerHeadhour = 0;
            PlannedPcs = 0;
            KpiDowntimeTime = 0;
            HrDowntimeTime = 0;
            ScrapCountedWeight = 0;
            DowntimesSums.Clear();
                      

            var listHPids = prods.Select(a => a.ID_HP).ToList();

            var assetsList = COSContext.Current.ProductionAssets.Where(a => listHPids.Contains(a.ID_HP)).ToList();

            if (this.ShowDetail)
            {
                //var groupedHPS = prods.GroupBy(a => a.Note).ToList();
                if (SelectedCountFilter.Key == 1)
                {
                    var groupedAssets = assetsList.GroupBy(a => a.Downtime.Description).OrderByDescending(a => a.Sum(v => v.Time_min)).Take(10).ToList();

                    var sumActTime = groupedAssets.Sum(a => a.Sum(v => v.GetProduction(prods).ActualTime_min));

                    foreach (var gitm in groupedAssets)
                    {
                        DowntimesSums.Add(new DowntimeSum() { Description = gitm.Key, Time = gitm.Sum(a => a.Time_min), Rate = gitm.Count(), ActualTime = sumActTime });


                        foreach (var ast in gitm)
                        {
                            var exist = DowntimesSums.FirstOrDefault(a => a.Note == ast.GetProduction(prods).Note && a.DescriptionHide == ast.Downtime.Description);
                            if (exist != null)
                            {
                                exist.Time += ast.Time_min;
                                exist.Rate++;
                                exist.ActualTime = sumActTime;
                            }
                            else
                            {
                                DowntimesSums.Add(new DowntimeSum() { DescriptionHide = ast.Downtime.Description, Time = ast.Time_min, Rate = 1, ActualTime = sumActTime, Note = ast.GetProduction(prods).Note });
                            }
                        }
                    }
                }
                else 
                {
                    var groupedAssets = assetsList.GroupBy(a => a.Downtime.Description).OrderByDescending(a => a.Sum(v => v.Time_min)).ToList();

                    var sumActTime = groupedAssets.Sum(a => a.Sum(v => v.GetProduction(prods).ActualTime_min));

                    foreach (var gitm in groupedAssets)
                    {
                        DowntimesSums.Add(new DowntimeSum() { Description = gitm.Key, Time = gitm.Sum(a => a.Time_min), Rate = gitm.Count(), ActualTime = sumActTime });


                        foreach (var ast in gitm)
                        {
                            var exist = DowntimesSums.FirstOrDefault(a => a.Note == ast.GetProduction(prods).Note && a.DescriptionHide == ast.Downtime.Description);
                            if (exist != null)
                            {
                                exist.Time += ast.Time_min;
                                exist.Rate++;
                                exist.ActualTime = sumActTime;
                            }
                            else
                            {
                                DowntimesSums.Add(new DowntimeSum() { DescriptionHide = ast.Downtime.Description, Time = ast.Time_min, Rate = 1, ActualTime = sumActTime, Note = ast.GetProduction(prods).Note });
                            }
                        }
                    }
                }

            }
            else
            {
                if (SelectedCountFilter.Key == 1)
                {
                    var groupedAssets = assetsList.GroupBy(a => a.Downtime.Description).OrderByDescending(a => a.Sum(v => v.Time_min)).Take(10).ToList();

                    var sumActTime = groupedAssets.Sum(a => a.Sum(v => v.GetProduction(prods).ActualTime_min));
                    
                    foreach (var gitm in groupedAssets)
                    {
                        DowntimesSums.Add(new DowntimeSum() { Description = gitm.Key, Time = gitm.Sum(a => a.Time_min), Rate = gitm.Count(), ActualTime = sumActTime });
                    }
                }
                else
                {
                    var groupedAssets = assetsList.GroupBy(a => a.Downtime.Description).OrderByDescending(a => a.Sum(v => v.Time_min)).ToList();
                    
                    var sumActTime = groupedAssets.Sum(a => a.Sum(v => v.GetProduction(prods).ActualTime_min));
                  
                    foreach (var gitm in groupedAssets)
                    {
                        DowntimesSums.Add(new DowntimeSum() { Description = gitm.Key, Time = gitm.Sum(a => a.Time_min), Rate = gitm.Count(), ActualTime = sumActTime });
                    }
                }


                //DowntimesSums.Add(new DowntimeSum() { Description = COS.Resources.ResourceHelper.GetResource<string>("rep_SetupInStd"), Time = prods.Sum(a => a.hlpInSetupTime), Rate = prods.Where(a => a.hlpInSetupTime != 0).Count(), ActualTime = sumActTime });
                //DowntimesSums.Add(new DowntimeSum() { Description = COS.Resources.ResourceHelper.GetResource<string>("rep_SetupOutStd"), Time = prods.Sum(a => a.hlpOutSetupTime), Rate = prods.Where(a => a.hlpOutSetupTime != 0).Count(), ActualTime = sumActTime });
            }


        }

        public List<DowntimeSum> DowntimesSums = new List<DowntimeSum>();

        public List<ADSProductionDetailViewSumDTModel> Models = null;


        public decimal TotalOEE
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("ReportKpiQuality", (double)TotalQuality);
                values.Add("ReportKpiPerformance", (double)TotalPerformance);
                values.Add("ReportKpiAvailability", (double)TotalAvailability);


                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiOEE", values), 2);


                return result;
            }
        }


        public decimal TotalPerformance
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();
                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("hlpPerformance", (double)HlpPerformance);



                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiPerformance", values), 2);


                return result;
            }
        }


        public decimal TotalProductivity
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("hlpProductivity", (double)HlpProductivity);



                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiProductivity", values), 2);


                return result;
            }
        }


        public decimal TotalAvailability
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("ActualTime", (double)KpiActualTime);
                values.Add("OperationalTime", (double)OperationalTime);

                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiAvailability", values), 2);


                return result;
            }
        }


        public decimal TotalQuality
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("ProducedPcs", (double)KpiProducedPcs);
                values.Add("ScrapPcs", ScrapPcs);

                result = Math.Round((decimal)Calculation.CalculateFunction("ReportKpiQuality", values), 2);


                return result;
            }
        }

        private int _kpiActualTime = 0;
        public int KpiActualTime
        {
            set
            {
                _kpiActualTime = value;
                OnPropertyChanged("KpiActualTime");
            }
            get
            {
                return _kpiActualTime;
            }
        }

        public decimal ActualPcsPerHeadhour
        {
            set;
            get;
        }

        private decimal _kpiProducedPcs = 0;
         public decimal KpiProducedPcs
        {
            set
            {
                _kpiProducedPcs = value;
                OnPropertyChanged("KpiProducedPcs");
            }
            get
            {
                return _kpiProducedPcs;
            }
        }

        private int _kpiDowntimeTime = 0;
        public int KpiDowntimeTime
        {
            set
            {
                _kpiDowntimeTime = value;
                OnPropertyChanged("KpiDowntimeTime");
            }
            get
            {
                return _kpiDowntimeTime;
            }
        }

        private int _hrDowntimeTime = 0;
        public int HrDowntimeTime
        {
            set
            {
                _hrDowntimeTime = value;
                OnPropertyChanged("HrDowntimeTime");
            }
            get
            {
                return _hrDowntimeTime;
            }
        }

        public decimal KpiActualPcsPerHeadhour
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("SumActPiecesPerHeadHourOperationalTime", (double)SumActPiecesPerHeadHourOperationalTime);


                result = Math.Round((decimal)Calculation.CalculateFunction("ReportActPcsPerHeadHour", values), 2);


                return result;
            }
        }


        public decimal HlpPerformance { set; get; }

        public int ScrapPcs { set; get; }

        public decimal PlannedPcs { set; get; }

        public decimal ScrapCountedWeight { set; get; }



        public decimal OperationalTime
        {
            set;
            get;
        }

        public decimal SumActPiecesPerHeadHourOperationalTime
        {
            get
            {
                return OperationalTime == 0 ? 0 : ActualPcsPerHeadhour / ReportData.Count * OperationalTime;
            }
        }
        public decimal HlpProductivity { set; get; }



        private List<WorkGroup> _localWorkGroups = null;
        public List<WorkGroup> LocalWorkGroups
        {
            set
            {
                _localWorkGroups = value;
                OnPropertyChanged("LocalWorkGroups");
            }
            get
            {
                return _localWorkGroups;
            }
        }

        private List<WorkCenter> _usedWorkCenters = null;
        public List<WorkCenter> UsedWorkCenters
        {
            set
            {
                _usedWorkCenters = value;
                OnPropertyChanged("UsedWorkCenters");
            }
            get
            {
                return _usedWorkCenters;
            }
        }


        private List<WorkGroup> _usedWorkGroups = null;
        public List<WorkGroup> UsedWorkGroups
        {
            set
            {
                _usedWorkGroups = value;
                OnPropertyChanged("UsedWorkGroups");
            }
            get
            {
                return _usedWorkGroups;
            }
        }

        private List<WorkCenter> _localWorkCenters = null;
        public List<WorkCenter> LocalWorkCenters
        {
            set
            {
                _localWorkCenters = value;
                OnPropertyChanged("LocalWorkCenters");
            }
            get
            {
                return _localWorkCenters;
            }
        }

        private string _usedWorkCentersText = "Nastavit";
        public string UsedWorkCentersText
        {
            set
            {
                _usedWorkCentersText = value;
                OnPropertyChanged("UsedWorkCentersText");
            }
            get
            {
                return _usedWorkCentersText;
            }
        }

        private string _usedWorkGroupsText = "Nastavit";
        public string UsedWorkGroupsText
        {
            set
            {
                _usedWorkGroupsText = value;
                OnPropertyChanged("UsedWorkGroupsText");
            }
            get
            {
                return _usedWorkGroupsText;
            }
        }

        private bool _isUsedWorkGroups = false;
        public bool IsUsedWorkGroups
        {
            set
            {
                _isUsedWorkGroups = value;
                OnPropertyChanged("IsUsedWorkGroups");
            }
            get
            {
                return _isUsedWorkGroups;
            }
        }

        private bool _isUsedWorkCenters = false;
        public bool IsUsedWorkCenters
        {
            set
            {
                _isUsedWorkCenters = value;
                OnPropertyChanged("IsUsedWorkCenters");
            }
            get
            {
                return _isUsedWorkCenters;
            }
        }


        private DateTime? _selectedDateFrom = DateTime.Now.Date;
        public DateTime? SelectedDateFrom
        {
            set
            {
                _selectedDateFrom = value;
                OnPropertyChanged("SelectedDateFrom");
            }
            get
            {
                return _selectedDateFrom;
            }
        }

        private DateTime? _selectedDateTo = DateTime.Now.Date;
        public DateTime? SelectedDateTo
        {
            set
            {
                _selectedDateTo = value;
                OnPropertyChanged("SelectedDateTo");
            }
            get
            {
                return _selectedDateTo;
            }
        }

        private ShowBy _selectedShowBy = ShowBy.Day;
        public ShowBy SelectedShowBy
        {
            set
            {
                _selectedShowBy = value;
                OnPropertyChanged("SelectedShowBy");
            }
            get
            {
                return _selectedShowBy;
            }
        }


        private string _localError = "";
        public string LocalError
        {
            set
            {
                _localError = value;
                OnPropertyChanged("LocalError");
            }
            get
            {
                return _localError;
            }
        }


        private HourlyProduction _selectedHourlyProduction = null;
        public HourlyProduction SelectedHourlyProduction
        {
            set
            {
                _selectedHourlyProduction = value;
                OnPropertyChanged("SelectedHourlyProduction");
            }
            get
            {
                return _selectedHourlyProduction;
            }
        }

        private Division _selectedDivision = null;
        public Division SelectedDivision
        {
            set
            {
                _selectedDivision = value;
                OnPropertyChanged("SelectedDivision");
            }
            get
            {
                return _selectedDivision;
            }
        }

        private Shift _selectedShift = null;
        public Shift SelectedShift
        {
            set
            {
                _selectedShift = value;
                OnPropertyChanged("SelectedShift");
            }
            get
            {
                return _selectedShift;
            }
        }

        private ShiftType _selectedShiftType = null;
        public ShiftType SelectedShiftType
        {
            set
            {
                _selectedShiftType = value;
                OnPropertyChanged("SelectedShiftType");
            }
            get
            {
                return _selectedShiftType;
            }
        }



        private WorkGroup _selectedWorkGroup = null;
        public WorkGroup SelectedWorkGroup
        {
            set
            {
                _selectedWorkGroup = value;
                OnPropertyChanged("SelectedWorkGroup");
            }
            get
            {
                return _selectedWorkGroup;
            }
        }

        private WorkCenter _selectedWorkCenter = null;
        public WorkCenter SelectedWorkCenter
        {
            set
            {
                _selectedWorkCenter = value;
                OnPropertyChanged("SelectedWorkCenter");
            }
            get
            {
                return _selectedWorkCenter;
            }
        }


        private List<ParetoDataClass> _reportData = null;
        public List<ParetoDataClass> ReportData
        {
            set
            {
                _reportData = value;
                OnPropertyChanged("ReportData");
            }
            get
            {
                return _reportData;
            }
        }


        private double? _yearFrom = COSContext.Current.DateTimeServer.Year;
        public double? YearFrom
        {
            set
            {
                _yearFrom = value;
                OnPropertyChanged("YearFrom");
            }
            get
            {
                return _yearFrom;
            }
        }
        private double? _yearTo = COSContext.Current.DateTimeServer.Year;
        public double? YearTo
        {
            set
            {
                _yearTo = value;
                OnPropertyChanged("YearTo");
            }
            get
            {
                return _yearTo;
            }
        }

        private double? _monthFrom = COSContext.Current.DateTimeServer.Month;
        public double? MonthFrom
        {
            set
            {
                _monthFrom = value;
                OnPropertyChanged("MonthFrom");
            }
            get
            {
                return _monthFrom;
            }
        }

        private double? _monthTo = COSContext.Current.DateTimeServer.Month;
        public double? MonthTo
        {
            set
            {
                _monthTo = value;
                OnPropertyChanged("MonthTo");
            }
            get
            {
                return _monthTo;
            }
        }

        private double? _yearOfMonth = COSContext.Current.DateTimeServer.Year;
        public double? YearOfMonth
        {
            set
            {
                _yearOfMonth = value;
                OnPropertyChanged("YearOfMonth");
            }
            get
            {
                return _yearOfMonth;
            }
        }

        private double? _WeekFrom = COSContext.Current.Week;
        public double? WeekFrom
        {
            set
            {
                _WeekFrom = value;
                OnPropertyChanged("WeekFrom");
            }
            get
            {
                return _WeekFrom;
            }
        }

        private double? _WeekTo = COSContext.Current.Week;
        public double? WeekTo
        {
            set
            {
                _WeekTo = value;
                OnPropertyChanged("WeekTo");
            }
            get
            {
                return _WeekTo;
            }
        }

        private double? _yearOfWeek = COSContext.Current.DateTimeServer.Year;
        public double? YearOfWeek
        {
            set
            {
                _yearOfWeek = value;
                OnPropertyChanged("YearOfWeek");
            }
            get
            {
                return _yearOfWeek;
            }
        }

        private int _displayMember = 1;
        public int DisplayMember
        {
            set
            {
                _displayMember = value;
                OnPropertyChanged("DisplayMember");
            }
            get
            {
                return _displayMember;
            }
        }

        private bool _isYearSelected = false;
        public bool IsYearSelected
        {
            set
            {
                if (_isYearSelected != value)
                {
                    _isYearSelected = value;
                    OnPropertyChanged("IsYearSelected");
                }
            }
            get
            {
                return _isYearSelected;
            }
        }
        private bool _isMonthSelected = false;
        public bool IsMonthSelected
        {
            set
            {
                if (_isMonthSelected != value)
                {
                    _isMonthSelected = value;
                    OnPropertyChanged("IsMonthSelected");
                }
            }
            get
            {
                return _isMonthSelected;
            }
        }

        private bool _isWeekSelected = false;
        public bool IsWeekSelected
        {
            set
            {
                if (_isWeekSelected != value)
                {
                    _isWeekSelected = value;
                    OnPropertyChanged("IsWeekSelected");
                }
            }
            get
            {
                return _isWeekSelected;
            }
        }

        private bool _isDaySelected = true;
        public bool IsDaySelected
        {
            set
            {
                if (_isDaySelected != value)
                {
                    _isDaySelected = value;
                    OnPropertyChanged("IsDaySelected");
                }
            }
            get
            {
                return _isDaySelected;
            }
        }


        private bool _showDetail = false;
        public bool ShowDetail
        {
            set
            {
                if (_showDetail != value)
                {
                    _showDetail = value;
                    OnPropertyChanged("ShowDetail");
                }
            }
            get
            {
                return _showDetail;
            }
        }


        private KeyValuePair<int, string> _selectedCountFilter;
        public KeyValuePair<int, string> SelectedCountFilter
        {
            set
            {
                _selectedCountFilter = value;
                OnPropertyChanged("SelectedCountFilter");

            }
            get
            {
                return _selectedCountFilter;
            }
        }


        public Dictionary<int, string> SelectionMini
        {
            get
            {
                Dictionary<int, string> result = new Dictionary<int, string>();

                result.Add(1, COS.Resources.ResourceHelper.GetResource<string>("rep_T10desc"));
                result.Add(2, COS.Resources.ResourceHelper.GetResource<string>("rep_T10allDesc"));

                return result;
            }
        }

        public Dictionary<int, string> DisplayMembers
        {
            get
            {
                Dictionary<int, string> displayMembers = new Dictionary<int, string>();

                displayMembers.Add(1, COS.Resources.ResourceHelper.GetResource<string>("rep_OEE"));
                displayMembers.Add(2, COS.Resources.ResourceHelper.GetResource<string>("rep_Productivity"));
                displayMembers.Add(3, COS.Resources.ResourceHelper.GetResource<string>("rep_Availability"));
                displayMembers.Add(4, COS.Resources.ResourceHelper.GetResource<string>("rep_Quality"));
                displayMembers.Add(5, COS.Resources.ResourceHelper.GetResource<string>("rep_Performance"));
                displayMembers.Add(6, COS.Resources.ResourceHelper.GetResource<string>("rep_ProducedPcs"));
                displayMembers.Add(7, COS.Resources.ResourceHelper.GetResource<string>("rep_ActualTime"));
                displayMembers.Add(8, COS.Resources.ResourceHelper.GetResource<string>("rep_ActPcsHeadHour"));

                return displayMembers;
            }
        }

        public Dictionary<int, string> DisplayValuesMember
        {
            get
            {
                Dictionary<int, string> displayMembers = new Dictionary<int, string>();

                displayMembers.Add(1, "KpiOEE");
                displayMembers.Add(2, "KpiProductivity");
                displayMembers.Add(3, "KpiAvailability");
                displayMembers.Add(4, "KpiQuality");
                displayMembers.Add(5, "KpiPerformance");
                displayMembers.Add(6, "KpiProducedPcs");
                displayMembers.Add(7, "KpiActualTime");
                displayMembers.Add(8, "KpiActualPcsPerHeadhour");

                return displayMembers;
            }
        }


        public Dictionary<ShowBy, string> ShowBys
        {
            get
            {
                Dictionary<ShowBy, string> displayMembers = new Dictionary<ShowBy, string>();

                //displayMembers.Add(ShowBy.All, "Date");
                displayMembers.Add(ShowBy.Day, COS.Resources.ResourceHelper.GetResource<string>("rep_Day"));
                displayMembers.Add(ShowBy.Week, COS.Resources.ResourceHelper.GetResource<string>("rep_Week"));
                displayMembers.Add(ShowBy.Month, COS.Resources.ResourceHelper.GetResource<string>("rep_Month"));

                return displayMembers;
            }
        }

        public Dictionary<ShowBy, string> ShowByValues
        {
            get
            {
                Dictionary<ShowBy, string> displayMembers = new Dictionary<ShowBy, string>();

                //displayMembers.Add(ShowBy.All, "Date");
                displayMembers.Add(ShowBy.Day, "Date");
                displayMembers.Add(ShowBy.Week, "Week");
                displayMembers.Add(ShowBy.Month, "Month");

                return displayMembers;
            }
        }
    }


    public class ParetoDataClass
    {
        public ParetoDataClass()
        {
            KPIData = new KPIReportData();

        }
        public string PersonalNumber { get; set; }
        public string FullName { set; get; }
        public string WorkGroupString { set; get; }
        public string ShiftString { set; get; }
        public Boolean IsLeader { set; get; }

        public string IsLeaderString
        {
            get
            {

                if (IsLeader)
                    return ResourceHelper.GetResource<string>("rep_Yes");
                else
                    return ResourceHelper.GetResource<string>("rep_No");
            }
        }




        public KPIReportData KPIData { set; get; }


    }




}
