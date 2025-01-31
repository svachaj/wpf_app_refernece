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
using COS.Application.TechnicalMaintenance.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Windows.Threading;

namespace COS.Application.TechnicalMaintenance.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class TpmPlanOverviewView : BaseUserControl
    {
        public TpmPlanOverviewView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new TpmPlanOverviewViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                Loaded += new RoutedEventHandler(TpmPlanOverviewView_Loaded);

                //refreshTimer = new DispatcherTimer();
                //refreshTimer.Interval = new TimeSpan(0, 0, 5);
                //refreshTimer.Tick += new EventHandler(refreshTimer_Tick);

                //refreshTimer.Start();
            }
        }

        void refreshTimer_Tick(object sender, EventArgs e)
        {
            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.TpmPlans);

            var from = Model.SelectedDate;
            var to = Model.SelectedDate.AddDays(1);

            Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to).ToList();

        }

        DispatcherTimer refreshTimer = null;

        void TpmPlanOverviewView_Loaded(object sender, RoutedEventArgs e)
        {

            var from = Model.SelectedDate;
            var to = Model.SelectedDate.AddDays(1);

            Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to).ToList();
        }

        TechnicalMaintenance.Models.TpmPlanOverviewViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvTpmPlans.Rebind();
            }
        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LocalPlans")
            {
                grvTpmPlans.ItemsSource = Model.LocalPlans;
                grvTpmPlans.Rebind();
            }
            else if (e.PropertyName == "SelectedDate")
            {
                var from = Model.SelectedDate;
                var to = Model.SelectedDate.AddDays(1);

                Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to).ToList();
            }
            else if (e.PropertyName == "SelectedWorkCenter")
            {
                if (Model.SelectedWorkCenter != null)
                {
                    var from = Model.SelectedDate;
                    var to = Model.SelectedDate.AddDays(1);

                    Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to && a.Equipment.ID_WorkCenter == Model.SelectedWorkCenter.ID).ToList();
                }
                else if (Model.SelectedWorkGroup != null)
                {
                    var from = Model.SelectedDate;
                    var to = Model.SelectedDate.AddDays(1);

                    Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to && a.Equipment.ID_Workgroup == Model.SelectedWorkGroup.ID).ToList();
                }
                else 
                {
                    var from = Model.SelectedDate;
                    var to = Model.SelectedDate.AddDays(1);

                    Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to).ToList();
                }
            }
            else if (e.PropertyName == "SelectedWorkGroup")
            {
                if (Model.SelectedWorkGroup != null)
                {
                    var from = Model.SelectedDate;
                    var to = Model.SelectedDate.AddDays(1);

                    Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to && a.Equipment.ID_Workgroup == Model.SelectedWorkGroup.ID).ToList();
                }
                else 
                {
                    var from = Model.SelectedDate;
                    var to = Model.SelectedDate.AddDays(1);

                    Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to).ToList();
                }
            }
            else if (e.PropertyName == "LocalWorkCenters")
            {

            }


            else if (e.PropertyName == "Deleted")
            {
                //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.TpmPlans);
                grvTpmPlans.ItemsSource = COSContext.Current.TpmPlans.ToList();
                grvTpmPlans.Rebind();
            }
        }



        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.TpmPlans);

            var from = Model.SelectedDate;
            var to = Model.SelectedDate.AddDays(1);

            Model.LocalPlans = COSContext.Current.TpmPlans.Where(a => a.TpmStartDateTime >= from && a.TpmStartDateTime < to).ToList();

            
        }

        private void minusDay_click(object sender, RoutedEventArgs e)
        {
            if (Model != null)
                Model.SelectedDate = Model.SelectedDate.AddDays(-1);
        }

        private void plusDay_click(object sender, RoutedEventArgs e)
        {
            if (Model != null)
                Model.SelectedDate = Model.SelectedDate.AddDays(1);
        }
    }
}
