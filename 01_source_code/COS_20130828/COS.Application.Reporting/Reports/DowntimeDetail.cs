namespace COS.Application.Reporting.Reports
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using COS.Application.Reporting.Models;
    using System.Linq;
    using COS.Resources;

    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class DowntimeDetail : Telerik.Reporting.Report
    {
        public DowntimeDetail(DowntimeDetailModel source)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            //Labels
            tbHeader.Value = ResourceHelper.GetResource<string>("rep_DowntimeDetailHeader");
            tbGridDay.Value = ResourceHelper.GetResource<string>("rep_Day");
            tbGridActualTime.Value = ResourceHelper.GetResource<string>("rep_DdActualTime");
            tbGridActualTimeTotal.Value = ResourceHelper.GetResource<string>("rep_DdActualTime");
            tbGridDowntimeTime.Value = ResourceHelper.GetResource<string>("rep_DdDowntimeTime");
            tbGridDowntimeTotal.Value = ResourceHelper.GetResource<string>("rep_DdDowntimeTime");
            tbGridTotal.Value = ResourceHelper.GetResource<string>("rep_DdTotalTime");
            tbLoss.Value = ResourceHelper.GetResource<string>("rep_DdLoss");


            table1.DataSource = source.ReportData;
            tbDowntime.Value = source.SelectedDowntime.Description;
            decimal p1 = source.ActualTimeForTotal;
            decimal p2 = source.ReportData.Sum(a => a.DowntimeTime);


            if (p1 != 0)
            {
                decimal p3 = (p2 / p1) * 100;
                tbtotalLoss.Value = p3.ToString("N2") + " %";
            }
            else
            {
                var p4 = 0;
                tbtotalLoss.Value = p4 + " %";
            }


            tbWorkcenter.Value = source.WorkCenterString;


            tbtotalActualtime.Value = p1.ToString() + " " + ResourceHelper.GetResource<string>("rep_Ddminute");
            tbtotalDowntime.Value = p2.ToString() + " " + ResourceHelper.GetResource<string>("rep_Ddminute");
        }
    }
}