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

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for AdministrationView.xaml
    /// </summary>
    public partial class AdministrationView : BaseUserControl
    {
        public AdministrationView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new Administration.Models.AdministrationViewModel();
                this.DataContext = model;

                COSContext.Current.SavingChanges += new EventHandler(Current_SavingChanges);

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                this.Loaded += new RoutedEventHandler(AdministrationView_Loaded);

            }
        }

        void AdministrationView_Loaded(object sender, RoutedEventArgs e)
        {
            trvUsedEGPS.Rebind();
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedGroup")
            {
                trvUsedEGPS.Rebind();
            }
        }

        AdministrationViewModel model;

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

        private void trvUsedEGPS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show((trvUsedEGPS.SelectedItem as SysClass).Name);
        }

        private void trvUsedEGPS_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
           // StackPanel panel = e.Row.ChildrenOfType<StackPanel>().Where(a => a.Name == "stpActions").FirstOrDefault();
            StackPanel panel = e.Row.Cells.Count > 1 ? e.Row.Cells[1].Content as StackPanel : null;
            
            SysClass cls = e.DataElement as SysClass;

            //var ee = COSContext.Current.SysEGPs.Where(a => a.ID_sys_class == cls.ID);
            //List<SysEGP> egps = ee.Count() > 0 != null ? COSContext.Current.SysEGPs.Where(a => a.ID_sys_class == cls.ID).ToList() : new List<SysEGP>(); 


            if (panel != null && cls != null && cls.ID_Parent != null)
            {
                List<SysEGP> egps = COSContext.Current.SysEGPs.Where(a => a.ID_sys_class == cls.ID && a.ID_sys_group == model.SelectedGroup.ID).ToList();


                CheckBox chb = null;
                //panel.Children.Clear();
                foreach (var itm in COSContext.Current.SysActions.Where(a => a.ID_sys_classes == null))
                {
                    if (!string.IsNullOrEmpty(cls.UserControlTag) && (itm.Code.ToLower() == "edit" || itm.Code.ToLower() == "visibility"))
                    {
                        chb = panel.FindName("chb" + itm.Code.ToLower()) as CheckBox;
                        chb.Visibility = System.Windows.Visibility.Visible;
                        chb.Content = itm.t_sys_localization.cs_Czech;
                        chb.Margin = new Thickness(3, 4, 3, 4);

                        chb.Tag = new Tuple<SysAction, SysClass>(itm, cls);

                        var egp = egps.FirstOrDefault(a => a.ID_sys_action == itm.ID);

                        if (egp != null && egp.Granted.HasValue && egp.Granted.Value)
                            chb.IsChecked = true;
                        else
                            chb.IsChecked = false;

                        chb.Checked += new RoutedEventHandler(chb_Checked);
                        chb.Unchecked += new RoutedEventHandler(chb_Unchecked);

                        //panel.Children.Add(chb);
                    }
                    else if (string.IsNullOrEmpty(cls.UserControlTag) && cls.ID_Parent != null && (itm.Code.ToLower() == "view" || itm.Code.ToLower() == "insert" || itm.Code.ToLower() == "update" || itm.Code.ToLower() == "delete"))
                    {
                        chb = panel.FindName("chb" + itm.Code.ToLower()) as CheckBox;
                        chb.Visibility = System.Windows.Visibility.Visible;
                        chb.Content = itm.t_sys_localization.cs_Czech;
                        chb.Margin = new Thickness(3, 4, 3, 4);

                        chb.Tag = new Tuple<SysAction, SysClass>(itm, cls);

                        var egp = egps.FirstOrDefault(a => a.ID_sys_action == itm.ID);

                        if (egp != null && egp.Granted.HasValue && egp.Granted.Value)
                            chb.IsChecked = true;
                        else
                            chb.IsChecked = false;

                        chb.Checked += new RoutedEventHandler(chb_Checked);
                        chb.Unchecked += new RoutedEventHandler(chb_Unchecked);

                        //panel.Children.Add(chb);
                    }

                }

                foreach (var itm in COSContext.Current.SysActions.Where(a => a.ID_sys_classes == cls.ID))
                {

                    chb = panel.FindName("chb" + itm.Code.ToLower()) as CheckBox;
                    chb.Visibility = System.Windows.Visibility.Visible;
                    chb.Content = itm.t_sys_localization.cs_Czech;
                    chb.Margin = new Thickness(3, 4, 3, 4);
                    chb.Tag = new Tuple<SysAction, SysClass>(itm, cls);

                    var egp = egps.FirstOrDefault(a => a.ID_sys_action == itm.ID);

                    if (egp != null && egp.Granted.HasValue && egp.Granted.Value)
                        chb.IsChecked = true;
                    else
                        chb.IsChecked = false;

                    chb.Checked += new RoutedEventHandler(chb_Checked);
                    chb.Unchecked += new RoutedEventHandler(chb_Unchecked);

                    //panel.Children.Add(chb);


                }
            }
            else if (panel != null && cls != null && cls.ID_Parent == null)
            {
                List<SysEGP> egps = COSContext.Current.SysEGPs.Where(a => a.ID_sys_class == cls.ID && a.ID_sys_group == model.SelectedGroup.ID).ToList();

                CheckBox chb = null;
                //panel.Children.Clear();

                var itm = COSContext.Current.SysActions.Where(a => a.ID_sys_classes == null && a.Code == "visibility").FirstOrDefault();

                chb = panel.FindName("chb" + itm.Code.ToLower()) as CheckBox;
                chb.Visibility = System.Windows.Visibility.Visible;
                chb.Content = itm.t_sys_localization.cs_Czech;
                chb.Margin = new Thickness(3, 4, 3, 4);

                chb.Tag = new Tuple<SysAction, SysClass>(itm, cls);

                var egp = egps.FirstOrDefault(a => a.ID_sys_action == itm.ID);

                if (egp != null && egp.Granted.HasValue && egp.Granted.Value)
                    chb.IsChecked = true;
                else
                    chb.IsChecked = false;

                chb.Checked += new RoutedEventHandler(chb_Checked);
                chb.Unchecked += new RoutedEventHandler(chb_Unchecked);

                //panel.Children.Add(chb);
            }


        }

        void chb_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chb = sender as CheckBox;
            var coc = COSContext.Current;

            if (chb != null)
            {
                Tuple<SysAction, SysClass> tup = chb.Tag as Tuple<SysAction, SysClass>;
                SysClass cls = tup.Item2;
                SysAction act = tup.Item1;
                bool rebind = false;
                if (cls != null && model != null)
                {
                    var egp = coc.SysEGPs.Where(a => a.ID_sys_class == cls.ID && a.ID_sys_action == act.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                    var otheregps = coc.SysEGPs.Where(a => a.ID_sys_class == cls.ID && a.ID_sys_action != act.ID && a.Granted.Value && a.ID_sys_group == model.SelectedGroup.ID);

                    if (egp != null)
                    {
                        egp.Granted = false;


                        if (act.Code.ToLower() == "view" && otheregps.Count() == 0)
                        {
                            var childAction = coc.SysActions.FirstOrDefault(a => a.Code.ToLower() == "visibility");

                            foreach (var child in coc.SysClasses.Where(a => a.ID_Parent == cls.ID))
                            {

                                var egpChild = coc.SysEGPs.Where(a => a.ID_sys_class == child.ID && a.ID_sys_action == childAction.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                                if (egpChild == null)
                                {
                                    rebind = true;
                                    egpChild = new SysEGP();
                                    egpChild.ID_sys_action = childAction.ID;
                                    egpChild.ID_sys_class = child.ID;
                                    egpChild.ID_sys_group = model.SelectedGroup.ID;
                                    egpChild.Granted = false;

                                    coc.SysEGPs.AddObject(egpChild);
                                }
                                else if (egpChild.Granted != null && egpChild.Granted.Value)
                                {
                                    rebind = true;
                                    egpChild.Granted = false;
                                }
                            }
                        }
                        else
                        {
                            var viewact = coc.SysActions.FirstOrDefault(a => a.Code.ToLower() == "view");
                            otheregps = coc.SysEGPs.Where(a => a.ID_sys_class == cls.ID && a.ID_sys_action != act.ID && a.Granted.Value && a.ID_sys_group == model.SelectedGroup.ID && a.ID_sys_action != viewact.ID);

                            if ((act.Code.ToLower() == "insert" || act.Code.ToLower() == "update" || act.Code.ToLower() == "delete") && otheregps.Count() == 0)
                            {
                                var childAction = coc.SysActions.FirstOrDefault(a => a.Code.ToLower() == "visibility");

                                foreach (var child in coc.SysClasses.Where(a => a.ID_Parent == cls.ID))
                                {

                                    var egpChild = coc.SysEGPs.Where(a => a.ID_sys_class == child.ID && a.ID_sys_action == childAction.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();
                                    var egpView = coc.SysEGPs.Where(a => a.ID_sys_class == cls.ID && a.ID_sys_action == viewact.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                                    if (egpChild == null && (egpView == null || egpView.Granted == null || !egpView.Granted.Value))
                                    {
                                        rebind = true;
                                        egpChild = new SysEGP();
                                        egpChild.ID_sys_action = childAction.ID;
                                        egpChild.ID_sys_class = child.ID;
                                        egpChild.ID_sys_group = model.SelectedGroup.ID;
                                        egpChild.Granted = false;

                                        coc.SysEGPs.AddObject(egpChild);
                                    }
                                    else if (egpChild.Granted != null && egpChild.Granted.Value && (egpView == null || egpView.Granted == null || !egpView.Granted.Value))
                                    {
                                        rebind = true;
                                        egpChild.Granted = false;
                                    }
                                }

                                childAction = coc.SysActions.FirstOrDefault(a => a.Code.ToLower() == "edit");

                                foreach (var child in coc.SysClasses.Where(a => a.ID_Parent == cls.ID))
                                {

                                    var egpChild = coc.SysEGPs.Where(a => a.ID_sys_class == child.ID && a.ID_sys_action == childAction.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                                    if (egpChild == null)
                                    {
                                        rebind = true;
                                        egpChild = new SysEGP();
                                        egpChild.ID_sys_action = childAction.ID;
                                        egpChild.ID_sys_class = child.ID;
                                        egpChild.ID_sys_group = model.SelectedGroup.ID;
                                        egpChild.Granted = false;

                                        coc.SysEGPs.AddObject(egpChild);
                                    }
                                    else if (egpChild.Granted != null && egpChild.Granted.Value)
                                    {
                                        rebind = true;
                                        egpChild.Granted = false;
                                    }
                                }
                            }
                        }
                    }

                    coc.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    if (rebind)
                        trvUsedEGPS.Rebind();
                }
            }
        }

        void chb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chb = sender as CheckBox;
            var coc = COSContext.Current;

            if (chb != null)
            {
                Tuple<SysAction, SysClass> tup = chb.Tag as Tuple<SysAction, SysClass>;
                SysClass cls = tup.Item2;
                SysAction act = tup.Item1;
                bool rebind = false;
                if (cls != null && model != null)
                {
                    var egp = coc.SysEGPs.Where(a => a.ID_sys_class == cls.ID && a.ID_sys_action == act.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                    if (egp == null)
                    {
                        egp = new SysEGP();
                        egp.ID_sys_action = act.ID;
                        egp.ID_sys_class = cls.ID;
                        egp.ID_sys_group = model.SelectedGroup.ID;
                        egp.Granted = true;

                        coc.SysEGPs.AddObject(egp);
                    }
                    else
                    {
                        egp.Granted = true;
                    }


                    if (act.Code.ToLower() == "view")
                    {
                        var childAction = coc.SysActions.FirstOrDefault(a => a.Code.ToLower() == "visibility");

                        foreach (var child in coc.SysClasses.Where(a => a.ID_Parent == cls.ID))
                        {

                            var egpChild = coc.SysEGPs.Where(a => a.ID_sys_class == child.ID && a.ID_sys_action == childAction.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                            if (egpChild == null)
                            {
                                rebind = true;
                                egpChild = new SysEGP();
                                egpChild.ID_sys_action = childAction.ID;
                                egpChild.ID_sys_class = child.ID;
                                egpChild.ID_sys_group = model.SelectedGroup.ID;
                                egpChild.Granted = true;

                                coc.SysEGPs.AddObject(egpChild);
                            }
                            else if (egpChild.Granted == null || !egpChild.Granted.Value)
                            {
                                rebind = true;
                                egpChild.Granted = true;
                            }
                        }
                    }
                    else if (act.Code.ToLower() == "insert" || act.Code.ToLower() == "update" || act.Code.ToLower() == "delete")
                    {
                        var childAction = coc.SysActions.FirstOrDefault(a => a.Code.ToLower() == "visibility");

                        foreach (var child in coc.SysClasses.Where(a => a.ID_Parent == cls.ID))
                        {

                            var egpChild = coc.SysEGPs.Where(a => a.ID_sys_class == child.ID && a.ID_sys_action == childAction.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                            if (egpChild == null)
                            {
                                rebind = true;
                                egpChild = new SysEGP();
                                egpChild.ID_sys_action = childAction.ID;
                                egpChild.ID_sys_class = child.ID;
                                egpChild.ID_sys_group = model.SelectedGroup.ID;
                                egpChild.Granted = true;

                                coc.SysEGPs.AddObject(egpChild);
                            }
                            else if (egpChild.Granted == null || !egpChild.Granted.Value)
                            {
                                rebind = true;
                                egpChild.Granted = true;
                            }
                        }

                        childAction = coc.SysActions.FirstOrDefault(a => a.Code.ToLower() == "edit");

                        foreach (var child in coc.SysClasses.Where(a => a.ID_Parent == cls.ID))
                        {

                            var egpChild = coc.SysEGPs.Where(a => a.ID_sys_class == child.ID && a.ID_sys_action == childAction.ID && a.ID_sys_group == model.SelectedGroup.ID).FirstOrDefault();

                            if (egpChild == null)
                            {
                                rebind = true;
                                egpChild = new SysEGP();
                                egpChild.ID_sys_action = childAction.ID;
                                egpChild.ID_sys_class = child.ID;
                                egpChild.ID_sys_group = model.SelectedGroup.ID;
                                egpChild.Granted = true;

                                coc.SysEGPs.AddObject(egpChild);
                            }
                            else if (egpChild.Granted == null || !egpChild.Granted.Value)
                            {
                                rebind = true;
                                egpChild.Granted = true;
                            }
                        }
                    }

                    coc.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    if (rebind)
                        trvUsedEGPS.Rebind();
                }
            }
        }

    }


}
