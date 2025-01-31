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
using System.Collections.ObjectModel;
using Telerik.Windows.Controls.ScheduleView;

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class TpmPlanViewModel : ValidationViewModelBase
    {
        public TpmPlanViewModel()
            : base()
        {
            LoadEquipments(COSContext.Current.Equipments.ToList());
            DateTime tocompare = SelectedDate.AddDays(-SelectedDate.Day);

            RefreshData();
        }

        private TpmPlan _selectedItem = null;
        public TpmPlan SelectedItem
        {
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        private Appointment _selectedApp = null;
        public Appointment SelectedApp
        {
            set
            {
                _selectedApp = value;
                if (_selectedApp != null)
                    SelectedItem = LocalPlans.FirstOrDefault(a => a.ID == int.Parse(_selectedApp.UniqueId));
                else
                    SelectedItem = null;
                OnPropertyChanged("SelectedApp");
            }
            get
            {
                return _selectedApp;
            }
        }

        private DateTime _SelectedDate = COSContext.Current.DateTimeServer.Date;
        public DateTime SelectedDate
        {
            set
            {
                _SelectedDate = value;
                OnPropertyChanged("SelectedDate");
                RefreshData();
            }
            get
            {
                return _SelectedDate;
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

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(param => this.RefreshData());
            }
        }

        private void RefreshData()
        {
            DateTime fromCompare = SelectedDate.AddDays(-SelectedDate.Day).AddDays(-10);

            DateTime toCompare = fromCompare.AddMonths(1).AddDays(20);

            var eqs = LocalEquipments.Where(a => a.IsChecked).ToList();
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= fromCompare && a.TpmStartDateTime <= toCompare));
            LoadPlans(COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= fromCompare && a.TpmStartDateTime <= toCompare).ToList().Where(a => eqs.Where(i => i.ID == a.ID_MachineEquipment).Count() > 0).ToList());

            LoadPlansAppointments(CreatePlansAppointments(LocalPlans));
        }




        public ICommand DeleteOneCommand
        {
            get
            {
                return new RelayCommand(param => this.DeleteOne());
            }
        }

        private ObservableCollection<Equipment> _localEquipments = new ObservableCollection<Equipment>();
        public ObservableCollection<Equipment> LocalEquipments
        {
            get
            {
                return _localEquipments;
            }
        }

        public void LoadEquipments(IEnumerable<Equipment> equips)
        {
            LocalEquipments.Clear();
            foreach (var itm in equips)
            {
                itm.IsChecked = true;
                itm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(itm_PropertyChanged);
                LocalEquipments.Add(itm);
            }
        }

        void itm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked" && TrackIsChecked)
            {
                RefreshData();
            }
        }

        public bool TrackIsChecked = true;

        private ObservableCollection<TpmPlan> _LocalPlans = new ObservableCollection<TpmPlan>();
        public ObservableCollection<TpmPlan> LocalPlans
        {
            get
            {
                return _LocalPlans;
            }
        }

        public void LoadPlans(IEnumerable<TpmPlan> plans)
        {
            LocalPlans.Clear();
            foreach (var itm in plans)
                LocalPlans.Add(itm);
        }

        private ObservableCollection<Appointment> _LocalPlansAppointments = new ObservableCollection<Appointment>();
        public ObservableCollection<Appointment> LocalPlansAppointments
        {
            get
            {
                return _LocalPlansAppointments;
            }
        }

        public void LoadPlansAppointments(IEnumerable<Appointment> plansAppointments)
        {
            LocalPlansAppointments.Clear();
            foreach (var itm in plansAppointments)
                LocalPlansAppointments.Add(itm);
        }

        public List<Appointment> CreatePlansAppointments(IEnumerable<TpmPlan> plans)
        {
            List<Appointment> apps = new List<Appointment>();
            Appointment appm = null;
            foreach (var itm in plans)
            {
                appm = new Appointment();
                appm.Subject = itm.Equipment.EquipmentName + " - " + itm.Equipment.EquipmentNumber + "  " + itm.TpmStartDateTime.ToShortTimeString() + " - " + itm.TpmEndDatetime.ToShortTimeString();

                appm.Start = itm.TpmStartDateTime;
                appm.End = itm.TpmEndDatetime;
                appm.UniqueId = itm.ID.ToString();
                apps.Add(appm);
            }

            return apps;
        }


        private void DeleteOne()
        {
            if (SelectedItem != null)
            {
                try
                {
                    COSContext.Current.TpmPlans.DeleteObject(SelectedItem);
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                    OnPropertyChanged("Deleted");
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_TM00000020"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (Window)COSContext.Current.RadMainWindow });
                }

            }
        }


        public ICommand DeleteRecurCommand
        {
            get
            {
                return new RelayCommand(param => this.DeleteRecur());
            }
        }

        public ICommand UpdateRecurCommand
        {
            get
            {
                return new RelayCommand(param => this.UpdateRecur());
            }
        }

        private void UpdateRecur()
        {
            OnPropertyChanged("UpdateRecurrency");
        }


        private void DeleteRecur()
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.Recurrence != null)
                {
                    try
                    {
                        var recurence = SelectedItem.Recurrence;

                        foreach (var itm in COSContext.Current.TpmPlans.Where(a => a.ID_RecurrenceID == SelectedItem.ID_RecurrenceID))
                        {
                            COSContext.Current.TpmPlans.DeleteObject(itm);
                        }

                        COSContext.Current.TpmRecurrencePatterns.DeleteObject(recurence);

                        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                        OnPropertyChanged("Deleted");
                    }
                    catch (Exception exc)
                    {
                        Logging.LogException(exc, LogType.ToFileAndEmail);
                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_TM00000020"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (Window)COSContext.Current.RadMainWindow });
                    }
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_TM00000021"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
                }

            }
        }



    }
}
