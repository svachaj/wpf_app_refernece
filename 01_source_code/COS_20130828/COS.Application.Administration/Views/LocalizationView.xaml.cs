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

using Telerik.Windows.Controls;
using COS.Application.Administration.Views;
using COS.Application.Shared;


namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for LocalizationView.xaml
    /// </summary>
    public partial class LocalizationView : BaseUserControl
    {
        public LocalizationView()
        {
            InitializeComponent();
        }

        private void RadGridView_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
        }
    }
}
