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
using Telerik.Windows.Controls;
using COS.Application.Shared;
using COS.Resources;
using System.Configuration;

namespace COS.Splash.Views
{
    /// <summary>
    /// Interaction logic for SplashView.xaml
    /// </summary>
    public partial class SplashView : BaseUserControl
    {
        public SplashView()
        {
            InitializeComponent();

            Model = new Models.SplashViewModel();
            this.DataContext = Model;
            Model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);

            COSContext.Current.RadMainWindow = System.Windows.Application.Current.MainWindow;

            Loaded += new RoutedEventHandler(SplashView_Loaded);
        }

        void SplashView_Loaded(object sender, RoutedEventArgs e)
        {
            tbxUserName.Focus();

            string remember = ConfigurationManager.AppSettings["RememberMe"];

            if (!string.IsNullOrEmpty(remember))
            {
                bool remb = false;
                if (bool.TryParse(remember, out remb))
                {
                    if (remb)
                    {
                        string rememberUserName = ConfigurationManager.AppSettings["RememberUserName"];

                        if (!string.IsNullOrEmpty(rememberUserName))
                        {
                            tbxUserName.Text = rememberUserName;
                            pwxPassword.Focus();
                        }
                    }
                }
                Model.RememberMe = remb;
            }

            var versType = ConfigurationManager.AppSettings["VersionType"];

            if (!string.IsNullOrEmpty(versType))
            {
                tbkWelcome.Text += " [ " + versType + " ]";
            }


        }

        void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LoginFailed")
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_SLP00000011"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
                pwxPassword.Password = "";
                pwxPassword.Focus();
            }
            else if (e.PropertyName == "UserUnactive")
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_SLP00000012"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
                pwxPassword.Password = "";
                tbxUserName.Text = "";
                tbxUserName.Focus();

            }
            else if (e.PropertyName == "PwdForceChange")
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_SLP00000013"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow, Closed = new EventHandler<WindowClosedEventArgs>(PwdForceChangeOnConfirmClosed) });
            }
            else if (e.PropertyName == "PwdExpire")
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_SLP00000014"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow, Closed = new EventHandler<WindowClosedEventArgs>(PwdExpireChangeOnConfirmClosed) });
            }
            else if (e.PropertyName == "AccountExpire")
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_SLP00000015"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
                pwxPassword.Password = "";
                tbxUserName.Text = "";
                tbxUserName.Focus();
            }

        }

        private void PwdForceChangeOnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            pwxPassword.Password = "";

            if (e.DialogResult == true)
            {
                Model.OpenChangePwdWindow();
            }
        }

        private void PwdExpireChangeOnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            pwxPassword.Password = "";

            if (e.DialogResult == true)
            {
                Model.OpenChangePwdWindow();
            }
        }



        public Models.SplashViewModel Model = null;

        private void pwxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Model.Password = pwxPassword.Password;
        }

        private void btnEngLanguage_Click(object sender, RoutedEventArgs e)
        {
            COSContext.Current.Language = "en-US";
        }

        private void btnCzeLanguage_Click(object sender, RoutedEventArgs e)
        {
            COSContext.Current.Language = "cs-CZ";
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void pwxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Model.Login();
            }
        }

    }


}
