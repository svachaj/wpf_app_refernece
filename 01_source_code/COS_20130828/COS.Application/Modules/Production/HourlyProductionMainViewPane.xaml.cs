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
    public partial class HourlyProductionMainViewPane
    {
        public HourlyProductionMainViewPane()
        {

            InitializeComponent();

            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_HourlyProduction");

            this.Loaded += new RoutedEventHandler(HourlyProductionMainViewPane_Loaded);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(HourlyProductionMainViewPane_IsVisibleChanged);

        }

        void HourlyProductionMainViewPane_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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


        void HourlyProductionMainViewPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
               this.Content = new HourlyProductionMainView();               
            }
        }



        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (newValue && !this.IsHidden)
                {
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.HourlyProductions);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Divisions);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.WorkCenters);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.WorkGroups);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Shifts);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ShiftTypes);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ShiftPatterns);
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Employees);

                    if (this.Content == null)
                        this.Content = new HourlyProductionMainView();

                    HourlyProductionMainView hpv = this.Content as HourlyProductionMainView;

                    if (hpv != null)
                    {
                        hpv.RebindControls();
                    }
                }
                else
                {
                    if (this.Content != null)
                    {
                        COSContext.Current.RejectChanges();
                        //this.Content = null;
                    }
                }
                //else if(!this.IsHidden)
                //{
                //    RadWindow.Confirm(new DialogParameters() { OkButtonContent = "Ano", CancelButtonContent = "Ne", 
                //        Content = "Opravdu si přejete opustit zadávání produkce? Jestli-že ano, neuložená data mohou být ztracena!", Header="Potvrzení", Owner = this , Closed = new EventHandler<WindowClosedEventArgs>(confirm_closed)});


                //}
            }


        }

      
    }
}
