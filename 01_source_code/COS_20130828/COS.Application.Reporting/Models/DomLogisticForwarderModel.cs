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
    public partial class DomLogisticForwarderModel : ValidationViewModelBase
    {
        public DomLogisticForwarderModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;

            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(LogisticBaseReportViewModel_PropertyChanged);

            LocalCountries = this.dataContext.Countries.ToList().OrderBy(a => a.Code ).ToList();
            LocalForwarders = this.dataContext.DomesticForwarders.ToList().OrderBy(a => a.Name).ToList();
            LocalCarTypes = this.dataContext.DomesticCarTypes.ToList().OrderBy(a => a.CarTypeName).ToList();
            LocalCostCenters = this.dataContext.CostCenters.ToList().OrderBy(a => a.Description).ToList();
            LocalDestinations = this.dataContext.DomesticDestinations.ToList().OrderBy(a => a.DestinationName).ToList();
            LocalPointOfOrigins = LocalDestinations.Where(a => a.IsPointOfOrigin == true).ToList();
            LocalCustomers = this.dataContext.DomesticCustomers.ToList().OrderBy(a => a.CustomerName).ToList();

        }

        COSContext dataContext = null;

        void LogisticBaseReportViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedCountry")
            {
                if (SelectedCountry != null)
                {

                }
            }
        }


        public ICommand ShowReport
        {
            get
            {
                return new RelayCommand(param => this.ShowReportInternal());
            }
        }


        public ICommand ClearForwarderCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearForwarder());
            }
        }

        public ICommand ClearCarTypeCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearCarType());
            }
        }

        public ICommand ClearDestinationCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearDestination());
            }
        }

        public ICommand ClearCostCenterCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearCostCenter());
            }
        }

        public ICommand ClearCountryCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearCountry());
            }
        }

        public ICommand ClearPointOfOriginCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearPointOfOrigin());
            }
        }

        public ICommand ClearCustomerCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearCustomer());
            }
        }


        private void ClearForwarder()
        {
            SelectedForwarder = null;
        }

        private void ClearCarType()
        {
            SelectedCarType = null;
        }

        private void ClearCountry()
        {
            SelectedCountry = null;
        }

        private void ClearPointOfOrigin()
        {
            SelectedPointOfOrigin = null;
        }

        private void ClearDestination()
        {
            SelectedDestination = null;
        }

        private void ClearCostCenter()
        {
            SelectedCostCenter = null;
        }

        private void ClearCustomer()
        {
            SelectedCustomer = null;
        }

        public bool Validate(out string error) 
        {
            bool result = true;
            error = "";

            if (SelectedForwarder == null) 
            {
                error += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000015");
                error += Environment.NewLine;
                result = false;
            }

            return result;
        }

        public void ShowReportInternal()
        {

            var logistics = this.dataContext.DomesticExports.AsQueryable();

            if (SelectedCarType != null)
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.Export.ID_carType == SelectedCarType.ID).Count() > 0);

            if (SelectedCostCenter != null)
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.ID_costCenter == SelectedCostCenter.ID).Count() > 0);

            if (SelectedCountry != null)
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.Destination.ID_country == SelectedCountry.ID).Count() > 0);

            if (SelectedCustomer != null)
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.ID_customer == SelectedCustomer.ID).Count() > 0);

            if (SelectedPointOfOrigin != null)
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.Export.ID_pointOfOrigin == SelectedPointOfOrigin.ID).Count() > 0);

            if (!SelectedSoNumber.IsNullOrEmptyString())
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.TrpSos.Where(x => x.SO == SelectedSoNumber).Count() > 0).Count() > 0);

            if (SelectedForwarder != null)
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.Export.ID_forwarder == SelectedForwarder.ID).Count() > 0);

            if (SelectedDestination != null)
                logistics = logistics.Where(a => a.ExportDetails.Where(c => c.ID_destination == SelectedDestination.ID).Count() > 0);


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

                    SelectedDateFrom = (Helpers.FirstDateOfWeek((int)YearOfWeek, (int)WeekFrom, CalendarWeekRule.FirstFourDayWeek)).AddDays(-7);

                }
                else
                    SelectedDateFrom = null;

                if (WeekTo.HasValue && YearOfWeek.HasValue)
                {
                    CultureInfo myCI = new CultureInfo("cs-CZ");
                    Calendar myCal = myCI.Calendar;

                    SelectedDateTo = (Helpers.FirstDateOfWeek((int)YearOfWeek, (int)WeekTo, CalendarWeekRule.FirstFourDayWeek).AddDays(6)).AddDays(-7);
                }
                else
                    SelectedDateTo = null;
            }



            if (SelectedDateFrom.HasValue)
                logistics = logistics.Where(a => a.PlannedDate >= SelectedDateFrom);

            if (SelectedDateTo.HasValue)
                logistics = logistics.Where(a => a.PlannedDate <= SelectedDateTo);

            logistics = logistics.OrderBy(a => a.PlannedDate);

            this.dataContext.Refresh(System.Data.Objects.RefreshMode.StoreWins, logistics);

            var dd = logistics.ToList();

            if (ReportData != null)
                ReportData.Clear();

            ReportData = dd;          
        }
               
        private decimal _forwarderPrice = 0;
        public decimal ForwarderPrice
        {
            set
            {
                _forwarderPrice = value;
                OnPropertyChanged("ForwarderPrice");
            }
            get
            {
                return _forwarderPrice;
            }
        }



        private decimal _totalPrice = 0;
        public decimal TotalPrice
        {
            set
            {
                _totalPrice = value;
                OnPropertyChanged("TotalPrice");
            }
            get
            {
                return _totalPrice;
            }
        }


        private decimal _bafPrice = 0;
        public decimal BafPrice
        {
            set
            {
                _bafPrice = value;
                OnPropertyChanged("BafPrice");
            }
            get
            {
                return _bafPrice;
            }
        }


        private decimal _volumeCBM = 0;
        public decimal VolumeCBM
        {
            set
            {
                _volumeCBM = value;
                OnPropertyChanged("VolumeCBM");
            }
            get
            {
                return _volumeCBM;
            }
        }

        private decimal _transportCount = 0;
        public decimal TransportCount
        {
            set
            {
                _transportCount = value;
                OnPropertyChanged("TransportCount");
            }
            get
            {
                return _transportCount;
            }
        }


        private List<DomesticCarType> _localCarTypes = null;
        public List<DomesticCarType> LocalCarTypes
        {
            set
            {
                _localCarTypes = value;
                OnPropertyChanged("LocalCarTypes");
            }
            get
            {
                return _localCarTypes;
            }
        }

        private List<DomesticCustomer> _localCustomers = null;
        public List<DomesticCustomer> LocalCustomers
        {
            set
            {
                _localCustomers = value;
                OnPropertyChanged("LocalCustomers");
            }
            get
            {
                return _localCustomers;
            }
        }

        private List<CostCenter> _localCostCenters = null;
        public List<CostCenter> LocalCostCenters
        {
            set
            {
                _localCostCenters = value;
                OnPropertyChanged("LocalCostCenters");
            }
            get
            {
                return _localCostCenters;
            }
        }

        private List<Country> _localCountries = null;
        public List<Country> LocalCountries
        {
            set
            {
                _localCountries = value;
                OnPropertyChanged("LocalCountries");
            }
            get
            {
                return _localCountries;
            }
        }


        private List<DomesticDestination> _localPointOfOrigins = null;
        public List<DomesticDestination> LocalPointOfOrigins
        {
            set
            {
                _localPointOfOrigins = value;
                OnPropertyChanged("LocalPointOfOrigins");
            }
            get
            {
                return _localPointOfOrigins;
            }
        }

        private List<DomesticDestination> _localDestinations = null;
        public List<DomesticDestination> LocalDestinations
        {
            set
            {
                _localDestinations = value;
                OnPropertyChanged("LocalDestinations");
            }
            get
            {
                return _localDestinations;
            }
        }

        private List<DomesticForwarder> _localForwarders = null;
        public List<DomesticForwarder> LocalForwarders
        {
            set
            {
                _localForwarders = value;
                OnPropertyChanged("LocalForwarders");
            }
            get
            {
                return _localForwarders;
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



        private DomesticCarType _selectedCarType = null;
        public DomesticCarType SelectedCarType
        {
            set
            {
                _selectedCarType = value;
                OnPropertyChanged("SelectedCarType");
            }
            get
            {
                return _selectedCarType;
            }
        }

        private DomesticCustomer _selectedCustomer = null;
        public DomesticCustomer SelectedCustomer
        {
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged("SelectedCustomer");
            }
            get
            {
                return _selectedCustomer;
            }
        }

        private DomesticForwarder _selectedForwarder = null;
        public DomesticForwarder SelectedForwarder
        {
            set
            {
                _selectedForwarder = value;
                OnPropertyChanged("SelectedForwarder");
            }
            get
            {
                return _selectedForwarder;
            }
        }



        private DomesticDestination _selectedPointOfOrigin = null;
        public DomesticDestination SelectedPointOfOrigin
        {
            set
            {
                _selectedPointOfOrigin = value;
                OnPropertyChanged("SelectedPointOfOrigin");
            }
            get
            {
                return _selectedPointOfOrigin;
            }
        }

        private DomesticDestination _selectedDestination = null;
        public DomesticDestination SelectedDestination
        {
            set
            {
                _selectedDestination = value;
                OnPropertyChanged("SelectedDestination");
            }
            get
            {
                return _selectedDestination;
            }
        }

        private Country _selectedCountry = null;
        public Country SelectedCountry
        {
            set
            {
                _selectedCountry = value;
                OnPropertyChanged("SelectedCountry");
            }
            get
            {
                return _selectedCountry;
            }
        }


        private CostCenter _selectedCostCenter = null;
        public CostCenter SelectedCostCenter
        {
            set
            {
                _selectedCostCenter = value;
                OnPropertyChanged("SelectedCostCenter");
            }
            get
            {
                return _selectedCostCenter;
            }
        }

        private string _selectedSoNumber = null;
        public string SelectedSoNumber
        {
            set
            {
                _selectedSoNumber = value;
                OnPropertyChanged("SelectedSoNumber");
            }
            get
            {
                return _selectedSoNumber;
            }
        }


        private List<DomesticExport> _reportData = null;
        public List<DomesticExport> ReportData
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


        private double? _yearFrom = DateTime.Now.Year;
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
        private double? _yearTo = DateTime.Now.Year;
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

        private double? _monthFrom = DateTime.Now.Month;
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

        private double? _monthTo = DateTime.Now.Month;
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

        private double? _yearOfMonth = DateTime.Now.Year;
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

        private double? _yearOfWeek = DateTime.Now.Year;
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


}
