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
    public partial class cbProdManufactureViewModel : ValidationViewModelBase
    {
        public cbProdManufactureViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
            LoadLocalManufactures();
        }

        COSContext dataContext;

        public ObservableCollection<ProdPlanManufacture> LocalManufactures { set; get; }

        public void LoadLocalManufactures() 
        {
            if (LocalManufactures == null)
                LocalManufactures = new ObservableCollection<ProdPlanManufacture>();
            else
                LocalManufactures.Clear();

            foreach (var itm in dataContext.ProdPlanManufactures) 
            {
                LocalManufactures.Add(itm);
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
