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

namespace COS.Application.Logistics.Models
{
    public partial class ForeignViewModel : ValidationViewModelBase
    {
        public ForeignViewModel()
            : base()
        {
            SelectedMiniFilter = LocalMiniFilter.First();
        }


        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNew());
            }
        }

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }

        public void Delete()
        {
            OnPropertyChanged("Delete");
        }

        public void AddNew()
        {
            OnPropertyChanged("AddNew");
        }

        private DateTime _selectedDate = DateTime.Now.Date;
        public DateTime SelectedDate
        {
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
            get
            {
                return _selectedDate;
            }
        }

        private DateTime? _lastSendTimeWindowDate = null;
        public DateTime? LastSendTimeWindowDate
        {
            set
            {
                _lastSendTimeWindowDate = value;
                OnPropertyChanged("LastSendTimeWindowDate");
            }
            get
            {
                return _lastSendTimeWindowDate;
            }
        }


        private decimal _sumVolumeCBM = 0;
        public decimal SumVolumeCBM
        {
            set
            {
                _sumVolumeCBM = value;
                OnPropertyChanged("SumVolumeCBM");
            }
            get
            {
                return _sumVolumeCBM;
            }
        }



        private decimal _sumVolumeCBMVI = 0;
        public decimal SumVolumeCBMVI
        {
            set
            {
                _sumVolumeCBMVI = value;
                OnPropertyChanged("SumVolumeCBMVI");
            }
            get
            {
                return _sumVolumeCBMVI;
            }
        }

        private decimal _sumVolumeCBMVA = 0;
        public decimal SumVolumeCBMVA
        {
            set
            {
                _sumVolumeCBMVA = value;
                OnPropertyChanged("SumVolumeCBMVA");
            }
            get
            {
                return _sumVolumeCBMVA;
            }
        }

        private decimal _sumVolumeCBMVA4H = 0;
        public decimal SumVolumeCBMVA4H
        {
            set
            {
                _sumVolumeCBMVA4H = value;
                OnPropertyChanged("SumVolumeCBMVA4H");
            }
            get
            {
                return _sumVolumeCBMVA4H;
            }
        }


        private decimal _sumVolumeCBMVAComp = 0;
        public decimal SumVolumeCBMVAComp
        {
            set
            {
                _sumVolumeCBMVAComp = value;
                OnPropertyChanged("SumVolumeCBMVAComp");
            }
            get
            {
                return _sumVolumeCBMVAComp;
            }
        }

        private int _sumCount = 0;
        public int SumCount
        {
            set
            {
                _sumCount = value;
                OnPropertyChanged("SumCount");
            }
            get
            {
                return _sumCount;
            }
        }

        private int _sumCountWithCondition = 0;
        public int SumCountWithCondition
        {
            set
            {
                _sumCountWithCondition = value;
                OnPropertyChanged("SumCountWithCondition");
            }
            get
            {
                return _sumCountWithCondition;
            }
        }


        private List<ForeignExport> _localForeignExports = new List<ForeignExport>();
        public List<ForeignExport> LocalForeignExports
        {
            set
            {
                if (_localForeignExports != value)
                {
                    _localForeignExports = value;
                    OnPropertyChanged("LocalForeignExports");

                    if (_localForeignExports != null)
                    {
                        SumCount = _localForeignExports.Count;
                        SumVolumeCBM = _localForeignExports.Sum(a => a.VolumeCbm);
                        SumCountWithCondition = _localForeignExports.Where(a => a.Finished).Count();
                        SumVolumeCBMVA = _localForeignExports.Sum(a => a.VAVolumeCBM);
                        SumVolumeCBMVI = _localForeignExports.Sum(a => a.VIVolumeCBM);
                        SumVolumeCBMVA4H = _localForeignExports.Sum(a => a.VA4HVolumeCBM);
                        SumVolumeCBMVAComp = _localForeignExports.Sum(a => a.VAinc4HVolumeCBM);
                    }
                    else
                    {
                        SumCountWithCondition = 0;
                        SumCount = 0;
                        SumVolumeCBM = 0;
                        SumVolumeCBMVI = 0;
                        SumVolumeCBMVA = 0;
                        SumVolumeCBMVA4H = 0;
                        SumVolumeCBMVAComp = 0;
                    }
                }
            }
            get
            {
                return _localForeignExports;
            }
        }


        private KeyValuePair<int, string> _selectedMiniFilter;
        public KeyValuePair<int, string> SelectedMiniFilter
        {
            set
            {
                _selectedMiniFilter = value;
                if (_selectedMiniFilter.Key == 0)
                {
                    ConditionCountVisibility = Visibility.Visible;
                }
                else
                {
                    ConditionCountVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged("SelectedMiniFilter");
            }
            get
            {
                return _selectedMiniFilter;
            }
        }

        public Dictionary<int, string> LocalMiniFilter
        {
            get
            {
                Dictionary<int, string> result = new Dictionary<int, string>();

                result.Add(0, ResourceHelper.GetResource<string>("log_All"));
                result.Add(1, ResourceHelper.GetResource<string>("log_Planned"));
                result.Add(2, ResourceHelper.GetResource<string>("log_LoadDepart"));
                result.Add(3, ResourceHelper.GetResource<string>("log_CompleteLoading"));

                return result;
            }
        }

        private Visibility _conditionCountVisibility = Visibility.Visible;
        public Visibility ConditionCountVisibility
        {
            set
            {
                _conditionCountVisibility = value;
                OnPropertyChanged("ConditionCountVisibility");
            }
            get
            {
                return _conditionCountVisibility;
            }
        }


        public void RecalculateForeignExport(DateTime? dateFrom, DateTime? dateTo)
        {
            COSContext.Current.Refresh(RefreshMode.StoreWins, COSContext.Current.ForeignExports.Where(a => a.PlannedDate >= dateFrom && a.PlannedDate <= dateTo));

            List<ForeignExport> list = COSContext.Current.ForeignExports.Where(a => a.PlannedDate >= dateFrom && a.PlannedDate <= dateTo).ToList<ForeignExport>();

            ForeignDetailViewModel model = new ForeignDetailViewModel();
            foreach (ForeignExport export in list)
            {
                model.SelectedItem = export;
                foreach (ForeignExportConnection connection in export.Connections)
                {
                    foreach (ForeignExportDetail detail in connection.ExportDetails)
                    {
                        model.SelectedDetailItem = detail;
                        if (!model.SelectedItem.Forwarder.CanCalculateBaf)
                        {
                            model.SelectedDetailItem.BafPRice = 0M;
                        }
                        model.SelectedItem.ForwarderPrice += 1;
                        model.SelectedItem.ForwarderPrice -= 1;
                        model.SumTotalPrice();
                        model.SumDetailValues();
                    }
                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    COSContext.Current.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    scope.Complete();
                }
                catch (Exception exception)
                {
                    Logging.LogException(exception, LogType.ToFileAndEmail);
                }
                finally
                {
                    if (scope != null)
                    {
                        scope.Dispose();
                    }
                }
            }
        }

        public static void GenerateTimeWindowEmail(List<ForeignViewExportClass> exports, out string errorMessage)
        {
            errorMessage = "";
          
            System.Net.Mail.Attachment att = null;
            System.Net.Mail.SmtpClient smtp = null;
            var directoryTemp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string fName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\template_timewindow1.xls";

            COS.Excel.COSExcel excel = new COS.Excel.COSExcel(Properties.Resources.template_timewindow1, fName);

            try
            {

                int row = 3;

                foreach (var itm in exports)
                {
                    excel.SetData(1, row, 1, itm.CustomerNumbers, new Tuple<int, int>(2, 1));
                    excel.SetData(1, row, 2, itm.Country, new Tuple<int, int>(2, 2));
                    excel.SetData(1, row, 3, itm.Destination, new Tuple<int, int>(2, 3));
                    excel.SetData(1, row, 4, itm.TimeWindow, new Tuple<int, int>(2, 4));

                    row++;
                }

                excel.Save(directoryTemp + @"\TimeWindows.xlsx");

                excel.Dispose();

                att = new System.Net.Mail.Attachment(directoryTemp + @"\TimeWindows.xlsx");

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                var emails = COSContext.Current.SysEmailAddresses.Where(a => a.GroupCode == "LogForTW1");
                COSContext.Current.Refresh(RefreshMode.StoreWins, emails);

                var adds = emails.ToList();

                foreach (var itm in adds)
                    message.To.Add(itm.EmailAddress);

                message.Subject = ResourceHelper.GetResource<string>("log_TimewWindowSubj") + DateTime.Now.ToString();
                message.From = new System.Net.Mail.MailAddress("cos@lindab.cz");
                message.Body += Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body = "V příloze naleznete seznam časových oken pro den : " + DateTime.Now.ToString();
                message.Body += Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body += Environment.NewLine;
                message.Body = "Tento email byl generován aplikací COS." + DateTime.Now.ToString();
                message.Body += Environment.NewLine;
                message.Body = "Oddělení - Logistika - VA & VI ." + DateTime.Now.ToString();

                message.Attachments.Add(att);

                smtp = new System.Net.Mail.SmtpClient("se861057");
                //smtp = new System.Net.Mail.SmtpClient("smtp.calch.cz");

                //System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential("helpdesk@calch.cz", "Calch12349");
                smtp.UseDefaultCredentials = true;
                //smtp.Credentials = SMTPUserInfo;


                smtp.Send(message);

            }
            catch (Exception mex)
            {
                Logging.LogException(mex, LogType.ToFileAndEmail);
                errorMessage = ResourceHelper.GetResource<string>("m_Body_LOG00000031");
            }
            finally
            {
                try
                {
                    if (excel != null)
                        excel.Dispose();
                }
                catch { }
                if (smtp != null)
                    smtp.Dispose();
                if (att != null)
                    att.Dispose();
                try
                {
                    System.IO.File.Delete(directoryTemp + @"\TimeWindows.xlsx");
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                }
            }
        }

        public static List<ForeignViewExportClass> GetExportClasses(List<ForeignExport> exports)
        {
            List<ForeignViewExportClass> result = new List<ForeignViewExportClass>();
            ForeignViewExportClass data = null;

            foreach (var itm in exports)
            {
                data = new ForeignViewExportClass();

                string cns = "";


                foreach (var dconn in itm.Connections)
                {

                    foreach (var cn in dconn.Destination.t_log_foreignExport_Zone_CustomerNumber.Where(a => dconn.ExportDetails.Select(o => o.ID_OrderedBy).Contains(a.ID_OrderedBy)).Select(a => a.cNumber))
                    {
                        string cnv = cn.Replace("Z", "");
                        cnv = cnv.TrimStart('0');

                        cns += cnv;
                        cns += "/";
                    }
                }


                if (!string.IsNullOrEmpty(cns))
                {
                    cns = cns.Remove(cns.Length - 1, 1);
                }


                data.CustomerNumbers = cns;
                data.Country = itm.Destination.Country.Code;
                data.Destination = itm.Destination.DestinationName;
                data.TimeWindow = itm.Destination.ProdTimeWindow.HasValue ? itm.Destination.ProdTimeWindow.Value.ToShortTime() : "00:00";
                data.ID_Destination = itm.ID_Destination;

                result.Add(data);

            }

            return result;
        }
    }

    public class ForeignViewExportClass : COS.Common.NotifyBase
    {

        private string _customerNumber = "";
        public string CustomerNumbers { set { _customerNumber = value; OnPropertyChanged("CustomerNumbers"); } get { return _customerNumber; } }

        private string _country = "";
        public string Country { set { _country = value; OnPropertyChanged("Country"); } get { return _country; } }

        private string _destination = "";
        public string Destination { set { _destination = value; OnPropertyChanged("Destination"); } get { return _destination; } }

        private string _timeWindow = "";
        public string TimeWindow { set { _timeWindow = value; OnPropertyChanged("TimeWindow"); } get { return _timeWindow; } }

        public int ID_Destination { set; get; }

    }
}
