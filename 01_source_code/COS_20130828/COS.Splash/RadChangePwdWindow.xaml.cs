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
using System.Windows.Shapes;
using COS.Resources;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Application.Shared;
using System.ComponentModel;

namespace COS.Splash
{
    /// <summary>
    /// Interaction logic for RadChangePwdWindow.xaml
    /// </summary>
    public partial class RadChangePwdWindow
    {
        public RadChangePwdWindow(string userName)
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("spl_ChangePasswordHeader");
            COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);


            Loaded += new RoutedEventHandler(RadChangePwdWindow_Loaded);

            UserName = userName;
        }

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                COS.Common.WPF.Helpers.LoadLocalizeResources(this);
            }
        }

        void RadChangePwdWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tblUserName.Text = UserName;

            COS.Common.WPF.Helpers.LoadLocalizeResources(this);

        }

        public string UserName { set; get; }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            string customErrors = "";

            string ActualPWD = pwxActualPWD.Password;
            string NewPWD = pwxNewPWD.Password;
            string ConfirmPWD = pwxConfirmPWD.Password;

            User SelectedUser = COSContext.Current.Users.FirstOrDefault(a => a.LoginName == UserName);
            if (SelectedUser != null)
            {
                if (!string.IsNullOrEmpty(ActualPWD))
                {
                    if (string.IsNullOrEmpty(NewPWD))
                    {
                        customErrors += ResourceHelper.GetResource<string>("m_Body_SLP00000001");
                        customErrors += Environment.NewLine;
                    }
                    else
                    {
                        if (!Crypto.VerifyHash(ActualPWD, "SHA512", SelectedUser.PwdHash))
                        {
                            customErrors += ResourceHelper.GetResource<string>("m_Body_SLP00000002");
                            customErrors += Environment.NewLine;
                        }
                        else
                        {
                            if (Crypto.VerifyHash(NewPWD, "SHA512", SelectedUser.PwdHash))
                            {
                                customErrors += ResourceHelper.GetResource<string>("m_Body_SLP00000003");
                                customErrors += Environment.NewLine;
                            }
                            else if (NewPWD.Equals(ConfirmPWD))
                            {
                                if (ValidPwdPolicy(NewPWD))
                                {
                                    SelectedUser.PwdHistoryHash = SelectedUser.PwdHash;
                                    SelectedUser.PwdHash = Crypto.ComputeHash(NewPWD, "SHA512", null);
                                }
                                else
                                {
                                    var policy = COSContext.Current.SysPwdPolicies.FirstOrDefault();

                                    customErrors += ResourceHelper.GetResource<string>("m_Body_SLP00000007") + policy.MinimalLenght.ToString() + ResourceHelper.GetResource<string>("m_Body_SLP00000008") + policy.LowerCaseChar.ToString() + ResourceHelper.GetResource<string>("m_Body_SLP00000009") + policy.UpperCaseChar.ToString() + ResourceHelper.GetResource<string>("m_Body_SLP00000010") + policy.CountOfNumber.ToString();
                                    customErrors += Environment.NewLine;
                                }
                            }
                            else
                            {
                                customErrors += ResourceHelper.GetResource<string>("m_Body_SLP00000004");
                                customErrors += Environment.NewLine;
                            }
                        }
                    }
                }
                else
                {
                    customErrors += ResourceHelper.GetResource<string>("m_Body_SLP00000005");
                    customErrors += Environment.NewLine;
                }
            }
            else 
            {
                customErrors += ResourceHelper.GetResource<string>("m_Body_SLP00000006");
                customErrors += Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(customErrors))
            {
                RadWindow.Alert(new DialogParameters() { Content = customErrors, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (Window)COSContext.Current.RadMainWindow });
            }
            else
            {
                SelectedUser.PwdLastChange = COSContext.Current.DateTimeServer;

                SelectedUser.ModifiedBy_UID = SelectedUser.ID;
                SelectedUser.ModifyDate = COSContext.Current.DateTimeServer;
                SelectedUser.PwdForceChange = false;
                SelectedUser.PwdExpireDate = COSContext.Current.DateTimeServer;

               
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                this.Close();
            }


        }

        private bool ValidPwdPolicy(string NewPWD)
        {
            bool result = false;

            var policy = COSContext.Current.SysPwdPolicies.FirstOrDefault();

            if (policy != null)
            {

                int upperChars = 0;
                int lowerChars = 0;
                int numbers = 0;

                foreach (char ch in NewPWD)
                {
                    if (char.IsDigit(ch))
                        numbers++;
                    else if (char.IsLower(ch))
                        lowerChars++;
                    else if (char.IsUpper(ch))
                        upperChars++;
                }

                if (NewPWD.Length < policy.MinimalLenght || upperChars < policy.UpperCaseChar || lowerChars < policy.LowerCaseChar || numbers < policy.CountOfNumber)
                {
                    result = false;
                }
                else
                    result = true;
            }

            return result;
        }
    }
}
