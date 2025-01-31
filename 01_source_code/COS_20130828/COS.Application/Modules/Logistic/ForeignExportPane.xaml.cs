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
using COS.Application.Logistics.Views;
using COS.Application.Shared;


namespace COS.Application.Modules.Logistic
{
    /// <summary>
    /// Interaction logic for ForeignExportPane.xaml
    /// </summary>
    public partial class ForeignExportPane
    {
        public ForeignExportPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_ForeignExport");
            Loaded += new RoutedEventHandler(ForeignExportPane_Loaded);
            GotFocus += new RoutedEventHandler(ForeignExportPane_GotFocus);
        }

        void ForeignExportPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void ForeignExportPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                //this.Content = new ForeignsView();
                //SourceOfAccident ss;ss.SysLocalize

                this.Content = new COS.Application.WorkSafety.Views.AccidentView(null);

                //var view = new COS.Common.WPF.Controls.GenericCodebookView(COSContext.Current, "SourceOfAccidents");

                //List<COS.Common.WPF.Controls.GridViewColumnDefinition> definitions = new List<COS.Common.WPF.Controls.GridViewColumnDefinition>();

                //definitions.Add(new COS.Common.WPF.Controls.GridViewColumnDefinition() { Name = "SysLocalize", HeaderText = "Popis", IsMandatory = true, IsLocalize = true, GenerateColumn = true });

                //view.TransformData<SourceOfAccident>(COSContext.Current.SourceOfAccidents.ToList(), definitions);

                //this.Content = view;
            }
        }
    }
}
