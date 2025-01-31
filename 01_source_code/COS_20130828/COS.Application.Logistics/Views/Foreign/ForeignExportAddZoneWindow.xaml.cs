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
using System.Windows.Shapes;

using Telerik.Windows.Controls;
using COS.Application.Shared;
using COS.Application.Logistics.Models;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for ForeignExportAddZoneWindow.xaml
    /// </summary>
    public partial class ForeignExportAddZoneWindow : RadWindow
    {
        public ForeignExportAddZoneWindow(ForeignDetailViewModel model)
        {
            InitializeComponent();

            this.Closed += new EventHandler<WindowClosedEventArgs>(ForeignExportAddZoneWindow_Closed);
            Model = model;

            btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
            btnSave.Click += new RoutedEventHandler(btnSave_Click);
            grvCodebook.MouseDoubleClick += new MouseButtonEventHandler(grvCodebook_MouseDoubleClick);

            //grvCodebook.ItemsSource = COSContext.Current.ZoneLogistics.Where(a => a.cNumber == null || a.cNumber == string.Empty).ToList();
            //grvCodebook.ItemsSource = COSContext.Current.ZoneLogistics.Where(a => (a.cNumber == null || a.cNumber == string.Empty)).ToList().Where(a=>a.ID_Country == model.SelectedItem.Connections.FirstOrDefault().Destination.ID_Country).ToList();

            allZones = COSContext.Current.ZoneLogistics.Where(a => a.cNumber == null || a.cNumber == string.Empty).ToList();
            grvCodebook.ItemsSource = allZones;

            tbxDestName.Focus();

          

        }

        void grvCodebook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void ForeignExportAddZoneWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                if (grvCodebook.SelectedItem != null)
                {
                    ZoneLogistics zone = grvCodebook.SelectedItem as ZoneLogistics;

                    if (zone != null)
                    {
                        ForeignExportConnection conn = COSContext.Current.ForeignExportConnections.CreateObject();
                        COSContext.Current.ForeignExportConnections.AddObject(conn);

                        conn.Destination = zone;
                        conn.ForeignExport = Model.SelectedItem;

                        ForeignExportDetail detail = COSContext.Current.ForeignExportDetails.CreateObject();
                        COSContext.Current.ForeignExportDetails.AddObject(detail);

                        detail.Connection = conn;

                        Model.SelectedItem.Connections.Add(conn);
                        Model.SelectedDetailItem = detail;

                        

                        
                    }
                }
            }
        }

        ForeignDetailViewModel Model { set; get; }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string val = tbxDestName.Text;

            if (!string.IsNullOrEmpty(val))
            {
                var newlist = allZones.Where(a => a.DestinationName.ToLower().Contains(val.ToLower())).ToList();

                grvCodebook.ItemsSource = newlist;

                if (newlist.Count > 0)
                    grvCodebook.SelectedItem = newlist.FirstOrDefault();
            }
            else
                grvCodebook.ItemsSource = allZones;
        }

        private void tbxDestName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && grvCodebook.SelectedItem != null)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        List<ZoneLogistics> allZones;
    }
}
