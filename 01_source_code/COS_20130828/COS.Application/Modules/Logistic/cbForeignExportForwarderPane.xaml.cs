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
using System.ComponentModel;

namespace COS.Application.Modules.Logistic
{
    /// <summary>
    /// Interaction logic for cbForeignExportForwarderPane.xaml
    /// </summary>
    public partial class cbForeignExportForwarderPane
    {
        public cbForeignExportForwarderPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbForeignExportForwarderPane");
            Loaded += new RoutedEventHandler(cbForeignExportForwarderPane_Loaded);
          
        }



        void cbForeignExportForwarderPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new cbForwarderView();
            }
        }

        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (newValue && !this.IsHidden)
                {
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Forwarders );

                }
            }
        }
    }
}
