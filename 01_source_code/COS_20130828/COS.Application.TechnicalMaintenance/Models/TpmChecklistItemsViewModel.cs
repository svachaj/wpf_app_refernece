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
using System.Transactions;

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class TpmChecklistItemsViewModel : ValidationViewModelBase
    {
        public TpmChecklistItemsViewModel()
            : base()
        {

        }

        public List<TpmCheckList> CheckLists
        {
            get
            {
                return COSContext.Current.TpmCheckLists.Where(a => a.IsPreImage == true).ToList();
            }
        }


        private TpmCheckList _selectedCheckList = null;
        public TpmCheckList SelectedCheckList
        {
            set
            {
                _selectedCheckList = value;
                OnPropertyChanged("SelectedCheckList");
            }
            get
            {
                return _selectedCheckList;
            }
        }

        private TpmCheckListItem _selectedCheckListItem = null;
        public TpmCheckListItem SelectedCheckListItem
        {
            set
            {
                _selectedCheckListItem = value;
                OnPropertyChanged("SelectedCheckListItem");
            }
            get
            {
                return _selectedCheckListItem;
            }
        }

        public List<TpmCheckListItem> ItemsToAdd
        {
            get
            {
                if (SelectedCheckList != null && SelectedCheckList.Items != null)
                    return COSContext.Current.TpmCheckListItems1.ToList().Except(SelectedCheckList.Items.Select(a => a.CheckListItems)).ToList();
                //COSContext.Current.TpmCheckListItems1.Where(a => a.ID != SelectedItem.ID_tpmCheckListItem).ToList(); //    .Distinct.Where(a => (COSContext.Current.WorkGroupsWorkCenters.Where(w => w.ID_WorkCenter == a.ID)).Count() == 0).ToList();
                else
                    return COSContext.Current.TpmCheckListItems1.ToList();
            }
        }

        public List<TpmCheckList_CheckListItem> SelectedItems
        {
            get
            {
                return COSContext.Current.TpmCheckList_CheckListItem.Where(a => a.ID_tpmCheckList == SelectedCheckList.ID).ToList();
            }
        }

        private TpmCheckList_CheckListItem _selectedItem = null;
        public TpmCheckList_CheckListItem SelectedItem
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
                COSContext.Current.TpmCheckList_CheckListItem.DeleteObject(SelectedItem);

                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    OnPropertyChanged("Delete");
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                    OnPropertyChanged("Error");
                    //Telerik.Windows.Controls.RadWindow.Alert(new Telerik.Windows.Controls.DialogParameters() { Content="Chyba při ukládání", });
                }
            }


        }

        public void AddNew()
        {
            TpmCheckList_CheckListItem clci = new TpmCheckList_CheckListItem();

            clci.ID_tpmCheckList = SelectedCheckList.ID;
            clci.ID_tpmCheckListItem = SelectedCheckListItem.ID;

            COSContext.Current.TpmCheckList_CheckListItem.AddObject(clci);

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
                Logging.LogException(exc, LogType.ToFileAndEmail);
                OnPropertyChanged("Error");
                //Telerik.Windows.Controls.RadWindow.Alert(new Telerik.Windows.Controls.DialogParameters() { Content="Chyba při ukládání", });
            }


        }


    }
}
