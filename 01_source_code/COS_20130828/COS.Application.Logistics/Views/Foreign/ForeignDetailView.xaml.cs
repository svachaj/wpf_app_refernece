using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using COS.Application.Shared;
using System.ComponentModel;
using COS.Application.Logistics.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class ForeignDetailView : BaseUserControl
    {
        public ForeignDetailView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new ForeignDetailViewModel();
                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ForeignDetailView_Loaded);
            }
        }

        void ForeignDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            if (model.SelectedItem != null && model.SelectedItem.ID > 0)
            {
                cmbCountries.SelectedItem = model.SelectedItem.Destination.Country;
                cmbZones.SelectedItem = model.SelectedItem.Destination;
            }
            else
            {
                cmbCountries.SelectedItem = null;
                model.LocalZoneLogistics = new List<ZoneLogistics>();
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cancel")
            {
                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = false;
                    RaiseWindow.Close();
                }
            }
            else if (e.PropertyName == "Save")
            {

                if (RaiseWindow != null)
                {

                    RaiseWindow.DialogResult = true;
                    RaiseWindow.Close();
                }

            }
            else if (e.PropertyName == "SelectedItem")
            {
                if (model.SelectedItem != null)
                {

                }
            }
            else if (e.PropertyName == "RaiseErrors")
            {
                if (model.SelectedItem != null && !string.IsNullOrEmpty(model.RaiseErrors))
                {
                    RadWindow.Alert(new DialogParameters() { Content = model.RaiseErrors, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    //RadWindows.Alert("Chyba", model.RaiseErrors, RaiseWindow);
                }
            }

        }


        public ForeignDetailViewModel model = null;
        public RadWindow RaiseWindow = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExportDetail_CO", "Insert"))
            {
                if (model.SelectedItem != null && model.SelectedItem.Connections.Count > 0)
                {
                    ForeignExportAddZoneWindow wnd = new ForeignExportAddZoneWindow(model);

                    wnd.ShowDialog();
                }
            }

        }

        private void deleteSelectedItemClick(object sender, RoutedEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExportDetail_CO", "Delete"))
            {
                RadButton btn = sender as RadButton;

                if (btn != null)
                {
                    ForeignExportDetail detail = btn.DataContext as ForeignExportDetail;

                    if (detail != null)
                    {

                        if (detail.Connection.ExportDetails.Count > 1)
                        {
                            var conn = detail.Connection;

                            detail.Connection.ExportDetails.Remove(detail);
                            COSContext.Current.ForeignExportDetails.DeleteObject(detail);
                            // grvDetails.Rebind();

                            if (conn.ExportDetails.Count > 0)
                                model.SelectedDetailItem = conn.ExportDetails.FirstOrDefault();
                            else
                                model.SelectedDetailItem = model.SelectedItem.Connections.FirstOrDefault().ExportDetails.FirstOrDefault();
                        }
                        else
                        {
                            string err = COS.Resources.ResourceHelper.GetResource<string>("m_Body_LOG00000007");
                            RadWindow.Alert(new DialogParameters() { Content = err, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                        }

                    }
                }
            }
        }



        private void countryRadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadComboBox cmb = sender as RadComboBox;

            if (cmb != null)
            {
                if (cmb.SelectedItem != null)
                {
                    Country cnt = cmb.SelectedItem as Country;
                    if (cnt != null)
                    {
                        model.LocalZoneLogistics = COSContext.Current.ZoneLogistics.Where(a => a.ID_Country == cnt.ID).OrderBy(a => a.DestinationName).ToList();

                    }
                }
            }
        }



        private void grvDetails_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RadGridView rgv = sender as RadGridView;

            if (rgv != null)
            {
                ForeignExportDetail detail = rgv.SelectedItem as ForeignExportDetail;

                if (detail != null)
                {
                    model.SelectedDetailItem = detail;
                }
            }
        }

        private void grvDetails_GotFocus(object sender, RoutedEventArgs e)
        {

            RadGridView rdgv = sender as RadGridView;

            if (rdgv.SelectedItem == null)
            {
                var details = (System.Data.Objects.DataClasses.EntityCollection<ForeignExportDetail>)rdgv.ItemsSource;

                if (details != null)
                    rdgv.SelectedItem = details.FirstOrDefault();

            }



        }

        private void RadListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            RadListBox rlb = sender as RadListBox;

            var connection = rlb.SelectedItem as ForeignExportConnection;

            if (connection != null)
            {
                model.SelectedDetailItem = connection.ExportDetails.FirstOrDefault();
            }


        }

        private void btnDetailAdd_Click(object sender, RoutedEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExportDetail_CO", "Insert"))
            {
                Button btn = sender as Button;

                if (btn != null)
                {

                    ForeignExportConnection conn = btn.DataContext as ForeignExportConnection;

                    if (conn != null)
                    {
                        ForeignExportDetail newDetail = COSContext.Current.ForeignExportDetails.CreateObject();
                        COSContext.Current.ForeignExportDetails.AddObject(newDetail);

                        conn.ExportDetails.Add(newDetail);

                        model.SelectedDetailItem = newDetail;
                    }
                }
            }

        }

        private void btnConnlDelete_Click(object sender, RoutedEventArgs e)
        {

            if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExportDetail_CO", "Delete"))
            {
                Button btn = sender as Button;

                if (btn != null)
                {

                    ForeignExportConnection conn = btn.DataContext as ForeignExportConnection;

                    if (conn != null)
                    {
                        COSContext.Current.ForeignExportConnections.DeleteObject(conn);

                        if (model.SelectedItem.Connections.Count > 0)
                        {
                            model.SelectedDetailItem = model.SelectedItem.Connections.FirstOrDefault().ExportDetails.FirstOrDefault();
                        }
                    }
                }
            }


        }


    }


}
