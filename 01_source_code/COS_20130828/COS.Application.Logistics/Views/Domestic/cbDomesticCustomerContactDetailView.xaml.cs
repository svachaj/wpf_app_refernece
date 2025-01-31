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
using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace COS.Application.Logistics.Views.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticDriverDetailView.xaml
    /// </summary>
    public partial class cbDomesticCustomerContactDetailView : BaseUserControl
    {
        public cbDomesticCustomerContactDetailView(DomesticCustomer customer, COSContext datacontext)
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new cbDomesticContactDetailViewModel(customer, datacontext);
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
                    try
                    {
                        var entry = COSContext.Current.ObjectStateManager.GetObjectStateEntry(Model.SelectedItem);

                        for (int i = 0; i < entry.OriginalValues.FieldCount; i++)
                        {
                            if (entry.OriginalValues.GetValue(i) != entry.CurrentValues.GetValue(i))
                                entry.CurrentValues.SetValue(i, entry.OriginalValues.GetValue(i));
                        }
                    }
                    catch { }

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


        public cbDomesticContactDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;
    }
}
