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
    public partial class DowntimePareto : Telerik.Reporting.Report
    {
        public DowntimePareto(DowntimeParetoModel source)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            model = source;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            //Labels
            tbHeader.Value = ResourceHelper.GetResource<string>("rep_T10dHeader");
            tbGridDowntime.Value = ResourceHelper.GetResource<string>("rep_T10downtime");
            tbGridDowntimeTime.Value = ResourceHelper.GetResource<string>("rep_T10downtimeTime");
            tbGridDowntimeCount.Value = ResourceHelper.GetResource<string>("rep_T10Count");
            tbGridDowntimeScale.Value = ResourceHelper.GetResource<string>("rep_T10Scale");
            tbGridDowntimeNote.Value = ResourceHelper.GetResource<string>("rep_T10Note");
            //tbGridProductivity.Value = ResourceHelper.GetResource<string>("rep_Productivity");
            //tbGridQuality.Value = ResourceHelper.GetResource<string>("rep_Quality");
        

            table1.DataSource = source.DowntimesSums;
           // tbBonusGroup.Value = source.SelectedBonusGroup.Description;


        }

        public DowntimeParetoModel model = null;

        private void textBox4_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;
            DowntimeSum sumd = tb.DataObject.RawData as DowntimeSum;

            if (sumd.Description != null && model.ShowDetail)
            {
                tb.Style.BackgroundColor = System.Drawing.Color.LightBlue;
            }
        }

        private void textBox5_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;
            DowntimeSum sumd = tb.DataObject.RawData as DowntimeSum;

            if (sumd.Description != null && model.ShowDetail)
            {
                tb.Style.BackgroundColor = System.Drawing.Color.LightBlue;
            }
        }

        private void textBox6_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;
            DowntimeSum sumd = tb.DataObject.RawData as DowntimeSum;

            if (sumd.Description != null && model.ShowDetail)
            {
                tb.Style.BackgroundColor = System.Drawing.Color.LightBlue;
            }
        }

        private void textBox2_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;
            DowntimeSum sumd = tb.DataObject.RawData as DowntimeSum;

            if (sumd.Description != null && model.ShowDetail)
            {
                tb.Style.BackgroundColor = System.Drawing.Color.LightBlue;
            }
        }

        private void textBox3_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;
            DowntimeSum sumd = tb.DataObject.RawData as DowntimeSum;

            if (sumd.Description != null && model.ShowDetail)
            {
                tb.Style.BackgroundColor = System.Drawing.Color.LightBlue;
            }

            if (sumd.Description == null && string.IsNullOrEmpty(sumd.Note) && model.ShowDetail)
                tb.Value = ResourceHelper.GetResource<string>("rep_T10Others");
        }

       

    }
}