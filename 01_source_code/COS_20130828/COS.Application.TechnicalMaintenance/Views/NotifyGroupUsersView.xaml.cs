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
using COS.Application.TechnicalMaintenance.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Resources;

namespace COS.Application.TechnicalMaintenance.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class NotifyGroupUsersView : BaseUserControl
    {
        public NotifyGroupUsersView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new NotifyGroupUsersViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);


            }
        }

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

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

        TechnicalMaintenance.Models.NotifyGroupUsersViewModel Model = null;

        private void lstCheckLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Model.SelectedNotifyGroup != null)
            {
                lstItemsToAdd.ItemsSource = Model.ItemsToAdd;
                lstSelectedItems.ItemsSource = Model.SelectedItems;
            }
        }

        private void lstSelectedItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstSelectedItems.IsEnabled)
                {
                    if (COS.Common.WPF.Helpers.HasRightForOperation("tpmChecklistItems", "Update"))
                    if (1 == 1)
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

        DateTime prevclick = DateTime.Now;
        private void lstItemsToAdd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstItemsToAdd.IsEnabled)
                {
                    if (COS.Common.WPF.Helpers.HasRightForOperation("tpmChecklistItems", "Update"))
                    if (1 == 1)
                    {
                        TimeSpan ts = (DateTime.Now - prevclick);
                        if (ts.Seconds == 0 && ts.Milliseconds < 300)
                        {
                            lstItemsToAdd.IsEnabled = false;
                            if (Model.SelectedUser != null)
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



    }
}
