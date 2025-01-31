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
    public partial class KPIWCWGReportViewModel : ValidationViewModelBase
    {
        public KPIWCWGReportViewModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(KPIReportViewModel_PropertyChanged);

            string decryptString = Crypto.DecryptString(System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString, Security.SecurityHelper.SecurityKey);

            CurrentContext = new COSContext(decryptString);
        }

        public void Dispose()
        {
            if (CurrentContext != null)
                CurrentContext.Dispose();
        }

        public COSContext CurrentContext = null;

        void KPIReportViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDivision")
            {
                if (SelectedDivision != null)
                {
                    LocalWorkGroups = CurrentContext.WorkGroups.Where(a => a.ID_Division == SelectedDivision.ID).OrderBy(a => a.Value).ToList();
                }

                UsedWorkCenters = null;
                UsedWorkGroups = null;

            }
            else if (e.PropertyName == "SelectedWorkGroup")
            {
                if (SelectedWorkGroup != null)
                    LocalWorkCenters = CurrentContext.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID).Select(a => a.WorkCenter).OrderBy(a => a.Value).ToList();
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

        public string DivisionParam
        {
            get
            {
                return SelectedDivision != null ? SelectedDivision.Value : "";
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
            ShowReport1();
            ShowReport2();
            ShowReport3();
        }


        public void ShowReport1()
        {
            if (SelectedItem1 != null)
            {
                CurrentContext.ContextOptions.LazyLoadingEnabled = false;

                var productions = CurrentContext.HourlyProductions.AsQueryable();

                if (ReportingMode == ReportMode.AllWCs)
                {
                    WorkCenter swc = SelectedItem1 as WorkCenter;
                    if (swc != null)
                    {
                        productions = productions.Where(a => a.ID_WorkCenter == swc.ID);
                    }
                }
                else if (ReportingMode == ReportMode.AllWGs)
                {
                    WorkGroup swg = SelectedItem1 as WorkGroup;
                    if (swg != null)
                    {
                        productions = productions.Where(a => a.ID_WorkGroup == swg.ID);
                    }
                }
                else if (ReportingMode == ReportMode.SomeWCs)
                {
                    WorkCenter swc = SelectedItem1 as WorkCenter;
                    if (swc != null)
                    {
                        productions = productions.Where(a => a.ID_WorkCenter == swc.ID);
                    }
                }

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

                CurrentContext.ContextOptions.LazyLoadingEnabled = true;

                //System.Threading.Thread.Sleep(500);

                ReportData1 = new List<KPIReportData>();
                KPIReportData data = null;

                switch (SelectedShowBy)
                {
                    case ShowBy.All:

                        foreach (var itm in prods)
                        {
                            KPIReportData existed = ReportData1.FirstOrDefault(a => a.Date == itm.Date);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Date = itm.Date.Date;

                                ReportData1.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Day:

                        foreach (var itm in prods)
                        {
                            int dayofyear = itm.Date.DayOfYear;
                            int year = itm.Date.Year;
                            KPIReportData existed = ReportData1.FirstOrDefault(a => a.Day == dayofyear && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Day = dayofyear;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData1.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Month:

                        foreach (var itm in prods)
                        {
                            int month = itm.Date.Month;
                            int year = itm.Date.Year;

                            KPIReportData existed = ReportData1.FirstOrDefault(a => a.Month == month && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Month = month;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData1.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Week:

                        foreach (var itm in prods)
                        {
                            int year = itm.Date.Year;

                            KPIReportData existed = ReportData1.FirstOrDefault(a => a.Week == itm.Week && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Week = itm.Week;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData1.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Year:

                        foreach (var itm in prods)
                        {

                            KPIReportData existed = ReportData1.FirstOrDefault(a => a.Week == itm.Date.Year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Year = itm.Date.Year;
                                data.Date = itm.Date.Date;

                                ReportData1.Add(data);
                            }
                        }
                        break;
                }
            }
        }

        public void ShowReport2()
        {
            if (SelectedItem2 != null)
            {
                CurrentContext.ContextOptions.LazyLoadingEnabled = false;

                var productions = CurrentContext.HourlyProductions.AsQueryable();

                if (ReportingMode == ReportMode.AllWCs)
                {
                    WorkCenter swc = SelectedItem2 as WorkCenter;
                    if (swc != null)
                    {
                        productions = productions.Where(a => a.ID_WorkCenter == swc.ID);
                    }
                }
                else if (ReportingMode == ReportMode.AllWGs)
                {
                    WorkGroup swg = SelectedItem2 as WorkGroup;
                    if (swg != null)
                    {
                        productions = productions.Where(a => a.ID_WorkGroup == swg.ID);
                    }
                }
                else if (ReportingMode == ReportMode.SomeWCs)
                {
                    WorkCenter swc = SelectedItem2 as WorkCenter;
                    if (swc != null)
                    {
                        productions = productions.Where(a => a.ID_WorkCenter == swc.ID);
                    }
                }

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

                CurrentContext.ContextOptions.LazyLoadingEnabled = true;

                //System.Threading.Thread.Sleep(500);

                ReportData2 = new List<KPIReportData>();
                KPIReportData data = null;



                switch (SelectedShowBy)
                {
                    case ShowBy.All:

                        foreach (var itm in prods)
                        {
                            KPIReportData existed = ReportData2.FirstOrDefault(a => a.Date == itm.Date);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Date = itm.Date.Date;

                                ReportData2.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Day:

                        foreach (var itm in prods)
                        {
                            int dayofyear = itm.Date.DayOfYear;
                            int year = itm.Date.Year;
                            KPIReportData existed = ReportData2.FirstOrDefault(a => a.Day == dayofyear && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Day = dayofyear;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData2.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Month:

                        foreach (var itm in prods)
                        {
                            int month = itm.Date.Month;
                            int year = itm.Date.Year;

                            KPIReportData existed = ReportData2.FirstOrDefault(a => a.Month == month && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Month = month;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData2.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Week:

                        foreach (var itm in prods)
                        {
                            int year = itm.Date.Year;

                            KPIReportData existed = ReportData2.FirstOrDefault(a => a.Week == itm.Week && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Week = itm.Week;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData2.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Year:

                        foreach (var itm in prods)
                        {

                            KPIReportData existed = ReportData2.FirstOrDefault(a => a.Week == itm.Date.Year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Year = itm.Date.Year;
                                data.Date = itm.Date.Date;

                                ReportData2.Add(data);
                            }
                        }
                        break;
                }
            }
        }

        public void ShowReport3()
        {
            if (SelectedItem3 != null)
            {
                CurrentContext.ContextOptions.LazyLoadingEnabled = false;

                var productions = CurrentContext.HourlyProductions.AsQueryable();

                if (ReportingMode == ReportMode.AllWCs)
                {
                    WorkCenter swc = SelectedItem3 as WorkCenter;
                    if (swc != null)
                    {
                        productions = productions.Where(a => a.ID_WorkCenter == swc.ID);
                    }
                }
                else if (ReportingMode == ReportMode.AllWGs)
                {
                    WorkGroup swg = SelectedItem3 as WorkGroup;
                    if (swg != null)
                    {
                        productions = productions.Where(a => a.ID_WorkGroup == swg.ID);
                    }
                }
                else if (ReportingMode == ReportMode.SomeWCs)
                {
                    WorkCenter swc = SelectedItem3 as WorkCenter;
                    if (swc != null)
                    {
                        productions = productions.Where(a => a.ID_WorkCenter == swc.ID);
                    }
                }

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

                CurrentContext.ContextOptions.LazyLoadingEnabled = true;

                //System.Threading.Thread.Sleep(500);

                ReportData3 = new List<KPIReportData>();
                KPIReportData data = null;



                switch (SelectedShowBy)
                {
                    case ShowBy.All:

                        foreach (var itm in prods)
                        {
                            KPIReportData existed = ReportData3.FirstOrDefault(a => a.Date == itm.Date);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Date = itm.Date.Date;

                                ReportData3.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Day:

                        foreach (var itm in prods)
                        {
                            int dayofyear = itm.Date.DayOfYear;
                            int year = itm.Date.Year;
                            KPIReportData existed = ReportData3.FirstOrDefault(a => a.Day == dayofyear && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Day = dayofyear;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData3.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Month:

                        foreach (var itm in prods)
                        {
                            int month = itm.Date.Month;
                            int year = itm.Date.Year;

                            KPIReportData existed = ReportData3.FirstOrDefault(a => a.Month == month && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Month = month;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData3.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Week:

                        foreach (var itm in prods)
                        {
                            int year = itm.Date.Year;

                            KPIReportData existed = ReportData3.FirstOrDefault(a => a.Week == itm.Week && a.Year == year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Week = itm.Week;
                                data.Year = year;
                                data.Date = itm.Date.Date;

                                ReportData3.Add(data);
                            }
                        }
                        break;
                    case ShowBy.Year:

                        foreach (var itm in prods)
                        {

                            KPIReportData existed = ReportData3.FirstOrDefault(a => a.Week == itm.Date.Year);

                            if (existed != null)
                            {
                                existed.ItemsCount++;
                                existed.KpiActualTime += itm.ActualTime_min;
                                existed.KpiProducedPcs += itm.ProducedPieces;
                                existed.ScrapPcs += itm.ScrapPieces;
                                existed.HlpPerformance += itm.hlpPerformance;
                                existed.HlpProductivity += itm.hlpHrProductivity;
                                existed.ActualPcsPerHeadhour += itm.ActPiecesPerHeadHour;
                                existed.OperationalTime += itm.ActOperationalTime_min;
                                existed.ActualPcsPerHeadhourOperTime += itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;
                            }
                            else
                            {
                                data = new KPIReportData();
                                data.IdDivision = SelectedDivision != null ? SelectedDivision.ID : 0;
                                data.KpiActualTime = itm.ActualTime_min;
                                data.KpiProducedPcs = itm.ProducedPieces;
                                data.ScrapPcs = itm.ScrapPieces;
                                data.HlpPerformance = itm.hlpPerformance;
                                data.HlpProductivity = itm.hlpHrProductivity;
                                data.ActualPcsPerHeadhour = itm.ActPiecesPerHeadHour;
                                data.OperationalTime = itm.ActOperationalTime_min;
                                data.ActualPcsPerHeadhourOperTime = itm.ActPiecesPerHeadHour * itm.ActOperationalTime_min;

                                data.Year = itm.Date.Year;
                                data.Date = itm.Date.Date;

                                ReportData3.Add(data);
                            }
                        }
                        break;
                }
            }
        }

        public ReportMode ReportingMode
        {
            get
            {
                ReportMode rm = ReportMode.AllWCs;

                if (IsAllWcsSelected)
                {
                    rm = ReportMode.AllWCs;
                }
                else if (IsAllWcsSelected == false && SelectedWorkGroup != null)
                {
                    rm = ReportMode.SomeWCs;
                }
                else if (IsAllWcsSelected == false && SelectedWorkGroup == null)
                {
                    rm = ReportMode.AllWGs;
                }

                return rm;
            }
        }

        public enum ReportMode
        {
            AllWCs,
            AllWGs,
            SomeWCs
        }

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

        public decimal KpiActualPcsPerHeadhour
        {
            get
            {
                decimal result = 0;

                Dictionary<string, double> values = new Dictionary<string, double>();

                values.Add("OperationalTime", (double)OperationalTime);
                values.Add("SumActPiecesPerHeadHourOperationalTime", (double)ActualPcsPerHeadhourOperTime);


                result = Math.Round((decimal)Calculation.CalculateFunction("ReportActPcsPerHeadHour", values), 2);


                return result;
            }
        }


        public decimal HlpPerformance { set; get; }

        public int ScrapPcs { set; get; }

        public decimal ActualPcsPerHeadhourOperTime
        {
            set;
            get;
        }



        public decimal OperationalTime
        {
            set;
            get;
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

        private string _usedWorkCentersText = ResourceHelper.GetResource<string>("rep_FiltrUse");
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

        private string _usedWorkGroupsText = ResourceHelper.GetResource<string>("rep_FiltrUse");
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

        private ShowBy _selectedShowBy = ShowBy.All;
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


        private List<KPIReportData> _reportData1 = null;
        public List<KPIReportData> ReportData1
        {
            set
            {
                _reportData1 = value;
                OnPropertyChanged("ReportData1");
            }
            get
            {
                return _reportData1;
            }
        }

        private List<KPIReportData> _reportData2 = null;
        public List<KPIReportData> ReportData2
        {
            set
            {
                _reportData2 = value;
                OnPropertyChanged("ReportData2");
            }
            get
            {
                return _reportData2;
            }
        }

        private List<KPIReportData> _reportData3 = null;
        public List<KPIReportData> ReportData3
        {
            set
            {
                _reportData3 = value;
                OnPropertyChanged("ReportData3");
            }
            get
            {
                return _reportData3;
            }
        }

        private object _selectedItem1 = null;
        public object SelectedItem1
        {
            set
            {
                _selectedItem1 = value;
                OnPropertyChanged("SelectedItem1");
            }
            get
            {
                return _selectedItem1;
            }
        }

        private object _selectedItem2 = null;
        public object SelectedItem2
        {
            set
            {
                _selectedItem2 = value;
                OnPropertyChanged("SelectedItem2");
            }
            get
            {
                return _selectedItem2;
            }
        }

        private object _selectedItem3 = null;
        public object SelectedItem3
        {
            set
            {
                _selectedItem3 = value;
                OnPropertyChanged("SelectedItem3");
            }
            get
            {
                return _selectedItem3;
            }
        }

        public IList<object> ItemsSource1 { set; get; }
        public IList<object> ItemsSource2 { set; get; }
        public IList<object> ItemsSource3 { set; get; }


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

        private bool _isAllWcsSelected = true;
        public bool IsAllWcsSelected
        {
            set
            {
                if (_isAllWcsSelected != value)
                {
                    _isAllWcsSelected = value;
                    OnPropertyChanged("IsAllWcsSelected");
                }
            }
            get
            {
                return _isAllWcsSelected;
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
                displayMembers.Add(ShowBy.All, COS.Resources.ResourceHelper.GetResource<string>("rep_Day"));
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
                displayMembers.Add(ShowBy.All, "Date");
                displayMembers.Add(ShowBy.Week, "Week");
                displayMembers.Add(ShowBy.Month, "Month");

                return displayMembers;
            }
        }
    }



}
