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
using System.Data.Objects;
using System.Transactions;

namespace COS.Application.Logistics.Models
{
    public partial class DomesticExportViewModel : ValidationViewModelBase
    {
        public DomesticExportViewModel()
            : base()
        {
            SelectedMiniFilter = LocalMiniFilter.First();
        }


        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNew());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }

        public void Delete()
        {
            OnPropertyChanged("Delete");
        }

        public void AddNew()
        {
            OnPropertyChanged("AddNew");
        }

        private DateTime _selectedDate = DateTime.Now.Date;
        public DateTime SelectedDate
        {
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
            get
            {
                return _selectedDate;
            }
        }

        private decimal _sumVolumeCBM = 0;
        public decimal SumVolumeCBM
        {
            set
            {
                _sumVolumeCBM = value;
                OnPropertyChanged("SumVolumeCBM");
            }
            get
            {
                return _sumVolumeCBM;
            }
        }

        private decimal _sumVolumeCBMVI = 0;
        public decimal SumVolumeCBMVI
        {
            set
            {
                _sumVolumeCBMVI = value;
                OnPropertyChanged("SumVolumeCBMVI");
            }
            get
            {
                return _sumVolumeCBMVI;
            }
        }

        private decimal _sumVolumeCBMVA = 0;
        public decimal SumVolumeCBMVA
        {
            set
            {
                _sumVolumeCBMVA = value;
                OnPropertyChanged("SumVolumeCBMVA");
            }
            get
            {
                return _sumVolumeCBMVA;
            }
        }

        private int _sumCount = 0;
        public int SumCount
        {
            set
            {
                _sumCount = value;
                OnPropertyChanged("SumCount");
            }
            get
            {
                return _sumCount;
            }
        }

        private int _sumCountWithCondition = 0;
        public int SumCountWithCondition
        {
            set
            {
                _sumCountWithCondition = value;
                OnPropertyChanged("SumCountWithCondition");
            }
            get
            {
                return _sumCountWithCondition;
            }
        }


        private List<DomesticExport> _localDomesticExport = new List<DomesticExport>();
        public List<DomesticExport> LocalDomesticExport
        {
            set
            {
                if (_localDomesticExport != value)
                {
                    _localDomesticExport = value;
                    OnPropertyChanged("LocalDomesticExport");

                    if (_localDomesticExport != null)
                    {
                        SumCount = _localDomesticExport.Count;
                        SumVolumeCBM = _localDomesticExport.Sum(a => a.TotalVolume);
                        //SumCountWithCondition = _localDomesticExport.Where(a => a.Finished).Count();
                        SumVolumeCBMVA = _localDomesticExport.Sum(a => a.VAVolumeCBM);
                        SumVolumeCBMVI = _localDomesticExport.Sum(a => a.VIVolumeCBM);
                    }
                    else
                    {
                        SumCountWithCondition = 0;
                        SumCount = 0;
                        SumVolumeCBM = 0;
                        SumVolumeCBMVI = 0;
                        SumVolumeCBMVA = 0;
                    }
                }
            }
            get
            {
                return _localDomesticExport;
            }
        }


        private KeyValuePair<int, string> _selectedMiniFilter;
        public KeyValuePair<int, string> SelectedMiniFilter
        {
            set
            {
                _selectedMiniFilter = value;
                if (_selectedMiniFilter.Key == 0)
                {
                    ConditionCountVisibility = Visibility.Visible;
                }
                else
                {
                    ConditionCountVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged("SelectedMiniFilter");
            }
            get
            {
                return _selectedMiniFilter;
            }
        }

        public Dictionary<int, string> LocalMiniFilter
        {
            get
            {
                Dictionary<int, string> result = new Dictionary<int, string>();

                result.Add(0, ResourceHelper.GetResource<string>("log_All"));
                //result.Add(1, ResourceHelper.GetResource<string>("log_Planned"));
                //result.Add(2, ResourceHelper.GetResource<string>("log_LoadDepart"));
                result.Add(3, ResourceHelper.GetResource<string>("log_CompleteLoading"));

                return result;
            }
        }

        private Visibility _conditionCountVisibility = Visibility.Visible;
        public Visibility ConditionCountVisibility
        {
            set
            {
                _conditionCountVisibility = value;
                OnPropertyChanged("ConditionCountVisibility");
            }
            get
            {
                return _conditionCountVisibility;
            }
        }

        public void RecalculateForeignExport(DateTime? dateFrom, DateTime? dateTo)
        {
            COSContext.Current.Refresh(RefreshMode.StoreWins, COSContext.Current.DomesticExports.Where(a => a.PlannedDate >= dateFrom && a.PlannedDate <= dateTo));

            List<DomesticExport> list = COSContext.Current.DomesticExports.Where(a => a.PlannedDate >= dateFrom && a.PlannedDate <= dateTo).ToList<DomesticExport>();

            DomesticExportDetailViewModel model = new DomesticExportDetailViewModel();
            foreach (DomesticExport export in list)
            {
                model.SelectedItem = export;
                foreach (DomesticExportDetail detail in export.ExportDetails)
                {
                    model.SelectedDetailItem = detail;
                    if (!model.SelectedItem.Forwarder.CanRecalculateBafPrice)
                    {
                        model.SelectedDetailItem.BafPrice = 0M;
                    }
                    model.RecalculateWithoutAPI(false);

                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    COSContext.Current.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    scope.Complete();
                }
                catch (Exception exception)
                {
                    Logging.LogException(exception, LogType.ToFileAndEmail);
                }
                finally
                {
                    if (scope != null)
                    {
                        scope.Dispose();
                    }
                }
            }
        }

    }
}
