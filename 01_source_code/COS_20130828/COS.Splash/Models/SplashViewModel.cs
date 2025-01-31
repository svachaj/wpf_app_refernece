using System.Windows;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using COS.Resources;
using COS.Common;
using System.Windows.Input;
using System;
using COS.Application.Shared;
using System.Linq;
using Telerik.Windows.Controls;
using System.Configuration;

namespace COS.Splash.Models
{
    public class SplashViewModel : NotifyBase
    {
        public SplashViewModel()
            : base()
        {
            //_backgroundWorker = new BackgroundWorker();
            //_backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            //_backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
            //_backgroundWorker.WorkerSupportsCancellation = true;

            //_backgroundWorker.RunWorkerAsync();

            AppVersion = COSContext.Current.AppVersion;

            LoginEnabled = true;
        }

        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoginEnabled = true;
        }



        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateLauncher();
        }

        private void UpdateLauncher()
        {
            CheckServerFiles();

            Thread.Sleep(500);

            CheckAndCopyLocal();

            Thread.Sleep(500);

            CheckAndUpdateLocal();
        }

        public void CheckServerFiles()
        {
            // SetUpdateStatus(ResourceHelper.GetResource<string>("ControlingServerFiles"));
            foreach (var file in IOPaths.FilesToUpdate2)
            {
                if (file.SourceFile == null && file.SourceFileAlternate == null)
                {
                    LogErrorAndShutDown(ResourceHelper.GetResource<string>("FilesUnintegrated"));
                    break;
                }
                //CurrentFilesTick++;
            }
        }


        public void CheckAndCopyLocal()
        {

            foreach (var file in IOPaths.FilesToUpdate2)
            {
                //SetUpdateStatus(ResourceHelper.GetResource<string>("ControlingLocalFilesText"));

                try
                {
                    if (file.SourceFile != null)
                    {

                        if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFile.Name))
                        {
                            //SetUpdateStatus(ResourceHelper.GetResource<string>("CopyNewFileText"));
                            File.Copy(file.SourceFile.FullName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFile.Name, true);
                        }
                        //CurrentFilesTick++;
                    }
                    else
                    {
                        if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFileAlternate.Name))
                        {
                            //SetUpdateStatus(ResourceHelper.GetResource<string>("CopyNewFileText"));
                            File.Copy(file.SourceFileAlternate.FullName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFileAlternate.Name, true);
                        }
                        //CurrentFilesTick++;
                    }
                }
                catch (Exception e)
                {
                    Logging.LogException(e, LogType.ToFileAndEmail, null);
                    LogErrorAndShutDown(e.Message);
                    break;
                }
                //CurrentFilesTick++;
            }
        }

        public void CheckAndUpdateLocal()
        {
            foreach (var file in IOPaths.FilesToUpdate2)
            {
                //SetUpdateStatus(ResourceHelper.GetResource<string>("ControlingVersionLocalFilesText"));
                try
                {
                    if (file.SourceFile != null)
                    {
                        if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFile.Name))
                        {
                            if (file.IsFileAssembly)
                            {
                                Assembly asm = Assembly.LoadFile(new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFile.Name).FullName);

                                if (asm.GetName().Version.CompareTo(file.SourceFileAssembly.GetName().Version) > 0)
                                {
                                    //SetUpdateStatus(ResourceHelper.GetResource<string>("UpdatingFileText"));
                                    File.Copy(file.SourceFile.FullName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFile.Name, true);
                                }
                            }
                            else
                            {
                                if (file.SourceFile.LastWriteTime > new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFile.Name).LastWriteTime)
                                {
                                    //SetUpdateStatus(ResourceHelper.GetResource<string>("UpdatingFileText"));
                                    File.Copy(file.SourceFile.FullName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFile.Name, true);
                                }
                            }
                        }
                        //CurrentFilesTick++;
                    }
                    else
                    {
                        if (file.IsFileAlternateAssembly)
                        {
                            Assembly asm = Assembly.LoadFile(new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFileAlternate.Name).FullName);

                            if (asm.GetName().Version.CompareTo(file.SourceFileAlternateAssembly.GetName().Version) > 0)
                            {
                                //SetUpdateStatus(ResourceHelper.GetResource<string>("UpdatingFileText"));
                                File.Copy(file.SourceFileAlternate.FullName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFileAlternate.Name, true);
                            }
                        }
                        else
                        {
                            if (file.SourceFileAlternate.LastWriteTime > new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFileAlternate.Name).LastWriteTime)
                            {
                                //SetUpdateStatus(ResourceHelper.GetResource<string>("UpdatingFileText"));
                                File.Copy(file.SourceFileAlternate.FullName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Launcher\" + file.SourceFileAlternate.Name, true);
                            }
                        }
                        //CurrentFilesTick++;
                    }
                }
                catch (Exception e)
                {
                    Logging.LogException(e, LogType.ToFileAndEmail, null);
                    LogErrorAndShutDown(e.Message);
                    break;
                }
                //CurrentFilesTick++;
            }
        }


        private void LogErrorAndShutDown(string errMessage)
        {

            //ještě logovat do souboru a posílat chyby!!!!!!!!!!!!!!!!!!!
            Logging.LogException(errMessage, LogType.ToFile);

            MessageBox.Show(errMessage, ResourceHelper.GetResource<string>("Error"), MessageBoxButton.OK, MessageBoxImage.Error);

            //chyba = true;

            //if (_backgroundWorker != null)
            //    _backgroundWorker.CancelAsync();
            //else
            //    App.Current.Shutdown();
        }


        BackgroundWorker _backgroundWorker = null;

        #region events

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(param => this.Login());
            }
        }

        public ICommand ChangePwdOpenWindowCommand
        {
            get
            {
                return new RelayCommand(param => this.OpenChangePwdWindow());
            }
        }

        public void OpenChangePwdWindow()
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                RadWindow wnd = new RadChangePwdWindow(UserName);
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wnd.ShowDialog();
            }
        }


        public void Login()
        {

            bool validLogin = false;
            User user = null;
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Users);
                user = COSContext.Current.Users.FirstOrDefault(a => a.LoginName == UserName);
                if (user != null)
                {
                    if (Crypto.VerifyHash(Password, "SHA512", user.PwdHash))
                    {
                        validLogin = true;

                        COSContext.Current.CurrentUser = user;
                        COSContext.Current.GroupID = user.Group_ID;
                    }
                }
            }

            if (!validLogin)
            {
                OnPropertyChanged("LoginFailed");
            }
            else
            {
                if (user.IsActive)
                {
                    if (user.AccountExpireDays > 0 && user.AccountExpireDate.HasValue && (COSContext.Current.DateTimeServer - user.AccountExpireDate.Value).Days >= user.AccountExpireDays)
                    {
                        OnPropertyChanged("AccountExpire");
                    }
                    else
                    {
                        if (!user.PwdForceChange)
                        {
                            if (user.PwdExpireDays > 0 && user.PwdExpireDate.HasValue && (COSContext.Current.DateTimeServer - user.PwdExpireDate.Value).Days >= user.PwdExpireDays)
                            {
                                OnPropertyChanged("PwdExpire");
                            }
                            else
                            {
                                if (RememberMe)
                                {
                                    Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                    oConfig.AppSettings.Settings["RememberMe"].Value = "True";
                                    oConfig.AppSettings.Settings["RememberUserName"].Value = UserName;
                                    oConfig.Save(ConfigurationSaveMode.Full);
                                    ConfigurationManager.RefreshSection("appSettings");
                                }

                                string seckey = Crypto.ComputeHash(Security.SecurityHelper.SecurityKey, "SHA512", null);
                                string loginname = Crypto.EncryptString(user.LoginName, Security.SecurityHelper.SecurityKey);
                                string secGroupID = Crypto.EncryptString(user.Group_ID.ToString(), Security.SecurityHelper.SecurityKey);

                                //Process.Start("COS.Application.exe", seckey + " " + loginname + " " + secGroupID + " " + COSContext.Current.Language + " " + user.Name + " " + user.Surname + " " + user.HR_ID + " " + user.Email);


                                LoginEnabled = false;
                                LoadingApp = true;
                                ((Window)COSContext.Current.RadMainWindow).Cursor = Cursors.Wait;

                                COSContext.Current.OnPropertyChanged("LoginToApp");
                            }
                        }
                        else
                        {
                            OnPropertyChanged("PwdForceChange");
                        }
                    }
                }
                else
                {
                    OnPropertyChanged("UserUnactive");
                }
            }


            // MessageBox.Show("user: " + UserName + Environment.NewLine + "pwd: " + Password);
            //Process.Start("COS.Application.exe", Security.SecurityHelper.SecurityKey);
            //App.Current.Shutdown();
        }


        #endregion

        private bool _loginEnabled = false;
        public bool LoginEnabled
        {
            set
            {
                if (_loginEnabled != value)
                {
                    _loginEnabled = value;
                    OnPropertyChanged("LoginEnabled");
                }
            }
            get
            {
                return _loginEnabled;
            }
        }

        private bool _loadingApp = false;
        public bool LoadingApp
        {
            set
            {
                if (_loadingApp != value)
                {
                    _loadingApp = value;
                    OnPropertyChanged("LoadingApp");
                }
            }
            get
            {
                return _loadingApp;
            }
        }

        #region user info - muže pak být nahrazeno jednou propertou User - až bude model

        private string _userName = "";
        public string UserName
        {
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
            get
            {
                return _userName;
            }
        }

        private string _appVersion = "";
        public string AppVersion
        {
            set
            {
                if (_appVersion != value)
                {
                    _appVersion = value;
                    OnPropertyChanged("AppVersion");
                }
            }
            get
            {
                return _appVersion;
            }
        }

        private string _password = "";
        public string Password
        {
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged("Password");
                }
            }
            get
            {
                return _password;
            }
        }

        private bool _remeberMe = false;
        public bool RememberMe
        {
            set
            {
                if (_remeberMe != value)
                {
                    _remeberMe = value;
                    OnPropertyChanged("RememberMe");
                }
            }
            get
            {
                return _remeberMe;
            }
        }

        #endregion
    }
}
