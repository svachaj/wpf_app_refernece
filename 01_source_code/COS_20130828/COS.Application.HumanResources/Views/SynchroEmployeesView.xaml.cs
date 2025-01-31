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
using System.ComponentModel;
using COS.Application.Shared;
using COS.Application.HumanResources.Models;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using COS.Resources;

namespace COS.Application.HumanResources.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class SynchroEmployeesView : BaseUserControl
    {
        public SynchroEmployeesView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new SynchroEmployeesViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                Loaded += new RoutedEventHandler(SynchroEmployeesView_Loaded);

            }
        }

        void SynchroEmployeesView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Model != null)
            {
                Model.Analyze();
            }
        }

        SynchroEmployeesViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvSyncItems.Rebind();
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WorkCompleted")
            {
                if (Model.WorkCompleted && Model.SynchroItems.Count > 0)
                {
                    grvSyncItems.Rebind();
                }
                else
                {
                    if (!Model.IsSynchro && Model.AnalyzeRuned && Model.WorkCompleted)
                    {
                        Model.AnalyzeRuned = false;
                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_HR00000004"), Header = ResourceHelper.GetResource<string>("m_Header3_I"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                        RadWindow wnd = this.GetParents().OfType<RadWindow>().FirstOrDefault();
                        wnd.Close();
                    }
                }


                if (Model.IsSynchro && Model.WorkCompleted && Model.SynchroItems.Where(a=>a.IsSelected).Count() == 0)
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_HR00000005"), Header = ResourceHelper.GetResource<string>("m_Header3_I"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
                else if (Model.IsSynchro && Model.WorkCompleted && Model.SynchroItems.Where(a => a.IsSelected).Count() > 0)                 
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_HR00000006"), Header = ResourceHelper.GetResource<string>("m_Header3_I"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    Model.SynchroItems.Clear();
                    grvSyncItems.Rebind();
                    RadWindow wnd = this.GetParents().OfType<RadWindow>().FirstOrDefault();
                    wnd.Close();
                }
            }
           

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Model != null)
            {
                foreach (var itm in Model.SynchroItems)
                {
                    itm.IsSelected = true;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Model != null)
            {
                foreach (var itm in Model.SynchroItems)
                {
                    itm.IsSelected = false;
                }
            }
        }

        private void grvSyncItems_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.DataElement != null)
            {
                SynchronizeItem item = e.DataElement as SynchronizeItem;

                if (item != null)
                {
                    if (item.Action == SynchroAction.New)
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Green);
                    }
                    else if (item.Action == SynchroAction.Delete)
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Red);
                    }
                    else if (item.Action == SynchroAction.Update)
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Blue);
                    }
                }
            }
        }

        private void grvSyncItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
             FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
             if (originalSender != null)
             {
                 var row = originalSender.ParentOfType<GridViewRow>();
                 if (row != null)
                 {
                     if (row.DetailsVisibility.HasValue && row.DetailsVisibility.Value == System.Windows.Visibility.Visible)
                         row.DetailsVisibility = System.Windows.Visibility.Collapsed;
                     else
                         row.DetailsVisibility = System.Windows.Visibility.Visible;
                 }
             }
        }



    }
}
