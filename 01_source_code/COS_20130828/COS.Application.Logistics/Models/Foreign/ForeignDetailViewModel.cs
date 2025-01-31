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

namespace COS.Application.Logistics.Models
{
    public partial class ForeignDetailViewModel : ValidationViewModelBase
    {
        public ForeignDetailViewModel()
            : base()
        {

            LocalCountries = COSContext.Current.Countries.ToList().OrderBy(a => a.Description).ToList();

        }

        private ForeignExport _selectedItem = null;

        private decimal bafPercent = 0;

        public ForeignExport SelectedItem
        {
            set
            {
                temporaryConnections = new Dictionary<ForeignExportConnection, List<ForeignExportDetail>>();

                _selectedItem = value;

                if (_selectedItem != null)
                {
                    _selectedItem.PropertyChanged -= _selectedItem_PropertyChanged;



                    foreach (var cn in value.Connections)
                    {
                        var dets = cn.ExportDetails.ToList();
                        temporaryConnections.Add(cn, dets);
                    }

                    var bafp = COSContext.Current.BafPrices.OrderByDescending(a => a.ValidFromDate).FirstOrDefault(a => a.ValidFromDate <= _selectedItem.PlannedDate);

                    if (bafp != null)
                        bafPercent = bafp.BafPercent;

                    if (_selectedItem != null)
                        _selectedItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedItem_PropertyChanged);

                    if (_selectedDetailItem != null)
                        _selectedDetailItem.PropertyChanged -= _selectedDetailItem_PropertyChanged;


                    if (value != null && value.Connections.Count > 0)
                        SelectedDetailItem = value.Connections.FirstOrDefault().ExportDetails.FirstOrDefault();
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
            if (e.PropertyName == "ID_Forwarder")
            {
                ControlPriceList();
            }
            else if (e.PropertyName == "ID_Destination")
            {
                CheckSubzonesAndProcess();
                ControlPriceList();
            }
            else if (e.PropertyName == "VolumeCbm")
            {
                //var totals = SelectedItem.ExportDetails.Sum(a => a.VolumeCbm);
                //if (SelectedItem.VolumeCbm < totals)
                //    SelectedItem.VolumeCbm = totals;

                ControlPriceList();

                if (SelectedItem.VolumeCbm == 0)
                {
                    SelectedItem.TollPrice = 0;
                    SelectedItem.TollPriceGerlach = 0;
                    SelectedItem.ForwarderPrice = 0;
                }
            }
            else if (e.PropertyName == "ForwarderPrice")
            {
                if (!SelectedItem.IsBafPriceChanged)
                {
                    if (SelectedItem.Forwarder != null && SelectedItem.Forwarder.CanCalculateBaf)
                        SelectedItem.BafPrice = Math.Round((bafPercent / 100) * SelectedItem.ForwarderPrice, 0);
                }

                SumTotalPrice();
                SumDetailValues();


            }
            else if (e.PropertyName == "BafPrice")
            {
                SumTotalPrice();
                SumDetailValues();

            }
            else if (e.PropertyName == "TollPrice")
            {

                SumTotalPrice();
                SumDetailValues();


            }
            else if (e.PropertyName == "TollPriceGerlach")
            {
                SumTotalPrice();
                SumDetailValues();


            }
            else if (e.PropertyName == "ID_Unit")
            {
                SelectedItem.NoUnit = 1;

                ComputeUnits();

                //if (SelectedItem.Unit != null && string.IsNullOrEmpty(SelectedItem.Unit.UnitName))
                //{
                //    SelectedItem.NoUnit = 0;
                //}
            }
            else if (e.PropertyName == "NoUnit")
            {
                ComputeUnits();
            }
            else if (e.PropertyName == "InvoiceCheck")
            {
                if (SelectedItem.InvoiceCheck)
                    SelectedItem.InvoiceCheckDate = COSContext.Current.DateTimeServer;
                else
                    SelectedItem.InvoiceCheckDate = null;
            }



        }


        Dictionary<ForeignExportConnection, List<ForeignExportDetail>> temporaryConnections = null;

        private void CheckSubzonesAndProcess()
        {
            if (SelectedItem != null)
            {
                var conns = SelectedItem.Connections.ToList();

                foreach (var cn in conns)
                {

                    var dets = cn.ExportDetails.ToList();

                    foreach (var dt in dets)
                    {
                        COSContext.Current.DeleteObject(dt);
                    }
                    COSContext.Current.DeleteObject(cn);
                }


                var subzones = COSContext.Current.ZoneCombinations.Where(a => a.ID_Zone_Parent == SelectedItem.ID_Destination).ToList();

                if (subzones.Count() > 0)
                {
                    foreach (var itm in subzones)
                    {
                        var conn = COSContext.Current.ForeignExportConnections.FirstOrDefault(a => a.ID_ForeignExport == SelectedItem.ID && a.ID_Zone == itm.ID_Zone_Child);

                        if (conn == null || conn.EntityState == System.Data.EntityState.Deleted)
                        {
                            conn = COSContext.Current.ForeignExportConnections.CreateObject();
                            COSContext.Current.ForeignExportConnections.AddObject(conn);

                            var zn = COSContext.Current.ZoneLogistics.FirstOrDefault(a => a.ID == itm.ID_Zone_Child);
                            conn.Destination = zn;

                            ForeignExportDetail detailExp = COSContext.Current.ForeignExportDetails.CreateObject();
                            COSContext.Current.ForeignExportDetails.AddObject(detailExp);
                            conn.ExportDetails.Add(detailExp);
                        }

                        if (!SelectedItem.Connections.Contains(conn))
                            SelectedItem.Connections.Add(conn);
                    }
                }
                else
                {
                    var conn = COSContext.Current.ForeignExportConnections.FirstOrDefault(a => a.ID_ForeignExport == SelectedItem.ID && a.ID_Zone == SelectedItem.ID_Destination);

                    if (conn == null || conn.EntityState == System.Data.EntityState.Deleted)
                    {
                        conn = COSContext.Current.ForeignExportConnections.CreateObject();
                        COSContext.Current.ForeignExportConnections.AddObject(conn);

                        var zn = COSContext.Current.ZoneLogistics.FirstOrDefault(a => a.ID == SelectedItem.ID_Destination);
                        conn.Destination = zn;

                        ForeignExportDetail detailExp = COSContext.Current.ForeignExportDetails.CreateObject();
                        COSContext.Current.ForeignExportDetails.AddObject(detailExp);
                        conn.ExportDetails.Add(detailExp);
                    }

                    if (!SelectedItem.Connections.Contains(conn))
                        SelectedItem.Connections.Add(conn);
                }

                if (SelectedItem.Connections.Count > 0)
                    SelectedDetailItem = SelectedItem.Connections.FirstOrDefault().ExportDetails.FirstOrDefault();
            }
        }

        private void ControlPriceList()
        {
            if (SelectedItem != null && !SelectedItem.IsForwarderPriceChanged)
            {
                PriceList plist = null;
                double volTemp = (double)SelectedItem.VolumeCbm;
                var maxVolume = COSContext.Current.PriceLists.Where(a => a.ID_Forwarder == SelectedItem.ID_Forwarder && a.ZoneCode == SelectedItem.Destination.Code && a.Country.ID == SelectedItem.Destination.Country.ID).Max(a => a.Volume);

                while (maxVolume.HasValue && plist == null && volTemp <= maxVolume)
                {
                    plist = COSContext.Current.PriceLists.FirstOrDefault(a => a.ID_Forwarder == SelectedItem.ID_Forwarder && a.ZoneCode == SelectedItem.Destination.Code && a.Country.ID == SelectedItem.Destination.Country.ID && a.Volume == (double)volTemp);

                    if (plist != null)
                        SelectedItem.ForwarderPrice = plist.Price;

                    volTemp++;
                }

                if (maxVolume == null && SelectedItem.ForwarderPrice > 0)
                {
                    SelectedItem.ForwarderPrice = 0;
                    SelectedItem.TollPrice = 0;
                    SelectedItem.TollPriceGerlach = 0;
                    SelectedItem.BafPrice = 0;
                }
            }
        }

        private ForeignExportDetail _selectedDetailItem = null;

        public ForeignExportDetail SelectedDetailItem
        {
            set
            {
                _selectedDetailItem = value;
                if (_selectedDetailItem != null)
                {
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

        void _selectedDetailItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VolumeCbm")
            {

                ControlTotalVolume();

                SumDetailValues();

            }
            else if (e.PropertyName == "ForwarderPrice")
            {

            }
            else if (e.PropertyName == "BafPRice")
            {

            }
            else if (e.PropertyName == "TollPrice")
            {

            }
            else if (e.PropertyName == "ID_TransportPayment")
            {
                SelectedDetailItem.RefreshTransportPayment();
            }
            else if (e.PropertyName == "ID_OrderedBy")
            {
                SelectedDetailItem.RefreshOrderedBy();
            }


        }

        public void SumDetailValues()
        {
            if (SelectedItem.VolumeCbm > 0)
            {
                if (SelectedItem.Connections.Count > 0)
                {
                    var kf = SelectedItem.Connections.Sum(a => a.ExportDetails.Sum(c => c.VolumeCbm));

                    foreach (var conn in SelectedItem.Connections)
                    {
                        foreach (var itm in conn.ExportDetails)
                        {
                            if (itm.VolumeCbm > 0)
                            {
                                // var koef = SelectedItem.VolumeCbm / itm.VolumeCbm;

                                itm.ForwarderPrice = Math.Round(SelectedItem.ForwarderPrice / kf * itm.VolumeCbm, 2);

                                if (SelectedItem.Forwarder != null && SelectedItem.Forwarder.CanCalculateBaf)
                                    itm.BafPRice = Math.Round(SelectedItem.BafPrice / kf * itm.VolumeCbm, 2);
                                itm.TollPrice = Math.Round(SelectedItem.TollPrice / kf * itm.VolumeCbm, 2);
                                itm.TollPriceGerlach = Math.Round(SelectedItem.TollPriceGerlach / kf * itm.VolumeCbm, 2);
                                itm.TotalPrice = Math.Round(SelectedItem.TotalPrice / kf * itm.VolumeCbm, 2);
                            }
                        }
                    }
                }
            }
        }


        public void ControlTotalVolume()
        {
            var total = SelectedItem.Connections.Sum(c => c.ExportDetails.Sum(a => a.VolumeCbm));

            if (total > SelectedItem.VolumeCbm)
            {

                var dif = SelectedItem.VolumeCbm - (total - SelectedDetailItem.VolumeCbm);

                SelectedDetailItem.VolumeCbm = dif;
            }

        }

        private void SumTotalValues()
        {
            if (SelectedItem != null)
            {
                //SelectedItem.ForwarderPrice = SelectedItem.ExportDetails.Sum(a => a.ForwarderPrice);
                //SelectedItem.BafPrice = SelectedItem.ExportDetails.Sum(a => a.BafPRice);
                //SelectedItem.TollPrice = SelectedItem.ExportDetails.Sum(a => a.TollPrice);
                //SelectedItem.TotalPrice = SelectedItem.ExportDetails.Sum(a => a.TotalPrice);
                //SelectedItem.VolumeCbm = SelectedItem.ExportDetails.Sum(a => a.VolumeCbm);
            }
        }

        public void ComputeDetailUnits()
        {
            if (SelectedDetailItem != null)
            {
                if (SelectedDetailItem.Unit != null && SelectedDetailItem.NoUnit.HasValue)
                {
                    SelectedDetailItem.VolumeCbm = SelectedDetailItem.Unit.CBM * SelectedDetailItem.NoUnit.Value;
                }
            }
        }

        public void ComputeUnits()
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.Unit != null && SelectedItem.NoUnit.HasValue)
                {
                    SelectedItem.VolumeCbm = SelectedItem.Unit.CBM * SelectedItem.NoUnit.Value;
                }
            }
        }

