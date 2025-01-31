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
using System.Windows.Media;

namespace COS.Application.Reporting.Models
{
    public partial class ProductionEmployeesModel : ValidationViewModelBase
    {
        public ProductionEmployeesModel()
            : base()
        {

        }

        public decimal GetUserProp(string key, List<UserProperty> userProps)
        {
            decimal result = 0;
            if (userProps != null)
            {
                var prop = userProps.FirstOrDefault(a => a.Key == key);

                if (prop != null)
                    decimal.TryParse(prop.Value, out result);
            }
            return result;
        }

        public void SetUserProp(string key, decimal value, List<UserProperty> userProps)
        {
            if (userProps != null)
            {
                var prop = userProps.FirstOrDefault(a => a.Key == key);

                if (prop != null)
                    prop.Value = value.ToString();
            }
        }


        public string GetUserPropColor(string key, List<UserProperty> userProps)
        {
            string result = "";
            if (userProps != null)
            {
                var prop = userProps.FirstOrDefault(a => a.Key == key);
                if (prop != null)
                    result = prop.Value.ToString();
            }
            return result;
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

        private DateTime? _selectedDate = COSContext.Current.DateTimeServer.Date;
        public DateTime? SelectedDate
        {
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
            get
            {
                return _selectedDate;
            }
        }

        public List<WorkGroup> LocalWorkGroups
        {
            get
            {
                return COSContext.Current.WorkGroups.OrderBy(a => a.Value).ToList();
            }
        }

        public List<BonusGroup> LocalBonusGroups
        {
            get
            {
                return COSContext.Current.BonusGroups.ToList();
            }
        }

        public List<Shift> LocalShifts
        {
            get
            {
                return COSContext.Current.Shifts.ToList();
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

        private BonusGroup _selectedBonusGroup = null;
        public BonusGroup SelectedBonusGroup
        {
            set
            {
                _selectedBonusGroup = value;
                OnPropertyChanged("SelectedBonusGroup");
            }
            get
            {
                return _selectedBonusGroup;
            }
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

        private List<ProdEmployeeDataClass> _reportData = new List<ProdEmployeeDataClass>();
        public List<ProdEmployeeDataClass> ReportData
        {
            get
            {
                return _reportData;
            }
        }

        private double? _Selectedmonth = COSContext.Current.DateTimeServer.Month;
        public double? SelectedMonth
        {
            set
            {
                _Selectedmonth = value;
                OnPropertyChanged("SelectedMonth");
            }
            get
            {
                return _Selectedmonth;
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

        private double? _SelectedWeek = COSContext.Current.Week;
        public double? SelectedWeek
        {
            set
            {
                _SelectedWeek = value;
                OnPropertyChanged("SelectedWeek");
            }
            get
            {
                return _SelectedWeek;
            }
        }

        private double? _SelectedYear = COSContext.Current.DateTimeServer.Year;
        public double? SelectedYear
        {
            set
            {
                _SelectedYear = value;
                OnPropertyChanged("SelectedYear");
            }
            get
            {
                return _SelectedYear;
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
        private bool _isMonthSelected = true;
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

        public void GenerateData()
        {
            var cc = COSContext.Current;

            if (IsYearSelected && SelectedYear.HasValue)
            {
                try
                {
                    SelectedDateFrom = Convert.ToDateTime("1.1." + SelectedYear.ToString());
                    SelectedDateTo = Convert.ToDateTime("31.12." + SelectedYear.ToString());
                }
                catch
                {
                    //no nic se neděje nekdo neco spatne zadal....
                }
            }
            else if (IsMonthSelected && SelectedMonth.HasValue && YearOfMonth.HasValue)
            {
                try
                {
                    SelectedDateFrom = Convert.ToDateTime("1." + SelectedMonth.ToString() + "." + YearOfMonth.ToString());
                    SelectedDateTo = SelectedDateFrom.Value.AddMonths(1).AddDays(-1);
                }
                catch
                {
                    //no nic se neděje nekdo neco spatne zadal....
                }
            }
            else if (IsWeekSelected && SelectedWeek.HasValue && YearOfWeek.HasValue)
            {
                try
                {


                    SelectedDateFrom = Convert.ToDateTime("1." + SelectedWeek.ToString() + "." + YearOfWeek.ToString());
                    SelectedDateTo = Convert.ToDateTime("31." + SelectedWeek.ToString() + "." + YearOfWeek.ToString());
                }
                catch
                {
                    //no nic se neděje nekdo neco spatne zadal....
                }
            }


            if (SelectedDateFrom.HasValue && SelectedDateTo.HasValue && SelectedBonusGroup != null)
            {
                ReportData.Clear();


                var division = cc.Divisions.FirstOrDefault(a => a.Value == "VA");

                if (SelectedBonusGroup.Value == BonusGroup.ADSAll)
                {
                    var empls = cc.Employees.Where(a => a.BonusGroup_ID == SelectedBonusGroup.ID && (a.LeaveDate == null || (a.LeaveDate.Value.Month >= SelectedDateFrom.Value.Month && a.LeaveDate.Value.Month <= SelectedDateTo.Value.Month) && (a.LeaveDate.Value.Year >= SelectedDateFrom.Value.Year && a.LeaveDate.Value.Year <= SelectedDateTo.Value.Year)));

                    if (SelectedShift != null)
                        empls = empls.Where(a => a.Shift_ID == SelectedShift.ID);

                    if (SelectedWorkGroup != null)
                        empls = empls.Where(a => a.WorkGroupHR_ID == SelectedWorkGroup.Value);

                    IQueryable<HourlyProduction> prods = null;

                    if (IsWeekSelected)
                    {
                        prods = from prds in cc.HourlyProductions
                                where prds.Date.Year == YearOfWeek && prds.Week == SelectedWeek && prds.ID_Division == division.ID
                                select prds;
                    }
                    else
                    {
                        prods = from prds in cc.HourlyProductions
                                where prds.Date >= SelectedDateFrom && prds.Date <= SelectedDateTo && prds.ID_Division == division.ID
                                select prds;
                    }

                    if (SelectedShift != null)
                        prods = prods.Where(a => a.ID_Shift == SelectedShift.ID);

                    if (SelectedWorkGroup != null)
                        prods = prods.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID);

                    var emplList = empls.OrderBy(a => a.Surname).ToList();
                    var prodList = prods.ToList();

                    foreach (var empl in emplList)
                    {
                        ProdEmployeeDataClass data = new ProdEmployeeDataClass();

                        data.FullName = empl.FullName;
                        data.PersonalNumber = empl.HR_ID;
                        data.ShiftString = empl.Shift != null ? empl.Shift.Description : string.Empty;
                        data.IsLeader = empl.IsLeader;
                        data.WorkGroupString = empl.WorkGroup != null ? empl.WorkGroup.Value : string.Empty;

                        data.KPIData.KpiProducedPcs = prodList.Sum(a => a.ProducedPieces);
                        data.KPIData.ScrapPcs = prodList.Sum(a => a.ScrapPieces);
                        data.KPIData.HlpProductivity = prodList.Sum(a => a.hlpHrProductivity);
                        data.KPIData.OperationalTime = prodList.Sum(a => a.ActOperationalTime_min);

                        ReportData.Add(data);
                    }

                }
                else if (SelectedBonusGroup.Value == BonusGroup.ADSPerShift)
                {

                    var empls = cc.Employees.Where(a => a.BonusGroup_ID == SelectedBonusGroup.ID && (a.LeaveDate == null || (a.LeaveDate.Value.Month >= SelectedDateFrom.Value.Month && a.LeaveDate.Value.Month <= SelectedDateTo.Value.Month) && (a.LeaveDate.Value.Year >= SelectedDateFrom.Value.Year && a.LeaveDate.Value.Year <= SelectedDateTo.Value.Year)));

                    if (SelectedShift != null)
                        empls = empls.Where(a => a.Shift_ID == SelectedShift.ID);

                    if (SelectedWorkGroup != null)
                        empls = empls.Where(a => a.WorkGroupHR_ID == SelectedWorkGroup.Value);

                    IQueryable<HourlyProduction> prods = null;



                    var emplList = empls.OrderBy(a => a.Surname).ToList();
                    //var prodList = prods.ToList();

                    foreach (var empl in emplList)
                    {
                        if (IsWeekSelected)
                        {
                            prods = from prds in cc.HourlyProductions
                                    where prds.Date.Year == YearOfWeek && prds.Week == SelectedWeek && prds.ID_Division == division.ID
                                    select prds;
                        }
                        else
                        {
                            prods = from prds in cc.HourlyProductions
                                    where prds.Date >= SelectedDateFrom && prds.Date <= SelectedDateTo && prds.ID_Division == division.ID
                                    select prds;
                        }

                        if (SelectedShift != null)
                            prods = prods.Where(a => a.ID_Shift == SelectedShift.ID);
                        else
                            prods = prods.Where(a => a.ID_Shift == empl.Shift_ID);

                        if (SelectedWorkGroup != null)
                            prods = prods.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID);

                        var prodList = prods.ToList();

                        ProdEmployeeDataClass data = new ProdEmployeeDataClass();

                        data.FullName = empl.FullName;
                        data.PersonalNumber = empl.HR_ID;
                        data.ShiftString = empl.Shift != null ? empl.Shift.Description : string.Empty;
                        data.IsLeader = empl.IsLeader;
                        data.WorkGroupString = empl.WorkGroup != null ? empl.WorkGroup.Value : string.Empty;

                        data.KPIData.KpiProducedPcs = prodList.Sum(a => a.ProducedPieces);
                        data.KPIData.ScrapPcs = prodList.Sum(a => a.ScrapPieces);
                        data.KPIData.HlpProductivity = prodList.Sum(a => a.hlpHrProductivity);
                        data.KPIData.OperationalTime = prodList.Sum(a => a.ActOperationalTime_min);

                        ReportData.Add(data);
                    }

                }
                else if (SelectedBonusGroup.Value == BonusGroup.PerEmployee)
                {

                    var empls = cc.Employees.Where(a => a.BonusGroup_ID == SelectedBonusGroup.ID && (a.LeaveDate == null || (a.LeaveDate.Value.Month >= SelectedDateTo.Value.Month && a.LeaveDate.Value.Year >= SelectedDateTo.Value.Year)));

                    if (SelectedShift != null)
                        empls = empls.Where(a => a.Shift_ID == SelectedShift.ID);

                    if (SelectedWorkGroup != null)
                        empls = empls.Where(a => a.WorkGroupHR_ID == SelectedWorkGroup.Value);

                    IQueryable<HourlyProduction> prods = null;
                    //if (IsWeekSelected)
                    //{
                    //    prods = (from prds in cc.HourlyProductions
                    //             join emps in cc.ProductionHRResources on prds.ID_HP equals emps.ID_HP into tmpEmpls
                    //             from emps in tmpEmpls.DefaultIfEmpty()
                    //             where prds.Date.Year == YearOfWeek && prds.Week == SelectedWeek && prds.ID_Division == division.ID
                    //             select prds);
                    //}
                    //else
                    //{
                    //    prods = (from prds in cc.HourlyProductions
                    //             join emps in cc.ProductionHRResources on prds.ID_HP equals emps.ID_HP into tmpEmpls
                    //             from emps in tmpEmpls.DefaultIfEmpty()
                    //             where prds.Date >= SelectedDateFrom && prds.Date <= SelectedDateTo
                    //             select prds);
                    //}

                    //if (SelectedShift != null)
                    //    prods = prods.Where(a => a.ID_Shift == SelectedShift.ID);

                    //if (SelectedWorkGroup != null)
                    //    prods = prods.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID);

                    var emplList = empls.OrderBy(a => a.Surname).ToList();
                    //var prodList = prods.ToList();


                    foreach (var empl in emplList)
                    {
                        ProdEmployeeDataClass data = new ProdEmployeeDataClass();

                        data.FullName = empl.FullName;
                        data.PersonalNumber = empl.HR_ID;
                        data.ShiftString = empl.Shift != null ? empl.Shift.Description : string.Empty;
                        data.IsLeader = empl.IsLeader;
                        data.WorkGroupString = empl.WorkGroup != null ? empl.WorkGroup.Value : string.Empty;

                        if (IsWeekSelected)
                        {
                            prods = (from prds in cc.HourlyProductions
                                     join emps in cc.ProductionHRResources on prds.ID_HP equals emps.ID_HP into tmpEmpls
                                     from emps in tmpEmpls.DefaultIfEmpty()
                                     where prds.Date.Year == YearOfWeek && prds.Week == SelectedWeek && prds.ID_Division == division.ID && emps.ID_HR == empl.HR_ID
                                     select prds);
                        }
                        else
                        {
                            prods = (from prds in cc.HourlyProductions
                                     join emps in cc.ProductionHRResources on prds.ID_HP equals emps.ID_HP into tmpEmpls
                                     from emps in tmpEmpls.DefaultIfEmpty()
                                     where prds.Date >= SelectedDateFrom && prds.Date <= SelectedDateTo && emps.ID_HR == empl.HR_ID
                                     select prds);
                        }

                        if (SelectedShift != null)
                            prods = prods.Where(a => a.ID_Shift == SelectedShift.ID);
                        //else
                        //    prods = prods.Where(a => a.ID_Shift == empl.Shift_ID);

                        if (SelectedWorkGroup != null)
                            prods = prods.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID);

                        var prodList = prods.ToList();

                        data.KPIData.KpiProducedPcs = prodList.Sum(a => a.ProducedPieces);
                        data.KPIData.ScrapPcs = prodList.Sum(a => a.ScrapPieces);
                        data.KPIData.HlpProductivity = prodList.Sum(a => a.hlpHrProductivity);
                        data.KPIData.OperationalTime = prodList.Sum(a => a.ActOperationalTime_min);

                        ReportData.Add(data);
                    }

                }
                else if (SelectedBonusGroup.Value == BonusGroup.PerHrWgShift)
                {

                    var empls = cc.Employees.Where(a => a.BonusGroup_ID == SelectedBonusGroup.ID && (a.LeaveDate == null || (a.LeaveDate.Value.Month >= SelectedDateFrom.Value.Month && a.LeaveDate.Value.Month <= SelectedDateTo.Value.Month) && (a.LeaveDate.Value.Year >= SelectedDateFrom.Value.Year && a.LeaveDate.Value.Year <= SelectedDateTo.Value.Year)));

                    if (SelectedShift != null)
                        empls = empls.Where(a => a.Shift_ID == SelectedShift.ID);

                    if (SelectedWorkGroup != null)
                        empls = empls.Where(a => a.WorkGroupHR_ID == SelectedWorkGroup.Value);


                    IQueryable<HourlyProduction> prods = null;


                    var emplList = empls.OrderBy(a => a.Surname).ToList();
                    //var prodList = prods.ToList();

                    foreach (var empl in emplList)
                    {
                        if (IsWeekSelected)
                        {
                            prods = from prds in cc.HourlyProductions
                                    where prds.Date.Year == YearOfWeek && prds.Week == SelectedWeek && prds.ID_Division == division.ID
                                    select prds;
                        }
                        else
                        {
                            prods = from prds in cc.HourlyProductions
                                    where prds.Date >= SelectedDateFrom && prds.Date <= SelectedDateTo && prds.ID_Division == division.ID
                                    select prds;
                        }

                        if (SelectedShift != null)
                            prods = prods.Where(a => a.ID_Shift == SelectedShift.ID);
                        else
                            prods = prods.Where(a => a.ID_Shift == empl.Shift_ID);

                        bool hasWG = empl.WorkGroup != null;

                        if (SelectedWorkGroup != null)
                            prods = prods.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID);
                        else
                        {
                            if (hasWG)
                                prods = prods.Where(a => a.ID_WorkGroup == empl.WorkGroup.ID);
                        }

                        List<HourlyProduction> prodList = new List<HourlyProduction>();
                        if (hasWG)
                            prodList = prods.ToList();

                        ProdEmployeeDataClass data = new ProdEmployeeDataClass();

                        data.FullName = empl.FullName;
                        data.PersonalNumber = empl.HR_ID;
                        data.ShiftString = empl.Shift != null ? empl.Shift.Description : string.Empty;
                        data.IsLeader = empl.IsLeader;
                        data.WorkGroupString = empl.WorkGroup != null ? empl.WorkGroup.Value : string.Empty;

                        data.KPIData.KpiProducedPcs = prodList.Sum(a => a.ProducedPieces);
                        data.KPIData.ScrapPcs = prodList.Sum(a => a.ScrapPieces);
                        data.KPIData.HlpProductivity = prodList.Sum(a => a.hlpHrProductivity);
                        data.KPIData.OperationalTime = prodList.Sum(a => a.ActOperationalTime_min);

                        ReportData.Add(data);
                    }


                }

            }
        }


    }

    public class ProdEmployeeDataClass
    {
        public ProdEmployeeDataClass()
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
