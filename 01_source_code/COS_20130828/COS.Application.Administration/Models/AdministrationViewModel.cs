using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using System.Windows;
using COS.Application.Shared;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using COS.Resources;

namespace COS.Application.Administration.Models
{
    public partial class AdministrationViewModel : ValidationViewModelBase
    {
        public AdministrationViewModel()
            : base()
        {
            ReloadGroups();

           
        }


        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.PreInsertNewGroup());
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

        private void Delete()
        {
            if (SelectedGroup != null)
            {
                LocalGroups.Remove(SelectedGroup);
            }
        }

        private void Cancel()
        {
            LocalGroups.Remove(SelectedGroup);
            SelectedGroup = LocalGroups.FirstOrDefault();
            EditingMode = EditMode.View;
        }


        //public List<SysGroup> Groups
        //{
        //    get
        //    {
        //        return COSContext.Current.SysGroups.Union(COSContext.Current.ObjectStateManager
        //            .GetObjectStateEntries(System.Data.EntityState.Added).OfType<SysGroup>()).ToList();
        //    }
        //}



        private SysGroup _selectedGroup = null;
        public SysGroup SelectedGroup
        {
            set
            {
                if (_selectedGroup != value)
                {
                    _selectedGroup = value;
                    OnPropertyChanged("SelectedGroup");
                    OnPropertyChanged("DescriptionValid");
                }
            }
            get
            {
                return _selectedGroup;
            }
        }

        //[Required(ErrorMessage = ResourceHelper.GetResource<string>("m_Body_ADM00000004"))]
        public string DescriptionValid
        {
            set
            {
                if (SelectedGroup.Description != value)
                {
                    SelectedGroup.Description = value;
                    OnPropertyChanged("DescriptionValid");
                }
            }
            get
            {
                if (SelectedGroup != null)
                    return SelectedGroup.Description;
                else
                    return null;
            }
        }

        public bool IsNewItem = false;

        public void Save()
        {
            if (string.IsNullOrEmpty(Error))
            {
                if (IsNewItem)
                {
                    IsNewItem = false;
                    COSContext.Current.SysGroups.AddObject(SelectedGroup);
                }

                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                EditingMode = EditMode.View;
                //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysGroups);
            }
            else
            {
                MessageBox.Show(Error);
            }
        }

        public void PreInsertNewGroup()
        {
            IsNewItem = true;
            SelectedGroup = new SysGroup();
            LocalGroups.Add(SelectedGroup);
            EditingMode = EditMode.New;
        }

        private ObservableCollection<SysGroup> _localGroups = new ObservableCollection<SysGroup>();

        public ObservableCollection<SysGroup> LocalGroups
        {
            set
            {
                if (_localGroups != value)
                {
                    _localGroups = value;
                    _localGroups.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_localGroups_CollectionChanged);
                    OnPropertyChanged("LocalGroups");
                }
            }
            get
            {
                return _localGroups;
            }
        }

        private ObservableCollection<SysEGP> _localReadyEGPs = new ObservableCollection<SysEGP>();
        public ObservableCollection<SysEGP> LocalReadyEGPs
        {
            set
            {
                if (_localReadyEGPs != value)
                {
                    _localReadyEGPs = value;
                    _localReadyEGPs.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_localReadyEGPs_CollectionChanged);
                    OnPropertyChanged("LocalReadyEGPs");
                }
            }
            get
            {
                return _localReadyEGPs;
            }
        }

        private ObservableCollection<SysEGP> _localUsedEGPs = new ObservableCollection<SysEGP>();
        public ObservableCollection<SysEGP> LocalUsedEGPs
        {
            set
            {
                if (_localUsedEGPs != value)
                {
                    _localUsedEGPs = value;
                    _localUsedEGPs.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_localUsedEGPs_CollectionChanged);
                    OnPropertyChanged("LocalUsedEGPs");
                }
            }
            get
            {
                return _localUsedEGPs;
            }
        }

        private ObservableCollection<LocalClassEGP> _localClasses = new ObservableCollection<LocalClassEGP>();
        public ObservableCollection<LocalClassEGP> LocalClasses
        {
            set
            {
                if (_localClasses != value)
                {
                    _localClasses = value;
                    _localClasses.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_localClasses_CollectionChanged);
                    OnPropertyChanged("LocalClasses");
                }
            }
            get
            {
                return _localClasses;
            }
        }

        void _localClasses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }

        void _localReadyEGPs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        void _localUsedEGPs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        public List<SysClass> Classes
        {
            get
            {
                return COSContext.Current.SysClasses.Where(a => a.ID_Parent == null).ToList();
            }
        }


        void _localGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (IsNewItem)
                    IsNewItem = false;
                else
                {
                    COSContext.Current.SysGroups.DeleteObject(SelectedGroup);
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    ReloadGroups();
                }
            }
        }


        public void ReloadGroups()
        {
            LocalGroups = new ObservableCollection<SysGroup>();
            foreach (var itm in COSContext.Current.SysGroups)
            {
                LocalGroups.Add(itm);
            }
            SelectedGroup = LocalGroups.FirstOrDefault();
            //LocalClassEGP lcg = null;
            //foreach (var itm in COSContext.Current.SysClasses.Where(a=>a.ID_Parent == null)) 
            //{
            //    lcg = new LocalClassEGP();
            //    lcg.Class = itm;

            //    for


            //    LocalClasses.Add(lcg);
            //}            

        }

    }


    public class LocalClassEGP : ValidationViewModelBase
    {
        public LocalClassEGP()
            : base()
        {
        }

        private int _idClass;
        public int IDClass 
        {
            set
            {
                if (_idClass != value) 
                {
                    _idClass = value;
                    OnPropertyChanged("IDClass");
                }
            }
            get 
            {
                return _idClass;
            }
        }

        private SysClass _class;
        public SysClass Class
        {
            set
            {
                if (_class != value)
                {
                    _class = value;
                    OnPropertyChanged("Class");
                }
            }
            get
            {
                return _class;
            }
        }

        private List<LocalClassEGP> _childClasses = new List<LocalClassEGP>();
        public List<LocalClassEGP> ChildClasses
        {
            set 
            {
                if (_childClasses != null) 
                {
                    _childClasses = value;
                    OnPropertyChanged("ChildClasses");
                }
            }
        }
    }
}
