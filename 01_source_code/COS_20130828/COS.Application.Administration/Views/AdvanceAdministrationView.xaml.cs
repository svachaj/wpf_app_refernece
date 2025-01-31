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
using COS.Application.Administration.Models;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Common.WPF;
using COS.Resources;

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class AdvanceAdministrationView : BaseUserControl
    {
        public AdvanceAdministrationView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new Administration.Models.AdvanceAdministrationViewModel();
                this.DataContext = model;

                COSContext.Current.SavingChanges += new EventHandler(Current_SavingChanges);

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(AdministrationView_Loaded);

            }
        }

        void AdministrationView_Loaded(object sender, RoutedEventArgs e)
        {
            if (model.SelectedGroup != null)
            {
                trvUsedEGPS.ItemsSource = null;
                trvUsedEGPS.ItemsSource = AdvanceAdministrationViewModel.GetClassesItems(model.SelectedGroup.ID);
                trvUsedEGPS.ExpandAll();
            }
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedGroup")
            {
                if (model.SelectedGroup != null)
                {
                    trvUsedEGPS.ItemsSource = null;
                    trvUsedEGPS.ItemsSource = AdvanceAdministrationViewModel.GetClassesItems(model.SelectedGroup.ID);
                    trvUsedEGPS.ExpandAll();
                }
            }
        }

        AdvanceAdministrationViewModel model;

        void Current_SavingChanges(object sender, EventArgs e)
        {
            //ltvGroups.ItemsSource = model.Groups;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!model.IsNewItem)
            {
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }
        }

        private void chbAction_checked(object sender, RoutedEventArgs e)
        {
            CheckBox chb = sender as CheckBox;

            SysEGP cls = chb.DataContext as SysEGP;

            if (cls != null)
            {
                foreach (var itm in cls.SysClass.t_sys_classes1)
                {
                    var child = itm.t_sys_egp.FirstOrDefault(a => a.ID_sys_group == cls.ID_sys_group && a.ID_sys_action == cls.ID_sys_action);
                    if (child != null)
                        child.Granted = cls.Granted;
                }
            }

            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
        }

        private void btnCollapseAll_click(object sender, RoutedEventArgs e)
        {
            trvUsedEGPS.CollapseAll();
        }

        private void btnExpandAll_click(object sender, RoutedEventArgs e)
        {
            trvUsedEGPS.ExpandAll();
        }

        private void btnEA_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            AdminGroupClass egp = btn.DataContext as AdminGroupClass;

            selectedAG = egp;

            if (egp != null)
            {
                ActionsAssocView view = new ActionsAssocView(egp.SystemClass, model.SelectedGroup.ID);

                RadWindow DetailWindow = new RadWindow();

                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                DetailWindow.MaxHeight = ((RadWindow)COSContext.Current.RadMainWindow).ActualHeight;
                DetailWindow.ResizeMode = ResizeMode.CanResize;
                DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_DomesticExportDetail");
                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                view.RaiseWindow = DetailWindow;
                DetailWindow.Content = view;

                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());

                DetailWindow.ShowDialog();
            }


        }

        AdminGroupClass selectedAG = null;

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                
                trvUsedEGPS.ItemsSource = null;
                trvUsedEGPS.ItemsSource = AdvanceAdministrationViewModel.GetClassesItems(model.SelectedGroup.ID);
                trvUsedEGPS.ExpandAll();
                //trvUsedEGPS.ExpandItemByPath(path);
            }
        }

        RadWindow DetailWindow;
        ClassesDetailView detailView;

        private void btnCopyClass_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            AdminGroupClass egp = btn.DataContext as AdminGroupClass;

            if (egp != null)
            {
                if (COS.Common.WPF.Helpers.HasRightForOperation("Groups", "Update"))
                {
                    if (DetailWindow == null)
                    {
                        DetailWindow = new RadWindow();

                        DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                        DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                        DetailWindow.Header = "Class";// ResourceHelper.GetResource<string>("adm_Division");// this.Resources["Divisions"].ToString();
                        DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        detailView = new ClassesDetailView();
                        detailView.RaiseWindow = DetailWindow;
                        DetailWindow.Content = detailView;

                        StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                    }


                    detailView.Model.SelectedItem = detailView.Model.CopyClass(egp.SystemClass);
                    detailView.Model.SelectedGroupClass = egp;

                    DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    DetailWindow.ShowDialog();
                }
            }
            
        }

        private void btnNewClass_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            AdminGroupClass egp = btn.DataContext as AdminGroupClass;

            if (egp != null)
            {
                if (COS.Common.WPF.Helpers.HasRightForOperation("Groups", "Insert"))
                {
                    if (DetailWindow == null)
                    {
                        DetailWindow = new RadWindow();

                        DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                        DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                        DetailWindow.Header = "Class";// ResourceHelper.GetResource<string>("adm_Division");// this.Resources["Divisions"].ToString();
                        DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        detailView = new ClassesDetailView();
                        detailView.RaiseWindow = DetailWindow;
                        DetailWindow.Content = detailView;

                        StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                    }


                    detailView.Model.SelectedItem = detailView.Model.NewClass(egp.SystemClass);
                    detailView.Model.SelectedGroupClass = egp.ParentItem;

                    DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    DetailWindow.ShowDialog();
                }
            }
            
        }

        private void btnEditClass_click(object sender, RoutedEventArgs e)
        {
            RadButton btn = sender as RadButton;

            AdminGroupClass egp = btn.DataContext as AdminGroupClass;

            if (egp != null)
            {
                if (COS.Common.WPF.Helpers.HasRightForOperation("Groups", "Update"))
                {
                    if (DetailWindow == null)
                    {
                        DetailWindow = new RadWindow();

                        DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                        DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                        DetailWindow.Header = "Class";// ResourceHelper.GetResource<string>("adm_Division");// this.Resources["Divisions"].ToString();
                        DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        detailView = new ClassesDetailView();
                        detailView.RaiseWindow = DetailWindow;
                        DetailWindow.Content = detailView;

                        StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                    }


                    detailView.Model.SelectedItem = egp.SystemClass;
                    detailView.Model.SelectedGroupClass = egp.ParentItem;

                    DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                    DetailWindow.ShowDialog();
                }
            }
        }





    }


}
