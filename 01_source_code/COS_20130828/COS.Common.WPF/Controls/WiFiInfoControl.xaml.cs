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
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;
using COS.Application.Shared;
using System.Diagnostics;

namespace COS.Common.WPF.Controls
{
    /// <summary>
    /// Interaction logic for BatteryInfoControl.xaml
    /// </summary>
    public partial class WiFiInfoControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public WiFiInfoControl()
        {
            InitializeComponent();

            this.DataContext = this;


            myTimer.Tick += new EventHandler(myTimer_Tick);
            myTimer.Interval = new TimeSpan(0, 0, 3);
            myTimer.Start();
        }


        void myTimer_Tick(object sender, EventArgs e)
        {
            StateValue = RetrieveSignalString();

            if (!Debugger.IsAttached)
            {
                if (StateValue == disconnectedBrush)
                {
                    COSContext.Current.IsBusy = true;
                    COSContext.Current.BusyContent = "Není dostupné WiFi připojení, čekám na signál...";
                }
                else
                {
                    COSContext.Current.IsBusy = false;
                    COSContext.Current.BusyContent = "Čekejte prosím...";
                }
            }
        }

        private Brush RetrieveSignalString()
        {
            Brush result = disconnectedBrush;

            platformInvoke.wifi wifiAPI = new platformInvoke.wifi();
            double state = wifiAPI.EnumerateNICs();

            if (state == 1)
                result = connectedBrush;

            return result;

        }

        public DispatcherTimer myTimer = new DispatcherTimer();

        private Brush _stateValue;
        public Brush StateValue
        {
            set
            {
                if (_stateValue != value)
                {
                    _stateValue = value;
                    OnPropertyChanged("StateValue");
                    OnPropertyChanged("BatteryValueString");
                }
            }
            get
            {
                return _stateValue;
            }
        }

        Brush connectedBrush = Brushes.Green;
        Brush disconnectedBrush = Brushes.Red;

        public string StateValueString
        {
            get
            {
                return "Stav připojení: " + (StateValue == connectedBrush ? "připojeno" : "nepřipojeno");
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
    }
}

namespace platformInvoke
{
    class wifi
    {

        #region declarations

        private const int WLAN_API_VERSION_2_0 = 2; //Windows Vista WiFi API Version
        private const int WLAN_API_XP_VERSION = 1;
        private const int ERROR_SUCCESS = 0;

        /// <summary>
        /// Opens a connection to the server
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanOpenHandle(UInt32 dwClientVersion, IntPtr pReserved, out UInt32 pdwNegotiatedVersion, ref IntPtr phClientHandle);

        /// <summary>
        /// Closes a connection to the server
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

        /// <summary>
        /// Enumerates all wireless interfaces in the laptop
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanEnumInterfaces(IntPtr hClientHandle, IntPtr pReserved, ref IntPtr ppInterfaceList);

        /// <summary>
        /// Frees memory returned by native WiFi functions
        /// </summary>
        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern void WlanFreeMemory(IntPtr pmemory);

        /// <summary>
        /// Interface state enums
        /// </summary>
        public enum WLAN_INTERFACE_STATE : int
        {
            wlan_interface_state_not_ready = 0,
            wlan_interface_state_connected,
            wlan_interface_state_ad_hoc_network_formed,
            wlan_interface_state_disconnecting,
            wlan_interface_state_disconnected,
            wlan_interface_state_associating,
            wlan_interface_state_discovering,
            wlan_interface_state_authenticating
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_INTERFACE_INFO
        {
            /// GUID->_GUID
            public Guid InterfaceGuid;

            /// WCHAR[256]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strInterfaceDescription;

            /// WLAN_INTERFACE_STATE->_WLAN_INTERFACE_STATE
            public WLAN_INTERFACE_STATE isState;
        }
        /// <summary>
        /// This structure contains an array of NIC information
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WLAN_INTERFACE_INFO_LIST
        {
            public Int32 dwNumberofItems;
            public Int32 dwIndex;
            public WLAN_INTERFACE_INFO[] InterfaceInfo;

            public WLAN_INTERFACE_INFO_LIST(IntPtr pList)
            {
                // The first 4 bytes are the number of WLAN_INTERFACE_INFO structures.
                dwNumberofItems = Marshal.ReadInt32(pList, 0);

                // The next 4 bytes are the index of the current item in the unmanaged API.
                dwIndex = Marshal.ReadInt32(pList, 4);

                // Construct the array of WLAN_INTERFACE_INFO structures.
                InterfaceInfo = new WLAN_INTERFACE_INFO[dwNumberofItems];

                for (int i = 0; i < dwNumberofItems; i++)
                {
                    // The offset of the array of structures is 8 bytes past the beginning. Then, take the index and multiply it by the number of bytes in the structure.
                    // the length of the WLAN_INTERFACE_INFO structure is 532 bytes - this was determined by doing a sizeof(WLAN_INTERFACE_INFO) in an unmanaged C++ app.
                    IntPtr pItemList = new IntPtr(pList.ToInt32() + (i * 532) + 8);

                    // Construct the WLAN_INTERFACE_INFO structure, marshal the unmanaged structure into it, then copy it to the array of structures.
                    WLAN_INTERFACE_INFO wii = new WLAN_INTERFACE_INFO();
                    wii = (WLAN_INTERFACE_INFO)Marshal.PtrToStructure(pItemList, typeof(WLAN_INTERFACE_INFO));
                    InterfaceInfo[i] = wii;
                }
            }
        }

