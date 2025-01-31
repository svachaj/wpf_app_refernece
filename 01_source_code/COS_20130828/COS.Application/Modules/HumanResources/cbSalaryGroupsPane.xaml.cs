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
    /// Interaction logic for cbSalaryGroupsPane.xaml
    /// </summary>
    public partial class cbSalaryGroupsPane
    {
        public cbSalaryGroupsPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_CbSalaryGroups");
            Loaded += new RoutedEventHandler(cbSalaryGroupsPane_Loaded);
         
        }

     

        void cbSalaryGroupsPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new SalaryGroupsView();
            }
        }

        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (newValue && !this.IsHidden)
                {
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SalaryGroups);
                }
            }
        }
    }
}
