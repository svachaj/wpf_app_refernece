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
using System.IO;
using System.Windows;
using System.Globalization;
using System.Transactions;
using System.Configuration;

namespace COS.Application.Logistics.Models
{
    public partial class DomesticExportDetailViewModel : ValidationViewModelBase
    {
        public DomesticExportDetailViewModel()
            : base()
        {
            credUserName = ConfigurationManager.AppSettings["ProxyCredUserName"];
            var pwd = ConfigurationManager.AppSettings["ProxyCredPassword"];
            if (!string.IsNullOrEmpty(pwd))
                credPassword = Crypto.DecryptString(pwd, Security.SecurityHelper.SecurityKey);
            credProxy = ConfigurationManager.AppSettings["ProxyCredUrl"];
            credDomain = ConfigurationManager.AppSettings["ProxyCredDomain"];

            RefreshData();
        }

        public void RefreshData()
        {
            LocalCountries = COSContext.Current.Countries.ToList().OrderBy(a => a.Description).ToList();
            LocalZoneLogistics = COSContext.Current.DomesticDestinations.Where(a => a.IsPointOfOrigin == true).OrderBy(a => a.DestinationName).ToList();
            LocalForwarders = COSContext.Current.DomesticForwarders.OrderBy(a => a.Name).ToList();
            LocalDrivers = COSContext.Current.DomesticDrivers.OrderBy(a => a.Name).ToList();
            LocalCarTypes = COSContext.Current.DomesticCarTypes.OrderBy(a => a.CarTypeName).ToList();
            LocalCompositions = COSContext.Current.DomesticCompositions.ToList().OrderBy(a => a.Description).ToList();
            foreach (var itm in COSContext.Current.DomesticCustomers.OrderBy(a => a.CustomerName).ToList())
                LocalCustomers.Add(itm);

            foreach (var itm in COSContext.Current.DomesticDestinations.OrderBy(a => a.DestinationName).ToList())
                LocalDetailDestinations.Add(itm);
            LocalCostCenters = COSContext.Current.CostCenters.ToList();

        }

        private DomesticExport _selectedItem = null;

        private decimal bafPercent = 0;

        public DomesticExport SelectedItem
        {
            set
            {

                _selectedItem = value;

                if (_selectedItem != null)
                {
                    _selectedItem.PropertyChanged -= _selectedItem_PropertyChanged;

                    if (_selectedDetailItem != null)
                        _selectedDetailItem.PropertyChanged -= _selectedDetailItem_PropertyChanged;


                    var bafp = COSContext.Current.DomescticBafPrices.OrderByDescending(a => a.ValidFrom).FirstOrDefault(a => a.ValidFrom <= _selectedItem.PlannedDate);

                    if (bafp != null)
                        bafPercent = bafp.Percent;

                    if (_selectedItem != null)
                    {
                        _selectedItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedItem_PropertyChanged);
                    }


                    if (value != null && value.ExportDetails.Count > 0)
                        SelectedDetailItem = value.ExportDetails.FirstOrDefault();
                    else
                        SelectedDetailItem = null;

                }

                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        void _selectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ID_carType")
            {
                RecalculateAllForwarderPrices(false);
                foreach (var itm in SelectedItem.ExportDetails)
                {
                    RecalculateDetailForwarderPrice(itm);
                    RecalculateDetailPrice(itm);
                }
                RecalculateTotalPrice();
            }
            else if (e.PropertyName == "ID_forwarder")
            {

                RecalculateAllForwarderPrices(false);
                foreach (var itm in SelectedItem.ExportDetails)
                {
                    RecalculateDetailForwarderPrice(itm);
                    RecalculateDetailPrice(itm);
                }
                RecalculateTotalPrice();

                if (SelectedItem.Forwarder != null)
                {
                    if (SelectedItem.Forwarder.Name == "Osobní")
                    {
                        var drv = COSContext.Current.DomesticDrivers.FirstOrDefault(a => a.Name == "Osobní");
                        var cart = COSContext.Current.DomesticCarTypes.FirstOrDefault(a => a.CarTypeName == "Osobní");


                        SelectedItem.Driver = drv;
                        SelectedItem.CarType = cart;

                        if (SelectedDetailItem != null)
                        {
                            SelectedDetailItem.Destination = SelectedItem.PointOfOrigin;
                        }
                    }
                }
                SelectedItem.RefreshBinding("Driver");
                SelectedItem.RefreshBinding("CarType");

            }
            else if (e.PropertyName == "PlannedDate")
            {
                var bafp = COSContext.Current.DomescticBafPrices.OrderByDescending(a => a.ValidFrom).FirstOrDefault(a => a.ValidFrom <= _selectedItem.PlannedDate);

                if (bafp != null)
                    bafPercent = bafp.Percent;

                RecalculateAllForwarderPrices(false);
                foreach (var itm in SelectedItem.ExportDetails)
                {
                    RecalculateDetailForwarderPrice(itm);
                    RecalculateDetailPrice(itm);
                }
                RecalculateTotalPrice();
            }
        }

