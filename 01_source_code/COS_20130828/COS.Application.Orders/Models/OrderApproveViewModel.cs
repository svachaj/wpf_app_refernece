using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Transactions;
using System.Collections.ObjectModel;

namespace COS.Application.Orders.Models
{
    public partial class OrderApproveViewModel : ValidationViewModelBase
    {
        public OrderApproveViewModel()
            : base()
        {
            RefreshOrders();

            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OrderInputlViewModel_PropertyChanged);
        }

        void OrderInputlViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedFilterStatus" || e.PropertyName == "SelectedDateFrom" || e.PropertyName == "SelectedDateTo")
            {
                RefreshOrders();
            }
        }

        private Order _selectedItem = null;
        public Order SelectedItem
        {
            set
            {
                _selectedItem = value;

                if (_selectedItem != null)
                    InitDetails(_selectedItem.OrderDetails);
                else
                    SelectedItemDetails.Clear();


                OnPropertyChanged("SelectedItem");
                OnPropertyChanged("IsReadOnly");
                OnPropertyChanged("CanChangeStatuses");
            }
            get
            {
                return _selectedItem;
            }
        }

        public bool CanChangeStatuses
        {
            get
            {
                bool result = false;

                if (SelectedItem != null)
                {
                    if (SelectedItem.ApproveDate == null)
                        result = true;
                }

                return result;
            }
        }


        private OrderStatus _selectedFilterStatus = null;
        public OrderStatus SelectedFilterStatus
        {
            set
            {
                _selectedFilterStatus = value;
                OnPropertyChanged("SelectedFilterStatus");
            }
            get
            {
                return _selectedFilterStatus;
            }
        }

        private DateTime? _selectedDateFrom = DateTime.Today.AddDays(-30);
        public DateTime? SelectedDateFrom
        {
            set
            {
                _selectedDateFrom = value;
                OnPropertyChanged("SelectedDateFrom");
            }
            get
            {
                return _selectedDateFrom;
            }
        }

        private DateTime? _selectedDateTo = DateTime.Today;
        public DateTime? SelectedDateTo
        {
            set
            {
                _selectedDateTo = value;
                OnPropertyChanged("SelectedDateTo");
            }
            get
            {
                return _selectedDateTo;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                bool result = true;

                if (SelectedItem == null)
                    result = true;
                else
                {
                    if (SelectedItem.ID == 0)
                        result = false;
                    else
                        result = true;
                }

                return result;
            }
        }

        private ObservableCollection<OrderDetail> _selectedItemDetails = new ObservableCollection<OrderDetail>();
        public ObservableCollection<OrderDetail> SelectedItemDetails
        {
            get
            {
                return _selectedItemDetails;
            }
        }

        public void InitDetails(IEnumerable<OrderDetail> details)
        {
            SelectedItemDetails.Clear();

            foreach (var itm in details)
                SelectedItemDetails.Add(itm);
        }

        private ObservableCollection<Order> _localOrders = new ObservableCollection<Order>();

        public ObservableCollection<Order> LocalOrders
        {
            get
            {
                return _localOrders;
            }
        }

        public void InitOrders(IEnumerable<Order> orders)
        {
            LocalOrders.Clear();

            foreach (var itm in orders.OrderBy(a => a.ApproveDate).OrderByDescending(a => a.CreateDate))
                LocalOrders.Add(itm);
        }

        public void RefreshOrders()
        {

            var prevItem = SelectedItem;

            var group = COSContext.Current.OrderApprovalUsers.Where(a => a.ID_user == COSContext.Current.CurrentUser.ID).Select(a => a.ID_department);
            var myDepartments = COSContext.Current.OrderApprovalUsers.Where(a => group.Contains(a.ID_department)).Select(a => a.User.ID);

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Orders.Where(a => myDepartments.Contains(a.CreateByID)));

            var orders = COSContext.Current.Orders.Where(a => myDepartments.Contains(a.CreateByID));

            if (SelectedFilterStatus != null)
            {
                InitOrders(orders.Where(a => a.CreateDate >= SelectedDateFrom && a.CreateDate <= SelectedDateTo && a.Status.ID == SelectedFilterStatus.ID));
            }
            else
            {
                InitOrders(orders.Where(a => a.CreateDate >= SelectedDateFrom && a.CreateDate <= SelectedDateTo));
            }

            if (LocalOrders.Count > 0)
                SelectedItem = prevItem;
            else
                SelectedItem = null;
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }

        public void AddNew()
        {
            SelectedItem = new Order();

        }

        public void AddNewDetail()
        {
            SelectedItemDetails.Add(new OrderDetail());
        }

        public void RemoveDetail(OrderDetail detail)
        {
            SelectedItemDetails.Remove(detail);
        }

        public void Save()
        {

            string customErrors = "";
            if (SelectedItem != null)
            {
                customErrors = IsValid();

                if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
                {
                    if (SelectedItem.ID == 0)
                    {

                    }
                    else
                    {
                        if (SelectedItem.Status.Code == "F" || SelectedItem.Status.Code == "D" || SelectedItem.Status.Code == "P")
                        {
                            SelectedItem.FinishedBy = COSContext.Current.CurrentUser;
                            SelectedItem.FinishedDate = DateTime.Today;
                        }
                    }

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

                    OnPropertyChanged("Save");
                    OnPropertyChanged("IsReadOnly");
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });

                }
            }
        }


        public void Cancel()
        {
            SelectedItem = null;
            COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        public string IsValid()
        {
            string err = "";

            if (SelectedItem.Department == null)
            {
                err += ResourceHelper.GetResource<string>("m_OrdDepartment");
                err += Environment.NewLine;

            }

            if (string.IsNullOrEmpty(SelectedItem.Subject))
            {
                err += ResourceHelper.GetResource<string>("m_OrdSubject");
                err += Environment.NewLine;

            }

            if (string.IsNullOrEmpty(SelectedItem.Note) && SelectedItemDetails.Count == 0)
            {
                err += ResourceHelper.GetResource<string>("m_OrdDetail");
                err += Environment.NewLine;

            }

            if (SelectedItem.RequiredDate == null)
            {
                err += ResourceHelper.GetResource<string>("m_OrdDeliveryDate");
                err += Environment.NewLine;

            }

            if (SelectedItem.Priority == null)
            {
                err += ResourceHelper.GetResource<string>("m_OrdPriority");
                err += Environment.NewLine;

            }

            foreach (var itm in SelectedItemDetails)
            {
                if (string.IsNullOrEmpty(itm.ItemName))
                {
                    err += ResourceHelper.GetResource<string>("m_OrdItemName");
                    err += Environment.NewLine;

                }

                if (itm.ItemCount == 0)
                {
                    err += ResourceHelper.GetResource<string>("m_OrdItemCount");
                    err += Environment.NewLine;

                }
            }

            return err;
        }
    }
}
