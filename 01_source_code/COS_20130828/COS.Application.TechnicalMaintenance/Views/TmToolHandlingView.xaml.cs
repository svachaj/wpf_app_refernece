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
    public partial class TmToolHandlingView : BaseUserControl
    {
        public TmToolHandlingView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new TmToolHandlingViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);


            }
        }

        TechnicalMaintenance.Models.TmToolHandlingViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvTools.Rebind();
                grvHistories.Rebind();
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            if (btn != null)
            {
                Model.SelectedItem = btn.DataContext as TmToolWC;

                if (Model.SelectedItem != null)
                    Model.SelectedItem.RestartCommand.Execute(null);
            }
        }


    }
}
