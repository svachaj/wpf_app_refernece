using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using COS.Application.Shared;
using Telerik.Windows.Controls;

namespace COS.Common.WPF
{
    public static class Helpers
    {
        public static void LoadLocalizeResources(FrameworkElement element)
        {
            if (element != null)
            {
                //string cult = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                string cult = COSContext.Current.Language;

                ResourceDictionary dictToDelete = null;
                string origSource = null;

                foreach (var res in element.Resources.MergedDictionaries)
                {
                    if (res.Source.ToString().ToLower().Contains("cosresources") && res.Source.ToString().ToLower().Contains("localization"))
                    {
                        dictToDelete = res;
                        origSource = res.Source.ToString();
                        break;
                    }
                }

                if (dictToDelete != null)
                {
                    string newSource = origSource;

                    try
                    {
                        int start = 37;

                        int end = 37 + origSource.Substring(start, 10).IndexOf("/");

                        newSource = origSource.Replace(origSource.Substring(start, end - start), cult);

                        element.Resources.MergedDictionaries.Remove(dictToDelete);

                        element.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(newSource, UriKind.Relative) });
                    }
                    catch
                    {
                        try
                        {
                            element.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(origSource, UriKind.Relative) });
                        }
                        catch
                        {
                            //jinak by to melo zustat nezmeneno
                        }
                    }
                }
            }
        }

        public static void ApplyAllRights(UserControl element)
        {
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysEGPs.Where(a => a.ID_sys_group == COSContext.Current.GroupID));
           
            var groupRights = COSContext.Current.SysEGPs.Where(a => a.ID_sys_group == COSContext.Current.GroupID).ToList();

            foreach (var item in element.ChildrenOfType<FrameworkElement>())
            {
                if (item.Tag != null && !string.IsNullOrEmpty(item.Tag.ToString()))
                {
                    string itemtag = item.Tag.ToString();

                    if (itemtag.StartsWith("module_"))
                    {
                        string modul = itemtag.Replace("module_", "");
                        var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.Name == modul);
                        if (cls != null)
                        {
                            var egp = groupRights.FirstOrDefault(a => a.Action.Code == "Visibility" && a.ID_sys_class == cls.ID);

                            if (egp != null)
                            {
                                item.Visibility = egp.Granted.HasValue ? egp.Granted.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                            }
                            else
                                item.Visibility = Visibility.Collapsed;

                        }
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                    else if (itemtag.StartsWith("subm_"))
                    {
                        string submodul = itemtag.Replace("subm_", "");
                        var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.Name == submodul);
                        if (cls != null)
                        {
                            var egp = groupRights.FirstOrDefault(a => a.ID_sys_class == cls.ID && a.Granted == true);

                            if (egp != null)
                            {
                                item.Visibility = egp.Granted.HasValue ? egp.Granted.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                            }
                            else
                                item.Visibility = Visibility.Collapsed;

                        }
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                    else if (itemtag.StartsWith("dyntbl"))
                    {


                    }
                    else
                    {
                        var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.UserControlTag == itemtag);
                        if (cls != null)
                        {
                            var egp = groupRights.FirstOrDefault(a => a.Action.Code == "Visibility" && a.ID_sys_class == cls.ID);

                            if (egp != null)
                            {
                                item.Visibility = egp.Granted.HasValue ? egp.Granted.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                            }
                            else
                                item.Visibility = Visibility.Collapsed;

                            egp = groupRights.FirstOrDefault(a => a.Action.Code == "Edit" && a.ID_sys_class == cls.ID);

                            if (egp != null)
                            {
                                item.IsEnabled = egp.Granted.HasValue ? egp.Granted.Value ? true : false : false;
                            }
                            else
                                item.IsEnabled = false;

                        }
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public static void ApplyAllRightsRwindow(RadWindow element)
        {
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysEGPs.Where(a => a.ID_sys_group == COSContext.Current.GroupID));
           
            var groupRights = COSContext.Current.SysEGPs.Where(a => a.ID_sys_group == COSContext.Current.GroupID).ToList();

            foreach (var item in element.ChildrenOfType<FrameworkElement>())
            {
                if (item.Tag != null && !string.IsNullOrEmpty(item.Tag.ToString()))
                {
                    string itemtag = item.Tag.ToString();

                    if (itemtag.StartsWith("module_"))
                    {
                        string modul = itemtag.Replace("module_", "");
                        var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.Name == modul);
                        if (cls != null)
                        {
                            var egp = groupRights.FirstOrDefault(a => a.Action.Code == "Visibility" && a.ID_sys_class == cls.ID);

                            if (egp != null)
                            {
                                item.Visibility = egp.Granted.HasValue ? egp.Granted.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                            }
                            else
                                item.Visibility = Visibility.Collapsed;

                        }
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                    else if (itemtag.StartsWith("subm_"))
                    {
                        string submodul = itemtag.Replace("subm_", "");
                        var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.Name == submodul);
                        if (cls != null)
                        {
                            var egp = groupRights.FirstOrDefault(a => a.ID_sys_class == cls.ID && a.Granted == true);

                            if (egp != null)
                            {
                                item.Visibility = egp.Granted.HasValue ? egp.Granted.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                            }
                            else
                                item.Visibility = Visibility.Collapsed;

                        }
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                    else if (itemtag.StartsWith("dyntbl"))
                    {


                    }
                    else
                    {
                        var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.UserControlTag == itemtag);
                        if (cls != null)
                        {
                            var egp = groupRights.FirstOrDefault(a => a.Action.Code == "Visibility" && a.ID_sys_class == cls.ID);

                            if (egp != null)
                            {
                                item.Visibility = egp.Granted.HasValue ? egp.Granted.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                            }
                            else
                                item.Visibility = Visibility.Collapsed;

                            egp = groupRights.FirstOrDefault(a => a.Action.Code == "Edit" && a.ID_sys_class == cls.ID);

                            if (egp != null)
                            {
                                item.IsEnabled = egp.Granted.HasValue ? egp.Granted.Value ? true : false : false;
                            }
                            else
                                item.IsEnabled = false;

                        }
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public static bool HasRightForOperation(string classname, string actionCode)
        {
            bool result = false;

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysEGPs.Where(a => a.ID_sys_group == COSContext.Current.GroupID));
            var groupRights = COSContext.Current.SysEGPs.Where(a => a.ID_sys_group == COSContext.Current.GroupID).ToList();

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysClasses.Where(a => a.Name == classname));          
            var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.Name == classname);
            if (cls != null)
            {
                var egp = groupRights.FirstOrDefault(a => a.Action.Code == actionCode && a.ID_sys_class == cls.ID);

                if (egp != null && egp.Granted.HasValue)
                {
                    result = egp.Granted.Value;
                }
                else
                    result = false;
            }


            return result;
        }

        public static void SaveDockingLayout(string xmlLayout, string key)
        {
            UserProperty prop = COSContext.Current.UserProperties.Where(a => a.ID_user == COSContext.Current.CurrentUser.ID && a.Key == key).FirstOrDefault();

            if (prop == null)
            {
                prop = COSContext.Current.UserProperties.CreateObject();
                prop.ID_user = COSContext.Current.CurrentUser.ID;

                prop.Key = key;

                COSContext.Current.UserProperties.AddObject(prop);
            }

            prop.Value = xmlLayout;
            try
            {

                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }
            catch { }

        }

        public static string LoadDockingLayout(string key)
        {
            string result = null;

            UserProperty prop = COSContext.Current.UserProperties.Where(a => a.ID_user == COSContext.Current.CurrentUser.ID && a.Key == key).FirstOrDefault();

            if (prop != null)
            {
                result = prop.Value;
            }

            return result;
        }

        static List<MathFunction> mathfunctions = null;

        public static double CalculateFunction(string functionKey, Dictionary<string, double> values)
        {
            double result = 0;

            if (mathfunctions == null)
                mathfunctions = COSContext.Current.MathFunctions.ToList();

            var func = mathfunctions.FirstOrDefault(a => a.Key == functionKey);

            if (func != null)
            {
                dotMath.EqCompiler compiler = new dotMath.EqCompiler(true);

                compiler.SetFunction(func.Function);
                foreach (var itm in values)
                {
                    compiler.SetVariable(itm.Key, itm.Value);
                }

                result = compiler.Calculate();

                if (double.IsInfinity(result) || double.IsNaN(result))
                    result = 0;
            }

            return result;
        }

       
    }


    public class RadWindows
    {
        public static RadWindow Alert(string header, object content, ContentControl owner)
        {
            RadWindow wnd = new RadWindow();
            Grid gd = new Grid();

            gd.Children.Add(new TextBlock() { Text = content.ToString() });
            gd.Margin = new Thickness(5);

            wnd.Content = gd;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wnd.ResizeMode = ResizeMode.NoResize;

            StyleManager.SetTheme(wnd, new Expression_DarkTheme());

            wnd.Owner = owner;

            wnd.IsTopmost = true;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();

            return wnd;
        }
    }
}
