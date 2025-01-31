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
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Resources;

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for UsersView.xaml
    /// </summary>
    public partial class UsersView : BaseUserControl
    {
        public UsersView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                model = new UsersViewModel();
                this.DataContext = model;

                model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                Loaded += new RoutedEventHandler(UsersView_Loaded);

            }
        }

        void UsersView_Loaded(object sender, RoutedEventArgs e)
        {
            grvUsers.Rebind();
        }

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvUsers.Rebind();
            }
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BeginIsert")
            {
                User user = new User();
                //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysPwdPolicies);
                SysPwdPolicy policy = COSContext.Current.SysPwdPolicies.FirstOrDefault();

                user.CreateDate = COSContext.Current.DateTimeServer;
                user.CreatedBy_UID = COSContext.Current.CurrentUser.ID;
                user.IsActive = true;
                user.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;
                user.ModifyDate = COSContext.Current.DateTimeServer;
                user.AccountType = COSContext.Current.SysAccountTypes.FirstOrDefault(a => a.ID == 2);
                user.PwdLastChange = DateTime.Now;
                user.PwdExpireDays = policy.GlobalPwdExpiration;
                user.AccountExpireDate = COSContext.Current.DateTimeServer;
                user.AccountExpireDays = 0;

                if (UserDetailWindow == null)
                {
                    UserDetailWindow = new RadWindow();


                    UserDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    UserDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(UserDetailWindow_Closed);
                    UserDetailWindow.Header = ResourceHelper.GetResource<string>("adm_UserDetail");
                    UserDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new UserDetailView();
                    detailView.RaiseWindow = UserDetailWindow;
                    UserDetailWindow.Content = detailView;

                    StyleManager.SetTheme(UserDetailWindow, new Expression_DarkTheme());
                }

                //detailView.Model.SelectedUser = detailView.Model.HelperUser;
                detailView.Model.SelectedUser = user;
                UserDetailWindow.ShowDialog();
            }
            else if (e.PropertyName == "DeleteUser")
            {
                if (MessageBox.Show(COS.Resources.ResourceHelper.GetResource<string>("m_Body_ADM00000022"), COS.Resources.ResourceHelper.GetResource<string>("m_Header1_A"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (grvUsers.SelectedItem != null)
                    {
                        ViewUser vuser = grvUsers.SelectedItem as ViewUser;
                        User user = COSContext.Current.Users.FirstOrDefault(a => a.ID == vuser.ID);

                        if (user != null)
                        {
                            COSContext.Current.Users.DeleteObject(user);
                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            grvUsers.Rebind();
                        }
                    }
                }
            }


        }



        UsersViewModel model = null;

        //private void grvUsers_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        //{

        //    User user = new User();

        //    user.CreateDate = COSContext.Current.DateTimeServer;
        //    COSContext.Current.CurrentUser = COSContext.Current.Users.FirstOrDefault();
        //    user.CreatedBy_UID = COSContext.Current.CurrentUser.ID;
        //    user.IsActive = true;
        //    user.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;
        //    user.ModifyDate = COSContext.Current.DateTimeServer;
        //    user.AccountType = COSContext.Current.SysAccountTypes.FirstOrDefault();
        //    user.PwdLastChange = DateTime.Now;
        //    //user.Group = COSContext.Current.SysGroups.FirstOrDefault();
        //    //user.PwdHash = "wefwwe";

        //    e.NewObject = user;
        //}

        private void grvUsers_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Cancel)
            {
                COSContext.Current.RejectChanges();
            }
            else if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {

                bool isvalid = false;

                User user = e.EditedItem as User;

                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.PwdHash))
                    {
                        //e.Row.canc();
                        e.Handled = false;
                        isvalid = false;

                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_ADM00000023"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    }

                    if (model.EditingMode == Common.EditMode.New && isvalid)
                    {

                        COSContext.Current.Users.AddObject(e.EditedItem as User);

                        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                        grvUsers.Rebind();
                        model.EditingMode = Common.EditMode.View;
                    }
                }
            }



            //e.Handled = true;
        }

        private void grvUsers_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {
                if (model.EditingMode != Common.EditMode.New)
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                }
            }
            else if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Cancel)
            {
                COSContext.Current.RejectChanges();
            }
        }

        private void grvUsers_CellValidating(object sender, Telerik.Windows.Controls.GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.Column.Tag != null && e.Cell.Column.Tag.ToString() == "users_HR_ID_column")
            {

                if (string.IsNullOrEmpty(e.NewValue as String))
                {
                    e.IsValid = false;
                    e.ErrorMessage = COS.Resources.ResourceHelper.GetResource<string>("m_Body_ADM00000006");
                }
                else
                    e.IsValid = true;

            }
            else if (e.Cell.Column.Tag != null && e.Cell.Column.Tag.ToString() == "users_LoginName_column")
            {

                if (string.IsNullOrEmpty(e.NewValue as String))
                {
                    e.IsValid = false;
                    e.ErrorMessage = COS.Resources.ResourceHelper.GetResource<string>("m_Body_ADM00000018");
                }
                else
                    e.IsValid = true;

            }
            else if (e.Cell.Column.Tag != null && e.Cell.Column.Tag.ToString() == "users_group_column")
            {
                RadComboBox combo = e.EditingElement as RadComboBox;

                if (combo.SelectedItem == null)
                {
                    e.IsValid = false;
                    e.ErrorMessage = COS.Resources.ResourceHelper.GetResource<string>("m_Body_ADM00000017");
                }
                else
                    e.IsValid = true;

            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            COSContext.Current.Language = "en-US";
        }


        private void grvUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("Users", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        ViewUser user = grvUsers.SelectedItem as ViewUser;

                        if (user != null)
                        {
                            if (UserDetailWindow == null)
                            {
                                UserDetailWindow = new RadWindow();

                                UserDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                                UserDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(UserDetailWindow_Closed);
                                UserDetailWindow.Header = ResourceHelper.GetResource<string>("adm_UserDetail");
                                UserDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new UserDetailView();
                                detailView.RaiseWindow = UserDetailWindow;
                                UserDetailWindow.Content = detailView;

                                StyleManager.SetTheme(UserDetailWindow, new Expression_DarkTheme());
                            }

                            detailView.Model.SelectedUser = detailView.Model.HelperUser;
                            User usr = COSContext.Current.Users.FirstOrDefault(a => a.ID == user.ID);
                            detailView.Model.SelectedUser = usr;
                            UserDetailWindow.ShowDialog();
                        }
                    }
                }
            }
        }

        void UserDetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ViewUsers);
                grvUsers.Rebind();
            }
            else
            {
                COSContext.Current.RejectChanges();
            }
        }

        private UserDetailView detailView = null;
        public RadWindow UserDetailWindow = null;

        private void grvUsers_DataLoading(object sender, GridViewDataLoadingEventArgs e)
        {

            foreach (var col in grvUsers.Columns)
            {
                if (COSContext.Current.Language == "cs-CZ")
                {
                    if (col.Name.ToLower().Contains("local_") && !col.Name.ToLower().Contains("local_cz"))
                    {
                        col.IsVisible = false;
                    }
                }
                else
                {
                    if (col.Name.ToLower().Contains("local_cz"))
                    {
                        col.IsVisible = false;
                    }
                }
            }
        }


    }
}
