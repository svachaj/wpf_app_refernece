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
using COS.Application.Shared;
using COS.Application.Logistics.Views.Domestic;


namespace COS.Application.Modules.Logistic.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticExportCustomerPane.xaml
    /// </summary>
    public partial class cbDomesticExportCustomerPane
    {
        public cbDomesticExportCustomerPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbDomesticExportCustomerPane");
            Loaded += new RoutedEventHandler(cbDomesticExportCustomerPane_Loaded);
            GotFocus += new RoutedEventHandler(cbDomesticExportCustomerPane_GotFocus);
        }

        void cbDomesticExportCustomerPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbDomesticExportCustomerPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new cbDomesticCustomerContactsView();
            }
        }
    }
}
