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

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for UserDetailView.xaml
    /// </summary>
    public partial class UserDetailView : BaseUserControl
    {
        public UserDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new UserDetailViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

            }
        }

       
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
            else if (e.PropertyName == "SelectedUser")
            {
                //if(Model.SelectedUser!=null)
                //    Model.SelectedUser.PropertyChanged += new PropertyChangedEventHandler(SelectedUser_PropertyChanged);

                pwdConfirmPassword.Password = "";
                //pwdActualPassword.Password = "";
                pwdNewPassword.Password = "";
            }

        }

        void SelectedUser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "AccountType_ID" || e.PropertyName == "AccountType")
            //{
            //    tbxHRID.Background = new SolidColorBrush(Colors.Orange);
            //}
        }

        public UserDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

       
        private void pwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //Model.ActualPWD = pwdActualPassword.Password;
            Model.NewPWD = pwdNewPassword.Password;
            Model.ConfirmPWD = pwdConfirmPassword.Password;

        }

        private void tbxHRID_LostFocus(object sender, RoutedEventArgs e)
        {
            Model.FindHRData();
        }


    }
}
