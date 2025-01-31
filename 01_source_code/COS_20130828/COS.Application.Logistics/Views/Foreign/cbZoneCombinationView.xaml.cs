using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using COS.Application.Shared;
using System.ComponentModel;
using COS.Application.Logistics.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using Microsoft.Win32;
using System.IO;
using COS.Resources;
using System.Diagnostics;
using Telerik.Windows.Controls.GridView;
using System.Collections.ObjectModel;
using System.Transactions;

namespace COS.Application.Logistics.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class cbZoneCombinationView : BaseUserControl, INotifyPropertyChanged
    {
        public cbZoneCombinationView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = this;
                this.Loaded += new RoutedEventHandler(cbZoneCombinationView_Loaded);
                this.PropertyChanged += new PropertyChangedEventHandler(cbZoneCombinationView_PropertyChanged);
            }
        }

        void cbZoneCombinationView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedParent")
            {
                if (SelectedParent != null)
                {
                    mainLoading = true;
                    LoadUsedItems(COSContext.Current.ZoneCombinations.Where(a => a.ID_Zone_Parent == SelectedParent.ID).Select(a => a.Child).OrderBy(a => a.DestinationName).ToList());
                    LoadReadyItems(AllItems.Except(UsedItems).OrderBy(a => a.DestinationName).ToList());
                    lstItemsToAdd.ItemsSource = null;
                    lstItemsToAdd.ItemsSource = ReadyItems;
                    lstSelectedItems.ItemsSource = null;
                    lstSelectedItems.ItemsSource = UsedItems;
                    mainLoading = false;


                }
            }
        }


        void cbZoneCombinationView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllItems(COSContext.Current.ZoneLogistics.OrderBy(a => a.DestinationName).ToList());
            LoadAllParentItems(COSContext.Current.ZoneLogistics.Where(a => a.cNumber.StartsWith("C-")).OrderBy(a => a.DestinationName).ToList());

            SelectedParent = AllParentItems.FirstOrDefault();

            LoadUsedItems(COSContext.Current.ZoneCombinations.Where(a => a.ID_Zone_Parent == SelectedParent.ID).Select(a => a.Child).OrderBy(a => a.DestinationName).ToList());
            LoadReadyItems(AllItems.Except(UsedItems).OrderBy(a => a.DestinationName).ToList());

            lstItemsToAdd.ItemsSource = null;
            lstItemsToAdd.ItemsSource = ReadyItems;
            lstSelectedItems.ItemsSource = null;
            lstSelectedItems.ItemsSource = UsedItems;

            //ReadyItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ReadyItems_CollectionChanged);
            //UsedItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(UsedItems_CollectionChanged);
        }

        bool mainLoading = false;

        void UsedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && !mainLoading) 
            //{
            //    var newItem = COSContext.Current.ZoneCombinations.CreateObject();
            //    newItem.ID_Zone_Parent = SelectedParent.ID;
            //    newItem.ID_Zone_Child = (e.NewItems[0] as ZoneLogistics).ID;

            //    COSContext.Current.ZoneCombinations.AddObject(newItem);

            //    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            //}
        }

        void ReadyItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && !mainLoading)
            //{
            //    var oldItem = COSContext.Current.ZoneCombinations.FirstOrDefault(a => a.ID_Zone_Child == (e.NewItems[0] as ZoneLogistics).ID && a.ID_Zone_Parent == SelectedParent.ID);
            //    COSContext.Current.ZoneCombinations.DeleteObject(oldItem);

            //    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            //}
        }


        private void LoadUsedItems(List<ZoneLogistics> zones)
        {
            UsedItems.Clear();

            foreach (var itm in zones)
                UsedItems.Add(itm);

            //lstSelectedItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void LoadReadyItems(List<ZoneLogistics> zones)
        {
            ReadyItems.Clear();

            foreach (var itm in zones)
                ReadyItems.Add(itm);

            //lstItemsToAdd.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void LoadAllItems(List<ZoneLogistics> zones)
        {
            AllItems.Clear();

            foreach (var itm in zones)
                AllItems.Add(itm);

            //lstAllItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void LoadAllParentItems(List<ZoneLogistics> zones)
        {
            AllParentItems.Clear();

            foreach (var itm in zones)
                AllParentItems.Add(itm);

            //lstAllItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
        }

        private ObservableCollection<ZoneLogistics> _readyItems = new ObservableCollection<ZoneLogistics>();
        public ObservableCollection<ZoneLogistics> ReadyItems
        {
            get
            {
                return _readyItems;
            }
            set
            {
                _readyItems = value;
                OnPropertyChanged("ReadyItems");
            }
        }


        private ObservableCollection<ZoneLogistics> _usedItems = new ObservableCollection<ZoneLogistics>();
        public ObservableCollection<ZoneLogistics> UsedItems
        {
            get
            {
                return _usedItems;
            }
            set
            {
                _usedItems = value;
                OnPropertyChanged("UsedItems");
            }
        }

        private ObservableCollection<ZoneLogistics> _allItems = new ObservableCollection<ZoneLogistics>();
        public ObservableCollection<ZoneLogistics> AllItems
        {
            get
            {
                return _allItems;
            }
            set
            {
                _allItems = value;
                OnPropertyChanged("AllItems");
            }
        }

        private ObservableCollection<ZoneLogistics> _allParentItems = new ObservableCollection<ZoneLogistics>();
        public ObservableCollection<ZoneLogistics> AllParentItems
        {
            get
            {
                return _allParentItems;
            }
            set
            {
                _allParentItems = value;
                OnPropertyChanged("AllParentItems");
            }
        }


        private ZoneLogistics _selectedParent = null;
        public ZoneLogistics SelectedParent
        {
            get
            {
                return _selectedParent;
            }
            set
            {
                _selectedParent = value;
                OnPropertyChanged("SelectedParent");
            }
        }

        private ZoneLogistics _selectedToAdd = null;
        public ZoneLogistics SelectedToAdd
        {
            get
            {
                return _selectedToAdd;
            }
            set
            {
                _selectedToAdd = value;
                OnPropertyChanged("SelectedToAdd");
            }
        }

        private ZoneLogistics _selectedItem = null;
        public ZoneLogistics SelectedItem
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        DateTime prevclick = DateTime.Now;

        private void lstSelectedItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstSelectedItems.IsEnabled)
                {

                    if (COS.Common.WPF.Helpers.HasRightForOperation("cbAddDestination", "Update"))
                    {
                        TimeSpan ts = (DateTime.Now - prevclick);
                        if (ts.Seconds == 0 && ts.Milliseconds < 300)
                        {
                            lstSelectedItems.IsEnabled = false;
                            if (SelectedItem != null)
                            {
                                Delete();
                            }
                            lstSelectedItems.IsEnabled = true;
                        }

                        prevclick = DateTime.Now;
                    }
                }
            }
            catch
            {

            }
        }

        private void Delete()
        {
            if (SelectedItem != null)
            {
                var itemToDelete = COSContext.Current.ZoneCombinations.FirstOrDefault(a => a.ID_Zone_Child == SelectedItem.ID && a.ID_Zone_Parent == SelectedParent.ID);

                COSContext.Current.ZoneCombinations.DeleteObject(itemToDelete);

                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    OnPropertyChanged("Delete");

                    ReadyItems.Add(SelectedItem);
                    UsedItems.Remove(SelectedItem);
                }
                catch (Exception exc)
                {

                    OnPropertyChanged("Error");
                    //Telerik.Windows.Controls.RadWindow.Alert(new Telerik.Windows.Controls.DialogParameters() { Content="Chyba při ukládání", });
                }
            }
        }

        private void lstItemsToAdd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstItemsToAdd.IsEnabled)
                {

                    if (COS.Common.WPF.Helpers.HasRightForOperation("cbRemoveDestination", "Update"))
                    {
                        TimeSpan ts = (DateTime.Now - prevclick);
                        if (ts.Seconds == 0 && ts.Milliseconds < 300)
                        {
                            lstItemsToAdd.IsEnabled = false;
                            if (SelectedToAdd != null)
                            {
                                AddNew();
                            }
                            lstItemsToAdd.IsEnabled = true;
                        }

                        prevclick = DateTime.Now;
                    }
                }
            }
            catch
            {

            }
        }

        private void AddNew()
        {
            var newItem = COSContext.Current.ZoneCombinations.CreateObject();
            newItem.ID_Zone_Parent = SelectedParent.ID;
            newItem.ID_Zone_Child = SelectedToAdd.ID;

            COSContext.Current.ZoneCombinations.AddObject(newItem);

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    try
                    {
                        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                        scope.Complete();

                        UsedItems.Add(SelectedToAdd);
                        ReadyItems.Remove(SelectedToAdd);

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
