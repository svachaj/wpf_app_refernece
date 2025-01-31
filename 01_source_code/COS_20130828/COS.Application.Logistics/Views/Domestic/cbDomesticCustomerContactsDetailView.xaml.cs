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
using COS.Application.Shared;
using COS.Application.Logistics.Models.Domestic;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace COS.Application.Logistics.Views.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticDriverDetailView.xaml
    /// </summary>
    public partial class cbDomesticCustomerContactsDetailView : BaseUserControl
    {
        public cbDomesticCustomerContactsDetailView(COSContext datacontext)
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                dataContext = datacontext;

                Model = new cbDomesticCustomerContactsDetailViewModel(dataContext);
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

            }
        }

        COSContext dataContext;

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cancel")
            {
                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = false;
                    RaiseWindow.Close();
                }

            }
            else if (e.PropertyName == "Save")
            {

                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = true;
                    RaiseWindow.Close();
                }

            }

        }


        public cbDomesticCustomerContactsDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

        private void grvCodeBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DomesticCustomerContact item = (sender as RadGridView).SelectedItem as DomesticCustomerContact;
            if (item != null)
            {
                detailView = new cbDomesticCustomerContactDetailView(Model.SelectedItem, Model.dataContext);

                if (DetailWindow == null)
                {
                    DetailWindow = new RadWindow();

                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_cbDomesticDriver");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                }

                detailView.RaiseWindow = DetailWindow;
                DetailWindow.Content = detailView;
                detailView.Model.SelectedItem = item;
                DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                DetailWindow.ShowDialog();
            }
        }

        RadWindow DetailWindow;
        cbDomesticCustomerContactDetailView detailView;
        DomesticCustomerContact newItem;
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            newItem = new DomesticCustomerContact();

            newItem.Customer = this.Model.SelectedItem;
            detailView = new cbDomesticCustomerContactDetailView(Model.SelectedItem, Model.dataContext);

            Model.ToAdd.Add(newItem);

            if (DetailWindow == null)
            {
                DetailWindow = new RadWindow();


                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_cbDomesticDriver");
                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
            }

            detailView.RaiseWindow = DetailWindow;
            DetailWindow.Content = detailView;
            detailView.Model.SelectedItem = newItem;
            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            DetailWindow.ShowDialog();
        }

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {

            }
            else
            {
                COSContext.Current.ObjectStateManager.ChangeObjectState(newItem, System.Data.EntityState.Detached);
                Model.ToAdd.Remove(newItem);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DomesticCustomerContact cont = grvCodebook.SelectedItem as DomesticCustomerContact;

            if (cont != null)
            {
                Model.SelectedItem.Contacts.Remove(cont);
                Model.ToDelete.Add(cont);
                // COSContext.Current.DomesticCustomerContacts.DeleteObject(cont);
            }
        }
    }
}
