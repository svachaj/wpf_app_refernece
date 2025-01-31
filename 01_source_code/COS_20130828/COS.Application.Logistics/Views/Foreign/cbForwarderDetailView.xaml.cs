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
    /// Interaction logic for cbForwarderDetailView.xaml
    /// </summary>
    public partial class cbForwarderDetailView : BaseUserControl
    {
        public cbForwarderDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new cbForwarderDetailViewModel();
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


        public cbForwarderDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

        private void btnEmailsForwarder_click(object sender, RoutedEventArgs e)
        {
            var view = new COS.Common.WPF.Controls.GenericCodebookView(COSContext.Current, "ForwarderEmails");

            List<COS.Common.WPF.Controls.GridViewColumnDefinition> definitions = new List<COS.Common.WPF.Controls.GridViewColumnDefinition>();

           
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "Forwarder", HeaderText = "", IsMandatory = true, DefaultValue = Model.SelectedItem, GenerateColumn = false });
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "EmailType", HeaderText = "", IsMandatory = true, DefaultValue = COSContext.Current.ForwarderEmailTypes.FirstOrDefault(a=>a.TypeName == "Sender") , GenerateColumn = false });
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "Email", HeaderText = ResourceHelper.GetResource<string>("log_EmailAddress"), IsMandatory = true, GenerateColumn = true });

            view.TransformData<ForwarderEmail>(Model.SelectedItem.t_log_forwarderEmails.Where(a => a.EmailType.TypeName == "Sender").ToList(), definitions);

            emailsWindow = new RadWindow();

            emailsWindow.MinHeight = 350;
            emailsWindow.MinWidth = 400;
            emailsWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
            emailsWindow.ResizeMode = ResizeMode.CanResize;
            emailsWindow.Closed += new EventHandler<WindowClosedEventArgs>(emailsWindow_Closed);
            emailsWindow.Header = ResourceHelper.GetResource<string>("log_CbConAdress");
            emailsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            view.RaiseWindow = emailsWindow;
            emailsWindow.Content = view;

            StyleManager.SetTheme(emailsWindow, new Expression_DarkTheme());


            emailsWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            emailsWindow.ShowDialog();
        }

        void emailsWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            Model.SelectedItem.RefreshForwarderToEmails();
            Model.SelectedItem.RefreshRecieptEmails();
        }

        private void btnRecieptEmails_click(object sender, RoutedEventArgs e)
        {
            var view = new COS.Common.WPF.Controls.GenericCodebookView(COSContext.Current, "ForwarderEmails");

            List<COS.Common.WPF.Controls.GridViewColumnDefinition> definitions = new List<COS.Common.WPF.Controls.GridViewColumnDefinition>();


            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "Forwarder", HeaderText = "", IsMandatory = true, DefaultValue = Model.SelectedItem, GenerateColumn = false });
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "EmailType", HeaderText = "", IsMandatory = true, DefaultValue = COSContext.Current.ForwarderEmailTypes.FirstOrDefault(a => a.TypeName == "Reciept"), GenerateColumn = false });
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "Email", HeaderText = ResourceHelper.GetResource<string>("log_EmailAddress"), IsMandatory = true, GenerateColumn = true });

            view.TransformData<ForwarderEmail>(Model.SelectedItem.t_log_forwarderEmails.Where(a => a.EmailType.TypeName == "Reciept").ToList(), definitions);

            emailsWindow = new RadWindow();

            emailsWindow.MinHeight = 350;
            emailsWindow.MinWidth = 400;
            emailsWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
            emailsWindow.ResizeMode = ResizeMode.CanResize;
            emailsWindow.Closed += new EventHandler<WindowClosedEventArgs>(emailsWindow_Closed);
            emailsWindow.Header = ResourceHelper.GetResource<string>("log_CbReadReceiptEmail");
            emailsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            view.RaiseWindow = emailsWindow;
            emailsWindow.Content = view;

            StyleManager.SetTheme(emailsWindow, new Expression_DarkTheme());


            emailsWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            emailsWindow.ShowDialog();
        }

        public RadWindow emailsWindow { get; set; }
    }
}
