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
using COS.Application.Engeneering.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Windows.Threading;
using COS.Application.Controls;
using Microsoft.Win32;
using System.IO;

namespace COS.Application.Engeneering.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class MatrixConfiguratorsView : BaseUserControl
    {
        public MatrixConfiguratorsView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new MatrixConfiguratorsViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                Loaded += new RoutedEventHandler(TpmPlanOverviewView_Loaded);
            }
        }



        void TpmPlanOverviewView_Loaded(object sender, RoutedEventArgs e)
        {
            Model.EditingMode = Common.EditMode.AllMode;

            //cmbGroups.ItemsSource = null;
            //cmbGroups.ItemsSource = Model.LocalConfiguratorGroups;

            grvConfigs.SelectedItem = Model.LocalConfigurators.FirstOrDefault();


        }

        Engeneering.Models.MatrixConfiguratorsViewModel Model = null;



        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateDataCompleted")
            {
                ConfiguratorGroup grp = cmbGroupsAll.SelectedItem as ConfiguratorGroup;
                if (grp != null)
                {
                    var prevSelectedItem = Model.SelectedItem;
                    Model.LocalConfigurators = COSContext.Current.Configurators.Where(a => a.ID_ConfigGroup == grp.ID && a.IsMatrix == true).ToList();
                    Model.SelectedGroup = grp;

                    Model.SelectedItem = prevSelectedItem;
                }
                grvConfigs.Rebind();
                //grvConfigs.SelectedItem = Model.LocalConfigurators.LastOrDefault();
            }
            else if (e.PropertyName == "Errors")
            {
                RadWindow.Alert(new DialogParameters() { Content = Model.Errors, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
            else if (e.PropertyName == "InsertDataCompleted")
            {
                tbxName.Focus();
                grvConfigs.Rebind();
            }
            else if (e.PropertyName == "CancelDataCompleted")
            {
                grvConfigs.SelectedItem = Model.LocalConfigurators.FirstOrDefault();
            }
            else if (e.PropertyName == "DeleteDataCompleted")
            {
                ConfiguratorGroup grp = cmbGroupsAll.SelectedItem as ConfiguratorGroup;
                if (grp != null)
                {
                    Model.LocalConfigurators = COSContext.Current.Configurators.Where(a => a.ID_ConfigGroup == grp.ID && a.IsMatrix == true).ToList();
                    Model.SelectedGroup = grp;
                }
                grvConfigs.Rebind();
                grvConfigs.SelectedItem = Model.LocalConfigurators.FirstOrDefault();
            }
            else if (e.PropertyName == "SelectedItem")
            {
                if (Model.SelectedItem != null)
                    gridSelectedItem.IsEnabled = true;
                else
                    gridSelectedItem.IsEnabled = false;

                GenerateFormItems();

            }

        }

        ConfiguratorFormControl ctrl = null;
        private void testFormula_click(object sender, RoutedEventArgs e)
        {
            RadWindow wnd = new RadWindow();

            if (Model.SelectedItem != null)
                wnd.Header = Model.SelectedItem.Name;

            StyleManager.SetTheme(wnd, new Expression_DarkTheme());

            wnd.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;


            ctrl = new ConfiguratorFormControl(Model.SelectedItem);
            ctrl.PropertyChanged += new PropertyChangedEventHandler(ctrl_PropertyChanged);

            wnd.Content = ctrl;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();


        }

        void ctrl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Result")
            {
                //MessageBox.Show(ctrl.Result.ToString());
            }
        }



        public void GenerateFormItems()
        {
            if (Model.SelectedItem != null)
                cnvMainMatrix.GenerateControls(Model.SelectedItem.FormItems.ToList());
            else
                cnvMainMatrix.GenerateControls(null);
        }



        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".xls";
                dialog.Filter = "Excel files|*.xls;*.xlsx";
                dialog.FilterIndex = 1;

                if (dialog.ShowDialog() == true)
                {
                    Model.ImportDataFromExcel(dialog.FileName);

                    if (Model.ImportData != null && Model.ImportData.Count > 0)
                    {
                        foreach (var itm in Model.SelectedItem.FormItems.ToList())
                            COSContext.Current.ConfiguratorFormItems.DeleteObject(itm);

                        ConfiguratorFormItem cItem = null;
                        foreach (var itm in Model.ImportData)
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
                                string yval = Model.ImportData.Where(a => a.HeaderText != null && a.RowIndex == itm.RowIndex).FirstOrDefault().HeaderText;
                                string xval = Model.ImportData.Where(a => a.HeaderText != null && a.ColumnIndex == itm.ColumnIndex).FirstOrDefault().HeaderText;
                                cItem.HelpValue = xval + ";" + yval;
                            }

                            COSContext.Current.ConfiguratorFormItems.AddObject(cItem);
                            Model.SelectedItem.FormItems.Add(cItem);
                        }

                        GenerateFormItems();
                    }

                    if (Model.EditingMode != Common.EditMode.New)
                        Model.UpdateData(false);
                }


            }
            catch (Exception exc)
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000003"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                COSContext.Current.RejectChanges();
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedItem != null)
            {
                try
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.DefaultExt = ".xlsx";
                    dialog.Filter = "Excel files|*.xlsx";
                    dialog.FilterIndex = 1;
                    dialog.FileName = "Konfigurátor_" + Model.SelectedItem.Name;

                    if (dialog.ShowDialog() == true)
                    {
                        Model.ExportDataToExcel(dialog.FileName);
                    }
                }
                catch (Exception exc)
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000003"), Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    COSContext.Current.RejectChanges();
                }
            }
        }

        private void cmbGroupsAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfiguratorGroup grp = cmbGroupsAll.SelectedItem as ConfiguratorGroup;
            if (grp != null)
            {
                Model.LocalConfigurators = COSContext.Current.Configurators.Where(a => a.ID_ConfigGroup == grp.ID && a.IsMatrix == true).ToList();
                Model.SelectedGroup = grp;
            }
            Model.SelectedItem = Model.LocalConfigurators.FirstOrDefault();
        }

        private void btnExportAll_click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var lisconf = COSContext.Current.Configurators.Where(a => a.IsMatrix == true).ToList();

                foreach (var itm in lisconf)
                {
                    var filename = dialog.SelectedPath + @"\Konfigurátor_" + itm.Name + ".xlsx";

                    if (File.Exists(filename))
                        File.Delete(filename);

                    Model.ExportDataToExcel(filename, itm.FormItems.ToList());
                }
            }
        }

        private void wcsSet_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedItem != null && Model.SelectedItem.Division != null)
            {
                ConfiguratorWCSWindow wnd = new ConfiguratorWCSWindow(Model);

                wnd.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Owner = (RadWindow)COSContext.Current.RadMainWindow, Content = "Nastavte nejprve divizi prosím.", Header = "Upozornění" });
            }
        }



    }
}
