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
using System.Data.Objects;
using System.ComponentModel;

namespace COS.Application.Logistics.Models
{
    public partial class TransportOrdesCoViewModel : ValidationViewModelBase
    {
        public TransportOrdesCoViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;

            SelectedWeek = COSContext.Current.Week;
            SelectedYear = COSContext.Current.DateTimeServer.Year;

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            ReloadDataAsync();
        }

        public void ReloadDataAsync()
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            OnPropertyChanged("DataReloaded");
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;
            ReloadData();
        }

        BackgroundWorker worker = null;

        private COSContext dataContext = null;

        public ICommand RefreshToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.ReloadData());
            }
        }

        private int? _selectedWeek = null;
        public int? SelectedWeek
        {
            get
            {
                return _selectedWeek;
            }
            set
            {
                _selectedWeek = value;
                OnPropertyChanged("SelectedWeek");

                if (worker != null && !worker.IsBusy)
                    worker.RunWorkerAsync();
            }
        }

        private TransportOrderSumClass _selectedCounts = null;
        public TransportOrderSumClass SelectedCounts
        {
            get
            {
                return _selectedCounts;
            }
            set
            {
                _selectedCounts = value;
                OnPropertyChanged("SelectedCounts");

            }
        }

        private int? _selectedYear = null;
        public int? SelectedYear
        {
            get
            {
                return _selectedYear;
            }
            set
            {
                _selectedYear = value;
                OnPropertyChanged("SelectedYear");

                if (worker != null && !worker.IsBusy)
                    worker.RunWorkerAsync();
            }
        }

        public void ReloadData()
        {
            if (SelectedWeek != null && SelectedYear != null)
            {
                LocalTransports.Clear();

                CultureInfo myCI = new CultureInfo("cs-CZ");
                Calendar myCal = myCI.Calendar;

                DateTime selectedDateFrom = (Helpers.FirstDateOfWeek(SelectedYear.Value, SelectedWeek.Value, CalendarWeekRule.FirstFourDayWeek)).AddDays(-7);
                DateTime selectedDateTo = (Helpers.FirstDateOfWeek(SelectedYear.Value, SelectedWeek.Value, CalendarWeekRule.FirstFourDayWeek).AddDays(6)).AddDays(-7);

                var foreignsQuery = dataContext.ForeignExports.Where(a => a.PlannedDate >= selectedDateFrom && a.PlannedDate <= selectedDateTo && a.Forwarder.IsOrderAvailable == true);
                var transportsQuery = dataContext.ForeignExportTransportOrderCoes.Where(a => a.OrderDate >= selectedDateFrom && a.OrderDate <= selectedDateTo);

                dataContext.Refresh(RefreshMode.StoreWins, foreignsQuery);
                dataContext.Refresh(RefreshMode.StoreWins, transportsQuery);

                TransportOrderForeignExportClass trCl = null;

                TransportOrderSumClass sumClass = new TransportOrderSumClass();

                foreach (var itm in foreignsQuery.GroupBy(a => a.Forwarder))
                {
                    var transports = transportsQuery.Where(a => a.ForeignExport.ID_Forwarder == itm.Key.ID);

                    sumClass.TotalMondayCount += transports.Where(a => a.OrderDate == selectedDateFrom).Count();
                    var nd = selectedDateFrom.AddDays(1);
                    sumClass.TotalTuesdayCount += transports.Where(a => a.OrderDate == nd).Count();
                    nd = selectedDateFrom.AddDays(2);
                    sumClass.TotalWednesdayCount += transports.Where(a => a.OrderDate == nd).Count();
                    nd = selectedDateFrom.AddDays(3);
                    sumClass.TotalThursdayCount += transports.Where(a => a.OrderDate == nd).Count();
                    nd = selectedDateFrom.AddDays(4);
                    sumClass.TotalFridayCount += transports.Where(a => a.OrderDate == nd).Count();

                    string selectedKey = SelectedWeek.ToString().PadLeft(2, '0') + "-" + SelectedYear.ToString() + "-" + itm.Key.ID.ToString().PadLeft(6, '0');
                    var processDateEntity = this.dataContext.ProcessDates.FirstOrDefault(a => a.Type.ProcessName == "TransportOrdesCoDetailView" && a.ProcessKey == selectedKey);

                    DateTime? lastOrderDate = null;

                    if (processDateEntity != null)
                        lastOrderDate = processDateEntity.ProcessDate;

                    trCl = new TransportOrderForeignExportClass(itm.ToList(), transports.ToList(), selectedDateFrom, selectedDateTo, lastOrderDate);

                    LocalTransports.Add(trCl);
                }

                //TransportOrderSumClass sumClass = new TransportOrderSumClass();

                //sumClass.TotalMondayCount = LocalTransports.Where(a => a.Transports.Where(k => k.OrderDate == selectedDateFrom).Count() > 0).Select(a => a.Transports).Count();
                //sumClass.TotalTuesdayCount = LocalTransports.Where(a => a.Transports.Where(k => k.OrderDate == selectedDateFrom.AddDays(1)).Count() > 0).Select(a => a.Transports).Count();
                //sumClass.TotalWednesdayCount = LocalTransports.Where(a => a.Transports.Where(k => k.OrderDate == selectedDateFrom.AddDays(2)).Count() > 0).Sum(a => a.Transports.Count);
                //sumClass.TotalThursdayCount = LocalTransports.Where(a => a.Transports.Where(k => k.OrderDate == selectedDateFrom.AddDays(3)).Count() > 0).Select(a => a.Transports).Count();
                //sumClass.TotalFridayCount = LocalTransports.Where(a => a.Transports.Where(k => k.OrderDate == selectedDateFrom.AddDays(4)).Count() > 0).Select(a => a.Transports).Count();

                SelectedCounts = sumClass;
            }


        }



        public ObservableCollection<TransportOrderForeignExportClass> _localTransports = new ObservableCollection<TransportOrderForeignExportClass>();

        public ObservableCollection<TransportOrderForeignExportClass> LocalTransports
        {
            get
            {
                return _localTransports;
            }
        }

        public void SetForwarderCounts(Forwarder forwarder)
        {
            var export = LocalTransports.FirstOrDefault(a => a.Forwarder == forwarder);

            DateTime selectedDateFrom = (Helpers.FirstDateOfWeek(SelectedYear.Value, SelectedWeek.Value, CalendarWeekRule.FirstFourDayWeek)).AddDays(-7);
                    

            SelectedCounts.ForwarderMondayCount = export.Transports.Where(p => p.OrderDate == selectedDateFrom).Count();
            SelectedCounts.ForwarderTuesdayCount = export.Transports.Where(p => p.OrderDate == selectedDateFrom.AddDays(1)).Count();
            SelectedCounts.ForwarderWednesdayCount = export.Transports.Where(p => p.OrderDate == selectedDateFrom.AddDays(2)).Count();
            SelectedCounts.ForwarderThursdayCount = export.Transports.Where(p => p.OrderDate == selectedDateFrom.AddDays(3)).Count();
            SelectedCounts.ForwarderFridayCount = export.Transports.Where(p => p.OrderDate == selectedDateFrom.AddDays(4)).Count();
            
            SelectedCounts = SelectedCounts;
            OnPropertyChanged("SelectedCounts");
        }

    }

    /// <summary>
    /// pracovní třída pro zobrazení spojení transportů objednáných a neobjednaných
    /// </summary>
    public class TransportOrderForeignExportClass
    {
        public TransportOrderForeignExportClass(List<ForeignExport> exports, List<ForeignExportTransportOrderCo> transports, DateTime dateFrom, DateTime dateTo, DateTime? lastOrderDate)
        {
            ForeigExports = exports;
            Transports = transports;
            DateFrom = dateFrom;
            DateTo = dateTo;
            LastOrderDate = lastOrderDate;
        }

        public List<ForeignExport> ForeigExports { set; get; }
        public List<ForeignExportTransportOrderCo> Transports { set; get; }

        public Forwarder Forwarder
        {
            get
            {
                Forwarder fw = null;

                if (ForeigExports != null)
                    fw = ForeigExports.FirstOrDefault().Forwarder;

                return fw;
            }
        }
        public DateTime? LastOrderDate
        {
            get;
            set;
        }



        public int OrderedTransportCount
        {
            get
            {
                return Transports.Count;
            }
        }

        public DateTime DateFrom { set; get; }
        public DateTime DateTo { set; get; }
    }

    public class TransportOrderSumClass
    {
        public int ForwarderMondayCount { set; get; }
        public int ForwarderTuesdayCount { set; get; }
        public int ForwarderWednesdayCount { set; get; }
        public int ForwarderThursdayCount { set; get; }
        public int ForwarderFridayCount { set; get; }
        public int ForwarderSaturdayCount { set; get; }
        public int ForwarderSundayCount { set; get; }

        public int TotalMondayCount { set; get; }
        public int TotalTuesdayCount { set; get; }
        public int TotalWednesdayCount { set; get; }
        public int TotalThursdayCount { set; get; }
        public int TotalFridayCount { set; get; }
        public int TotalSaturdayCount { set; get; }
        public int TotalSundayCount { set; get; }
    }
}
