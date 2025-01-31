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
    /// Interaction logic for cbForeignExportTimeWindowPane.xaml
    /// </summary>
    public partial class cbForeignExportTimeWindowPane
    {
        public cbForeignExportTimeWindowPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbForeignExportTimeWindowPane");
            Loaded += new RoutedEventHandler(cbForeignExportTimeWindowPane_Loaded);
            GotFocus += new RoutedEventHandler(cbForeignExportTimeWindowPane_GotFocus);
        }

        void cbForeignExportTimeWindowPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbForeignExportTimeWindowPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new cbTimeWindowView();
            }
        }
    }
}
