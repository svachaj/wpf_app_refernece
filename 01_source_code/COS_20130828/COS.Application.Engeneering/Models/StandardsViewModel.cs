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

namespace COS.Application.Engeneering.Models
{
    public partial class StandardsViewModel : ValidationViewModelBase
    {
        public StandardsViewModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(StandardsViewModel_PropertyChanged);

            // LocalStandards = COSContext.Current.Standards.ToList();
        }

        void StandardsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDivision")
            {
                if (SelectedDivision != null)
                {
                    LocalWorkGroups = COSContext.Current.WorkGroups.Where(a => a.ID_Division == SelectedDivision.ID).ToList();
                }
            }
            else if (e.PropertyName == "SelectedWorkGroup")
            {
                if (SelectedWorkGroup != null)
                    LocalWorkCenters = COSContext.Current.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID).Select(a => a.WorkCenter).ToList();
            }


            if (e.PropertyName == "SelectedDivisionExport")
            {
                if (SelectedDivisionExport != null)
                {
                    LocalWorkGroupsExport = COSContext.Current.WorkGroups.Where(a => a.ID_Division == SelectedDivisionExport.ID).ToList();
                }
            }
            else if (e.PropertyName == "SelectedWorkGroupExport")
            {
                if (SelectedWorkGroupExport != null)
                    LocalWorkCentersExport = COSContext.Current.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == SelectedWorkGroupExport.ID).Select(a => a.WorkCenter).ToList();
            }
        }

        public bool IsNewItem = false;

        private List<WorkCenter> _localWorkCenterList = null;
        public List<WorkCenter> LocalWorkCentersList
        {
            get
            {
                if (_localWorkCenterList == null)
                    _localWorkCenterList = COSContext.Current.WorkCenters.ToList();

                return _localWorkCenterList;
            }
        }

        public Standard PrevStandard = null;
        private Standard _selectedStandard = null;
        public Standard SelectedStandard
        {
            set
            {
                if (_selectedStandard != value)
                {
                    COSContext.Current.RejectChanges();
                    PrevStandard = _selectedStandard;
                    _selectedStandard = value;
                    if (_selectedStandard != null)
                    {
                        _selectedStandard.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_selectedStandard_PropertyChanged);
                    }
                    OnPropertyChanged("SelectedStandard");
                }
            }
            get
            {
                return _selectedStandard;
            }
        }

        void _selectedStandard_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (SelectedStandard != null)
            {
                if (e.PropertyName == "PcsPerHour")
                {
                    SelectedStandard.PcsPerMinute = SelectedStandard.PcsPerHour / 60;
                }
            }
        }


        private List<Standard> _exportData = new List<Standard>();
        public List<Standard> ExportData
        {
            get
            {
                return _exportData;
            }
            set
            {
                _exportData = value;
            }
        }

        private List<Standard> _LocalStandards = new List<Standard>();
        public List<Standard> LocalStandards
        {
            get
            {
                return _LocalStandards;
            }
            set
            {
                _LocalStandards = value;
            }
        }




        private List<StandardExportItem> _exportDataUnexists = new List<StandardExportItem>();
        public List<StandardExportItem> ExportDataUnexists
        {
            get
            {
                return _exportDataUnexists;
            }
            set
            {
                _exportDataUnexists = value;
                OnPropertyChanged("ExportDataUnexists");
            }
        }

        public HourlyProduction TempProduction { set; get; }

        private List<HourlyProduction> _unexistStandardHPs = new List<HourlyProduction>();
        public List<HourlyProduction> UnexistStandardHPs
        {
            get
            {
                return _unexistStandardHPs;
            }
            set
            {
                _unexistStandardHPs = value;
                OnPropertyChanged("UnexistStandardHPs");
            }
        }


        private List<ImportStandardData> _importData = new List<ImportStandardData>();
        public List<ImportStandardData> ImportData
        {
            get
            {
                return _importData;
            }
            private set
            {
                _importData = value;
            }
        }

        public void SetExportData()
        {

            ExportData.Clear();

            var stands = COSContext.Current.Standards.AsQueryable();

            if (SelectedWorkCenterExport != null)
                stands = stands.Where(a => a.ID_WorkCenter == SelectedWorkCenterExport.ID);

            if (SelectedWorkGroupExport != null)
                stands = stands.Where(a => a.ID_WorkGroup == SelectedWorkGroupExport.ID);

            if (SelectedDivisionExport != null)
                stands = stands.Where(a => a.WorkGroup.ID_Division == SelectedDivisionExport.ID);

            ExportData = stands.ToList();



        }



        public void ImportStandartsFromCSV(string fileName, char separator)
        {
            var lines = File.ReadAllLines(fileName, Encoding.Default);

            ImportStandardData standard;
            ImportData.Clear();
            COSContext.Current.RejectChanges();


            bool firstline = true;
            foreach (string line in lines)
            {
                if (firstline)
                    firstline = false;
                else
                {
                    standard = new ImportStandardData();
                    standard.NewStandard = new Standard();
                    standard.NewStandard.CreateDate = DateTime.Now;
                    standard.NewStandard.ModifyDate = DateTime.Now;
                    standard.NewStandard.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;
                    standard.NewStandard.CreatedBy_UID = COSContext.Current.CurrentUser.ID;

                    var values = line.Split(new char[] { separator }, StringSplitOptions.None);

                    if (values.Length > 0)
                    {
                        if (string.IsNullOrEmpty(values[0]))
                        {
                            standard.ItemNumberText = ResourceHelper.GetResource<string>("eng_NotFilled");
                        }
                        else
                        {
                            standard.NewStandard.ItemNumber = values[0].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' });
                        }
                    }
                    else
                    {
                        standard.ItemNumberText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }

                    if (values.Length > 1)
                    {
                        if (string.IsNullOrEmpty(values[1]))
                        {
                            standard.NewStandard.isConfig = false;
                        }
                        else
                        {
                            bool newval;
                            if (bool.TryParse(values[1].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' }), out newval))
                            {
                                standard.NewStandard.isConfig = newval;
                            }
                            else
                            {
                                standard.IsConfigText = ResourceHelper.GetResource<string>("eng_InvalidData");
                            }
                        }
                    }
                    else
                    {
                        standard.IsConfigText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }


                    if (values.Length > 2)
                    {
                        if (string.IsNullOrEmpty(values[2]))
                        {
                            //standard.DescriptionText = "Nevyplněno";
                        }
                        else
                        {
                            string newval = values[2].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' });

                            standard.NewStandard.ItemDescription = newval;
                        }
                    }
                    else
                    {
                        standard.DescriptionText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }

                    if (values.Length > 3)
                    {
                        if (string.IsNullOrEmpty(values[3]))
                        {
                            standard.LaboursText = ResourceHelper.GetResource<string>("eng_NotFilled");
                        }
                        else
                        {
                            int newval;
                            if (int.TryParse(values[3].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' }), out newval))
                            {
                                standard.NewStandard.Labour = newval;
                            }
                            else
                            {
                                standard.LaboursText = ResourceHelper.GetResource<string>("eng_InvalidData");
                            }
                        }
                    }
                    else
                    {
                        standard.LaboursText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }


                    if (values.Length > 4)
                    {
                        if (string.IsNullOrEmpty(values[4]))
                        {
                            standard.WorkGroupText = ResourceHelper.GetResource<string>("eng_NotFilled");
                        }
                        else
                        {
                            string wgs = values[4].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' });

                            var wg = COSContext.Current.WorkGroups.FirstOrDefault(a => a.Value == wgs);

                            if (wg != null)
                            {
                                standard.NewStandard.WorkGroup = wg;
                            }
                            else
                            {
                                standard.WorkGroupText = ResourceHelper.GetResource<string>("eng_WgNotExist");
                            }
                        }
                    }
                    else
                    {
                        standard.WorkGroupText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }

                    if (values.Length > 5)
                    {
                        if (string.IsNullOrEmpty(values[5]))
                        {
                            standard.WorkCenterText = ResourceHelper.GetResource<string>("eng_NotFilled");
                        }
                        else
                        {
                            string wcs = values[5].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' });

                            var wc = COSContext.Current.WorkCenters.FirstOrDefault(a => a.Value == wcs);

                            if (wc != null)
                            {
                                standard.NewStandard.WorkCenter = wc;
                            }
                            else
                            {
                                standard.WorkCenterText = ResourceHelper.GetResource<string>("eng_WcNotExist");
                            }
                        }
                    }
                    else
                    {
                        standard.WorkCenterText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }

                    if (values.Length > 6)
                    {
                        if (string.IsNullOrEmpty(values[6]))
                        {
                            //standard.ItemGroupsText = "Nevyplněno";
                        }
                        else
                        {
                            string igs = values[6].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' });

                            var ig = COSContext.Current.ItemGroups.FirstOrDefault(a => a.SysLocalize.cs_Czech == igs || a.SysLocalize.en_English == igs);

                            if (ig != null)
                            {
                                standard.NewStandard.ItemGroup = ig;
                            }
                            else
                            {
                                standard.ItemGroupsText = ResourceHelper.GetResource<string>("eng_IgNotExist");
                            }
                        }
                    }
                    else
                    {
                        standard.ItemGroupsText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }


                    if (values.Length > 7)
                    {
                        if (string.IsNullOrEmpty(values[7]))
                        {
                            //standard.WeighText = "Nevyplněno";
                        }
                        else
                        {
                            decimal newval;
                            if (decimal.TryParse(values[7].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' }), out newval))
                            {
                                standard.NewStandard.Weight_Kg = newval;
                            }
                            else
                            {
                                standard.WeighText = ResourceHelper.GetResource<string>("eng_InvalidData");
                            }
                        }
                    }
                    else
                    {
                        standard.WeighText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }

                    if (values.Length > 8)
                    {
                        if (string.IsNullOrEmpty(values[8]))
                        {
                            standard.SetupTimeText = ResourceHelper.GetResource<string>("eng_NotFilled");
                        }
                        else
                        {
                            int newval;
                            if (int.TryParse(values[8].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' }), out newval))
                            {
                                standard.NewStandard.SetupTime_mm = newval;
                            }
                            else
                            {
                                standard.SetupTimeText = ResourceHelper.GetResource<string>("eng_InvalidData");
                            }
                        }
                    }
                    else
                    {
                        standard.SetupTimeText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }


                    if (values.Length > 9)
                    {
                        if (string.IsNullOrEmpty(values[9]))
                        {
                            standard.PcsPerMinText = ResourceHelper.GetResource<string>("eng_NotFilled");
                        }
                        else
                        {
                            decimal newval;
                            if (decimal.TryParse(values[9].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' }), out newval))
                            {
                                standard.NewStandard.PcsPerMinute = newval;
                            }
                            else
                            {
                                standard.PcsPerMinText = ResourceHelper.GetResource<string>("eng_InvalidData");
                            }
                        }
                    }
                    else
                    {
                        standard.PcsPerMinText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }

                    if (values.Length > 10)
                    {
                        if (string.IsNullOrEmpty(values[10]))
                        {
                            standard.PcsPerHourText = ResourceHelper.GetResource<string>("eng_NotFilled");
                        }
                        else
                        {
                            decimal newval;
                            if (decimal.TryParse(values[10].TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' }), out newval))
                            {
                                standard.NewStandard.PcsPerHour = newval;
                            }
                            else
                            {
                                standard.PcsPerHourText = ResourceHelper.GetResource<string>("eng_InvalidData");
                            }
                        }
                    }
                    else
                    {
                        standard.PcsPerHourText = ResourceHelper.GetResource<string>("eng_BadCollNumber");
                    }

                    if (standard.NewStandard.WorkCenter != null && !string.IsNullOrEmpty(standard.NewStandard.ItemNumber))
                    {
                        var exs = COSContext.Current.Standards.FirstOrDefault(a => a.ID_Standard == standard.NewStandard.WorkCenter.Value + standard.NewStandard.ItemNumber.Trim());

                        if (exs != null)
                        {
                            standard.ExistingStandard = exs;
                        }
                    }

                    ImportData.Add(standard);
                }
            }

        }

        public void ImportDataInDB()
        {
            List<ImportStandardData> itemsAdded = new List<ImportStandardData>();

            foreach (var item in ImportData)
            {
                bool savethis = true;

                if (item.NewStandard != null)
                {
                    if (!string.IsNullOrEmpty(item.ItemNumberText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.IsConfigText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.DescriptionText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.LaboursText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.WorkCenterText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.WorkGroupText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.ItemGroupsText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.WeighText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.SetupTimeText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.PcsPerHourText))
                    {
                        savethis = false;
                    }

                    if (!string.IsNullOrEmpty(item.PcsPerMinText))
                    {
                        savethis = false;
                    }
                }
                if (savethis)
                {

                    if (item.ExistingStandard != null)
                    {
                        item.ExistingStandard.ItemNumber = item.NewStandard.ItemNumber;
                        item.ExistingStandard.isConfig = item.NewStandard.isConfig;
                        item.ExistingStandard.ItemDescription = item.NewStandard.ItemDescription;
                        item.ExistingStandard.Labour = item.NewStandard.Labour;
                        item.ExistingStandard.WorkCenter = item.NewStandard.WorkCenter;
                        item.ExistingStandard.WorkGroup = item.NewStandard.WorkGroup;
                        item.ExistingStandard.ItemGroup = item.NewStandard.ItemGroup;
                        item.ExistingStandard.Weight_Kg = item.NewStandard.Weight_Kg;
                        item.ExistingStandard.SetupTime_mm = item.NewStandard.SetupTime_mm;
                        item.ExistingStandard.PcsPerHour = item.NewStandard.PcsPerHour;
                        item.ExistingStandard.PcsPerMinute = item.NewStandard.PcsPerMinute;
                        item.ExistingStandard.ID_Standard = item.ExistingStandard.WorkCenter.Value + item.ExistingStandard.ItemNumber;

                        COSContext.Current.Detach(item.NewStandard);
                    }
                    else
                    {
                        item.NewStandard.ID_Standard = item.NewStandard.WorkCenter.Value + item.NewStandard.ItemNumber;

                        COSContext.Current.Standards.AddObject(item.NewStandard);
                    }

                    itemsAdded.Add(item);
                }
                else
                {
                    COSContext.Current.Detach(item.NewStandard);
                }


            }

            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

            foreach (var itm in itemsAdded)
                ImportData.Remove(itm);

            foreach (var itm in ImportData)
            {
                Standard standard = new Standard();
                standard.CreateDate = DateTime.Now;
                standard.ModifyDate = DateTime.Now;
                standard.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;
                standard.CreatedBy_UID = COSContext.Current.CurrentUser.ID;

                standard.ItemNumber = itm.NewStandard.ItemNumber;
                standard.isConfig = itm.NewStandard.isConfig;
                standard.ItemDescription = itm.NewStandard.ItemDescription;
                standard.Labour = itm.NewStandard.Labour;
                standard.WorkCenter = COSContext.Current.WorkCenters.FirstOrDefault(a => a.ID == itm.NewStandard.ID_WorkCenter);
                standard.WorkGroup = COSContext.Current.WorkGroups.FirstOrDefault(a => a.ID == itm.NewStandard.ID_WorkGroup);
                standard.ItemGroup = COSContext.Current.ItemGroups.FirstOrDefault(a => a.ID == itm.NewStandard.ID_ItemGroup);
                standard.SetupTime_mm = itm.NewStandard.SetupTime_mm;
                standard.Weight_Kg = itm.NewStandard.Weight_Kg;
                standard.PcsPerHour = itm.NewStandard.PcsPerHour;
                standard.PcsPerMinute = itm.NewStandard.PcsPerMinute;
                standard.ID_Standard = standard.WorkCenter.Value + standard.ItemNumber;

                itm.NewStandard = standard;
            }



        }


        public ICommand InsertToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.AddNewStandard());
            }
        }

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

        public ICommand DeleteToolBarCommand
        {
            get
            {
                return new RelayCommand(param => this.Delete());
            }
        }


        public void Save()
        {
            string message = null;
            if (IsValid(out message))
            {
                SelectedStandard.ID_Standard = SelectedStandard.WorkCenter.Value + SelectedStandard.ItemNumber;
                //COSContext.Current.Standards.AddObject(SelectedStandard);
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                EditingMode = EditMode.View;
                IsNewItem = false;
            }
            else if (!string.IsNullOrEmpty(message))
            {
                RadWindow.Alert(new DialogParameters() { Content = message, Header = ResourceHelper.GetResource<string>("m_Header3_I"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }

            OnPropertyChanged("Save");
        }

        public void Cancel()
        {
            EditingMode = EditMode.View;
            IsNewItem = false;
            COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }


        public void Delete()
        {
            if (SelectedStandard != null)
            {
                COSContext.Current.Standards.DeleteObject(SelectedStandard);
                LocalStandards.Remove(SelectedStandard);
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }

            OnPropertyChanged("Delete");
        }

        public void DeleteSelection(List<Standard> items)
        {
            if (items != null)
            {
                foreach (var itm in items)
                {
                    COSContext.Current.Standards.DeleteObject(itm);
                    LocalStandards.Remove(itm);
                }
                COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            }

            OnPropertyChanged("Delete");
        }

        public void AddNewStandard()
        {
            IsNewItem = true;
            EditingMode = EditMode.New;

            Standard stand = new Standard();


            stand.CreateDate = DateTime.Now;
            stand.ModifyDate = DateTime.Now;
            stand.ModifiedBy_UID = COSContext.Current.CurrentUser.ID;
            stand.CreatedBy_UID = COSContext.Current.CurrentUser.ID;

            COSContext.Current.Standards.AddObject(stand);
            LocalStandards.Add(stand);

            SelectedStandard = stand;

        }


        public bool IsValid(out string message)
        {
            bool result = true;
            message = null;
            if (SelectedStandard == null)
                result = false;
            else
            {
                if (string.IsNullOrEmpty(SelectedStandard.ItemNumber))
                {
                    message += ResourceHelper.GetResource<string>("m_Body_ENG00000006");
                    message += Environment.NewLine;
                    result = false;
                }

                if (SelectedStandard.Labour < 1)
                {
                    message += ResourceHelper.GetResource<string>("m_Body_ENG00000007");
                    message += Environment.NewLine;
                    result = false;
                }

                if (SelectedStandard.WorkGroup == null)
                {
                    message += ResourceHelper.GetResource<string>("m_Body_ENG00000008");
                    message += Environment.NewLine;
                    result = false;
                }

                if (SelectedStandard.WorkCenter == null)
                {
                    message += ResourceHelper.GetResource<string>("m_Body_ENG00000009");
                    message += Environment.NewLine;
                    result = false;
                }

                //if (SelectedStandard.ItemGroup == null)
                //{
                //    message += "Skupina položek je povinná!";
                //    message += Environment.NewLine;
                //    result = false;
                //}

                if (SelectedStandard.SetupTime_mm < 0)
                {
                    message += ResourceHelper.GetResource<string>("m_Body_ENG00000010");
                    message += Environment.NewLine;
                    result = false;
                }

                if (SelectedStandard.PcsPerMinute <= 0)
                {
                    message += ResourceHelper.GetResource<string>("m_Body_ENG00000011");
                    message += Environment.NewLine;
                    result = false;
                }

                if (SelectedStandard.PcsPerHour <= 0)
                {
                    message += ResourceHelper.GetResource<string>("m_Body_ENG00000012");
                    message += Environment.NewLine;
                    result = false;
                }
            }

            return result;
        }



        private List<WorkGroup> _localWorkGroups = null;
        public List<WorkGroup> LocalWorkGroups
        {
            set
            {
                _localWorkGroups = value;
                OnPropertyChanged("LocalWorkGroups");
            }
            get
            {
                return _localWorkGroups;
            }
        }

        private List<WorkCenter> _localWorkCenters = null;
        public List<WorkCenter> LocalWorkCenters
        {
            set
            {
                _localWorkCenters = value;
                OnPropertyChanged("LocalWorkCenters");
            }
            get
            {
                return _localWorkCenters;
            }
        }

        private List<WorkGroup> _localWorkGroupsExport = null;
        public List<WorkGroup> LocalWorkGroupsExport
        {
            set
            {
                _localWorkGroupsExport = value;
                OnPropertyChanged("LocalWorkGroupsExport");
            }
            get
            {
                return _localWorkGroupsExport;
            }
        }

        private List<WorkCenter> _localWorkCentersExport = null;
        public List<WorkCenter> LocalWorkCentersExport
        {
            set
            {
                _localWorkCentersExport = value;
                OnPropertyChanged("LocalWorkCentersExport");
            }
            get
            {
                return _localWorkCentersExport;
            }
        }


        private DateTime? _selectedDateFrom = DateTime.Now.Date;
        public DateTime? SelectedDateFrom
        {
            set
            {
                _selectedDateFrom = value;
                OnPropertyChanged("SelectedDateFrom");
            }
            get
            {
                return _selectedDateFrom;
            }
        }

        private DateTime? _selectedDateTo = DateTime.Now.Date;
        public DateTime? SelectedDateTo
        {
            set
            {
                _selectedDateTo = value;
                OnPropertyChanged("SelectedDateTo");
            }
            get
            {
                return _selectedDateTo;
            }
        }

        private Division _selectedDivision = null;
        public Division SelectedDivision
        {
            set
            {
                _selectedDivision = value;
                OnPropertyChanged("SelectedDivision");
            }
            get
            {
                return _selectedDivision;
            }
        }

        private Division _selectedDivisionExport = null;
        public Division SelectedDivisionExport
        {
            set
            {
                _selectedDivisionExport = value;
                OnPropertyChanged("SelectedDivisionExport");
            }
            get
            {
                return _selectedDivisionExport;
            }
        }

        private Shift _selectedShift = null;
        public Shift SelectedShift
        {
            set
            {
                _selectedShift = value;
                OnPropertyChanged("SelectedShift");
            }
            get
            {
                return _selectedShift;
            }
        }

        private ShiftType _selectedShiftType = null;
        public ShiftType SelectedShiftType
        {
            set
            {
                _selectedShiftType = value;
                OnPropertyChanged("SelectedShiftType");
            }
            get
            {
                return _selectedShiftType;
            }
        }



        private WorkGroup _selectedWorkGroup = null;
        public WorkGroup SelectedWorkGroup
        {
            set
            {
                _selectedWorkGroup = value;
                OnPropertyChanged("SelectedWorkGroup");
            }
            get
            {
                return _selectedWorkGroup;
            }
        }

        private WorkCenter _selectedWorkCenter = null;
        public WorkCenter SelectedWorkCenter
        {
            set
            {
                _selectedWorkCenter = value;
                OnPropertyChanged("SelectedWorkCenter");
            }
            get
            {
                return _selectedWorkCenter;
            }
        }

        private WorkGroup _selectedWorkGroupExport = null;
        public WorkGroup SelectedWorkGroupExport
        {
            set
            {
                _selectedWorkGroupExport = value;
                OnPropertyChanged("SelectedWorkGroupExport");
            }
            get
            {
                return _selectedWorkGroupExport;
            }
        }

        private WorkCenter _selectedWorkCenterExport = null;
        public WorkCenter SelectedWorkCenterExport
        {
            set
            {
                _selectedWorkCenterExport = value;
                OnPropertyChanged("SelectedWorkCenterExport");
            }
            get
            {
                return _selectedWorkCenterExport;
            }
        }


        private List<KPIReportData> _reportData = null;
        public List<KPIReportData> ReportData
        {
            set
            {
                _reportData = value;
                OnPropertyChanged("ReportData");
            }
            get
            {
                return _reportData;
            }
        }


        private double? _yearFrom = null;
        public double? YearFrom
        {
            set
            {
                _yearFrom = value;
                OnPropertyChanged("YearFrom");
            }
            get
            {
                return _yearFrom;
            }
        }
        private double? _yearTo = null;
        public double? YearTo
        {
            set
            {
                _yearTo = value;
                OnPropertyChanged("YearTo");
            }
            get
            {
                return _yearTo;
            }
        }

        private double? _monthFrom = null;
        public double? MonthFrom
        {
            set
            {
                _monthFrom = value;
                OnPropertyChanged("MonthFrom");
            }
            get
            {
                return _monthFrom;
            }
        }

        private double? _monthTo = null;
        public double? MonthTo
        {
            set
            {
                _monthTo = value;
                OnPropertyChanged("MonthTo");
            }
            get
            {
                return _monthTo;
            }
        }

        private double? _yearOfMonth = null;
        public double? YearOfMonth
        {
            set
            {
                _yearOfMonth = value;
                OnPropertyChanged("YearOfMonth");
            }
            get
            {
                return _yearOfMonth;
            }
        }

        private double? _WeekFrom = null;
        public double? WeekFrom
        {
            set
            {
                _WeekFrom = value;
                OnPropertyChanged("WeekFrom");
            }
            get
            {
                return _WeekFrom;
            }
        }

        private double? _WeekTo = null;
        public double? WeekTo
        {
            set
            {
                _WeekTo = value;
                OnPropertyChanged("WeekTo");
            }
            get
            {
                return _WeekTo;
            }
        }

        private double? _yearOfWeek = null;
        public double? YearOfWeek
        {
            set
            {
                _yearOfWeek = value;
                OnPropertyChanged("YearOfWeek");
            }
            get
            {
                return _yearOfWeek;
            }
        }

        private int _currentHPCount = 10;
        public int CurrentHPCount
        {
            set
            {
                _currentHPCount = value;
                OnPropertyChanged("CurrentHPCount");
            }
            get
            {
                return _currentHPCount;
            }
        }

        private bool _isYearSelected = false;
        public bool IsYearSelected
        {
            set
            {
                if (_isYearSelected != value)
                {
                    _isYearSelected = value;
                    OnPropertyChanged("IsYearSelected");
                }
            }
            get
            {
                return _isYearSelected;
            }
        }
        private bool _isMonthSelected = false;
        public bool IsMonthSelected
        {
            set
            {
                if (_isMonthSelected != value)
                {
                    _isMonthSelected = value;
                    OnPropertyChanged("IsMonthSelected");
                }
            }
            get
            {
                return _isMonthSelected;
            }
        }

        private bool _isWeekSelected = false;
        public bool IsWeekSelected
        {
            set
            {
                if (_isWeekSelected != value)
                {
                    _isWeekSelected = value;
                    OnPropertyChanged("IsWeekSelected");
                }
            }
            get
            {
                return _isWeekSelected;
            }
        }

        private bool _isDaySelected = true;
        public bool IsDaySelected
        {
            set
            {
                if (_isDaySelected != value)
                {
                    _isDaySelected = value;
                    OnPropertyChanged("IsDaySelected");
                }
            }
            get
            {
                return _isDaySelected;
            }
        }

        private bool _unexistsStandards = false;
        public bool UnexistsStandards
        {
            set
            {
                if (_unexistsStandards != value)
                {
                    _unexistsStandards = value;
                    OnPropertyChanged("UnexistsStandards");
                }
            }
            get
            {
                return _unexistsStandards;
            }
        }

        public ICommand ClearDivisionCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearDivision());
            }
        }

        private void ClearDivision()
        {
            SelectedDivision = null;
        }

        public ICommand ClearShiftTypeCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearShiftType());
            }
        }

        private void ClearShiftType()
        {
            SelectedShiftType = null;
        }

        public ICommand ClearShiftCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearShift());
            }
        }

        private void ClearShift()
        {
            SelectedShift = null;
        }

        public ICommand ClearWorkGroupCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearWorkGroup());
            }
        }

        private void ClearWorkGroup()
        {
            SelectedWorkGroup = null;
        }

        public ICommand ClearWorkCenterCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearWorkCenter());
            }
        }

        private void ClearWorkCenter()
        {
            SelectedWorkCenter = null;
        }


        public ICommand ClearWorkGroupExportCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearWorkGroupExport());
            }
        }



        public ICommand ClearDivisionExportCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearDivisionExport());
            }
        }

        private void ClearDivisionExport()
        {
            SelectedDivisionExport = null;
        }

        private void ClearWorkGroupExport()
        {
            SelectedWorkGroupExport = null;
        }

        public ICommand ClearWorkCenterExportCommand
        {
            get
            {
                return new RelayCommand(param => this.ClearWorkCenterExport());
            }
        }

        private void ClearWorkCenterExport()
        {
            SelectedWorkCenterExport = null;
        }



        public void SetDataForUnexistsStandards()
        {
            ExportDataUnexists.Clear();
            UnexistStandardHPs.Clear();

            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.HourlyProductions);
            COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.Standards);

            var productions = COSContext.Current.HourlyProductions.AsQueryable();

            if (SelectedWorkGroup != null)
                productions = productions.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID);

            if (SelectedWorkCenter != null)
                productions = productions.Where(a => a.ID_WorkCenter == SelectedWorkCenter.ID);

            if (SelectedDivision != null)
                productions = productions.Where(a => a.ID_Division == SelectedDivision.ID);

            if (SelectedShift != null)
                productions = productions.Where(a => a.ID_Shift == SelectedShift.ID);

            if (SelectedShiftType != null)
                productions = productions.Where(a => a.ID_ShiftType == SelectedShiftType.ID);



            if (IsMonthSelected)
            {
                if (YearOfMonth.HasValue && MonthFrom.HasValue)
                    SelectedDateFrom = new DateTime((int)YearOfMonth, (int)MonthFrom, 1);
                else
                    SelectedDateFrom = null;

                if (YearOfMonth.HasValue && MonthTo.HasValue)
                    SelectedDateTo = new DateTime((int)YearOfMonth, (int)MonthTo, 1).AddMonths(1).AddDays(-1);
                else
                    SelectedDateTo = null;
            }
            else if (IsYearSelected)
            {
                if (YearFrom.HasValue)
                    SelectedDateFrom = new DateTime((int)YearFrom, 1, 1);
                else
                    SelectedDateFrom = null;

                if (YearTo.HasValue)
                    SelectedDateTo = new DateTime((int)YearTo, 12, 31);
                else
                    SelectedDateTo = null;
            }
            else if (IsWeekSelected)
            {
                if (WeekFrom.HasValue && YearOfWeek.HasValue)
                {
                    CultureInfo myCI = new CultureInfo("cs-CZ");
                    Calendar myCal = myCI.Calendar;

                    SelectedDateFrom = myCal.AddWeeks(new DateTime((int)YearOfWeek, 1, 2), (int)WeekFrom).AddDays(-7);

                }
                else
                    SelectedDateFrom = null;

                if (WeekTo.HasValue && YearOfWeek.HasValue)
                {
                    CultureInfo myCI = new CultureInfo("cs-CZ");
                    Calendar myCal = myCI.Calendar;

                    SelectedDateTo = myCal.AddWeeks(new DateTime((int)YearOfWeek, 1, 2), (int)WeekTo);
                }
                else
                    SelectedDateTo = null;
            }

            if (IsWeekSelected)
            {
                if (WeekFrom.HasValue && YearOfWeek.HasValue)
                    productions = productions.Where(a => a.Week >= WeekFrom && a.Date.Year == YearOfWeek);

                if (WeekTo.HasValue && YearOfWeek.HasValue)
                    productions = productions.Where(a => a.Week <= WeekTo && a.Date.Year == YearOfWeek);
            }
            else
            {
                if (SelectedDateFrom.HasValue)
                    productions = productions.Where(a => a.Date >= SelectedDateFrom);

                if (SelectedDateTo.HasValue)
                    productions = productions.Where(a => a.Date <= SelectedDateTo);
            }

            //var prss = (from ppp in productions
            //           orderby ppp.Date
            //           group ppp by ppp.ItemNumber into selp
            //           select selp).ToList();

            //productions = productions.Where(a=>a.ProductionOrder == 

            var prsng = productions.OrderBy(a => a.Date).ToList();

            var prss = prsng.GroupBy(a => a.ItemNumber);

            var listStands = COSContext.Current.Standards.ToList();

            StandardExportItem stand = null;
            foreach (var prodG in prss)
            {
                var prodWC = prodG.GroupBy(a => a.WorkCenter.Value);

                foreach (var prdWC in prodWC)
                {
                    HourlyProduction prod = prdWC.FirstOrDefault();

                    if (!prod.ProductionOrder.ToLower().Equals("np") && !prod.ProductionOrder.ToLower().Equals("pm"))
                    {

                        var tempstand = listStands.FirstOrDefault(a => a.ID_Standard == prod.hlp_ID_Standard);

                        if (tempstand == null)
                            UnexistStandardHPs.Add(prod);

                        if (tempstand == null && ExportDataUnexists.Where(a => a.WorkCenter == prod.WorkCenter.Value && a.WorkGroup == prod.WorkGroup.Value && a.ItemNumber == prod.ItemNumber).Count() == 0 && !prod.ItemNumber.ToLower().Equals("np") && !prod.ItemNumber.ToLower().Equals("pm"))
                        {
                            stand = new StandardExportItem();
                            stand.WorkCenter = prod.WorkCenter.Value;
                            stand.WorkGroup = prod.WorkGroup.Value;
                            stand.ItemNumber = prod.ItemNumber;
                            ExportDataUnexists.Add(stand);
                        }
                    }
                }
            }

        }
    }
}

