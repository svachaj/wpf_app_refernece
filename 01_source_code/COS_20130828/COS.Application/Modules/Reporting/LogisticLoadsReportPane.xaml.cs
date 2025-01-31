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
    /// Interaction logic for LogisticĹoadsReportPane.xaml
    /// </summary>
    public partial class LogisticLoadsReportPane
    {
        public LogisticLoadsReportPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_LogisticLoadsReportPane");
            Loaded += new RoutedEventHandler(LogisticLoadsReportPane_Loaded);
            GotFocus += new RoutedEventHandler(LogisticLoadsReportPane_GotFocus);
            Unloaded += new RoutedEventHandler(LogisticLoadsReportPane_Unloaded);
        }

        void LogisticLoadsReportPane_Unloaded(object sender, RoutedEventArgs e)
        {
           
        }

        void LogisticLoadsReportPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void LogisticLoadsReportPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new LogisticDomesticLoadsReportView(null);
              
                
            }
        }
    }
}
