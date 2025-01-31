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
    public partial class UsersViewModel : ValidationViewModelBase
    {
        public UsersViewModel()
            : base()
        {
            RefreshData();
        }


        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNewUser());
            }
        }

        public ICommand UpdateToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }

        List<User> _localUsers = new List<User>();
        public List<User> LocalUsers
        {
            get
            {
                return _localUsers;
            }
            set
            {
                _localUsers = value;
                OnPropertyChanged("LocalUsers");
            }
        }

        public void RefreshData()
        {
            List<User> users = new List<User>();
            foreach (var itm in COSContext.Current.Users)
            {
                users.Add(itm);
            }
            LocalUsers.Clear();
            LocalUsers = users;
        }

        public void Save()
        {
            OnPropertyChanged("SaveUser");
            //COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
        }

        public void Cancel()
        {
            OnPropertyChanged("CancelUser");
            //COSContext.Current.RejectChanges();
        }


        public void Delete()
        {
            OnPropertyChanged("DeleteUser");
        }

        public void AddNewUser()
        {
            OnPropertyChanged("BeginIsert");
            //EditingMode = EditMode.New;            
        }
    }
}
