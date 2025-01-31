using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;

namespace COS.Resources
{
    public class ResourceHelper
    {
        public static void ChangeLocalization(Application app, CultureInfo culture)
        {

        }


        public static T GetResource<T>(string key)
        {
            if (Application.Current.Resources.Contains(key))
            {
                try
                {
                    return (T)Application.Current.Resources[key];
                }
                catch 
                {
                    return default(T);
                }
            }
            else
                return default(T);
            
        }
    }
}
