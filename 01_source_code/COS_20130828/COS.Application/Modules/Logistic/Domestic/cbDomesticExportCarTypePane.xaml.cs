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
    /// Interaction logic for cbDomesticExportCarTypePane.xaml
    /// </summary>
    public partial class cbDomesticExportCarTypePane
    {
        public cbDomesticExportCarTypePane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbDomesticExportCarTypePane");
            Loaded += new RoutedEventHandler(cbDomesticExportCarTypePane_Loaded);
            GotFocus += new RoutedEventHandler(cbDomesticExportCarTypePane_GotFocus);
        }

        void cbDomesticExportCarTypePane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbDomesticExportCarTypePane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new cbDomesticCarTypeView();
            }
        }
    }
}
