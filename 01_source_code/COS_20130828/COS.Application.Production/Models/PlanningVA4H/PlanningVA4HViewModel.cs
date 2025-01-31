using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;

namespace COS.Application.Production.Models
{
    public partial class PlanningVA4HViewModel : ValidationViewModelBase
    {
        public PlanningVA4HViewModel(COSContext datacontext)
            : base()
        {
            dataContext = datacontext;

            ReloadLocalConstructers();

            LocalItems = new ObservableCollection<VA4H>();

            InitLocalItems(null);
        }

        COSContext dataContext;

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

        private DateTime _selectedDate = DateTime.Now.Date;
        public DateTime SelectedDate
        {
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
            get
            {
                return _selectedDate;
            }
        }

        public void InitLocalItems(IEnumerable<VA4H> items)
        {
            LocalItems.Clear();
            if (items != null)
            {
                foreach (var itm in items)
                {
                    LocalItems.Add(itm);
                }
            }
            else
            {
                var data = dataContext.VA4H.Where(a => a.SOCreatedDate == SelectedDate);

                if (SelectedConstructer != null)
                {
                    data = data.Where(a => a.ID_constructer == SelectedConstructer.ID);
                }

                foreach (var itm in data)
                {
                    LocalItems.Add(itm);
                }
            }
        }

        private User _selectedConstructer = null;
        public User SelectedConstructer
        {
            set
            {
                _selectedConstructer = value;
                OnPropertyChanged("SelectedConstructer");
            }
            get
            {
                return _selectedConstructer;
            }
        }

        public ObservableCollection<User> LocalConstructers { set; get; }
        public void ReloadLocalConstructers()
        {
            if (LocalConstructers == null)
                LocalConstructers = new ObservableCollection<User>();
            else
                LocalConstructers.Clear();

            foreach (var itm in dataContext.Users.ToList().Where(a => a.Employee != null && a.Employee.WorkPosition != null && a.Employee.WorkPosition.Code != null && a.Employee.WorkPosition.Code == "CC1"))
            {
                LocalConstructers.Add(itm);
            }
        }

        public ObservableCollection<VA4H> LocalItems { set; get; }

        public VA4H CreateNewItem()
        {
            VA4H item = dataContext.VA4H.CreateObject();

            return item;
        }

        public void DeleteItem(VA4H item)
        {
            LocalItems.Remove(item);
            dataContext.VA4H.DeleteObject(item);
        }

    }
}
