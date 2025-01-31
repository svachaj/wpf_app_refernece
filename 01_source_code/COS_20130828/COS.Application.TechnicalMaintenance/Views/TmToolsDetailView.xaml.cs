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
using COS.Application.TechnicalMaintenance.Models;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace COS.Application.TechnicalMaintenance.Views
{
    /// <summary>
    /// Interaction logic for TmToolsDetailView.xaml
    /// </summary>
    public partial class TmToolsDetailView : BaseUserControl
    {
        public TmToolsDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new TmToolsDetailViewModel();
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

        }


        public TmToolsDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

        private void btnadditemnumber_click(object sender, RoutedEventArgs e)
        {
            AddItemNumber();
        }

        public void AddItemNumber()
        {
            string newNumber = tbxItemNumberNew.Text;

            if (!string.IsNullOrEmpty(newNumber) && Model.SelectedItem.ToolItemNumbers.Where(a => a.ItemNumber == newNumber).Count() == 0)
            {
                ToolItemNumber nnum = new ToolItemNumber();
                nnum.ItemNumber = newNumber;
                Model.SelectedItem.ToolItemNumbers.Add(nnum);

                COSContext.Current.ToolItemNumbers.AddObject(nnum);

                tbxItemNumberNew.Text = "";
            }
        }

        private void tbxItemNumberNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AddItemNumber();
        }

        private void btnremoveitemnumber_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            if (btn != null)
            {
                ToolItemNumber tit = btn.DataContext as ToolItemNumber;

                if (tit != null) 
                {
                    Model.SelectedItem.ToolItemNumbers.Remove(tit);
                    COSContext.Current.ToolItemNumbers.DeleteObject(tit);
                }
            }
        }
    }
}