        void _selectedDetailItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ForwarderPrice" || e.PropertyName == "Discount")
            {

                if (!SelectedDetailItem.IsBafPriceChanged)
                {
                    if (SelectedItem.Forwarder != null && SelectedItem.Forwarder.CanRecalculateBafPrice)
                        SelectedDetailItem.BafPrice = Math.Round((bafPercent / 100) * SelectedDetailItem.ForwarderPrice, 0);
                }

                RecalculateDetailPrice(SelectedDetailItem);
                RecalculateTotalPrice();
            }
            else if (e.PropertyName == "BafPrice")
            {

                RecalculateDetailPrice(SelectedDetailItem);
                RecalculateTotalPrice();
            }
            else if (e.PropertyName == "Distance")
            {
                //RecalculateAllForwarderPrices();
                RecalculateDetailForwarderPrice(SelectedDetailItem);
                RecalculateTotalDistance();
            }
            else if (e.PropertyName == "VolumeCBM")
            {
                RecalculateTotalVolume();
            }
            else if (e.PropertyName == "ID_destination")
            {
                OnPropertyChanged("DetailsRefresh");
                OnPropertyChanged("SelectedDetailItem");
            }
            else if (e.PropertyName == "ID_customer" || e.PropertyName == "ID_destination")
            {
                RefreshContantsToAdd();
                OnPropertyChanged("DetailsRefresh");
            }
        }


        public void RecalculateDetailPrice(DomesticExportDetail detail)
        {
            if (detail != null)
            {
                if (detail.Discount.HasValue)
                    detail.TotalPrice = (detail.ForwarderPrice + detail.BafPrice) * ((100 - detail.Discount.Value) / 100);
                else
                    detail.TotalPrice = (detail.ForwarderPrice + detail.BafPrice);
            }
        }

        public void RecalculateDetailForwarderPrice(DomesticExportDetail detail)
        {
            if (detail != null)
            {
                if (!detail.IsForwarderPriceChanged)
                {
                    var detailsDist = SelectedItem.ExportDetails.Sum(a => a.Distance);
                    if (detailsDist > 0)
                    {
                        var backDist = SelectedItem.TotalDistance - detailsDist;
                        var koefDist = backDist / detailsDist;
                        var plist = COSContext.Current.DomesticPriceLists.FirstOrDefault(a => a.ID_carType == SelectedItem.ID_carType && a.ID_forwarder == SelectedItem.ID_forwarder);


                        if (plist != null)
                        {
                            decimal tPrice = (SelectedItem.TotalDistance * plist.PricePerKm) * (1 + (1 - ((100 - SelectedItem.Forwarder.DeviationPercent) / 100)));
                            detail.ForwarderPrice = Math.Round(tPrice / SelectedItem.ExportDetails.Count, 2);

                        }
                        else
                        {
                            detail.ForwarderPrice = 0;

                        }
                        // detail.ForwarderPrice = Math.Round((detail.Distance + (koefDist * detail.Distance)) * plist.PricePerKm, 2);
                    }
                }
            }
        }

        public void RecalculateTotalVolume()
        {
            if (SelectedItem != null)
                SelectedItem.TotalVolume = SelectedItem.ExportDetails.Sum(a => a.VolumeCBM);
        }

        public void RecalculateTotalPrice()
        {
            if (SelectedItem != null)
            {
                //var plist = COSContext.Current.DomesticPriceLists.FirstOrDefault(a => a.ID_carType == SelectedItem.ID_carType && a.ID_forwarder == SelectedItem.ID_forwarder);
                //decimal tPrice = 0;

                //if (plist != null)
                //{
                //    decimal backDistance = SelectedItem.TotalDistance - SelectedItem.ExportDetails.Sum(a => a.Distance);

                //    tPrice = (backDistance * plist.PricePerKm) * (1 + (1 - ((100 - SelectedItem.Forwarder.DeviationPercent) / 100)));
                //}

                SelectedItem.TotalPrice = SelectedItem.ExportDetails.Sum(a => a.TotalPrice);// + tPrice;
            }
        }

