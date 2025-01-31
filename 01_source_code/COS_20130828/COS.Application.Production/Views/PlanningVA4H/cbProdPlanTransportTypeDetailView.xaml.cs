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
using COS.Application.Production.Models.PlanningVA4H;
using COS.Common;

namespace COS.Application.Production.Views.PlanningVA4H
{
    /// <summary>
    /// Interaction logic for cbDomesticCompositionDetailView.xaml
    /// </summary>
    public partial class cbProdPlanTransportTypeDetailView : BaseUserControl
    {
        public cbProdPlanTransportTypeDetailView(COSContext dataContext)
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

                Model = new cbProdPlanTransportTypeDetailViewModel(this.dataContext);
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
            }
        }

        COSContext dataContext;

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


        public cbProdPlanTransportTypeDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;
    }
}
