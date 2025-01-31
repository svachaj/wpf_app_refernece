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
using COS.Application.Reporting.Views;
using COS.Application.Shared;


namespace COS.Application.Modules.Reporting
{
    /// <summary>
    /// Interaction logic for cbBonusGroupsPane.xaml
    /// </summary>
    public partial class cbKpiReportPane
    {
        public cbKpiReportPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_KpiReport");
            Loaded += new RoutedEventHandler(cbBonusGroupPane_Loaded);
            GotFocus += new RoutedEventHandler(cbBonusGroupsPane_GotFocus);
            Unloaded += new RoutedEventHandler(cbKpiReportPane_Unloaded);
        }

        void cbKpiReportPane_Unloaded(object sender, RoutedEventArgs e)
        {
            KPIReportView view = this.Content as KPIReportView;
            if (view != null)
            {
                view.Dispose();
                this.Content = null;
            }
        }

        void cbBonusGroupsPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbBonusGroupPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new KPIReportView();
              
            }
        }


    }
}
