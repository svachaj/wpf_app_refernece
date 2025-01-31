using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.ComponentModel;
using System.Timers;
using System.IO;
using System.Globalization;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Configuration;
using COS.Resources;


namespace COS.Application.Shared
{
    public class COSContext : COSDataModelContainer, INotifyPropertyChanged
    {
        public COSContext()
            : base()
        {
            InitDateTimer();
        }

        public COSContext(string connectionString)
            : base(connectionString)
        {

            this.ContextOptions.ProxyCreationEnabled = false;

            InitDateTimer();
            InitPingTimer();

        }


        public override int SaveChanges(System.Data.Objects.SaveOptions options)
        {
            int result = 0;

            try
            {
                result = base.SaveChanges(options);
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFile);
                COSContext.Current.RejectChanges();

                MessageBox.Show(exc.InnerException != null ? exc.InnerException.Message : exc.Message);

                
            }

          

            return result;
        }

        public object RadMainWindow { set; get; }

        public object VisualizationEngine { set; get; }

        Timer dateTimer;

        Timer pingTimer;

        private void InitDateTimer()
        {
            dateTimer = new Timer();
            dateTimer.Enabled = false;
            dateTimer.Interval = 1000;
            dateTimer.Elapsed += new ElapsedEventHandler(dateTimer_Elapsed);

            DateTimeServer = this.v_ServerDateTime.FirstOrDefault().ServerDateTime;

            dateTimer.Start();
        }

        private void InitPingTimer()
        {
            pingTimer = new Timer();
            pingTimer.Enabled = false;
            pingTimer.Interval = 1000;
            pingTimer.Elapsed += new ElapsedEventHandler(pingTimer_Elapsed);

            pingTimer.Start();
        }

        void pingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            long rtt = 9999;

            IsServerConnected = COS.Common.Connection.IsConnectedToServer(COS.Common.Connection.DBServerNameOrIP, out rtt);