        #endregion

        #region Private Functions
        /// <summary>
        ///get NIC state 
        /// </summary>
        private string getStateDescription(WLAN_INTERFACE_STATE state)
        {
            string stateDescription = string.Empty;
            switch (state)
            {
                case WLAN_INTERFACE_STATE.wlan_interface_state_not_ready:
                    stateDescription = "not ready to operate";
                    break;
                case WLAN_INTERFACE_STATE.wlan_interface_state_connected:
                    stateDescription = "connected";
                    break;
                case WLAN_INTERFACE_STATE.wlan_interface_state_ad_hoc_network_formed:
                    stateDescription = "first node in an adhoc network";
                    break;
                case WLAN_INTERFACE_STATE.wlan_interface_state_disconnecting:
                    stateDescription = "disconnecting";
                    break;
                case WLAN_INTERFACE_STATE.wlan_interface_state_disconnected:
                    stateDescription = "disconnected";
                    break;
                case WLAN_INTERFACE_STATE.wlan_interface_state_associating:
                    stateDescription = "associating";
                    break;
                case WLAN_INTERFACE_STATE.wlan_interface_state_discovering:
                    stateDescription = "discovering";
                    break;
                case WLAN_INTERFACE_STATE.wlan_interface_state_authenticating:
                    stateDescription = "authenticating";
                    break;
            }

            return stateDescription;
        }
        #endregion
        #region Public Functions

        /// <summary>
        /// enumerate wireless network adapters using wifi api
        /// </summary>
        public double EnumerateNICs()
        {
            double result = 0;
            uint serviceVersion = 0;
            IntPtr handle = IntPtr.Zero;
            if (WlanOpenHandle(2, IntPtr.Zero, out serviceVersion, ref handle) == ERROR_SUCCESS)
            {
                IntPtr ppInterfaceList = IntPtr.Zero;
                WLAN_INTERFACE_INFO_LIST interfaceList;

                if (WlanEnumInterfaces(handle, IntPtr.Zero, ref ppInterfaceList) == ERROR_SUCCESS)
                {
                    //Tranfer all values from IntPtr to WLAN_INTERFACE_INFO_LIST structure 
                    interfaceList = new WLAN_INTERFACE_INFO_LIST(ppInterfaceList);

                    Console.WriteLine("Enumerating Wireless Network Adapters...");
                    for (int i = 0; i < interfaceList.dwNumberofItems; i++)
                    {
                        result = interfaceList.InterfaceInfo[i].isState == WLAN_INTERFACE_STATE.wlan_interface_state_connected ? 1 : 0;
                        //  Console.WriteLine("{0}-->{1}", interfaceList.InterfaceInfo[i].strInterfaceDescription, getStateDescription(interfaceList.InterfaceInfo[i].isState));
                    }
                    //frees memory
                    if (ppInterfaceList != IntPtr.Zero)
                        WlanFreeMemory(ppInterfaceList);
                }
                //close handle
                WlanCloseHandle(handle, IntPtr.Zero);
            }

            return result;
        }
        #endregion
    }
}
