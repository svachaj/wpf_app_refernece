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
using COS.Application.Administration.Views;
using COS.Application.Shared;
using System.ComponentModel;

namespace COS.Application.Modules.Administration
{
    /// <summary>
    /// Interaction logic for cbCostCentersPane.xaml
    /// </summary>
    public partial class cbCostCentersPane
    {
        public cbCostCentersPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_CbCostCenter");
            Loaded += new RoutedEventHandler(cbCostCentersPane_Loaded);
          
        }

   

        void cbCostCentersPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new CostCentersView();
            }
        }

        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (newValue && !this.IsHidden)
                {
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.CostCenters);

                }
            }
        }
    }
}
