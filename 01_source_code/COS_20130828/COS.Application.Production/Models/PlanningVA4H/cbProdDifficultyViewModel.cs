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
    public partial class cbProdDifficultyViewModel : ValidationViewModelBase
    {
        public cbProdDifficultyViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
            LoadLocalDifficulty();
        }

        COSContext dataContext;

        public ObservableCollection<VA4H_difficultyType> LocalDifficulty { set; get; }

        public void LoadLocalDifficulty() 
        {
            if (LocalDifficulty == null)
                LocalDifficulty = new ObservableCollection<VA4H_difficultyType>();
            else
                LocalDifficulty.Clear();

            foreach (var itm in dataContext.VA4H_difficultyType) 
            {
                LocalDifficulty.Add(itm);
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
