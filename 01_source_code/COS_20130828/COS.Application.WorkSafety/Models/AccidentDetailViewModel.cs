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
using System.Net.Mail;

namespace COS.Application.WorkSafety.Models
{
    public partial class AccidentDetailViewModel : ValidationViewModelBase
    {
        public AccidentDetailViewModel(COSContext dataContext, TypeOfTypeAccident typeOfTypeAccident)
            : base()
        {
            TypeOfTypeAccident = typeOfTypeAccident;
            this.dataContext = dataContext;
            ReloadLocalCodebooks();

            this.PropertyChanged += new PropertyChangedEventHandler(AccidentDetailViewModel_PropertyChanged);
        }

        void AccidentDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //LocalHealtInsuranceCompanies
            //SelectedItem.WitnessName
                
        }


        private COSContext dataContext = null;
        public TypeOfTypeAccident TypeOfTypeAccident = TypeOfTypeAccident.Accident;

        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
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
                if (_SelectedItem != null)
                    _SelectedItem.PropertyChanged -= _SelectedItem_PropertyChanged;
                _SelectedItem = value;
                if (_SelectedItem != null)
                {
                    _SelectedItem.PropertyChanged += new PropertyChangedEventHandler(_SelectedItem_PropertyChanged);
                    if (_SelectedItem.AreaOfAccident != null)
                        this.SelectedDepartment = _SelectedItem.AreaOfAccident.Department;
                }
                OnPropertyChanged("SelectedItem");
            }
        }

        void _SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SelectedItem != null)
            {
                if (e.PropertyName == "IncapacityFrom")
                {
                    SelectedItem.CalculateNumberOfCalendarDaysOfIncapacity();
                    SelectedItem.CalculateNumberOfWorkDaysOfIncapacity();
                }
                else if (e.PropertyName == "IncapacityTo")
                {
                    SelectedItem.CalculateNumberOfCalendarDaysOfIncapacity();
                    SelectedItem.CalculateNumberOfWorkDaysOfIncapacity();
                }
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
            if (TypeOfTypeAccident == Models.TypeOfTypeAccident.Accident)
            {
                foreach (var itm in this.dataContext.TypeOfAccidents.Where(a => a.Code == "A"))
                    LocalTypesOfAccident.Add(itm);
            }
            else if (TypeOfTypeAccident == Models.TypeOfTypeAccident.NearMiss)
            {
                foreach (var itm in this.dataContext.TypeOfAccidents.Where(a => a.Code == "N"))
                    LocalTypesOfAccident.Add(itm);
            }

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

        public void CreateNewAccident()
        {
            AccidentLog acc = this.dataContext.AccidentLogs.CreateObject();

            acc.DateOfAccident = DateTime.Today;
            acc.IncapacityFrom = DateTime.Today;
            acc.IncapacityTo = DateTime.Today;
            acc.CreatedByID = COSContext.Current.CurrentUser.ID;
            acc.CreatedByDate = COSContext.Current.DateTimeServer;

            this.SelectedItem = acc;
        }

        public void CreateNewNearMiss()
        {
            AccidentLog acc = this.dataContext.AccidentLogs.CreateObject();

            acc.DateOfAccident = DateTime.Today;
            acc.IncapacityFrom = DateTime.Today;
            acc.IncapacityTo = DateTime.Today;
            acc.CreatedByID = COSContext.Current.CurrentUser.ID;
            acc.CreatedByDate = COSContext.Current.DateTimeServer;

            this.dataContext.AccidentLogs.AddObject(acc);

            this.SelectedItem = acc;
        }

        public void Save()
        {
            try
            {
                string err = Validate();

                if (err.IsNullOrEmptyString())
                {
                    this.dataContext.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    OnPropertyChanged("Save");
                }
                else
                {
                    this.RaiseErrors = err;
                }
            }
            catch (Exception exc)
            {
                RadWindow.Alert(exc.Message);
                Logging.LogException(exc, LogType.ToFileAndEmail);
            }
        }

        public void Cancel()
        {
            OnPropertyChanged("Cancel");
        }

        public string Validate()
        {
            string err = null;

            if (this.TypeOfTypeAccident == Models.TypeOfTypeAccident.Accident)
            {
                if (SelectedItem.AreaOfAccident == null)
                {
                    err += "Are of acc must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.Employee == null)
                {
                    err += "Employee must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.CountInvolvedEmployes == 0)
                {
                    err += "CountInvolvedEmployes must be filled";
                    err += Environment.NewLine;
                }


                if (SelectedItem.AccidentActivityOccured.IsNullOrEmptyString())
                {
                    err += "AccidentActivityOccured must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.AccidentDescription.IsNullOrEmptyString())
                {
                    err += "AccidentDescription must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.TypeOfAccident == null)
                {
                    err += "TypeOfAccident must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.SourceOfAccident == null)
                {
                    err += "SourceOfAccident must be filled";
                    err += Environment.NewLine;
                }
                if (SelectedItem.CauseOfAccident == null)
                {
                    err += "CauseOfAccident must be filled";
                    err += Environment.NewLine;
                }

            }
            else if (this.TypeOfTypeAccident == Models.TypeOfTypeAccident.NearMiss)
            {
                if (SelectedItem.AreaOfAccident == null)
                {
                    err += "Are of acc must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.Employee == null)
                {
                    err += "Employee must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.CountInvolvedEmployes == 0)
                {
                    err += "CountInvolvedEmployes must be filled";
                    err += Environment.NewLine;
                }


                if (SelectedItem.AccidentActivityOccured.IsNullOrEmptyString())
                {
                    err += "AccidentActivityOccured must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.AccidentDescription.IsNullOrEmptyString())
                {
                    err += "AccidentDescription must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.TypeOfAccident == null)
                {
                    err += "TypeOfAccident must be filled";
                    err += Environment.NewLine;
                }

                if (SelectedItem.SourceOfAccident == null)
                {
                    err += "SourceOfAccident must be filled";
                    err += Environment.NewLine;
                }
                if (SelectedItem.CauseOfAccident == null)
                {
                    err += "CauseOfAccident must be filled";
                    err += Environment.NewLine;
                }
            }

            return err;
        }


    }

    //kod typu typu nehody 1 = nehoda, 2 = skoronehoda
    public enum TypeOfTypeAccident
    {
        /// <summary>
        /// Nehoda
        /// </summary>
        Accident = 1,
        /// <summary>
        /// Skoronehoda
        /// </summary>
        NearMiss = 2
    }
}