        public void RecalculateTotalDistance()
        {
            if (SelectedItem != null)
            {
                SelectedItem.TotalDistance = SelectedItem.ExportDetails.Sum(a => a.Distance) + SelectedItem.ReturnDistance;
            }
        }


        public string credUserName = "";
        public string credPassword = "";
        public string credDomain = "";
        public string credProxy = "";


        public void RecalculateAllDistances()
        {
            if (SelectedItem != null && SelectedItem.PointOfOrigin != null && SelectedItem.ExportDetails.Count > 0)
            {
                var origin = SelectedItem.PointOfOrigin.DistanceComputeString;

                var destinations = SelectedItem.ExportDetails.Where(a => a.Destination != null).OrderBy(a => a.DestinationOrder).Select(a => a.Destination.DistanceComputeString).ToList();
                destinations.Add(origin);

                try
                {
                    var distances = COS.Direction.DistanceClass.GetDirection(origin, destinations, this.credUserName, this.credPassword, this.credDomain, this.credProxy);

                    foreach (var itm in SelectedItem.ExportDetails.Where(a => a.Destination != null))
                    {
                        if (!itm.IsDistanceChanged)
                        {
                            itm.Distance = distances[itm.DestinationOrder - 1].DistanceKM;
                        }
                    }

                    SelectedItem.ReturnDistance = distances.Last().DistanceKM;

                    OnPropertyChanged("ReloadMap");
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                    RaiseErrors = "Chyba ve volání google API. Zadejte prosím vzdálenosti ručně nebo zksute akci opakovat.";
                }
            }
        }

        public bool canSave = true;

        public void RecalculateAllForwarderPrices(bool showerr)
        {
            decimal tDist = SelectedItem.TotalDistance;


            var plist = COSContext.Current.DomesticPriceLists.FirstOrDefault(a => a.ID_carType == SelectedItem.ID_carType && a.ID_forwarder == SelectedItem.ID_forwarder);



            if (plist != null)
            {
                canSave = true;

                decimal tPrice = (SelectedItem.TotalDistance * plist.PricePerKm) * (1 + (1 - ((100 - SelectedItem.Forwarder.DeviationPercent) / 100)));

                var detailsDist = SelectedItem.ExportDetails.Sum(a => a.Distance);
                if (detailsDist > 0)
                {
                    var backDist = SelectedItem.TotalDistance - detailsDist;
                    var koefDist = backDist / detailsDist;

                    foreach (var itm in SelectedItem.ExportDetails.Where(a => a.Destination != null))
                    {

                        if (!itm.IsForwarderPriceChanged)
                        {
                            try
                            {
                                var addDist = itm.Distance * koefDist;
                                //itm.ForwarderPrice = Math.Round((itm.Distance + addDist) / tDist * tPrice, 2);
                                itm.ForwarderPrice = Math.Round(tPrice / SelectedItem.ExportDetails.Count, 2);
                            }
                            catch { }
                            RecalculateDetailPrice(itm);
                        }

                        if (!itm.IsBafPriceChanged)
                        {
                            if (SelectedItem.Forwarder != null && SelectedItem.Forwarder.CanRecalculateBafPrice)
                                itm.BafPrice = Math.Round((bafPercent / 100) * itm.ForwarderPrice, 0);
                        }
                    }
                }

                if (plist == null)
                {
                    foreach (var itm in SelectedItem.ExportDetails.Where(a => a.Destination != null))
                    {
                        if (!itm.IsBafPriceChanged)
                        {
                            if (SelectedItem.Forwarder != null && SelectedItem.Forwarder.CanRecalculateBafPrice)
                                itm.BafPrice = Math.Round((bafPercent / 100) * itm.ForwarderPrice, 0);
                        }

                        if (!itm.IsForwarderPriceChanged)
                            itm.ForwarderPrice = 0;
                    }
                }

            }
            else if (showerr)
            {
                canSave = false;
                RaiseConfirm = ResourceHelper.GetResource<string>("m_Body_LOG00000017");
            }
        }

        private DomesticExportDetail _selectedDetailItem = null;

