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
using COS.Logistics;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class DomesticExportView : BaseUserControl
    {
        public DomesticExportView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new DomesticExportViewModel();
                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ForeignsView_Loaded);


                //  model.LocalDomesticExport.First()

            }
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                if (COS.Common.WPF.Helpers.HasRightForOperation("DomesticExport", "Insert"))
                {
                    DomesticExport item = new DomesticExport();

                    if (DetailWindow == null)
                    {
                        DetailWindow = new RadWindow();

                        DetailWindow.MaxHeight = ((RadWindow)COSContext.Current.RadMainWindow).ActualHeight;
                        DetailWindow.ResizeMode = ResizeMode.CanResize;
                        DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                        DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_DomesticExportDetail");
                        DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        detailView = new DomesticExportDetailView();
                        detailView.RaiseWindow = DetailWindow;
                        DetailWindow.Content = detailView;


                        StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                    }

                    //nastaveni vychozich hodnot....
                    item.PlannedDate = model.SelectedDate;
                    item.PointOfOrigin = COSContext.Current.DomesticDestinations.Where(a => a.IsDefault).FirstOrDefault();
                    DomesticExportDetail detail = new DomesticExportDetail();
                    detail.DestinationOrder = 1;
                    item.ExportDetails.Add(detail);

                    detailView.model.SelectedItem = item;

                    //detailView.model.SelectedDetailItem = null;

                    DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    DetailWindow.ShowDialog();

                }

            }
            else if (e.PropertyName == "Delete")
            {
                if (COS.Common.WPF.Helpers.HasRightForOperation("DomesticExport", "Delete"))
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

        public DomesticExportViewModel model;
        public RadWindow DetailWindow = null;
        private DomesticExportDetailView detailView = null;

        void ForeignsView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }


        private void LoadData()
        {
            var forexps = COSContext.Current.DomesticExports.Where(a => a.PlannedDate == model.SelectedDate.Date);
            try
            {
                COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, forexps);
            }
            catch (Exception exc) 
            {

            }
            try
            {
                var dets = COSContext.Current.DomesticExportDetails.Where(a => forexps.Contains(a.Export));
                COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, dets);
            }
            catch (Exception exc)
            {

            }
          


            if (model.SelectedMiniFilter.Key == 0)
                model.LocalDomesticExport = COSContext.Current.DomesticExports.Where(a => a.PlannedDate == model.SelectedDate.Date).OrderBy(a => a.PlannedDate).OrderBy(a => a.Round).ToList().OrderBy(a => a.Driver.Name).ToList();
            //else if (model.SelectedMiniFilter.Key == 1)
            //    model.LocalDomesticExport = COSContext.Current.DomesticExports.Where(a => a.PlannedDate == model.SelectedDate.Date && !a.Finished).ToList();
            //else if (model.SelectedMiniFilter.Key == 2)
            //    model.LocalDomesticExport = COSContext.Current.DomesticExports.Where(a => a.PlannedDate == model.SelectedDate.Date && a.Finished).ToList();
            else if (model.SelectedMiniFilter.Key == 3)
                model.LocalDomesticExport = COSContext.Current.DomesticExports.Where(a => a.PlannedDate == model.SelectedDate.Date && !a.IsCompleted).OrderBy(a => a.PlannedDate).OrderBy(a => a.Round).ToList().OrderBy(a => a.Driver.Name).ToList();

            grvForeigns.ItemsSource = model.LocalDomesticExport;
            grvForeigns.Rebind();
        }

        private void OnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                if (grvForeigns.SelectionUnit == GridViewSelectionUnit.Cell)
                {
                    if (grvForeigns.CurrentCell != null)
                    {
                        DomesticExport item = grvForeigns.CurrentCell.DataContext as DomesticExport;

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
                        DomesticExport item = grvForeigns.SelectedItem as DomesticExport;

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
                DomesticExportDetailView view = DetailWindow.Content as DomesticExportDetailView;

                view.model.CancelChanges();
            }
        }


        private void grvForeigns_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("DomesticExport", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        DomesticExport item = row.DataContext as DomesticExport;

                        if (item != null)
                        {
                            if (DetailWindow == null)
                            {
                                DetailWindow = new RadWindow();

                                DetailWindow.MaxHeight = ((RadWindow)COSContext.Current.RadMainWindow).ActualHeight;
                                DetailWindow.ResizeMode = ResizeMode.CanResize;
                                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                                DetailWindow.Header = ResourceHelper.GetResource<string>("log_DomesticExportDetail");
                                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new DomesticExportDetailView();
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
            //DomesticExport export = e.DataElement as DomesticExport;

            //if (export != null)
            //{
            //    if (export.IsInProgess)
            //    {
            //        e.Row.Background = new SolidColorBrush(Color.FromRgb(178, 161, 199));
            //        e.Row.Foreground = Brushes.Black;
            //    }
            //    else
            //    {
            //        e.Row.Background = Brushes.Transparent;
            //        e.Row.Foreground = Brushes.White;
            //    }
            //}
        }

        private void RecalculateBafPrice_Click(object sender, RoutedEventArgs e)
        {
            COS.Logistics.BafPricesRecalculationWindow wnd = new BafPricesRecalculationWindow(model);

            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.ShowDialog();
        }        

    }


}
