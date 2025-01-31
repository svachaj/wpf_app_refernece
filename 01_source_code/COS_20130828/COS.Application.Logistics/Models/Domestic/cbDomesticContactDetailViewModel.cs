using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
//using System.ComponentModel.DataAnnotations;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Transactions;
using System.Collections.ObjectModel;

namespace COS.Application.Logistics.Models.Domestic
{
    public partial class cbDomesticContactDetailViewModel : ValidationViewModelBase
    {
        public cbDomesticContactDetailViewModel(DomesticCustomer customer, COSContext datacontext)
            : base()
        {
            Customer = customer;
            LocalContactTypes = new ObservableCollection<DomesticContactType>();
            foreach (var itm in datacontext.DomesticContactTypes)
                LocalContactTypes.Add(itm);
        }

        public DomesticCustomer Customer { set; get; }

        private DomesticCustomerContact _selectedItem = null;
        public DomesticCustomerContact SelectedItem
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

        public ObservableCollection<DomesticContactType> LocalContactTypes { set; get; }

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
                        //COSContext.Current.DomesticCustomerContacts.AddObject(SelectedItem);
                        Customer.Contacts.Add(SelectedItem);
                    }
                    else
                    {

                    }
                    //using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    //{
                    //    try
                    //    {
                    //        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    //        scope.Complete();
                    //    }
                    //    catch (Exception exc)
                    //    {
                    //        Logging.LogException(exc, LogType.ToFileAndEmail);
                    //        scope.Dispose();
                    //        COSContext.Current.RejectChanges();

                    //        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    //    }

                    //}

                    OnPropertyChanged("Save");
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
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


            if (string.IsNullOrEmpty(SelectedItem.Name))
            {
                err += ResourceHelper.GetResource<string>("m_Body_ADM00000005");
                err += Environment.NewLine;
            }

            if (string.IsNullOrEmpty(SelectedItem.Surname))
            {
                err += ResourceHelper.GetResource<string>("m_Body_ADM00000006");
                err += Environment.NewLine;
            }

            if (SelectedItem.ContactType == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_ADM00000006");
                err += Environment.NewLine;
            }

            return err;
        }
    }
}
