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
    /// Interaction logic for cbOrderApproveUsrPane.xaml
    /// </summary>
    public partial class cbOrderApproveUsrPane
    {
        public cbOrderApproveUsrPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbOrderApproveUsrPane");
            Loaded += new RoutedEventHandler(cbOrderApproveUsrPane_Loaded);
            GotFocus += new RoutedEventHandler(cbOrderApproveUsrPane_GotFocus);
        }

        void cbOrderApproveUsrPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbOrderApproveUsrPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new HrDepartmentUserApproveView();
            }
        }
    }
}
