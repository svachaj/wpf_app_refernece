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
using COS.Application.Engeneering.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Threading;
using System.Windows.Threading;
using Telerik.Windows.Data;

namespace COS.Application.Engeneering.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class StandardsView : BaseUserControl
    {
        public StandardsView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new StandardsViewModel();
                this.DataContext = model;

                if (COSContext.Current.Language == "cs-CZ")
                {
                    grvAllStandards.Columns["cz_itemgroupColumn"].IsVisible = true;
                    grvAllStandards.Columns["en_itemgroupColumn"].IsVisible = false;
                }
                else
                {
                    grvAllStandards.Columns["en_itemgroupColumn"].IsVisible = true;
                    grvAllStandards.Columns["cz_itemgroupColumn"].IsVisible = false;
                }

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(StandardsView_Loaded);

                _worker = new BackgroundWorker();
                _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
                _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);

                _workerImportDB = new BackgroundWorker();
                _workerImportDB.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_workerImportDB_RunWorkerCompleted);
                _workerImportDB.DoWork += new DoWorkEventHandler(_workerImportDB_DoWork);

                _workerGetUnexists = new BackgroundWorker();
                _workerGetUnexists.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_workerGetUnexists_RunWorkerCompleted);
                _workerGetUnexists.DoWork += new DoWorkEventHandler(_workerGetUnexists_DoWork);

                _workerLoadAll = new BackgroundWorker();
                _workerLoadAll.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_workerLoadAll_RunWorkerCompleted);
                _workerLoadAll.DoWork += new DoWorkEventHandler(_workerLoadAll_DoWork);

                if (!_workerLoadAll.IsBusy)
                    _workerLoadAll.RunWorkerAsync();
            }
        }

        void _workerLoadAll_DoWork(object sender, DoWorkEventArgs e)
        {
            model.IsBusy = true;
            RefreshAllStandards();
        }

        public void RefreshAllStandards()
        {
            model.LocalStandards = COSContext.Current.AvailableStandards();//COSContext.Current.Standards.ToList()

        }

        void _workerLoadAll_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            model.IsBusy = false;

            grvAllStandards.ItemsSource = null;
            grvAllStandards.ItemsSource = model.LocalStandards;
            grvAllStandards.Rebind();
            grvAllStandards.SelectedItem = model.LocalStandards.FirstOrDefault();

        }

        void _workerImportDB_DoWork(object sender, DoWorkEventArgs e)
        {
            model.IsBusy = true;
            model.ImportDataInDB();

        }

        void _workerImportDB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            grvImportStandards.Rebind();
            model.IsBusy = false;

            model.OnPropertyChanged("DataImportedInDB");
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                model.IsBusy = true;

                model.ImportStandartsFromCSV(e.Argument.ToString(), ';');
            }
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            grvImportStandards.Rebind();
            model.IsBusy = false;
        }


        BackgroundWorker _worker = null;
        BackgroundWorker _workerImportDB = null;
        BackgroundWorker _workerGetUnexists = null;
        BackgroundWorker _workerLoadAll = null;

        void StandardsView_Loaded(object sender, RoutedEventArgs e)
        {
           
        }



        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Save")
            {
                if (model.TempProduction != null)
                {
                    model.UnexistStandardHPs.Remove(model.TempProduction);
                    model.TempProduction = null;

                    RebindUnexists();
                }


                //model.LocalStandards = COSContext.Current.Standards.ToList();
                //grvAllStandards.ItemsSource = null;
                //grvAllStandards.ItemsSource = model.LocalStandards;
                grvAllStandards.Rebind();
            }
            else if (e.PropertyName == "Cancel")
            {
                model.TempProduction = null;

                if (grvAllStandards.Items.Count > 0)
                {
                    grvAllStandards.SelectedItem = model.LocalStandards.FirstOrDefault();
                }
            }
            else if (e.PropertyName == "Delete")
            {
                model.TempProduction = null;

                if (grvAllStandards.Items.Count > 0)
                {
                    //model.LocalStandards = COSContext.Current.Standards.ToList();
                    //grvAllStandards.ItemsSource = null;
                    //grvAllStandards.ItemsSource = model.LocalStandards;
                    grvAllStandards.Rebind();
                    grvAllStandards.SelectedItem = model.LocalStandards.FirstOrDefault();
                }
            }
            else if (e.PropertyName == "DataImportedInDB")
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000002"), Header = ResourceHelper.GetResource<string>("m_Header3_I"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
            else if (e.PropertyName == "SelectedStandard")
            {
                //if (model.SelectedStandard != null && model.SelectedStandard.ID > 0)
                //    Save();
                wrapselectedGrid.DataContext = model.SelectedStandard;
            }
        }

        StandardsViewModel model;


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Save();
        }

        private void Save()
        {
            string message = null;
            if (!model.IsNewItem && model.IsValid(out message) && model.SelectedStandard.ID > 0)
            {
                try
                {
                    model.SelectedStandard.ID_Standard = model.SelectedStandard.WorkCenter.Value + model.SelectedStandard.ItemNumber;
                    model.SelectedStandard.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;
                    model.SelectedStandard.ModifyDate = DateTime.Now;
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                    grvAllStandards.Rebind();
                }
                catch
                {
                    COSContext.Current.RejectChanges();
                }
            }
            else if (!string.IsNullOrEmpty(message))
            {
                if (!model.IsNewItem)
                {
                    COSContext.Current.RejectChanges();
                }
                RadWindow.Alert(new DialogParameters() { Content = message, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }


        public List<WorkCenter> ComboWCS = null;
        private void cmbWorkGroup_changed(object sender, SelectionChangedEventArgs e)
        {
            // wrapselectedGrid.DataContext = model.SelectedStandard;

            cmbWorkCenter.GetBindingExpression(RadComboBox.SelectedItemProperty).UpdateTarget();


            WorkGroup wg = (sender as RadComboBox).SelectedItem as WorkGroup;

            var wcss = model.LocalWorkCentersList;


            if (wg != null)
            {
                var items = from wcc in wcss
                            join wgc in COSContext.Current.WorkGroupsWorkCenters on wcc.ID equals wgc.ID_WorkCenter
                            where wgc.ID_WorkGroup == wg.ID
                            select wcc;

                COSContext.Current.LocalWorkCenters = items.ToList();

                //cmbWorkCenter.ItemsSource = null;
                //cmbWorkCenter.ItemsSource = COSContext.Current.LocalWorkCenters;

                // cmbWorkCenter.SelectedIndex = 0;
                //  model.SelectedStandard.WorkCenter = items.FirstOrDefault();

                //Save();

                if (model.PrevStandard != null)
                    model.PrevStandard.WorkCenter = wcss.FirstOrDefault(a => a.ID == model.PrevStandard.ID_WorkCenter);
            }


        }

        private void cmbWorkCenter_changed(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbWorkCenter.SelectedItem != null)
            //    Save();
        }

        private void cmbItemGroup_changed(object sender, SelectionChangedEventArgs e)
        {
            //Save();
        }

        private void RadTabControl_SelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            RadTabControl tabc = sender as RadTabControl;

            if (tabc != null)
            {
                RadTabItem item = tabc.SelectedItem as RadTabItem;

                if (item != null)
                {
                    if (item.Name == "tabItemAll" && item.IsVisible)
                    {
                      //  grvAllStandards.FilterDescriptors.Clear();
                    }

                }
            }
        }

        private void grvAllStandardsExport_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            model.IsBusy = false;

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            model.IsBusy = true;

            worker.RunWorkerAsync();



        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            model.IsBusy = false;
            grvAllStandardsExport.ItemsSource = model.ExportData;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            model.SetExportData();

            //this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            //{
            //    System.Threading.Thread.Sleep(1000);

            //    grvAllStandardsExport.Rebind();
            //}));

        }

        BackgroundWorker worker = null;

        private void btnExportToCsv_click(object sender, RoutedEventArgs e)
        {


            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "csv";
            dialog.Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "csv", ExportFormat.Csv);
            dialog.FilterIndex = 1;


            if (dialog.ShowDialog() == true)
            {

                string fname = dialog.FileName;


                model.IsBusy = true;

                if (exportworker1 == null)
                {
                    exportworker1 = new BackgroundWorker();
                    exportworker1.DoWork += new DoWorkEventHandler(exportworker1_DoWork);
                    exportworker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(exportworker1_RunWorkerCompleted);
                }

                exportworker1.RunWorkerAsync(fname);
            }

            ////SaveFileDialog dialog = new SaveFileDialog();
            ////dialog.DefaultExt = "csv";
            ////dialog.Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "csv", ExportFormat.Csv);
            ////dialog.FilterIndex = 1;

            ////grvAllStandardsExport.IsBusy = true;

            ////if (dialog.ShowDialog() == true)
            ////{
            ////    using (Stream stream = dialog.OpenFile())
            ////    {
            ////        GridViewCsvExportOptions exportOptions = new GridViewCsvExportOptions();
            ////        exportOptions.Format = ExportFormat.Csv;
            ////        exportOptions.ShowColumnFooters = true;
            ////        exportOptions.ShowColumnHeaders = true;
            ////        exportOptions.ShowGroupFooters = true;

            ////        exportOptions.Encoding = Encoding.UTF8;
            ////        exportOptions.ColumnDelimiter = ";";


            ////        grvAllStandardsExport.Export(stream, exportOptions);

            ////        grvAllStandardsExport.IsBusy = false;

            ////        ////if (exportworker1 == null)
            ////        ////{
            ////        ////    exportworker1 = new BackgroundWorker();
            ////        ////    exportworker1.DoWork += new DoWorkEventHandler(exportworker1_DoWork);
            ////        ////    exportworker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(exportworker1_RunWorkerCompleted);
            ////        ////}

            ////        ////exportworker1.RunWorkerAsync(new Tuple<Stream, GridViewCsvExportOptions>(stream, exportOptions));

            ////    }
            ////}
        }

        DispatcherTimer superTimer = new DispatcherTimer();

        void exportworker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            model.IsBusy = false;
        }

        void exportworker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string fname = e.Argument.ToString();


            vv(fname);


        }

        void vv(object param)
        {
            string fname = param.ToString();

            using (var stream = File.Create(fname))
            {
                using (StreamWriter swr = new StreamWriter(stream, Encoding.UTF8))
                {

                    StringBuilder sbLine = new StringBuilder(); // """Číslo položky"";""Config"";""Popis"";""Operátoři"";""Pracovní skupina"";""Pracovní středisko";//"Skupina položek";"Váha (kg)";"Čas setupu";"Kusů za minutu";"Kusů za hodinu"";

                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_ItemNumber")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_IsConfig")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_Description")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_Labour")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_WorkGroup")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_WorkCenter")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_ItemGroup")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_Weight")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_Setup")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_PcsMin")));
                    sbLine.Append(";");
                    sbLine.Append(CreateCsvItem(ResourceHelper.GetResource<string>("eng_PcsHour")));

                    swr.WriteLine(sbLine.ToString());

                    foreach (var itm in model.ExportData)
                    {
                        sbLine = new StringBuilder();

                        sbLine.Append(CreateCsvItem(itm.ItemNumber));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.isConfig.ToString()));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.ItemDescription));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.Labour.ToString()));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.WorkGroup.Value));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.WorkCenter.Value));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.ItemGroup != null ? itm.ItemGroup.Description : ""));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.Weight_Kg.ToString()));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.SetupTime_mm.ToString()));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.PcsPerMinute.ToString()));
                        sbLine.Append(";");
                        sbLine.Append(CreateCsvItem(itm.PcsPerHour.ToString()));

                        swr.WriteLine(sbLine.ToString());
                    }

                    swr.Close();

                }
                stream.Close();

            }


        }

        public string CreateCsvItem(string baseText)
        {
            StringBuilder sbLine = new StringBuilder();
            sbLine.Append('"');
            sbLine.Append(baseText);
            sbLine.Append('"');

            return sbLine.ToString();

        }

        BackgroundWorker exportworker1 = null;

        void grvAllStandardsExport_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {

        }

        private void btnExportUnexistsToCsv_click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "csv";
            dialog.Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "csv", ExportFormat.Csv);
            dialog.FilterIndex = 1;

            if (dialog.ShowDialog() == true)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    GridViewCsvExportOptions exportOptions = new GridViewCsvExportOptions();
                    exportOptions.Format = ExportFormat.Csv;
                    exportOptions.ShowColumnFooters = true;
                    exportOptions.ShowColumnHeaders = true;
                    exportOptions.ShowGroupFooters = true;

                    exportOptions.Encoding = Encoding.UTF8;
                    exportOptions.ColumnDelimiter = ";";

                    model.IsBusy = true;

                    grvUnexistsStandards.Export(stream, exportOptions);

                    model.IsBusy = false;
                }
            }
        }

        private void btnRefresh_click(object sender, RoutedEventArgs e)
        {
            StdFilterWindow wnd = new StdFilterWindow(model);
            wnd.Owner = this;
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();

            //model.SetExportData();
            //grvAllStandardsExport.ItemsSource = null;
            //grvAllStandardsExport.ItemsSource = model.ExportData;
            //grvAllStandardsExport.Rebind();

        }

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {

            model.SetExportData();


            grvAllStandardsExport.ItemsSource = model.ExportData;
            grvAllStandardsExport.Rebind();

        }

        private void grvImportStandards_CellLoaded(object sender, Telerik.Windows.Controls.GridView.CellEventArgs e)
        {
            if (e.Cell.ParentRow.Item != null)
            {
                ImportStandardData data = e.Cell.DataContext as ImportStandardData;

                if (data != null)
                {
                    if (e.Cell.Column.Name == "colItemNumber")
                    {
                        if (!string.IsNullOrEmpty(data.ItemNumberText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.ItemNumberText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colIsConfig")
                    {
                        if (!string.IsNullOrEmpty(data.IsConfigText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.IsConfigText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colItemDescription")
                    {
                        if (!string.IsNullOrEmpty(data.DescriptionText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.DescriptionText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colLabour")
                    {
                        if (!string.IsNullOrEmpty(data.LaboursText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.LaboursText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colWorkGroup")
                    {
                        if (!string.IsNullOrEmpty(data.WorkGroupText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.WorkGroupText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colWorkCenter")
                    {
                        if (!string.IsNullOrEmpty(data.WorkCenterText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.WorkCenterText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colItemGroup")
                    {
                        if (!string.IsNullOrEmpty(data.ItemGroupsText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.ItemGroupsText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colWeight_Kg")
                    {
                        if (!string.IsNullOrEmpty(data.WeighText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.WeighText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colSetupTime_mm")
                    {
                        if (!string.IsNullOrEmpty(data.SetupTimeText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.SetupTimeText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colPcsPerMinute")
                    {
                        if (!string.IsNullOrEmpty(data.PcsPerMinText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.PcsPerMinText;
                        }
                    }
                    else if (e.Cell.Column.Name == "colPcsPerHour")
                    {
                        if (!string.IsNullOrEmpty(data.PcsPerHourText))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.PcsPerHourText;
                        }
                    }

                }
            }
        }

        private void grvImportStandards_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            ImportStandardData data = e.DataElement as ImportStandardData;
            if (data != null)
            {
                if (data.ExistingStandard != null)
                {
                    e.Row.Background = Brushes.Blue;
                }
                else if (data.NewStandard != null)
                {
                    e.Row.Background = Brushes.Green;
                }
            }
        }

        private void btnLoadFile_click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = "csv";
                dialog.Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", "csv", ExportFormat.Csv);
                dialog.FilterIndex = 1;

                if (dialog.ShowDialog() == true)
                {
                    _worker.RunWorkerAsync(dialog.FileName);
                }

            }
            catch (Exception exc)
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000003"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                model.ImportData.Clear();
                COSContext.Current.RejectChanges();

                if (_worker.IsBusy)
                    _worker.CancelAsync();

            }
        }

        private void btnImportData_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model.ImportData != null && model.ImportData.Count > 0)
                {
                    _workerImportDB.RunWorkerAsync();
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000004"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
            catch (Exception exc)
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000005"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                model.ImportData.Clear();
                COSContext.Current.RejectChanges();
            }

        }

        private void grvImportStandards_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {
                ImportStandardData data = e.Cell.DataContext as ImportStandardData;

                if (data != null)
                {
                    if (e.Cell.Column.Name == "colItemNumber")
                    {
                        if (string.IsNullOrEmpty(data.NewStandard.ItemNumber))
                        {
                            e.Cell.Background = Brushes.Red;
                            e.Cell.Content = data.ItemNumberText;
                        }
                        else
                        {
                            data.ItemNumberText = "";
                            e.Cell.Background = Brushes.Transparent;
                            e.Cell.Content = data.NewStandard.ItemNumber;
                        }
                    }
                    else if (e.Cell.Column.Name == "colIsConfig")
                    {
                        data.IsConfigText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;

                    }
                    else if (e.Cell.Column.Name == "colItemDescription")
                    {
                        data.DescriptionText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colLabour")
                    {
                        data.LaboursText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colWorkGroup")
                    {
                        data.WorkGroupText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colWorkCenter")
                    {
                        data.WorkCenterText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colItemGroup")
                    {
                        data.ItemGroupsText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colWeight_Kg")
                    {
                        data.WeighText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colSetupTime_mm")
                    {
                        data.SetupTimeText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colPcsPerMinute")
                    {
                        data.PcsPerMinText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }
                    else if (e.Cell.Column.Name == "colPcsPerHour")
                    {
                        data.PcsPerHourText = "";
                        e.Cell.Background = Brushes.Transparent;
                        e.Cell.Content = null;
                    }

                }
            }
        }

        private void grvUnexistsStandards_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }



        private void btnUnexistsFilter_click(object sender, RoutedEventArgs e)
        {
            UnexistsStdFilterWindow wnd = new UnexistsStdFilterWindow(model);
            wnd.Owner = this;
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wndUnexists_Closed);
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();

        }

        void wndUnexists_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                ReloadUnexistsStandards();
            }
            //COSContext.Current.RejectChanges();
        }

        public void ReloadUnexistsStandards()
        {
            try
            {
                model.IsBusy = true;
                _workerGetUnexists.RunWorkerAsync();
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_E00000002"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }

        void _workerGetUnexists_DoWork(object sender, DoWorkEventArgs e)
        {
            model.SetDataForUnexistsStandards();

        }

        void _workerGetUnexists_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            model.IsBusy = false;

            grvUnexistsStandards.ItemsSource = model.ExportDataUnexists;
            grvUnexistsStandards.Rebind();

            RebindUnexists();
        }

        private void RebindUnexists()
        {
            grvUnexistsStandardsHPs.ItemsSource = model.UnexistStandardHPs;

            grvAllStandards.Filtered += new EventHandler<Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs>(grvAllStandards_Filtered);

            if (filters != null)
            {
                grvAllStandards.FilterDescriptors.Clear();
                foreach (var fdsc in filters)
                    grvAllStandards.FilterDescriptors.Add(fdsc);
            }

            grvUnexistsStandardsHPs.Rebind();
        }


        IEnumerable<IFilterDescriptor> filters = null;

        void grvAllStandards_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            filters = e.Added;
        }

        private void btnHPlink_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            if (btn != null)
            {
                HourlyProduction hp = btn.DataContext as HourlyProduction;

                if (hp != null)
                {
                    if (COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "View"))
                    {
                        COSContext.Current.HourlyProductionToNavigate = hp;
                    }
                    else
                    {
                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000014"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });

                    }
                }
            }
        }

        private void btnstdCreateFromHP_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            if (btn != null)
            {
                HourlyProduction hp = btn.DataContext as HourlyProduction;

                if (hp != null)
                {
                    model.TempProduction = hp;
                    if (COS.Common.WPF.Helpers.HasRightForOperation("Standards", "Insert"))
                    {
                        model.AddNewStandard();
                        if (model.SelectedStandard != null)
                        {
                            model.SelectedStandard.ItemNumber = hp.ItemNumber;
                            cmbWorkGroup.SelectedItem = hp.WorkGroup;
                            cmbWorkCenter.SelectedItem = hp.WorkCenter;
                            model.SelectedStandard.isConfig = hp.IsConfig;
                        }
                    }
                    else
                    {
                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000014"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });

                    }
                }
            }
        }

        private void RefreshUnexists_Click(object sender, RoutedEventArgs e)
        {
            ReloadUnexistsStandards();
        }



        private void grvAllStandards_Deleting(object sender, GridViewDeletingEventArgs e)
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


        private void OnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                List<Standard> items = new List<Standard>();
                foreach (var itm in grvAllStandards.SelectedItems)
                    items.Add(itm as Standard);
                model.DeleteSelection(items);

            }
        }

        private void btnrefreshAllStandards_click(object sender, RoutedEventArgs e)
        {
            if (!_workerLoadAll.IsBusy)
                _workerLoadAll.RunWorkerAsync();
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {

            grvAllStandards.ClearAllColumnFilters();
      
        }

    }


}