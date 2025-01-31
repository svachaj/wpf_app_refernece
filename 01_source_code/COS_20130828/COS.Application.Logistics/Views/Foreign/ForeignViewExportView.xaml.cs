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
using COS.Application.Shared;
using System.ComponentModel;
using COS.Application.Logistics.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;
using Telerik.Windows.Controls.GridView;
using COS.Logistics;
using COS.Application.Logistics.Views.Foreign;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for TransportOrdesCoView.xaml
    /// </summary>
    public partial class ForeignViewExportView : BaseUserControl
    {
        public ForeignViewExportView(COSContext dataContext)
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (dataContext == null)
                {
                    var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                    string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                    this.dataContext = new COSContext(decryptString);
                }
                else
                    this.dataContext = dataContext;

                model = new ForeignViewExportViewModel(this.dataContext);

                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(ForeignsView_Loaded);
            }
        }

        COSContext dataContext = null;
        public ForeignViewExportViewModel model = null;

        void ForeignsView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataReloaded")
            {
                grvExports.ItemsSource = null;
                grvExports.ItemsSource = model.LocalExports;
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            grvExports.BeginInsert();
        }



        public RadWindow RaiseWindow { get; set; }

        private void btnSend_click(object sender, RoutedEventArgs e)
        {
            RaiseWindow.DialogResult = true;
            RaiseWindow.Close();
        }

        private void btnCancel_click(object sender, RoutedEventArgs e)
        {
            RaiseWindow.DialogResult = false;
            RaiseWindow.Close();
        }


        RadWindow emailsWindow = null;


        void emailsWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            model.RefreshEmails();
        }

        private void RadButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (grvExports.SelectedItem != null)
                grvExports.Items.Remove(grvExports.SelectedItem);
        }

        private void RadButtonEditEmails_Click(object sender, RoutedEventArgs e)
        {
            var view = new COS.Common.WPF.Controls.GenericCodebookView(this.dataContext, "SysEmailAddresses");

            List<COS.Common.WPF.Controls.GridViewColumnDefinition> definitions = new List<COS.Common.WPF.Controls.GridViewColumnDefinition>();

            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "GroupCode", HeaderText = ResourceHelper.GetResource<string>("log_EmGroup"), IsMandatory = true, DefaultValue = "LogForTW1", GenerateColumn = false });
            definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "EmailAddress", HeaderText = ResourceHelper.GetResource<string>("log_EmailAddress"), IsMandatory = true, GenerateColumn = true });

            view.TransformData<SysEmailAddress>(this.dataContext.SysEmailAddresses.Where(a => a.GroupCode == "LogForTW1").ToList(), definitions);

            emailsWindow = new RadWindow();

            emailsWindow.MinHeight = 350;
            emailsWindow.MinWidth = 400;
            emailsWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 80;
            emailsWindow.ResizeMode = ResizeMode.CanResize;
            emailsWindow.Closed += emailsWindow_Closed;
            emailsWindow.Header = ResourceHelper.GetResource<string>("log_ProdTimeWindowEmail");
            emailsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            view.RaiseWindow = emailsWindow;
            emailsWindow.Content = view;

            StyleManager.SetTheme(emailsWindow, new Expression_DarkTheme());


            emailsWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            emailsWindow.ShowDialog();
        }

        private void btnEditTimeWindow_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            if (btn != null)
            {
                ForeignViewExportClass sel = btn.DataContext as ForeignViewExportClass;

                if (sel != null)
                {
                    OpenEditWindow(sel);
                }
            }
        }

        private void OpenEditWindow(ForeignViewExportClass sel)
        {
            TimeWindowEditWindow win = new TimeWindowEditWindow(sel, dataContext);

            win.Owner = (RadWindow)COSContext.Current.RadMainWindow;

            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            win.ShowDialog();

        }
                
    }


}
