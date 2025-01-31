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
using COS.Application.Administration.Views;
using System.IO;
using Telerik.Windows.Controls;
using COS.Application.Shared;
using COS.Application.Production.Views;
using COS.Application.Logistics.Views;
using COS.Application.Orders.Views;
using System.Windows.Media.Animation;




namespace COS.Application.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : BaseUserControl
    {
        public MainView()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainView_Loaded);

            COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

          
        }

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HourlyProductionToNavigate")
            {
                //MessageBox.Show("HP: " + COSContext.Current.HourlyProductionToNavigate.ID.ToString());


                btnHourlyProduction_Click(null, null);

                HourlyProduction hp = COSContext.Current.HourlyProductionToNavigate;

                HourlyProductionMainView view = cbHourlyProductionMainViewPane.Content as HourlyProductionMainView;

                if (view != null && hp != null)
                {
                    view.RemoveThemeFromProdsGrid();

                    if (view.Model.IsSet)
                    {
                        view.Model.IsSet = false;
                        view.Model.SetUnset();
                    }

                    view.Model.SelectedDivision = hp.Division;
                    view.Model.SelectedShift = hp.Shift;
                    view.Model.SelectedShiftType = hp.ShiftType;
                    view.Model.SelectedDate = hp.Date;

                    if (!view.Model.IsSet)
                    {
                        view.Model.IsSet = true;
                        view.Model.SetUnset();
                    }


                    view.Model.SelectedWorkGroup = hp.WorkGroup;
                    view.Model.SelectedWorkCenter = hp.WorkCenter;

                    view.RefreshGridLayout();
                }
            }
            else if (e.PropertyName == "ForeignExportToNavigateID")
            {
                if (COSContext.Current.ForeignExportToNavigateID > 0)
                {
                    ForeignDetailView detailView = new ForeignDetailView();

                    RadWindow DetailWindow = new RadWindow();

                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_ForeignExportDetail");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new ForeignDetailView();
                    detailView.RaiseWindow = DetailWindow;
                    DetailWindow.Content = detailView;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());

                    detailView.model.SelectedItem = COSContext.Current.ForeignExports.FirstOrDefault(a => a.ID == COSContext.Current.ForeignExportToNavigateID);
                    //DetailWindow.IsTopmost = true;
                    DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    DetailWindow.ShowDialog();

                }
            }

            else if (e.PropertyName == "DomesticExportToNavigateID")
            {
                if (COSContext.Current.DomesticExportToNavigateID > 0)
                {
                    DomesticExportDetailView detailView =  null;

                    RadWindow DetailWindow = new RadWindow();

                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_DomesticExportDetail");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new DomesticExportDetailView();
                    detailView.RaiseWindow = DetailWindow;
                    DetailWindow.Content = detailView;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());

                    detailView.model.SelectedItem = COSContext.Current.DomesticExports.FirstOrDefault(a => a.ID == COSContext.Current.DomesticExportToNavigateID);
                    //DetailWindow.IsTopmost = true;
                    DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    DetailWindow.ShowDialog();

                }
            }
        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            //string xml = Helpers.LoadDockingLayout("COS.Application.MainWidnow.Layout");

            //if (!string.IsNullOrEmpty(xml))
            //{
            //    using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            //    {
            //        stream.Seek(0, SeekOrigin.Begin);
            //        this.dckMainDocking.LoadLayout(stream);
            //    }

            //}

            foreach (var tab in radRibbonView1.Items)
            {
                RadRibbonTab rTab = tab as RadRibbonTab;

                if (rTab != null)
                {

                    foreach (var grp in rTab.Items)
                    {
                        RadRibbonGroup group = grp as RadRibbonGroup;

                        if (group != null)
                        {
                            foreach (var button in group.ChildrenOfType<RadRibbonButton>())
                            {
                                //RadRibbonButton button = btn as RadRibbonButton;
                                if (button != null)
                                {
                                    if (button.Visibility == System.Windows.Visibility.Visible)
                                    {
                                        group.Visibility = System.Windows.Visibility.Visible;
                                        break;
                                    }
                                    else
                                    {
                                        group.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }



        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {

            usersPane.IsHidden = false;
            usersPane.Focus();
            usersPane.IsSelected = true;

        }

        private void btnGroups_Click(object sender, RoutedEventArgs e)
        {

            groupsPane.IsHidden = false;
            groupsPane.Focus();
            groupsPane.IsSelected = true;
        }

        private void btnLocalization_Click(object sender, RoutedEventArgs e)
        {

            LocalizationPane.IsHidden = false;
            LocalizationPane.Focus();
            LocalizationPane.IsSelected = true;

        }

        private void btnSecuritySetting_Click(object sender, RoutedEventArgs e)
        {

            SecuritySettingsPane.IsHidden = false;
            SecuritySettingsPane.Focus();
            SecuritySettingsPane.IsSelected = true;

        }

        private void btnCbAccType_Click(object sender, RoutedEventArgs e)
        {
            accTypePane.IsHidden = false;
            accTypePane.Focus();
            accTypePane.IsSelected = true;
        }


        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            employeePane.IsHidden = false;
            employeePane.Focus();
            employeePane.IsSelected = true;
        }

        private void btnCbBonusGroup_Click(object sender, RoutedEventArgs e)
        {
            cbBonusGroupsPane.IsHidden = false;
            cbBonusGroupsPane.Focus();
            cbBonusGroupsPane.IsSelected = true;
        }

        private void btnCbSalaryGroups_Click(object sender, RoutedEventArgs e)
        {
            cbSalaryGroupsPane.IsHidden = false;
            cbSalaryGroupsPane.Focus();
            cbSalaryGroupsPane.IsSelected = true;
        }

        private void btnCbWorkPositions_Click(object sender, RoutedEventArgs e)
        {
            cbWorkPositionsPane.IsHidden = false;
            cbWorkPositionsPane.Focus();
            cbWorkPositionsPane.IsSelected = true;
        }

        private void btnCbEmployers_Click(object sender, RoutedEventArgs e)
        {
            cbEmployersPane.IsHidden = false;
            cbEmployersPane.Focus();
            cbEmployersPane.IsSelected = true;
        }

        private void BaseUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            MemoryStream stream = new MemoryStream();
            this.dckMainDocking.SaveLayout(stream);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);
            string xml = reader.ReadToEnd();

            Helpers.SaveDockingLayout(xml, "COS.Application.MainWidnow.Layout");
        }

        private void btnStandrad_Click(object sender, RoutedEventArgs e)
        {
            standardPane.IsHidden = false;
            standardPane.Focus();
            standardPane.IsSelected = true;
        }

        private void btnCbWorkGroup_Click(object sender, RoutedEventArgs e)
        {
            cbWorkGroupPane.IsHidden = false;
            cbWorkGroupPane.Focus();
            cbWorkGroupPane.IsSelected = true;
        }

        private void btnCbWorkCenter_Click(object sender, RoutedEventArgs e)
        {
            cbWorkCenterPane.IsHidden = false;
            cbWorkCenterPane.Focus();
            cbWorkCenterPane.IsSelected = true;
        }

        private void btnCbDowntimes_Click(object sender, RoutedEventArgs e)
        {
            cbDowntimesPane.IsHidden = false;
            cbDowntimesPane.Focus();
            cbDowntimesPane.IsSelected = true;
        }

        private void btnCbDowntimesGroup_Click(object sender, RoutedEventArgs e)
        {
            cbDowntimesGroupPane.IsHidden = false;
            cbDowntimesGroupPane.Focus();
            cbDowntimesGroupPane.IsSelected = true;
        }

        private void btnCbItemsGroup_Click(object sender, RoutedEventArgs e)
        {
            cbItemsGroupPane.IsHidden = false;
            cbItemsGroupPane.Focus();
            cbItemsGroupPane.IsSelected = true;
        }

        private void btnCbShifts_Click(object sender, RoutedEventArgs e)
        {
            cbShiftsPane.IsHidden = false;
            cbShiftsPane.Focus();
            cbShiftsPane.IsSelected = true;
        }

        private void btnCbShiftsType_Click(object sender, RoutedEventArgs e)
        {
            cbShiftsTypePane.IsHidden = false;
            cbShiftsTypePane.Focus();
            cbShiftsTypePane.IsSelected = true;
        }

        private void btnCbDivisions_Click(object sender, RoutedEventArgs e)
        {
            cbDivisionPane.IsHidden = false;
            cbDivisionPane.Focus();
            cbDivisionPane.IsSelected = true;
        }

        private void btnCbCostCenter_Click(object sender, RoutedEventArgs e)
        {
            cbCostCentersPane.IsHidden = false;
            cbCostCentersPane.Focus();
            cbCostCentersPane.IsSelected = true;
        }

        private void btnCbShiftPatterns_Click(object sender, RoutedEventArgs e)
        {
            cbShiftPatternsPane.IsHidden = false;
            cbShiftPatternsPane.Focus();
            cbShiftPatternsPane.IsSelected = true;
        }

        private void btnCbWorkGroupWorkCenters_Click(object sender, RoutedEventArgs e)
        {
            cbWorkGroupsWorkCentersPane.IsHidden = false;
            cbWorkGroupsWorkCentersPane.Focus();
            cbWorkGroupsWorkCentersPane.IsSelected = true;
        }

        private void btnCbDowntimesGroupDowntimes_Click(object sender, RoutedEventArgs e)
        {
            cbDowntimesGroupDowntimePane.IsHidden = false;
            cbDowntimesGroupDowntimePane.Focus();
            cbDowntimesGroupDowntimePane.IsSelected = true;
        }

        private void btnHourlyProduction_Click(object sender, RoutedEventArgs e)
        {
            cbHourlyProductionMainViewPane.IsHidden = false;
            cbHourlyProductionMainViewPane.Focus();
            cbHourlyProductionMainViewPane.IsSelected = true;
        }

        private void btnFontSizePlus_Click(object sender, RoutedEventArgs e)
        {
            RadPaneGroup group = cbHourlyProductionMainViewPane.Parent as RadPaneGroup;

            if (group != null)
            {
                RadPane pane = group.SelectedPane;

                if (pane != null)
                {
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<TextBlock>())
                    {
                        item.FontSize += 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<TextBox>())
                    {
                        item.FontSize += 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<RadComboBox>())
                    {
                        item.FontSize += 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<CheckBox>())
                    {
                        item.FontSize += 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<RadListBox>())
                    {
                        item.FontSize += 1;
                    }
                }
            }
        }

        private void btnFontSizeMinus_Click(object sender, RoutedEventArgs e)
        {
            RadPaneGroup group = cbHourlyProductionMainViewPane.Parent as RadPaneGroup;

            if (group != null)
            {
                RadPane pane = group.SelectedPane;

                if (pane != null)
                {
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<TextBlock>())
                    {
                        if (item.FontSize > 1)
                            item.FontSize -= 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<TextBox>())
                    {
                        if (item.FontSize > 1)
                            item.FontSize -= 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<RadComboBox>())
                    {
                        if (item.FontSize > 1)
                            item.FontSize -= 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<CheckBox>())
                    {
                        if (item.FontSize > 1)
                            item.FontSize -= 1;
                    }
                    foreach (var item in (pane.Content as BaseUserControl).ChildrenOfType<RadListBox>())
                    {
                        if (item.FontSize > 1)
                            item.FontSize -= 1;
                    }
                }
            }
        }

        private void btnKpiReport_Click(object sender, RoutedEventArgs e)
        {
            cbKpiReportPane.IsHidden = false;
            cbKpiReportPane.Focus();
            cbKpiReportPane.IsSelected = true;
        }

        private void btnHourlyProductionReCalc_Click(object sender, RoutedEventArgs e)
        {
            HourlyProductionCalculationViewPane.IsHidden = false;
            HourlyProductionCalculationViewPane.Focus();
            HourlyProductionCalculationViewPane.IsSelected = true;


        }

        private void btnAdsCustomerService_Click(object sender, RoutedEventArgs e)
        {
            AdsCustomerServiceViewPane.IsHidden = false;
            AdsCustomerServiceViewPane.Focus();
            AdsCustomerServiceViewPane.IsSelected = true;
        }

        private void btnTpmPlan_Click(object sender, RoutedEventArgs e)
        {
            cbTpmPlansPane.IsHidden = false;
            cbTpmPlansPane.Focus();
            cbTpmPlansPane.IsSelected = true;
        }

        private void btnTpmCheckList_Click(object sender, RoutedEventArgs e)
        {
            TpmCheckListPane.IsHidden = false;
            TpmCheckListPane.Focus();
            TpmCheckListPane.IsSelected = true;
        }

        private void btnCbTpmCheckListItems_Click(object sender, RoutedEventArgs e)
        {
            cbTpmCheckListItemsPane.IsHidden = false;
            cbTpmCheckListItemsPane.Focus();
            cbTpmCheckListItemsPane.IsSelected = true;
        }

        private void btnCbTpmEquipment_Click(object sender, RoutedEventArgs e)
        {
            cbTpmEquipmentPane.IsHidden = false;
            cbTpmEquipmentPane.Focus();
            cbTpmEquipmentPane.IsSelected = true;
        }


        private void btnTpmHolidaysCalendar_Click(object sender, RoutedEventArgs e)
        {
            TpmHolidayCalendarPane.IsHidden = false;
            TpmHolidayCalendarPane.Focus();
            TpmHolidayCalendarPane.IsSelected = true;
        }

        private void btnTpmChecklistSetup_Click(object sender, RoutedEventArgs e)
        {
            TpmCheckListSetupPane.IsHidden = false;
            TpmCheckListSetupPane.Focus();
            TpmCheckListSetupPane.IsSelected = true;
        }

        private void btnTpmPlanOverview_Click(object sender, RoutedEventArgs e)
        {

            TpmPlanOVerviewPane.IsHidden = false;
            TpmPlanOVerviewPane.Focus();
            TpmPlanOVerviewPane.IsSelected = true;

        }

        private void btnConfiguratos_Click(object sender, RoutedEventArgs e)
        {

            ConfiguratorsPane.IsHidden = false;
            ConfiguratorsPane.Focus();
            ConfiguratorsPane.IsSelected = true;

        }


        private void btnForeignExport_Click(object sender, RoutedEventArgs e)
        {

            ForeignExportPane.IsHidden = false;
            ForeignExportPane.Focus();
            ForeignExportPane.IsSelected = true;

        }

        private void btnCbForeignBafPrice_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportBafPane.IsHidden = false;
            cbForeignExportBafPane.Focus();
            cbForeignExportBafPane.IsSelected = true;
        }

        private void btnCbForeignForwarder_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportForwarderPane.IsHidden = false;
            cbForeignExportForwarderPane.Focus();
            cbForeignExportForwarderPane.IsSelected = true;
        }

        private void btnCbForeignOrderedBy_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportOrderedByPane.IsHidden = false;
            cbForeignExportOrderedByPane.Focus();
            cbForeignExportOrderedByPane.IsSelected = true;
        }

        private void btnForeignExportPriceList_Click(object sender, RoutedEventArgs e)
        {

            ForeignExportPriceListImportPane.IsHidden = false;
            ForeignExportPriceListImportPane.Focus();
            ForeignExportPriceListImportPane.IsSelected = true;
        }

        private void btnCbForeignPlatform_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportPlatformPane.IsHidden = false;
            cbForeignExportPlatformPane.Focus();
            cbForeignExportPlatformPane.IsSelected = true;
        }

        private void btnCbForeignTimeWindow_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportTimeWindowPane.IsHidden = false;
            cbForeignExportTimeWindowPane.Focus();
            cbForeignExportTimeWindowPane.IsSelected = true;
        }

        private void btnCbForeignTransportPayment_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportTransportPaymentPane.IsHidden = false;
            cbForeignExportTransportPaymentPane.Focus();
            cbForeignExportTransportPaymentPane.IsSelected = true;
        }

        private void btnCbForeignUnit_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportUnitPane.IsHidden = false;
            cbForeignExportUnitPane.Focus();
            cbForeignExportUnitPane.IsSelected = true;
        }

        private void btnCbForeignCountry_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportCountryPane.IsHidden = false;
            cbForeignExportCountryPane.Focus();
            cbForeignExportCountryPane.IsSelected = true;
        }

        private void btnCbForeignZone_Click(object sender, RoutedEventArgs e)
        {
            cbForeignExportZonePane.IsHidden = false;
            cbForeignExportZonePane.Focus();
            cbForeignExportZonePane.IsSelected = true;
        }

        private void btnProductionDetail_Click(object sender, RoutedEventArgs e)
        {
            ProductionDetailReportPane.IsHidden = false;
            ProductionDetailReportPane.Focus();
            ProductionDetailReportPane.IsSelected = true;
        }

        private void btnProductionDetailDtSum_Click(object sender, RoutedEventArgs e)
        {
            ProductionDetailDtSumReportPane.IsHidden = false;
            ProductionDetailDtSumReportPane.Focus();
            ProductionDetailDtSumReportPane.IsSelected = true;
        }

        private void btnDowntimeDetail_Click(object sender, RoutedEventArgs e)
        {
            DowntimeDetailReportPane.IsHidden = false;
            DowntimeDetailReportPane.Focus();
            DowntimeDetailReportPane.IsSelected = true;
        }

        private void btnKpiWcReport_Click(object sender, RoutedEventArgs e)
        {
            WcReportPane.IsHidden = false;
            WcReportPane.Focus();
            WcReportPane.IsSelected = true;
        }

        private void btnShiftFollowUp_Click(object sender, RoutedEventArgs e)
        {
            ShiftFollowUpPane.IsHidden = false;
            ShiftFollowUpPane.Focus();
            ShiftFollowUpPane.IsSelected = true;
        }

        private void btnAdsEmpProductivityReport_Click(object sender, RoutedEventArgs e)
        {
            AdsEmpProductivityReportPane.IsHidden = false;
            AdsEmpProductivityReportPane.Focus();
            AdsEmpProductivityReportPane.IsSelected = true;
        }

        private void btnConfiguratosMatrix_Click(object sender, RoutedEventArgs e)
        {
            ConfiguratorsMatrixPane.IsHidden = false;
            ConfiguratorsMatrixPane.Focus();
            ConfiguratorsMatrixPane.IsSelected = true;
        }

        private void btnCbConfigGroup_Click(object sender, RoutedEventArgs e)
        {
            cbConfiguratorGroupPane.IsHidden = false;
            cbConfiguratorGroupPane.Focus();
            cbConfiguratorGroupPane.IsSelected = true;
        }

        private void btnLogisticReport_Click(object sender, RoutedEventArgs e)
        {

            LogisticReportPane.IsHidden = false;
            LogisticReportPane.Focus();
            LogisticReportPane.IsSelected = true;
        }

        private void btnCbZoneCombination_Click(object sender, RoutedEventArgs e)
        {

            cbForeignExportZoneCombinationPane.IsHidden = false;
            cbForeignExportZoneCombinationPane.Focus();
            cbForeignExportZoneCombinationPane.IsSelected = true;
        }

        private void btnDowntimePareto_Click(object sender, RoutedEventArgs e)
        {
            DowntimeParetoReportPane.IsHidden = false;
            DowntimeParetoReportPane.Focus();
            DowntimeParetoReportPane.IsSelected = true;
        }

        private void btnOrderInput_Click(object sender, RoutedEventArgs e)
        {
            OrderInputPane.IsHidden = false;
            OrderInputPane.Focus();
            OrderInputPane.IsSelected = true;
        }

        private void btnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            OrderCompletionPane.IsHidden = false;
            OrderCompletionPane.Focus();
            OrderCompletionPane.IsSelected = true;
        }

        private void btnOrderApprove_Click(object sender, RoutedEventArgs e)
        {
            OrderApprovePane.IsHidden = false;
            OrderApprovePane.Focus();
            OrderApprovePane.IsSelected = true;
        }

        private void btnCbExecOrdUsr_Click(object sender, RoutedEventArgs e)
        {
            cbOrderExecuteUsrPane.IsHidden = false;
            cbOrderExecuteUsrPane.Focus();
            cbOrderExecuteUsrPane.IsSelected = true;
        }

        private void btnCbAppordUsr_Click(object sender, RoutedEventArgs e)
        {
            cbOrderApproveUsrPane.IsHidden = false;
            cbOrderApproveUsrPane.Focus();
            cbOrderApproveUsrPane.IsSelected = true;
        }


        /// <summary>
        /// Handles the Click event of the btnCbDepartments control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCbDepartments_Click(object sender, RoutedEventArgs e)
        {
            cbDepartmentsPane.IsHidden = false;
            cbDepartmentsPane.Focus();
            cbDepartmentsPane.IsSelected = true;
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            anim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 900)));
            anim.EasingFunction = new BounceEase();
            anim.Completed += new EventHandler(anim_Completed);
            borderNotify.BeginAnimation(Border.HeightProperty, anim);
        }

        void anim_Completed(object sender, EventArgs e)
        {
            borderNotify.Visibility = System.Windows.Visibility.Collapsed;
            anim.Completed -= anim_Completed;
        }

        private void StatusBarItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            borderNotify.Visibility = System.Windows.Visibility.Visible;

            anim = new DoubleAnimation(400, new Duration(new TimeSpan(0, 0, 0, 0, 900)));
            anim.EasingFunction = new BounceEase();

            borderNotify.BeginAnimation(Border.HeightProperty, anim);
        }

        DoubleAnimation anim;

        private void btnLogisticReport2_Click(object sender, RoutedEventArgs e)
        {
            LogisticReport2Pane.IsHidden = false;
            LogisticReport2Pane.Focus();
            LogisticReport2Pane.IsSelected = true;
        }

        private void btnCbTmTool_Click(object sender, RoutedEventArgs e)
        {
            cbTmToolPane.IsHidden = false;
            cbTmToolPane.Focus();
            cbTmToolPane.IsSelected = true;
        }

        private void btnTmToolhandling_Click(object sender, RoutedEventArgs e)
        {
            TmToolHandlePane.IsHidden = false;
            TmToolHandlePane.Focus();
            TmToolHandlePane.IsSelected = true;
        }

        private void btnShiftRangeFollowUp_Click(object sender, RoutedEventArgs e)
        {
            ShiftFollowUpRangePane.IsHidden = false;
            ShiftFollowUpRangePane.Focus();
            ShiftFollowUpRangePane.IsSelected = true;
        }

        private void btnForeignVolumeControl_Click(object sender, RoutedEventArgs e)
        {
            ForeignVolumeCheckPane.IsHidden = false;
            ForeignVolumeCheckPane.Focus();
            ForeignVolumeCheckPane.IsSelected = true;
        }

        private void btnDomesticExport_Click(object sender, RoutedEventArgs e)
        {

            //Window wnd = new Window();

            //DomesticExportView vv = new DomesticExportView();

            //wnd.Content = vv;

            //wnd.Show();

            DomesticExportPane.IsHidden = false;
            DomesticExportPane.Focus();
            DomesticExportPane.IsSelected = true;
        }

        private void btnCbDomesticPriceList_Click(object sender, RoutedEventArgs e)
        {

            cbDomesticExportPriceListPane.IsHidden = false;
            cbDomesticExportPriceListPane.Focus();
            cbDomesticExportPriceListPane.IsSelected = true;
        }

        private void btnCbDomesticCustomer_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportCustomerPane.IsHidden = false;
            cbDomesticExportCustomerPane.Focus();
            cbDomesticExportCustomerPane.IsSelected = true;
        }

        private void btnCbDomesticContactType_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportContactTypePane.IsHidden = false;
            cbDomesticExportContactTypePane.Focus();
            cbDomesticExportContactTypePane.IsSelected = true;
        }

        private void btnCbDomesticForwarder_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportForwarderPane.IsHidden = false;
            cbDomesticExportForwarderPane.Focus();
            cbDomesticExportForwarderPane.IsSelected = true;
        }

        private void btnCbDomesticDriver_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportDriverPane.IsHidden = false;
            cbDomesticExportDriverPane.Focus();
            cbDomesticExportDriverPane.IsSelected = true;
        }

        private void btnCbDomesticCartype_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportCarTypePane.IsHidden = false;
            cbDomesticExportCarTypePane.Focus();
            cbDomesticExportCarTypePane.IsSelected = true;
        }

        private void btnCbDomesticComposition_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportCompositionPane.IsHidden = false;
            cbDomesticExportCompositionPane.Focus();
            cbDomesticExportCompositionPane.IsSelected = true;
        }

        private void btnCbDomesticDestination_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportDestinationPane.IsHidden = false;
            cbDomesticExportDestinationPane.Focus();
            cbDomesticExportDestinationPane.IsSelected = true;
        }

        private void btnCbDomesticBafPrice_Click(object sender, RoutedEventArgs e)
        {
            cbDomesticExportBafPricePane.IsHidden = false;
            cbDomesticExportBafPricePane.Focus();
            cbDomesticExportBafPricePane.IsSelected = true;
        }

        private void btnToolImport_Click(object sender, RoutedEventArgs e)
        {
            COS.Application.TechnicalMaintenance.Views.TPMImport imp = new TechnicalMaintenance.Views.TPMImport();
            imp.ShowDialog();
        }

        private void btnWcWgComparationReport_Click(object sender, RoutedEventArgs e)
        {
            WgWcComparationReportPane.IsHidden = false;
            WgWcComparationReportPane.Focus();
            WgWcComparationReportPane.IsSelected = true;
            
        }

        private void btnProduction4hPlanning_Click(object sender, RoutedEventArgs e)
        {
            Planning4hProductionViewPane.IsHidden = false;
            Planning4hProductionViewPane.Focus();
            Planning4hProductionViewPane.IsSelected = true;
            
        }

        private void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Planning4hProductionViewPane.IsHidden = false;
            Planning4hProductionViewPane.Focus();
            Planning4hProductionViewPane.IsSelected = true;
        }

        private void btnCbSupplier_Click(object sender, RoutedEventArgs e)
        {
            cbP4hSupplierViewPane.IsHidden = false;
            cbP4hSupplierViewPane.Focus();
            cbP4hSupplierViewPane.IsSelected = true;
        }

        private void btnCbManufacturer_Click(object sender, RoutedEventArgs e)
        {
            cbP4hManufacturerViewPane.IsHidden = false;
            cbP4hManufacturerViewPane.Focus();
            cbP4hManufacturerViewPane.IsSelected = true;
        }

        private void btnCbPlanExecution_Click(object sender, RoutedEventArgs e)
        {
            cbP4hPlanExecutionViewPane.IsHidden = false;
            cbP4hPlanExecutionViewPane.Focus();
            cbP4hPlanExecutionViewPane.IsSelected = true;
        }

        private void btncbP4HState_Click(object sender, RoutedEventArgs e)
        {
            cbP4hStateViewPane.IsHidden = false;
            cbP4hStateViewPane.Focus();
            cbP4hStateViewPane.IsSelected = true;
        }

        private void btnCbTransportType_Click(object sender, RoutedEventArgs e)
        {
            cbP4hTransportTypeViewPane.IsHidden = false;
            cbP4hTransportTypeViewPane.Focus();
            cbP4hTransportTypeViewPane.IsSelected = true;
        }

        private void btnLogForwarderReport_Click(object sender, RoutedEventArgs e)
        {
            LogisticForwarderReportPane.IsHidden = false;
            LogisticForwarderReportPane.Focus();
            LogisticForwarderReportPane.IsSelected = true;
        }

        private void btnCbP4hDifficulty_Click(object sender, RoutedEventArgs e)
        {
            cbP4hDifficultyViewPane.IsHidden = false;
            cbP4hDifficultyViewPane.Focus();
            cbP4hDifficultyViewPane.IsSelected = true;
        }

        private void btnLogLoadsReport_Click(object sender, RoutedEventArgs e)
        {
            LogisticLoadsReportPane.IsHidden = false;
            LogisticLoadsReportPane.Focus();
            LogisticLoadsReportPane.IsSelected = true;
        }

        private void btnLogForwarderExport_Click(object sender, RoutedEventArgs e)
        {
            LogisticDomForwarderPrReportPane.IsHidden = false;
            LogisticDomForwarderPrReportPane.Focus();
            LogisticDomForwarderPrReportPane.IsSelected = true;
        }

        private void btnForeignExportTransportOrderCo_Click(object sender, RoutedEventArgs e)
        {
            ForeignExportTrOrderCoPane.IsHidden = false;
            ForeignExportTrOrderCoPane.Focus();
            ForeignExportTrOrderCoPane.IsSelected = true;
        }

        private void btnCbAreaOfAccident_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCbTypeOfInjury_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCbInjPartOfBody_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCbTypeOfAccident_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCbSourceOfAccident_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCbCauseOfAccident_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCbPreventAccident_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAccidentLog_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCbInsuranceCompany_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
