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
    /// Interaction logic for AdsEmpProductivityReportPane.xaml
    /// </summary>
    public partial class AdsEmpProductivityReportPane
    {
        public AdsEmpProductivityReportPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_AdsEmpProductivityReportPane");
            Loaded += new RoutedEventHandler(AdsEmpProductivityReportPane_Loaded);
            GotFocus += new RoutedEventHandler(AdsEmpProductivityReportPane_GotFocus);
        }

        void AdsEmpProductivityReportPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void AdsEmpProductivityReportPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new ProductionEmployeesView();
            }
        }
    }
}
