using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace COS.Application.Engeneering.Models
{
    public partial class ConfiguratorGroupsViewModel : ValidationViewModelBase
    {
        public ConfiguratorGroupsViewModel()
            : base()
        {
            RefreshData();
            EditingMode = EditMode.AllMode;
        }

        public void RefreshData()
        {
            LocalConfiguratorGroups = COSContext.Current.ConfiguratorGroups.ToList().Where(a => a.ParentID == null).ToList();
            SelectedItem = LocalConfiguratorGroups.FirstOrDefault();
        }

        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNew());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }

        public ICommand UpdateToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.UpdateData());
            }
        }

        private void UpdateData()
        {
            try
            {
                if (EditingMode == EditMode.New)
                    COSContext.Current.ConfiguratorGroups.AddObject(SelectedItem);
              

                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                //RadWindow.Alert(new DialogParameters() { Content = "Při ukládání se vyskytla neočekávaná chyba!" });
            }

            if (EditingMode == EditMode.New)
            {

                //RefreshData();
            }
            else
            {

            }

            EditingMode = EditMode.AllMode;
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
            COSContext.Current.RejectChanges();

          //  RefreshData();

            EditingMode = EditMode.AllMode;

        }

        public void Delete()
        {
            if (SelectedItem != null)
            {
                COSContext.Current.ConfiguratorGroups.DeleteObject(SelectedItem);


                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                    //RadWindow.Alert(new DialogParameters() { Content = "Při ukládání se vyskytla neočekávaná chyba!" });
                }

               // RefreshData();
            }
        }


        public void AddNew()
        {
            if (SelectedItem != null)
            {
                var parent = SelectedItem;

                ConfiguratorGroup newSelectedItem = new ConfiguratorGroup();

                newSelectedItem.ParentID = parent.ID;

                SysLocalize tempLocalize = new SysLocalize();
                tempLocalize.ID = COSContext.Current.SysLocalizes.Max(a => a.ID) + 1;
                COSContext.Current.SysLocalizes.AddObject(tempLocalize);
                newSelectedItem.SysLocalize = tempLocalize;

                SelectedItem = newSelectedItem;


                EditingMode = EditMode.New;
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


        private ConfiguratorGroup _selectedItem = null;
        public ConfiguratorGroup SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    if (_selectedItem != null)
                    {
                        if (_selectedItem.SysLocalize != null)
                            _selectedItem.SysLocalize.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SysLocalize_PropertyChanged);
                    }
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        void SysLocalize_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }



    }
}
