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
    /// Interaction logic for LogisticForwarderReportPane.xaml
    /// </summary>
    public partial class LogisticForwarderReportPane
    {
        public LogisticForwarderReportPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_LogisticForwarderReportPane");
            Loaded += new RoutedEventHandler(LogisticForwarderReportPane_Loaded);
            GotFocus += new RoutedEventHandler(LogisticForwarderReportPane_GotFocus);
            Unloaded += new RoutedEventHandler(LogisticForwarderReportPane_Unloaded);
        }

        void LogisticForwarderReportPane_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        void LogisticForwarderReportPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void LogisticForwarderReportPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new LogisticDomesticForwarderReportView(null);
            }
        }
    }
}
