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
using COS.Application.Engeneering.Models;
using Telerik.Windows.Controls;

namespace COS.Application.Engeneering.Views
{
    /// <summary>
    /// Interaction logic for KpiReportFilterWindow.xaml
    /// </summary>
    public partial class UnexistsStdFilterWindow : RadWindow
    {
        public UnexistsStdFilterWindow(StandardsViewModel model)
        {
            InitializeComponent();

            Model = model;

            this.DataContext = Model;

            Loaded += new RoutedEventHandler(KpiReportFilterWindow_Loaded);
        }

        void KpiReportFilterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Model.IsDaySelected)
                rowDay.Background = Brushes.Green;
            else
                rowDay.Background = Brushes.Transparent;

            if (Model.IsMonthSelected)
                rowMonth.Background = Brushes.Green;
            else
                rowMonth.Background = Brushes.Transparent;

            if (Model.IsWeekSelected)
                rowWeek.Background = Brushes.Green;
            else
                rowWeek.Background = Brushes.Transparent;

            if (Model.IsYearSelected)
                rowYear.Background = Brushes.Green;
            else
                rowYear.Background = Brushes.Transparent;

        }

        StandardsViewModel Model = null;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
            //Model.ShowReportInternal();
        }

        

        private void yearGotFocus(object sender, RoutedEventArgs e)
        {
            Model.IsYearSelected = true;
            Model.IsMonthSelected = false;
            Model.IsWeekSelected = false;
            Model.IsDaySelected = false;

            rowWeek.Background = Brushes.Transparent;
            rowMonth.Background = Brushes.Transparent;
            rowDay.Background = Brushes.Transparent;
            rowYear.Background = Brushes.Green;
        }

        private void monthGotFocus(object sender, RoutedEventArgs e)
        {
            Model.IsMonthSelected = true;
            Model.IsYearSelected = false;          
            Model.IsWeekSelected = false;
            Model.IsDaySelected = false;

            rowWeek.Background = Brushes.Transparent;
            rowMonth.Background = Brushes.Green;
            rowDay.Background = Brushes.Transparent;
            rowYear.Background = Brushes.Transparent;
        }

        private void weekGotFocus(object sender, RoutedEventArgs e)
        {
            Model.IsWeekSelected = true;
            Model.IsYearSelected = false;
            Model.IsMonthSelected = false;
            Model.IsDaySelected = false;

            rowWeek.Background = Brushes.Green;
            rowMonth.Background = Brushes.Transparent;
            rowDay.Background = Brushes.Transparent;
            rowYear.Background = Brushes.Transparent;
        }

        private void dayGotFocus(object sender, RoutedEventArgs e)
        {
            Model.IsDaySelected = true;
            Model.IsYearSelected = false;
            Model.IsMonthSelected = false;
            Model.IsWeekSelected = false;

            rowWeek.Background = Brushes.Transparent;
            rowMonth.Background = Brushes.Transparent;
            rowDay.Background = Brushes.Green;
            rowYear.Background = Brushes.Transparent;
        }

    }
}
