using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Transactions;

namespace COS.Application.HumanResources.Models
{
    public partial class EmployeeDetailViewModel : ValidationViewModelBase
    {
        public EmployeeDetailViewModel()
            : base()
        {
            //SelectedUser = user;
        }

        private Employee _selectedItem = null;
        public Employee SelectedItem
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




        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
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
            SelectedItem .Shift  = null;
        }

        public ICommand ClearBonusGroupCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearBonusGroup());
            }
        }

        private void ClearBonusGroup()
        {
            SelectedItem.BonusGroup_ID = null;
        }


        public ICommand ClearWorkPositionCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearWorkPosition());
            }
        }

        private void ClearWorkPosition()
        {
            SelectedItem.WorkPosition_ID = null;
        }

        public void Save()
        {

            string customErrors = "";

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
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

                OnPropertyChanged("Save");
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }

     
        public void Cancel()
        {
            //COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }      


    }

   
}
