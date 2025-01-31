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
using COS.Application.Engeneering.Models;
using Telerik.Windows.Controls;
using System.ComponentModel;
using COS.Application.Shared;

namespace COS.Application.Engeneering.Views
{
    /// <summary>
    /// Interaction logic for KpiReportFilterWindow.xaml
    /// </summary>
    public partial class ConfiguratorWCSWindow : RadWindow, INotifyPropertyChanged
    {
        public ConfiguratorWCSWindow(MatrixConfiguratorsViewModel model)
        {
            InitializeComponent();

            Model = model;

            this.DataContext = this;

            Loaded += new RoutedEventHandler(ConfiguratorWCSWindow_Loaded);
        }

        void ConfiguratorWCSWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var curwcs = COSContext.Current.WorkCenters.Where(a => a.ID_Division == Model.SelectedItem.ID_Division).ToList();

            List<WorkCenter> workcenters = new List<WorkCenter>();

            foreach (var cwc in Model.SelectedItem.ConfiguratorWorkCenters)
            {
                var wc = curwcs.FirstOrDefault(a => a.ID == cwc.ID_WorkCenter);

                if (wc != null)
                {
                    workcenters.Add(wc);
                }
            }

            UsedItems = workcenters;

            ReadyItems = curwcs.Except(UsedItems).ToList();
        }

        MatrixConfiguratorsViewModel Model = null;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            var liswcs = Model.SelectedItem.ConfiguratorWorkCenters.ToList();
            foreach (var itm in liswcs)
                COSContext.Current.ConfiguratorsWorkCenters.DeleteObject(itm);

            Model.SelectedItem.ConfiguratorWorkCenters.Clear();

            foreach (var itm in UsedItems)
            {
                var cwc = new ConfiguratorsWorkCenter();
                cwc.WorkCenters = itm;
                cwc.Configurator = Model.SelectedItem;

                Model.SelectedItem.ConfiguratorWorkCenters.Add(cwc);
            }

            this.DialogResult = true;
            this.CloseWithoutEventsAndAnimations();
        }

        private List<WorkCenter> _readyItems = new List<WorkCenter>();
        public List<WorkCenter> ReadyItems
        {
            get
            {
                return _readyItems;
            }
            set
            {
                _readyItems = value;
                OnPropertyChanged("ReadyItems");
            }
        }


        private List<WorkCenter> _usedItems = new List<WorkCenter>();
        public List<WorkCenter> UsedItems
        {
            get
            {
                return _usedItems;
            }
            set
            {
                _usedItems = value;
                OnPropertyChanged("UsedItems");
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

        private void lstItemsToAdd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void lstSelectedItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void RadButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.CloseWithoutEventsAndAnimations();
        }

        private void lstItemsToAdd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lstItemsToAdd.IsEnabled = false;

                WorkCenter wc = lstItemsToAdd.SelectedItem as WorkCenter;

                if (wc != null && !UsedItems.Contains(wc))
                {
                    ReadyItems.Remove(wc);
                    UsedItems.Add(wc);

                    lstItemsToAdd.ItemsSource = null;
                    lstItemsToAdd.ItemsSource = ReadyItems;
                    lstSelectedItems.ItemsSource = null;
                    lstSelectedItems.ItemsSource = UsedItems;
                }
            }
            catch (Exception exc)
            {

            }
            finally
            {
                lstItemsToAdd.IsEnabled = true;
            }
        }

        private void lstSelectedItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lstItemsToAdd.IsEnabled = false;

                WorkCenter wc = lstSelectedItems.SelectedItem as WorkCenter;

                if (wc != null && !ReadyItems.Contains(wc))
                {
                    UsedItems.Remove(wc);
                    ReadyItems.Add(wc);


                    lstItemsToAdd.ItemsSource = null;
                    lstItemsToAdd.ItemsSource = ReadyItems;
                    lstSelectedItems.ItemsSource = null;
                    lstSelectedItems.ItemsSource = UsedItems;
                }
            }
            catch (Exception exc)
            {

            }
            finally
            {
                lstItemsToAdd.IsEnabled = true;
            }
        }
    }
}

