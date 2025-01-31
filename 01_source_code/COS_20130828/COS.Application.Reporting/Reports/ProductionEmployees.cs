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
    public partial class ProductionEmployees : Telerik.Reporting.Report
    {
        public ProductionEmployees(ProductionEmployeesModel source)
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
            tbHeader.Value = ResourceHelper.GetResource<string>("rep_ePrHeader");
            tbGridHrId.Value = ResourceHelper.GetResource<string>("rep_ePrHrId");
            tbGridName.Value = ResourceHelper.GetResource<string>("rep_ePrName");
            tbGridWorkGroup.Value = ResourceHelper.GetResource<string>("rep_WorkGroup");
            tbShiftColor.Value = ResourceHelper.GetResource<string>("rep_ePrShift");
            tbGridProductivity.Value = ResourceHelper.GetResource<string>("rep_Productivity");
            tbGridQuality.Value = ResourceHelper.GetResource<string>("rep_Quality");
        

            table1.DataSource = source.ReportData;
            tbBonusGroup.Value = source.SelectedBonusGroup.Description;


        }

        public ProductionEmployeesModel model = null;


    }
}