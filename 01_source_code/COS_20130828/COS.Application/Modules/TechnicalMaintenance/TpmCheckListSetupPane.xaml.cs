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
using COS.Application.TechnicalMaintenance.Views;
using COS.Application.Shared;


namespace COS.Application.Modules.TechnicalMaintenance
{
    /// <summary>
    /// Interaction logic for TpmCheckListSetupPane.xaml
    /// </summary>
    public partial class TpmCheckListSetupPane
    {
        public TpmCheckListSetupPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_TpmChecklistItems");
            Loaded += new RoutedEventHandler(TpmCheckListSetupPane_Loaded);
            GotFocus += new RoutedEventHandler(TpmCheckListSetupPane_GotFocus);
        }

        void TpmCheckListSetupPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void TpmCheckListSetupPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new TpmChecklistItemsView();
            }
        }
    }
}
