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
using COS.Application.Logistics.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;
using Telerik.Windows.Controls.GridView;
using COS.Direction;
using System.Reflection;
using System.Windows.Controls.Primitives;
using COS.Application.Logistics.Views.Domestic;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class DomesticExportDetailView : BaseUserControl
    {
        public DomesticExportDetailView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new DomesticExportDetailViewModel();
                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ForeignsView_Loaded);
                this.Unloaded += new RoutedEventHandler(DomesticExportDetailView_Unloaded);

                this.SizeChanged += new SizeChangedEventHandler(DomesticExportDetailView_SizeChanged);
            }
        }

        void DomesticExportDetailView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (RaiseWindow != null)
            //    RaiseWindow.Width = this.Width - 30;
        }

        void DomesticExportDetailView_Unloaded(object sender, RoutedEventArgs e)
        {
            imgmap.Source = null;
        }

        public override void RefreshData()
        {

        }

        public DomesticExportDetailViewModel model;

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
                    LoadMapRoute();
                    RefreshDetailsGird(null);
                }
            }
            else if (e.PropertyName == "RaiseErrors")
            {
                if (model.SelectedItem != null && !string.IsNullOrEmpty(model.RaiseErrors))
                {
                    RadWindow.Alert(new DialogParameters() { Content = model.RaiseErrors, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    //RadWindows.Alert("Chyba", model.RaiseErrors, RaiseWindow);
                }
            }
            else if (e.PropertyName == "RaiseConfirm")
            {
                if (model.SelectedItem != null && !string.IsNullOrEmpty(model.RaiseConfirm))
                {
                    RadWindow.Confirm(new DialogParameters() { Content = model.RaiseConfirm, Header = ResourceHelper.GetResource<string>("m_Body_LOG00000026"), OkButtonContent = ResourceHelper.GetResource<string>("m_Body_LOG00000027"), CancelButtonContent = ResourceHelper.GetResource<string>("m_Body_LOG00000028"), Owner = RaiseWindow, Closed = new EventHandler<WindowClosedEventArgs>(confirmWindow_Closed) });


                }
            }
            else if (e.PropertyName == "ReloadMap")
            {
                if (model.SelectedItem != null)
                {
                    LoadMapRoute();
                }
            }
            else if (e.PropertyName == "DetailsRefresh")
            {
                grvDetails.Rebind();
            }

        }

        void confirmWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                model.canSave = false;
            }
            else
            {
                model.canSave = true;
                model.Save(true);
            }
        }


        void ForeignsView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void countryRadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadComboBox cmb = sender as RadComboBox;

            if (cmb != null)
            {
                if (cmb.SelectedItem != null)
                {
                    Country cnt = cmb.SelectedItem as Country;
                    if (cnt != null)
                    {
                        model.LocalZoneLogistics = COSContext.Current.DomesticDestinations.Where(a => a.ID_country == cnt.ID).OrderBy(a => a.DestinationName).ToList();
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            model.AddDetail();
            RefreshDetailsGird(model.SelectedItem.ExportDetails.LastOrDefault());
        }



        public void LoadMapRoute()
        {

            try
            {
                if (model.SelectedItem != null && model.SelectedItem.PointOfOrigin != null && model.SelectedItem.ExportDetails.Count > 0 && model.SelectedItem.ExportDetails.First().Destination != null)
                {
                    var imgs = COS.Direction.DistanceClass.GetMapImage(model.SelectedItem.PointOfOrigin.DistanceComputeString, model.SelectedItem.ExportDetails.OrderBy(a => a.DestinationOrder).Select(a => a.Destination.DistanceComputeString).ToList(), model.credUserName, model.credPassword, model.credDomain, model.credProxy);

                    imgmap.Source = imgs;
                    imgToolTipMap.Source = imgs;
                }
            }
            catch (Exception exc)
            {
                //Logging.LogException(exc, LogType.ToFileAndEmail);
            }
        }



        public RadWindow RaiseWindow { get; set; }

        private void btnDelContact_Click(object sender, RoutedEventArgs e)
        {
            if (model.SelectedDetailItem != null)
            {
                RadButton btn = sender as RadButton;

                if (btn != null)
                {
                    DomesticDetailContact cnt = btn.DataContext as DomesticDetailContact;

                    if (cnt != null)
                    {
                        model.SelectedDetailItem.Contacts.Remove(cnt);
                        COSContext.Current.DomesticDetailContacts.DeleteObject(cnt);
                    }
                }
                model.RefreshContantsToAdd();
            }
        }

        private void btnDownOrder_click(object sender, RoutedEventArgs e)
        {
            var prevdet = model.SelectedDetailItem;

            model.ChangeDetailOrder(model.SelectedDetailItem, 2);
            RefreshDetailsGird(null);

            model.SelectedDetailItem = prevdet;
        }

        private void btnUpOrder_click(object sender, RoutedEventArgs e)
        {
            var prevdet = model.SelectedDetailItem;

            model.ChangeDetailOrder(model.SelectedDetailItem, 1);
            RefreshDetailsGird(null);

            model.SelectedDetailItem = prevdet;
        }

        private void RefreshDetailsGird(DomesticExportDetail detail)
        {
            grvDetails.ItemsSource = null;
            grvDetails.ItemsSource = model.Details;
            if (detail != null)
                grvDetails.SelectedItem = detail;
            else
                grvDetails.SelectedItem = model.Details.FirstOrDefault();
        }

        private void deleteSelectedItemClick(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;
            if (btn != null)
            {
                DomesticExportDetail det = btn.DataContext as DomesticExportDetail;
                if (det != null)
                    model.RemoveDetail(det);
                RefreshDetailsGird(null);
            }
        }

        private void RadComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (model.SelectedDetailItem != null)
            {
                if (e.Key == Key.Enter)
                {
                    RadComboBox cmb = sender as RadComboBox;

                    DomesticCustomerContact cont = cmb.SelectedItem as DomesticCustomerContact;

                    if (cont != null)
                    {
                        model.SelectedDetailItem.Contacts.Add(new DomesticDetailContact() { CustomerContact = cont, ExportDetail = model.SelectedDetailItem });
                        model.RefreshContantsToAdd();
                    }
                }
            }
        }

        private void btnAddContact_click(object sender, RoutedEventArgs e)
        {
            if (model.SelectedDetailItem != null)
            {

                DomesticCustomerContact cont = cmbContants.SelectedItem as DomesticCustomerContact;

                if (cont != null)
                {
                    model.SelectedDetailItem.Contacts.Add(new DomesticDetailContact() { CustomerContact = cont, ExportDetail = model.SelectedDetailItem });
                    model.RefreshContantsToAdd();
                }
            }
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            //ToggleButton btn = sender as ToggleButton;

            //if (btn.IsChecked.HasValue && btn.IsChecked.Value)
            //    borderMaps.Visibility = System.Windows.Visibility.Visible;
            //else
            //    borderMaps.Visibility = System.Windows.Visibility.Collapsed;

        }

        cbDomesticDestinationDetailView detailView;
        RadWindow DetailWindow;
        private void btnAddDestination_click(object sender, RoutedEventArgs e)
        {           
            if (DetailWindow == null)
            {
                DetailWindow = new RadWindow();

                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_cbDomesticDestination");
                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                detailView = new cbDomesticDestinationDetailView(null);
                detailView.RaiseWindow = DetailWindow;
                DetailWindow.Content = detailView;

                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
            }

            DomesticDestination item = detailView.Model.CreateNewItem();

            detailView.Model.SelectedItem = item;
            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindow.ShowDialog();
        }

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                var newItem = COSContext.Current.DomesticDestinations.FirstOrDefault(a => a.ID == detailView.Model.SelectedItem.ID);

                model.LocalDetailDestinations.Add(newItem);
                if (detailView.Model.SelectedItem.IsPointOfOrigin)
                    model.LocalZoneLogistics.Add(newItem);
            }
            else
            {
                COSContext.Current.ObjectStateManager.ChangeObjectState(detailView.Model.SelectedItem, System.Data.EntityState.Detached);
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
                var newItem = COSContext.Current.DomesticCustomers.FirstOrDefault(a => a.ID == detailViewCust.Model.SelectedItem.ID);

                model.LocalCustomers.Add(newItem);
                //COSContext.Current.ObjectStateManager.GetObjectStateEntry(detailViewCust.Model.SelectedItem).AcceptChanges();
            }
            else
            {
                COSContext.Current.ObjectStateManager.ChangeObjectState(detailViewCust.Model.SelectedItem, System.Data.EntityState.Detached);
            }
        }

        private void btnEditSOS_click(object sender, RoutedEventArgs e)
        {
            if (grdSos.Visibility == System.Windows.Visibility.Visible)
            {
                grdSos.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                grdSos.Visibility = System.Windows.Visibility.Visible;
            }

        }

        private void grdSos_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            if (model.SelectedDetailItem.Customer != null)
            {

            }
            else
            {
                e.Cancel = true;
            }

        }

        private void btnDelSO_Click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;
            DomesticDetailTrpSo item = btn.DataContext as DomesticDetailTrpSo;

            if (item != null && item.SO != null)
            {
                model.SelectedDetailItem.TrpSos.Remove(item);
                COSContext.Current.DomesticDetailTrpSoes.DeleteObject(item);
            }
        }

        private void grdSos_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction == GridViewEditAction.Commit && e.EditOperationType == GridViewEditOperationType.Insert)
            {
                DomesticDetailTrpSo newItem = e.EditedItem as DomesticDetailTrpSo;

                if (newItem.SO != null)
                {
                    newItem.TRP = model.SelectedDetailItem.TRP;
                    newItem.ExportDetail = model.SelectedDetailItem;

                    COSContext.Current.DomesticDetailTrpSoes.AddObject(newItem);
                }
                else 
                {

                }
            }
            else if (e.EditAction == GridViewEditAction.Cancel)
            {
                e.Handled = true;
            }
        }


    }


}
