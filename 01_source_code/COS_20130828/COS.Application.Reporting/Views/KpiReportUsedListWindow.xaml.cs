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
using COS.Application.Reporting.Models;
using Telerik.Windows.Controls;
using COS.Application.Shared;
using System.ComponentModel;
using COS.Common;
using COS.Resources;

namespace COS.Application.Reporting.Views
{
    /// <summary>
    /// Interaction logic for KpiReportFilterWindow.xaml
    /// </summary>
    public partial class KpiReportUsedListWindow : RadWindow, INotifyPropertyChanged
    {
        public KpiReportUsedListWindow(KPIReportViewModel model, int wog)
        {
            InitializeComponent();
            this.Header = COS.Resources.ResourceHelper.GetResource<string>("rep_Filter");
            Model = model;
            WOG = wog;

            this.DataContext = this;

            Loaded += new RoutedEventHandler(KpiReportFilterWindow_Loaded);

            divisionParam = model.SelectedDivision != null ? model.SelectedDivision.Value : "";
        }

        string divisionParam = "";

        //1 = workgroups ,  2 = workcenters
        public int WOG { set; get; }

        void KpiReportFilterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (WOG == 1)
            {
                objectColumn.Header = ResourceHelper.GetResource<string>("rep_FiltrWgSel");


                var wgs = Model.SelectedDivision != null ? COSContext.Current.WorkGroups.Where(a => a.ID_Division == Model.SelectedDivision.ID).ToList() : COSContext.Current.WorkGroups.ToList();
                var sets = COSContext.Current.UserProperties.FirstOrDefault(a => a.ID_user == null && a.Key == "KPIReportFilterUsedWGS" + divisionParam + divisionParam);

                LocalUsed.Clear();

                List<int> ids = new List<int>();

                if (sets != null)
                {
                    foreach (var id in sets.Value.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(id))
                            ids.Add(int.Parse(id));
                    }
                }

                if (ids.Count > 0)
                {
                    foreach (var wg in wgs)
                    {
                        if (ids.Contains(wg.ID))
                            LocalUsed.Add(new UsedClass() { IsUsed = true, UsedObject = wg });
                        else
                            LocalUsed.Add(new UsedClass() { IsUsed = false, UsedObject = wg });
                    }
                }
                else
                {
                    foreach (var wg in wgs)
                    {
                        LocalUsed.Add(new UsedClass() { IsUsed = false, UsedObject = wg });
                    }
                }

                grdLocals.ItemsSource = LocalUsed;
                grdLocals.Rebind();
            }
            else if (WOG == 2)
            {
                objectColumn.Header = ResourceHelper.GetResource<string>("rep_FiltrWcSel");

                var wgs = Model.SelectedDivision != null ? COSContext.Current.WorkCenters.Where(a=>a.ID_Division == Model.SelectedDivision.ID).ToList() : COSContext.Current.WorkCenters.ToList();
                var sets = COSContext.Current.UserProperties.FirstOrDefault(a => a.ID_user == null && a.Key == "KPIReportFilterUsedWCS" + divisionParam);

                LocalUsed.Clear();

                List<int> ids = new List<int>();

                if (sets != null)
                {
                    foreach (var id in sets.Value.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(id))
                            ids.Add(int.Parse(id));
                    }
                }
                if (ids.Count > 0)
                {
                    foreach (var wc in wgs)
                    {
                        if (ids.Contains(wc.ID))
                            LocalUsed.Add(new UsedClass() { IsUsed = true, UsedObject = wc });
                        else
                            LocalUsed.Add(new UsedClass() { IsUsed = false, UsedObject = wc });
                    }
                }
                else
                {
                    foreach (var wc in wgs)
                    {
                        LocalUsed.Add(new UsedClass() { IsUsed = false, UsedObject = wc });
                    }
                }

                grdLocals.ItemsSource = LocalUsed;
                grdLocals.Rebind();
            }

        }

        KPIReportViewModel Model = null;

        private void btnUse_Click(object sender, RoutedEventArgs e)
        {
            string ids = "";
            if (WOG == 1)
            {
                if (Model.UsedWorkGroups == null)
                    Model.UsedWorkGroups = new List<WorkGroup>();
                Model.UsedWorkGroups.Clear();
                foreach (var itm in LocalUsed)
                {
                    if (itm.IsUsed)
                    {
                        Model.UsedWorkGroups.Add(itm.UsedObject as WorkGroup);
                        if (ids.Length > 0)
                            ids += ";";
                        ids += (itm.UsedObject as WorkGroup).ID.ToString();
                    }
                }

                var sets = COSContext.Current.UserProperties.FirstOrDefault(a => a.ID_user == null && a.Key == "KPIReportFilterUsedWGS" + divisionParam);

                if (sets == null)
                {
                    sets = new UserProperty();
                    sets.Key = "KPIReportFilterUsedWGS" + divisionParam;
                    sets.Value = ids;
                    COSContext.Current.UserProperties.AddObject(sets);
                }
                else
                {
                    sets.Value = ids;
                }
            }
            else if (WOG == 2)
            {
                if (Model.UsedWorkCenters == null)
                    Model.UsedWorkCenters = new List<WorkCenter>();
                Model.UsedWorkCenters.Clear();
                foreach (var itm in LocalUsed)
                {
                    if (itm.IsUsed)
                    {
                        if (ids.Length > 0)
                            ids += ";";
                        ids += (itm.UsedObject as WorkCenter).ID.ToString();
                        Model.UsedWorkCenters.Add(itm.UsedObject as WorkCenter);
                    }
                }

                var sets = COSContext.Current.UserProperties.FirstOrDefault(a => a.ID_user == null && a.Key == "KPIReportFilterUsedWCS" + divisionParam);

                if (sets == null)
                {
                    sets = new UserProperty();
                    sets.Key = "KPIReportFilterUsedWCS" + divisionParam;
                    sets.Value = ids;
                    COSContext.Current.UserProperties.AddObject(sets);
                }
                else
                {
                    sets.Value = ids;
                }
            }


            try
            {
                COSContext.Current.SaveChanges();
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
            }

            this.DialogResult = true;
            this.Close();
        }


        private List<UsedClass> _localUsed = new List<UsedClass>();
        public List<UsedClass> LocalUsed
        {
            set
            {
                _localUsed = value;
                OnPropertyChanged("LocalUsed");
            }
            get
            {
                return _localUsed;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var itm in LocalUsed)
                itm.IsUsed = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var itm in LocalUsed)
                itm.IsUsed = false;
        }

    }

    public class UsedClass : NotifyBase
    {
        public UsedClass() { }

        private bool _isUsed = false;
        public bool IsUsed
        {
            set
            {
                if (_isUsed != value)
                {
                    _isUsed = value;
                    OnPropertyChanged("IsUsed");
                }
            }
            get
            {
                return _isUsed;
            }
        }

        private object _usedObject = false;
        public object UsedObject
        {
            set
            {
                if (_usedObject != value)
                {
                    _usedObject = value;
                    OnPropertyChanged("UsedObject");
                }
            }
            get
            {
                return _usedObject;
            }
        }
    }
}
