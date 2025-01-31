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
using COS.Application.Production.Models.PlanningVA4H;

namespace COS.Application.Production.Views.PlanningVA4H
{
    /// <summary>
    /// Interaction logic for cbDomesticCompositionView.xaml
    /// </summary>
    public partial class cbProdPlanStateView : BaseUserControl
    {
        public cbProdPlanStateView(COSContext dataContext)

            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {

                if (dataContext == null)
                {
                    var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                    string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                    this.dataContext = new COSContext(decryptString);
                }
                else
                    this.dataContext = dataContext;

                Model = new cbProdPlanStateViewModel(this.dataContext);
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                this.dataContext.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

            }

        }

        COSContext dataContext = null;
        cbProdPlanStateViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvCodebook.Rebind();
            }
        }

        private cbProdPlanStateDetailView detailView = null;
        public RadWindow DetailWindow = null;

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                ProdPlanState item = new ProdPlanState();

                if (DetailWindow == null)
                {
                    DetailWindow = new RadWindow();


                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_cbP4hStateViewPane");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new cbProdPlanStateDetailView(dataContext);
                    detailView.RaiseWindow = DetailWindow;
                    DetailWindow.Content = detailView;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                }

                tempLocalize = new SysLocalize();
                tempLocalize.ID = dataContext.SysLocalizes.Max(a => a.ID) + 1;
                dataContext.SysLocalizes.AddObject(tempLocalize);
                item.SysLocalize = tempLocalize;

                detailView.Model.SelectedItem = item;
                DetailWindow.Owner = (RadWindow)dataContext.RadMainWindow;
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
                    ProdPlanState item = grvCodebook.SelectedItem as ProdPlanState;

                    if (item != null)
                    {
                        dataContext.ProdPlanStates.DeleteObject(item);
                        dataContext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                        Model.LoadLocalProdPlanStates();
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
                Model.LoadLocalProdPlanStates();
                grvCodebook.Rebind();
            }
            else
            {
                dataContext.RejectChanges();

            }
        }


        private void grvCodeBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (COS.Common.WPF.Helpers.HasRightForOperation("cbDomesticComposition", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        ProdPlanState item = grvCodebook.SelectedItem as ProdPlanState;

                        if (item != null)
                        {
                            if (DetailWindow == null)
                            {
                                DetailWindow = new RadWindow();

                                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                                DetailWindow.Header = ResourceHelper.GetResource<string>("pane_cbP4hStateViewPane");// this.Resources["Divisions"].ToString();
                                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new cbProdPlanStateDetailView(dataContext);
                                detailView.RaiseWindow = DetailWindow;
                                DetailWindow.Content = detailView;

                                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                            }
                            tempLocalize = null;

                            detailView.Model.SelectedItem = item;
                            DetailWindow.Owner = (RadWindow)dataContext.RadMainWindow;
                            DetailWindow.ShowDialog();
                        }
                    }
                }
            }
        }
    }
}
