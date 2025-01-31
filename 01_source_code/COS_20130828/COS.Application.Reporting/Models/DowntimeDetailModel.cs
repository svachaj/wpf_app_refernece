using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using COS.Application.Reporting.Views;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using System.Globalization;

namespace COS.Application.Reporting.Models
{
    public partial class DowntimeDetailModel : ValidationViewModelBase
    {
        public DowntimeDetailModel()
            : base()
        {


        }




        private DateTime? _selectedDateFrom = COSContext.Current.DateTimeServer.Date;
        public DateTime? SelectedDateFrom
        {
            set
            {
                _selectedDateFrom = value;
                OnPropertyChanged("SelectedDateFrom");
            }
            get
            {
                return _selectedDateFrom;
            }
        }

        private DateTime? _selectedDateTo = COSContext.Current.DateTimeServer.Date;
        public DateTime? SelectedDateTo
        {
            set
            {
                _selectedDateTo = value;
                OnPropertyChanged("SelectedDateTo");
            }
            get
            {
                return _selectedDateTo;
            }
        }

        public List<WorkCenter> LocalWorkCenters
        {
            get
            {
                return COSContext.Current.WorkCenters.OrderBy(a => a.Value).ToList();
            }
        }

        public List<Downtime> LocalDowntimes
        {
            get
            {
                return COSContext.Current.Downtimes.ToList().OrderBy(a => a.Description).ToList();
            }
        }


        private WorkCenter _selectedWorkCenter = null;
        public WorkCenter SelectedWorkCenter
        {
            set
            {
                _selectedWorkCenter = value;
                OnPropertyChanged("SelectedWorkCenter");
            }
            get
            {
                return _selectedWorkCenter;
            }
        }

        private Downtime _selectedDowntime = null;
        public Downtime SelectedDowntime
        {
            set
            {
                _selectedDowntime = value;
                OnPropertyChanged("SelectedDowntime");
            }
            get
            {
                return _selectedDowntime;
            }
        }

        private List<ReportDataClass1> _reportData = new List<ReportDataClass1>();
        public List<ReportDataClass1> ReportData
        {
            get
            {
                return _reportData;
            }
        }

        public void GenerateData()
        {
            var cc = COSContext.Current;

            if (SelectedWorkCenter != null && SelectedDowntime != null && SelectedDateFrom != null && SelectedDateTo != null)
            {
                ReportData.Clear();

                var prods = cc.HourlyProductions.Where(a => a.ID_WorkCenter == SelectedWorkCenter.ID && a.Date >= SelectedDateFrom && a.Date <= SelectedDateTo);

                ActualTimeForTotal = prods.Sum(a => (int?)a.ActualTime_min) ?? 0;
                var groupByDay = prods.GroupBy(a => a.Date);

                WorkCenterString = SelectedWorkCenter.Value + " - " + SelectedWorkCenter.Description;

                ReportDataClass1 data = null;
                foreach (var itm in groupByDay)
                {
                    data = new ReportDataClass1();
                    data.Date = itm.Key;
                    data.Downtime = SelectedDowntime.Description;
                                

                    foreach (var dt in itm)
                    {
                        var asset = dt.Assets.FirstOrDefault(a => a.ID_Downtime == SelectedDowntime.ID);

                        data.ActualTime += dt.ActualTime_min;

                        if (asset != null)
                        {                        
                            data.DowntimeTime += asset.Time_min;
                        }
                    }

                    if (data.DowntimeTime > 0)
                        ReportData.Add(data);
                }
            }
        }

        public string WorkCenterString { set; get; }
        public int ActualTimeForTotal { set; get; }
    }

    public class ReportDataClass1
    {
        public DateTime Date { set; get; }
        public string DateString { get { return Date.ToShortDateString(); } }

        public string ActualTimeString { get { return ActualTime.ToString() + " " + ResourceHelper.GetResource<string>("rep_Ddminute"); } }
        public string DowntimeTimeString { get { return DowntimeTime.ToString() + " " + ResourceHelper.GetResource<string>("rep_Ddminute"); } }

        public string Downtime { set; get; }
       
        public int ActualTime { set; get; }
       
        public int DowntimeTime { set; get; }
    }

}
