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
using Microsoft.Win32;
using COS.Application.Shared;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using COS.Resources;

namespace COS.Common.WPF.HP
{
    /// <summary>
    /// Interaction logic for HourlyProductionsExport.xaml
    /// </summary>
    public partial class HourlyProductionsExport
    {
        public HourlyProductionsExport()
        {
            InitializeComponent();

            worker = new BackgroundWorker();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.IsEnabled = false;

            btnExport.Click += new RoutedEventHandler(btnExport_Click);
            dtpTo.SelectedDate = DateTime.Today;
            dtpFrom.SelectedDate = DateTime.Today;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            ExportData(filename, dtpFrom.SelectedDate, dtpTo.SelectedDate, ((Division)cmbDivisions.SelectedValue).ID);
            busy1.IsBusy = false;
            this.CloseWithoutEventsAndAnimations();
        }

        DispatcherTimer timer = null;
        string filename;

        private void ExportData(string filename, DateTime? from, DateTime? to, int? division)
        {
           
            var prods = COSContext.Current.p_select_productions(from, to, COSContext.Current.Language, division);

            RadGridView rrr = new RadGridView();
            rrr.Columns.Clear();
            rrr.AutoGenerateColumns = true;
            rrr.ItemsSource = prods.ToList();
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {

                rrr.Export(fs, new GridViewExportOptions() { Format = ExportFormat.ExcelML, ShowColumnHeaders = true, Encoding = Encoding.UTF8 });
                fs.Close();
            }

            Process.Start(filename);
            rrr = null;
        }

        BackgroundWorker worker = null;
        void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDivisions.SelectedValue != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".xls";
                sfd.AddExtension = true;
                sfd.Filter = "*.xls|*.xls";

                var selected = sfd.ShowDialog();

                if (selected.HasValue && selected.Value)
                {
                    busy1.IsBusy = true;
                    filename = sfd.FileName;
                    timer.Start();

                }
            }
            else 
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_REP00000012"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }

        }

    }
}
