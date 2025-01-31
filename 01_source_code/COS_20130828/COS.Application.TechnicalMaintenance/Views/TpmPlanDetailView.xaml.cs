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
    /// Interaction logic for UserDetailView.xaml
    /// </summary>
    public partial class TpmPlanDetailView : BaseUserControl
    {
        public TpmPlanDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new TpmPlanDetailViewModel();
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
            else if (e.PropertyName == "Equipment")
            {
                var bind = tbxWG.GetBindingExpression(TextBox.TextProperty);

                bind.UpdateTarget();

                bind = tbxWC.GetBindingExpression(TextBox.TextProperty);

                bind.UpdateTarget();

            }
            else if (e.PropertyName == "Recurrency")
            {
                var bind = chbWeekly.GetBindingExpression(CheckBox.IsCheckedProperty);

                bind.UpdateTarget();

                bind = chbMonthly.GetBindingExpression(CheckBox.IsCheckedProperty);

                bind.UpdateTarget();

                bind = chbDaily.GetBindingExpression(CheckBox.IsCheckedProperty);

                bind.UpdateTarget();

                bind = grdDaily.GetBindingExpression(Grid.HeightProperty);

                bind.UpdateTarget();

                bind = grdWeekly.GetBindingExpression(Grid.HeightProperty);

                bind.UpdateTarget();

                bind = grdMonthly.GetBindingExpression(Grid.HeightProperty);

                bind.UpdateTarget();


                foreach (var ctrl in grdRecurently.ChildrenOfType<FrameworkElement>())
                {
                    if (ctrl is CheckBox)
                    {
                        bind = ctrl.GetBindingExpression(CheckBox.IsCheckedProperty);
                        if (bind != null)
                            bind.UpdateTarget();
                    }

                    if (ctrl is RadNumericUpDown)
                    {
                        bind = ctrl.GetBindingExpression(RadNumericUpDown.ValueProperty);
                        if (bind != null)
                            bind.UpdateTarget();
                    }

                    if (ctrl is RadTimePicker)
                    {
                        bind = ctrl.GetBindingExpression(RadTimePicker.SelectedTimeProperty);
                        if (bind != null)
                            bind.UpdateTarget();
                    }

                    if (ctrl is RadDatePicker)
                    {
                        bind = ctrl.GetBindingExpression(RadDatePicker.SelectedDateProperty);
                        if (bind != null)
                            bind.UpdateTarget();
                    }

                    if (ctrl is Border)
                    {
                        bind = ctrl.GetBindingExpression(Border.BorderBrushProperty);
                        if (bind != null)
                            bind.UpdateTarget();
                    }
                }

            }
        }



        public TpmPlanDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

        private void RadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadComboBox cmb = sender as RadComboBox;

            if (cmb != null)
            {
                TpmCheckList chlist = cmb.SelectedItem as TpmCheckList;

                if (chlist != null && Model.SelectedItem != null)
                {
                    if (!Model.FirstLoad || Model.EditingMode == Common.EditMode.New)
                    {
                        Model.SelectedItem.TpmPlannedLabour = chlist.TpmPlannedlabour;
                        Model.SelectedItem.TpmPlannedTime = chlist.TpmPlannedTime;

                        Model.FirstLoad = false;
                    }
                    else
                    {
                        Model.FirstLoad = false;
                    }
                }
            }
        }



    }
}
