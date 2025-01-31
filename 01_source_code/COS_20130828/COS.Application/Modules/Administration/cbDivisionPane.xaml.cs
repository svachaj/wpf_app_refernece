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
using COS.Application.Administration.Views;
using COS.Application.Shared;
using System.ComponentModel;

namespace COS.Application.Modules.Administration
{
    /// <summary>
    /// Interaction logic for cbDivisionPane.xaml
    /// </summary>
    public partial class cbDivisionPane
    {
        public cbDivisionPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_CbDivision");
            Loaded += new RoutedEventHandler(cbDivisionPane_Loaded);
        
        }

   

        void cbDivisionPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new DivisionsView();
            }
        }

        protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            base.OnIsSelectedChanged(oldValue, newValue);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (newValue && !this.IsHidden)
                {
                    //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Divisions);

                }
            }
        }

    }
}
