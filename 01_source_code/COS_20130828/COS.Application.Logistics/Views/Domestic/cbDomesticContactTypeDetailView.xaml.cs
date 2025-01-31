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
using COS.Application.Logistics.Models.Domestic;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace COS.Application.Logistics.Views.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticContactTypeDetailView.xaml
    /// </summary>
    public partial class cbDomesticContactTypeDetailView : BaseUserControl
    {
        public cbDomesticContactTypeDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new cbDomesticContactTypeDetailViewModel();
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


        public cbDomesticContactTypeDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;
    }
}
