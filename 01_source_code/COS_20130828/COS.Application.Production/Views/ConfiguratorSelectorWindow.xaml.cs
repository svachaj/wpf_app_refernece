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
using System.Windows.Shapes;
using COS.Application.Production.Models;
using Telerik.Windows.Controls;
using System.ComponentModel;
using COS.Application.Shared;
using COS.Application.Controls;
using COS.Resources;
using COS.Common.WPF.Controls;

namespace COS.Application.Production.Views
{
    /// <summary>
    /// Interaction logic for KpiReportFilterWindow.xaml
    /// </summary>
    public partial class ConfiguratorSelectorWindow : RadWindow, INotifyPropertyChanged
    {
        public ConfiguratorSelectorWindow(HourlyProductionMainViewModel model)
        {
            InitializeComponent();

            Production = model.SelectedHourlyProduction;
            Model = model;

            LocalConfiguratorGroups = COSContext.Current.ConfiguratorGroups.ToList().Where(a => a.ParentID == null).ToList();
            LocalConfigurators = COSContext.Current.Configurators.Where(a => a.ID_Division == Production.ID_Division && a.ConfiguratorWorkCenters.Where(c => c.ID_WorkCenter == c.ID_WorkCenter).Count() > 0).ToList();

            this.DataContext = this;

        }

        HourlyProduction Production { set; get; }
        HourlyProductionMainViewModel Model { set; get; }

        private Configurator _selectedItem = null;
        public Configurator SelectedItem
        {
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        List<Configurator> _localConfigurators = null;
        public List<Configurator> LocalConfigurators
        {
            set
            {
                _localConfigurators = value;
                OnPropertyChanged("LocalConfigurators");
            }
            get
            {
                return _localConfigurators;
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



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                OpenConfig();
            }
        }

        private void grvConfigs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem != null)
            {
                OpenConfig();
            }
        }


        ConfiguratorFormControl ctrl = null;
        MatrixConfiguratorGrid matrix = null;
        RadWindow wnd = null;

        private void OpenConfig()
        {
            wnd = new RadWindow();


            //   wnd.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            wnd.Header = SelectedItem.Name;

            if (SelectedItem.IsMatrix)
            {
                var xmax = SelectedItem.FormItems.Max(a => a.LeftPosition) * 50;
                var ymax = SelectedItem.FormItems.Max(a => a.TopPosition) * 30;

                wnd.MinHeight = ymax.HasValue ? ymax.Value + 120 : 600;
                wnd.MinWidth = xmax.HasValue ? xmax.Value + 80 : 800;
                matrix = new MatrixConfiguratorGrid();
                matrix.IsComputeMode = true;

                matrix.PropertyChanged += new PropertyChangedEventHandler(matrix_PropertyChanged);
                wnd.Content = matrix;

                matrix.GenerateControls(SelectedItem.FormItems.ToList());
                StyleManager.SetTheme(wnd, new Expression_DarkTheme());
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }
            else
            {
                ctrl = new ConfiguratorFormControl(SelectedItem);
                ctrl.PropertyChanged += new PropertyChangedEventHandler(ctrl_PropertyChanged);

                wnd.Content = ctrl;
                StyleManager.SetTheme(wnd, new Expression_DarkTheme());
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }

        }