        public void SumDetailTotalPrice()
        {
            if (SelectedDetailItem != null)
            {
                SelectedDetailItem.TotalPrice = SelectedDetailItem.ForwarderPrice + SelectedDetailItem.BafPRice + SelectedDetailItem.TollPrice + SelectedItem.TollPriceGerlach;
            }
        }

        public void SumTotalPrice()
        {
            if (SelectedItem != null)
            {
                SelectedItem.TotalPrice = SelectedItem.ForwarderPrice + SelectedItem.BafPrice + SelectedItem.TollPrice + SelectedItem.TollPriceGerlach;
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save(false));
            }
        }

        public ICommand UpdateAndRewriteCommand
        {
            get
            {
                return new RelayCommand(param => this.Save(true));
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }


        public void Save(bool rewriteVolumeControl)
        {
            if (SelectedItem.ID > 0)
            {
                var ent = COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedItem);


                var origVal = ent.OriginalValues["PlannedDate"];
                var newVal = ent.CurrentValues["PlannedDate"];


                if (origVal != null && newVal != null)
                {
                    if (origVal.Equals(newVal) || SelectedItem.IsOrderLocked)
                    {
                        SaveInternal(rewriteVolumeControl);
                    }
                    else
                    {
                        rewriteVol = rewriteVolumeControl;
                        RadWindow.Confirm(new DialogParameters()
                        {
                            OkButtonContent = ResourceHelper.GetResource<string>("app_Yes"),
                            CancelButtonContent = ResourceHelper.GetResource<string>("app_No"),
                            Content = ResourceHelper.GetResource<string>("m_Body_LOG00000037"),
                            Closed = new EventHandler<WindowClosedEventArgs>(confirmEmail_closed),
                            Owner = (RadWindow)COSContext.Current.RadMainWindow,
                            Header = ResourceHelper.GetResource<string>("app_Alert")
                        });
                    }
                }
                else
                {
                    SaveInternal(rewriteVolumeControl);
                }
            }
            else 
            {
                SaveInternal(rewriteVolumeControl);
            }

        }

