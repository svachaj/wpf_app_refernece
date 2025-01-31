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
using COS.Application.Logistics.Views;
using COS.Application.Orders.Views;
using COS.Application.Shared;


namespace COS.Application.Modules.Orders
{
    /// <summary>
    /// Interaction logic for OrderCompletionPane.xaml
    /// </summary>
    public partial class OrderCompletionPane
    {
        public OrderCompletionPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_OrderCompletion");
            Loaded += new RoutedEventHandler(OrderCompletionPane_Loaded);
            GotFocus += new RoutedEventHandler(OrderCompletionPane_GotFocus);
        }

        void OrderCompletionPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void OrderCompletionPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new OrderCompletionView();
            }
        }
    }
}
