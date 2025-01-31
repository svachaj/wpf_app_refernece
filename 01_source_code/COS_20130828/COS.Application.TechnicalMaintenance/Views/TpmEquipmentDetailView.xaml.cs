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
    /// Interaction logic for TpmEquipmentDetailView.xaml
    /// </summary>
    public partial class TpmEquipmentDetailView : BaseUserControl
    {
        public TpmEquipmentDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new TpmEquipmentDetailViewModel();
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
            else if (e.PropertyName == "LocalWorkCenters") 
            {
                cmbWorkCenters.ItemsSource = Model.LocalWorkCenters;                
            }

        }


        public TpmEquipmentDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;
    }
}
