namespace COS.Application.Reporting.Reports
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using COS.Application.Reporting.Models;
    using COS.Resources;
    using COS.Application.Shared;

    /// <summary>
    /// Summary description for DomLogisticForwarder.
    /// </summary>
    public partial class DomLogisticForwarder : Telerik.Reporting.Report
    {
        public DomLogisticForwarder(DomLogisticForwarderModel source)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

       
            
               tbGridDate.Value = ResourceHelper.GetResource<string>("rep_DomDate");
               tbGridPointOfOrigin.Value = ResourceHelper.GetResource<string>("rep_DomPoint");
               tbGridCustomer.Value = ResourceHelper.GetResource<string>("rep_DomCustomer");
               tbGridDestination.Value = ResourceHelper.GetResource<string>("rep_DomDestination");
               tbGridAddress.Value = ResourceHelper.GetResource<string>("rep_DomAddress");
               tbGridRound.Value = ResourceHelper.GetResource<string>("rep_DomRound");
               tbGridDriver.Value = ResourceHelper.GetResource<string>("rep_DomDriver");
               tbGridCarType.Value = ResourceHelper.GetResource<string>("rep_DomCarType");
               tbGridVolume.Value = ResourceHelper.GetResource<string>("rep_DomVolume");
               tbGridPrice.Value = ResourceHelper.GetResource<string>("rep_DomPrice");
               tbGridDistance.Value = ResourceHelper.GetResource<string>("rep_DomDistance");



            tbForwarder.Value = ResourceHelper.GetResource<string>("rep_DomForwarder");
            tbForwarderName.Value = source.SelectedForwarder.Name;

            tblMain.DataSource = source.ReportData;
            
            
        }


      
      
      
    }
}