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
    /// Interaction logic for LogisticReport2Pane.xaml
    /// </summary>
    public partial class LogisticReport2Pane
    {
        public LogisticReport2Pane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_LogisticReport2Pane");
            Loaded += new RoutedEventHandler(LogisticReport2Pane_Loaded);
            GotFocus += new RoutedEventHandler(LogisticReport2Pane_GotFocus);
            //Unloaded += new RoutedEventHandler(LogisticReport2Pane_Unloaded);
        }

        //void LogisticReport2Pane_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    KPIWcsReportView view = this.Content as KPIWcsReportView;
        //    if (view != null)
        //    {
        //        view.Dispose();
        //        this.Content = null;
        //    }
        //}

        void LogisticReport2Pane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void LogisticReport2Pane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new LogisticBaseReport2View();
            }
        }
    }
}
