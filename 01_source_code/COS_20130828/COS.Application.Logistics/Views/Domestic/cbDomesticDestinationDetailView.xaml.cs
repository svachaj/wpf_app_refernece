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
using COS.Common;

namespace COS.Application.Logistics.Views.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticDestinationDetailView.xaml
    /// </summary>
    public partial class cbDomesticDestinationDetailView : BaseUserControl
    {
        public cbDomesticDestinationDetailView(COSContext datacontext)
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (datacontext != null)
                    dataContext = datacontext;
                else 
                {
                    var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                    string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                    dataContext = new COSContext(decryptString);
                }
                Model = new cbDomesticDestinationDetailViewModel(dataContext);
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
                
            }
        }

        private COSContext dataContext = null;

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


        public cbDomesticDestinationDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;
    }
}
