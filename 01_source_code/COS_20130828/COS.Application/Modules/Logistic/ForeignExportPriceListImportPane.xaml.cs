﻿using System;
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
    /// Interaction logic for ForeignExportPriceListImportPane.xaml
    /// </summary>
    public partial class ForeignExportPriceListImportPane
    {
        public ForeignExportPriceListImportPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_ForeignExportpriceListImport");
            Loaded += new RoutedEventHandler(ForeignExportPriceListImportPane_Loaded);
            GotFocus += new RoutedEventHandler(ForeignExportPriceListImportPane_GotFocus);
        }

        void ForeignExportPriceListImportPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void ForeignExportPriceListImportPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new ImportPriceListView();
            }
        }
    }
}
