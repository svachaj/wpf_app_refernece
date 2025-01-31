using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using COS.Application.TechnicalMaintenance.Views;
using System.Transactions;

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class NotifyGroupUsersViewModel : ValidationViewModelBase
    {
        public NotifyGroupUsersViewModel()
            : base()
        {

        }

        public List<NotificationGroup> NotifyGroups
        {
            get
            {
                return COSContext.Current.NotificationGroups.ToList();
            }
        }


        private NotificationGroup _selectedNotifyGroup = null;
        public NotificationGroup SelectedNotifyGroup
        {
            set
            {
                _selectedNotifyGroup = value;
                OnPropertyChanged("SelectedNotifyGroup");
            }
            get
            {
                return _selectedNotifyGroup;
            }
        }

        private User _selectedUser = null;
        public User SelectedUser
        {
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
            get
            {
                return _selectedUser;
            }
        }

        public List<User> ItemsToAdd
        {
            get
            {
                if (SelectedNotifyGroup != null && SelectedNotifyGroup.NotificationUsers != null)
                    return COSContext.Current.Users.ToList().Except(SelectedNotifyGroup.NotifyGroupUsers.Select(a => a.User)).ToList().OrderBy(a => a.FullName).ToList();
                else
                    return COSContext.Current.Users.ToList();
            }
        }

        public List<NotifyGroupUser> SelectedItems
        {
            get
            {
                return COSContext.Current.NotifyGroupUsers.Where(a => a.ID_notifyGroup == SelectedNotifyGroup.ID).ToList().OrderBy(a => a.User.FullName).ToList();
            }
        }

        private NotifyGroupUser _selectedItem = null;
        public NotifyGroupUser SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
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
            if (SelectedItem != null)
            {
                COSContext.Current.NotifyGroupUsers.DeleteObject(SelectedItem);

                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    OnPropertyChanged("Delete");
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                    OnPropertyChanged("Error");
                }
            }


        }

        public void AddNew()
        {
            NotifyGroupUser clci = new NotifyGroupUser();

            clci.ID_notifyGroup = SelectedNotifyGroup.ID;
            clci.ID_user = SelectedUser.ID;

            COSContext.Current.NotifyGroupUsers.AddObject(clci);

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    try
                    {
                        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                        scope.Complete();
                    }
                    catch (Exception exc)
                    {
                        Logging.LogException(exc, LogType.ToFileAndEmail);
                        scope.Dispose();
                        COSContext.Current.RejectChanges();

                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    }

                }

                OnPropertyChanged("AddNew");
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                OnPropertyChanged("Error");
            }


        }


    }
}
