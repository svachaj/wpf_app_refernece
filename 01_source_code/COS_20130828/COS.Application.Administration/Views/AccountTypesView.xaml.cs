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
using COS.Application.Administration.Models;
using System.ComponentModel;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Resources;

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for UsersView.xaml
    /// </summary>
    public partial class AccountTypesView : BaseUserControl
    {
        public AccountTypesView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new AccountTypesViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

            }

        }

        AccountTypesViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvAccTypes.Rebind();
            }
        }

        private AccountTypeDetailView detailView = null;
        public RadWindow AccTypeDetailWindow = null;

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                SysAccountType accType = new SysAccountType();


                if (AccTypeDetailWindow == null)
                {
                    AccTypeDetailWindow = new RadWindow();


                    AccTypeDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    AccTypeDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(AccTypeDetailWindow_Closed);
                    AccTypeDetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("adm_AccType");
                    AccTypeDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new AccountTypeDetailView();
                    detailView.RaiseWindow = AccTypeDetailWindow;
                    AccTypeDetailWindow.Content = detailView;

                    StyleManager.SetTheme(AccTypeDetailWindow, new Expression_DarkTheme());
                }

                tempLocalize = new SysLocalize();
                COSContext.Current.SysLocalizes.AddObject(tempLocalize);
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                accType.ID_Localization_description = tempLocalize.ID;
                detailView.Model.SelectedItem = accType;
                AccTypeDetailWindow.ShowDialog();
            }
            else if (e.PropertyName == "Delete")
            {
                if (MessageBox.Show(ResourceHelper.GetResource<string>("m_Body_ADM00000003"), ResourceHelper.GetResource<string>("m_Header1_A"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (grvAccTypes.SelectedItem != null)
                    {
                        ViewAccountType vAccType = grvAccTypes.SelectedItem as ViewAccountType;
                        SysAccountType accType = COSContext.Current.SysAccountTypes.FirstOrDefault(a => a.ID == vAccType.ID);
                        if (accType != null)
                        {
                            COSContext.Current.DeleteObject(accType);
                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            grvAccTypes.Rebind();
                        }
                    }
                }
            }
        }

        SysLocalize tempLocalize = null;

        void AccTypeDetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ViewAccountTypes);
                grvAccTypes.Rebind();
            }
            else
            {
                COSContext.Current.RejectChanges();
                if (tempLocalize != null)
                {
                    COSContext.Current.DeleteObject(tempLocalize);
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                }

            }
        }


        private void grvAccType_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("AccType", "Update"))
            {

                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        ViewAccountType accType = grvAccTypes.SelectedItem as ViewAccountType;

                        if (accType != null)
                        {
                            if (AccTypeDetailWindow == null)
                            {
                                AccTypeDetailWindow = new RadWindow();

                                AccTypeDetailWindow.ResizeMode = ResizeMode.CanMinimize;
                                AccTypeDetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(AccTypeDetailWindow_Closed);
                                AccTypeDetailWindow.Header = ResourceHelper.GetResource<string>("adm_AccType");
                                AccTypeDetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new AccountTypeDetailView();
                                detailView.RaiseWindow = AccTypeDetailWindow;
                                AccTypeDetailWindow.Content = detailView;

                                StyleManager.SetTheme(AccTypeDetailWindow, new Expression_DarkTheme());
                            }
                            tempLocalize = null;
                            SysAccountType at = COSContext.Current.SysAccountTypes.FirstOrDefault(a => a.ID == accType.ID);
                            detailView.Model.SelectedItem = at;
                            AccTypeDetailWindow.ShowDialog();
                        }
                    }
                }
            }
        }


    }
}
