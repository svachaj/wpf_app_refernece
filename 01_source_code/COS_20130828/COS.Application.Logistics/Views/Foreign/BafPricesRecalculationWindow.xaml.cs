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
using COS.Application.Logistics.Models;

namespace COS.Logistics
{
    /// <summary>
    /// Interaction logic for Logistic
    /// </summary>
    public partial class BafPricesRecalculationWindow : INotifyPropertyChanged
    {
        public BafPricesRecalculationWindow(ForeignViewModel model)
        {
            InitializeComponent();

            worker = new BackgroundWorker();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            btnExport.Click += new RoutedEventHandler(btnExport_Click);

            this.model = model;

            this.DataContext = this;
        }

        public BafPricesRecalculationWindow(DomesticExportViewModel model)
        {
            InitializeComponent();

            worker = new BackgroundWorker();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            btnExport.Click += new RoutedEventHandler(btnExport_Click);

            this.model = model;

            this.DataContext = this;
        }




        dynamic model;

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            busy1.IsBusy = false;
            this.DialogResult = true;
            this.Close();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
           
            model.RecalculateForeignExport(FromDate, ToDate);
        }



        BackgroundWorker worker = null;
        void btnExport_Click(object sender, RoutedEventArgs e)
        {
            busy1.IsBusy = true;
            worker.RunWorkerAsync();
        }

        private DateTime? _fromDate = DateTime.Now.Date;
        public DateTime? FromDate 
        {
            set 
            {
                _fromDate = value;
                OnPropertyChanged("FromDate");
            }
            get 
            {
                return _fromDate;
            }
        }

        private DateTime? _toDate = DateTime.Now.Date;
        public DateTime? ToDate
        {
            set
            {
                _toDate = value;
                OnPropertyChanged("ToDate");
            }
            get
            {
                return _toDate;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
