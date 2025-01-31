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
using COS.Logistics;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class ForeignsView : BaseUserControl
    {
        public ForeignsView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new ForeignViewModel();
                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ForeignsView_Loaded);
            }
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExport", "Insert"))
                {
                    ForeignExport item = new ForeignExport();


                    if (DetailWindow == null)
                    {
                        DetailWindow = new RadWindow();

                        DetailWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
                        DetailWindow.ResizeMode = ResizeMode.NoResize;
                        DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                        DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_ForeignExportDetail");
                        DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        detailView = new ForeignDetailView();
                        detailView.RaiseWindow = DetailWindow;
                        DetailWindow.Content = detailView;


                        StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                    }

                    //nastaveni vychozich hodnot....
                    item.PlannedDate = model.SelectedDate;

                    detailView.model.SelectedItem = item;

                    //detailView.model.SelectedDetailItem = null;

                    DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    DetailWindow.ShowDialog();

                }

            }
            else if (e.PropertyName == "Delete")
            {
                if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExport", "Delete"))
                {
                    RadWindow.Confirm(new DialogParameters()
                    {
                        OkButtonContent = COS.Resources.ResourceHelper.GetResource<string>("m_General_Y"),
                        CancelButtonContent = COS.Resources.ResourceHelper.GetResource<string>("m_General_N"),
                        DialogStartupLocation = WindowStartupLocation.CenterOwner,
                        Content = COS.Resources.ResourceHelper.GetResource<string>("m_General_Delete"),
                        Header = COS.Resources.ResourceHelper.GetResource<string>("m_Header1_A"),
                        Closed = new EventHandler<WindowClosedEventArgs>(OnConfirmClosed)
                    });
                }
            }
            else if (e.PropertyName == "SelectedDate" || e.PropertyName == "SelectedMiniFilter")
            {
                LoadData();
            }
        }

        public ForeignViewModel model;
        public RadWindow DetailWindow = null;
        private ForeignDetailView detailView = null;

        void ForeignsView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }


        private void LoadData()
        {
            var forexps = COSContext.Current.ForeignExports.Where(a => a.PlannedDate == model.SelectedDate.Date);
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, forexps);
            var conns = COSContext.Current.ForeignExportConnections.Where(a => forexps.Contains(a.ForeignExport));
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, conns);
            var dets = COSContext.Current.ForeignExportDetails.Where(a => conns.Contains(a.Connection));
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, dets);
            var dests = COSContext.Current.ZoneLogistics;//.Where(a => conns.Select(d => d.ID_Zone).Contains(a.ID));
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, dests);


            if (model.SelectedMiniFilter.Key == 0)
                model.LocalForeignExports = COSContext.Current.ForeignExports.Where(a => a.PlannedDate == model.SelectedDate.Date).ToList();
            else if (model.SelectedMiniFilter.Key == 1)
                model.LocalForeignExports = COSContext.Current.ForeignExports.Where(a => a.PlannedDate == model.SelectedDate.Date && !a.Finished).ToList();
            else if (model.SelectedMiniFilter.Key == 2)
                model.LocalForeignExports = COSContext.Current.ForeignExports.Where(a => a.PlannedDate == model.SelectedDate.Date && a.Finished).ToList();
            else if (model.SelectedMiniFilter.Key == 3)
                model.LocalForeignExports = COSContext.Current.ForeignExports.Where(a => a.PlannedDate == model.SelectedDate.Date && !a.IsCompleted).ToList();

            grvForeigns.ItemsSource = model.LocalForeignExports;
            grvForeigns.Rebind();

            string dateToCompare = model.SelectedDate.ToString("yyyy-MM-dd");
            var processDateEntity = COSContext.Current.ProcessDates.FirstOrDefault(a => a.Type.ProcessName == "ForeignView" && a.ProcessKey == dateToCompare);

            if (processDateEntity != null)
            {
                model.LastSendTimeWindowDate = processDateEntity.ProcessDate;
            }
            else
            {
                model.LastSendTimeWindowDate = null;
            }
        }

        private void OnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                if (grvForeigns.SelectionUnit == GridViewSelectionUnit.Cell)
                {
                    if (grvForeigns.CurrentCell != null)
                    {
                        ForeignExport item = grvForeigns.CurrentCell.DataContext as ForeignExport;

                        if (item != null)
                        {
                            COSContext.Current.DeleteObject(item);
                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            LoadData();
                        }
                    }
                }
                else
                {
                    if (grvForeigns.SelectedItem != null)
                    {
                        ForeignExport item = grvForeigns.SelectedItem as ForeignExport;

                        if (item != null)
                        {
                            COSContext.Current.DeleteObject(item);
                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            LoadData();
                        }
                    }
                }
            }


        }


        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                LoadData();
            }
            else
            {
                ForeignDetailView view = DetailWindow.Content as ForeignDetailView;

                view.model.CancelChanges();
            }
        }


        private void grvForeigns_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExport", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        ForeignExport item = row.DataContext as ForeignExport;

                        if (item != null)
                        {
                            if (DetailWindow == null)
                            {
                                DetailWindow = new RadWindow();

                                DetailWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
                                DetailWindow.ResizeMode = ResizeMode.NoResize;
                                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                                DetailWindow.Header = ResourceHelper.GetResource<string>("log_ForeignExportDetail");
                                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new ForeignDetailView();
                                detailView.RaiseWindow = DetailWindow;
                                DetailWindow.Content = detailView;

                                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                            }

                            detailView.model.SelectedItem = item;
                            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                            DetailWindow.ShowDialog();
                        }
                    }
                }
            }
        }

        private void minusDay_click(object sender, RoutedEventArgs e)
        {
            if (model != null)
                model.SelectedDate = model.SelectedDate.AddDays(-1);
        }

        private void plusDay_click(object sender, RoutedEventArgs e)
        {
            if (model != null)
                model.SelectedDate = model.SelectedDate.AddDays(1);
        }

        int prevOrder = -1;
        private void defaultColumns_click(object sender, RoutedEventArgs e)
        {
            if (prevOrder > 0)
            {
                colForwarder.DisplayIndex = prevOrder;
                prevOrder = -1;

                grvForeigns.SelectionUnit = GridViewSelectionUnit.FullRow;
            }
        }

        private void copingColumns_click(object sender, RoutedEventArgs e)
        {
            if (colForwarder.DisplayIndex != prevOrder)
            {
                prevOrder = colForwarder.DisplayIndex;

                colForwarder.DisplayIndex = 1;

                grvForeigns.SelectionUnit = GridViewSelectionUnit.Cell;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
                LoadData();
        }

        private void grvForeigns_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            ForeignExport export = e.DataElement as ForeignExport;

            if (export != null)
            {
                if (export.IsInProgess)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(178, 161, 199));
                    e.Row.Foreground = Brushes.Black;
                }
                else
                {
                    e.Row.Background = Brushes.Transparent;
                    e.Row.Foreground = Brushes.White;
                }
            }
        }

        private void RecalculateBafPrice_Click(object sender, RoutedEventArgs e)
        {
            COS.Logistics.BafPricesRecalculationWindow wnd = new BafPricesRecalculationWindow(model);

            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.ShowDialog();
        }

        private void sendEmailTimeWindow_Click(object sender, RoutedEventArgs e)
        {
            //RadWindow.Confirm(new DialogParameters()
            //{
            //    OkButtonContent = ResourceHelper.GetResource<string>("app_Yes"),
            //    CancelButtonContent = ResourceHelper.GetResource<string>("app_No"),
            //    Content = ResourceHelper.GetResource<string>("m_Body_LOG00000036"),
            //    Closed = new EventHandler<WindowClosedEventArgs>(confirmEmail_closed),
            //    Owner = (RadWindow)COSContext.Current.RadMainWindow,
            //    Header = ResourceHelper.GetResource<string>("app_Alert")
            //});

            if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExport", "Update"))
            {
                if (DetailWindowTimeWindow == null)
                {
                    DetailWindowTimeWindow = new RadWindow();

                    DetailWindowTimeWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
                    DetailWindowTimeWindow.ResizeMode = ResizeMode.CanResize;
                    DetailWindowTimeWindow.Closed += DetailWindowTimeWindow_Closed;
                    DetailWindowTimeWindow.Header = ResourceHelper.GetResource<string>("log_ProdTimeWindowNot");
                    DetailWindowTimeWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    viewExportView = new ForeignViewExportView(null);
                    viewExportView.RaiseWindow = DetailWindowTimeWindow;
                    DetailWindowTimeWindow.Content = viewExportView;

                    StyleManager.SetTheme(DetailWindowTimeWindow, new Expression_DarkTheme());
                }

                viewExportView.model.ReloadData(ForeignViewModel.GetExportClasses(model.LocalForeignExports));

                DetailWindowTimeWindow.DialogResult = false;
                DetailWindowTimeWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                DetailWindowTimeWindow.ShowDialog();

            }
        }

        //void confirmEmail_closed(object sender, WindowClosedEventArgs e)
        //{
        //    if (e.DialogResult.HasValue && e.DialogResult.Value)
        //    {
        //        if (COS.Common.WPF.Helpers.HasRightForOperation("ForeignExport", "Update"))
        //        {
        //            if (DetailWindowTimeWindow == null)
        //            {
        //                DetailWindowTimeWindow = new RadWindow();

        //                DetailWindowTimeWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
        //                DetailWindowTimeWindow.ResizeMode = ResizeMode.CanResize;
        //                DetailWindowTimeWindow.Closed += DetailWindowTimeWindow_Closed;
        //                DetailWindowTimeWindow.Header = ResourceHelper.GetResource<string>("log_ForeignExportDetail");
        //                DetailWindowTimeWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //                viewExportView = new ForeignViewExportView(null);
        //                viewExportView.RaiseWindow = DetailWindowTimeWindow;
        //                DetailWindowTimeWindow.Content = viewExportView;

        //                StyleManager.SetTheme(DetailWindowTimeWindow, new Expression_DarkTheme());
        //            }

        //            viewExportView.model.ReloadData(ForeignViewModel.GetExportClasses(model.LocalForeignExports));

        //            DetailWindowTimeWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
        //            DetailWindowTimeWindow.ShowDialog();


        //        }

        //    }
        //}

        void DetailWindowTimeWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                if (emailBackgroundWorker == null)
                {
                    emailBackgroundWorker = new BackgroundWorker();
                    emailBackgroundWorker.DoWork += new DoWorkEventHandler(emailBackgroundWorker_DoWork);
                    emailBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(emailBackgroundWorker_RunWorkerCompleted);
                }

                if (!emailBackgroundWorker.IsBusy)
                    emailBackgroundWorker.RunWorkerAsync();
            }
            else 
            {
                LoadData(); 
            }
        }


        void emailBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            model.IsBusy = false;

            if (!string.IsNullOrEmpty(emailError))
            {
                RadWindow.Alert(new DialogParameters() { Content = emailError, Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_LOG00000030"), Owner = (RadWindow)COSContext.Current.RadMainWindow });

                string dateToCompare = model.SelectedDate.ToString("yyyy-MM-dd");
                var processDateEntity = COSContext.Current.ProcessDates.FirstOrDefault(a => a.Type.ProcessName == "ForeignView" && a.ProcessKey == dateToCompare);

                if (processDateEntity == null)
                {
                    processDateEntity = COSContext.Current.ProcessDates.CreateObject();
                    processDateEntity.Type = COSContext.Current.ProcessDatesTypes.FirstOrDefault(a => a.ProcessName == "ForeignView");
                    processDateEntity.ProcessKey = model.SelectedDate.ToString("yyyy-MM-dd");

                    COSContext.Current.ProcessDates.AddObject(processDateEntity);
                }

                processDateEntity.ProcessDate = COSContext.Current.DateTimeServer;

                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                model.LastSendTimeWindowDate = processDateEntity.ProcessDate;
            }

            emailError = "";
        }


        RadWindow DetailWindowTimeWindow = null;
        ForeignViewExportView viewExportView = null;

        void emailBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            model.IsBusy = true;

            ForeignViewModel.GenerateTimeWindowEmail(viewExportView.model.LocalExports.ToList(), out emailError);

            LoadData();
        }

        string emailError = "";
        BackgroundWorker emailBackgroundWorker = null;

    }

    public class ForeignExportSpecialVIConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ForeignExport export = value as ForeignExport;

            Visibility result = Visibility.Collapsed;

            if (export != null)
            {
                if (export.VIVolumeCBM > 0)
                    result = Visibility.Visible;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ForeignExportSpecialVAConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ForeignExport export = value as ForeignExport;

            Visibility result = Visibility.Collapsed;

            if (export != null)
            {
                if (export.VAVolumeCBM > 0 || export.VA4HVolumeCBM > 0)
                    result = Visibility.Visible;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
