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
    public partial class cbProdPlanTransportTypeViewModel : ValidationViewModelBase
    {
        public cbProdPlanTransportTypeViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
            LoadLocalProdPlanTransportTypes();
        }

        COSContext dataContext;

        public ObservableCollection<ProdPlanTransportType> LocalProdPlanTransportTypes { set; get; }

        public void LoadLocalProdPlanTransportTypes() 
        {
            if (LocalProdPlanTransportTypes == null)
                LocalProdPlanTransportTypes = new ObservableCollection<ProdPlanTransportType>();
            else
                LocalProdPlanTransportTypes.Clear();

            foreach (var itm in dataContext.ProdPlanTransportTypes) 
            {
                LocalProdPlanTransportTypes.Add(itm);
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
