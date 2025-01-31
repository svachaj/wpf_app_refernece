using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Transactions;

namespace COS.Application.Administration.Models
{
    public partial class UserDetailViewModel : ValidationViewModelBase
    {
        public UserDetailViewModel()
            : base()
        {
            //SelectedUser = user;
        }

        private User _selectedUser = null;
        public User SelectedUser
        {
            set
            {

                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
                OnPropertyChanged("LoginNameValid");
                OnPropertyChanged("HRIDValid");
                OnPropertyChanged("NameValid");
                OnPropertyChanged("SurnameValid");
                ActualPWD = "";
                NewPWD = "";
                ConfirmPWD = "";

            }
            get
            {
                return _selectedUser;
            }
        }


        public string HRIDValid
        {
            set
            {
                if (SelectedUser.HR_ID != value)
                {
                    //if (!SelectedUser.AccountType.Description.ToLower().Contains("admin"))
                    //{
                    //    if (string.IsNullOrEmpty(value))
                    //        throw new ValidationException("Číslo HR je povinné");
                    //}

                    SelectedUser.HR_ID = value;
                    OnPropertyChanged("HRIDValid");
                }
            }
            get
            {
                if (SelectedUser != null)
                    return SelectedUser.HR_ID;
                else
                    return null;
            }
        }


        //[Required(ErrorMessage = "Login je povinný!")]
        public string LoginNameValid
        {
            set
            {
                if (SelectedUser.LoginName != value)
                {
                    SelectedUser.LoginName = value;
                    OnPropertyChanged("LoginNameValid");
                }
            }
            get
            {
                if (SelectedUser != null)
                    return SelectedUser.LoginName;
                else
                    return null;
            }
        }


        //[Required(ErrorMessage = "Jméno je povinné!")]
        public string NameValid
        {
            set
            {
                if (SelectedUser.Name != value)
                {
                    //if (!SelectedUser.AccountType.Description.ToLower().Contains("admin"))
                    //{
                    //    if (string.IsNullOrEmpty(value))
                    //        throw new ValidationException("Jméno je povinné");
                    //}

                    SelectedUser.Name = value;
                    OnPropertyChanged("NameValid");
                }
            }
            get
            {
                if (SelectedUser != null)
                    return SelectedUser.Name;
                else
                    return null;
            }
        }

        //[SurnameValidate(AdminAccount=, ErrorMessage="hurááá")]
        public string SurnameValid
        {
            set
            {
                if (SelectedUser.Surname != value)
                {
                    //if (!SelectedUser.AccountType.Description.ToLower().Contains("admin"))
                    //{
                    //    if (string.IsNullOrEmpty(value))
                    //        throw new ValidationException("Přijmení je povinné");
                    //}

                    SelectedUser.Surname = value;
                    OnPropertyChanged("SurnameValid");
                }
            }
            get
            {
                if (SelectedUser != null)
                    return SelectedUser.Surname;
                else
                    return null;
            }
        }

        private string _actualPWD = null;
        public string ActualPWD
        {
            set
            {

                _actualPWD = value;
                OnPropertyChanged("ActualPWD");

            }
            get
            {
                return _actualPWD;
            }
        }

        private string _newPWD = null;
        public string NewPWD
        {
            set
            {

                _newPWD = value;
                OnPropertyChanged("NewPWD");

            }
            get
            {
                return _newPWD;
            }
        }

        private string _confirmPWD = null;
        public string ConfirmPWD
        {
            set
            {

                _confirmPWD = value;
                OnPropertyChanged("ConfirmPWD");

            }
            get
            {
                return _confirmPWD;
            }
        }


        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }


