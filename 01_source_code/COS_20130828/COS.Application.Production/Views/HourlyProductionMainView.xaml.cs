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
using System.Windows.Navigation;
using System.Windows.Shapes;
using COS.Common.WPF;
using System.ComponentModel;
using COS.Application.Shared;
using COS.Application.Production.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;

namespace COS.Application.Production.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class HourlyProductionMainView : BaseUserControl
    {
        public HourlyProductionMainView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new HourlyProductionMainViewModel();

                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                this.Loaded += new RoutedEventHandler(HourlyProductionMainView_Loaded);
            }
        }

        void HourlyProductionMainView_Loaded(object sender, RoutedEventArgs e)
        {
            grdHourlyProductions.UpdateLayout();
        }

        public HourlyProductionMainViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {

            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LocalError")
            {
                if (!string.IsNullOrEmpty(Model.LocalError))
                {
                    RadWindow.Alert(new DialogParameters() { Content = Model.LocalError, Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
            else if (e.PropertyName == "AddNewDowntime")
            {
                grdDowntimes.BeginInsert();
            }
            else if (e.PropertyName == "AddNewOperator")
            {
                grdOperators.BeginInsert();
            }
            else if (e.PropertyName == "Saved")
            {
                txtOrder.Focus();
            }

            else if (e.PropertyName == "AddNew")
            {
                cmbHour.Focus();
            }

            else if (e.PropertyName == "Cycle1")
            {
                txtOrder.Focus();
            }

            else if (e.PropertyName == "Cycle2")
            {
                txtOrder.Focus();
            }

            else if (e.PropertyName == "Cycle3")
            {
                txtOrder.Focus();
            }

            else if (e.PropertyName == "DeleteHp")
            {
                cmbHour.Focus();
            }
            else if (e.PropertyName == "ConfigStandard") 
            {
#warning //odkomentovat pro pouziti configu

                ConfiguratorSelectorWindow wnd = new ConfiguratorSelectorWindow(Model);

                wnd.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }
            
        }


        public void RemoveThemeFromProdsGrid()
        {
          //  StyleManager.SetTheme(grdHourlyProductions, new VistaTheme());
        }

        public void RefreshGridLayout()
        {
            //StyleManager.SetTheme(grdHourlyProductions, new Expression_DarkTheme());
            //grdHourlyProductions.UpdateLayout();


            //var isrc = grdHourlyProductions.ItemsSource;

         
        }

        public void RebindControls()
        {
            cmbShifts.ItemsSource = COSContext.Current.Shifts.ToList();
            cmbShiftTypes.ItemsSource = COSContext.Current.ShiftTypes.ToList();
            cmbDivisions.ItemsSource = COSContext.Current.Divisions.ToList();         
        }

        private void btnAddDowntime_click(object sender, RoutedEventArgs e)
        {
            grdDowntimes.BeginInsert();
        }

        private void btnAddOperator_click(object sender, RoutedEventArgs e)
        {
            grdOperators.BeginInsert();
        }

        private void grdOperators_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction == GridViewEditAction.Commit)
            {
                var item = e.EditedItem as ProductionHRResource;
                if (item != null)
                {
                    if (string.IsNullOrEmpty(item.HR_ID_HELP))
                    {
                        Model.ProductionHRResources.Remove(item);
                        grdOperators.Rebind();
                    }
                    else 
                    {
                        item.ID_HR = item.HR_ID_HELP;
                    }
                }
            }
        }

        private void grdDowntimes_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction == GridViewEditAction.Commit)
            {
                var item = e.EditedItem as ProductionAsset;
                if (item != null)
                {
                    if (item.ID_Downtime == 0)
                    {
                        Model.ProductionAssets.Remove(item);
                        grdDowntimes.Rebind();
                    }
                }
            }
        }


        private void cmbOperators_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                grdOperators.BeginInsert();
            }
        }

        private void cmbDowntimes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                grdDowntimes.BeginInsert();
            }

        }

        private void cmbOpers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Employee empl = cmbOpers.SelectedItem as Employee;

                if (empl != null)
                {
                    ProductionHRResource prodhr = new ProductionHRResource();

                    prodhr.ID_HP = Model.SelectedHourlyProduction.ID_HP;
                    prodhr.ID_HR = empl.HR_ID;

                    Model.ProductionHRResources.Add(prodhr);

                    grdOperators.Rebind();

                    cmbOpers.SelectedItem = null;
                }
            }
        }

        private void cmbDowns_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (cmbDowns.SelectedItem != null)
                {
                    numDowns.Value = 0;
                    numDowns.Focus();
                    numDowns.SelectAll();
                }
            }
        }



        private void numDowns_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Downtime dt = cmbDowns.SelectedItem as Downtime;

                if (dt != null)
                {
                    ProductionAsset asset = new ProductionAsset();

                    asset.ID_HP = Model.SelectedHourlyProduction.ID_HP;
                    asset.Downtime = dt;
                    asset.Time_min = (int)numDowns.Value.Value;

                    Model.ProductionAssets.Add(asset);

                    grdDowntimes.Rebind();

                    numDowns.Value = 0;
                    cmbDowns.SelectedItem = null;
                    cmbDowns.Focus();
                }
            }
        }

        private void btnDelOper_Click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            if (btn != null)
            {
                ProductionHRResource oper = btn.DataContext as ProductionHRResource;

                if (oper != null)
                    Model.ProductionHRResources.Remove(oper);

                grdOperators.Rebind();
            }
        }

        private void btnDelAsset_Click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            if (btn != null)
            {
                ProductionAsset asset = btn.DataContext as ProductionAsset;

                if (asset != null)
                    Model.ProductionAssets.Remove(asset);

                grdDowntimes.Rebind();
            }
        }

        private void txtOrder_GotFocus(object sender, RoutedEventArgs e)
        {
            txtOrder.SelectAll() ;

        }

            private void txtItemNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            txtItemNumber.SelectAll();
        }

            private void RadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {

            }

            private void RadComboBox_TextInput(object sender, TextCompositionEventArgs e)
            {

            }



    }
}