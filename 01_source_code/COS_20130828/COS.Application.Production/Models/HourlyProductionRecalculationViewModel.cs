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
using COS.Application.Production.Views;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
using System.Transactions;

namespace COS.Application.Production.Models
{
    public partial class HourlyProductionRecalculationViewModel : ValidationViewModelBase
    {
        public HourlyProductionRecalculationViewModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(HourlyProductionRecalculationViewModel_PropertyChanged);

            _recalculateWorker = new BackgroundWorker();
            _recalculateWorker.DoWork += new DoWorkEventHandler(_recalculateWorker_DoWork);
            _recalculateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_recalculateWorker_RunWorkerCompleted);
        }

        void _recalculateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            COSContext.Current.ReleaseAccessProcess(COSContext.HPRecalculation);

            IsBusy = false;

            ShowDataInternal();
        }

        void _recalculateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                RecalculateData();
            }
            catch (Exception exc) 
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                MessageBox.Show("Při přepočtu došlo k chybě.");

                COSContext.Current.ReleaseAccessProcess(COSContext.HPRecalculation);

                IsBusy = false;

                ShowDataInternal();
            }
        }

        void HourlyProductionRecalculationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDivision")
            {
                if (SelectedDivision != null)
                {
                    LocalWorkGroups = COSContext.Current.WorkGroups.Where(a => a.ID_Division == SelectedDivision.ID).ToList();
                }
            }
            else if (e.PropertyName == "SelectedWorkGroup")
            {
                if (SelectedWorkGroup != null)
                    LocalWorkCenters = COSContext.Current.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID).Select(a => a.WorkCenter).ToList();
            }
        }

        BackgroundWorker _recalculateWorker = null;

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


        private List<KPIReportData> _reportData = null;
        public List<KPIReportData> ReportData
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

        private int _currentHPCount = 10;
        public int CurrentHPCount
        {
            set
            {
                _currentHPCount = value;
                OnPropertyChanged("CurrentHPCount");
            }
            get
            {
                return _currentHPCount;
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

        public ICommand RecalculateCommand
        {
            get
            {
                return new RelayCommand(param => this.RecalculateDataInternal());
            }
        }



        public ICommand ShowDataCommand
        {
            get
            {
                return new RelayCommand(param => this.ShowDataInternal());
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

        private void ClearWorkCenter()
        {
            SelectedWorkCenter = null;
        }


        private void RecalculateDataInternal()
        {
            SysLock resultLock = null;
            try
            {
                if (LocalHourlyProductions != null && LocalHourlyProductions.Count > 0)
                {
                    if (COSContext.Current.CanAccessProcess(COSContext.HPRecalculation, out resultLock))
                    {
                        IsBusy = true;
                        CurrentHPCount = 0;

                        _recalculateWorker.RunWorkerAsync();
                    }
                    else
                    {
                        string cont = ResourceHelper.GetResource<string>("m_Body_PROD00000019");
                        cont += Environment.NewLine;
                        cont += ResourceHelper.GetResource<string>("prod_User") + " ";
                        cont += resultLock.User.Surname + " " + resultLock.User.Name;
                        cont += Environment.NewLine;
                        cont += ResourceHelper.GetResource<string>("prod_LockTime") + " ";
                        cont += resultLock.StartTime.ToString();

                        LocalError = cont;
                    }

                }
                else
                {
                    LocalError = ResourceHelper.GetResource<string>("m_Body_PROD00000010");
                }


            }
            catch (Exception exc)
            {
                if (resultLock != null)
                {
                    if (resultLock.State && resultLock.ID_User == COSContext.Current.CurrentUser.ID)
                        COSContext.Current.ReleaseAccessProcess(COSContext.HPRecalculation);
                }

                LocalError = ResourceHelper.GetResource<string>("m_Body_PROD00000020");
            }

        }

        private void RecalculateData()
        {
            ShowDataInternal();

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Standards);
            var stands = COSContext.Current.Standards.ToList();

            foreach (var hp in LocalHourlyProductions)
            {
                //System.Threading.Thread.Sleep(500);

                //vytahnutí standardu
                string itemnumber = hp.ItemNumber.Trim();
                string workcentervalue = hp.WorkCenter.Value;

                string inwc = workcentervalue + itemnumber;

                var stand = stands.FirstOrDefault(a => a.ID_Standard == inwc);

                //hodnoty pro vytahnutí ze standardu - s inicializovanými hodnotami, když standard neexistuje...
                decimal PcsPerHour = 0;
                int stdLabour = 0;
                decimal weighPcs = 0;
                int? totalPlanned = null;
                if (stand != null)
                {

                    totalPlanned = stand.SetupTime_mm;
                    PcsPerHour = stand.PcsPerHour;
                    stdLabour = stand.Labour;
                    weighPcs = stand.Weight_Kg.HasValue ? stand.Weight_Kg.Value : 0;
                    hp.StdLabour = stdLabour;
                }

                //ProductionAssets = COSContext.Current.ProductionAssets.Where(a => a.ID_HP == hp.ID_HP).ToList();
                //ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == hp.ID_HP).ToList();

                int tempSetupTime = hp.hlpOutSetupTime;
                int tempHRSetupTime = hp.hlpInSetupTime;



                //dopočítávání hodnot - počítá se se vzorci z databáze
                //nastavování proměných pro příslušné vzorce
                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("A", 1.0);
                values.Add("ActualTime", hp.ActualTime_min);
                values.Add("StdPcsHour", (double)PcsPerHour);
                values.Add("RunFactor", (double)hp.WorkCenter.RunFactor_Perc);
                values.Add("TotalLabour", (double)(hp.LabourOwn + hp.LabourTemp));

                hp.LabourTotal = hp.LabourOwn + hp.LabourTemp;

                hp.DowntimeTime_min = hp.Assets.Where(a => a.Downtime.IsLabourAffection == false).Sum(a => a.Time_min) + tempSetupTime;
                hp.HrDowntimeTime_min = hp.Assets.Where(a => a.Downtime.IsLabourAffection == true).Sum(a => a.Time_min) + tempHRSetupTime;
                hp.StdPiecesPerHour = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("StdPiecesPerHour", values), 2);

                values.Add("hrDowntime", (double)hp.HrDowntimeTime_min);
                values.Add("Downtime", (double)hp.DowntimeTime_min);
                hp.ActOperationalTime_min = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("ActOperationalTime_min", values), 2);

                values.Add("StdPcsHourCalculated", (double)hp.StdPiecesPerHour);
                hp.ActIdealTaktTime_min = (decimal)COS.Common.WPF.Helpers.CalculateFunction("ActIdealTaktTime_min", values);

                values.Add("ProducedPcs", (double)hp.ProducedPieces);
                values.Add("OperationalTime", (double)hp.ActOperationalTime_min);
                hp.ActPiecesPerHeadHour = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("ActPiecesPerHeadHour", values), 2);

                values.Add("StdLabour", stdLabour);
                hp.StdPiecesPerHeadHour = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("StdPiecesPerHeadHour", values), 2);

                values.Add("ScrapPcs", hp.ScrapPieces);
                hp.KpiQuality = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiQuality", values), 2);

                values.Add("actIdealTime", (double)hp.ActIdealTaktTime_min);
                hp.KpiPerformance = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiPerformance", values), 2);

                hp.KpiAvailability = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiAvailability", values), 2);

                hp.KpiProductivity = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiProductivity", values), 2);

                values.Add("KpiProductivity", (double)hp.KpiProductivity);
                hp.hlpHrProductivity = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("hlpHrProductivity", values), 2);

                values.Add("KpiPerformance", (double)hp.KpiPerformance);
                hp.hlpPerformance = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("hlpPerformance", values), 2);

                values.Add("WeighPcs", (double)weighPcs);
                hp.ScrapCountedWeigh = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("ScrapWeigh", values), 2);



                CurrentHPCount++;
            }

            using (TransactionScope trans = new TransactionScope())
            {
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                trans.Complete();
            }

        }

        private List<HourlyProduction> _localHourlyProductions = null;
        public List<HourlyProduction> LocalHourlyProductions
        {
            set
            {
                _localHourlyProductions = value;
                OnPropertyChanged("LocalHourlyProductions");
            }
            get
            {
                return _localHourlyProductions;
            }
        }

        public void ShowDataInternal()
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

            productions = productions.Where(a => a.CanRecalculate == null || a.CanRecalculate == true);

            productions = productions.OrderBy(a => a.Date);

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, productions);

            LocalHourlyProductions = productions.ToList();
        }


        private List<ProductionAsset> _productionAssets = null;
        public List<ProductionAsset> ProductionAssets
        {
            set
            {
                _productionAssets = value;
                OnPropertyChanged("ProductionAssets");
            }
            get
            {
                return _productionAssets;
            }
        }

        private List<ProductionHRResource> _productionHRResources = null;
        public List<ProductionHRResource> ProductionHRResources
        {
            set
            {
                _productionHRResources = value;
                OnPropertyChanged("ProductionHRResources");
            }
            get
            {
                return _productionHRResources;
            }
        }


    }




}