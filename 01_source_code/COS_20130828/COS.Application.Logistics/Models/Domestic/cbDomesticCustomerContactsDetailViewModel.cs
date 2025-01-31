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
    public partial class cbDomesticCustomerContactsDetailViewModel : ValidationViewModelBase
    {
        public cbDomesticCustomerContactsDetailViewModel(COSContext datacontext)
            : base()
        {
            if (datacontext != null)
                dataContext = datacontext;
            else
            {
                var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                dataContext = new COSContext(decryptString);
            }
            RefreshCountries();
        }

        public COSContext dataContext = null;

       
        private DomesticCustomer _selectedItem = null;
        public DomesticCustomer SelectedItem
        {
            set
            {
                _selectedItem = value;
                ToDelete.Clear();
                ToAdd.Clear();
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

        public ObservableCollection<Country> LocalCountries { set; get; }

        public void RefreshCountries()
        {
            if (LocalCountries == null)
                LocalCountries = new ObservableCollection<Country>();
            else
                LocalCountries.Clear();

            foreach (var itm in dataContext.Countries)
                LocalCountries.Add(itm);
        }

        public void Save()
        {

            string customErrors = "";

            customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {               
                    if (SelectedItem.ID == 0)
                    {
                        try
                        {
                            COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedItem).ChangeState(System.Data.EntityState.Detached);
                        }
                        catch { }
                        dataContext.DomesticCustomers.AddObject(SelectedItem);
                    }
                    else
                    {

                    }

                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        try
                        {

                            foreach (var itm in ToDelete)
                            {
                                dataContext.DomesticCustomerContacts.DeleteObject(itm);
                            }

                            foreach (var itm in ToAdd)
                            {
                                try
                                {
                                    COSContext.Current.ObjectStateManager.GetObjectStateEntry(itm).ChangeState(System.Data.EntityState.Detached);
                                }
                                catch { }
                                dataContext.DomesticCustomerContacts.AddObject(itm);
                            }

                            dataContext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            scope.Complete();
                        }
                        catch (Exception exc)
                        {
                            Logging.LogException(exc, LogType.ToFileAndEmail);
                            scope.Dispose();
                            dataContext.RejectChanges();

                            RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                        }

                    
                }

                OnPropertyChanged("Save");
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }

        }

        public DomesticCustomer CreateNewItem() 
        {
            DomesticCustomer newItem = dataContext.DomesticCustomers.CreateObject();

            return newItem;
        }

        public List<DomesticCustomerContact> ToDelete = new List<DomesticCustomerContact>();
        public List<DomesticCustomerContact> ToAdd = new List<DomesticCustomerContact>();

        public void Cancel()
        {
            COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        public string IsValid()
        {
            string err = "";


            if (SelectedItem.Country == null)
            {
                err += "Země musí být vyplněna!";// ResourceHelper.GetResource<string>("m_Body_ADM00000005");
                err += Environment.NewLine;
            }

            if (string.IsNullOrEmpty(SelectedItem.CustomerName))
            {
                err += "Jméno zákazníka musí být vyplněno!";// ResourceHelper.GetResource<string>("m_Body_ADM00000006");
                err += Environment.NewLine;
            }

            return err;
        }
    }
}
