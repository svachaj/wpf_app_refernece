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
    public partial class TransportOrdesCoDetailView : BaseUserControl
    {
        public TransportOrdesCoDetailView(COSContext dataContext)
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (dataContext == null)
                {
                    var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                    string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                    this.dataContext = new COSContext(decryptString);
                }
                else
                    this.dataContext = dataContext;

                Model = new TransportOrdesCoDetailViewModel(this.dataContext);

                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ForeignsView_Loaded);

                worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                emailWorker = new BackgroundWorker();
                emailWorker.DoWork += new DoWorkEventHandler(emailWorker_DoWork);
                emailWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(emailWorker_RunWorkerCompleted);

                UIworker = new BackgroundWorker();
                UIworker.DoWork += new DoWorkEventHandler(UIworker_DoWork);
                UIworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UIworker_RunWorkerCompleted);
            }
        }

        void UIworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Model.IsBusy = false;
        }

        void UIworker_DoWork(object sender, DoWorkEventArgs e)
        {
            Model.IsBusy = true;

            p = 0;
            grvTransportsRecap.ItemsSource = null;
            grvTransportsRecap.ItemsSource = Model.LocalTransportsTemps;

            p = 0;
            grvTransports1.ItemsSource = null;
            grvTransports1.ItemsSource = Model.LocalTransports1;

            p = 0;
            grvTransports2.ItemsSource = null;
            grvTransports2.ItemsSource = Model.LocalTransports2;

            p = 0;
            grvTransports3.ItemsSource = null;
            grvTransports3.ItemsSource = Model.LocalTransports3;

            p = 0;
            grvTransports4.ItemsSource = null;
            grvTransports4.ItemsSource = Model.LocalTransports4;

            p = 0;
            grvTransports5.ItemsSource = null;
            grvTransports5.ItemsSource = Model.LocalTransports5;
        }


        BackgroundWorker UIworker = null;
        BackgroundWorker worker = null;
        BackgroundWorker emailWorker = null;
        COSContext dataContext = null;

        void ForeignsView_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                //if (!UIworker.IsBusy)
                //    UIworker.RunWorkerAsync();
            }
            else if (e.PropertyName == "Save")
            {
                RaiseWindow.DialogResult = true;
                RaiseWindow.Close();

                grvTransportsRecap.ItemsSource = null;
                grvTransports1.ItemsSource = null;
                grvTransports2.ItemsSource = null;
                grvTransports3.ItemsSource = null;
                grvTransports4.ItemsSource = null;
                grvTransports5.ItemsSource = null;

            }
            else if (e.PropertyName == "Cancel")
            {
                RaiseWindow.DialogResult = false;
                RaiseWindow.Close();

                grvTransportsRecap.ItemsSource = null;
                grvTransports1.ItemsSource = null;
                grvTransports2.ItemsSource = null;
                grvTransports3.ItemsSource = null;
                grvTransports4.ItemsSource = null;
                grvTransports5.ItemsSource = null;
            }
        }

        public TransportOrdesCoDetailViewModel Model;





        public RadWindow RaiseWindow { get; set; }

        TransportAdviClass selectedAviClass = null;
        ForeignExportAdviceWindow wnd = null;
        private void btnAdvice_click(object sender, RoutedEventArgs e)
        {
            selectedAviClass = (sender as RadButton).DataContext as TransportAdviClass;

            if (selectedAviClass != null)
            {
                wnd = new ForeignExportAdviceWindow(selectedAviClass.Destination, dataContext);
                wnd.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                wnd.Closed -= wnd_Closed;
                wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);

                wnd.ShowDialog();

               
            }
        }

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true && selectedAviClass != null)
            {
                selectedAviClass.Advice = wnd.SelectedItem;

                foreach (var itm in Model.LocalTransports1.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports2.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports3.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports4.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports5.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;

            }
        }

        ForeignExportInfoWindow wndInfo = null;
        ForeignExportTransportOrderCo selectedTr = null;
        private void btnInfo_click(object sender, RoutedEventArgs e)
        {
            selectedTr = (sender as RadButton).DataContext as ForeignExportTransportOrderCo;

            if (selectedTr != null)
            {
                wndInfo = new ForeignExportInfoWindow(selectedTr.ForeignExport.Destination, dataContext);
                wndInfo.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                wndInfo.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                wndInfo.Closed -= wndInfo_Closed;
                wndInfo.Closed += new EventHandler<WindowClosedEventArgs>(wndInfo_Closed);

                wndInfo.ShowDialog();
            }
        }

        void wndInfo_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true && selectedTr != null)
            {
                selectedTr.Info = wndInfo.SelectedItem;
                grvTransports1.Rebind();
                grvTransports2.Rebind();
                grvTransports3.Rebind();
                grvTransports4.Rebind();
                grvTransports5.Rebind();
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Model.IsBusy = false;
            if (!Model.RaiseErrors.IsNullOrEmptyString())
                RadWindow.Alert(new DialogParameters() { Content = Model.RaiseErrors, Owner = (RadWindow)COSContext.Current.RadMainWindow });

            Model.RaiseErrors = "";
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var filename = e.Argument.ToString();

            Model.RaiseErrors = "";
            Model.IsBusy = true;
            try
            {
                Model.RaiseErrors = Model.ExportToExcel(filename);
                //Process.Start(fileName);
            }
            catch (Exception exc)
            {
                Model.RaiseErrors = ResourceHelper.GetResource<string>("m_Body_LOG00000032");
                Logging.LogException(exc, LogType.ToFileAndEmail);
            }
        }

        private void btnExportData_click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.AddExtension = true;
            dlg.FileName = "OrderSheet";
            dlg.DefaultExt = "*.xlsx";

            dlg.Filter = "Excel file|*.xlsx";

            var dlgres = dlg.ShowDialog();

            if (dlgres.HasValue && dlgres.Value)
            {
                if (!worker.IsBusy)
                    worker.RunWorkerAsync(dlg.FileName);
            }



        }

        void emailWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Model.IsBusy = false;

            if (!Model.RaiseErrors.IsNullOrEmptyString())
                RadWindow.Alert(new DialogParameters() { Content = Model.RaiseErrors, Owner = (RadWindow)COSContext.Current.RadMainWindow });
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_LOG00000033"), Owner = (RadWindow)COSContext.Current.RadMainWindow });

                string selectedKey = Model.SelectedWeek.ToString().PadLeft(2, '0') + "-" + Model.SelectedYear.ToString() + "-" + Model.SelectedItem.Forwarder.ID.ToString().PadLeft(6, '0');
                var processDateEntity = this.dataContext.ProcessDates.FirstOrDefault(a => a.Type.ProcessName == "TransportOrdesCoDetailView" && a.ProcessKey == selectedKey);

                if (processDateEntity == null)
                {
                    processDateEntity = this.dataContext.ProcessDates.CreateObject();
                    processDateEntity.Type = this.dataContext.ProcessDatesTypes.FirstOrDefault(a => a.ProcessName == "TransportOrdesCoDetailView");
                    processDateEntity.ProcessKey = selectedKey;
                    this.dataContext.ProcessDates.AddObject(processDateEntity);
                }

                processDateEntity.ProcessDate = COSContext.Current.DateTimeServer;

                Model.Save();
            }

            Model.RaiseErrors = "";
        }

        void emailWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Model.IsBusy = true;
            try
            {
                Model.RaiseErrors = Model.ExportToExcelAndSendEmail();
            }
            catch (Exception exc)
            {
                Model.RaiseErrors = ResourceHelper.GetResource<string>("m_Body_LOG00000032");
                Logging.LogException(exc, LogType.ToFileAndEmail);
            }
        }

        private void btnSendExportedDataMail_click(object sender, RoutedEventArgs e)
        {
            string err = "";

            if (Model.SelectedItem.Forwarder.ForwarderToEmails.IsNullOrEmptyString())
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000038") + Environment.NewLine;

            if (Model.SelectedItem.Forwarder.RecieptEmails.IsNullOrEmptyString())
                err += ResourceHelper.GetResource<string>("m_Body_LOG00000039") + Environment.NewLine;

            if (err.IsNullOrEmptyString())
            {
                if (!emailWorker.IsBusy)
                    emailWorker.RunWorkerAsync();
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Header = ResourceHelper.GetResource<string>("m_Header1_A"), Content = err, Owner = (RadWindow)COSContext.Current.RadMainWindow, DialogStartupLocation = WindowStartupLocation.CenterOwner });
            }
        }

        private void mainScroller_changed(object sender, ScrollChangedEventArgs e)
        {
            scrollHeader.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void btnInfoDelete_click(object sender, RoutedEventArgs e)
        {
            selectedTr = (sender as RadButton).DataContext as ForeignExportTransportOrderCo;

            if (selectedTr != null)
            {
                selectedTr.Info = null;
                grvTransports1.Rebind();
                grvTransports2.Rebind();
                grvTransports3.Rebind();
                grvTransports4.Rebind();
                grvTransports5.Rebind();
            }
        }

        private void btnAdviceDelete_click(object sender, RoutedEventArgs e)
        {
            selectedAviClass = (sender as RadButton).DataContext as TransportAdviClass;

            if (selectedAviClass != null)
            {
                selectedAviClass.Advice = null;
                foreach (var itm in Model.LocalTransports1.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports2.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports3.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports4.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
                foreach (var itm in Model.LocalTransports5.Where(a => a != null && a.ForeignExport.Destination == selectedAviClass.Destination))
                    itm.Advice = selectedAviClass.Advice;
            }
        }

        private int p = 0;
        private int p1 = 0;
        private int p2 = 0;
        private int p3 = 0;
        private int p4 = 0;
        private int p5 = 0;

        private void grvTransportsRecap_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (p % 2 == 0)
            {
                e.Row.Background = this.Resources["CosColor7"] as SolidColorBrush;
            }
            else
            {
                e.Row.Background = Brushes.Transparent;
            }

            p++;
        }

        private void grvTransports1_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (p1 % 2 == 0)
            {
                e.Row.Background = this.Resources["CosBrush6"] as SolidColorBrush;
            }
            else
            {
                e.Row.Background = Brushes.Transparent;
            }

            p1++;
        }

        private void grvTransports2_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (p2 % 2 == 0)
            {
                e.Row.Background = this.Resources["CosBrush6"] as SolidColorBrush;
            }
            else
            {
                e.Row.Background = Brushes.Transparent;
            }

            p2++;
        }

        private void grvTransports3_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (p3 % 2 == 0)
            {
                e.Row.Background = this.Resources["CosBrush6"] as SolidColorBrush;
            }
            else
            {
                e.Row.Background = Brushes.Transparent;
            }

            p3++;
        }

        private void grvTransports4_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (p4 % 2 == 0)
            {
                e.Row.Background = this.Resources["CosBrush6"] as SolidColorBrush;
            }
            else
            {
                e.Row.Background = Brushes.Transparent;
            }

            p4++;
        }

        private void grvTransports5_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (p5 % 2 == 0)
            {
                e.Row.Background = this.Resources["CosBrush6"] as SolidColorBrush;
            }
            else
            {
                e.Row.Background = Brushes.Transparent;
            }

            p5++;
        }


    }



}
