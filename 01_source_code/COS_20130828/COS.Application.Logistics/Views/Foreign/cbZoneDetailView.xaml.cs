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
using COS.Application.Logistics.Models;
using System.ComponentModel;
using Telerik.Windows.Controls;
using COS.Resources;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for cbZoneDetailView.xaml
    /// </summary>
    public partial class cbZoneDetailView : BaseUserControl
    {
        public cbZoneDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new cbZoneDetailViewModel();
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


        public cbZoneDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ForeignCustomerNumberWindow wnd = new ForeignCustomerNumberWindow(Model.SelectedItem);

            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
        }

        private void btnAdvices_Click(object sender, RoutedEventArgs e)
        {
            var view = new COS.Common.WPF.Controls.GenericCodebookView(COSContext.Current, "ForeignExportAdvices");
         
            List<COS.Common.WPF.Controls.GridViewColumnDefinition> definitions = new List<COS.Common.WPF.Controls.GridViewColumnDefinition>();

            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "IsDefault", HeaderText = ResourceHelper.GetResource<string>("log_AdvDefValue"), IsMandatory = true, DefaultValue = false, GenerateColumn = true });
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "Zone", HeaderText = "", IsMandatory = true, DefaultValue = this.Model.SelectedItem, GenerateColumn = false });
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "Value", HeaderText = ResourceHelper.GetResource<string>("log_AdvValue"), IsMandatory = true, GenerateColumn = true });

            view.TransformData<ForeignExportAdvice>(this.Model.SelectedItem.Advices.ToList(), definitions);

            advicesWindow = new RadWindow();

            advicesWindow.MinHeight = 350;
            advicesWindow.MinWidth = 400;
            advicesWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
            advicesWindow.ResizeMode = ResizeMode.CanResize;
            advicesWindow.Header = ResourceHelper.GetResource<string>("log_AdvAddAdvice");
            advicesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            view.RaiseWindow = advicesWindow;
            advicesWindow.Content = view;

            StyleManager.SetTheme(advicesWindow, new Expression_DarkTheme());


            advicesWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            advicesWindow.ShowDialog();
        }

        public RadWindow advicesWindow { get; set; }
    }
}
