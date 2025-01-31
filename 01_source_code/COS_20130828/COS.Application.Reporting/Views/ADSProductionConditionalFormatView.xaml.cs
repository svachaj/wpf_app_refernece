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
using System.ComponentModel;
using COS.Application.Shared;
using COS.Application.Reporting.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
using System.Threading;

namespace COS.Application.Reporting.Views
{
    /// <summary>
    /// Interaction logic for KPIReportView.xaml
    /// </summary>
    public partial class ADSProductionConditionalFormatView : BaseUserControl
    {
        public ADSProductionConditionalFormatView(ADSProductionWCShiftModel model)
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = model;
                this.DataContext = Model;
            }
        }

        public ADSProductionConditionalFormatView(ADSProductionWCShiftRangeModel model)
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = model;
                this.DataContext = Model;
            }
        }

        public dynamic Model = null;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Model.SaveFormatting();   
        }
      


    }
}
