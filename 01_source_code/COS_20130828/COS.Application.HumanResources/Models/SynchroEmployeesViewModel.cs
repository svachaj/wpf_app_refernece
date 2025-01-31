using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Windows;
using System.ComponentModel;
using COS.Resources;

namespace COS.Application.HumanResources.Models
{
    public partial class SynchroEmployeesViewModel : ValidationViewModelBase
    {
        public SynchroEmployeesViewModel()
            : base()
        {
            _bgworker = new BackgroundWorker();
            _bgworker.DoWork += new DoWorkEventHandler(_bgworker_DoWork);
            _bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgworker_RunWorkerCompleted);
        }

        void _bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkCompleted = true;


        }

        void _bgworker_DoWork(object sender, DoWorkEventArgs e)
        {


            if (e.Argument.ToString() == "sync")
            {
                SynchronizeData();
            }
            else if (e.Argument.ToString() == "analyze")
            {
                AnalyzeData();
            }
        }

        BackgroundWorker _bgworker = null;

        public void Synchronize()
        {
            AnalyzeRuned = false;
            IsSynchro = true;

            WorkCompleted = false;

            _bgworker.RunWorkerAsync("sync");
        }

        public void Analyze()
        {
            AnalyzeRuned = true;
            IsSynchro = false;

            WorkCompleted = false;

            _bgworker.RunWorkerAsync("analyze");
        }

        private void SynchronizeData()
        {

            ProgressText = ResourceHelper.GetResource<string>("hr_SyncProgress");

            foreach (var itm in SynchroItems.Where(a => a.IsSelected))
            {
                if (itm.Action == SynchroAction.New)
                {
                    Employee empl = new Employee();

                    empl.HR_ID = itm.RONEmployee.HR_ID;
                    empl.Name = itm.RONEmployee.Name;
                    empl.Surname = itm.RONEmployee.Surname;
                    empl.Email = itm.RONEmployee.Email;
                    empl.Street = itm.RONEmployee.Street;
                    empl.City = itm.RONEmployee.City;
                    empl.PostalCode = itm.RONEmployee.PostalCode;
                    empl.DivisionHR_ID = itm.RONEmployee.Division;
                    empl.HireDate = itm.RONEmployee.HireDate;
                    empl.CostCenterHR_ID = itm.RONEmployee.CostCenter;
                    empl.SalaryGroupHR_ID = itm.RONEmployee.SalaryGroup;
                    empl.WorkGroupHR_ID = itm.RONEmployee.WorkGroup;
                    empl.LeaveDate = itm.RONEmployee.LeaveDate;


                    COSContext.Current.Employees.AddObject(empl);
                }
                else if (itm.Action == SynchroAction.Delete)
                {
                    COSContext.Current.Employees.DeleteObject(itm.COSEmployee);
                }
                else if (itm.Action == SynchroAction.Update)
                {
                    itm.COSEmployee.HR_ID = itm.RONEmployee.HR_ID;
                    itm.COSEmployee.Name = itm.RONEmployee.Name;
                    itm.COSEmployee.Surname = itm.RONEmployee.Surname;
                    itm.COSEmployee.Email = itm.RONEmployee.Email;
                    itm.COSEmployee.Street = itm.RONEmployee.Street;
                    itm.COSEmployee.City = itm.RONEmployee.City;
                    itm.COSEmployee.PostalCode = itm.RONEmployee.PostalCode;
                    itm.COSEmployee.HireDate = itm.RONEmployee.HireDate;
                    itm.COSEmployee.DivisionHR_ID = itm.RONEmployee.Division;
                    itm.COSEmployee.CostCenterHR_ID = itm.RONEmployee.CostCenter;
                    itm.COSEmployee.SalaryGroupHR_ID = itm.RONEmployee.SalaryGroup;
                    itm.COSEmployee.WorkGroupHR_ID = itm.RONEmployee.WorkGroup;
                    itm.COSEmployee.LeaveDate = itm.RONEmployee.LeaveDate;



                }
            }

            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

        }

        private void AnalyzeData()
        {


            ProgressText = ProgressText = ResourceHelper.GetResource<string>("hr_AnalyzeData");

            SynchroItems = new List<SynchronizeItem>();

            SynchronizeItem item = null;

            var cosEmpls = COSContext.Current.Employees.ToList();


            string decryptString = Crypto.DecryptString(System.Configuration.ConfigurationManager.ConnectionStrings["RONEntities"].ConnectionString, Security.SecurityHelper.SecurityKey);

            RONEntities ron = new RONEntities(decryptString);

            var ronEmpls = ron.RonEmployees.ToList();

            TotalTicks = 0;
            TotalItems = cosEmpls.Count * 2 + ronEmpls.Count;

            //hleání nových záznamů
            foreach (var itm in ronEmpls)
            {
                TotalTicks++;
                //System.Threading.Thread.Sleep(200);

                if (cosEmpls.Where(a => a.HR_ID == itm.HR_ID).Count() == 0)
                {
                    item = new SynchronizeItem();

                    item.HR_ID = itm.HR_ID;
                    item.HR_NameSurname = itm.Surname + " " + itm.Name;
                    item.RONEmployee = itm;
                    item.Action = SynchroAction.New;

                    SynchroItems.Add(item);
                }
            }

            //hledání smazaných záznamů
            foreach (var itm in cosEmpls)
            {
                TotalTicks++;
                //System.Threading.Thread.Sleep(200);

                if (ronEmpls.Where(a => a.HR_ID == itm.HR_ID).Count() == 0)
                {
                    item = new SynchronizeItem();

                    item.HR_ID = itm.HR_ID;
                    item.HR_NameSurname = itm.Surname + " " + itm.Name;
                    item.COSEmployee = itm;
                    item.Action = SynchroAction.Delete;

                    SynchroItems.Add(item);
                }
            }

            //hledání pozměněných záznamů
            foreach (var itm in cosEmpls)
            {
                TotalTicks++;
                //System.Threading.Thread.Sleep(200);

                var ronitem = ronEmpls.FirstOrDefault(a => a.HR_ID == itm.HR_ID);

                if (ronitem != null)
                {
                    string ronName = string.IsNullOrEmpty(ronitem.Name) ? null : ronitem.Name.Trim();
                    string cosName = string.IsNullOrEmpty(itm.Name) ? null : itm.Name.Trim();

                    string ronSurname = string.IsNullOrEmpty(ronitem.Surname) ? null : ronitem.Surname.Trim();
                    string cosSurname = string.IsNullOrEmpty(itm.Surname) ? null : itm.Surname.Trim();

                    string ronEmail = string.IsNullOrEmpty(ronitem.Email) ? null : ronitem.Email.Trim();
                    string cosEmail = string.IsNullOrEmpty(itm.Email) ? null : itm.Email.Trim();

                    string ronStreet = string.IsNullOrEmpty(ronitem.Street) ? null : ronitem.Street.Trim();
                    string cosStreet = string.IsNullOrEmpty(itm.Street) ? null : itm.Street.Trim();

                    string ronCity = string.IsNullOrEmpty(ronitem.City) ? null : ronitem.City.Trim();
                    string cosCity = string.IsNullOrEmpty(itm.City) ? null : itm.City.Trim();

                    string ronPostalCode = string.IsNullOrEmpty(ronitem.PostalCode) ? null : ronitem.PostalCode.Trim();
                    string cosPostalCode = string.IsNullOrEmpty(itm.PostalCode) ? null : itm.PostalCode.Trim();

                    string ronHrID = string.IsNullOrEmpty(ronitem.HR_ID) ? null : ronitem.HR_ID.Trim();
                    string cosHrID = string.IsNullOrEmpty(itm.HR_ID) ? null : itm.HR_ID.Trim();

                    string ronCostCenter = string.IsNullOrEmpty(ronitem.CostCenter) ? null : ronitem.CostCenter.Trim();
                    string cosCostCenter = string.IsNullOrEmpty(itm.CostCenterHR_ID) ? null : itm.CostCenterHR_ID.Trim();

                    string ronSalaryGroup = string.IsNullOrEmpty(ronitem.SalaryGroup) ? null : ronitem.SalaryGroup.Trim();
                    string cosSalaryGroup = string.IsNullOrEmpty(itm.SalaryGroupHR_ID) ? null : itm.SalaryGroupHR_ID.Trim();

                    string ronDivision = string.IsNullOrEmpty(ronitem.Division) ? null : ronitem.Division.Trim();
                    string cosDivision = string.IsNullOrEmpty(itm.DivisionHR_ID) ? null : itm.DivisionHR_ID.Trim();

                    string ronWorkGroup = string.IsNullOrEmpty(ronitem.WorkGroup) ? null : ronitem.WorkGroup.Trim();
                    string cosWorkGroup = string.IsNullOrEmpty(itm.WorkGroupHR_ID) ? null : itm.WorkGroupHR_ID.Trim();

             


                    if (ronName != cosName || ronSurname != cosSurname || ronEmail != cosEmail ||
                        ronStreet != cosStreet || ronCity != cosCity || ronPostalCode != cosPostalCode || ronHrID != cosHrID || ronitem.HireDate != itm.HireDate
                        || ronCostCenter != cosCostCenter || ronSalaryGroup != cosSalaryGroup || ronDivision != cosDivision || ronWorkGroup != cosWorkGroup || ronitem.LeaveDate != itm.LeaveDate)
                    {
                        item = new SynchronizeItem();

                        item.HR_ID = itm.HR_ID;
                        item.HR_NameSurname = itm.Surname + " " + itm.Name;
                        item.COSEmployee = itm;
                        item.RONEmployee = ronitem;
                        item.Action = SynchroAction.Update;

                        SynchroItems.Add(item);
                    }
                }
            }


        }

        public List<SynchronizeItem> SynchroItems
        {
            private set;
            get;
        }

        public ICommand AcceptCommand
        {
            get
            {
                return new RelayCommand(param => this.Accept());
            }
        }

        private void Accept()
        {
            //var items = SynchroItems.Where(a => a.IsSelected).Count();

            //MessageBox.Show(items.ToString());

            Synchronize();
        }

        private double _totalItems = 999999;

        public double TotalItems
        {
            set
            {
                if (_totalItems != value)
                {
                    _totalItems = value;
                    OnPropertyChanged("TotalItems");
                }
            }
            get
            {
                return _totalItems;
            }
        }

        private double _totalTicks = 0;

        public double TotalTicks
        {
            set
            {
                if (_totalTicks != value)
                {
                    _totalTicks = value;
                    OnPropertyChanged("TotalTicks");
                }
            }
            get
            {
                return _totalTicks;
            }
        }

        private bool _workCompleted = true;

        public bool WorkCompleted
        {
            set
            {
                if (_workCompleted != value)
                {
                    _workCompleted = value;
                    OnPropertyChanged("WorkCompleted");
                }
            }
            get
            {
                return _workCompleted;
            }
        }

        private bool _isSynchro = false;

        public bool IsSynchro
        {
            set
            {
                if (_isSynchro != value)
                {
                    _isSynchro = value;
                    OnPropertyChanged("IsSynchro");
                }
            }
            get
            {
                return _isSynchro;
            }
        }


        private string _progressText = "";

        public string ProgressText
        {
            set
            {
                if (_progressText != value)
                {
                    _progressText = value;
                    OnPropertyChanged("ProgressText");
                }
            }
            get
            {
                return _progressText;
            }
        }

        public bool AnalyzeRuned = false;

    }
}
