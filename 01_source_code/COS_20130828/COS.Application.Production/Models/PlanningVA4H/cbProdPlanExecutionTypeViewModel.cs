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
    public partial class cbProdPlanExecutionTypeViewModel : ValidationViewModelBase
    {
        public cbProdPlanExecutionTypeViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
            LoadLocalProdPlanExecutionTypes();
        }

        COSContext dataContext;

        public ObservableCollection<ProdPlanExecutionType> LocalProdPlanExecutionTypes { set; get; }

        public void LoadLocalProdPlanExecutionTypes() 
        {
            if (LocalProdPlanExecutionTypes == null)
                LocalProdPlanExecutionTypes = new ObservableCollection<ProdPlanExecutionType>();
            else
                LocalProdPlanExecutionTypes.Clear();

            foreach (var itm in dataContext.ProdPlanExecutionTypes) 
            {
                LocalProdPlanExecutionTypes.Add(itm);
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
