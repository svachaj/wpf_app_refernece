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
using COS.Resources;
using COS.Application.Production.Views;
using COS.Application.Engeneering.Views;
using COS.Application.Shared;
using System.ComponentModel;

namespace COS.Application.Modules.HourlyProduction
{
    /// <summary>
    /// Interaction logic for AdsCustomerService.xaml
    /// </summary>
    public partial class AdsCustomerServiceViewPane
    {
        public AdsCustomerServiceViewPane()
        {

            InitializeComponent();

            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_PROD_AdsCustomerService");

            this.Loaded += new RoutedEventHandler(AdsCustomerServiceViewPane_Loaded);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(AdsCustomerServiceViewPane_IsVisibleChanged);

        }

        void AdsCustomerServiceViewPane_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (!(bool)e.NewValue)
                {
                    COSContext.Current.RejectChanges();
                }
            }
        }


        void AdsCustomerServiceViewPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new AdsCustomerServiceView();
            }
        }



        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (newValue && !this.IsHidden)
                {
                   
                    if (this.Content == null)
                        this.Content = new AdsCustomerServiceView();
                   
                }
               
               
            }


        }

      
    }
}
