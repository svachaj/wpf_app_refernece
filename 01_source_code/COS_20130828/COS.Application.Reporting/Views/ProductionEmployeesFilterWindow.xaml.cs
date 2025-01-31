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
using COS.Application.Reporting.Models;
using Telerik.Windows.Controls;
using COS.Application.Shared;


namespace COS.Application.Reporting.Views
{
    /// <summary>
    /// Interaction logic for KpiReportFilterWindow.xaml
    /// </summary>
    public partial class ProductionEmployeesFilterWindow : RadWindow
    {
        public ProductionEmployeesFilterWindow(ProductionEmployeesModel model)
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("rep_Filter");
            Model = model;

            this.DataContext = Model;

            Loaded += new RoutedEventHandler(KpiReportFilterWindow_Loaded);
        }

        void KpiReportFilterWindow_Loaded(object sender, RoutedEventArgs e)
        {

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

        ProductionEmployeesModel Model = null;

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            string err = ValidInput();

            if (string.IsNullOrEmpty(err))
            {
                this.DialogResult = true;
                this.Close();
            }
            else 
            {
                RadWindow.Alert(new DialogParameters() { Content = err, Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }


        private string ValidInput()
        {
            string err = "";


            if (Model.SelectedBonusGroup == null)
            {
                err += COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000011");
                err += Environment.NewLine;
            }

            //if (Model.SelectedShiftType == null)
            //{
            //    err += "Vyberte typ směny";//COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000003");
            //    err += Environment.NewLine;
            //}

            //if (Model.SelectedDate == null)
            //{
            //    err += "Vyberte datum";// COS.Resources.ResourceHelper.GetResource<string>("m_Body_REP00000004");
            //    err += Environment.NewLine;
            //}


            return err;
        }


        private void yearGotFocus(object sender, RoutedEventArgs e)
        {
            Model.IsYearSelected = true;
            Model.IsMonthSelected = false;
            Model.IsWeekSelected = false;
        
            rowWeek.Background = Brushes.Transparent;
            rowMonth.Background = Brushes.Transparent;
            rowYear.Background = Brushes.Green;
        }

        private void monthGotFocus(object sender, RoutedEventArgs e)
        {
            Model.IsMonthSelected = true;
            Model.IsYearSelected = false;          
            Model.IsWeekSelected = false;
          
            rowWeek.Background = Brushes.Transparent;
            rowMonth.Background = Brushes.Green;
            rowYear.Background = Brushes.Transparent;
        }

        private void weekGotFocus(object sender, RoutedEventArgs e)
        {
            Model.IsWeekSelected = true;
            Model.IsYearSelected = false;
            Model.IsMonthSelected = false;
         
            rowWeek.Background = Brushes.Green;
            rowMonth.Background = Brushes.Transparent;
            rowYear.Background = Brushes.Transparent;
        }

    }
}
