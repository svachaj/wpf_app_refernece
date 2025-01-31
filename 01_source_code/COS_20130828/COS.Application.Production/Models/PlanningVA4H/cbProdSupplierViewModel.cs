using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;

namespace COS.Application.Production.Models.PlanningVA4H
{
    public partial class cbProdSupplierViewModel : ValidationViewModelBase
    {
        public cbProdSupplierViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
            LoadLocalSuppliers();
        }

        COSContext dataContext;

        public ObservableCollection<ProdPlanSupplier> LocalSuppliers { set; get; }

        public void LoadLocalSuppliers() 
        {
            if (LocalSuppliers == null)
                LocalSuppliers = new ObservableCollection<ProdPlanSupplier>();
            else
                LocalSuppliers.Clear();

            foreach (var itm in dataContext.ProdPlanSuppliers) 
            {
                LocalSuppliers.Add(itm);
            }
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
    }
}
