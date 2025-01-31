using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using COS.Resources;
using System.IO;
using System.Windows;
using System.Globalization;
using System.Transactions;
using System.Data.Objects;
using System.ComponentModel;

namespace COS.Application.Logistics.Models
{
    public partial class ForeignViewExportViewModel : ValidationViewModelBase
    {
        public ForeignViewExportViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
        }

        private COSContext dataContext = null;

        public void ReloadData(List<ForeignViewExportClass> exports)
        {
            LocalExports.Clear();
            foreach (var itm in exports)
                LocalExports.Add(itm);

            OnPropertyChanged("DataReloaded");

            RefreshEmails();
        }

        public void RefreshEmails()
        {
            var emails = this.dataContext.SysEmailAddresses.Where(a => a.GroupCode == "LogForTW1");

            EmailAdresses = "";

            foreach (var itm in emails)
                EmailAdresses += itm.EmailAddress + "; ";

            EmailAdresses = EmailAdresses.Trim(' ', ';');
        }

        private string _emailAddresses = "";
        public string EmailAdresses { set { _emailAddresses = value; OnPropertyChanged("EmailAdresses"); } get { return _emailAddresses; } }

        public ObservableCollection<ForeignViewExportClass> _localExports = new ObservableCollection<ForeignViewExportClass>();

        public ObservableCollection<ForeignViewExportClass> LocalExports
        {
            get
            {
                return _localExports;
            }
        }

    }


}
