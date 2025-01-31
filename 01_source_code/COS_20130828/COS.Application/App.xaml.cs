using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using COS.Application.Shared;
using COS.Common;
using COS.Resources;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace COS.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string tempFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\COS\tempdata.tmpd";

            StreamReader file = null;
            try
            {
                if (File.Exists(tempFile))
                {
                    string username = "";

                    using (file = new StreamReader(File.OpenRead(tempFile), System.Text.Encoding.UTF8))
                    {
                        username = file.ReadLine();
                    }

                    File.Delete(tempFile);

                    if (!string.IsNullOrEmpty(username))
                    {
                        ConfigurationManager.AppSettings["RememberUserName"] = username;
                        ConfigurationManager.AppSettings["RememberMe"] = "True";
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            var cult = ConfigurationManager.AppSettings["culture"];

            if (!string.IsNullOrEmpty(cult))
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cult);
                Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("cs-CZ");
                Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;
            }


            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

            AppStartClass.Init();

            AppStartClass.Current.PropertyChanged += new PropertyChangedEventHandler(AppStart);

            updateWindow = new UpdateWindow();
            updateWindow.Show();

        }

        UpdateWindow updateWindow = null;

        void AppStart(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResultStartCode")
            {
                //chyba - VYPNEME APP
                if (AppStartClass.Current.ResultStartCode == 1)
                {
                    MessageBox.Show("Error durning COS updating. Please check your network connection or contact technical support.");
                    App.Current.Shutdown();
                }//updated - restartujeme APP
                else if (AppStartClass.Current.ResultStartCode == 2)
                {

                    var shortcutName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                                      "\\", "COS Production", "\\", "COS - Production", ".appref-ms");
                    try
                    {
                        string username = ConfigurationManager.AppSettings["RememberUserName"];
                        bool remb = false;

                        bool.TryParse(ConfigurationManager.AppSettings["RememberMe"], out remb);

                        if (remb && !string.IsNullOrEmpty(username))
                        {
                            string tempDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\COS";
                            if (!Directory.Exists(tempDir))
                                Directory.CreateDirectory(tempDir);

                            using (var file = new StreamWriter(File.Create(tempDir + @"\tempdata.tmpd"), System.Text.Encoding.UTF8))
                            {
                                file.Write(username);
                                file.Close();
                            }

                            Process.Start(shortcutName, username);
                        }
                        else
                            Process.Start(shortcutName);
                    }
                    finally
                    {
                        App.Current.Shutdown();
                    }
                }
                else if (AppStartClass.Current.ResultStartCode == 3)
                {
                    GoToApp();
                    updateWindow.Close();
                }
            }
        }




        private void GoToApp()
        {
            bool continueToApp = true;
            try
            {
                COSContext.Init(System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString);
            }
            catch (Exception exc)
            {
                continueToApp = false;
                Dictionary<string, string> info = new Dictionary<string, string>();
                try
                {
                    info.Add("User", COSContext.Current.UserFullName);
                }
                catch
                { }
                try
                {
                    info.Add("App version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                }
                catch
                { }

                Logging.LogException(exc, LogType.ToFileAndEmail, info);

                MessageBox.Show(ResourceHelper.GetResource<string>("m_Body_E00000001"));
                this.Shutdown();
            }

            if (continueToApp)
            {
                COSContext.Current.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Current_PropertyChanged);
                App.Current.Resources.Add("COSC", COSContext.Current);


                #region stará verze - pro nastavení usera možno dokomentit

                //string login = Crypto.DecryptString(e.Args[1], Security.SecurityHelper.SecurityKey);

                //var user = COSContext.Current.Users.FirstOrDefault(a => a.LoginName == login);

                //if (user != null)
                //{
                //    COSContext.Current.CurrentUser = user;
                //    COSContext.Current.GroupID = user.Group_ID;
                //}
                //COSContext.Current.Language = e.Args[3];


                //string cult = COSContext.Current.Language;

                //ResourceDictionary dictToDelete = null;
                //string origSource = null;
                //List<ResourceDictionary> ditsToDelete = new List<ResourceDictionary>();
                //foreach (var res in this.Resources.MergedDictionaries)
                //{
                //    if (res.Source.ToString().ToLower().Contains("cosresources") && res.Source.ToString().ToLower().Contains("localization"))
                //    {
                //        ditsToDelete.Add(res);
                //    }
                //}

                //foreach (var res in ditsToDelete)
                //{
                //    origSource = res.Source.ToString();
                //    string newSource = res.Source.ToString();

                //    try
                //    {
                //        int start = 37;

                //        int end = 37 + origSource.Substring(start, 10).IndexOf("/");

                //        newSource = origSource.Replace(origSource.Substring(start, end - start), cult);

                //        this.Resources.MergedDictionaries.Remove(dictToDelete);

                //        this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(newSource, UriKind.Relative) });
                //    }
                //    catch
                //    {
                //        try
                //        {
                //            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(origSource, UriKind.Relative) });
                //        }
                //        catch
                //        {
                //            //jinak by to melo zustat nezmeneno
                //        }
                //    }

                //}

                //RadMainWindow wnd = new RadMainWindow();

                //COSContext.Current.RadMainWindow = wnd;

                #endregion

                #region nová verze - odkomentit do ostrého provozu

                wndSplash = new Splash.SplashWindow();

                COSContext.Current.RadMainWindow = wndSplash;

                wndSplash.Show();

                #endregion
            }
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is ValidationException)
            {
                e.Handled = true;
            }
            else
            {
                Dictionary<string, string> info = new Dictionary<string, string>();
                try
                {
                    info.Add("User", COSContext.Current.UserFullName);
                }
                catch
                { }
                try
                {
                    info.Add("App version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                }
                catch
                { }

                Logging.LogException(e.Exception, LogType.ToFileAndEmail, info);

                e.Handled = true;

                SqlException sqlexc = e.Exception as SqlException;
                if (sqlexc == null && e.Exception.InnerException != null)
                    sqlexc = e.Exception.InnerException as SqlException;

                if (sqlexc != null && sqlexc.Number == 547)
                {

                    MessageBox.Show("Na tuto položku je vázán jeden nebo více jiných záznamů v dalších modulech.");
                    COSContext.Current.ErrorHandled = true;

                }
                else
                {
                    MessageBox.Show(ResourceHelper.GetResource<string>("m_Body_E00000001"));

                    e.Dispatcher.InvokeShutdown();
                }
            }
        }

        COS.Splash.SplashWindow wndSplash = null;

        void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LoginToApp")
            {
                //wndSplash.Close();

                RadMainWindow wnd = new RadMainWindow();

                COSContext.Current.RadMainWindow = wnd;

                wnd.Loaded += new RoutedEventHandler(wnd_Loaded);

                wnd.Show();

                wndSplash.Hide();
            }
            else if (e.PropertyName == "Language")
            {
                string cult = COSContext.Current.Language;


                Dictionary<ResourceDictionary, string> dictsToDelete = new Dictionary<ResourceDictionary, string>();

                foreach (var res in this.Resources.MergedDictionaries)
                {
                    if (res.Source.ToString().ToLower().Contains("cosresources") && res.Source.ToString().ToLower().Contains("localization"))
                    {
                        dictsToDelete.Add(res, res.Source.ToString());
                    }
                }

                foreach (var dictToDelete in dictsToDelete)
                {

                    string newSource = dictToDelete.Value;

                    try
                    {
                        int start = 37;

                        int end = 37 + dictToDelete.Value.Substring(start, 10).IndexOf("/");

                        newSource = dictToDelete.Value.Replace(dictToDelete.Value.Substring(start, end - start), cult);

                        this.Resources.MergedDictionaries.Remove(dictToDelete.Key);

                        this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(newSource, UriKind.Relative) });
                    }
                    catch
                    {
                        try
                        {
                            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(dictToDelete.Value, UriKind.Relative) });
                        }
                        catch
                        {
                            //jinak by to melo zustat nezmeneno
                        }
                    }
                }
            }
        }

        void wnd_Loaded(object sender, RoutedEventArgs e)
        {
            wndSplash.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
