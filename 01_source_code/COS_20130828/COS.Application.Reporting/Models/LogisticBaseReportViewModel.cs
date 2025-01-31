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
    public partial class LogisticBaseReportViewModel : ValidationViewModelBase
    {
        public LogisticBaseReportViewModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(LogisticBaseReportViewModel_PropertyChanged);

            LocalCountries = COSContext.Current.Countries.ToList().OrderBy(a => a.Code).ToList();
            LocalForwarders = COSContext.Current.Forwarders.ToList().OrderBy(a => a.Name).ToList();
            LocalOrderedBys = COSContext.Current.ForeignOrderedBies.ToList().OrderBy(a => a.CustomerName ).ToList();
            LocalTransportPayments = COSContext.Current.ForeignTransportPayments.ToList().OrderBy(a => a.Description ).ToList();

        }

        void LogisticBaseReportViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedCountry")
            {
                if (SelectedCountry != null)
                {
                    LocalZoneLogistics = COSContext.Current.ZoneLogistics.Where(a => a.ID_Country == SelectedCountry.ID).ToList().OrderBy(a => a.DestinationName ).ToList();
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

        public ICommand ClearZoneCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearZone());
            }
        }

        public ICommand ClearPaymentCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearPayment());
            }
        }

        public ICommand ClearCountryCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearCountry());
            }
        }

        public ICommand ClearOrderedByCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearOrderedBy());
            }
        }


        private void ClearForwarder()
        {
            SelectedForwarder = null;
        }

        private void ClearOrderedBy()
        {
            SelectedOrderedBy = null;
        }

        private void ClearCountry()
        {
            SelectedCountry = null;
        }

        private void ClearPayment()
        {
            SelectedTransportPayment = null;
        }

        private void ClearZone()
        {
            SelectedZone = null;
        }


        public void ShowReportInternal()
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.HourlyProductions);

            var logistics = COSContext.Current.ForeignExports.AsQueryable();

            if (SelectedCountry != null && SelectedZone == null)
                logistics = logistics.Where(a => a.Destination.ID_Country == SelectedCountry.ID);

            if (SelectedZone != null)
                logistics = logistics.Where(a => a.ID_Destination == SelectedZone.ID);

            if (SelectedForwarder != null)
                logistics = logistics.Where(a => a.ID_Forwarder == SelectedForwarder.ID);

            if (SelectedOrderedBy != null)
                logistics = logistics.Where(a => a.Connections.Where(s => s.ExportDetails.Where(c => c.ID_OrderedBy == SelectedOrderedBy.ID).Count() > 0).Count() > 0);

            if (SelectedTransportPayment != null)
                logistics = logistics.Where(a => a.Connections.Where(s => s.ExportDetails.Where(c=>c.ID_TransportPayment == SelectedTransportPayment.ID).Count() > 0).Count() > 0);



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

                    SelectedDateTo = (Helpers.FirstDateOfWeek((int)YearOfWeek, (int)WeekTo, CalendarWeekRule.FirstFourDayWeek).AddDays(6)).AddDays(-7) ;
                }
                else
                    SelectedDateTo = null;
            }

            //if (IsWeekSelected)
            //{
            //    if (WeekFrom.HasValue && YearOfWeek.HasValue)
            //        logistics = logistics.Where(a => a. >= WeekFrom && a.Date.Year == YearOfWeek);

            //    if (WeekTo.HasValue && YearOfWeek.HasValue)
            //        logistics = logistics.Where(a => a.Week <= WeekTo && a.Date.Year == YearOfWeek);
            //}
            //else
            //{

            if (SelectedDateFrom.HasValue)
                logistics = logistics.Where(a => a.PlannedDate >= SelectedDateFrom);

            if (SelectedDateTo.HasValue)
                logistics = logistics.Where(a => a.PlannedDate <= SelectedDateTo);
            //}

            logistics = logistics.OrderBy(a => a.PlannedDate);


            var dd = logistics.ToList();
            
            ReportData = dd;

            ForwarderPrice = ReportData.Sum(a => a.ForwarderPrice);
            TollPrice = ReportData.Sum(a => a.TollPrice);
            TollPriceGerlach = ReportData.Sum(a => a.TollPriceGerlach);
            BafPrice = ReportData.Sum(a => a.BafPrice);
            TotalPrice = ReportData.Sum(a => a.TotalPrice);
            VolumeCBM = ReportData.Sum(a => a.VolumeCbm);
            TransportCount = ReportData.Count;

            Models = new List<LogisticBaseReportViewModel>();

            Models.Add(this);
        }

        public List<LogisticBaseReportViewModel> Models = null;


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

        private decimal _tollPrice = 0;
        public decimal TollPrice
        {
            set
            {
                _tollPrice = value;
                OnPropertyChanged("TollPrice");
            }
            get
            {
                return _tollPrice;
            }
        }


        private decimal _tollPriceGerlach = 0;
        public decimal TollPriceGerlach
        {
            set
            {
                _tollPriceGerlach = value;
                OnPropertyChanged("TollPriceGerlach");
            }
            get
            {
                return _tollPriceGerlach;
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

      
               

      


        private List<ForeignTransportPayment> _localTransportPayments = null;
        public List<ForeignTransportPayment> LocalTransportPayments
        {
            set
            {
                _localTransportPayments = value;
                OnPropertyChanged("LocalTransportPayments");
            }
            get
            {
                return _localTransportPayments;
            }
        }

        private List<ForeignOrderedBy> _localOrderedBys = null;
        public List<ForeignOrderedBy> LocalOrderedBys
        {
            set
            {
                _localOrderedBys = value;
                OnPropertyChanged("LocalOrderedBys");
            }
            get
            {
                return _localOrderedBys;
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


        private List<ZoneLogistics> _localZoneLogistics = null;
        public List<ZoneLogistics> LocalZoneLogistics
        {
            set
            {
                _localZoneLogistics = value;
                OnPropertyChanged("LocalZoneLogistics");
            }
            get
            {
                return _localZoneLogistics;
            }
        }

        private List<Forwarder> _localForwarders = null;
        public List<Forwarder> LocalForwarders
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



        private ForeignTransportPayment _selectedTransportPayment = null;
        public ForeignTransportPayment SelectedTransportPayment
        {
            set
            {
                _selectedTransportPayment = value;
                OnPropertyChanged("SelectedTransportPayment");
            }
            get
            {
                return _selectedTransportPayment;
            }
        }

        private ForeignOrderedBy _selectedOrderedBy = null;
        public ForeignOrderedBy SelectedOrderedBy
        {
            set
            {
                _selectedOrderedBy = value;
                OnPropertyChanged("SelectedOrderedBy");
            }
            get
            {
                return _selectedOrderedBy;
            }
        }

        private Forwarder _selectedForwarder = null;
        public Forwarder SelectedForwarder
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



        private ZoneLogistics _selectedZone = null;
        public ZoneLogistics SelectedZone
        {
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
            get
            {
                return _selectedZone;
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


        private List<ForeignExport> _reportData = null;
        public List<ForeignExport> ReportData
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
