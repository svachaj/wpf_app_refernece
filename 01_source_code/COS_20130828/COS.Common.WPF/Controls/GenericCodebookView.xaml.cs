using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using COS.Application.Shared;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls.GridView;
using System.Windows;
using System.Windows.Input;
using COS.Resources;
using System.Collections;

namespace COS.Common.WPF.Controls
{
    public partial class GenericCodebookView : BaseUserControl
    {
        public GenericCodebookView(COSContext cosContext, string entitySetName)
        {
            InitializeComponent();

            this.DataContext = this;
            this.cosContext = cosContext;

            EntitySetName = entitySetName;
        }

        COSContext cosContext = null;

        public string EntitySetName { set; get; }

        public RadWindow RaiseWindow { set; get; }

        public Type SourceDataType { set; get; }
        List<GridViewColumnDefinition> definitions = null;

        public ObservableCollection<T> TransformData<T>(List<T> data, List<GridViewColumnDefinition> definitions)
        {
            SourceDataType = typeof(T);
            ObservableCollection<T> result = new ObservableCollection<T>();

            foreach (var itm in data)
            {
                result.Add(itm);
            }

            grvMainData.AutoGenerateColumns = false;

            GridViewDataColumn col = null;
            GridViewComboBoxColumn cmbCol = null;

            this.definitions = definitions;

            var toGenerate = definitions.Where(a => a.GenerateColumn);
            foreach (var itm in toGenerate)
            {
                if (itm.ItemsSource != null)
                {
                    cmbCol = new GridViewComboBoxColumn();
                    cmbCol.DataMemberBinding = new Binding(itm.Name);
                    cmbCol.Header = itm.HeaderText;
                    cmbCol.UniqueName = itm.Name;
                    cmbCol.SelectedValueMemberPath = itm.SelectedValueMemberPath;
                    cmbCol.DisplayMemberPath = itm.DisplayMemberPath;
                    cmbCol.ItemsSource = itm.ItemsSource;

                    grvMainData.Columns.Add(cmbCol);
                }
                else if (itm.IsLocalize)
                {
                    col = new GridViewDataColumn();
                    col.UniqueName = itm.Name + "cs_Czech";
                    col.Header = itm.HeaderText + " česky";
                    col.DataMemberBinding = new Binding(itm.Name + ".cs_Czech");
                    grvMainData.Columns.Add(col);

                    col = new GridViewDataColumn();
                    col.UniqueName = itm.Name + "en_English";
                    col.Header = itm.HeaderText + " anglicky";
                    col.DataMemberBinding = new Binding(itm.Name + ".en_English");
                    grvMainData.Columns.Add(col);
                }
                else
                {
                    col = new GridViewDataColumn();
                    col.UniqueName = itm.Name;
                    col.Header = itm.HeaderText;
                    col.DataMemberBinding = new Binding(itm.Name);
                    grvMainData.Columns.Add(col);
                }

                if (toGenerate.Count() == 1)
                    col.Width = new GridViewLength(1, GridViewLengthUnitType.Star);

            }

            grvMainData.ItemsSource = result;

            return result;
        }


        object tempNewObject = null;
        int maxLocalizeID = 0;
        private void grvMainData_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var newObj = System.Reflection.Assembly.GetAssembly(this.cosContext.GetType()).CreateInstance(SourceDataType.FullName);

            foreach (var def in this.definitions.Where(a => a.DefaultValue != null))
            {
                newObj.GetType().GetProperty(def.Name).SetValue(newObj, def.DefaultValue, null);
            }

            SysLocalize localize = null;
            foreach (var def in this.definitions.Where(a => a.IsLocalize == true))
            {
                localize = this.cosContext.CreateObject<SysLocalize>();
                if (maxLocalizeID == 0)
                    maxLocalizeID = this.cosContext.SysLocalizes.Max(a => a.ID) + 1;
                else
                    maxLocalizeID++;

                localize.ID = maxLocalizeID;

                this.cosContext.SysLocalizes.AddObject(localize);

                newObj.GetType().GetProperty(def.Name).SetValue(newObj, localize, null);
            }

            e.NewObject = newObj;
            tempNewObject = e.NewObject;

            this.cosContext.AddObject(EntitySetName, e.NewObject);
        }

        private void ButtonInsert_Click(object sender, RoutedEventArgs e)
        {
            grvMainData.BeginInsert();
        }

        private void grvMainData_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            foreach (var itm in e.Items)
                this.cosContext.DeleteObject(itm);
        }

        private void grvMainData_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction == GridViewEditAction.Cancel)
            {
                if (tempNewObject != null)
                    this.cosContext.Detach(tempNewObject);
            }
            else if (e.EditAction == GridViewEditAction.Commit)
            {
                if (e.EditOperationType == GridViewEditOperationType.Insert)
                {
                    tempNewObject = null;
                }
            }
        }

        private void grvMainData_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {

        }

        private void grvMainData_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.NewValue == null || e.NewValue.ToString().IsNullOrEmptyString())
            {
                e.IsValid = false;
                e.ErrorMessage = ResourceHelper.GetResource<string>("m_Body_LOG00000040");
            }
        }

        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNew());
            }
        }

        public ICommand UpdateToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }

        public ICommand CancelToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }

        private void Cancel()
        {
            grvMainData.CancelEdit();
        }

        private void Delete()
        {
            if (grvMainData.SelectedItem != null)
            {
                var ent = grvMainData.SelectedItem;

                this.cosContext.DeleteObject(ent);
                grvMainData.Items.Remove(grvMainData.SelectedItem);
            }
        }

        private void Save()
        {
            string error = "";

            foreach (var itm in grvMainData.Items)
            {
                foreach (var vl in this.definitions)
                {
                    if (vl.IsMandatory)
                    {
                        var val = itm.GetType().GetProperty(vl.Name).GetValue(itm, null);

                        if (val == null || val.ToString().IsNullOrEmptyString())
                        {
                            error += ResourceHelper.GetResource<string>("m_Body_LOG00000041") + vl.HeaderText.ToLower() + ResourceHelper.GetResource<string>("m_Body_LOG00000042") + Environment.NewLine;
                        }
                    }
                }
            }

            if (error.IsNullOrEmptyString())
            {
                this.cosContext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = true;
                    RaiseWindow.Close();
                }
            }
            else
            {
                var strerr = ResourceHelper.GetResource<string>("m_Body_LOG00000043") + Environment.NewLine + error;

                RadWindow.Alert(new DialogParameters() { Header = ResourceHelper.GetResource<string>("m_Header1_A"), Content = strerr, Owner = (RadWindow)COSContext.Current.RadMainWindow, DialogStartupLocation = WindowStartupLocation.CenterOwner });
            }
        }

        private void AddNew()
        {
            grvMainData.BeginInsert();
        }

        private void grvMainData_DataError(object sender, DataErrorEventArgs e)
        {

        }
    }


    public class GridViewColumnDefinition
    {
        public bool GenerateColumn { set; get; }
        public string Name { set; get; }
        public string HeaderText { set; get; }
        public bool IsMandatory { set; get; }
        public object DefaultValue { set; get; }
        public string DisplayMemberPath { set; get; }
        public string SelectedValueMemberPath { set; get; }
        public IEnumerable ItemsSource { set; get; }
        public bool IsLocalize { set; get; }

    }
}
