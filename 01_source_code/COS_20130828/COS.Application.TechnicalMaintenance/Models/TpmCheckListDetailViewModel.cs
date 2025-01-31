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

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class TpmCheckListDetailViewModel : ValidationViewModelBase
    {
        public TpmCheckListDetailViewModel()
            : base()
        {

        }

        private TpmCheckList _selectedItem = null;

        public TpmCheckList SelectedItem
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


        public void Save()
        {

            string customErrors = "";

            customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {
                if (SelectedItem.ID == 0)
                {
                    COSContext.Current.TpmCheckLists.AddObject(SelectedItem);
                }
                else
                {

                }
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


        public List<TpmCheckList> LocalCheckLists
        {
            get
            {
                return COSContext.Current.TpmCheckLists.Where(a => a.IsPreImage == true).ToList();
            }
        }

        public void Cancel()
        {
            //COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        public string IsValid()
        {
            string err = "";

            if (string.IsNullOrEmpty(SelectedItem.CheckListName))
            {
                err += ResourceHelper.GetResource<string>("m_Body_TM00000024");
                err += Environment.NewLine;
            }

            if (SelectedItem.TpmPlannedTime < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_TM00000025");
                err += Environment.NewLine;
            }

            if (SelectedItem.TpmPlannedlabour < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_TM00000026");
                err += Environment.NewLine;
            }

         
            return err;
        }
    }
}
