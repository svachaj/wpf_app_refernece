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
    /// Interaction logic for cbForeignExportOrderedByPane.xaml
    /// </summary>
    public partial class cbForeignExportOrderedByPane
    {
        public cbForeignExportOrderedByPane()
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbForeignExportOrderedByPane");
            Loaded += new RoutedEventHandler(cbForeignExportOrderedByPane_Loaded);
            GotFocus += new RoutedEventHandler(cbForeignExportOrderedByPane_GotFocus);
        }

        void cbForeignExportOrderedByPane_GotFocus(object sender, RoutedEventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.BonusGroups);
        }

        void cbForeignExportOrderedByPane_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsHidden && this.Content == null)
            {
                this.Content = new cbOrderedByView();
            }
        }
    }
}
