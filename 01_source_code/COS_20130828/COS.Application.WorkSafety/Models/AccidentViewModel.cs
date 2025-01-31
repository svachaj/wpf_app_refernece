using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using COS.Resources;
using System.IO;
using System.Windows;
using System.Globalization;
using System.Transactions;
using System.Data.Objects;
using System.ComponentModel;

namespace COS.Application.WorkSafety.Models
{
    public partial class AccidentViewModel : ValidationViewModelBase
    {
        public AccidentViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;

            ReloadLocalCodebooks();

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            ReloadDataAsync();
        }

        public void ReloadDataAsync()
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LocalAccidents.Clear();
            foreach (var itm in acclogs)
            {
                LocalAccidents.Add(itm);
            }

            IsBusy = false;
            OnPropertyChanged("DataReloaded");
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;
            ReloadData();
        }

        BackgroundWorker worker = null;

        private COSContext dataContext = null;

        public ICommand RefreshToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.ReloadData());
            }
        }

        public ICommand AddAccidentToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddAccident());
            }
        }

        private void AddAccident()
        {
            OnPropertyChanged("AddAccident");
        }

        public ICommand AddNearMissToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNearMiss());
            }
        }

        private void AddNearMiss()
        {
            OnPropertyChanged("AddNearMiss");
        }

        public ICommand DeleteAccidentToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.DeleteAccident());
            }
        }

        private void DeleteAccident()
        {
            if (SelectedItem != null)
            {
                try
                {
                    this.dataContext.DeleteObject(SelectedItem);
                    LocalAccidents.Remove(SelectedItem);
                    this.dataContext.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                }
                catch (Exception exc)
                {
                    RadWindow.Alert("Nepodařilo se smazat záznam.");
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                }
            }
            OnPropertyChanged("DeleteAccident");
        }


        private AccidentLog _SelectedItem = null;
        public AccidentLog SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
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

        IQueryable<AccidentLog> acclogs;
        public void ReloadData()
        {
            var accs = this.dataContext.AccidentLogs.AsQueryable();

            if (this.SelectedDepartment != null)
                accs = accs.Where(a => a.AreaOfAccident.ID_department == SelectedDepartment.ID);

            if (this.SelectedAreaOfAccident != null)
                accs = accs.Where(a => a.ID_AreaOfAccident == SelectedAreaOfAccident.ID);

            if (this.SelectedWorkGroup != null)
                accs = accs.Where(a => a.AreaOfAccident.ID_workGroup == SelectedWorkGroup.ID);

            if (this.SelectedEmployee != null)
                accs = accs.Where(a => a.ID_Employee == SelectedEmployee.ID);

            if (this.SelectedEmployer != null)
                accs = accs.Where(a => a.Employee.EmployerHR_ID == SelectedEmployer.ID_RON);

            if (this.SelectedTypeOfAccident != null)
                accs = accs.Where(a => a.ID_TypeOfAccident == SelectedTypeOfAccident.ID);

            if (this.SelectedTypeOfInjury != null)
                accs = accs.Where(a => a.ID_TypeOfInjury == SelectedTypeOfInjury.ID);

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

            accs = accs.Where(a => a.DateOfAccident >= SelectedDateFrom && a.DateOfAccident <= SelectedDateTo);

            this.dataContext.Refresh(RefreshMode.StoreWins, accs);

            acclogs = accs;
        }

        public void ReloadLocalCodebooks()
        {
            LocalAreasOfAccident = new ObservableCollection<AreaOfAccident>();
            foreach (var itm in this.dataContext.AreaOfAccidents)
                LocalAreasOfAccident.Add(itm);

            LocalDepartments = new ObservableCollection<HrDepartment>();
            foreach (var itm in this.dataContext.HrDepartments)
                LocalDepartments.Add(itm);

            LocalEmployees = new ObservableCollection<Employee>();
            foreach (var itm in this.dataContext.Employees)
                LocalEmployees.Add(itm);

            LocalEmployers = new ObservableCollection<Employer>();
            foreach (var itm in this.dataContext.Employers)
                LocalEmployers.Add(itm);

            LocalWorkGroups = new ObservableCollection<WorkGroup>();
            foreach (var itm in this.dataContext.WorkGroups)
                LocalWorkGroups.Add(itm);

            LocalTypesOfAccident = new ObservableCollection<TypeOfAccident>();
            foreach (var itm in this.dataContext.TypeOfAccidents)
                LocalTypesOfAccident.Add(itm);

            LocalTypesOfInjury = new ObservableCollection<TypeOfInjury>();
            foreach (var itm in this.dataContext.TypeOfInjuries)
                LocalTypesOfInjury.Add(itm);

            LocalInjPartsOfBody = new ObservableCollection<InjPartOfBody>();
            foreach (var itm in this.dataContext.InjPartOfBodies)
                LocalInjPartsOfBody.Add(itm);

            LocalMeasurePreventTypes = new ObservableCollection<MeasurePreventType>();
            foreach (var itm in this.dataContext.MeasurePreventTypes)
                LocalMeasurePreventTypes.Add(itm);


            LocalHealtInsuranceCompanies = new ObservableCollection<HealtInsuranceComp>();
            foreach (var itm in this.dataContext.HealtInsuranceComps)
                LocalHealtInsuranceCompanies.Add(itm);


            LocalCausesOfAccident = new ObservableCollection<CauseOfAccident>();
            foreach (var itm in this.dataContext.CauseOfAccidents)
                LocalCausesOfAccident.Add(itm);

            LocalSourcesOfAccident = new ObservableCollection<SourceOfAccident>();
            foreach (var itm in this.dataContext.SourceOfAccidents)
                LocalSourcesOfAccident.Add(itm);

        }


        public ObservableCollection<AccidentLog> _localAccidents = new ObservableCollection<AccidentLog>();

        public ObservableCollection<AccidentLog> LocalAccidents
        {
            get
            {
                return _localAccidents;
            }
        }

        #region filters + codebooks

        private HrDepartment _SelectedDepartment = null;
        public HrDepartment SelectedDepartment
        {
            get
            {
                return _SelectedDepartment;
            }
            set
            {
                _SelectedDepartment = value;

                if (LocalAreasOfAccident != null && _SelectedDepartment != null)
                {
                    LocalAreasOfAccident.Clear();
                    foreach (var itm in this.dataContext.AreaOfAccidents.Where(a => a.ID_department == _SelectedDepartment.ID))
                        LocalAreasOfAccident.Add(itm);
                }

                OnPropertyChanged("SelectedDepartment");
            }
        }

        public ICommand ClearDepartmentCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearDepartment());
            }
        }

        private void ClearDepartment()
        {
            SelectedDepartment = null;
        }

        public ObservableCollection<HrDepartment> LocalDepartments { set; get; }

        private AreaOfAccident _SelectedAreaOfAccident = null;
        public AreaOfAccident SelectedAreaOfAccident
        {
            get
            {
                return _SelectedAreaOfAccident;
            }
            set
            {
                _SelectedAreaOfAccident = value;
                OnPropertyChanged("SelectedAreaOfAccident");
            }
        }

        public ICommand ClearAreOfAccidentCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearAreOfAccident());
            }
        }

        private void ClearAreOfAccident()
        {
            SelectedAreaOfAccident = null;
        }

        public ObservableCollection<AreaOfAccident> LocalAreasOfAccident { set; get; }


        private WorkGroup _SelectedWorkGroup = null;
        public WorkGroup SelectedWorkGroup
        {
            get
            {
                return _SelectedWorkGroup;
            }
            set
            {
                _SelectedWorkGroup = value;
                OnPropertyChanged("SelectedWorkGroup");
            }
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

        public ObservableCollection<WorkGroup> LocalWorkGroups { set; get; }


        private Employee _SelectedEmployee = null;
        public Employee SelectedEmployee
        {
            get
            {
                return _SelectedEmployee;
            }
            set
            {
                _SelectedEmployee = value;
                if (_SelectedEmployee != null)
                {
                    SelectedEmployer = _SelectedEmployee.EmployerHR;
                }
                OnPropertyChanged("SelectedEmployee");
            }
        }

        public ICommand ClearEmployeeCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearEmployee());
            }
        }

        private void ClearEmployee()
        {
            SelectedEmployee = null;
        }

        public ObservableCollection<Employee> LocalEmployees { set; get; }



        private Employer _SelectedEmployer = null;
        public Employer SelectedEmployer
        {
            get
            {
                return _SelectedEmployer;
            }
            set
            {
                _SelectedEmployer = value;
                OnPropertyChanged("SelectedEmployer");
            }
        }

        public ICommand ClearEmployerCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearEmployer());
            }
        }

        private void ClearEmployer()
        {
            SelectedEmployer = null;
        }

        public ObservableCollection<Employer> LocalEmployers { set; get; }


        private TypeOfAccident _SelectedTypeOfAccident = null;
        public TypeOfAccident SelectedTypeOfAccident
        {
            get
            {
                return _SelectedTypeOfAccident;
            }
            set
            {
                _SelectedTypeOfAccident = value;
                OnPropertyChanged("SelectedTypeOfAccident");
            }
        }

        public ICommand ClearTypeOfAccidentCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearTypeOfAccident());
            }
        }

        private void ClearTypeOfAccident()
        {
            SelectedTypeOfAccident = null;
        }

        public ObservableCollection<TypeOfAccident> LocalTypesOfAccident { set; get; }




        private TypeOfInjury _SelectedTypeOfInjury = null;
        public TypeOfInjury SelectedTypeOfInjury
        {
            get
            {
                return _SelectedTypeOfInjury;
            }
            set
            {
                _SelectedTypeOfInjury = value;
                OnPropertyChanged("SelectedTypeOfInjury");
            }
        }

        public ICommand ClearTypeOfInjuryCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearTypeOfInjury());
            }
        }

        private void ClearTypeOfInjury()
        {
            SelectedTypeOfInjury = null;
        }

        public ObservableCollection<TypeOfInjury> LocalTypesOfInjury { set; get; }

        public ObservableCollection<InjPartOfBody> LocalInjPartsOfBody { set; get; }

        public ObservableCollection<MeasurePreventType> LocalMeasurePreventTypes { set; get; }

        public ObservableCollection<HealtInsuranceComp> LocalHealtInsuranceCompanies { set; get; }

        public ObservableCollection<CauseOfAccident> LocalCausesOfAccident { set; get; }

        public ObservableCollection<SourceOfAccident> LocalSourcesOfAccident { set; get; }




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

        #endregion

    }


}
