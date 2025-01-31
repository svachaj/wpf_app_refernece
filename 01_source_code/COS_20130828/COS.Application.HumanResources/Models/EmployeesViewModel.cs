using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using COS.Application.HumanResources.Views;

namespace COS.Application.HumanResources.Models
{
    public partial class EmployeesViewModel : ValidationViewModelBase
    {
        public EmployeesViewModel()
            : base()
        {

        }

        public ICommand SynchronizeCommand
        {
            get
            {
                return new RelayCommand(param => this.Synchro());
            }
        }

        private void Synchro()
        {
            RadWindow wnd = new RadWindow();
            wnd.Closed += new EventHandler<WindowClosedEventArgs>(wnd_Closed);
            wnd.ResizeMode = ResizeMode.CanMinimize;
            wnd.Header = ResourceHelper.GetResource<string>("hr_Synchronization");
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wnd.MinHeight = 500;
            wnd.MinWidth = 600;
            SynchroEmployeesView modelview = new SynchroEmployeesView();
            wnd.Content = modelview;

            StyleManager.SetTheme(wnd, new Expression_DarkTheme());
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();
        }

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            OnPropertyChanged("RebindData");
        }




    }
}
