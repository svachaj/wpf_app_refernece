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
using COS.Application.Production.Models;
using System.ComponentModel;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Resources;
using System.Data.SqlClient;

namespace COS.Application.Production.Views
{
    /// <summary>

    /// </summary>
    public partial class PlanningVA4HView : BaseUserControl
    {
        public PlanningVA4HView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {

                var str = System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString;
                string decryptString = Crypto.DecryptString(str, Security.SecurityHelper.SecurityKey);
                dataContext = new COSContext(decryptString);

                Model = new PlanningVA4HViewModel(dataContext);
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                this.Loaded += new RoutedEventHandler(PlanningVA4HView_Loaded);

                mathFunc = dataContext.MathFunctions.FirstOrDefault(a => a.Key == "VA4H_difficulty");
               
            }

        }

        private MathFunction mathFunc = null;

        void PlanningVA4HView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        COSContext dataContext;
        PlanningVA4HViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvCodebook.Rebind();
            }
        }

        public override void RefreshData()
        {
            Model.InitLocalItems(null);
            grvCodebook.ItemsSource = null;
            grvCodebook.ItemsSource = Model.LocalItems;
        }

        private PlanningVA4HDetailView detailView = null;
        public RadWindow DetailWindow = null;

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                VA4H item = dataContext.VA4H.CreateObject();

                if (DetailWindow == null)
                {
                    DetailWindow = new RadWindow();

                    DetailWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 100;
                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("pane_Production4HPlanning");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new PlanningVA4HDetailView(dataContext);
                    detailView.RaiseWindow = DetailWindow;
                    DetailWindow.Content = detailView;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                }

                dataContext.Refresh(System.Data.Objects.RefreshMode.StoreWins, dataContext.VA4H_difficultyType);
                var diffTypes = dataContext.VA4H_difficultyType.ToList();

                VA4H_difficulty diff = null;
                foreach (var itm in diffTypes)
                {
                    diff = dataContext.VA4H_difficulty.CreateObject();
                    diff.DifficultyType = itm;
                    item.Difficulty.Add(diff);
                    diff.PropertyChanged += new PropertyChangedEventHandler(diff_PropertyChanged);

                }

                //item.SOConstructionDeadlineDate = DateTime.Today;
                //item.SOConstructionPlannedDate = DateTime.Today;
                item.SOCreatedDate = DateTime.Today;
                //item.SOExpeditionDeadlineDate = DateTime.Today;
                //item.SOProductionDeadlineDate = DateTime.Today;
                //item.SORecivedDate = DateTime.Today;

                detailView.model.SelectedItem = item;
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
                    Content = COS.Resources.ResourceHelper.GetResource<string>("m_General_Delete"),
                    Header = COS.Resources.ResourceHelper.GetResource<string>("m_Header1_A"),
                    Closed = new EventHandler<WindowClosedEventArgs>(OnConfirmClosed)
                });

            }
            else if (e.PropertyName == "SelectedDate" || e.PropertyName == "SelectedConstructer")
            {
                RefreshData();
            }
        }

        void diff_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                if (detailView != null && detailView.model != null && detailView.model.SelectedItem != null)
                {
                    detailView.model.SelectedItem.Difficulity = CalculateDifficulty();
                }
            }
        }

        private int? CalculateDifficulty()
        {
            int? result = null;
            if (detailView != null && detailView.model != null && detailView.model.SelectedItem != null)
            {
                result = 0;
                decimal percs = 0;
                foreach (var itm in detailView.model.SelectedItem.Difficulty)
                {
                    if (itm.IsChecked)
                        percs += itm.DifficultyType.PercentWeight;
                }

                var totalCount = detailView.model.SelectedItem.Difficulty.Where(a => a.IsChecked).Count();
                if (totalCount > 0)
                {
                    var tocal = percs / totalCount;
                    
                    dotMath.EqCompiler comp = new dotMath.EqCompiler(true);

                    comp.SetVariable("difficulty", (double)tocal);

                    comp.SetFunction(mathFunc.Function);

                    var res = comp.Calculate();

                    result = (int)res;
                }
                else 
                {
                    result = 0;
                }


            }
            return result;
        }

        private void OnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                if (grvCodebook.SelectedItem != null)
                {
                    VA4H item = grvCodebook.SelectedItem as VA4H;

                    if (item != null)
                    {
                        try
                        {
                            dataContext.DeleteObject(item);
                            dataContext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                            RefreshData();
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




        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                Model.InitLocalItems(null);
                grvCodebook.Rebind();
            }
            else
            {
                dataContext.RejectChanges();

            }
        }


        private void grvCodeBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (COS.Common.WPF.Helpers.HasRightForOperation("AdsCustomerService", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        VA4H item = grvCodebook.SelectedItem as VA4H;

                        if (item != null)
                        {
                            if (DetailWindow == null)
                            {
                                DetailWindow = new RadWindow();

                                DetailWindow.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - 100;
                                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                                DetailWindow.Header = ResourceHelper.GetResource<string>("pane_Production4HPlanning");
                                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new PlanningVA4HDetailView(dataContext);
                                detailView.RaiseWindow = DetailWindow;
                                DetailWindow.Content = detailView;

                                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                            }


                            detailView.model.SelectedItem = item;
                            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                            DetailWindow.ShowDialog();
                        }
                    }

                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void minusDay_click(object sender, RoutedEventArgs e)
        {
            Model.SelectedDate = Model.SelectedDate.AddDays(-1);
        }

        private void plusDay_click(object sender, RoutedEventArgs e)
        {
            Model.SelectedDate = Model.SelectedDate.AddDays(1);
        }
    }
}
