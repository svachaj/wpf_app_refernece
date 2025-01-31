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
using COS.Application.WorkSafety.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;
using Telerik.Windows.Controls.GridView;

using System.Reflection;
using System.Windows.Controls.Primitives;

namespace COS.Application.WorkSafety.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class NearMissDetailView : BaseUserControl
    {
        public NearMissDetailView(COSContext datacontext)
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                dataContext = datacontext;
                model = new AccidentDetailViewModel(datacontext, TypeOfTypeAccident.NearMiss);
                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

            }
        }

        COSContext dataContext;

        public override void RefreshData()
        {

        }

        public AccidentDetailViewModel model;
        public RadWindow RaiseWindow;

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
            else if (e.PropertyName == "SelectedItem")
            {
               
            }
            else if (e.PropertyName == "RaiseErrors")
            {
                if (model.SelectedItem != null && !string.IsNullOrEmpty(model.RaiseErrors))
                {
                    RadWindow.Alert(new DialogParameters() { Content = model.RaiseErrors, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
          
        }

    }


}
