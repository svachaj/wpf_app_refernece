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
using System.Collections.ObjectModel;

namespace COS.Application.Production.Models
{
    public partial class PlanningVA4HDetailViewModel : ValidationViewModelBase
    {
        public PlanningVA4HDetailViewModel(COSContext datacontext)
            : base()
        {
            dataContext = datacontext;
            LocalDetails = new ObservableCollection<VA4HAccessories>();
            ReloadCodebooks();
        }

        COSContext dataContext;

        private VA4H _selectedItem = null;
        public VA4H SelectedItem
        {
            set
            {
                if (_selectedItem != value)
                {
                    if (_selectedItem != null)
                        _selectedItem.PropertyChanged -= _selectedItem_PropertyChanged;

                    _selectedItem = value;
                    if (_selectedItem != null)
                    {
                        InitDetails();
                        _selectedItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedItem_PropertyChanged);
                    }

                    ReloadCodebooks();
                    OnPropertyChanged("SelectedItem");
                }
            }
            get
            {
                return _selectedItem;
            }
        }

        void _selectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ID_customer")
            {
                OnPropertyChanged("Contacts");
            }
            else if (e.PropertyName == "SOConstructionStartDate" || e.PropertyName == "SOConstructionEndDate")
            {
                if (SelectedItem.SOConstructionStartDate.HasValue && SelectedItem.SOConstructionEndDate.HasValue)
                {
                    var diff = SelectedItem.SOConstructionEndDate - SelectedItem.SOConstructionStartDate;
                    SelectedItem.RealHoursConstruction = diff.HasValue ? (decimal)diff.Value.TotalHours : 0;
                }
            }
            else if (e.PropertyName == "SOExpeditionDeadlineDate")
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.SOExpeditionDeadlineDate.HasValue)
                        SelectedItem.SOProductionDeadlineDate = SelectedItem.SOExpeditionDeadlineDate.Value.AddWorkDay(-1);
                }
            }
            else if (e.PropertyName == "SOProductionDeadlineDate")
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.SOProductionDeadlineDate.HasValue)
                        SelectedItem.SOConstructionDeadlineDate = SelectedItem.SOProductionDeadlineDate.Value.AddWorkDay(-1);
                }
            }
        }

        private void InitDetails()
        {
            LocalDetails.Clear();
            foreach (var itm in _selectedItem.VA4H_accessories)
            {
                LocalDetails.Add(itm);
            }

            SelectedDetailItem = LocalDetails.FirstOrDefault();
        }

        private VA4HAccessories _selectedDetailItem = null;
        public VA4HAccessories SelectedDetailItem
        {
            set
            {
                if (_selectedDetailItem != null)
                    _selectedDetailItem.PropertyChanged -= _selectedDetailItem_PropertyChanged;

                _selectedDetailItem = value;
                if (_selectedDetailItem != null)
                {
                    _selectedDetailItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedDetailItem_PropertyChanged);
                }
                OnPropertyChanged("SelectedDetailItem");
            }
            get
            {
                return _selectedDetailItem;
            }
        }

        void _selectedDetailItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

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

        public void AddDetail()
        {
            VA4HAccessories access = dataContext.VA4HAccessories.CreateObject();

            access.ProductionDeadlineDate = DateTime.Today;

            access.TRP = SelectedItem.TRP;
            access.TransportType = SelectedItem.TransportType;
            access.ExpeditionDeadlineDate = SelectedItem.SOExpeditionDeadlineDate;

            LocalDetails.Add(access);

            SelectedDetailItem = access;
        }

        public void RemoveDetail(VA4HAccessories item)
        {
            //if (dataContext.ObjectStateManager.GetObjectStateEntry(item).State == System.Data.EntityState.Added)
            //    dataContext.ObjectStateManager.ChangeObjectState(item, System.Data.EntityState.Detached);

            LocalDetails.Remove(item);

            if (item.ID > 0)
                dataContext.VA4HAccessories.DeleteObject(item);
        }

        public ObservableCollection<VA4HAccessories> LocalDetails { set; get; }

        public void Save()
        {

            string customErrors = "";

            //customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {

                foreach (var itm in LocalDetails)
                {
                    if (!SelectedItem.VA4H_accessories.Contains(itm))
                        SelectedItem.VA4H_accessories.Add(itm);
                }

                foreach (var itm in SelectedItem.VA4H_accessories)
                {
                    if (!LocalDetails.Contains(itm))
                    {
                        dataContext.DeleteObject(itm);
                        SelectedItem.VA4H_accessories.Remove(itm);
                    }
                }

                if (SelectedItem.ID == 0)
                {
                    dataContext.VA4H.AddObject(SelectedItem);

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

            if (SelectedItem.SO_Number.IsNullOrEmptyString())
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000021");
                err += Environment.NewLine;
            }

            //if (SelectedItem.Supplier == null)
            //{
            //    err += ResourceHelper.GetResource<string>("m_Body_PROD00000022");
            //    err += Environment.NewLine;
            //}

            //if (SelectedItem.Manufacture == null)
            //{
            //    err += ResourceHelper.GetResource<string>("m_Body_PROD00000023");
            //    err += Environment.NewLine;
            //}

            //if (SelectedItem.Customer == null)
            //{
            //    err += ResourceHelper.GetResource<string>("m_Body_PROD00000024");
            //    err += Environment.NewLine;
            //}

            //foreach (var itm in LocalDetails)
            //{
            //    if (itm.WorkGroup == null)
            //    {
            //        err += ResourceHelper.GetResource<string>("m_Body_PROD00000025");
            //        err += Environment.NewLine;
            //    }

            //    if (itm.State == null)
            //    {
            //        err += ResourceHelper.GetResource<string>("m_Body_PROD00000026");
            //        err += Environment.NewLine;
            //    }

            //    if (itm.ProductItem.IsNullOrEmptyString())
            //    {
            //        err += ResourceHelper.GetResource<string>("m_Body_PROD00000027");
            //        err += Environment.NewLine;
            //    }

            //}

            return err;
        }


        public void ReloadCodebooks()
        {
            ReloadLocalConstructers();
            ReloadLocalCustomers();
            ReloadLocalExecutionTypes();
            ReloadLocalManufactures();
            ReloadLocalSuppliers();
            ReloadLocalTransportTypes();
            ReloadLocalWorkCenters();
            ReloadLocalProdPlanStates();
        }

        public ObservableCollection<ProdPlanSupplier> LocalSuppliers { set; get; }
        public void ReloadLocalSuppliers()
        {
            if (LocalSuppliers == null)
                LocalSuppliers = new ObservableCollection<ProdPlanSupplier>();
            else
                LocalSuppliers.Clear();

            foreach (var itm in dataContext.ProdPlanSuppliers)
            {
                LocalSuppliers.Add(itm);
            }
        }

        public ObservableCollection<ProdPlanManufacture> LocalManufactures { set; get; }
        public void ReloadLocalManufactures()
        {
            if (LocalManufactures == null)
                LocalManufactures = new ObservableCollection<ProdPlanManufacture>();
            else
                LocalManufactures.Clear();

            foreach (var itm in dataContext.ProdPlanManufactures)
            {
                LocalManufactures.Add(itm);
            }
        }

        public ObservableCollection<DomesticCustomer> LocalCustomers { set; get; }
        public void ReloadLocalCustomers()
        {
            if (LocalCustomers == null)
                LocalCustomers = new ObservableCollection<DomesticCustomer>();
            else
                LocalCustomers.Clear();

            foreach (var itm in dataContext.DomesticCustomers)
            {
                LocalCustomers.Add(itm);
            }
        }

        public ObservableCollection<User> LocalConstructers { set; get; }
        public void ReloadLocalConstructers()
        {
            if (LocalConstructers == null)
                LocalConstructers = new ObservableCollection<User>();
            else
                LocalConstructers.Clear();

            foreach (var itm in dataContext.Users.ToList().Where(a => a.Employee != null && a.Employee.WorkPosition != null && a.Employee.WorkPosition.Code != null && a.Employee.WorkPosition.Code == "CC1"))
            {
                LocalConstructers.Add(itm);
            }
        }

        public ObservableCollection<ProdPlanTransportType> LocalTransportTypes { set; get; }
        public void ReloadLocalTransportTypes()
        {
            if (LocalTransportTypes == null)
                LocalTransportTypes = new ObservableCollection<ProdPlanTransportType>();
            else
                LocalTransportTypes.Clear();

            foreach (var itm in dataContext.ProdPlanTransportTypes)
            {
                LocalTransportTypes.Add(itm);
            }
        }

        public ObservableCollection<ProdPlanExecutionType> LocalExecutionTypes { set; get; }
        public void ReloadLocalExecutionTypes()
        {
            if (LocalExecutionTypes == null)
                LocalExecutionTypes = new ObservableCollection<ProdPlanExecutionType>();
            else
                LocalExecutionTypes.Clear();

            foreach (var itm in dataContext.ProdPlanExecutionTypes)
            {
                LocalExecutionTypes.Add(itm);
            }
        }

        public ObservableCollection<ProdPlanState> LocalProdPlanStates { set; get; }
        public void ReloadLocalProdPlanStates()
        {
            if (LocalProdPlanStates == null)
                LocalProdPlanStates = new ObservableCollection<ProdPlanState>();
            else
                LocalProdPlanStates.Clear();

            foreach (var itm in dataContext.ProdPlanStates)
            {
                LocalProdPlanStates.Add(itm);
            }
        }

        public ObservableCollection<WorkCenter> LocalWorkCenters { set; get; }
        public void ReloadLocalWorkCenters()
        {
            if (LocalWorkCenters == null)
                LocalWorkCenters = new ObservableCollection<WorkCenter>();
            else
                LocalWorkCenters.Clear();

            foreach (var itm in dataContext.WorkCenters.OrderBy(a => a.Value))
            {
                LocalWorkCenters.Add(itm);
            }
        }

    }
}
