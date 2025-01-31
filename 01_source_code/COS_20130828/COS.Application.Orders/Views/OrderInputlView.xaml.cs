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
using COS.Common;
using System.Diagnostics;

namespace COS.Application.Orders.Views
{
    /// <summary>
    /// Interaction logic for DowntimesDetailView.xaml
    /// </summary>
    public partial class OrderInputlView : BaseUserControl
    {
        public OrderInputlView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new OrderInputlViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
                this.Loaded += new RoutedEventHandler(OrderInputlView_Loaded);
            }
        }

        void OrderInputlView_Loaded(object sender, RoutedEventArgs e)
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

        private void RefreshData()
        {
            grdOrders.ItemsSource = null;
            grdOrders.ItemsSource = Model.LocalOrders;
            grdOrders.Rebind();
        }

        public OrderInputlViewModel Model = null;

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

        private void RadButton_Click_3(object sender, RoutedEventArgs e)
        {
            grdDetails.BeginInsert();
        }

        private void RadButton_Click_4(object sender, RoutedEventArgs e)
        {
            if (grdDetails.SelectedItem != null)
                Model.RemoveDetail(grdDetails.SelectedItem as OrderDetail);
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

        private void btnloadattachment_click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedItem != null)
            {
                OpenFileDialog ofs = new OpenFileDialog();

                var showed = ofs.ShowDialog();

                if (showed.HasValue && showed.Value)
                {
                    if (File.Exists(COSContext.Current.OrdersFilesPath + ofs.SafeFileName))
                    {
                        int i = 2;
                        string newname = i.ToString() + ofs.SafeFileName;

                        while (File.Exists(COSContext.Current.OrdersFilesPath + newname))
                        {
                            i++;
                            newname = i.ToString() + ofs.SafeFileName;
                        }

                        Model.SelectedItem.Attachment = newname;

                    }
                    else
                        Model.SelectedItem.Attachment = ofs.SafeFileName;

                    File.Copy(ofs.FileName, COSContext.Current.OrdersFilesPath + Model.SelectedItem.Attachment, true);
                }
            }
        }

        private void btndelete_click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedItem != null)
                Model.SelectedItem.Attachment = null;
        }


    }
}
