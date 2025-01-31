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

namespace COS.Application.Production.Models
{
    public partial class AdsCustomerServiceDetailViewModel : ValidationViewModelBase
    {
        public AdsCustomerServiceDetailViewModel()
            : base()
        {

        }

        private AdsCustomerService _selectedItem = null;

        public AdsCustomerService SelectedItem
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
                        COSContext.Current.AdsCustomerServices.AddObject(SelectedItem);
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


        public void Cancel()
        {
            //COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        public string IsValid()
        {
            string err = "";

            if (SelectedItem.Year < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000011");
                err += Environment.NewLine;
            }

            if (SelectedItem.Week < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000012");
                err += Environment.NewLine;
            }

            if (SelectedItem.Orders < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000013");
                err += Environment.NewLine;
            }

            if (SelectedItem.OrderInTime < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000014");
                err += Environment.NewLine;
            }

            if (SelectedItem.OrdersService < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000015");
                err += Environment.NewLine;
            }

            if (SelectedItem.Lines < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000016");
                err += Environment.NewLine;
            }

            if (SelectedItem.LinesOnTime < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000017");
                err += Environment.NewLine;
            }

            if (SelectedItem.LineService < 1)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000018");
                err += Environment.NewLine;
            }

            return err;
        }
    }
}
