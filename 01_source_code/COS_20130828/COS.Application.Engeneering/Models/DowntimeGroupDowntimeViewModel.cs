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
using System.Transactions;

namespace COS.Application.Engeneering.Models
{
    public partial class DowntimeGroupDowntimeViewModel : ValidationViewModelBase
    {
        public DowntimeGroupDowntimeViewModel()
            : base()
        {

        }

        public List<DowntimeGroup> DowntimeGroups
        {
            get
            {
                if (SelectedDivision != null)
                    return COSContext.Current.DowntimeGroups.Where(a => a.ID_Division == SelectedDivision.ID).ToList();
                else
                    return null;
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

        private DowntimeGroup _selectedDowntimeGroup = null;
        public DowntimeGroup SelectedDowntimeGroup
        {
            set
            {
                _selectedDowntimeGroup = value;
                OnPropertyChanged("SelectedDowntimeGroup");
            }
            get
            {
                return _selectedDowntimeGroup;
            }
        }

        private Downtime _selectedDowntime = null;
        public Downtime SelectedDowntime
        {
            set
            {
                _selectedDowntime = value;
                OnPropertyChanged("SelectedDowntime");
            }
            get
            {
                return _selectedDowntime;
            }
        }

        public List<Downtime> ItemsToAdd
        {
            get
            {

                var wcs = from wc in  COSContext.Current.Downtimes 
                          join wgwc in COSContext.Current.DowntimeGroupsDowntimes on wc.ID equals wgwc.ID_Downtime
                          select wc;

                return COSContext.Current.Downtimes.Except(wcs).ToList(); //    .Distinct.Where(a => (COSContext.Current.WorkGroupsWorkCenters.Where(w => w.ID_WorkCenter == a.ID)).Count() == 0).ToList();
            }
        }

        public List<DowntimeGroupsDowntime> SelectedItems
        {
            get
            {
                return COSContext.Current.DowntimeGroupsDowntimes.Where(a => a.ID_Division == SelectedDivision.ID && a.ID_DowntimeGroup == SelectedDowntimeGroup.ID).ToList();
            }
        }

        private DowntimeGroupsDowntime _selectedItem = null;
        public DowntimeGroupsDowntime SelectedItem
        {
            get 
            {
                return _selectedItem;
            }
            set 
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNew());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }

        public void Delete()
        {
            if (SelectedItem != null) 
            {
                COSContext.Current.DowntimeGroupsDowntimes.DeleteObject(SelectedItem);

                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    OnPropertyChanged("Delete");
                }
                catch (Exception exc)
                {

                    OnPropertyChanged("Error");
                    //Telerik.Windows.Controls.RadWindow.Alert(new Telerik.Windows.Controls.DialogParameters() { Content="Chyba při ukládání", });
                }
            }

            
        }

        public void AddNew()
        {
            DowntimeGroupsDowntime wgwc = new DowntimeGroupsDowntime();

            wgwc.ID_DowntimeGroup = SelectedDowntimeGroup.ID;
            wgwc.ID_Division = SelectedDivision.ID;
            wgwc.ID_Downtime = SelectedDowntime.ID;

            COSContext.Current.DowntimeGroupsDowntimes.AddObject(wgwc);

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    try
                    {
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

                OnPropertyChanged("AddNew");
            }
            catch (Exception exc)
            {
                
                OnPropertyChanged("Error");
                //Telerik.Windows.Controls.RadWindow.Alert(new Telerik.Windows.Controls.DialogParameters() { Content="Chyba při ukládání", });
            }


        }
    }
}
