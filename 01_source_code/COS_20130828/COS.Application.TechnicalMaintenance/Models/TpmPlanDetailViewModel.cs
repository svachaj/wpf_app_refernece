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
using COS.Application.TechnicalMaintenance.Views;
using System.Windows.Data;
using System.Transactions;

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class TpmPlanDetailViewModel : ValidationViewModelBase
    {
        public TpmPlanDetailViewModel()
            : base()
        {

        }


        private TpmPlan _selectedItem = null;
        public TpmPlan SelectedItem
        {
            set
            {
                FirstLoad = true;
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedItem.PropertyChanged -= _selectedItem_PropertyChanged;
                    _selectedItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedItem_PropertyChanged);

                    if (_selectedItem.Recurrence != null)
                    {
                        SelectedItem.Recurrence.PropertyChanged -= Recurrence_PropertyChanged;
                        SelectedItem.Recurrence.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Recurrence_PropertyChanged);
                    }
                }
                // COSContext.Current.TpmCheckLists
                OnPropertyChanged("SelectedItem");

            }
            get
            {
                return _selectedItem;
            }
        }



        public List<TpmCheckList> LocalCheckLists
        {
            get
            {
                return COSContext.Current.TpmCheckLists.Where(a => a.IsPreImage == true).OrderBy(a=>a.CheckListName).ToList();
            }
        }

        void _selectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "TpmStartDateTime")
            //{
            //    if (SelectedItem.TpmStartDateTime > SelectedItem.TpmEndDatetime)
            //    {
            //        SelectedItem.TpmStartDateTime = SelectedItem.TpmEndDatetime;
            //    }
            //}
            //else if (e.PropertyName == "TpmEndDatetime")
            //{
            //    if (SelectedItem.TpmStartDateTime > SelectedItem.TpmEndDatetime)
            //    {
            //        SelectedItem.TpmEndDatetime = SelectedItem.TpmStartDateTime;
            //    }
            //}
            if (e.PropertyName == "ID_MachineEquipment")
            {
                OnPropertyChanged("Equipment");
            }
            else if (e.PropertyName == "TimePlanMode")
            {
                if (EditingMode == EditMode.New)
                {
                    if (SelectedItem.TimePlanMode == 1)
                    {
                        if (SelectedItem.Recurrence != null)
                        {
                            SelectedItem.Recurrence = null;
                        }
                    }
                    else
                    {
                        if (SelectedItem.Recurrence == null)
                        {
                            TpmRecurrencePattern recur = COSContext.Current.CreateObject<TpmRecurrencePattern>();
                            recur.ID_Recurrence = Guid.NewGuid();
                            recur.isDaily = true;
                            recur.isDaily_EveryDay_Days = 1;
                            recur.isMonthlyDay = 1;
                            recur.isMonthlyMonth = 1;
                            recur.isWeeklyRecursEveryWeek = 1;
                            recur.RangeOfReccStartDate = DateTime.Now.Date;
                            recur.RangeOfReccEndDate = DateTime.Now.Date;
                            SelectedItem.Recurrence = recur;
                            SelectedItem.Recurrence.PropertyChanged -= Recurrence_PropertyChanged;
                            SelectedItem.Recurrence.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Recurrence_PropertyChanged);

                            OnPropertyChanged("Recurrency");
                        }
                    }
                }
            }
            else if (e.PropertyName == "TpmStartDateTime")
            {
                if (SelectedItem.TpmPlannedTime.HasValue)
                    SelectedItem.TpmEndDatetime = SelectedItem.TpmStartDateTime.AddMinutes(SelectedItem.TpmPlannedTime.Value);
            }
        }

        void Recurrence_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StartTime")
            {
                if (SelectedItem.Recurrence != null && SelectedItem.TpmPlannedTime.HasValue)
                    SelectedItem.Recurrence.EndTime = new TimeSpan(0, (int)SelectedItem.Recurrence.StartTime.TotalMinutes + SelectedItem.TpmPlannedTime.Value, 0);
            }
        }


        public bool FirstLoad = true;

        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save(true));
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }

        public Guid? IsRecurrencyUpdating { set; get; }

        public void Save(bool showAffectedHolidays)
        {

            string customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {

                AffectedHolidayDays.Clear();

                if (SelectedItem.Recurrence != null && EditingMode == EditMode.New)
                {
                    customErrors = CreateRecurrencyItems();
                }

                if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        try
                        {
                            if (IsRecurrencyUpdating.HasValue)
                            {
                                var plansToDel = COSContext.Current.TpmPlans.Where(a => a.ID_Recurrence == IsRecurrencyUpdating.Value && a.TpmStartDateTime >= SelectedItem.TpmStartDateTime);

                                foreach (var itm in plansToDel)
                                    COSContext.Current.DeleteObject(itm);
                            }

                            if (SelectedItem.ID == 0 || SelectedItem.CheckList.EntityState != System.Data.EntityState.Unchanged)
                            {
                                SelectedItem.CheckList = SelectedItem.CheckList.Copy();
                            }

                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            scope.Complete();
                        }
                        catch (Exception exc)
                        {
                            Logging.LogException(exc, LogType.ToFileAndEmail);
                            scope.Dispose();
                            COSContext.Current.RejectChanges();

                            RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                        }

                    }

                    OnPropertyChanged("Save");


                    if (AffectedHolidayDays.Count > 0 && showAffectedHolidays)
                    {
                        string msg = COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000001");

                        foreach (var itm in AffectedHolidayDays)
                        {
                            msg += Environment.NewLine;
                            msg += itm.HolidayDay.ToShortDateString() + ": " + itm.HolidayDescription;
                        }

                        RadWindow.Alert(new DialogParameters() { Content = msg, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    }
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }


        public List<CalendarHoliday> AffectedHolidayDays = new List<CalendarHoliday>();

        private string CreateRecurrencyItems()
        {
            SelectedItem.Recurrence.PropertyChanged -= Recurrence_PropertyChanged;
            SelectedItem.PropertyChanged -= _selectedItem_PropertyChanged;

            string result = "";

            AffectedHolidayDays.Clear();

            var holidays = COSContext.Current.CalendarHolidays.ToList();

            int itemsCreated = 0;
            int eachDayCount = 0;
            int eachWeekCount = 0;
            int eachMonthCount = 1;
            int curWeekk = 0;
            bool firstMonthLoad = true;

            if (SelectedItem.Recurrence.isDaily)
            {

                if (!SelectedItem.Recurrence.isDaily_EveryDay && !SelectedItem.Recurrence.isDaily_Weekdays)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000002");
                    result += Environment.NewLine;
                }
                else if (SelectedItem.Recurrence.isDaily_EveryDay && SelectedItem.Recurrence.isDaily_Weekdays)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000002");
                    result += Environment.NewLine;
                }
                else
                {


                    if (SelectedItem.Recurrence.isDaily_EveryDay)
                    {
                        for (int i = 0; i < (SelectedItem.Recurrence.RangeOfReccEndDate - SelectedItem.Recurrence.RangeOfReccStartDate).Days + 1; i++)
                        {
                            eachDayCount++;
                            DateTime curDate = SelectedItem.Recurrence.RangeOfReccStartDate.AddDays(i);

                            if (eachDayCount == SelectedItem.Recurrence.isDaily_EveryDay_Days)
                            {
                                eachDayCount = 0;

                                var hol = holidays.FirstOrDefault(a => a.HolidayDay.Date.Equals(curDate.Date));

                                if (hol != null)
                                {
                                    AffectedHolidayDays.Add(hol);
                                }
                                else
                                {
                                    CreateNewPlanItem(itemsCreated, curDate + SelectedItem.Recurrence.StartTime, curDate + SelectedItem.Recurrence.EndTime);
                                    itemsCreated++;
                                }
                            }

                        }
                    }
                    else if (SelectedItem.Recurrence.isDaily_Weekdays)
                    {
                        for (int i = 0; i < (SelectedItem.Recurrence.RangeOfReccEndDate - SelectedItem.Recurrence.RangeOfReccStartDate).Days + 1; i++)
                        {
                            DateTime curDate = SelectedItem.Recurrence.RangeOfReccStartDate.AddDays(i);

                            int dw = (int)curDate.DayOfWeek;
                            if (dw >= 1 && dw <= 5)
                            {
                                var hol = holidays.FirstOrDefault(a => a.HolidayDay.Date.Equals(curDate.Date));

                                if (hol != null)
                                {
                                    AffectedHolidayDays.Add(hol);
                                }
                                else
                                {
                                    CreateNewPlanItem(itemsCreated, curDate + SelectedItem.Recurrence.StartTime, curDate + SelectedItem.Recurrence.EndTime);
                                    itemsCreated++;
                                }
                            }

                        }
                    }

                }
            }
            else if (SelectedItem.Recurrence.isWeekly)
            {
                COS.RecurrenceGenerator.WeeklyRecurrenceSettings weekSett = new RecurrenceGenerator.WeeklyRecurrenceSettings(SelectedItem.Recurrence.RangeOfReccStartDate, SelectedItem.Recurrence.RangeOfReccEndDate);

                var dates = weekSett.GetValues(SelectedItem.Recurrence.isWeeklyRecursEveryWeek, new RecurrenceGenerator.SelectedDayOfWeekValues() { Monday = SelectedItem.Recurrence.isWeeklyMonday, Tuesday = SelectedItem.Recurrence.isWeeklyTuesday, Wednesday = SelectedItem.Recurrence.isWeeklyWednesday, Thursday = SelectedItem.Recurrence.isWeeklyThursday, Friday = SelectedItem.Recurrence.isWeeklyFriday, Saturday = SelectedItem.Recurrence.isWeeklySaturday, Sunday = SelectedItem.Recurrence.isWeeklySunday });

                foreach (var dt in dates.Values)
                {
                    var hol = holidays.FirstOrDefault(a => a.HolidayDay.Date.Equals(dt.Date));

                    if (hol != null)
                    {
                        AffectedHolidayDays.Add(hol);
                    }
                    else
                    {
                        CreateNewPlanItem(itemsCreated, dt + SelectedItem.Recurrence.StartTime, dt + SelectedItem.Recurrence.EndTime);
                        itemsCreated++;
                    }
                }
            }
            else if (SelectedItem.Recurrence.isMonthly)
            {
                COS.RecurrenceGenerator.MonthlyRecurrenceSettings monthly = new RecurrenceGenerator.MonthlyRecurrenceSettings(SelectedItem.Recurrence.RangeOfReccStartDate, SelectedItem.Recurrence.RangeOfReccEndDate);

                var dates = monthly.GetValues(SelectedItem.Recurrence.isMonthlyDay.Value, SelectedItem.Recurrence.isMonthlyMonth.Value);

                foreach (var dt in dates.Values)
                {
                    var hol = holidays.FirstOrDefault(a => a.HolidayDay.Date.Equals(dt.Date));

                    if (hol != null)
                    {
                        AffectedHolidayDays.Add(hol);
                    }
                    else
                    {
                        CreateNewPlanItem(itemsCreated, dt + SelectedItem.Recurrence.StartTime, dt + SelectedItem.Recurrence.EndTime);
                        itemsCreated++;
                    }
                }
            }

            if (itemsCreated == 0)
            {
                result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000003");
                result += Environment.NewLine;
            }

            SelectedItem.Recurrence.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Recurrence_PropertyChanged);
            SelectedItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedItem_PropertyChanged);

            return result;
        }

        private void CreateNewPlanItem(int itemsCreated, DateTime startDateTime, DateTime endDatetTime)
        {
            if (itemsCreated == 0)
            {
                SelectedItem.TpmStartDateTime = startDateTime;
                SelectedItem.TpmEndDatetime = endDatetTime;
                SelectedItem.ID_Recurrence = SelectedItem.Recurrence.ID_Recurrence;
            }
            else
            {
                TpmPlan item = new TpmPlan();

                item.TimePlanMode = SelectedItem.TimePlanMode;
                item.TpmStartDateTime = startDateTime;
                item.TpmEndDatetime = endDatetTime;
                item.Recurrence = SelectedItem.Recurrence;
                item.Equipment = SelectedItem.Equipment;
                item.CheckList = SelectedItem.CheckList;
                item.ID_Priority = SelectedItem.ID_Priority;
                item.ID_Status = SelectedItem.ID_Status;
                item.TpmPlannedLabour = SelectedItem.TpmPlannedLabour;
                item.TpmPlannedTime = SelectedItem.TpmPlannedTime;
                item.ID_Recurrence = SelectedItem.Recurrence.ID_Recurrence;

                COSContext.Current.TpmPlans.AddObject(item);
            }
        }


        public void Cancel()
        {
            //COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        private string IsValid()
        {
            string result = "";


            if (SelectedItem != null)
            {
                if (SelectedItem.Equipment == null)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000004");
                    result += Environment.NewLine;
                }

                if (SelectedItem.CheckList == null)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000005");
                    result += Environment.NewLine;
                }

                if (SelectedItem.TpmPlannedLabour.HasValue && SelectedItem.TpmPlannedLabour.Value < 1)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000006");
                    result += Environment.NewLine;
                }

                if (SelectedItem.TpmPlannedLabour.HasValue && SelectedItem.TpmPlannedTime.Value < 1)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000007");
                    result += Environment.NewLine;
                }

                if (EditingMode == EditMode.New)
                {
                    if (SelectedItem.TimePlanMode == 1)
                    {
                        if (SelectedItem.TpmStartDateTime == DateTime.MinValue)
                        {
                            result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000008");
                            result += Environment.NewLine;
                        }

                        if (SelectedItem.TpmEndDatetime == DateTime.MinValue)
                        {
                            result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000009");
                            result += Environment.NewLine;
                        }

                        if (SelectedItem.TpmStartDateTime != DateTime.MinValue && SelectedItem.TpmStartDateTime != DateTime.MinValue)
                        {
                            if (SelectedItem.TpmStartDateTime >= SelectedItem.TpmEndDatetime)
                            {
                                result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000010");
                                result += Environment.NewLine;
                            }
                        }
                    }
                    else if (SelectedItem.TimePlanMode == 2)
                    {
                        if (SelectedItem.Recurrence != null)
                        {
                            if (SelectedItem.Recurrence.RangeOfReccStartDate == DateTime.MinValue)
                            {
                                result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000011");
                                result += Environment.NewLine;
                            }

                            if (SelectedItem.Recurrence.RangeOfReccEndDate == DateTime.MinValue)
                            {
                                result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000012");
                                result += Environment.NewLine;
                            }


                            if (SelectedItem.TpmStartDateTime != DateTime.MinValue && SelectedItem.TpmStartDateTime != DateTime.MinValue)
                            {
                                if (SelectedItem.Recurrence.RangeOfReccStartDate >= SelectedItem.Recurrence.RangeOfReccEndDate)
                                {
                                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000013");
                                    result += Environment.NewLine;
                                }
                                else if ((SelectedItem.Recurrence.RangeOfReccEndDate - SelectedItem.Recurrence.RangeOfReccStartDate).TotalDays > (365 * 5))
                                {
                                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000014");
                                    result += Environment.NewLine;
                                }
                            }

                            if (SelectedItem.Recurrence.StartTime == TimeSpan.MinValue)
                            {
                                result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000015");
                                result += Environment.NewLine;
                            }

                            if (SelectedItem.Recurrence.EndTime == TimeSpan.MinValue)
                            {
                                result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000016");
                                result += Environment.NewLine;
                            }


                            if (SelectedItem.Recurrence.StartTime != TimeSpan.MinValue && SelectedItem.Recurrence.EndTime != TimeSpan.MinValue)
                            {
                                if (SelectedItem.Recurrence.StartTime >= SelectedItem.Recurrence.EndTime)
                                {
                                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000017");
                                    result += Environment.NewLine;
                                }
                            }
                        }
                    }
                }
                else if (EditingMode == EditMode.Edit)
                {
                    if (SelectedItem.TpmStartDateTime == DateTime.MinValue)
                    {
                        result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000008");
                        result += Environment.NewLine;
                    }

                    if (SelectedItem.TpmEndDatetime == DateTime.MinValue)
                    {
                        result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000009");
                        result += Environment.NewLine;
                    }

                    if (SelectedItem.TpmStartDateTime != DateTime.MinValue && SelectedItem.TpmStartDateTime != DateTime.MinValue)
                    {
                        if (SelectedItem.TpmStartDateTime >= SelectedItem.TpmEndDatetime)
                        {
                            result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000010");
                            result += Environment.NewLine;
                        }
                    }
                }

                if (SelectedItem.ID_Priority < 1)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000018");
                    result += Environment.NewLine;
                }

                if (SelectedItem.ID_Status < 1)
                {
                    result += COS.Resources.ResourceHelper.GetResource<string>("m_Body_TM00000019");
                    result += Environment.NewLine;
                }
            }

            return result;
        }



    }

    public class EditModeHPConvertor1 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.Edit)
            {

                if (parameter != null)
                {
                    string param = parameter.ToString();

                    if (param == "Save")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Update");

                        return res;
                    }
                    else if (param == "Delete")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Delete");

                        return res;
                    }
                    else if (param == "Setting1h")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Update");

                        return res;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else if (mode == EditMode.New)
            {
                return false;
            }
            else if (mode == EditMode.View)
            {
                return false;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class EditModeHPConvertor2 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.Edit || mode == EditMode.New)
            {

                if (parameter != null)
                {
                    string param = parameter.ToString();

                    if (param == "AddNew")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Insert");

                        return res;
                    }
                    else if (param == "ShiftNP" || param == "ShiftPM")
                    {
                        if (COSContext.Current.SelectedHourlyProductions != null && COSContext.Current.SelectedHourlyProductions.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            bool res = false;

                            res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Insert");

                            return res;
                        }

                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }

            }
            else if (mode == EditMode.View)
            {
                return false;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class EditModeHPConvertor3 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.Edit)
            {
                return true;
            }
            else if (mode == EditMode.New)
            {
                return true;
            }
            else if (mode == EditMode.View)
            {
                return true;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