        void matrix_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "Result")
            {

                MatrixConfiguratorGrid formControl = sender as MatrixConfiguratorGrid;
                if (formControl != null && formControl.Result != null)
                {
                    if (wnd != null)
                    {
                        //wnd.Visibility = System.Windows.Visibility.Collapsed;
                        wnd.Close();

                    }

                    //tady vytvoříme normu....
                    string itemDesc = "";
                    string[] xyvals = formControl.Result.HelpValue.Split(';');
                    if (xyvals.Length > 1)
                        itemDesc = formControl.Result.Configurator.Name + " - " + xyvals[0] + " - " + xyvals[1];
                    else
                        itemDesc = formControl.Result.Configurator.Name;


                    var existsDescStand = COSContext.Current.Standards.FirstOrDefault(a => a.ID_WorkCenter == Production.WorkCenter.ID && a.ItemDescription == itemDesc);


                    if (existsDescStand != null)
                    {
                        Model.SelectedHourlyProduction.ItemNumber = existsDescStand.ItemNumber;
                    }
                    else
                    {
                        Standard stand = COSContext.Current.Standards.CreateObject();

                        stand.WorkCenter = Production.WorkCenter;
                        stand.WorkGroup = Production.WorkGroup;
                        stand.ItemNumber = Production.ItemNumber;
                        stand.ID_Standard = stand.WorkCenter.Value + stand.ItemNumber;
                        //stand.ItemGroup = SelectedItem.ItemGroup;
                        stand.isConfig = true;

                        stand.CreateDate = COSContext.Current.DateTimeServer;
                        stand.ModifyDate = COSContext.Current.DateTimeServer;
                        stand.CreatedBy_UID = COSContext.Current.CurrentUser.ID;
                        stand.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;

                        stand.Labour = formControl.Result.Labours.Value;
                        stand.SetupTime_mm = formControl.Result.SetupTime.Value;
                        stand.Weight_Kg = 0;

                        stand.ItemDescription = itemDesc;

                        stand.PcsPerHour = formControl.Result.DecimalValue.Value;
                        stand.PcsPerMinute = Math.Round(stand.PcsPerHour / 60, 2);

                        COSContext.Current.Standards.AddObject(stand);

                        Production.StdLabour = stand.Labour;
                        Production.StdPiecesPerHour = stand.PcsPerHour;
                        Model.ItemDescription = stand.ItemDescription;
                    }

                    this.Visibility = System.Windows.Visibility.Collapsed;
                    this.CloseWithoutEventsAndAnimations();

                }
            }
        }

        void ctrl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Result")
            {
                //MessageBox.Show("Vytvoření normy....výsledek: " + ctrl.Result.ToString());

                //tady vytvoříme normu....
                var labval = SelectedItem.FormItems.FirstOrDefault(a => a.Name == "labour").DoubleValue;

                if (!labval.HasValue || labval.Value <= 0)
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ENG00000019"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
                }
                else
                {
                    Standard stand = COSContext.Current.Standards.CreateObject();

                    stand.WorkCenter = Production.WorkCenter;
                    stand.WorkGroup = Production.WorkGroup;
                    stand.ItemNumber = Production.ItemNumber;
                    stand.ID_Standard = stand.WorkCenter.Value + stand.ItemNumber;
                    //stand.ItemGroup = SelectedItem.ItemGroup;
                    stand.isConfig = true;

                    stand.CreateDate = COSContext.Current.DateTimeServer;
                    stand.ModifyDate = COSContext.Current.DateTimeServer;
                    stand.CreatedBy_UID = COSContext.Current.CurrentUser.ID;
                    stand.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;

                    stand.Labour = (int)SelectedItem.FormItems.FirstOrDefault(a => a.Name == "labour").DoubleValue;
                    stand.SetupTime_mm = (int)SelectedItem.FormItems.FirstOrDefault(a => a.Name == "setuptime").DoubleValue;
                    stand.Weight_Kg = (decimal)SelectedItem.FormItems.FirstOrDefault(a => a.Name == "weight").DoubleValue;
                    stand.ItemDescription = SelectedItem.FormItems.FirstOrDefault(a => a.Name == "itemdescription").StringValue;

                    stand.PcsPerHour = (decimal)ctrl.Result;
                    stand.PcsPerMinute = Math.Round(stand.PcsPerHour / 60, 2);

                    COSContext.Current.Standards.AddObject(stand);

                    Production.StdLabour = stand.Labour;
                    Production.StdPiecesPerHour = stand.PcsPerHour;
                    Model.ItemDescription = stand.ItemDescription;

                    this.Visibility = System.Windows.Visibility.Collapsed;
                    this.CloseWithoutEventsAndAnimations();
                }
            }
        }

        private void cmbGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGroups.SelectedItem != null)
            {
                ConfiguratorGroup grp = cmbGroups.SelectedItem as ConfiguratorGroup;

                if (grp != null)
                {
                    LocalConfigurators = COSContext.Current.Configurators.Where(a => a.ID_ConfigGroup == grp.ID && a.ID_Division == Production.ID_Division && a.ConfiguratorWorkCenters.Where(c => c.ID_WorkCenter == c.ID_WorkCenter).Count() > 0).ToList();
                }
            }
        }

    }
}
