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
    /// Interaction logic for ShiftFollowUpPane.xaml
    /// </summary>
    public partial class ShiftFollowUpRangePane
    {
        public ShiftFollowUpRangePane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_ShiftFollowUpRangePane");
            Loaded += new RoutedEventHandler(ShiftFollowUpPane_Loaded);
            GotFocus += new RoutedEventHandler(ShiftFollowUpPane_GotFocus);
        }

        void ShiftFollowUpPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void ShiftFollowUpPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new ADSProductionWCShiftRangeView();
            }
        }
    }
}
