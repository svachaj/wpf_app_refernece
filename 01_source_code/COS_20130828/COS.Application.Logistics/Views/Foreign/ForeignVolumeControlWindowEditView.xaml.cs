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
using COS.Direction;
using System.Transactions;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class ForeignVolumeControlWindowEditView : BaseUserControl
    {
        public ForeignVolumeControlWindowEditView(ForeignVolumeControlViewModel model)
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = model;
                this.DataContext = Model;
                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
                this.Loaded += new RoutedEventHandler(ForeignVolumeControlView_Loaded);
            }
        }

        void ForeignVolumeControlView_Loaded(object sender, RoutedEventArgs e)
        {

        }



        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public ForeignVolumeControlViewModel Model;
        public RadWindow RaiseWindow { set; get; }


        private void RadButton_Click(object sender, RoutedEventArgs e)
        {

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    scope.Complete();
                    RaiseWindow.DialogResult = true;
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                    scope.Dispose();
                    COSContext.Current.RejectChanges();

                    MessageBox.Show(exc.InnerException != null ? exc.InnerException.Message : exc.Message);

                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });

                    RaiseWindow.DialogResult = false;
                }

            }

            RaiseWindow.Close();

        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            COSContext.Current.RejectChanges();
            RaiseWindow.DialogResult = false;
            RaiseWindow.Close();
        }




    }


}