        bool rewriteVol = false;

        private void SaveInternal(bool rewriteVolumeControl) 
        {
            string customErrors = "";

            customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {
                if (SelectedItem.ID == 0)
                {
                    COSContext.Current.ForeignExports.AddObject(SelectedItem);

                    ForeignVolumeControlViewModel.CreateNewVolume(SelectedItem);
                }
                else
                {
                    if (rewriteVolumeControl)
                        ForeignVolumeControlViewModel.RewriteVolume(SelectedItem.VolumeControl.FirstOrDefault(), SelectedItem);
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

                        MessageBox.Show(exc.InnerException != null ? exc.InnerException.Message : exc.Message);

                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    }

                }

                OnPropertyChanged("Save");
            }
            else
            {
                // RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                RaiseErrors = customErrors;
            }
        }

        void confirmEmail_closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                SaveInternal(rewriteVol);
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

            if (temporaryConnections != null)
            {
                //SelectedItem.Connections.Clear();
                foreach (var itm in temporaryConnections)
                {
                    if (itm.Key.EntityState == System.Data.EntityState.Deleted)
                    {
                        COSContext.Current.ObjectStateManager.ChangeObjectState(itm.Key, System.Data.EntityState.Unchanged);

                        foreach (var det in itm.Value)
                        {
                            COSContext.Current.ObjectStateManager.ChangeObjectState(det, System.Data.EntityState.Unchanged);
                            if (!itm.Key.ExportDetails.Contains(det))
                                itm.Key.ExportDetails.Add(det);
                        }

                    }
                }
                COSContext.Current.AcceptAllChanges();
            }
        }

        public string IsValid()
        {
            string err = "";

            if (SelectedItem.Destination == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000010");
                err += Environment.NewLine;
            }

            if (SelectedItem.Forwarder == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000011");
                err += Environment.NewLine;
            }


            if (SelectedItem.PlannedDate <= DateTime.MinValue)
            {
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000013");
                err += Environment.NewLine;
            }

            foreach (var conn in SelectedItem.Connections)
            {
                foreach (var itm in conn.ExportDetails)
                {
                    if (itm.OrderedBy == null)
                    {
                        err += ResourceHelper.GetResource<string>("m_Body_LOG00000014");
                        err += Environment.NewLine;
                    }

                    //if (itm.TransportPayment == null)
                    //{
                    //    err += ResourceHelper.GetResource<string>("m_Body_LOG00000009");
                    //    err += Environment.NewLine;
                    //}

                }
            }

            if (SelectedItem.Connections.Where(a => a.ExportDetails.Count < 1).Count() > 0)
            {
                err = ResourceHelper.GetResource<string>("m_Body_LOG00000007");
                err += Environment.NewLine;
            }


            //var totals = SelectedItem.ExportDetails.Sum(a => a.VolumeCbm);

            //if (totals != SelectedItem.VolumeCbm)
            //{
            //    err = ResourceHelper.GetResource<string>("m_Body_LOG00000015");
            //    err += Environment.NewLine;
            //}

            return err;
        }

        private List<ZoneLogistics> _LocalZoneLogistics = new List<ZoneLogistics>();
        public List<ZoneLogistics> LocalZoneLogistics
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




    }
}