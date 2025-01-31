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
using COS.Application.Shared;
using Telerik.Windows.Controls;

namespace COS.Application.Orders.Views
{
    /// <summary>
    /// Interaction logic for RejectOrderWindow.xaml
    /// </summary>
    public partial class RejectOrderWindow : RadWindow
    {
        public RejectOrderWindow(Order order)
        {
            InitializeComponent();

            MyOrder = order;
            if (MyOrder != null)
                tbMain.Text = MyOrder.RejectedNote;
           
        }

        public Order MyOrder { set; get; }

        private void btnOK_click(object sender, RoutedEventArgs e)
        {
          
                MyOrder.RejectedNote = tbMain.Text;
                MyOrder.RejectedDate = DateTime.Today;
                MyOrder.RejectedBy = COSContext.Current.CurrentUser;
                MyOrder.Status = COSContext.Current.OrderStatus.FirstOrDefault(a => a.Code == "D");
                this.DialogResult = true;
                this.Close();
        }

        private void btnCancel_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.CloseWithoutEventsAndAnimations();
        }
    }
}
