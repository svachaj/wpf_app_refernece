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
using COS.Application.Shared;
using System.ComponentModel;
using COS.Application.Production.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;
using Telerik.Windows.Controls.GridView;

using System.Reflection;
using System.Windows.Controls.Primitives;
using COS.Application.Logistics.Views.Domestic;
using COS.Application.Production.Views.PlanningVA4H;

namespace COS.Application.Production.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class PlanningVA4HDetailView : BaseUserControl
    {
        public PlanningVA4HDetailView(COSContext datacontext)
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                dataContext = datacontext;
                model = new PlanningVA4HDetailViewModel(datacontext);
                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

            }
        }

        COSContext dataContext;

        public override void RefreshData()
        {

        }

        public PlanningVA4HDetailViewModel model;
        public RadWindow RaiseWindow;

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cancel")
            {
                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = false;
                    RaiseWindow.Close();
                }
            }
            else if (e.PropertyName == "Save")
            {
                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = true;
                    RaiseWindow.Close();
                }

            }
            else if (e.PropertyName == "SelectedItem")
            {
                if (model.SelectedItem != null)
                {
                    if (model.SelectedItem.ID == 0)
                    {
                        model.SelectedItem.RefreshAllValidations();
                    }
                    RefreshDetailsGird(null);

                    if (model.SelectedItem.Customer != null)
                    {
                        grdOperators.ItemsSource = null;
                        grdOperators.ItemsSource = model.SelectedItem.Customer.Contacts;
                    }
                    else 
                    {
                        grdOperators.ItemsSource = null;
                    }
                }
            }
            else if (e.PropertyName == "RaiseErrors")
            {
                if (model.SelectedItem != null && !string.IsNullOrEmpty(model.RaiseErrors))
                {
                    RadWindow.Alert(new DialogParameters() { Content = model.RaiseErrors, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
            else if (e.PropertyName == "RaiseConfirm")
            {
                if (model.SelectedItem != null && !string.IsNullOrEmpty(model.RaiseConfirm))
                {
                    RadWindow.Confirm(new DialogParameters() { Content = model.RaiseConfirm, Header = "Upozornění", OkButtonContent = "Pokračovat v editaci", CancelButtonContent = "Uložit a zavřít", Owner = RaiseWindow, Closed = new EventHandler<WindowClosedEventArgs>(confirmWindow_Closed) });


                }
            }
            else if (e.PropertyName == "DetailsRefresh")
            {
                grvDetails.Rebind();
            }
            else if (e.PropertyName == "Contacts")
            {
                if (model.SelectedItem.Customer != null)
                {
                    grdOperators.ItemsSource = null;
                    grdOperators.ItemsSource = model.SelectedItem.Customer.Contacts;
                }
            }




        }

        void confirmWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {

            }
            else
            {
                model.Save();
            }
        }



        private void countryRadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            model.AddDetail();
        }



        private void RefreshDetailsGird(VA4HAccessories detail)
        {
            grvDetails.ItemsSource = null;
            grvDetails.ItemsSource = model.LocalDetails;
            if (detail != null)
                grvDetails.SelectedItem = detail;
            else
                grvDetails.SelectedItem = model.LocalDetails.FirstOrDefault();
        }

        private void deleteSelectedItemClick(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;
            if (btn != null)
            {
                VA4HAccessories det = btn.DataContext as VA4HAccessories;
                model.RemoveDetail(det);
                RefreshDetailsGird(null);
            }
        }

        cbDomesticCustomerContactsDetailView detailViewCust;
        RadWindow DetailWindowCust;
        private void btnAddCustomer_click(object sender, RoutedEventArgs e)
        {
            if (DetailWindowCust == null)
            {
                DetailWindowCust = new RadWindow();

                DetailWindowCust.ResizeMode = ResizeMode.CanMinimize;
                DetailWindowCust.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindowCust_Closed);
                DetailWindowCust.Header = COS.Resources.ResourceHelper.GetResource<string>("log_DomesticCustomer");
                DetailWindowCust.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                detailViewCust = new cbDomesticCustomerContactsDetailView(null);
                detailViewCust.RaiseWindow = DetailWindowCust;
                DetailWindowCust.Content = detailViewCust;

                StyleManager.SetTheme(DetailWindowCust, new Expression_DarkTheme());
            }

            DomesticCustomer item = detailViewCust.Model.CreateNewItem();
            detailViewCust.Model.SelectedItem = item;
            DetailWindowCust.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindowCust.ShowDialog();
        }

        void DetailWindowCust_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                var newItem = dataContext.DomesticCustomers.FirstOrDefault(a => a.ID == detailViewCust.Model.SelectedItem.ID);

                model.LocalCustomers.Add(newItem);
            }
            else
            {
                dataContext.ObjectStateManager.ChangeObjectState(detailViewCust.Model.SelectedItem, System.Data.EntityState.Detached);
            }
        }

        cbProdSupplierDetailView detailViewSuppl;
        RadWindow DetailWindowSuppl;
        private void btnAddSupplier_click(object sender, RoutedEventArgs e)
        {
            if (DetailWindowSuppl == null)
            {
                DetailWindowSuppl = new RadWindow();

                DetailWindowSuppl.ResizeMode = ResizeMode.CanMinimize;
                DetailWindowSuppl.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindowSuppl_Closed);
                DetailWindowSuppl.Header = COS.Resources.ResourceHelper.GetResource<string>("log_DomesticCustomer");
                DetailWindowSuppl.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                detailViewSuppl = new cbProdSupplierDetailView(null);
                detailViewSuppl.RaiseWindow = DetailWindowSuppl;
                DetailWindowSuppl.Content = detailViewSuppl;

                StyleManager.SetTheme(DetailWindowSuppl, new Expression_DarkTheme());
            }

            ProdPlanSupplier item = detailViewSuppl.Model.CreateNewItem();
            detailViewSuppl.Model.SelectedItem = item;
            DetailWindowSuppl.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindowSuppl.ShowDialog();
        }

        void DetailWindowSuppl_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                dataContext.Refresh(System.Data.Objects.RefreshMode.StoreWins, dataContext.SysLocalizes.Where(a => a.ID == detailViewSuppl.Model.SelectedItem.SysLocalize.ID));
                var newItem = dataContext.ProdPlanSuppliers.FirstOrDefault(a => a.ID == detailViewSuppl.Model.SelectedItem.ID);

                model.LocalSuppliers.Add(newItem);
            }
            else
            {
                dataContext.ObjectStateManager.ChangeObjectState(detailViewSuppl.Model.SelectedItem, System.Data.EntityState.Detached);
            }
        }







    }


}
