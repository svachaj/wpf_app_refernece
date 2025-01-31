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
    public partial class ForeignVolumeControlViewModel : ValidationViewModelBase
    {
        public ForeignVolumeControlViewModel()
            : base()
        {
            InitExports(null);
        }


        public ICommand RefreshToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.RefreshData());
            }
        }

        public ICommand SaveToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }

        public void Save()
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

        public void Cancel()
        {
            COSContext.Current.RejectChanges();

            OnPropertyChanged("Cancel");
        }

        public void RefreshData()
        {
            InitExports(null);

            OnPropertyChanged("RefreshData");
        }

        private DateTime _selectedDateFrom = DateTime.Now.Date.AddDays(-7);
        public DateTime SelectedDateFrom
        {
            set
            {
                _selectedDateFrom = value;
                InitExports(null);
                OnPropertyChanged("SelectedDateFrom");
            }
            get
            {
                return _selectedDateFrom;
            }
        }

        private DateTime _selectedDateTo = DateTime.Now.Date;
        public DateTime SelectedDateTo
        {
            set
            {
                _selectedDateTo = value;
                InitExports(null);
                OnPropertyChanged("SelectedDateTo");
            }
            get
            {
                return _selectedDateTo;
            }
        }

        private ForeignExportVolumeControl _selectedItem;
        public ForeignExportVolumeControl SelectedItem
        {
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedItem.PropertyChanged -= _selectedItem_PropertyChanged;
                    _selectedItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedItem_PropertyChanged);
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
            if (e.PropertyName == "Volume_VA" || e.PropertyName == "Volume_VI")
            {
                RecalculateVolume(SelectedItem);
            }
            else if (e.PropertyName == "IsCompleted")
            {
                OnPropertyChanged("IsCompletedChanged");
            }
        }

        public static void RecalculateVolume(ForeignExportVolumeControl volumeItem)
        {
            if (volumeItem != null)
            {
                if (volumeItem.PlannedVolume_VA == 0)
                    volumeItem.Load_VA = 0;
                else
                    volumeItem.Load_VA = Math.Round((decimal)(((volumeItem.Volume_VA / volumeItem.PlannedVolume_VA) - 1) * 100), 2);

                if (volumeItem.PlannedVolume_VI == 0)
                    volumeItem.Load_VI = 0;
                else
                    volumeItem.Load_VI = Math.Round((decimal)(((volumeItem.Volume_VI / volumeItem.PlannedVolume_VI) - 1) * 100), 2);

                if (volumeItem.PlannedVolume_VI == 0 && volumeItem.PlannedVolume_VA == 0)
                    volumeItem.TotalLoadDifference = 0;
                else
                    volumeItem.TotalLoadDifference = Math.Round((decimal)(((volumeItem.Volume_VA + volumeItem.Volume_VI) / (volumeItem.PlannedVolume_VA + volumeItem.PlannedVolume_VI)) - 1) * 100, 2);
                
                volumeItem.TotalVolumeDifference = (volumeItem.Volume_VA + volumeItem.Volume_VI) - (volumeItem.PlannedVolume_VA + volumeItem.PlannedVolume_VI);

                if (volumeItem.ForeignExport.Unit.VolControlTopValue.HasValue)
                {
                    if ((volumeItem.Volume_VI + volumeItem.Volume_VA) >= volumeItem.ForeignExport.Unit.VolControlTopValue)
                    {
                        volumeItem.Cost = 0;
                    }
                    else
                    {
                        volumeItem.Cost = ((volumeItem.Volume_VA + volumeItem.Volume_VI) - volumeItem.ForeignExport.Unit.VolControlTopValue) * volumeItem.CBM_Price; 
                       
                    }
                }
                else
                    volumeItem.Cost = volumeItem.TotalVolumeDifference * volumeItem.CBM_Price;

                volumeItem.ChangedBy = COSContext.Current.CurrentUser;
                volumeItem.ChangedDate = COSContext.Current.DateTimeServer;
            }
        }

        public static ForeignExportVolumeControl CreateNewVolume(ForeignExport foreignExport)
        {
            ForeignExportVolumeControl newVol = COSContext.Current.ForeignExportVolumeControls.CreateObject();

            newVol.ForeignExport = foreignExport;
            newVol.PlannedVolume_VA = foreignExport.VAinc4HVolumeCBM;
            newVol.PlannedVolume_VI = foreignExport.VIVolumeCBM;
            newVol.CBM_Price = foreignExport.TotalPrice / foreignExport.VolumeCbm;

            COSContext.Current.ForeignExportVolumeControls.AddObject(newVol);

            return newVol;
        }

        public static void RewriteVolume(ForeignExportVolumeControl volume, ForeignExport foreignExport)
        {
            if (volume != null && foreignExport != null)
            {
                volume.ForeignExport = foreignExport;
                volume.PlannedVolume_VA = foreignExport.VAinc4HVolumeCBM;
                volume.PlannedVolume_VI = foreignExport.VIVolumeCBM;
                volume.CBM_Price = foreignExport.TotalPrice / foreignExport.VolumeCbm;

                RecalculateVolume(volume);
            }
        }


        private ObservableCollection<ForeignExportVolumeControl> _localForeignExports = new ObservableCollection<ForeignExportVolumeControl>();
        public ObservableCollection<ForeignExportVolumeControl> LocalForeignExports
        {
            get
            {
                return _localForeignExports;
            }
        }

        public void InitExports(IEnumerable<ForeignExportVolumeControl> exports)
        {
            LocalForeignExports.Clear();
            if (exports == null)
            {
                var qr = COSContext.Current.ForeignExportVolumeControls.Where(a => a.ForeignExport.PlannedDate >= SelectedDateFrom && a.ForeignExport.PlannedDate <= SelectedDateTo && a.ForeignExport.Forwarder.CanCalculateBaf == true);
                COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, qr);

                var qrlist = qr.ToList();
                foreach (var itm in qrlist)
                {
                    LocalForeignExports.Add(itm);
                }
            }
            else
            {
                foreach (var itm in exports)
                    LocalForeignExports.Add(itm);
            }
        }

    }
}
