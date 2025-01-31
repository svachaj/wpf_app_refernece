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
using COS.Application.Shared;
using System.ComponentModel;
using COS.Application.Production.Views.PlanningVA4H;

namespace COS.Application.Modules.HourlyProduction
{
    /// <summary>
    /// Interaction logic for cbP4hDifficultyViewPane.xaml
    /// </summary>
    public partial class cbP4hDifficultyViewPane
    {
        public cbP4hDifficultyViewPane()
        {

            InitializeComponent();

            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbP4hDifficultyViewPane");

            this.Loaded += new RoutedEventHandler(cbP4hDifficultyViewPane_Loaded);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(cbP4hDifficultyViewPane_IsVisibleChanged);

        }

        void cbP4hDifficultyViewPane_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (!(bool)e.NewValue)
                {
                    COSContext.Current.RejectChanges();
                    //this.Content = null;
                }
            }
        }


        void cbP4hDifficultyViewPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new cbProdDifficultyView (null);               
              
            }
        }



      
    }
}
