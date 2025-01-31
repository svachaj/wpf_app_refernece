using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;

namespace COS.Application.Logistics.Models.Domestic
{
    public partial class cbDomesticCustomerContactsViewModel : ValidationViewModelBase
    {
        public cbDomesticCustomerContactsViewModel(COSContext datacontext)
            : base()
        {
            dataContext = datacontext;
            RefreshData();          
        }

        COSContext dataContext;
      
        public void RefreshData() 
        {
            if (LocalCustomers == null)
                LocalCustomers = new ObservableCollection<DomesticCustomer>();
            else
                LocalCustomers.Clear();

            foreach (var itm in dataContext.DomesticCustomers)
                LocalCustomers.Add(itm);
        }

        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNew());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }

        public void Delete()
        {
            OnPropertyChanged("Delete");
        }

        public void AddNew()
        {
            OnPropertyChanged("AddNew");
        }

        public ObservableCollection<DomesticCustomer> LocalCustomers { set; get; }
    }
}
