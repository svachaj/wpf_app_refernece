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
using COS.Common.WPF;
using COS.Application.Shared;
using COS.Application.Administration.Models;
using System.ComponentModel;
using Telerik.Windows.Controls;
using System.Collections.ObjectModel;

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for UserDetailView.xaml
    /// </summary>
    public partial class ActionsAssocView : BaseUserControl, INotifyPropertyChanged
    {
        public ActionsAssocView(SysClass sysClass, int idGroup)
            : base()
        {
            InitializeComponent();

            this.CurrentClass = sysClass;
            this.GroupID = idGroup;

            ActionsToAdd = new ObservableCollection<SysAction>();
            ActionsUsed = new ObservableCollection<SysAction>();

            var avaActions = COSContext.Current.SysActions.Where(a => a.ID_sys_classes == null || a.ID_sys_classes == sysClass.ID).ToList();

            var usedActions = sysClass.t_sys_egp.Where(c => c.ID_sys_group == this.GroupID).Select(a => a.Action).ToList();

            foreach (var itm in usedActions)
            {
                ActionsUsed.Add(itm);
            }

            foreach (var itm in avaActions.Except(usedActions))
            {
                ActionsToAdd.Add(itm);
            }

            this.DataContext = this;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {

            }
        }

        private SysClass CurrentClass { set; get; }
        public int GroupID { set; get; }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cancel")
            {
                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = false;
                    RaiseWindow.Close();
                }
            }
            else if (e.PropertyName == "Save")
            {
                if (RaiseWindow != null)
                {

                    RaiseWindow.DialogResult = true;
                    RaiseWindow.Close();
                }
            }


        }

        void SelectedUser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }


        public RadWindow RaiseWindow = null;

        private void btnSave_click(object sender, RoutedEventArgs e)
        {
            var usedActions = this.CurrentClass.t_sys_egp.Where(c => c.ID_sys_group == this.GroupID).Select(a => a.Action).ToList();

            foreach (var itm in this.ActionsUsed)
            {
                if (!usedActions.Contains(itm))
                {
                    this.CurrentClass.t_sys_egp.Add(new SysEGP() { Action = itm, SysClass = this.CurrentClass, ID_sys_group = this.GroupID, Granted = true });
                }
            }

            foreach (var itm in ActionsToAdd)
            {
                if (usedActions.Contains(itm))
                {
                    var egp = this.CurrentClass.t_sys_egp.Where(c => c.ID_sys_group == this.GroupID && c.Action == itm).FirstOrDefault();
                    COSContext.Current.SysEGPs.DeleteObject(egp);
                    this.CurrentClass.t_sys_egp.Remove(egp);
                }
            }

            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

            RaiseWindow.DialogResult = true;
            RaiseWindow.Close();
        }

        private void btnCancel_click(object sender, RoutedEventArgs e)
        {
            COSContext.Current.RejectChanges();

            RaiseWindow.DialogResult = false;
            RaiseWindow.Close();
        }

        private void toAdd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RadGridView rgv = sender as RadGridView;

            SysAction action = rgv.SelectedItem as SysAction;

            if (action != null)
            {
                ActionsUsed.Add(action);
                ActionsToAdd.Remove(action);
            }
        }

        private void used_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RadGridView rgv = sender as RadGridView;

            SysAction action = rgv.SelectedItem as SysAction;

            if (action != null)
            {
                ActionsToAdd.Add(action);
                ActionsUsed.Remove(action);
            }
        }

        public ObservableCollection<SysAction> ActionsToAdd { set; get; }

        public ObservableCollection<SysAction> ActionsUsed { set; get; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
