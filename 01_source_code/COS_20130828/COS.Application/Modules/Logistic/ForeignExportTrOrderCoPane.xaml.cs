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


namespace COS.Application.Modules.Logistic
{
    /// <summary>
    /// Interaction logic for ForeignExportTrOrderCoPane.xaml
    /// </summary>
    public partial class ForeignExportTrOrderCoPane
    {
        public ForeignExportTrOrderCoPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_ForeignExportTrOrdCoPane");
            Loaded += new RoutedEventHandler(ForeignExportTrOrderCoPane_Loaded);
            GotFocus += new RoutedEventHandler(ForeignExportTrOrderCoPane_GotFocus);
        }

        void ForeignExportTrOrderCoPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void ForeignExportTrOrderCoPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new TransportOrdesCoView(null);
            }
        }
    }
}
