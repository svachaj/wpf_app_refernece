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

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class ImportPriceListView : BaseUserControl
    {
        public ImportPriceListView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new ImportPriceListViewModel();
                this.DataContext = model;

                //if (COSContext.Current.Language == "cs-CZ")
                //{
                //    grvAllStandards.Columns["cz_itemgroupColumn"].IsVisible = true;
                //    grvAllStandards.Columns["en_itemgroupColumn"].IsVisible = false;
                //}
                //else
                //{
                //    grvAllStandards.Columns["en_itemgroupColumn"].IsVisible = true;
                //    grvAllStandards.Columns["cz_itemgroupColumn"].IsVisible = false;
                //}

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ImportPriceListView_Loaded);

                _worker = new BackgroundWorker();
                _worker.WorkerSupportsCancellation = true;
                _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
                _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);

                _workerImportDB = new BackgroundWorker();
                _workerImportDB.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_workerImportDB_RunWorkerCompleted);
                _workerImportDB.DoWork += new DoWorkEventHandler(_workerImportDB_DoWork);

            }
        }

        void ImportPriceListView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void _workerImportDB_DoWork(object sender, DoWorkEventArgs e)
        {
            model.IsBusy = true;
            model.ImportDataInDB();

        }

        void _workerImportDB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            grvImportPriceList.Rebind();
            model.IsBusy = false;

            model.OnPropertyChanged("DataImportedInDB");
        }

        string errorLoading = "";
        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                model.IsBusy = true;
                errorLoading = "";
                try
                {
                    errorLoading = model.ImportPriceListFromExcel(e.Argument.ToString());
                }
                catch (Exception exc)
                {
                    if (!Debugger.IsAttached)
                        Logging.LogException(exc, LogType.ToFileAndEmail);
                    errorLoading = exc.Message;
                    GC.Collect();
                    e.Cancel = true;
                }
            }
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!string.IsNullOrEmpty(errorLoading))
            {
                if (errorLoading.Contains("5001"))
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_LOG00000001"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
                else if (errorLoading.Contains("5002"))
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_LOG00000002"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
                else if (errorLoading.Contains("5003"))
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_LOG00000003"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
                else
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_LOG00000004"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }

            grvImportPriceList.Rebind();
            model.IsBusy = false;
        }


        BackgroundWorker _worker = null;
        BackgroundWorker _workerImportDB = null;




        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataImportedInDB")
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000002"), Header = ResourceHelper.GetResource<string>("m_Header3_I"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }

        ImportPriceListViewModel model;


        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }


        private void grvImportPriceList_CellLoaded(object sender, Telerik.Windows.Controls.GridView.CellEventArgs e)
        {
            if (e.Cell.ParentRow.Item != null)
            {
                ImportPriceListData data = e.Cell.DataContext as ImportPriceListData;

                if (data != null)
                {
                    if (e.Cell.Column.Name == "colPrice")
                    {
                        if (data.Price == null || data.Price.Value <= 0)
                        {
                            e.Cell.Background = Brushes.Red;
                        }
                    }

                }
            }
        }

        private void grvImportPriceList_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            ImportPriceListData data = e.DataElement as ImportPriceListData;
            if (data != null)
            {
                if (!data.IsNew)
                {
                    e.Row.Background = Brushes.Blue;
                }
                else
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
                dialog.DefaultExt = ".xls";
                dialog.Filter = "Excel files|*.xls;*.xlsx";
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

        private void grvImportPriceList_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {
                ImportPriceListData data = e.Cell.DataContext as ImportPriceListData;

                if (data != null)
                {
                    if (e.Cell.Column.Name == "colPrice")
                    {
                        if (data.Price != null && data.Price.Value > 0)
                        {
                            e.Cell.Background = Brushes.Transparent;
                        }
                        else
                        {
                            e.Cell.Background = Brushes.Red;
                        }
                    }

                }
            }
        }


    }


}
