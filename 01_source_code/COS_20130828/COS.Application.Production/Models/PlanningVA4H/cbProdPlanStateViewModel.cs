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
    public partial class cbProdPlanStateViewModel : ValidationViewModelBase
    {
        public cbProdPlanStateViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
            LoadLocalProdPlanStates();
        }

        COSContext dataContext;

        public ObservableCollection<ProdPlanState> LocalProdPlanStates { set; get; }

        public void LoadLocalProdPlanStates() 
        {
            if (LocalProdPlanStates == null)
                LocalProdPlanStates = new ObservableCollection<ProdPlanState>();
            else
                LocalProdPlanStates.Clear();

            foreach (var itm in dataContext.ProdPlanStates) 
            {
                LocalProdPlanStates.Add(itm);
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
