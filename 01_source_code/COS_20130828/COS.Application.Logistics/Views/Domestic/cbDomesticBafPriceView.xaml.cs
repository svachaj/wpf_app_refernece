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

namespace COS.Application.Logistics.Views.Domestic
{
    /// <summary>
    /// Interaction logic for cbDomesticBafPriceView.xaml
    /// </summary>
    public partial class cbDomesticBafPriceView : BaseUserControl
    {
        public cbDomesticBafPriceView()

            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new cbDomesticBafPriceViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);
             
            }

        }

        cbDomesticBafPriceViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvCodebook.Rebind();
            }
        }

        private cbDomesticBafPriceDetailView detailView = null;
        public RadWindow DetailWindow = null;

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                DomescticBafPrice item = new DomescticBafPrice();

                item.ValidFrom = COSContext.Current.DateTimeServer;

                if (DetailWindow == null)
                {
                    DetailWindow = new RadWindow();


                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("log_cbBaf");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new cbDomesticBafPriceDetailView();
                    detailView.RaiseWindow = DetailWindow;
                    DetailWindow.Content = detailView;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                }

                tempLocalize = new SysLocalize();
                tempLocalize.ID = COSContext.Current.SysLocalizes.Max(a => a.ID) + 1;
                COSContext.Current.SysLocalizes.AddObject(tempLocalize);
                //item.SysLocalize = tempLocalize;
                //COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                //item.ID_Localization_description = tempLocalize.ID;
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
                    DomescticBafPrice item = grvCodebook.SelectedItem as DomescticBafPrice;

                    if (item != null)
                    {
                        COSContext.Current.DeleteObject(item);
                        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                        grvCodebook.Rebind();
                    }
                }
            }
        }


        SysLocalize tempLocalize = null;

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ViewAccountTypes);
                grvCodebook.Rebind();
            }
            else
            {
                COSContext.Current.RejectChanges();
                //if (tempLocalize != null)
                //{
                //    COSContext.Current.DeleteObject(tempLocalize);
                //    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                //}

            }
        }


        private void grvCodeBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("cbDomesticBafPrice", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        DomescticBafPrice item = grvCodebook.SelectedItem as DomescticBafPrice;

                        if (item != null)
                        {
                            if (DetailWindow == null)
                            {
                                DetailWindow = new RadWindow();

                                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                                DetailWindow.Header = ResourceHelper.GetResource<string>("log_cbBaf");// this.Resources["Divisions"].ToString();
                                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new cbDomesticBafPriceDetailView();
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
    }
}
