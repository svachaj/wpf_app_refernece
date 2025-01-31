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
    /// Interaction logic for cbOrderExecuteUsrPane.xaml
    /// </summary>
    public partial class cbOrderExecuteUsrPane
    {
        public cbOrderExecuteUsrPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_OrderExecutePane");
            Loaded += new RoutedEventHandler(cbOrderExecuteUsrPane_Loaded);
            GotFocus += new RoutedEventHandler(cbOrderExecuteUsrPane_GotFocus);
        }

        void cbOrderExecuteUsrPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbOrderExecuteUsrPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new HrDepartmentUserView();
            }
        }
    }
}
