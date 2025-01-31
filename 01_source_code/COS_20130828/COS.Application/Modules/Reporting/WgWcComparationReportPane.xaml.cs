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
    /// Interaction logic for WgWcComparationReportPane.xaml
    /// </summary>
    public partial class WgWcComparationReportPane
    {
        public WgWcComparationReportPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_WgWcComparationReportPane");
            Loaded += new RoutedEventHandler(WgWcComparationReportPane_Loaded);
            GotFocus += new RoutedEventHandler(WgWcComparationReportPane_GotFocus);
            Unloaded += new RoutedEventHandler(WgWcComparationReportPane_Unloaded);
        }

        void WgWcComparationReportPane_Unloaded(object sender, RoutedEventArgs e)
        {
            KPIWcWgReportView view = this.Content as KPIWcWgReportView;
            if (view != null)
            {
                view.Dispose();
                this.Content = null;
            }
        }

        void WgWcComparationReportPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void WgWcComparationReportPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new KPIWcWgReportView();
            }
        }
    }
}
