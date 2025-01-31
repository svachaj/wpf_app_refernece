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
using System.Windows.Media;

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class TpmPlanOverviewViewModel : ValidationViewModelBase
    {
        public TpmPlanOverviewViewModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TpmPlanOverviewViewModel_PropertyChanged);
        }

        void TpmPlanOverviewViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
              if (e.PropertyName == "SelectedWorkGroup")
            {
                if (SelectedWorkGroup != null)
                {
                    LocalWorkCenters = COSContext.Current.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID).Select(a => a.WorkCenter).ToList();
                }
                else 
                {
                    LocalWorkCenters = null;
                }
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

        private List<TpmPlan> _localPlans = null;
        public List<TpmPlan> LocalPlans
        {
            set
            {
                _localPlans = value;
                OnPropertyChanged("LocalPlans");
            }
            get
            {
                return _localPlans;
            }
        }

        private DateTime _selectedDate = DateTime.Now.Date;
        public DateTime SelectedDate
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




        public ICommand AddNewCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNew());
            }
        }

        private void AddNew()
        {
            OnPropertyChanged("AddNewItem");
        }






    }

    public class MarginPlanConverter1 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TpmPlan plan = value as TpmPlan;


            if (plan != null && parameter != null)
            {
                int param = int.Parse(parameter.ToString());
                int hour = plan.TpmStartDateTime.Hour;
                int minutes = plan.TpmStartDateTime.Minute;

                int endHour = plan.TpmEndDatetime.Hour;
                int endMinutes = plan.TpmEndDatetime.Minute;

                double leftMar = -5;
                double rightMar = -3;

                if (hour == param)
                {
                    if (minutes > 0)
                    {
                        leftMar += minutes;
                    }

                    if (endHour == param)
                    {
                        rightMar += 60 - endMinutes;
                    }

                    return new Thickness(leftMar, 0, rightMar, 0);
                }
                else if (endHour == param)
                {
                    if (endHour == param)
                    {
                        rightMar += 60 - endMinutes;
                    }

                    return new Thickness(leftMar, 0, rightMar, 0);
                }
                else if (param > hour && param < endHour)
                {
                    return new Thickness(leftMar, 0, rightMar, 0);
                }
                else
                    return new Thickness(35, 0, 33, 0);
            }
            else
                return new Thickness(35, 0, 33, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class StatusPlanConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            int id = (int)value;

            if (id == 1)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if (id == 2)
            {
                return new SolidColorBrush(Colors.Orange);
            }
            else
                return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
