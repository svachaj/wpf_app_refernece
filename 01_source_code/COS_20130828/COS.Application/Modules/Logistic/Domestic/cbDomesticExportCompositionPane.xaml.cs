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
using COS.Application.Logistics.Views.Domestic;
using COS.Application.Shared;


namespace COS.Application.Modules.Logistic.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticExportCompositionPane.xaml
    /// </summary>
    public partial class cbDomesticExportCompositionPane
    {
        public cbDomesticExportCompositionPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbDomesticExportCompositionPane");
            Loaded += new RoutedEventHandler(cbDomesticExportCompositionPane_Loaded);
            GotFocus += new RoutedEventHandler(cbDomesticExportCompositionPane_GotFocus);
        }

        void cbDomesticExportCompositionPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbDomesticExportCompositionPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new cbDomesticCompositionView();
            }
        }
    }
}
