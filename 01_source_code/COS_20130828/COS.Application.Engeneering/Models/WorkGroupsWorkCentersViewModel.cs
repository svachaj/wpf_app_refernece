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
    public partial class WorkGroupsWorkCentersViewModel : ValidationViewModelBase
    {
        public WorkGroupsWorkCentersViewModel()
            : base()
        {

        }

        public List<WorkGroup> WorkGroups
        {
            get
            {
                if (SelectedDivision != null)
                    return COSContext.Current.WorkGroups.Where(a => a.ID_Division == SelectedDivision.ID).ToList();
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

        public List<WorkCenter> ItemsToAdd
        {
            get
            {

                var wcs = from wc in  COSContext.Current.WorkCenters 
                          join wgwc in COSContext.Current.WorkGroupsWorkCenters on wc.ID equals wgwc.ID_WorkCenter
                          select wc;

                return COSContext.Current.WorkCenters.Except(wcs).ToList(); //    .Distinct.Where(a => (COSContext.Current.WorkGroupsWorkCenters.Where(w => w.ID_WorkCenter == a.ID)).Count() == 0).ToList();
            }
        }

        public List<WorkGroupsWorkCenter> SelectedItems
        {
            get
            {
                return COSContext.Current.WorkGroupsWorkCenters.Where(a => a.ID_Division == SelectedDivision.ID && a.ID_WorkGroup == SelectedWorkGroup.ID).ToList();
            }
        }

        private WorkGroupsWorkCenter _selectedItem = null;
        public WorkGroupsWorkCenter SelectedItem
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
                COSContext.Current.WorkGroupsWorkCenters.DeleteObject(SelectedItem);

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
            WorkGroupsWorkCenter wgwc = new WorkGroupsWorkCenter();

            wgwc.ID_WorkGroup = SelectedWorkGroup.ID;
            wgwc.ID_Division = SelectedDivision.ID;
            wgwc.ID_WorkCenter = SelectedWorkCenter.ID;

            COSContext.Current.WorkGroupsWorkCenters.AddObject(wgwc);

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
