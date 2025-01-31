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
    public partial class cbDomesticDestinationDetailViewModel : ValidationViewModelBase
    {
        public cbDomesticDestinationDetailViewModel(COSContext datacontext)
            : base()
        {
            dataContext = datacontext;
            ReloadCountries();
        }

        public void ReloadCountries() 
        {
            if (LocalCountries == null)
                LocalCountries = new ObservableCollection<Country>();
            foreach (var itm in dataContext.Countries)
                LocalCountries.Add(itm);
        }
        public ObservableCollection<Country> LocalCountries { set; get; }

        private COSContext dataContext = null;

        private DomesticDestination _selectedItem = null;
        public DomesticDestination SelectedItem
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
        public DomesticDestination CreateNewItem() 
        {
            DomesticDestination dest = dataContext.DomesticDestinations.CreateObject();

            return dest;
        }

        public void Save()
        {

            string customErrors = "";

            customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {

                if (SelectedItem.ID == 0)
                {
                    //COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedItem).ChangeState(System.Data.EntityState.Detached);
                    dataContext.DomesticDestinations.AddObject(SelectedItem);

                }
                else
                {

                }

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    try
                    {
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


        public void Cancel()
        {
            //COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        public string IsValid()
        {
            string err = "";


            //if (string.IsNullOrEmpty(SelectedItem.Value))
            //{
            //    err += ResourceHelper.GetResource<string>("m_Body_ADM00000005");
            //    err += Environment.NewLine;
            //}

            //if (string.IsNullOrEmpty(SelectedItem.ID_RON))
            //{
            //    err += ResourceHelper.GetResource<string>("m_Body_ADM00000006");
            //    err += Environment.NewLine;
            //}

            return err;
        }
    }
}
