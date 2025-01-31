using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using COS.Common.WPF;
using System.ComponentModel;
using COS.Application.Shared;
using COS.Application.TechnicalMaintenance.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Resources;

namespace COS.Application.TechnicalMaintenance.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class TpmPlanViewModelView : BaseUserControl
    {
        public TpmPlanViewModelView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new TpmPlanViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);


            }
        }

        TechnicalMaintenance.Models.TpmPlanViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {

            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RebindData")
            {
                Model.RefreshCommand.Execute(null);
            }
            else if (e.PropertyName == "AddNewItem")
            {
                AddNewItem();
            }
            else if (e.PropertyName == "Deleted")
            {
                Model.RefreshCommand.Execute(null);
                grvEquips.Rebind();
            }
            else if (e.PropertyName == "UpdateRecurrency")
            {
                UpdateRecurrency();
            }
            else if (e.PropertyName == "SelectedDate")
            {
                grvEquips.Rebind();
            }
        }

        private void UpdateRecurrency()
        {
            if (Model.SelectedItem != null && Model.SelectedItem.Recurrence != null)
            {
                var modelItem = Model.SelectedItem;


                TpmPlan item = new TpmPlan();

                if (item != null)
                {
                    if (TpmPlanDetailWindow == null)
                    {
                        TpmPlanDetailWindow = new RadWindow();

                        TpmPlanDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                        TpmPlanDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(TpmPlanDetailWindow_Closed);
                        TpmPlanDetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("tm_TpmNewPlan");
                        TpmPlanDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        TpmPlanDetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                        detailView = new TpmPlanDetailView();
                        detailView.RaiseWindow = TpmPlanDetailWindow;
                        TpmPlanDetailWindow.Content = detailView;

                        StyleManager.SetTheme(TpmPlanDetailWindow, new Expression_DarkTheme());
                    }

                    item.TimePlanMode = 2;
                    item.Equipment = modelItem.Equipment;
                    item.CheckList = modelItem.CheckList;
                    item.TpmPlannedLabour = modelItem.TpmPlannedLabour;
                    item.TpmPlannedTime = modelItem.TpmPlannedTime;
                    item.TpmPriority = modelItem.TpmPriority;
                    item.TpmStatus = modelItem.TpmStatus;
                    item.TpmStartDateTime = DateTime.Today.AddHours(modelItem.TpmStartDateTime.Hour).AddMinutes(modelItem.TpmStartDateTime.Minute);
                    item.TpmEndDatetime = DateTime.Today.AddHours(modelItem.TpmEndDatetime.Hour).AddMinutes(modelItem.TpmEndDatetime.Minute);

                    TpmRecurrencePattern recur = COSContext.Current.CreateObject<TpmRecurrencePattern>();
                    recur.ID_Recurrence = Guid.NewGuid();
                    recur.isDaily = modelItem.Recurrence.isDaily;
                    recur.isDaily_EveryDay = modelItem.Recurrence.isDaily_EveryDay;
                    recur.isDaily_EveryDay_Days = modelItem.Recurrence.isDaily_EveryDay_Days;
                    recur.isDaily_Weekdays = modelItem.Recurrence.isDaily_Weekdays;
                    recur.isMonthly = modelItem.Recurrence.isMonthly;
                    recur.isMonthlyDay = modelItem.Recurrence.isMonthlyDay;
                    recur.isMonthlyMonth = modelItem.Recurrence.isMonthlyMonth;
                    recur.isWeekly = modelItem.Recurrence.isWeekly;
                    recur.isWeeklyFriday = modelItem.Recurrence.isWeeklyFriday;
                    recur.isWeeklyMonday = modelItem.Recurrence.isWeeklyMonday;
                    recur.isWeeklyRecursEveryWeek = modelItem.Recurrence.isWeeklyRecursEveryWeek;
                    recur.isWeeklySaturday = modelItem.Recurrence.isWeeklySaturday;
                    recur.isWeeklySunday = modelItem.Recurrence.isWeeklySunday;
                    recur.isWeeklyThursday = modelItem.Recurrence.isWeeklyThursday;
                    recur.isWeeklyTuesday = modelItem.Recurrence.isWeeklyTuesday;
                    recur.isWeeklyWednesday = modelItem.Recurrence.isWeeklyWednesday;
                    recur.RecurrencyMode = modelItem.Recurrence.RecurrencyMode;
                    recur.StartTime = modelItem.Recurrence.StartTime;
                    recur.EndTime = modelItem.Recurrence.EndTime;
                    recur.RangeOfReccStartDate = modelItem.Recurrence.RangeOfReccStartDate;// DateTime.Today.AddHours(modelItem.TpmStartDateTime.Hour).AddMinutes(modelItem.TpmStartDateTime.Minute);
                    recur.RangeOfReccEndDate = modelItem.Recurrence.RangeOfReccEndDate;// DateTime.Today.AddHours(modelItem.TpmEndDatetime.Hour).AddMinutes(modelItem.TpmEndDatetime.Minute);

                    item.Recurrence = recur;

                    detailView.Model.SelectedItem = item;

                    detailView.Model.EditingMode = Common.EditMode.New;

                    detailView.Model.IsRecurrencyUpdating = modelItem.ID_Recurrence;



                    TpmPlanDetailWindow.ShowDialog();
                }
            }
        }

        private void AddNewItem()
        {
            TpmPlan item = new TpmPlan();

            if (item != null)
            {
                if (TpmPlanDetailWindow == null)
                {
                    TpmPlanDetailWindow = new RadWindow();

                    TpmPlanDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    TpmPlanDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(TpmPlanDetailWindow_Closed);
                    TpmPlanDetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("tm_TpmNewPlan");
                    TpmPlanDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    TpmPlanDetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    detailView = new TpmPlanDetailView();
                    detailView.RaiseWindow = TpmPlanDetailWindow;
                    TpmPlanDetailWindow.Content = detailView;

                    StyleManager.SetTheme(TpmPlanDetailWindow, new Expression_DarkTheme());
                }

                item.TimePlanMode = 1;

                item.TpmStartDateTime = DateTime.Now.Date;
                item.TpmEndDatetime = DateTime.Now.Date;

                detailView.Model.SelectedItem = item;
                detailView.Model.EditingMode = Common.EditMode.New;
                detailView.Model.IsRecurrencyUpdating = null;

                TpmPlanDetailWindow.ShowDialog();
            }

        }



        void TpmPlanDetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                Model.RefreshCommand.Execute(null);
                grvEquips.Rebind();
            }
            else
            {
                COSContext.Current.RejectChanges();
            }
        }

        RadWindow TpmPlanDetailWindow = null;
        TpmPlanDetailView detailView = null;

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Model != null)
            {
                Model.TrackIsChecked = false;
                foreach (var itm in Model.LocalEquipments)
                {
                    itm.IsChecked = true;
                }
                Model.TrackIsChecked = true;
                Model.RefreshCommand.Execute(null);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Model != null)
            {
                Model.TrackIsChecked = false;
                foreach (var itm in Model.LocalEquipments)
                {
                    itm.IsChecked = false;
                }
                Model.TrackIsChecked = true;
                Model.RefreshCommand.Execute(null);
            }
        }

        private void radScheduleView1_ShowDialog(object sender, ShowDialogEventArgs e)
        {
            e.Cancel = true;

            TpmPlan item = Model.SelectedItem as TpmPlan;

            if (item != null)
            {
                if (TpmPlanDetailWindow == null)
                {
                    TpmPlanDetailWindow = new RadWindow();

                    TpmPlanDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    TpmPlanDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(TpmPlanDetailWindow_Closed);
                    TpmPlanDetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("tm_TpmPlan");
                    TpmPlanDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    TpmPlanDetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    detailView = new TpmPlanDetailView();
                    detailView.RaiseWindow = TpmPlanDetailWindow;
                    TpmPlanDetailWindow.Content = detailView;

                    StyleManager.SetTheme(TpmPlanDetailWindow, new Expression_DarkTheme());
                }

                detailView.Model.SelectedItem = item;
                detailView.Model.EditingMode = Common.EditMode.Edit;

                detailView.Model.IsRecurrencyUpdating = null;
                TpmPlanDetailWindow.ShowDialog();
            }

        }

        private void RadGridView_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            Equipment eqp = e.DataElement as Equipment;

            if (eqp != null)
            {
                if (eqp.Plans.Where(a => a.TpmStartDateTime >= Model.SelectedDate).Count() > 0)
                {
                    e.Row.Background = Brushes.Green;
                }
                else
                {
                    e.Row.Background = Brushes.Transparent;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (leftColumn.ActualWidth != 320)
            {
                leftColumn.Width = new GridLength(320);
                btn.Content = ">";
            }
            else
            {
                leftColumn.Width = new GridLength(570);
                btn.Content = "<";
            }
        }

        private void radScheduleView1_AppointmentCreating(object sender, AppointmentCreatingEventArgs e)
        {
            e.Cancel = true;
        }

        private void radScheduleView1_AppointmentDeleting(object sender, AppointmentDeletingEventArgs e)
        {
            e.Cancel = true;

           
        }
    }
}
