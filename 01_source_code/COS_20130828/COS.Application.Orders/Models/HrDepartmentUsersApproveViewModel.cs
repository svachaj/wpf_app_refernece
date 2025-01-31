using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Transactions;

namespace COS.Application.Orders.Models
{
    public partial class HrDepartmentUsersApproveViewModel : ValidationViewModelBase
    {
        public HrDepartmentUsersApproveViewModel()
            : base()
        {

        }

        public List<HrDepartment> HrDepartments
        {
            get
            {

                return COSContext.Current.HrDepartments.ToList();

            }
        }



        private HrDepartment _selectedHrDepartment = null;
        public HrDepartment SelectedHrDepartment
        {
            set
            {
                _selectedHrDepartment = value;
                OnPropertyChanged("SelectedHrDepartment");
            }
            get
            {
                return _selectedHrDepartment;
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
                var ordepusers = COSContext.Current.OrderApprovalUsers.Where(a => a.ID_department == SelectedHrDepartment.ID);
                var wcs = from wc in COSContext.Current.Users
                          join wgwc in ordepusers on wc.ID equals wgwc.ID_user
                          select wc;

                return COSContext.Current.Users.Except(wcs).OrderBy(a => a.Surname).ToList(); //    .Distinct.Where(a => (COSContext.Current.WorkGroupsWorkCenters.Where(w => w.ID_WorkCenter == a.ID)).Count() == 0).ToList();
            }
        }

        public List<OrderApprovalUser> SelectedItems
        {
            get
            {
                return COSContext.Current.OrderApprovalUsers.Where(a => a.ID_department == SelectedHrDepartment.ID).OrderBy(a => a.User.Surname).ToList();
            }
        }

        private OrderApprovalUser _selectedItem = null;
        public OrderApprovalUser SelectedItem
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
                COSContext.Current.OrderApprovalUsers.DeleteObject(SelectedItem);

                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    OnPropertyChanged("Delete");
                }
                catch (Exception exc)
                {

                    OnPropertyChanged("Error");
                    //Telerik.Windows.Controls.RadWindow.Alert(new Telerik.Windows.Controls.DialogParameters() { Content="Chyba při ukládání", });
                }
            }


        }

        public void AddNew()
        {
            OrderApprovalUser wgwc = new OrderApprovalUser();

            wgwc.ID_department = SelectedHrDepartment.ID;
            wgwc.ID_user = SelectedUser.ID;

            COSContext.Current.OrderApprovalUsers.AddObject(wgwc);

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

                OnPropertyChanged("Error");
                //Telerik.Windows.Controls.RadWindow.Alert(new Telerik.Windows.Controls.DialogParameters() { Content="Chyba při ukládání", });
            }


        }
    }
}
