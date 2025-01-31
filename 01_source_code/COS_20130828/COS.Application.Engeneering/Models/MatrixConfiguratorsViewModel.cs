using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using COS.Application.Engeneering.Views;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace COS.Application.Engeneering.Models
{
    public partial class MatrixConfiguratorsViewModel : ValidationViewModelBase
    {
        public MatrixConfiguratorsViewModel()
            : base()
        {
            //  LocalConfigurators = COSContext.Current.Configurators.Where(a => a.IsMatrix).ToList(); 
            LocalConfiguratorGroups = COSContext.Current.ConfiguratorGroups.ToList().Where(a => a.ParentID == null).ToList();
        }



        private Configurator _selectedItem = null;
        public Configurator SelectedItem
        {
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    ImportData.Clear();
                }
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        public ConfiguratorGroup SelectedGroup { set; get; }

        private ConfiguratorFormItem _selectedFormItem = null;
        public ConfiguratorFormItem SelectedFormItem
        {
            set
            {
                _selectedFormItem = value;
                OnPropertyChanged("SelectedFormItem");
            }
            get
            {
                return _selectedFormItem;
            }
        }

        private string _errros = "";
        public string Errors
        {
            set
            {
                _errros = value;
                OnPropertyChanged("Errors");
            }
            get
            {
                return _errros;
            }
        }



        public ICommand UpdateToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.UpdateData(true));
            }
        }

        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.InsertData());
            }
        }

        public ICommand CancelToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.DeleteData());
            }
        }

        private void DeleteData()
        {
            try
            {

                if (SelectedItem != null)
                {
                    COSContext.Current.Configurators.DeleteObject(SelectedItem);

                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                    OnPropertyChanged("DeleteDataCompleted");
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000015"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }


        private void Cancel()
        {
            COSContext.Current.RejectChanges();
            EditingMode = EditMode.AllMode;

            OnPropertyChanged("CancelDataCompleted");
        }

        private List<Configurator> _localConfigurators = new List<Configurator>();
        public List<Configurator> LocalConfigurators
        {
            get
            {
                return _localConfigurators;
            }
            set
            {
                _localConfigurators = value;
                OnPropertyChanged("LocalConfigurators");
            }
        }

        public void UpdateData(bool refresh)
        {
            try
            {
                var err = ValidData();

                if (string.IsNullOrEmpty(err))
                {
                    if (ImportData.Count > 0)
                    {

                        foreach (var itm in SelectedItem.FormItems.ToList())
                            COSContext.Current.ConfiguratorFormItems.DeleteObject(itm);

                        ConfiguratorFormItem cItem = null;
                        foreach (var itm in ImportData)
                        {
                            cItem = COSContext.Current.ConfiguratorFormItems.CreateObject();
                            cItem.Name = "matrixItem" + itm.RowIndex.ToString() + "-" + itm.ColumnIndex.ToString();
                            cItem.LeftPosition = itm.ColumnIndex;
                            cItem.TopPosition = itm.RowIndex;

                            if (itm.IsHeader)
                            {
                                cItem.StringValue = itm.HeaderText;
                            }
                            else
                            {
                                cItem.Labours = itm.Labours;
                                cItem.SetupTime = itm.SetupTime;
                                cItem.DecimalValue = itm.ValuePcs;
                                string yval = ImportData.Where(a => a.HeaderText != null && a.RowIndex == itm.RowIndex).FirstOrDefault().HeaderText;
                                string xval = ImportData.Where(a => a.HeaderText != null && a.ColumnIndex == itm.ColumnIndex).FirstOrDefault().HeaderText;
                                cItem.HelpValue = xval + ";" + yval;
                            }

                            COSContext.Current.ConfiguratorFormItems.AddObject(cItem);
                            SelectedItem.FormItems.Add(cItem);
                        }
                    }

                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    EditingMode = EditMode.AllMode;

                    if (refresh)
                        OnPropertyChanged("UpdateDataCompleted");
                }
                else
                    Errors = err;
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000015"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }

        }

        private string ValidData()
        {
            string result = "";
            if (SelectedItem != null)
            {
                if (string.IsNullOrEmpty(SelectedItem.Name))
                {
                    result += ResourceHelper.GetResource<string>("m_Body_ENG00000016");
                    result += Environment.NewLine;
                }
                if (SelectedItem.Division == null)
                {
                    result += "Vyberte divizi";// ResourceHelper.GetResource<string>("m_Body_ENG00000017");
                    result += Environment.NewLine;
                }
                if (SelectedItem.ConfiguratorGroup == null)
                {
                    result += "Vyberte skupinu";// ResourceHelper.GetResource<string>("m_Body_ENG00000017");
                    result += Environment.NewLine;
                }

                if (EditingMode == EditMode.New)
                {
                    if (ImportData.Count == 0)
                    {
                        result += ResourceHelper.GetResource<string>("m_Body_ENG00000020");
                        result += Environment.NewLine;
                    }
                }
            }
            return result;
        }

        private void InsertData()
        {

            Configurator config = COSContext.Current.Configurators.CreateObject();

            config.CreatedByID = COSContext.Current.CurrentUser.ID;
            config.UpdatedByID = COSContext.Current.CurrentUser.ID;
            config.UpdateDate = COSContext.Current.DateTimeServer;
            config.IsMatrix = true;
            config.Formula = "";
            config.ConfiguratorGroup = SelectedGroup;

            COSContext.Current.Configurators.AddObject(config);

            SelectedItem = config;

            EditingMode = EditMode.New;

            OnPropertyChanged("InsertDataCompleted");
        }

        private List<Configurator> _configurators = new List<Configurator>();
        List<Configurator> Configurators
        {
            get
            {
                return _configurators;
            }
            set
            {
                if (_configurators != value)
                {
                    _configurators = value;
                    OnPropertyChanged("Configurators");
                }
            }
        }


        private List<ConfiguratorGroup> _localConfiguratorGroups = new List<ConfiguratorGroup>();
        public List<ConfiguratorGroup> LocalConfiguratorGroups
        {
            get
            {
                return _localConfiguratorGroups;
            }
            set
            {
                if (_localConfiguratorGroups != value)
                {
                    _localConfiguratorGroups = value;
                    OnPropertyChanged("LocalConfiguratorGroups");
                }
            }
        }



        public List<ImportMatrixConfiguratorData> ImportData = new List<ImportMatrixConfiguratorData>();

        public void ImportDataFromExcel(string filename)
        {
            ImportData.Clear();
            ImportMatrixConfiguratorData data = null;


            int rowIndex = 1;
            int startRowIndex = 1;
            int columnIndex = 1;
            int dataRowIndex = 0;
            int dataColumnIndex = 0;
            int startRowDataIndex = 0;
            int startColumnDataIndex = 0;

            bool startDataFound = true;

            using (COS.Excel.COSExcel excel = new COS.Excel.COSExcel(filename))
            {

                var rowData = excel.GetData(1, rowIndex, columnIndex);


                while (rowData == null)
                {
                    if (rowIndex < 5)
                        rowIndex++;

                    if (columnIndex < 5)
                        columnIndex++;


                    rowData = excel.GetData(1, rowIndex, columnIndex);

                    if (rowData == null && rowIndex == 5 && columnIndex == 5)
                    {
                        startDataFound = false;
                        break;
                    }
                    else if (rowData != null)
                        break;

                }

                if (startDataFound)
                {
                    startColumnDataIndex = dataColumnIndex;
                    startRowDataIndex = dataRowIndex;
                    startRowIndex = rowIndex;

                    data = new ImportMatrixConfiguratorData();
                    data.IsHeader = true;
                    data.HeaderText = rowData.ToString();
                    data.RowIndex = dataRowIndex;
                    data.ColumnIndex = dataColumnIndex;

                    dataRowIndex++;

                    ImportData.Add(data);

                    rowIndex++;
                    rowData = excel.GetData(1, rowIndex, columnIndex);
                    while (rowData != null)
                    {

                        data = new ImportMatrixConfiguratorData();
                        data.RowIndex = dataRowIndex;
                        data.ColumnIndex = dataColumnIndex;

                        if (dataColumnIndex == 0 || dataRowIndex == 0)
                        {
                            data.IsHeader = true;
                            data.HeaderText = rowData.ToString();
                        }
                        else
                        {
                            data.ValuePcs = Math.Round(decimal.Parse(rowData.ToString()), 2);

                            rowData = excel.GetData(1, rowIndex, columnIndex + 1);
                            data.Labours = int.Parse(rowData.ToString());

                            rowData = excel.GetData(1, rowIndex, columnIndex + 2);
                            data.SetupTime = int.Parse(rowData.ToString());
                        }

                        ImportData.Add(data);

                        dataRowIndex++;

                        rowIndex++;
                        rowData = excel.GetData(1, rowIndex, columnIndex);

                        if (rowData == null)
                        {
                            dataRowIndex = startRowDataIndex;
                            rowIndex = startRowIndex;

                            if (dataColumnIndex == 0)
                            {
                                columnIndex++;
                                dataColumnIndex++;
                            }
                            else
                            {
                                columnIndex += 3;
                                dataColumnIndex++;
                            }

                            rowData = excel.GetData(1, rowIndex, columnIndex);
                        }
                    }


                }
                else
                {
                    throw new Exception(ResourceHelper.GetResource<string>("m_Body_ENG00000021"));

                }
            }

        }

        public void ExportDataToExcel(string filename)
        {
            if (SelectedItem != null && SelectedItem.FormItems != null && SelectedItem.FormItems.Count > 0)
            {

                using (COS.Excel.COSExcel excel = new COS.Excel.COSExcel())
                {

                    foreach (var itm in SelectedItem.FormItems)
                    {
                        object val = null;
                        if (itm.DecimalValue.HasValue)
                            val = itm.DecimalValue.Value;
                        else
                            val = itm.StringValue;

                        int row = (int)itm.TopPosition.Value + 1;
                        int col = (int)itm.LeftPosition.Value + 1;

                        if (col > 2)
                        {
                            col = 2 * (int)itm.LeftPosition.Value + (int)itm.LeftPosition.Value - 2;
                            col++;

                            if (itm.DecimalValue.HasValue)
                            {
                                excel.SetData(1, row + 1, col + 1, val);
                                excel.SetData(1, row + 1, col + 2, itm.Labours);
                                excel.SetData(1, row + 1, col + 3, itm.SetupTime);
                            }
                            else
                            {
                                excel.SetData(1, row + 1, col + 1, val);
                            }
                        }
                        else
                        {
                            if (itm.DecimalValue.HasValue)
                            {
                                excel.SetData(1, row + 1, col + 2, itm.Labours);
                                excel.SetData(1, row + 1, col + 3, itm.SetupTime);
                            }

                            excel.SetData(1, row + 1, col + 1, val);
                        }
                    }

                    excel.Save(filename);
                }
            }
        }


        public void ExportDataToExcel(string filename, List<ConfiguratorFormItem> items)
        {
            if (items != null)
            {

                using (COS.Excel.COSExcel excel = new COS.Excel.COSExcel())
                {

                    foreach (var itm in items)
                    {
                        object val = null;
                        if (itm.DecimalValue.HasValue)
                            val = itm.DecimalValue.Value;
                        else
                            val = itm.StringValue;

                        int row = (int)itm.TopPosition.Value + 1;
                        int col = (int)itm.LeftPosition.Value + 1;

                        if (col > 2)
                        {
                            col = 2 * (int)itm.LeftPosition.Value + (int)itm.LeftPosition.Value - 2;
                            col++;

                            if (itm.DecimalValue.HasValue)
                            {
                                excel.SetData(1, row + 1, col + 1, val);
                                excel.SetData(1, row + 1, col + 2, itm.Labours);
                                excel.SetData(1, row + 1, col + 3, itm.SetupTime);
                            }
                            else
                            {
                                excel.SetData(1, row + 1, col + 1, val);
                            }
                        }
                        else
                        {
                            if (itm.DecimalValue.HasValue)
                            {
                                excel.SetData(1, row + 1, col + 2, itm.Labours);
                                excel.SetData(1, row + 1, col + 3, itm.SetupTime);
                            }

                            excel.SetData(1, row + 1, col + 1, val);
                        }
                    }

                    excel.Save(filename);
                }
            }
        }

    }


}
