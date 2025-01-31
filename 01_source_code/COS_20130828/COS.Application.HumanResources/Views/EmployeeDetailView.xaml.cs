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
using COS.Application.Shared;
using COS.Application.HumanResources.Models;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace COS.Application.HumanResources.Views
{
    /// <summary>
    /// Interaction logic for UserDetailView.xaml
    /// </summary>
    public partial class EmployeeDetailView : BaseUserControl
    {
        public EmployeeDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new EmployeeDetailViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

            }
        }

       
        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cancel")
            {
                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = false;
                    RaiseWindow.Close();
                }
            }
            else if (e.PropertyName == "Save")
            {
                if (RaiseWindow != null)
                {
                   
                    RaiseWindow.DialogResult = true;
                    RaiseWindow.Close();
                }
            }
          
            //COSContext.Current.ShiftPatterns
            
        }      

        public EmployeeDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Model.SelectedItem.WorkPosition_ID = null;
            cmbWP.SelectedItem = null;            
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            Model.SelectedItem.Shift_ID = null;
            cmbSH.SelectedItem = null;            
        }

        private void RadButton_Click_2(object sender, RoutedEventArgs e)
        {
            Model.SelectedItem.BonusGroup_ID = null;
            cmbBG.SelectedItem = null;            
        }
                     


    }
}
