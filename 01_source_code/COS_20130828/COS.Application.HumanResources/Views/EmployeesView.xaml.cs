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
using COS.Application.HumanResources.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Resources;

namespace COS.Application.HumanResources.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class EmployeesView : BaseUserControl
    {
        public EmployeesView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new EmployeesViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);


            }
        }

        EmployeesViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvEmployees.Rebind();
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RebindData")
            {
                COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ViewEmployees);
                grvEmployees.ItemsSource = COSContext.Current.EmployeesList;
                grvEmployees.Rebind();
            }
        }

        private void grvEmployees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    ViewEmployee itemView = grvEmployees.SelectedItem as ViewEmployee;

                    Employee item = COSContext.Current.Employees.FirstOrDefault(a => a.ID == itemView.ID);

                    if (item != null)
                    {
                        if (EmployeeDetailWindow == null)
                        {
                            EmployeeDetailWindow = new RadWindow();

                            EmployeeDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                            EmployeeDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(EmployeeDetailWindow_Closed);
                            EmployeeDetailWindow.Header = ResourceHelper.GetResource<string>("hr_DetailEmpl");
                            EmployeeDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            detailView = new EmployeeDetailView();
                            detailView.RaiseWindow = EmployeeDetailWindow;
                            EmployeeDetailWindow.Content = detailView;

                            StyleManager.SetTheme(EmployeeDetailWindow, new Expression_DarkTheme());
                        }

                        detailView.Model.SelectedItem = item;
                        EmployeeDetailWindow.ShowDialog();
                    }
                }
            }
        }

        void EmployeeDetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ViewEmployees);
                grvEmployees.ItemsSource = COSContext.Current.ViewEmployees.ToList();
                grvEmployees.Rebind();
            }
            else
            {
                COSContext.Current.RejectChanges();
            }
        }

        RadWindow EmployeeDetailWindow = null;
        EmployeeDetailView detailView = null;
    }
}
