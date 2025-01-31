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

using Telerik.Windows.Controls;
using System.Configuration;

namespace COS.Application
{
    /// <summary>
    /// Interaction logic for RadMainWindow.xaml
    /// </summary>
    public partial class RadMainWindow
    {
        public RadMainWindow()
        {
            InitializeComponent();

            RadWindowHelper windowHelper = new RadWindowHelper();
            windowHelper.TaskBarDisplayed += new EventHandler(windowHelper_TaskBarDisplayed);
            this.Closed += new EventHandler<WindowClosedEventArgs>(RadMainWindow_Closed);
            windowHelper.ShowWindowInTaskBar();

        }

        void RadMainWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            App.Current.Shutdown();
        }

        void windowHelper_TaskBarDisplayed(object sender, EventArgs e)
        {
            this.Show();
            //this.IconTemplate = this.Resources["WindowIconTemplate"] as DataTemplate;

            this.WindowState = WindowState.Normal;
            this.BringToFront();
            var window = this.ParentOfType<Window>();
            window.ShowInTaskbar = true;
            window.Icon = COS.Resources.ResourceHelper.GetResource<Image>("MainAppWindowIcon").Source;
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("App_WindownMain");

            var versType = ConfigurationManager.AppSettings["VersionType"];

            if (!string.IsNullOrEmpty(versType)) 
            {
                this.Header += " [ " + versType + " ]";
            }
         }

        private void RadWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
      
    }

    public class RadWindowHelper
    {
        public event EventHandler TaskBarDisplayed;

        public void ShowWindowInTaskBar()
        {
            if (this.TaskBarDisplayed != null)
                this.TaskBarDisplayed(this, new EventArgs());
        }
    }
}
