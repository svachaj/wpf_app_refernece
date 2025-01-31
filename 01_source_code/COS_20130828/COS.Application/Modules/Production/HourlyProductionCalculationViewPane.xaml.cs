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

namespace COS.Application.Modules.HourlyProduction
{
    /// <summary>
    /// Interaction logic for EmployeePane.xaml
    /// </summary>
    public partial class HourlyProductionCalculationViewPane
    {
        public HourlyProductionCalculationViewPane()
        {

            InitializeComponent();

            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_HP_Recalculation");

            this.Loaded += new RoutedEventHandler(HourlyProductionCalculationViewPane_Loaded);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(HourlyProductionCalculationViewPane_IsVisibleChanged);

        }

        void HourlyProductionCalculationViewPane_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }


        void HourlyProductionCalculationViewPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new HourlyProductionRecalculationView();
              
            }
        }



        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (newValue && !this.IsHidden)
                {
                    //  COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.HourlyProductions);
                    //  COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Divisions);
                    //  COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.WorkCenters);
                    //  COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.WorkGroups);
                    //  COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Shifts);
                    //  COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ShiftTypes);
                    //  COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ShiftPatterns);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Employees);

                    if (this.Content == null)
                    {
                        this.Content = new HourlyProductionRecalculationView();
                       
                    }

                }


            }


        }


    }
}
