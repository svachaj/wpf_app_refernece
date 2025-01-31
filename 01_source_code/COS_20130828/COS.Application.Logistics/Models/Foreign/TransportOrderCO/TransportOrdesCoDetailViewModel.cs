using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using COS.Resources;
using System.IO;
using System.Windows;
using System.Globalization;
using System.Transactions;
using System.Data.Objects;
using System.ComponentModel;
using System.Net.Mail;

namespace COS.Application.Logistics.Models
{
    public partial class TransportOrdesCoDetailViewModel : ValidationViewModelBase
    {
        public TransportOrdesCoDetailViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
            EditingMode = EditMode.Edit;

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            OnPropertyChanged("SelectedItem");
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;
            InitTransports();
        }

        BackgroundWorker worker = null;
        private COSContext dataContext = null;

        public ICommand UpdateToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }

        //public ICommand ExportToExcelToolBarCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(param => this.ExportToExcel());
        //    }
        //}

        public ICommand SendEmailToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.ExportToExcelAndSendEmail());
            }
        }


        private TransportOrderForeignExportClass _SelectedItem = null;
        public TransportOrderForeignExportClass SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;

                SelectedDate1 = _SelectedItem.DateFrom;
                SelectedDate2 = _SelectedItem.DateFrom.AddDays(1);
                SelectedDate3 = _SelectedItem.DateFrom.AddDays(2);
                SelectedDate4 = _SelectedItem.DateFrom.AddDays(3);
                SelectedDate5 = _SelectedItem.DateFrom.AddDays(4);

                if (!worker.IsBusy)
                    worker.RunWorkerAsync();

                //OnPropertyChanged("SelectedItem");
            }
        }

        private int? _selectedWeek = null;
        public int? SelectedWeek
        {
            get
            {
                return _selectedWeek;
            }
            set
            {
                _selectedWeek = value;
                OnPropertyChanged("SelectedWeek");
            }
        }

        private int? _selectedYear = null;
        public int? SelectedYear
        {
            get
            {
                return _selectedYear;
            }
            set
            {
                _selectedYear = value;
                OnPropertyChanged("SelectedYear");
            }
        }

        private DateTime _SelectedDate2;
        public DateTime SelectedDate2
        {
            get
            {
                return _SelectedDate2;
            }
            set
            {
                _SelectedDate2 = value;
                OnPropertyChanged("SelectedDate2");
            }
        }

        private DateTime _SelectedDate1;
        public DateTime SelectedDate1
        {
            get
            {
                return _SelectedDate1;
            }
            set
            {
                _SelectedDate1 = value;
                OnPropertyChanged("SelectedDate1");
            }
        }

        private DateTime _SelectedDate3;
        public DateTime SelectedDate3
        {
            get
            {
                return _SelectedDate3;
            }
            set
            {
                _SelectedDate3 = value;
                OnPropertyChanged("SelectedDate3");
            }
        }

        private DateTime _SelectedDate4;
        public DateTime SelectedDate4
        {
            get
            {
                return _SelectedDate4;
            }
            set
            {
                _SelectedDate4 = value;
                OnPropertyChanged("SelectedDate4");
            }
        }

        private DateTime _SelectedDate5;
        public DateTime SelectedDate5
        {
            get
            {
                return _SelectedDate5;
            }
            set
            {
                _SelectedDate5 = value;
                OnPropertyChanged("SelectedDate5");
            }
        }

        public ObservableCollection<ForeignExportTransportOrderCo> _localTransports1 = new ObservableCollection<ForeignExportTransportOrderCo>();

        public ObservableCollection<ForeignExportTransportOrderCo> LocalTransports1
        {
            get
            {
                return _localTransports1;
            }
        }

        public ObservableCollection<ForeignExportTransportOrderCo> _localTransports2 = new ObservableCollection<ForeignExportTransportOrderCo>();

        public ObservableCollection<ForeignExportTransportOrderCo> LocalTransports2
        {
            get
            {
                return _localTransports2;
            }
        }

        public ObservableCollection<ForeignExportTransportOrderCo> _localTransports3 = new ObservableCollection<ForeignExportTransportOrderCo>();

        public ObservableCollection<ForeignExportTransportOrderCo> LocalTransports3
        {
            get
            {
                return _localTransports3;
            }
        }

        public ObservableCollection<ForeignExportTransportOrderCo> _localTransports4 = new ObservableCollection<ForeignExportTransportOrderCo>();

        public ObservableCollection<ForeignExportTransportOrderCo> LocalTransports4
        {
            get
            {
                return _localTransports4;
            }
        }

        public ObservableCollection<ForeignExportTransportOrderCo> _localTransports5 = new ObservableCollection<ForeignExportTransportOrderCo>();

        public ObservableCollection<ForeignExportTransportOrderCo> LocalTransports5
        {
            get
            {
                return _localTransports5;
            }
        }

        public ObservableCollection<TransportAdviClass> _localTransportsTemps = new ObservableCollection<TransportAdviClass>();

        public ObservableCollection<TransportAdviClass> LocalTransportsTemps
        {
            get
            {
                return _localTransportsTemps;
            }
        }

        public void InitTransports()
        {
            if (_SelectedItem != null)
            {
                ForeignExportTransportOrderCo transport = null;
                TransportAdviClass adviClass = null;
                ForeignExportAdvice selectedAdvice = null;

                LocalTransportsTemps.Clear();
                LocalTransports1.Clear();
                LocalTransports2.Clear();
                LocalTransports3.Clear();
                LocalTransports4.Clear();
                LocalTransports5.Clear();

                //var tempTransports = dataContext.ForeignExportTransportOrderCoes.Where(a=>a.for

                ForeignExport export = null;
                foreach (var itm in _SelectedItem.ForeigExports.GroupBy(a => a.Destination))
                {
                    var dest = itm.FirstOrDefault().Destination;
                    selectedAdvice = dest.Advices.FirstOrDefault(a => a.IsDefault);

                    export = itm.FirstOrDefault(a => a.PlannedDate == SelectedDate1);
                    if (export != null)
                    {
                        transport = export.TransportOrders.FirstOrDefault(a => a.ID_foreignExport == export.ID);

                        if (transport != null)
                        {
                            if (transport.OrderDate == export.PlannedDate)
                            {
                                if (transport.Advice != null)
                                    selectedAdvice = transport.Advice;

                                LocalTransports1.Add(transport);
                            }
                            else
                            {
                                if (export.IsOrderLocked)
                                {
                                    LocalTransports1.Add(null);

                                    SetTransportToList(transport, LocalTransports1.Count);
                                }
                                else
                                {
                                    //došlo k přesunutí data už objednaného transportu
                                    if (transport.Advice != null)
                                        selectedAdvice = transport.Advice;

                                    transport.OrderDate = export.PlannedDate;
                                    LocalTransports1.Add(transport);
                                }
                            }
                        }
                        else
                        {
                            transport = new ForeignExportTransportOrderCo();
                            transport.ForeignExport = export;
                            transport.Volume = export.Unit.UnitName + " - (" + export.VolumeCbm.ToString() + " CBM)";
                            transport.OrderDate = SelectedDate1;
                            LocalTransports1.Add(transport);
                        }
                    }
                    else
                    {
                        transport = null;
                        LocalTransports1.Add(transport);
                    }

                    export = itm.FirstOrDefault(a => a.PlannedDate == SelectedDate2);
                    if (export != null)
                    {
                        //transport = dataContext.ForeignExportTransportOrderCoes.FirstOrDefault(a => a.ID_foreignExport == export.ID);
                        transport = export.TransportOrders.FirstOrDefault(a => a.ID_foreignExport == export.ID);

                        if (transport != null)
                        {
                            if (transport.OrderDate == export.PlannedDate)
                            {
                                if (transport.Advice != null)
                                    selectedAdvice = transport.Advice;

                                LocalTransports2.Add(transport);
                            }
                            else
                            {
                                if (export.IsOrderLocked)
                                {
                                    LocalTransports2.Add(null);

                                    SetTransportToList(transport, LocalTransports2.Count);
                                }
                                else
                                {
                                    //došlo k přesunutí data už objednaného transportu
                                    if (transport.Advice != null)
                                        selectedAdvice = transport.Advice;

                                    transport.OrderDate = export.PlannedDate;
                                    LocalTransports2.Add(transport);
                                }
                            }
                        }
                        else
                        {
                            transport = new ForeignExportTransportOrderCo();
                            transport.ForeignExport = export;
                            transport.Volume = export.Unit.UnitName + " - (" + export.VolumeCbm.ToString() + " CBM)";
                            transport.OrderDate = SelectedDate2;
                            LocalTransports2.Add(transport);
                        }
                    }
                    else
                    {
                        if (LocalTransports2.Count < LocalTransports1.Count)
                        {
                            transport = null;
                            LocalTransports2.Add(transport);
                        }
                    }

                    export = itm.FirstOrDefault(a => a.PlannedDate == SelectedDate3);
                    if (export != null)
                    {
                        transport = export.TransportOrders.FirstOrDefault(a => a.ID_foreignExport == export.ID);

                        if (transport != null)
                        {
                            if (transport.OrderDate == export.PlannedDate)
                            {
                                if (transport.Advice != null)
                                    selectedAdvice = transport.Advice;

                                LocalTransports3.Add(transport);
                            }
                            else
                            {
                                if (export.IsOrderLocked)
                                {
                                    LocalTransports3.Add(null);

                                    SetTransportToList(transport, LocalTransports3.Count);
                                }
                                else
                                {
                                    //došlo k přesunutí data už objednaného transportu
                                    if (transport.Advice != null)
                                        selectedAdvice = transport.Advice;

                                    transport.OrderDate = export.PlannedDate;
                                    LocalTransports3.Add(transport);
                                }
                            }
                        }
                        else
                        {
                            transport = new ForeignExportTransportOrderCo();
                            transport.ForeignExport = export;
                            transport.Volume = export.Unit.UnitName + " - (" + export.VolumeCbm.ToString() + " CBM)";
                            transport.OrderDate = SelectedDate3;
                            LocalTransports3.Add(transport);
                        }
                    }
                    else
                    {
                        if (LocalTransports3.Count < LocalTransports2.Count)
                        {
                            transport = null;
                            LocalTransports3.Add(transport);
                        }
                    }

                    export = itm.FirstOrDefault(a => a.PlannedDate == SelectedDate4);
                    if (export != null)
                    {
                        transport = export.TransportOrders.FirstOrDefault(a => a.ID_foreignExport == export.ID);

                        if (transport != null)
                        {
                            if (transport.OrderDate == export.PlannedDate)
                            {
                                if (transport.Advice != null)
                                    selectedAdvice = transport.Advice;

                                LocalTransports4.Add(transport);
                            }
                            else
                            {
                                if (export.IsOrderLocked)
                                {
                                    LocalTransports4.Add(null);

                                    SetTransportToList(transport, LocalTransports4.Count);
                                }
                                else
                                {
                                    //došlo k přesunutí data už objednaného transportu
                                    if (transport.Advice != null)
                                        selectedAdvice = transport.Advice;

                                    transport.OrderDate = export.PlannedDate;
                                    LocalTransports4.Add(transport);
                                }
                            }
                        }
                        else
                        {
                            transport = new ForeignExportTransportOrderCo();
                            transport.ForeignExport = export;
                            transport.Volume = export.Unit.UnitName + " - (" + export.VolumeCbm.ToString() + " CBM)";
                            transport.OrderDate = SelectedDate4;
                            LocalTransports4.Add(transport);
                        }
                    }
                    else
                    {
                        if (LocalTransports4.Count < LocalTransports3.Count)
                        {
                            transport = null;
                            LocalTransports4.Add(transport);
                        }
                    }

                    export = itm.FirstOrDefault(a => a.PlannedDate == SelectedDate5);
                    if (export != null)
                    {
                        transport = export.TransportOrders.FirstOrDefault(a => a.ID_foreignExport == export.ID);

                        if (transport != null)
                        {
                            if (transport.OrderDate == export.PlannedDate)
                            {
                                if (transport.Advice != null)
                                    selectedAdvice = transport.Advice;

                                LocalTransports5.Add(transport);
                            }
                            else
                            {
                                if (export.IsOrderLocked)
                                {
                                    LocalTransports5.Add(null);

                                    SetTransportToList(transport, LocalTransports5.Count);
                                }
                                else
                                {
                                    //došlo k přesunutí data už objednaného transportu
                                    if (transport.Advice != null)
                                        selectedAdvice = transport.Advice;

                                    transport.OrderDate = export.PlannedDate;
                                    LocalTransports5.Add(transport);
                                }
                            }
                        }
                        else
                        {
                            transport = new ForeignExportTransportOrderCo();
                            transport.ForeignExport = export;
                            transport.Volume = export.Unit.UnitName + " - (" + export.VolumeCbm.ToString() + " CBM)";
                            transport.OrderDate = SelectedDate5;
                            LocalTransports5.Add(transport);
                        }
                    }
                    else
                    {
                        if (LocalTransports5.Count < LocalTransports4.Count)
                        {
                            transport = null;
                            LocalTransports5.Add(transport);
                        }
                    }

                    if (LocalTransports1.LastOrDefault() != null)
                        LocalTransports1.LastOrDefault().Advice = selectedAdvice;
                    if (LocalTransports2.LastOrDefault() != null)
                        LocalTransports2.LastOrDefault().Advice = selectedAdvice;
                    if (LocalTransports3.LastOrDefault() != null)
                        LocalTransports3.LastOrDefault().Advice = selectedAdvice;
                    if (LocalTransports4.LastOrDefault() != null)
                        LocalTransports4.LastOrDefault().Advice = selectedAdvice;
                    if (LocalTransports5.LastOrDefault() != null)
                        LocalTransports5.LastOrDefault().Advice = selectedAdvice;


                    adviClass = new TransportAdviClass(itm.FirstOrDefault().Destination, selectedAdvice);
                    LocalTransportsTemps.Add(adviClass);
                }
            }
        }

        private void SetTransportToList(ForeignExportTransportOrderCo transport, int p)
        {
            if (transport.OrderDate == SelectedDate1)
            {
                if (LocalTransports1.Count >= p)
                {
                    LocalTransports1[p - 1] = transport;
                }
                else
                {
                    LocalTransports1.Add(transport);
                }
            }
            else if (transport.OrderDate == SelectedDate2)
            {
                if (LocalTransports2.Count >= p)
                {
                    LocalTransports2[p - 1] = transport;
                }
                else
                {
                    LocalTransports2.Add(transport);
                }
            }
            else if (transport.OrderDate == SelectedDate3)
            {
                if (LocalTransports3.Count >= p)
                {
                    LocalTransports3[p - 1] = transport;
                }
                else
                {
                    LocalTransports3.Add(transport);
                }
            }
            else if (transport.OrderDate == SelectedDate4)
            {
                if (LocalTransports4.Count >= p)
                {
                    LocalTransports4[p - 1] = transport;
                }
                else
                {
                    LocalTransports4.Add(transport);
                }
            }
            else if (transport.OrderDate == SelectedDate5)
            {
                if (LocalTransports5.Count >= p)
                {
                    LocalTransports5[p - 1] = transport;
                }
                else
                {
                    LocalTransports5.Add(transport);
                }
            }
        }



        public void Save()
        {
            try
            {
                string err = "";

                var addedEntities = dataContext.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added);

                foreach (var itm in addedEntities)
                {
                    if (itm.Entity is ForeignExportTransportOrderCo)
                    {
                        var cct = (itm.Entity as ForeignExportTransportOrderCo);

                        if (cct.Volume.IsNullOrEmptyString())
                        {
                            itm.ChangeState(System.Data.EntityState.Detached);
                        }
                    }
                }

                this.dataContext.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                OnPropertyChanged("Save");
            }
            catch (Exception exc)
            {
                RadWindow.Alert(exc.Message);
            }


        }

        public void Cancel()
        {
            OnPropertyChanged("Cancel");
        }

        public string ExportToExcel(string fileName)
        {
            string errorMessage = "";
            string fName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\template_OrderSheet.xlsx";

            COS.Excel.COSExcel excel = new COS.Excel.COSExcel(Properties.Resources.template_OrderSheet, fName);

            try
            {

                excel.SetData(1, 9, 2, SelectedWeek.ToString());
                excel.SetData(1, 9, 6, SelectedDate1.ToShortDateString());
                excel.SetData(1, 9, 10, SelectedDate2.ToShortDateString());
                excel.SetData(1, 9, 14, SelectedDate3.ToShortDateString());
                excel.SetData(1, 9, 18, SelectedDate4.ToShortDateString());
                excel.SetData(1, 9, 22, SelectedDate5.ToShortDateString());

                int row = 11;

                foreach (var itm in LocalTransportsTemps)
                {
                    if (itm != null)
                    {
                        excel.SetData(1, row, 2, itm.Destination.DestinationName, new Tuple<int, int>(11, 2));
                        excel.SetData(1, row, 3, itm.Destination.ordCompanyName + " | " + itm.Destination.ordStreet + " | " + itm.Destination.ordCity + " | " + itm.Destination.ordPostalCode + " | " + itm.Destination.Country.SysLocalize.en_English , new Tuple<int, int>(11, 3));
                        excel.SetData(1, row, 4, itm.Advice != null ? itm.Advice.Value : "", new Tuple<int, int>(11, 4));
                    }
                    row++;
                }

                row = 11;
                foreach (var itm in LocalTransports1)
                {
                    if (itm != null)
                    {
                        excel.SetData(1, row, 6, itm.Volume, new Tuple<int, int>(11, 6));
                        excel.SetData(1, row, 7, itm.Info != null ? itm.Info.Value : "", new Tuple<int, int>(11, 7));
                        excel.SetData(1, row, 8, itm.Note, new Tuple<int, int>(11, 8));
                    }
                    row++;
                }

                row = 11;
                foreach (var itm in LocalTransports2)
                {
                    if (itm != null)
                    {
                        excel.SetData(1, row, 10, itm.Volume, new Tuple<int, int>(11, 10));
                        excel.SetData(1, row, 11, itm.Info != null ? itm.Info.Value : "", new Tuple<int, int>(11, 11));
                        excel.SetData(1, row, 12, itm.Note, new Tuple<int, int>(11, 12));
                    }
                    row++;
                }

                row = 11;
                foreach (var itm in LocalTransports3)
                {
                    if (itm != null)
                    {
                        excel.SetData(1, row, 14, itm.Volume, new Tuple<int, int>(11, 14));
                        excel.SetData(1, row, 15, itm.Info != null ? itm.Info.Value : "", new Tuple<int, int>(11, 15));
                        excel.SetData(1, row, 16, itm.Note, new Tuple<int, int>(11, 15));
                    }
                    row++;
                }

                row = 11;
                foreach (var itm in LocalTransports4)
                {
                    if (itm != null)
                    {
                        excel.SetData(1, row, 18, itm.Volume, new Tuple<int, int>(11, 18));
                        excel.SetData(1, row, 19, itm.Info != null ? itm.Info.Value : "", new Tuple<int, int>(11, 19));
                        excel.SetData(1, row, 20, itm.Note, new Tuple<int, int>(11, 20));
                    }
                    row++;
                }

                row = 11;
                foreach (var itm in LocalTransports5)
                {
                    if (itm != null)
                    {
                        excel.SetData(1, row, 22, itm.Volume, new Tuple<int, int>(11, 22));
                        excel.SetData(1, row, 23, itm.Info != null ? itm.Info.Value : "", new Tuple<int, int>(11, 23));
                        excel.SetData(1, row, 24, itm.Note, new Tuple<int, int>(11, 24));
                    }
                    row++;
                }

                excel.Save(fileName);

            }
            catch (Exception mex)
            {
                Logging.LogException(mex, LogType.ToFileAndEmail);
                errorMessage = ResourceHelper.GetResource<string>("m_Body_LOG00000034");
            }
            finally
            {
                try
                {
                    if (excel != null)
                        excel.Dispose();
                }
                catch { }


                if (File.Exists(fName))
                    File.Delete(fName);
            }

            return errorMessage;
        }

        public string ExportToExcelAndSendEmail()
        {
            System.Net.Mail.Attachment att = null;
            System.Net.Mail.SmtpClient smtp = null;
            var directoryTemp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string errorMessage = "";

            try
            {
                ExportToExcel(directoryTemp + @"\TransportOrders.xlsx");

                att = new System.Net.Mail.Attachment(directoryTemp + @"\TransportOrders.xlsx");

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                var adds = SelectedItem.Forwarder.t_log_forwarderEmails.Where(a => a.EmailType.TypeName == "Sender");

                foreach (var itm in adds)
                    message.To.Add(itm.Email);

                message.Subject = "COS LINDAB - Objednávka dopravy";
                message.From = new System.Net.Mail.MailAddress("cos@lindab.cz");
                message.Body = "Test email from COS application:";
                message.Body += Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body += DateTime.Now.ToString();
                message.Body += Environment.NewLine;
                message.Body += Environment.NewLine;

                message.Attachments.Add(att);

                smtp = new System.Net.Mail.SmtpClient("se861057");
                //smtp = new System.Net.Mail.SmtpClient("smtp.calch.cz");

                //System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential("helpdesk@calch.cz", "Calch12349");
                smtp.UseDefaultCredentials = true;
                //smtp.Credentials = SMTPUserInfo;

                // Delivery notifications
                message.DeliveryNotificationOptions =
                DeliveryNotificationOptions.OnFailure |
                DeliveryNotificationOptions.OnSuccess |
                DeliveryNotificationOptions.Delay;

                // Ask for a read receipt

                message.Headers.Add("Disposition-Notification-To", SelectedItem.Forwarder.RecieptEmails);

                smtp.Send(message);

            }
            catch (Exception exc)
            {
                Logging.LogException(exc, LogType.ToFileAndEmail);
                errorMessage = ResourceHelper.GetResource<string>("m_Body_LOG00000035");
            }
            finally
            {

                if (smtp != null)
                    smtp.Dispose();
                if (att != null)
                    att.Dispose();
                try
                {
                    System.IO.File.Delete(directoryTemp + @"\TransportOrders.xlsx");
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                }
            }

            return errorMessage;
        }


    }

    public class TransportAdviClass : ViewModelBase
    {
        public TransportAdviClass(ZoneLogistics destination, ForeignExportAdvice advice)
        {
            Destination = destination;
            Advice = advice;
        }

        public ZoneLogistics Destination { set; get; }

        private ForeignExportAdvice _advice = null;
        public ForeignExportAdvice Advice { set { _advice = value; OnPropertyChanged("Advice"); } get { return _advice; } }
    }
}