        public void Save()
        {

            string customErrors = "";

            if (SelectedUser.AccountType_ID < 1)
            {
                customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000021");
                customErrors += Environment.NewLine;
            }
            else
            {
                if (SelectedUser.AccountType_ID > 1)
                {
                    if (string.IsNullOrEmpty(SelectedUser.HR_ID))
                    {
                        customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000020");
                        customErrors += Environment.NewLine;
                    }

                    if (string.IsNullOrEmpty(SelectedUser.Surname))
                    {
                        customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000019");
                        customErrors += Environment.NewLine;
                    }

                }
            }

            if (string.IsNullOrEmpty(SelectedUser.LoginName))
            {
                customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000018");
                customErrors += Environment.NewLine;
            }

            if (SelectedUser.Group_ID < 1)
            {
                customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000017");
                customErrors += Environment.NewLine;
            }

            if (SelectedUser.ID == 0)
            {
                if (string.IsNullOrEmpty(NewPWD))
                {
                    customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000016");
                    customErrors += Environment.NewLine;
                }
                else if (string.IsNullOrEmpty(ConfirmPWD))
                {
                    customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000015");
                    customErrors += Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(NewPWD) && !string.IsNullOrEmpty(ConfirmPWD))
                {
                    if (NewPWD.Equals(ConfirmPWD))
                    {
                        if (ValidPwdPolicy(NewPWD))
                        {
                            SelectedUser.PwdHash = Crypto.ComputeHash(NewPWD, "SHA512", null);
                            SelectedUser.PwdExpireDate = COSContext.Current.DateTimeServer;
                        }
                        else
                        {
                            var policy = COSContext.Current.SysPwdPolicies.FirstOrDefault();

                            customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000010") + policy.MinimalLenght.ToString() + ResourceHelper.GetResource<string>("m_Body_ADM00000011") + policy.LowerCaseChar.ToString() + ResourceHelper.GetResource<string>("m_Body_ADM00000012") + policy.UpperCaseChar.ToString() + ResourceHelper.GetResource<string>("m_Body_ADM00000013") + policy.CountOfNumber.ToString();
                            customErrors += Environment.NewLine;
                        }
                    }
                    else
                    {
                        customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000009");
                        customErrors += Environment.NewLine;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(NewPWD))
                {
                    if (Crypto.VerifyHash(NewPWD, "SHA512", SelectedUser.PwdHash))
                    {
                        customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000014");
                        customErrors += Environment.NewLine;
                    }
                    else if (NewPWD.Equals(ConfirmPWD))
                    {
                        if (ValidPwdPolicy(NewPWD))
                        {
                            SelectedUser.PwdHistoryHash = SelectedUser.PwdHash;
                            SelectedUser.PwdHash = Crypto.ComputeHash(NewPWD, "SHA512", null);
                            SelectedUser.PwdExpireDate = COSContext.Current.DateTimeServer;
                        }
                        else
                        {
                            var policy = COSContext.Current.SysPwdPolicies.FirstOrDefault();

                            customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000010") + policy.MinimalLenght.ToString() + ResourceHelper.GetResource<string>("m_Body_ADM00000011") + policy.LowerCaseChar.ToString() + ResourceHelper.GetResource<string>("m_Body_ADM00000012") + policy.UpperCaseChar.ToString() + ResourceHelper.GetResource<string>("m_Body_ADM00000013") + policy.CountOfNumber.ToString();
                            customErrors += Environment.NewLine;
                        }
                    }
                    else
                    {
                        customErrors += ResourceHelper.GetResource<string>("m_Body_ADM00000009");
                        customErrors += Environment.NewLine;
                    }
                }
            }




            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {
                if (SelectedUser.ID == 0)
                {
                    
                    COSContext.Current.Users.AddObject(SelectedUser);
                }
                else
                {

                    if (COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedUser).OriginalValues["AccountExpireDays"] != COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedUser).CurrentValues["AccountExpireDays"])
                    {
                        SelectedUser.AccountExpireDate = COSContext.Current.DateTimeServer;
                    }

                    if (COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedUser).OriginalValues["PwdExpireDays"] != COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedUser).CurrentValues["PwdExpireDays"])
                    {
                        SelectedUser.PwdExpireDate = COSContext.Current.DateTimeServer;
                    } 

                    SelectedUser.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;
                    SelectedUser.ModifyDate = COSContext.Current.DateTimeServer;


                }
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    try
                    {
                        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                        scope.Complete();
                    }
                    catch (Exception exc)
                    {
                        Logging.LogException(exc, LogType.ToFileAndEmail);
                        scope.Dispose();
                        COSContext.Current.RejectChanges();

                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    }

                }

                OnPropertyChanged("Save");
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
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

        public void Cancel()
        {
            //COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        public User HelperUser
        {
            get
            {
                User user = new User();

                user.LoginName = "a";
                user.Name = "a";
                user.Surname = "a";
                user.HR_ID = "a";

                return user;
            }
        }


        public void FindHRData()
        {
            if (SelectedUser != null && !string.IsNullOrEmpty(SelectedUser.HR_ID))
            {
                var empls = COSContext.Current.Employees.Where(a => a.HR_ID == SelectedUser.HR_ID);

                if (empls.Count() == 1)
                {
                    var empl = empls.FirstOrDefault();

                    SelectedUser.Name = empl.Name;
                    OnPropertyChanged("NameValid");
                    SelectedUser.Surname = empl.Surname;
                    OnPropertyChanged("SurnameValid");
                    SelectedUser.Email = empl.Email;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class SurnameValidateAttribute : ValidationAttribute
    {
        public bool AdminAccount { set; get; }

        public override bool IsValid(object value)
        {
            bool result = true;

            if (!AdminAccount)
            {
                if (value != null)
                {
                    if (string.IsNullOrEmpty(value.ToString()))
                        result = false;
                }
                else
                    result = false;
            }

            return result;
        }
    }
}
