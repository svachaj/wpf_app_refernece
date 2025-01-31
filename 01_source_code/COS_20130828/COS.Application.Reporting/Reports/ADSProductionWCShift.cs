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
    public partial class ADSProductionWCShift : Telerik.Reporting.Report
    {
        public ADSProductionWCShift(ADSProductionWCShiftModel source)
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
            tbHeader.Value = ResourceHelper.GetResource<string>("rep_ShiftFollowupHeader") + " - " + (source.SelectedDate.HasValue == true ? source.SelectedDate.Value.ToShortDateString() : "");
            tbGridActualtime.Value = ResourceHelper.GetResource<string>("rep_ActualTime");
            tbGridAvailability.Value = ResourceHelper.GetResource<string>("rep_Availability");
            tbGridOee.Value = ResourceHelper.GetResource<string>("rep_OEE");
            tbGridPerformance.Value = ResourceHelper.GetResource<string>("rep_Performance");
            tbGridQuality.Value = ResourceHelper.GetResource<string>("rep_Quality");
            tbGridWc.Value = ResourceHelper.GetResource<string>("rep_WorkCenter");


            tbDivision.Value = source.DivisionString;
            tbShift.Value = source.ShiftString;

            table1.DataSource = source.ReportData;

        }

        public ADSProductionWCShiftModel model = null;

        private void textBox5_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;

            decimal val = 0;
            if (tb.Value != null)
                decimal.TryParse(tb.Value.ToString(), out val);

            if (model.SelectedDivision.Value == "VI")
            {
                if (val == model.Formatting.Performance_Eq)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.Performance_More)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.Performance_LessEq && val >= model.Formatting.Performance_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.Performance_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }
            else 
            {
                if (val == model.Formatting.Performance_Eq_VA)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.Performance_More_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.Performance_LessEq_VA && val >= model.Formatting.Performance_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.Performance_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }

        }

        private void textBox6_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;

            decimal val = 0;
            if (tb.Value != null)
                decimal.TryParse(tb.Value.ToString(), out val);

            if (model.SelectedDivision.Value == "VI")
            {
                if (val == model.Formatting.Availability_Eq)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.Availability_More)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.Availability_LessEq && val >= model.Formatting.Availability_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.Availability_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }
            else 
            {
                if (val == model.Formatting.Availability_Eq_VA)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.Availability_More_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.Availability_LessEq_VA && val >= model.Formatting.Availability_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.Availability_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }
        }

        private void textBox2_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;

            decimal val = 0;
            if (tb.Value != null)
                decimal.TryParse(tb.Value.ToString(), out val);
           
            if (model.SelectedDivision.Value == "VI")
            {
                if (val == model.Formatting.Quality_Eq)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.Quality_More)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.Quality_LessEq && val >= model.Formatting.Quality_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.Quality_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }
            else 
            {
                if (val == model.Formatting.Quality_Eq_VA)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.Quality_More_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.Quality_LessEq_VA && val >= model.Formatting.Quality_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.Quality_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }
        }

        private void textBox7_ItemDataBound(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.TextBox tb = sender as Telerik.Reporting.Processing.TextBox;

            decimal val = 0;
            if (tb.Value != null)
                decimal.TryParse(tb.Value.ToString(), out val);
           
            if (model.SelectedDivision.Value == "VI")
            {
                if (val == model.Formatting.OEE_Eq)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.OEE_More)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.OEE_LessEq && val >= model.Formatting.OEE_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.OEE_MoreEq)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }
            else 
            {
                if (val == model.Formatting.OEE_Eq_VA)
                    tb.Style.BackgroundColor = model.Formatting.ColorEq;
                else
                {
                    if (val > model.Formatting.OEE_More_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMore;
                    else if (val <= model.Formatting.OEE_LessEq_VA && val >= model.Formatting.OEE_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorMoreEq;
                    else if (val < model.Formatting.OEE_MoreEq_VA)
                        tb.Style.BackgroundColor = model.Formatting.ColorLess;
                }
            }
        }



    }
}