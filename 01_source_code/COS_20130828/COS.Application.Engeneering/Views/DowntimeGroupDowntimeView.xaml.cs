﻿using System;
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
using COS.Application.Shared;
using COS.Application.Engeneering.Models;
using System.ComponentModel;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Common;

namespace COS.Application.Engeneering.Views
{
    /// <summary>
    /// Interaction logic for UsersView.xaml
    /// </summary>
    public partial class DowntimeGroupDowntimeView : BaseUserControl
    {
        public DowntimeGroupDowntimeView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new DowntimeGroupDowntimeViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

            }
        }

        DowntimeGroupDowntimeViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {



            }
        }


        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                lstItemsToAdd.ItemsSource = Model.ItemsToAdd;
                lstSelectedItems.ItemsSource = Model.SelectedItems;

            }
            else if (e.PropertyName == "Delete")
            {
                lstItemsToAdd.ItemsSource = Model.ItemsToAdd;
                lstSelectedItems.ItemsSource = Model.SelectedItems;
            }
            else if (e.PropertyName == "Error")
            {

            }
        }


        private void division_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstSelectedItems.ItemsSource = null;
            lstItemsToAdd.ItemsSource = null;
            lstWorkGroups.ItemsSource = Model.DowntimeGroups;
        }

        private void workgGroup_changed(object sender, SelectionChangedEventArgs e)
        {
            if (Model.SelectedDowntimeGroup != null)
            {
                lstItemsToAdd.ItemsSource = Model.ItemsToAdd;
                lstSelectedItems.ItemsSource = Model.SelectedItems;
            }
        }



        DateTime prevclick = DateTime.Now;
        private void lstItemsToAdd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstItemsToAdd.IsEnabled)
                {
                    if (COS.Common.WPF.Helpers.HasRightForOperation("cbDowntimesGroupDowntime", "Update"))
                    {
                        TimeSpan ts = (DateTime.Now - prevclick);
                        if (ts.Seconds == 0 && ts.Milliseconds < 300)
                        {
                            lstItemsToAdd.IsEnabled = false;
                            if (Model.SelectedDowntime != null)
                            {
                                Model.AddNew();
                            }
                            lstItemsToAdd.IsEnabled = true;
                        }

                        prevclick = DateTime.Now;
                    }
                }
            }
            catch
            {

            }
        }


        private void lstSelectedItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstSelectedItems.IsEnabled)
                {
                    if (COS.Common.WPF.Helpers.HasRightForOperation("cbDowntimesGroupDowntime", "Update"))
                    {
                        TimeSpan ts = (DateTime.Now - prevclick);
                        if (ts.Seconds == 0 && ts.Milliseconds < 300)
                        {
                            lstSelectedItems.IsEnabled = false;
                            if (Model.SelectedItem != null)
                            {
                                Model.Delete();
                            }
                            lstSelectedItems.IsEnabled = true;
                        }

                        prevclick = DateTime.Now;
                    }
                }
            }
            catch
            {

            }
        }


    }
}
