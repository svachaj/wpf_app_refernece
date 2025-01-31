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
    public partial class cbDomesticDestinationViewModel : ValidationViewModelBase
    {
        public cbDomesticDestinationViewModel(COSContext datacontext)
            : base()
        {
            dataContext = datacontext;

            LoadDestinations();
        }

        COSContext dataContext;

        public void LoadDestinations() 
        {
            if (LoaclDomesticDestinations == null)
                LoaclDomesticDestinations = new ObservableCollection<DomesticDestination>();

            LoaclDomesticDestinations.Clear();
            dataContext.Refresh(System.Data.Objects.RefreshMode.StoreWins, dataContext.DomesticDestinations);
            foreach (var itm in dataContext.DomesticDestinations)
                LoaclDomesticDestinations.Add(itm);
        }
        public ObservableCollection<DomesticDestination> LoaclDomesticDestinations { set; get; }

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
