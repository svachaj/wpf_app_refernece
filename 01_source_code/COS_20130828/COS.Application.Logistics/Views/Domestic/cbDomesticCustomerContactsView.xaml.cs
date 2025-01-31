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
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Resources;
using System.Data.SqlClient;

namespace COS.Application.Logistics.Views.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticDriverView.xaml
    /// </summary>
    public partial class cbDomesticCustomerContactsView : BaseUserControl
    {
        public cbDomesticCustomerContactsView()

            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {

                var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                dataContext = new COSContext(decryptString);

                Model = new cbDomesticCustomerContactsViewModel(dataContext);

                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                this.Loaded += new RoutedEventHandler(cbDomesticCustomerContactsView_Loaded);

            }

        }

        COSContext dataContext;

        void cbDomesticCustomerContactsView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        cbDomesticCustomerContactsViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvCodebook.Rebind();
            }
        }

        private cbDomesticCustomerContactsDetailView detailView = null;
        public RadWindow DetailWindow = null;

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
              
                isNew = true;

                if (DetailWindow == null)
                {
                    DetailWindow = new RadWindow();


                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_DomesticCustomer");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new cbDomesticCustomerContactsDetailView(dataContext);
                    detailView.RaiseWindow = DetailWindow;
                    DetailWindow.Content = detailView;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                }

                DomesticCustomer item = detailView.Model.CreateNewItem();

                detailView.Model.SelectedItem = item;
                DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                DetailWindow.ShowDialog();
            }
            else if (e.PropertyName == "Delete")
            {
                RadWindow.Confirm(new DialogParameters()
                {
                    OkButtonContent = COS.Resources.ResourceHelper.GetResource<string>("m_General_Y"),
                    CancelButtonContent = COS.Resources.ResourceHelper.GetResource<string>("m_General_N"),
                    DialogStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = COS.Resources.ResourceHelper.GetResource<string>("m_Body_LOG00000005"),
                    Header = COS.Resources.ResourceHelper.GetResource<string>("m_Header1_A"),
                    Closed = new EventHandler<WindowClosedEventArgs>(OnConfirmClosed)
                });

            }
        }

        private void OnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                if (grvCodebook.SelectedItem != null)
                {
                    DomesticCustomer item = grvCodebook.SelectedItem as DomesticCustomer;

                    if (item != null)
                    {
                        try
                        {
                            this.dataContext.DeleteObject(item);
                            this.dataContext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            Model.RefreshData();
                            grvCodebook.Rebind();
                        }
                        catch (Exception exc)
                        {
                            SqlException sqlexc = exc as SqlException;
                            if (sqlexc == null && exc.InnerException != null)
                                sqlexc = exc.InnerException as SqlException;

                            if (sqlexc != null && sqlexc.Number == 547)
                            {
                                MessageBox.Show("Na tuto položku je vázán jeden nebo více jiných záznamů v dalších modulech.");
                                COSContext.Current.ErrorHandled = true;

                            }
                            else
                            {
                                MessageBox.Show(ResourceHelper.GetResource<string>("m_Body_E00000001"));
                            }

                            dataContext.RejectChanges();
                        }
                    }
                }
            }
        }


        SysLocalize tempLocalize = null;
        bool isNew = false;

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                if (isNew)
                    RefreshData();

                grvCodebook.Rebind();
            }
            else
            {
                this.dataContext.RejectChanges();
                //if (tempLocalize != null)
                //{
                //    COSContext.Current.DeleteObject(tempLocalize);
                //    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                //}

            }

            isNew = false;
        }


        private void grvCodeBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("cbDomesticCustomer", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        DomesticCustomer item = grvCodebook.SelectedItem as DomesticCustomer;

                        if (item != null)
                        {
                            if (DetailWindow == null)
                            {
                                DetailWindow = new RadWindow();

                                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                                DetailWindow.Header = ResourceHelper.GetResource<string>("log_DomesticCustomer");// this.Resources["Divisions"].ToString();
                                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new cbDomesticCustomerContactsDetailView(dataContext);
                                detailView.RaiseWindow = DetailWindow;
                                DetailWindow.Content = detailView;

                                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                            }
                            tempLocalize = null;
                            //  WorkGroup at = COSContext.Current.WorkGroups.FirstOrDefault(a => a.ID == accType.ID);
                            detailView.Model.SelectedItem = item;
                            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                            DetailWindow.ShowDialog();
                        }
                    }
                }
            }
        }


        public override void RefreshData()
        {
            Model.RefreshData();
            grvCodebook.ItemsSource = null;
            grvCodebook.ItemsSource = Model.LocalCustomers;
        }
    }
}