            RoundtripTime = rtt;
        }

        void dateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTimeServer = DateTimeServer.AddSeconds(1);

            CultureInfo myCI = new CultureInfo("cs-CZ");
            Calendar myCal = myCI.Calendar;

            Week = myCal.GetWeekOfYear(DateTimeServer, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private DateTime _dateTimeServer;
        public DateTime DateTimeServer
        {
            set
            {
                _dateTimeServer = value;
                OnPropertyChanged("DateTimeServer");
            }
            get
            {
                return _dateTimeServer;
            }
        }



        private bool _isBusy = false;
        public bool IsBusy
        {
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
            get
            {
                return _isBusy;
            }
        }

        private string _busyContent = "Čekejte prosím...";
        public string BusyContent
        {
            set
            {
                _busyContent = value;
                OnPropertyChanged("BusyContent");
            }
            get
            {
                return _busyContent;
            }
        }

        public string AppVersion
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        public Dictionary<string, string> ExceptionInfo
        {
            get
            {
                Dictionary<string, string> info = new Dictionary<string, string>();
                try
                {
                    info.Add("User", COSContext.Current.UserFullName);
                }
                catch
                { }
                try
                {
                    info.Add("App version", AppVersion);

                }
                catch
                { }

                return info;
            }
        }

        private int _Week;
        public int Week
        {
            set
            {
                _Week = value;
                OnPropertyChanged("Week");
                OnPropertyChanged("WeekString");
            }
            get
            {
                return _Week;
            }
        }

        private HourlyProduction _hourlyProductionToNavigate;
        public HourlyProduction HourlyProductionToNavigate
        {
            set
            {
                _hourlyProductionToNavigate = value;
                OnPropertyChanged("HourlyProductionToNavigate");
            }
            get
            {
                return _hourlyProductionToNavigate;
            }
        }

        private int _foreignExportToNavigateID;
        public int ForeignExportToNavigateID
        {
            set
            {
                _foreignExportToNavigateID = value;
                OnPropertyChanged("ForeignExportToNavigateID");
            }
            get
            {
                return _foreignExportToNavigateID;
            }
        }


        private int _domesticExportToNavigateID;
        public int DomesticExportToNavigateID
        {
            set
            {
                _domesticExportToNavigateID = value;
                OnPropertyChanged("DomesticExportToNavigateID");
            }
            get
            {
                return _domesticExportToNavigateID;
            }
        }



        public string UserFullName
        {
            get
            {
                string result = null;
                if (CurrentUser != null)
                {
                    result = CurrentUser.Name + " " + CurrentUser.Surname;
                }
                return result;
            }
        }

        public string WeekString
        {
            get
            {
                return "W-" + Week.ToString();
            }
        }

        private List<HourlyProduction> _selectedHourlyProductions = null;
        public List<HourlyProduction> SelectedHourlyProductions
        {
            set
            {
                _selectedHourlyProductions = value;
                OnPropertyChanged("SelectedHourlyProductions");
            }
            get
            {
                return _selectedHourlyProductions;
            }
        }

        #region security and args properties

        private string _securityKey = null;
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public string SecurityKey
        {
            set
            {
                if (_securityKey != value)
                {
                    _securityKey = value;
                    OnPropertyChanged("SecurityKey");
                }
            }
            get
            {
                return _securityKey;
            }
        }

        private bool _errorHandled = false;
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public bool ErrorHandled
        {
            set
            {
                if (_errorHandled != value)
                {
                    _errorHandled = value;
                    OnPropertyChanged("ErrorHandled");
                }
            }
            get
            {
                return _errorHandled;
            }
        }

        private int _groupID = 1;
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public int GroupID
        {
            set
            {
                if (_groupID != value)
                {
                    _groupID = value;
                    OnPropertyChanged("GroupID");
                }
            }
            get
            {
                return _groupID;
            }
        }

        private string _language = "cs-CZ";
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public string Language
        {
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged("Language");
                }
            }
            get
            {
                return _language;
            }
        }


        public Visibility EnLngVisibility
        {
            get
            {
                if (Language == "en-US")
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility CsLngVisibility
        {
            get
            {
                if (Language == "cs-CZ")
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        private User _currentUser = null;
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public User CurrentUser
        {
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged("CurrentUser");
                }
            }
            get
            {
                return _currentUser;
            }
        }


        private bool _isServerConnected = false;
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public bool IsServerConnected
        {
            set
            {
                if (_isServerConnected != value)
                {
                    _isServerConnected = value;
                    OnPropertyChanged("IsServerConnected");
                }
            }
            get
            {
                return _isServerConnected;
            }
        }

        private long _roundtripTime = 9999;
        /// <summary>
        /// Security key for authenticate on start
        /// </summary>
        public long RoundtripTime
        {
            set
            {
                if (_roundtripTime != value)
                {
                    _roundtripTime = value;
                    OnPropertyChanged("RoundtripTime");
                }
            }
            get
            {
                return _roundtripTime;
            }
        }



        #endregion


        #region Model helper functions

        /// <summary>
        /// Reject all changes to this time
        /// </summary>
        public void RejectChanges()
        {
            foreach (var entry in this.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Modified))
            {
                if (entry.State == System.Data.EntityState.Modified)
                {
                    for (int i = 0; i < entry.OriginalValues.FieldCount; i++)
                    {
                        if (entry.OriginalValues.GetValue(i) != entry.CurrentValues.GetValue(i))
                            entry.CurrentValues.SetValue(i, entry.OriginalValues.GetValue(i));
                    }
                }
                try
                {
                    entry.ChangeState(System.Data.EntityState.Unchanged);
                }
                catch { }
            }

            foreach (var entry in this.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added))
            {
                try
                {
                    entry.ChangeState(System.Data.EntityState.Detached);
                }
                catch { }
            }

            foreach (var entry in this.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Deleted))
            {
                try
                {
                    entry.ChangeState(System.Data.EntityState.Detached);
                }
                catch { }
            }
        }

        #endregion

        public void ImportStandartsFromCSV(string fileName, char separator)
        {
            var lines = File.ReadAllLines(fileName, Encoding.UTF8);

            Standard standard = null;
            foreach (string line in lines)
            {
                var values = line.Split(new char[] { separator }, StringSplitOptions.None);

                standard = new Standard();
                int col = 0;
                foreach (var val in values)
                {
                    switch (col)
                    {
                        case 0:
                            standard.ItemNumber = val.Trim();
                            break;

                    }
                }
                Standards.AddObject(standard);
            }

            try
            {
                SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception exc)
            {

            }
        }


        public bool CanAccessProcess(string process, out  SysLock resultLock)
        {
            bool result = true;

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysLocks);

            SysLock mylock = COSContext.Current.SysLocks.FirstOrDefault(a => a.Process == process);

            if (mylock == null)
            {
                mylock = COSContext.Current.SysLocks.CreateObject();
                mylock.ID_User = CurrentUser.ID;
                mylock.Process = process;
                mylock.StartTime = COSContext.Current.DateTimeServer;
                mylock.State = true;

                COSContext.Current.SysLocks.AddObject(mylock);
            }
            else
            {
                if (mylock.State == true)
                {
                    if (mylock.ID_User == this.CurrentUser.ID)
                    {
                        result = true;
                        mylock.State = false;
                    }
                    else
                        result = false;
                }
                else
                {
                    mylock.ID_User = CurrentUser.ID;
                    mylock.Process = process;
                    mylock.StartTime = COSContext.Current.DateTimeServer;
                    mylock.State = true;
                }
            }

            COSContext.Current.SaveChanges();

            resultLock = mylock;

            return result;
        }

        public void ReleaseAccessProcess(string process)
        {

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.SysLocks);

            SysLock mylock = COSContext.Current.SysLocks.FirstOrDefault(a => a.Process == process);

            if (mylock == null)
            {
                mylock = COSContext.Current.SysLocks.CreateObject();
                mylock.ID_User = CurrentUser.ID;
                mylock.Process = process;
                mylock.StartTime = COSContext.Current.DateTimeServer;
                mylock.State = false;

                COSContext.Current.SysLocks.AddObject(mylock);
            }
            else
            {

                mylock.ID_User = CurrentUser.ID;
                mylock.Process = process;
                mylock.StartTime = COSContext.Current.DateTimeServer;
                mylock.State = false;

            }

            COSContext.Current.SaveChanges();

        }

        public const string HPRecalculation = "HPRecalculation";

        public static COSContext Current { private set; get; }

        public List<ViewEmployee> EmployeesList
        {
            get
            {
                return ViewEmployees.ToList();
            }
        }


        public List<ForeignExport> ForeignExportList
        {
            get
            {
                return this.ForeignExports.ToList();
            }
        }

        public List<int> AvailableDivisions()
        {
            List<int> result = new List<int>();

            var divisions = COSContext.Current.Divisions.ToList();

            if (HasRightForOperation("StandardsVI", "View"))
                result.Add(divisions.FirstOrDefault(a => a.Value == "VI").ID);

            if (HasRightForOperation("StandardsVA", "View"))
                result.Add(divisions.FirstOrDefault(a => a.Value == "VA").ID);

            return result;
        }

        public List<Standard> AvailableStandards()
        {
            List<Standard> result = null;

            var ids = COSContext.Current.AvailableDivisions();

            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Standards);

            var stands = COSContext.Current.Standards.ToList();

            var lc = stands.Where(a => ids.Contains(a.WorkGroup.Division.ID));

            result = lc.ToList();

            return result;
        }

        public bool HasRightForOperation(string submodule, string actionCode)
        {
            bool result = false;

            var groupRights = COSContext.Current.SysEGPs.Where(a => a.ID_sys_group == COSContext.Current.GroupID).ToList();

            var cls = COSContext.Current.SysClasses.FirstOrDefault(a => a.Name == submodule);
            if (cls != null)
            {
                var egp = groupRights.FirstOrDefault(a => a.Action.Code == actionCode && a.ID_sys_class == cls.ID);

                if (egp != null && egp.Granted.HasValue)
                {
                    result = egp.Granted.Value;
                }
                else
                    result = false;
            }


            return result;
        }

        private List<WorkGroup> _localWorkGroups = null;
        public List<WorkGroup> LocalWorkGroups
        {
            get
            {
                if (_localWorkGroups == null)
                    _localWorkGroups = WorkGroups.ToList();

                return _localWorkGroups;
            }
        }

        private List<WorkCenter> _LocalWorkCenters = null;
        public List<WorkCenter> LocalWorkCenters
        {
            get
            {
                if (_LocalWorkCenters == null)
                    _LocalWorkCenters = WorkCenters.ToList();

                return _LocalWorkCenters;
            }
            set
            {
                _LocalWorkCenters = value;
                OnPropertyChanged("LocalWorkCenters");
            }
        }

        public string OrdersFilesPath
        {
            get
            {
                return ConfigurationManager.AppSettings["OrderFilesPath"];
            }
        }


        #region TABLET




        public Action CurrentAction { set; get; }
        public int actionRiaseCount = 0;


        public void RaiseAction(Action action)
        {
            CurrentAction = action;

            if (!CurrentActionWorker.IsBusy)
                CurrentActionWorker.RunWorkerAsync();
        }

        private void RaiseCurrentAction()
        {
            try
            {
                if (CurrentAction != null)
                    CurrentAction();
            }
            catch (Exception exc)
            {
                actionRiaseCount++;
                if (actionRiaseCount > 10)
                {
                    actionRiaseCount = 0;
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                }
                else
                    RaiseCurrentAction();
            }
        }

        public BackgroundWorker CurrentActionWorker = null;

        public void Navigate(string viewCode)
        {
            CurrentTabletViewInfo = null;
            TabletViewsHistory.Add(viewCode);
            CurrentTabletViewCode = viewCode;
        }

        public void Navigate(string viewCode, object data)
        {
            CurrentTabletViewInfo = new TabletViewModelBase();
            CurrentTabletViewInfo.Data = data;
            TabletViewsHistory.Add(viewCode);
            CurrentTabletViewCode = viewCode;
        }

        public void Navigate(string viewCode, object data, Action<object> okAction, Action cancelAction)
        {
            CurrentTabletViewInfo = new TabletViewModelBase();
            CurrentTabletViewInfo.Data = data;
            CurrentTabletViewInfo.OKAction = okAction;
            CurrentTabletViewInfo.CancelAction = cancelAction;
            CurrentTabletViewInfo.IsModal = true;
            TabletViewsHistory.Add(viewCode);
            CurrentTabletViewCode = viewCode;
        }

        public void NavigateBack()
        {
            if (TabletViewsHistory.Count > 1)
            {
                CurrentTabletViewInfo = null;
                TabletViewsHistory.RemoveAt(TabletViewsHistory.Count - 1);
                CurrentTabletViewCode = TabletViewsHistory[TabletViewsHistory.Count - 1];

            }
            else
            {
                CurrentTabletViewCode = TabletViews.HomeView.ToString();
                CurrentTabletViewInfo = new TabletViewModelBase();
            }

        }


        private string _currentTabletViewCode;
        public string CurrentTabletViewCode
        {
            set
            {
                if (_currentTabletViewCode != value)
                {
                    _currentTabletViewCode = value;
                    OnPropertyChanged("CurrentTabletViewCode");
                }
            }
            get
            {
                return _currentTabletViewCode;
            }
        }

        private List<SysKeyboard> _tabletKeyboards = null;
        public List<SysKeyboard> TabletKeyboards
        {
            set
            {
                _tabletKeyboards = value;
            }
            get
            {
                if (_tabletKeyboards == null)
                    LoadKeyboards();
                return _tabletKeyboards;
            }
        }

        public void LoadKeyboards()
        {
            TabletKeyboards = COSContext.Current.SysKeyboards.ToList();
        }

        public void InitializeForTablet()
        {
            LoadKeyboards();

            CurrentActionWorker = new BackgroundWorker();
            CurrentActionWorker.DoWork += new DoWorkEventHandler(CurrentActionWorker_DoWork);
            CurrentActionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CurrentActionWorker_RunWorkerCompleted);
        }

        void CurrentActionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
        }

        void CurrentActionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;

            RaiseCurrentAction();
        }


        public Action<string> KeyboardResultAction { set; get; }
        public Action<DateTime> CalendarResultAction { set; get; }

        public string KeyboardTextFrom { set; get; }
        public string CalendarTextFrom { set; get; }

        public bool KeyboardIsOnlyNumeric { set; get; }
        public bool KeyboardPasswordMode { set; get; }

        private TabletViewModelBase _currentTabletViewInfo = null;
        public TabletViewModelBase CurrentTabletViewInfo
        {
            set
            {
                if (_currentTabletViewInfo != value)
                {
                    _currentTabletViewInfo = value;
                    OnPropertyChanged("CurrentTabletViewInfo");
                }
            }
            get
            {
                return _currentTabletViewInfo;
            }
        }

        public List<string> TabletViewsHistory { set; get; }

        #endregion


        public static void Init(string connectionString)
        {
            string decryptString = Crypto.DecryptString(connectionString, Security.SecurityHelper.SecurityKey);

            Current = new COSContext(decryptString);

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

        private string _tabletMessage = "";

        public string TabletMessage
        {
            set
            {
                _tabletMessage = value;
                OnPropertyChanged("TabletMessage");
            }
            get
            {
                return _tabletMessage;
            }
        }

        public void ShowTabletMessage(string err)
        {
            TabletMessage = err;
        }

        public void ShowWebCamControl(List<BitmapSource> images)
        {
            TabletImages = images;
            OnPropertyChanged("WebCamControl");
        }

        public List<BitmapSource> TabletImages { set; get; }
    }

    public static class TabletViews
    {
        public const string HomeView = "HomeView";
        public const string TpmPlansView = "TpmPlansView";
        public const string TpmPlanDetailView = "TpmPlanDetailView";
        public const string TpmPlanActionDetailView = "TpmPlanActionDetailView";
    }

    public static class SpecialChars
    {
        public const string Rect = "■";
    }
}