        public DomesticExportDetail SelectedDetailItem
        {
            set
            {
                _selectedDetailItem = value;
                if (_selectedDetailItem != null)
                {
                    RefreshContantsToAdd();
                    _selectedDetailItem.PropertyChanged -= _selectedDetailItem_PropertyChanged;
                    _selectedDetailItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedDetailItem_PropertyChanged);
                }
                OnPropertyChanged("SelectedDetailItem");
            }
            get
            {
                return _selectedDetailItem;
            }
        }

        public void AddDetail()
        {
            if (SelectedItem != null)
            {
                DomesticExportDetail det = new DomesticExportDetail();

                det.DestinationOrder = SelectedItem.ExportDetails.Max(a => a.DestinationOrder) + 1;

                SelectedItem.ExportDetails.Add(det);

                SelectedDetailItem = det;
            }
        }

        public void RemoveDetail(DomesticExportDetail detail)
        {
            if (SelectedItem != null && detail != null)
            {
                if (SelectedItem.ExportDetails.Count == 1)
                {
                    RaiseErrors = ResourceHelper.GetResource<string>("m_Body_LOG00000018");
                }
                else
                {
                    SelectedItem.ExportDetails.Remove(detail);
                    COSContext.Current.DomesticExportDetails.DeleteObject(detail);
                    SelectedDetailItem = SelectedItem.ExportDetails.FirstOrDefault();
                }
            }
        }

        //up 1, down 2
        public void ChangeDetailOrder(DomesticExportDetail detail, int updown)
        {
            if (detail != null)
            {
                if (updown == 1)
                {
                    var updet = SelectedItem.ExportDetails.FirstOrDefault(a => a.DestinationOrder == detail.DestinationOrder + 1);

                    if (updet != null)
                    {
                        detail.DestinationOrder = detail.DestinationOrder + 1;
                        updet.DestinationOrder = updet.DestinationOrder - 1;
                    }
                }
                else
                {
                    var dwdet = SelectedItem.ExportDetails.FirstOrDefault(a => a.DestinationOrder == detail.DestinationOrder - 1);

                    if (dwdet != null)
                    {
                        detail.DestinationOrder = detail.DestinationOrder - 1;
                        dwdet.DestinationOrder = dwdet.DestinationOrder + 1;
                    }
                }
            }
        }

        public List<DomesticExportDetail> Details
        {
            get
            {
                if (SelectedItem != null)
                    return SelectedItem.ExportDetails.OrderBy(a => a.DestinationOrder).ToList();
                else
                    return null;
            }
        }


        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save(false));
            }
        }

        public ICommand RecalculateCommand
        {
            get
            {
                return new RelayCommand(param => this.RecalculateWithAPI(true));
            }
        }

        public void RecalculateWithAPI(bool showErr)
        {
            RecalculateAllDistances();
            RecalculateTotalDistance();
            RecalculateAllForwarderPrices(false);
            foreach (var itm in SelectedItem.ExportDetails)
            {
                RecalculateDetailForwarderPrice(itm);
                RecalculateDetailPrice(itm);
            }
            RecalculateTotalPrice();
        }

        public void RecalculateWithoutAPI(bool showErr)
        {          
            RecalculateTotalDistance();
            RecalculateAllForwarderPrices(false);
            foreach (var itm in SelectedItem.ExportDetails)
            {
                RecalculateDetailForwarderPrice(itm);
                RecalculateDetailPrice(itm);
            }
            RecalculateTotalPrice();
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }


        public void Save(bool forceSave)
        {

            string customErrors = "";

            customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {
                RecalculateWithAPI(!forceSave);

                if (canSave || forceSave)
                {

                    var added = COSContext.Current.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(a => a.Entity).OfType<DomesticExportDetail>().ToList();

                    List<DomesticExportDetail> toDel = new List<DomesticExportDetail>();

                    foreach (var itm in added)
                    {
                        if (!SelectedItem.ExportDetails.Contains(itm))
                            toDel.Add(itm);
                    }

                    foreach (var itm in toDel)
                        COSContext.Current.DomesticExportDetails.DeleteObject(itm);

                    foreach (var itm in COSContext.Current.ObjectStateManager.GetRelationshipManager(SelectedItem).GetAllRelatedEnds())
                    {

                    }

                    foreach (var det in SelectedItem.ExportDetails)
                    {
                        foreach (var soitm in SelectedDetailItem.TrpSos)
                        {
                            if (soitm.SO == null)
                                soitm.SO = "";
                        }
                    }
                    SelectedItem.TotalCountOfDestinations = SelectedItem.ExportDetails.Count;

                    if (SelectedItem.ID == 0)
                    {
                        SelectedItem.CreatedBy = COSContext.Current.CurrentUser;
                        SelectedItem.CreatedDate = COSContext.Current.DateTimeServer;
                        COSContext.Current.DomesticExports.AddObject(SelectedItem);
                    }
                    else
                    {

                    }

                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        try
                        {
                            SelectedItem.UpdatedBy = COSContext.Current.CurrentUser;
                            SelectedItem.UpdatedDate = COSContext.Current.DateTimeServer;

                            //if (SelectedDetailItem.Destination.ID == 0)
                            //    COSContext.Current.DomesticDestinations.AddObject(SelectedDetailItem.Destination);

                            //foreach (var itm in SelectedDetailItem.Customer.Contacts)
                            //{
                            //    if (itm.ID == 0)
                            //        COSContext.Current.DomesticCustomerContacts.AddObject(itm);
                            //}
                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);


                            //COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedDetailItem.Customer).AcceptChanges();
                            //COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedDetailItem.Destination).AcceptChanges();
                            //foreach (var cont in SelectedDetailItem.Customer.Contacts)
                            //{
                            //    COSContext.Current.ObjectStateManager.GetObjectStateEntry(cont).AcceptChanges();
                            //}

                            //foreach (var det in SelectedItem.ExportDetails)
                            //{
                            //    if (det.ID == 0)
                            //        COSContext.Current.ObjectStateManager.GetObjectStateEntry(det).AcceptChanges();
                            //}
                            //COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedItem).AcceptChanges();


                            scope.Complete();
                        }
                        catch (Exception exc)
                        {
                            Logging.LogException(exc, LogType.ToFileAndEmail);
                            scope.Dispose();
                            COSContext.Current.RejectChanges();

                            MessageBox.Show(exc.InnerException != null ? exc.InnerException.Message : exc.Message);

                            RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                        }

                    }
                    //COSContext.Current.RejectChanges();
                    OnPropertyChanged("Save");
                }

            }
            else
            {
                // RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                RaiseErrors = customErrors;
            }



        }


        public void Cancel()
        {
            OnPropertyChanged("Cancel");
        }

        public void CancelChanges()
        {
            if (_selectedDetailItem != null)
                _selectedDetailItem.PropertyChanged -= _selectedDetailItem_PropertyChanged;

            if (_selectedItem != null)
                _selectedItem.PropertyChanged -= _selectedItem_PropertyChanged;

            COSContext.Current.RejectChanges();

            //if (temporaryDetails != null)
            //{
            //    //SelectedItem.Connections.Clear();
            //    foreach (var itm in temporaryDetails)
            //    {
            //        if (itm.EntityState == System.Data.EntityState.Deleted)
            //        {
            //            COSContext.Current.ObjectStateManager.ChangeObjectState(itm, System.Data.EntityState.Unchanged);
            //        }
            //    }
            //    COSContext.Current.AcceptAllChanges();
            //}
        }

        public string IsValid()
        {
            string err = "";

            if (SelectedItem.PointOfOrigin == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000019");
                err += Environment.NewLine;
            }

            if (SelectedItem.Forwarder == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000020");
                err += Environment.NewLine;
            }

            if (SelectedItem.Driver == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000021");
                err += Environment.NewLine;
            }

            if (SelectedItem.CarType == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000022");
                err += Environment.NewLine;
            }


            if (SelectedItem.PlannedDate <= DateTime.MinValue)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000023");
                err += Environment.NewLine;
            }


            foreach (var itm in SelectedItem.ExportDetails)
            {
                if (itm.Customer == null)
                {
                    err += ResourceHelper.GetResource<string>("m_Body_LOG00000024");
                    err += Environment.NewLine;
                }

                if (itm.Destination == null)
                {
                    err += ResourceHelper.GetResource<string>("m_Body_LOG00000025");
                    err += Environment.NewLine;
                }

            }

            if (SelectedItem.ExportDetails.Count() < 1)
            {
                err = ResourceHelper.GetResource<string>("m_Body_LOG00000007");
                err += Environment.NewLine;
            }


            if (SelectedItem.CarType != null)
            {
                var totals = SelectedItem.ExportDetails.Sum(a => a.VolumeCBM);

                if (totals > SelectedItem.CarType.CBM)
                {
                    err = ResourceHelper.GetResource<string>("m_Body_LOG00000016");
                    err += Environment.NewLine;
                }
            }

            return err;
        }

        private List<DomesticDestination> _LocalZoneLogistics = new List<DomesticDestination>();
        public List<DomesticDestination> LocalZoneLogistics
        {
            set
            {
                if (_LocalZoneLogistics != value)
                {
                    _LocalZoneLogistics = value;
                    OnPropertyChanged("LocalZoneLogistics");
                }
            }
            get
            {
                return _LocalZoneLogistics;
            }
        }

        private ObservableCollection<DomesticDestination> _localDetailDestinations = new ObservableCollection<DomesticDestination>();
        public ObservableCollection<DomesticDestination> LocalDetailDestinations
        {
            set
            {
                if (_localDetailDestinations != value)
                {
                    _localDetailDestinations = value;
                    OnPropertyChanged("LocalDetailDestinations");
                }
            }
            get
            {
                return _localDetailDestinations;
            }
        }


        private List<Country> _localCountries = new List<Country>();
        public List<Country> LocalCountries
        {
            set
            {
                if (_localCountries != value)
                {
                    _localCountries = value;
                    OnPropertyChanged("LocalCountries");
                }
            }
            get
            {
                return _localCountries;
            }
        }

        private List<DomesticDriver> _localDrivers = new List<DomesticDriver>();
        public List<DomesticDriver> LocalDrivers
        {
            set
            {
                if (_localDrivers != value)
                {
                    _localDrivers = value;
                    OnPropertyChanged("LocalDrivers");
                }
            }
            get
            {
                return _localDrivers;
            }
        }

        private List<DomesticForwarder> _localForwarders = new List<DomesticForwarder>();
        public List<DomesticForwarder> LocalForwarders
        {
            set
            {
                if (_localForwarders != value)
                {
                    _localForwarders = value;
                    OnPropertyChanged("LocalForwarders");
                }
            }
            get
            {
                return _localForwarders;
            }
        }

        private List<DomesticCarType> _localCarTypes = new List<DomesticCarType>();
        public List<DomesticCarType> LocalCarTypes
        {
            set
            {
                if (_localCarTypes != value)
                {
                    _localCarTypes = value;
                    OnPropertyChanged("LocalCarTypes");
                }
            }
            get
            {
                return _localCarTypes;
            }
        }

        private List<DomesticComposition> _localCompositions = new List<DomesticComposition>();
        public List<DomesticComposition> LocalCompositions
        {
            set
            {
                if (_localCompositions != value)
                {
                    _localCompositions = value;
                    OnPropertyChanged("LocalCompositions");
                }
            }
            get
            {
                return _localCompositions;
            }
        }

        private ObservableCollection<DomesticCustomer> _localCustomers = new ObservableCollection<DomesticCustomer>();
        public ObservableCollection<DomesticCustomer> LocalCustomers
        {
            set
            {
                if (_localCustomers != value)
                {
                    _localCustomers = value;
                    OnPropertyChanged("LocalCustomers");
                }
            }
            get
            {
                return _localCustomers;
            }
        }

        private List<CostCenter> _localCostCenters = new List<CostCenter>();
        public List<CostCenter> LocalCostCenters
        {
            set
            {
                if (_localCostCenters != value)
                {
                    _localCostCenters = value;
                    OnPropertyChanged("LocalCostCenters");
                }
            }
            get
            {
                return _localCostCenters;
            }
        }

        private ObservableCollection<DomesticCustomerContact> _localContacts = new ObservableCollection<DomesticCustomerContact>();
        public ObservableCollection<DomesticCustomerContact> LocalContacts
        {
            set
            {
                if (_localContacts != value)
                {
                    _localContacts = value;
                    OnPropertyChanged("LocalContacts");
                }
            }
            get
            {
                return _localContacts;
            }
        }

        public void RefreshContantsToAdd()
        {
            if (SelectedDetailItem != null && SelectedDetailItem.Customer != null)
            {
                LocalContacts.Clear();

                var avcon = COSContext.Current.DomesticCustomerContacts.Where(a => a.ID_customer == SelectedDetailItem.ID_customer).ToList();

                var ids = SelectedDetailItem.Contacts.Select(a => a.ID_contact).ToList();

                var toadd = avcon.Where(a => !ids.Contains(a.ID));

                foreach (var itm in toadd)
                    LocalContacts.Add(itm);
            }
        }

    }
}
