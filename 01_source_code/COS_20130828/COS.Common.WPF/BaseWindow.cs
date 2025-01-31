using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace COS.Common.WPF
{
    public class BaseWindow : Window
    {
        public BaseWindow()
            : base()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += new RoutedEventHandler(BaseWindow_Loaded);
            }
        }

        void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Helpers.LoadLocalizeResources(this);

            //Helpers.ApplyAllRights(this);
        }


    }
}
