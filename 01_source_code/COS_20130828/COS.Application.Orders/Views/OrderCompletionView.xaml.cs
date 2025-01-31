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
using COS.Common.WPF;
using COS.Application.Shared;
using COS.Application.Orders.Models;
using System.ComponentModel;
using Telerik.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using COS.Common;

namespace COS.Application.Orders.Views
{
    /// <summary>
    /// Interaction logic for DowntimesDetailView.xaml
    /// </summary>
    public partial class OrderCompletionView : BaseUserControl
    {
        public OrderCompletionView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new OrderCompletionViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
                this.Loaded += new RoutedEventHandler(OrderCompletionView_Loaded);
            }

            //this.Language = 
        }

        void OrderCompletionView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }


        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cancel")
            {

            }
            else if (e.PropertyName == "Save")
            {
                Model.RefreshOrders();
                RefreshData();
            }
        }

        public OrderCompletionViewModel Model = null;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Model.AddNew();
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            Model.Cancel();
        }

        private void RadButton_Click_2(object sender, RoutedEventArgs e)
        {
            Model.Save();
        }



        private void RadButton_Click_5(object sender, RoutedEventArgs e)
        {
            Model.SelectedFilterStatus = null;
        }

        private void btnrefresh_click(object sender, RoutedEventArgs e)
        {
            Model.RefreshOrders();
            RefreshData();
        }
        private void RefreshData()
        {
            grdOrders.ItemsSource = null;
            grdOrders.ItemsSource = Model.LocalOrders;
            grdOrders.Rebind();
        }


        private void grdOrders_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.DataElement != null)
            {
                Order order = e.DataElement as Order;

                if (order != null)
                {
                    if (!string.IsNullOrEmpty(order.Status.Color))
                    {
                        e.Row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(order.Status.Color));
                    }
                }
            }
        }

        private void RadButtonReject_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedItem != null)
            {
                RejectOrderWindow wnd = new RejectOrderWindow(Model.SelectedItem);
                wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
                wnd.ShowDialog();
            }


        }

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            RejectOrderWindow wnd = sender as RejectOrderWindow;

            if (wnd != null)
            {
                if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
                {
                    Model.Save();
                }
            }
        }

        private void btnsavefile_click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(Model.SelectedItem.Attachment))
                {
                    SaveFileDialog sfd = new SaveFileDialog();

                    sfd.FileName = Model.SelectedItem.Attachment;

                    var showed = sfd.ShowDialog();

                    if (showed.HasValue && showed.Value)
                    {
                        File.Copy(COSContext.Current.OrdersFilesPath + Model.SelectedItem.Attachment, sfd.FileName, true);
                    }
                }
            }
        }

        private void btnpreview_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.SelectedItem.Attachment))
                {
                    Process.Start(COSContext.Current.OrdersFilesPath + Model.SelectedItem.Attachment);
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
            }
        }

    }
}
