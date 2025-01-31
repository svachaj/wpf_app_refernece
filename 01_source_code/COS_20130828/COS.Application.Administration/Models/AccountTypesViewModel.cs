using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;

namespace COS.Application.Administration.Models
{
    public partial class AccountTypesViewModel : ValidationViewModelBase
    {
        public AccountTypesViewModel()
            : base()
        {
           
        }


        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNewUser());
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

        public void AddNewUser()
        {
            OnPropertyChanged("AddNew");
        }
    }
}
