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
using System.Collections.ObjectModel;
using System.Transactions;

namespace COS.Application.TechnicalMaintenance.Models
{
    public partial class TmToolHandlingViewModel : ValidationViewModelBase
    {
        public TmToolHandlingViewModel()
            : base()
        {
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.TmToolWCs);
            InitTools(COSContext.Current.TmToolWCs);
        }

        private TmToolWC _selectedItem = null;
        public TmToolWC SelectedItem
        {
            set
            {
                _selectedItem = value;
                if (_selectedItem != null) 
                {
                    _selectedItem.PropertyChanged -= _selectedItem_PropertyChanged;
                    _selectedItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedItem_PropertyChanged);
                    InitToolsHistory(_selectedItem.ToolHistory);
                }
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        void _selectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Restarted") 
            {
                InitToolsHistory(SelectedItem.ToolHistory);
            }
        }
  

        private ObservableCollection<TmToolWC> _localTools = new ObservableCollection<TmToolWC>();
        public ObservableCollection<TmToolWC> LocalTools
        {
            get
            {
                return _localTools;
            }
        }

        public void InitTools(IEnumerable<TmToolWC> tools)
        {
            LocalTools.Clear();
            foreach (var itm in tools)
                LocalTools.Add(itm);
        }

        private ObservableCollection<ToolHistory> _localToolsHistory = new ObservableCollection<ToolHistory>();
        public ObservableCollection<ToolHistory> LocalToolsHistory
        {
            get
            {
                return _localToolsHistory;
            }
        }

        public void InitToolsHistory(IEnumerable<ToolHistory> histories)
        {
            LocalToolsHistory.Clear();
            foreach (var itm in histories)
                LocalToolsHistory.Add(itm);
        }
    }
}
