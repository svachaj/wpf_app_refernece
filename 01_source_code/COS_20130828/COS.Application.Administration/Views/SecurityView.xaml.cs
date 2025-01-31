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
using COS.Application.Administration.Models;
using System.ComponentModel;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Resources;

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for CostCentersView.xaml
    /// </summary>
    public partial class SecurityView : BaseUserControl
    {
        public SecurityView()

            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new SecurityViewModel();
                this.DataContext = Model;

                if(Model.SelectedItem!=null)
                    Model.SelectedItem.PropertyChanged += new PropertyChangedEventHandler(SelectedItem_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

            }

        }

        void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception exc) 
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                MessageBox.Show("Došlo k chybě");
            }
        }

        SecurityViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
              
            }
        }

    }
}
