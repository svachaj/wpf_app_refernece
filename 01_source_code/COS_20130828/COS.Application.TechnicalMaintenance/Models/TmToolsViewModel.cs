using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class TmToolsViewModel : ValidationViewModelBase
    {
        public TmToolsViewModel()
            : base()
        {
            LoadTools(COSContext.Current.TmTools.ToList());
        }

        private ObservableCollection<TmTool> _LocalTmTools = new ObservableCollection<TmTool>();
        public ObservableCollection<TmTool> LocalTmTools
        {
            get
            {
                return _LocalTmTools;
            }
        }

        public void LoadTools(IEnumerable<TmTool> tools)
        {
            LocalTmTools.Clear();
            foreach (var itm in tools)
                LocalTmTools.Add(itm);
        }

      

        private TmTool _selectedTool = null;
        public TmTool SelectedItem
        {
            set
            {
                _selectedTool = value;
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedTool;
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
