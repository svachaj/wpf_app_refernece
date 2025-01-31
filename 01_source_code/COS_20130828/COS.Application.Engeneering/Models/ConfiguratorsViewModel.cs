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

namespace COS.Application.Engeneering.Models
{
    public partial class ConfiguratorsViewModel : ValidationViewModelBase
    {
        public ConfiguratorsViewModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TpmConfiguratorsViewModel_PropertyChanged);
        }

        void TpmConfiguratorsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }


        private Configurator _selectedItem = null;
        public Configurator SelectedItem
        {
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                    SelectedFormItem = _selectedItem.FormItems.FirstOrDefault(a => a.Name != "labour" && a.Name != "setuptime" && a.Name != "weight" && a.Name != "textlabour" && a.Name != "textsetuptime" && a.Name != "textweight" && a.Name != "itemdescription" && a.Name != "textitemdescription");
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        public List<Configurator> LocalConfigurators
        {
            get
            {
                return COSContext.Current.Configurators.Where(a => !a.IsMatrix).ToList();
            }
        }

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
                return new RelayCommand(param => this.UpdateData());
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
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000015"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
            }
        }


        private void Cancel()
        {
            COSContext.Current.RejectChanges();
            EditingMode = EditMode.AllMode;

            OnPropertyChanged("CancelDataCompleted");
        }


        private void UpdateData()
        {
            try
            {
                var err = ValidData();

                if (string.IsNullOrEmpty(err))
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    EditingMode = EditMode.AllMode;

                    OnPropertyChanged("UpdateDataCompleted");
                }
                else
                    Errors = err;
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000015"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
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
                    result += ResourceHelper.GetResource<string>("m_Body_ENG00000017");
                    result += Environment.NewLine;
                }

                if (string.IsNullOrEmpty(SelectedItem.Formula))
                {
                    result += ResourceHelper.GetResource<string>("m_Body_ENG00000018");
                    result += Environment.NewLine;
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

            COSContext.Current.Configurators.AddObject(config);

            FormInput input = COSContext.Current.FormInputs.FirstOrDefault(a => a.SystemType == "System.Double");

            ConfiguratorFormItem formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.DoubleValue = 0;
            formItem.Name = "labour";
            formItem.Width = 130;
            formItem.Height = 24;
            formItem.TopPosition = 20;
            formItem.LeftPosition = 120;

            config.FormItems.Add(formItem);

            formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.DoubleValue = 0;
            formItem.Name = "setuptime";
            formItem.Width = 130;
            formItem.Height = 24;
            formItem.TopPosition = 60;
            formItem.LeftPosition = 120;

            config.FormItems.Add(formItem);

            formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.DoubleValue = 0;
            formItem.Name = "weight";
            formItem.Width = 130;
            formItem.Height = 24;
            formItem.TopPosition = 100;
            formItem.LeftPosition = 120;

            config.FormItems.Add(formItem);

            input = COSContext.Current.FormInputs.FirstOrDefault(a => a.SystemType == "Label");
           
            formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.Name = "textlabour";
            formItem.Width = 80;
            formItem.Height = 24;
            formItem.TopPosition = 20;
            formItem.LeftPosition = 40;
            formItem.Text = "Operátoři";

            config.FormItems.Add(formItem);

            formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.Name = "textsetuptime";
            formItem.Width = 80;
            formItem.Height = 24;
            formItem.TopPosition = 60;
            formItem.LeftPosition = 40;
            formItem.Text = "Čas setupu";

            config.FormItems.Add(formItem);

            formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.Name = "textweight";
            formItem.Width = 80;
            formItem.Height = 24;
            formItem.TopPosition = 100;
            formItem.LeftPosition = 40;
            formItem.Text = "Váha";

            config.FormItems.Add(formItem);

            formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.Name = "textitemdescription";
            formItem.Width = 80;
            formItem.Height = 24;
            formItem.TopPosition = 140;
            formItem.LeftPosition = 40;
            formItem.Text = "Popis";

            config.FormItems.Add(formItem);

            input = COSContext.Current.FormInputs.FirstOrDefault(a => a.SystemType == "System.String");

            formItem = new ConfiguratorFormItem();
            formItem.Input = input;
            formItem.Name = "itemdescription";
            formItem.Width = 130;
            formItem.Height = 24;
            formItem.TopPosition = 140;
            formItem.LeftPosition = 120;            

            config.FormItems.Add(formItem);


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



    }


}
