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
using System.Windows.Shapes;

using Telerik.Windows.Controls;
using COS.Application.HumanResources.Views;
using COS.Application.Shared;
using System.ComponentModel;

namespace COS.Application.Modules.HumanResources
{
    /// <summary>
    /// Interaction logic for cbInsuranceCompanyPane.xaml
    /// </summary>
    public partial class cbInsuranceCompanyPane
    {
        public cbInsuranceCompanyPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_CbInsuranceCompany");
            Loaded += new RoutedEventHandler(cbInsuranceCompanyPane_Loaded);
            GotFocus += new RoutedEventHandler(cbInsuranceCompanyPane_GotFocus);
            
        }

        void cbInsuranceCompanyPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbInsuranceCompanyPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                //this.Content = new HrDepartmentView();
            }
        }

       
    }
}
