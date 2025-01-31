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
    public partial class ADSProductionWCShiftRangeModel : ValidationViewModelBase
    {
        public ADSProductionWCShiftRangeModel()
            : base()
        {
            Formatting = new ConditionalFormatClassWCShift();

            var props = COSContext.Current.UserProperties.Where(a => a.Key.Contains("ConFormat_ADS_WCS") && a.ID_user == null).ToList();

            Formatting.Availability_Eq = GetUserProp("ConFormat_ADS_WCS_Availability_Eq", props);
            Formatting.Availability_LessEq = GetUserProp("ConFormat_ADS_WCS_Availability_LessEq", props);
            Formatting.Availability_More = GetUserProp("ConFormat_ADS_WCS_Availability_More", props);
            Formatting.Availability_MoreEq = GetUserProp("ConFormat_ADS_WCS_Availability_MoreEq", props);

            Formatting.OEE_Eq = GetUserProp("ConFormat_ADS_WCS_OEE_Eq", props);
            Formatting.OEE_LessEq = GetUserProp("ConFormat_ADS_WCS_OEE_LessEq", props);
            Formatting.OEE_More = GetUserProp("ConFormat_ADS_WCS_OEE_More", props);
            Formatting.OEE_MoreEq = GetUserProp("ConFormat_ADS_WCS_OEE_MoreEq", props);

            Formatting.Quality_Eq = GetUserProp("ConFormat_ADS_WCS_Quality_Eq", props);
            Formatting.Quality_LessEq = GetUserProp("ConFormat_ADS_WCS_Quality_LessEq", props);
            Formatting.Quality_More = GetUserProp("ConFormat_ADS_WCS_Quality_More", props);
            Formatting.Quality_MoreEq = GetUserProp("ConFormat_ADS_WCS_Quality_MoreEq", props);

            Formatting.Performance_Eq = GetUserProp("ConFormat_ADS_WCS_Performance_Eq", props);
            Formatting.Performance_LessEq = GetUserProp("ConFormat_ADS_WCS_Performance_LessEq", props);
            Formatting.Performance_More = GetUserProp("ConFormat_ADS_WCS_Performance_More", props);
            Formatting.Performance_MoreEq = GetUserProp("ConFormat_ADS_WCS_Performance_MoreEq", props);

            Formatting.Availability_Eq_VA = GetUserProp("ConFormat_ADS_WCS_Availability_Eq_VA", props);
            Formatting.Availability_LessEq_VA = GetUserProp("ConFormat_ADS_WCS_Availability_LessEq_VA", props);
            Formatting.Availability_More_VA = GetUserProp("ConFormat_ADS_WCS_Availability_More_VA", props);
            Formatting.Availability_MoreEq_VA = GetUserProp("ConFormat_ADS_WCS_Availability_MoreEq_VA", props);

            Formatting.OEE_Eq_VA = GetUserProp("ConFormat_ADS_WCS_OEE_Eq_VA", props);
            Formatting.OEE_LessEq_VA = GetUserProp("ConFormat_ADS_WCS_OEE_LessEq_VA", props);
            Formatting.OEE_More_VA = GetUserProp("ConFormat_ADS_WCS_OEE_More_VA", props);
            Formatting.OEE_MoreEq_VA = GetUserProp("ConFormat_ADS_WCS_OEE_MoreEq_VA", props);

            Formatting.Quality_Eq_VA = GetUserProp("ConFormat_ADS_WCS_Quality_Eq_VA", props);
            Formatting.Quality_LessEq_VA = GetUserProp("ConFormat_ADS_WCS_Quality_LessEq_VA", props);
            Formatting.Quality_More_VA = GetUserProp("ConFormat_ADS_WCS_Quality_More_VA", props);
            Formatting.Quality_MoreEq_VA = GetUserProp("ConFormat_ADS_WCS_Quality_MoreEq_VA", props);

            Formatting.Performance_Eq_VA = GetUserProp("ConFormat_ADS_WCS_Performance_Eq_VA", props);
            Formatting.Performance_LessEq_VA = GetUserProp("ConFormat_ADS_WCS_Performance_LessEq_VA", props);
            Formatting.Performance_More_VA = GetUserProp("ConFormat_ADS_WCS_Performance_More_VA", props);
            Formatting.Performance_MoreEq_VA = GetUserProp("ConFormat_ADS_WCS_Performance_MoreEq_VA", props);

            Formatting.More_Color = GetUserPropColor("ConFormat_ADS_WCS_More_Color", props);
            Formatting.LessEq_Color = GetUserPropColor("ConFormat_ADS_WCS_LessEq_Color", props);
            Formatting.Eq_Color = GetUserPropColor("ConFormat_ADS_WCS_Eq_Color", props);
            Formatting.MoreEq_Color = GetUserPropColor("ConFormat_ADS_WCS_MoreEq_Color", props);
            Formatting.Less_Color = GetUserPropColor("ConFormat_ADS_WCS_Less_Color", props);
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

        public void SaveFormatting()
        {
            var props = COSContext.Current.UserProperties.Where(a => a.Key.Contains("ConFormat_ADS_WCS") && a.ID_user == null).ToList();

            SetUserProp("ConFormat_ADS_WCS_Availability_Eq", Formatting.Availability_Eq, props);
            SetUserProp("ConFormat_ADS_WCS_Availability_LessEq", Formatting.Availability_LessEq, props);
            SetUserProp("ConFormat_ADS_WCS_Availability_More", Formatting.Availability_More, props);
            SetUserProp("ConFormat_ADS_WCS_Availability_MoreEq", Formatting.Availability_MoreEq, props);

            SetUserProp("ConFormat_ADS_WCS_Performance_Eq", Formatting.Performance_Eq, props);
            SetUserProp("ConFormat_ADS_WCS_Performance_LessEq", Formatting.Performance_LessEq, props);
            SetUserProp("ConFormat_ADS_WCS_Performance_More", Formatting.Performance_More, props);
            SetUserProp("ConFormat_ADS_WCS_Performance_MoreEq", Formatting.Performance_MoreEq, props);

            SetUserProp("ConFormat_ADS_WCS_Quality_Eq", Formatting.Quality_Eq, props);
            SetUserProp("ConFormat_ADS_WCS_Quality_LessEq", Formatting.Quality_LessEq, props);
            SetUserProp("ConFormat_ADS_WCS_Quality_More", Formatting.Quality_More, props);
            SetUserProp("ConFormat_ADS_WCS_Quality_MoreEq", Formatting.Quality_MoreEq, props);

            SetUserProp("ConFormat_ADS_WCS_OEE_Eq", Formatting.OEE_Eq, props);
            SetUserProp("ConFormat_ADS_WCS_OEE_LessEq", Formatting.OEE_LessEq, props);
            SetUserProp("ConFormat_ADS_WCS_OEE_More", Formatting.OEE_More, props);
            SetUserProp("ConFormat_ADS_WCS_OEE_MoreEq", Formatting.OEE_MoreEq, props);

            SetUserProp("ConFormat_ADS_WCS_Availability_Eq_VA", Formatting.Availability_Eq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Availability_LessEq_VA", Formatting.Availability_LessEq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Availability_More_VA", Formatting.Availability_More_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Availability_MoreEq_VA", Formatting.Availability_MoreEq_VA, props);

            SetUserProp("ConFormat_ADS_WCS_Performance_Eq_VA", Formatting.Performance_Eq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Performance_LessEq_VA", Formatting.Performance_LessEq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Performance_More_VA", Formatting.Performance_More_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Performance_MoreEq_VA", Formatting.Performance_MoreEq_VA, props);

            SetUserProp("ConFormat_ADS_WCS_Quality_Eq_VA", Formatting.Quality_Eq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Quality_LessEq_VA", Formatting.Quality_LessEq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Quality_More_VA", Formatting.Quality_More_VA, props);
            SetUserProp("ConFormat_ADS_WCS_Quality_MoreEq_VA", Formatting.Quality_MoreEq_VA, props);

            SetUserProp("ConFormat_ADS_WCS_OEE_Eq_VA", Formatting.OEE_Eq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_OEE_LessEq_VA", Formatting.OEE_LessEq_VA, props);
            SetUserProp("ConFormat_ADS_WCS_OEE_More_VA", Formatting.OEE_More_VA, props);
            SetUserProp("ConFormat_ADS_WCS_OEE_MoreEq_VA", Formatting.OEE_MoreEq_VA, props);

            try
            {
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                OnPropertyChanged("FormattingSaved");
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);

            }

            //Formatting.More_Color = GetUserPropColor("ConFormat_ADS_WCS_More_Color", props);
            //Formatting.LessEq_Color = GetUserPropColor("ConFormat_ADS_WCS_LessEq_Color", props);
            //Formatting.Eq_Color = GetUserPropColor("ConFormat_ADS_WCS_Eq_Color", props);
            //Formatting.MoreEq_Color = GetUserPropColor("ConFormat_ADS_WCS_MoreEq_Color", props);
            //Formatting.Less_Color = GetUserPropColor("ConFormat_ADS_WCS_Less_Color", props);
        }

        public ConditionalFormatClassWCShift Formatting { set; get; }

        private DateTime? _selectedDateFrom = COSContext.Current.DateTimeServer.Date;
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

        private DateTime? _selectedDateTo = COSContext.Current.DateTimeServer.Date;
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


        public List<Division> LocalDivisions
        {
            get
            {
                return COSContext.Current.Divisions.ToList();
            }
        }

        public List<ShiftType> LocalShiftTypes
        {
            get
            {
                return COSContext.Current.ShiftTypes.ToList().OrderBy(a => a.ShiftHours).ToList();
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


        private List<WCShiftDataClass> _reportData = new List<WCShiftDataClass>();
        public List<WCShiftDataClass> ReportData
        {
            get
            {
                return _reportData;
            }
        }

        public void GenerateData()
        {
            var cc = COSContext.Current;

            if (SelectedDateFrom.HasValue && SelectedDateTo.HasValue && SelectedDivision != null && SelectedShiftType != null)
            {
                ReportData.Clear();

                var prods = cc.HourlyProductions.Where(a => a.ID_Division == SelectedDivision.ID && a.ID_ShiftType == SelectedShiftType.ID && a.Date >= SelectedDateFrom && a.Date <= SelectedDateTo);

                var groupByDay = prods.ToList().OrderBy(a => a.WorkCenter.Value).GroupBy(a => a.WorkCenter);

                DivisionString = SelectedDivision.Description;
                ShiftString = SelectedShiftType.Description;


                WCShiftDataClass data = null;
                foreach (var itm in groupByDay)
                {
                    data = new WCShiftDataClass();
                    data.WorkCenterString = itm.Key.StringForDivisions;

                    foreach (var dt in itm)
                    {
                        data.KPIData.KpiActualTime += dt.ActualTime_min;
                        data.KPIData.KpiProducedPcs += dt.ProducedPieces;
                        data.KPIData.ScrapPcs += dt.ScrapPieces;
                        data.KPIData.HlpPerformance += dt.hlpPerformance;
                        data.KPIData.HlpProductivity += dt.hlpHrProductivity;
                        data.KPIData.ActualPcsPerHeadhour += dt.ActPiecesPerHeadHour;
                        data.KPIData.OperationalTime += dt.ActOperationalTime_min;
                        data.Time += dt.ActualTime_min;
                    }

                    ReportData.Add(data);
                }
            }


            if (SelectedDateFrom.HasValue && SelectedDateTo.HasValue && SelectedDivision != null && SelectedShiftType == null)
            {
                ReportData.Clear();

                var prods = cc.HourlyProductions.Where(a => a.ID_Division == SelectedDivision.ID && a.Date >= SelectedDateFrom && a.Date <= SelectedDateTo);

                var groupByDay = prods.ToList().OrderBy(a => a.WorkCenter.Value).GroupBy(a => a.WorkCenter);

                DivisionString = SelectedDivision.Description;
                ShiftString = null;


                WCShiftDataClass data = null;
                foreach (var itm in groupByDay)
                {
                    data = new WCShiftDataClass();
                    data.WorkCenterString = itm.Key.StringForDivisions;

                    foreach (var dt in itm)
                    {
                        data.KPIData.KpiActualTime += dt.ActualTime_min;
                        data.KPIData.KpiProducedPcs += dt.ProducedPieces;
                        data.KPIData.ScrapPcs += dt.ScrapPieces;
                        data.KPIData.HlpPerformance += dt.hlpPerformance;
                        data.KPIData.HlpProductivity += dt.hlpHrProductivity;
                        data.KPIData.ActualPcsPerHeadhour += dt.ActPiecesPerHeadHour;
                        data.KPIData.OperationalTime += dt.ActOperationalTime_min;
                        data.Time += dt.ActualTime_min;
                    }

                    ReportData.Add(data);
                }
            }
        }

        public string DivisionString { set; get; }
        public string ShiftString { set; get; }

    }




}
