using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using COS.Application.Production.Views;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;
using System.Transactions;

namespace COS.Application.Production.Models
{
    public partial class HourlyProductionMainViewModel : ValidationViewModelBase
    {
        public HourlyProductionMainViewModel()
            : base()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(HourlyProductionMainViewModel_PropertyChanged);


        }

        void HourlyProductionMainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDivision")
            {
                if (SelectedDivision != null)
                {
                    LocalWorkGroups = COSContext.Current.WorkGroups.Where(a => a.ID_Division == SelectedDivision.ID).OrderBy(a => a.Value).ToList();
                    LocalDowntimes = COSContext.Current.Downtimes.Where(a => a.ID_Division == SelectedDivision.ID).ToList().OrderBy(a => a.Description).ToList();
                    LocalOperators = COSContext.Current.Employees.Where(a => a.HR_ID != null && (a.LeaveDate > DateTime.Today || a.LeaveDate == null)).OrderBy(a => a.Surname).ToList();
                }
            }
            else if (e.PropertyName == "SelectedWorkGroup")
            {
                if (SelectedWorkGroup != null)
                {
                    LocalWorkCenters = COSContext.Current.WorkGroupsWorkCenters.Where(a => a.ID_WorkGroup == SelectedWorkGroup.ID).Select(a => a.WorkCenter).OrderBy(a => a.Value).ToList();

                    if (LocalWorkCenters.Count > 0)
                        SelectedWorkCenter = LocalWorkCenters.FirstOrDefault();
                }
            }
            else if (e.PropertyName == "SelectedShiftType")
            {
                //LocalShiftPatterns  = COSContext.Current.ShiftPatterns.Where(a => a.ID_ShiftType == SelectedShiftType.ID).ToList();
            }
            else if (e.PropertyName == "SelectedWorkCenter")
            {
                if (SelectedWorkCenter != null)
                {
                    ClearAll();

                    EditingMode = EditMode.New;

                    LocalHourlyProductions = COSContext.Current.HourlyProductions.Where(a => a.ID_ShiftType == SelectedShiftType.ID && a.ID_Shift == SelectedShift.ID &&
                        a.Date == SelectedDate && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ID_WorkCenter == SelectedWorkCenter.ID).OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).ToList();


                    if (LocalHourlyProductions.Count > 0)
                    {
                        SelectedHourlyProduction = LocalHourlyProductions.OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).FirstOrDefault();

                        ProductionAssets = COSContext.Current.ProductionAssets.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                        ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                    }
                    else
                    {
                        //když count == 0 tak je něco enbled něco disabled.....

                        ClearAll();
                    }
                }
                else
                {
                    EditingMode = EditMode.View;
                }
            }
        }

        private void ClearAll()
        {


            SelectedOrderInfo = null;
            LocalHourlyProductions = null;
            SelectedHourlyProduction = null;
            ItemDescription = null;
            ProductionAssets = null;
            ProductionHRResources = null;
            ProductionAssets = new List<ProductionAsset>();
            ProductionHRResources = new List<ProductionHRResource>();

            COSContext.Current.RejectChanges();
        }

        void SelectedHourlyProduction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemNumber")
            {
                string itemnumber = SelectedHourlyProduction.ItemNumber.Trim();
                string workcentervalue = SelectedWorkCenter.Value;

                string inwc = workcentervalue + itemnumber;

                var stand = COSContext.Current.Standards.Where(a => a.ID_Standard == inwc).FirstOrDefault();

                if (stand == null && SelectedHourlyProduction.IsConfig)
                {
                    var addedStand = COSContext.Current.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Where(a => a.Entity.GetType() == typeof(Standard));

                    if (addedStand.FirstOrDefault() != null)
                        stand = (Standard)addedStand.FirstOrDefault().Entity;
                }

                if (stand != null)
                {
                    ItemDescription = stand.ItemDescription;
                    SelectedHourlyProduction.StdLabour = stand.Labour;
                    SelectedHourlyProduction.StdPiecesPerHour = stand.PcsPerHour;
                }
                else if (SelectedHourlyProduction.IsConfig)
                {
                    OnPropertyChanged("ConfigStandard");
                }

                if (stand == null)
                    ItemDescription = "";

                //if (SelectedOrderInfo != null)
                //{
                //    SelectedOrderInfo.ItemNumber = SelectedHourlyProduction.ItemNumber;
                //}

            }
            else if (e.PropertyName == "ProductionOrder")
            {
                //if (SelectedHourlyProduction != null)
                //{
                //    var soi = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ProductionOrder == SelectedHourlyProduction.ProductionOrder && a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID);

                //    if (soi != null)
                //        SelectedOrderInfo = soi;

                //    if (SelectedOrderInfo == null)
                //    {

                //        SelectedOrderInfo = new ProductionOrderInfo();
                //        if (SelectedHourlyProduction.ItemNumber != null)
                //            SelectedOrderInfo.ItemNumber = SelectedHourlyProduction.ItemNumber;
                //        SelectedOrderInfo.WorkCenter = SelectedWorkCenter;
                //        SelectedOrderInfo.WorkGroup = SelectedWorkGroup;

                //    }
                //    SelectedOrderInfo.ProductionOrder = SelectedHourlyProduction.ProductionOrder;
                //}
            }
            else if (e.PropertyName == "IsConfig")
            {
                if (SelectedHourlyProduction != null && SelectedHourlyProduction.IsConfig)
                {
                    if (SelectedHourlyProduction.IsConfig && SelectedHourlyProduction.ItemNumber != null)
                    {
                        string itemnumber = SelectedHourlyProduction.ItemNumber.Trim();
                        string workcentervalue = SelectedWorkCenter.Value;

                        string inwc = workcentervalue + itemnumber;

                        var stand = COSContext.Current.Standards.Where(a => a.ID_Standard == inwc).FirstOrDefault();
                        if (stand == null)
                        {
                            var addedStand = COSContext.Current.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Where(a => a.Entity.GetType() == typeof(Standard));
                            if (addedStand.FirstOrDefault() != null)
                                stand = (Standard)addedStand.FirstOrDefault().Entity;
                        }

                        if (stand == null)
                        {
                            OnPropertyChanged("ConfigStandard");
                        }
                    }
                }
            }
        }

        public ICommand SetCommand
        {
            get
            {
                return new RelayCommand(param => this.SetUnset());
            }
        }

        public ICommand AddNewHP
        {
            get
            {
                return new RelayCommand(param => this.AddNewHourlyProduction());

            }
        }


        public ICommand DeleteHP
        {
            get
            {
                return new RelayCommand(param => this.DeleteHourlyProduction());
            }
        }

        private void DeleteHourlyProduction()
        {
            if (SelectedHourlyProduction != null)
            {
                SelectedHourlyProduction.Changed = true;
                SelectedHourlyProduction.ChangedDate = COSContext.Current.DateTimeServer;

                try
                {

                    COS.Application.Shared.TM.Helper.ProcessTool(Shared.TM.Helper.ProcessType.Delete, SelectedHourlyProduction);

                    List<ProductionHRResource> hrtodel = new List<ProductionHRResource>();
                    foreach (var prodhr in COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP))
                    {
                        hrtodel.Add(prodhr);
                    }
                    foreach (var itm in hrtodel)
                    {
                        COSContext.Current.ProductionHRResources.DeleteObject(itm);
                    }

                    List<ProductionAsset> asstodel = new List<ProductionAsset>();
                    foreach (var assethr in COSContext.Current.ProductionAssets.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP))
                    {
                        asstodel.Add(assethr);
                    }
                    foreach (var itm in asstodel)
                    {
                        COSContext.Current.ProductionAssets.DeleteObject(itm);
                    }

                    COSContext.Current.HourlyProductions.DeleteObject(SelectedHourlyProduction);

                    if (SelectedOrderInfo != null)
                    {
                        if (COSContext.Current.HourlyProductions.Except(COSContext.Current.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Deleted).OfType<HourlyProduction>())
                            .Where(a => a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ProductionOrder == SelectedOrderInfo.ProductionOrder && a.ID_HP != SelectedHourlyProduction.ID_HP).Count() == 0)
                        {
                            var prodOrder = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ProductionOrder == SelectedOrderInfo.ProductionOrder && a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID);
                            if (prodOrder != null)
                                COSContext.Current.ProductionOrderInfos.DeleteObject(prodOrder);
                        }
                        else
                        {
                            SelectedOrderInfo.ProducedPieces -= SelectedHourlyProduction.ProducedPieces;
                        }
                    }



                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                    LocalHourlyProductions = COSContext.Current.HourlyProductions.Where(a => a.ID_ShiftType == SelectedShiftType.ID && a.ID_Shift == SelectedShift.ID &&
                        a.Date == SelectedDate && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ID_WorkCenter == SelectedWorkCenter.ID).ToList();


                    if (LocalHourlyProductions != null && LocalHourlyProductions.Count > 0)
                    {
                        SelectedHourlyProduction = LocalHourlyProductions.OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).FirstOrDefault();
                        ProductionAssets = COSContext.Current.ProductionAssets.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                        ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                    }
                    else
                    {
                        ClearAll();
                    }
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail, COSContext.Current.ExceptionInfo);

                    COSContext.Current.RejectChanges();

                    ClearAll();

                    LocalError = ResourceHelper.GetResource<string>("51ErrorCont");
                }

                OnPropertyChanged("DeleteHP");
            }
        }

        public ICommand DeleteAllHP
        {
            get
            {
                return new RelayCommand(param => this.DeleteAllHourlyProductions());
            }
        }

        private void DeleteAllHourlyProductions()
        {
            if (SelectedHourlyProduction != null)
            {
                SelectedHourlyProduction.Changed = true;
                SelectedHourlyProduction.ChangedDate = COSContext.Current.DateTimeServer;

                try
                {
                    COSContext.Current.RejectChanges();

                    List<string> prodCodes = new List<string>();
                    foreach (var prod in LocalHourlyProductions)
                    {
                        COS.Application.Shared.TM.Helper.ProcessTool(Shared.TM.Helper.ProcessType.Delete, prod);

                        if (!prodCodes.Contains(prod.ProductionOrder))
                            prodCodes.Add(prod.ProductionOrder);

                        List<ProductionHRResource> hrtodel = new List<ProductionHRResource>();
                        foreach (var prodhr in COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == prod.ID_HP))
                        {
                            hrtodel.Add(prodhr);
                        }
                        foreach (var itm in hrtodel)
                        {
                            COSContext.Current.ProductionHRResources.DeleteObject(itm);
                        }

                        List<ProductionAsset> asstodel = new List<ProductionAsset>();
                        foreach (var assethr in COSContext.Current.ProductionAssets.Where(a => a.ID_HP == prod.ID_HP))
                        {
                            asstodel.Add(assethr);
                        }
                        foreach (var itm in asstodel)
                        {
                            COSContext.Current.ProductionAssets.DeleteObject(itm);
                        }

                        var prodOrder = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ProductionOrder == prod.ProductionOrder && a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID);
                        if (prodOrder != null)
                        {
                            prodOrder.ProducedPieces -= prod.ProducedPieces;
                        }

                        COSContext.Current.HourlyProductions.DeleteObject(prod);


                        //if (COSContext.Current.HourlyProductions.Except(COSContext.Current.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Deleted).OfType<HourlyProduction>())
                        //    .Where(a => a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ProductionOrder == SelectedOrderInfo.ProductionOrder && a.ID_HP != prod.ID_HP).Count() == 0)
                        //{
                        //    var prodOrder = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ProductionOrder == SelectedOrderInfo.ProductionOrder && a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID);

                        //    COSContext.Current.ProductionOrderInfos.DeleteObject(prodOrder);
                        //}
                        //else
                        //{
                        //    var prodOrder = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ProductionOrder == prod.ProductionOrder && a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID);
                        //    if (prodOrder != null)
                        //    {
                        //        prodOrder.ProducedPieces -= prod.ProducedPieces;
                        //    }
                        //}
                    }

                    foreach (var itm in prodCodes)
                    {

                        List<HourlyProduction> temprods = new List<HourlyProduction>();

                        foreach (var pit in COSContext.Current.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Deleted))
                        {
                            if (pit.Entity is HourlyProduction)
                                temprods.Add(pit.Entity as HourlyProduction);
                        }

                        if (COSContext.Current.HourlyProductions.Where(a => a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ProductionOrder == itm).ToList().Except(temprods).Count() == 0)
                        {
                            var prodOrder = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ProductionOrder == itm && a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID);
                            if (prodOrder != null)
                                COSContext.Current.ProductionOrderInfos.DeleteObject(prodOrder);
                        }

                    }

                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                    LocalHourlyProductions = COSContext.Current.HourlyProductions.Where(a => a.ID_ShiftType == SelectedShiftType.ID && a.ID_Shift == SelectedShift.ID &&
                        a.Date == SelectedDate && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ID_WorkCenter == SelectedWorkCenter.ID).ToList();


                    if (LocalHourlyProductions.Count > 0)
                    {
                        SelectedHourlyProduction = LocalHourlyProductions.OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).FirstOrDefault();
                        ProductionAssets = COSContext.Current.ProductionAssets.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                        ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                    }
                    else
                    {
                        ClearAll();
                    }
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail, COSContext.Current.ExceptionInfo);
                    COSContext.Current.RejectChanges();

                    ClearAll();

                    LocalError = ResourceHelper.GetResource<string>("m_Body_PROD00000001") + Error;
                }
            }
        }




        public ICommand Save
        {
            get
            {
                return new RelayCommand(param => this.SaveHP(true));
            }
        }

        private bool SaveHP(bool refresh)
        {

            bool result = false;
            if (SelectedHourlyProduction != null)
            {
                try
                {
                    if (SelectedHourlyProduction.EntityState == System.Data.EntityState.Detached)
                    {
                        SelectedHourlyProduction.EntityKey = COSContext.Current.CreateEntityKey("HourlyProductions", SelectedHourlyProduction);
                        COSContext.Current.Attach(SelectedHourlyProduction);
                        COSContext.Current.ObjectStateManager.ChangeObjectState(SelectedHourlyProduction, System.Data.EntityState.Added);
                    }

                    //validace dat
                    string err = "";
                    if (ValivHP(out err))
                    {


                        //vytahnutí standardu
                        string itemnumber = SelectedHourlyProduction.ItemNumber.Trim();
                        string workcentervalue = SelectedWorkCenter.Value;

                        string inwc = workcentervalue + itemnumber;


                        var stand = COSContext.Current.Standards.Where(a => a.ID_Standard == inwc).FirstOrDefault();

                        if (stand == null && SelectedHourlyProduction.IsConfig)
                        {
                            var addedStand = COSContext.Current.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Where(a => a.Entity.GetType() == typeof(Standard));
                            var stdSet = addedStand.FirstOrDefault();
                            if (stdSet != null)
                                stand = (Standard)stdSet.Entity;
                        }


                        var soi = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ProductionOrder == SelectedHourlyProduction.ProductionOrder && a.ID_WorkCenter == SelectedWorkCenter.ID && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ItemNumber == SelectedHourlyProduction.ItemNumber);

                        if (soi != null)
                            SelectedOrderInfo = soi;
                        else
                        {
                            if (SelectedOrderInfo != null)
                                COSContext.Current.ProductionOrderInfos.DeleteObject(SelectedOrderInfo);

                            SelectedOrderInfo = COSContext.Current.ProductionOrderInfos.CreateObject();
                            //COSContext.Current.ProductionOrderInfos.AddObject(SelectedOrderInfo);

                            SelectedOrderInfo.ItemNumber = SelectedHourlyProduction.ItemNumber;
                            SelectedOrderInfo.WorkCenter = SelectedWorkCenter;
                            SelectedOrderInfo.WorkGroup = SelectedWorkGroup;
                            SelectedOrderInfo.ProductionOrder = SelectedHourlyProduction.ProductionOrder;
                        }

                        SelectedOrderInfo.PlannedPieces = OrderInfoPlannedPieces;

                        //hodnoty pro vytahnutí ze standardu - s inicializovanými hodnotami, když standard neexistuje...
                        decimal PcsPerHour = 0;
                        int stdLabour = 0;
                        decimal weighPcs = 0;
                        int? totalPlanned = null;
                        if (stand != null)
                        {
                            if (SelectedOrderInfo != null && SelectedOrderInfo.ID == 0)
                                SelectedOrderInfo.PlannedSetupTime = stand.SetupTime_mm;

                            totalPlanned = stand.SetupTime_mm;
                            PcsPerHour = stand.PcsPerHour;
                            stdLabour = stand.Labour;
                            weighPcs = stand.Weight_Kg.HasValue ? stand.Weight_Kg.Value : 0;
                            SelectedHourlyProduction.StdLabour = stdLabour;
                        }

                        //přičtení hodnot k aktuálnímu OrderInfo - kusy a setup time
                        if (SelectedHourlyProduction.ID > 0)
                        {
                            try
                            {
                                decimal origVal = (decimal)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).OriginalValues["ProducedPieces"];
                                decimal curVal = (decimal)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).CurrentValues["ProducedPieces"];

                                if (SelectedOrderInfo != null)
                                {
                                    SelectedOrderInfo.ProducedPieces -= origVal;
                                    SelectedOrderInfo.ProducedPieces += curVal;

                                    int origValint = 0;
                                    if (COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).State != System.Data.EntityState.Added)
                                        origValint = (int)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).OriginalValues["hlpActualSetupTime"];
                                    int curValint = (int)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).CurrentValues["hlpActualSetupTime"];


                                    if (SelectedOrderInfo != null)
                                        SelectedOrderInfo.SetupTime += (curValint - origValint);

                                }
                            }
                            catch (Exception exc)
                            {
                                Logging.LogException(exc, LogType.ToFileAndEmail, COSContext.Current.ExceptionInfo);
                            }
                        }
                        else
                        {
                            if (SelectedOrderInfo != null)
                            {
                                SelectedOrderInfo.ProducedPieces += SelectedHourlyProduction.ProducedPieces;
                                int origVal = 0;

                                if (COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).State != System.Data.EntityState.Added)
                                    origVal = (int)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).OriginalValues["hlpActualSetupTime"];
                                int curVal = (int)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).CurrentValues["hlpActualSetupTime"];

                                SelectedOrderInfo.SetupTime += curVal - origVal;
                            }
                        }

                        //int tempSetupTime = 0;
                        //int tempHRSetupTime = 0;

                        int aorigVal = 0;
                        if (COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).State != System.Data.EntityState.Added)
                            aorigVal = (int)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).OriginalValues["hlpActualSetupTime"];
                        int acurVal = (int)COSContext.Current.ObjectStateManager.GetObjectStateEntry(SelectedHourlyProduction).CurrentValues["hlpActualSetupTime"];


                        //rozdělení setup time - pro další setup a downtime
                        if (SelectedOrderInfo != null)
                        {
                            if ((acurVal - aorigVal) > 0)
                            {
                                //if (totalPlanned.HasValue && SelectedOrderInfo.SetupTime > totalPlanned)
                                //{                                   
                                if (SelectedOrderInfo.PlannedSetupTime > 0 && SelectedHourlyProduction.hlpOutSetupTime > 0)
                                {
                                    SelectedHourlyProduction.hlpOutSetupTime = 0;
                                }

                                if (SelectedHourlyProduction.hlpOutSetupTime > 0)
                                {
                                    SelectedHourlyProduction.hlpOutSetupTime += (acurVal - aorigVal);
                                    SelectedOrderInfo.PlannedSetupTime = 0;
                                }
                                else
                                {
                                    if (SelectedOrderInfo.PlannedSetupTime > 0)
                                    {
                                        if (SelectedOrderInfo.PlannedSetupTime >= (acurVal - aorigVal))
                                        {
                                            SelectedHourlyProduction.hlpInSetupTime += (acurVal - aorigVal);
                                            SelectedOrderInfo.PlannedSetupTime -= (acurVal - aorigVal);
                                        }
                                        else
                                        {
                                            int rest = (acurVal - aorigVal) - SelectedOrderInfo.PlannedSetupTime;

                                            SelectedHourlyProduction.hlpInSetupTime += SelectedOrderInfo.PlannedSetupTime;
                                            SelectedHourlyProduction.hlpOutSetupTime += rest;

                                            SelectedOrderInfo.PlannedSetupTime = 0;
                                        }
                                    }
                                    else
                                    {
                                        SelectedHourlyProduction.hlpOutSetupTime += (acurVal - aorigVal);
                                    }
                                }
                                //}
                                //else if (totalPlanned.HasValue)
                                //{
                                //    SelectedOrderInfo.PlannedSetupTime -= (acurVal - aorigVal);
                                //    SelectedHourlyProduction.hlpInSetupTime = SelectedHourlyProduction.hlpActualSetupTime;
                                //}
                            }
                            else if ((acurVal - aorigVal) < 0)
                            {
                                if (SelectedHourlyProduction.hlpOutSetupTime + (acurVal - aorigVal) < 0)
                                {
                                    int rest = SelectedHourlyProduction.hlpOutSetupTime + (acurVal - aorigVal);

                                    SelectedHourlyProduction.hlpOutSetupTime = 0;
                                    SelectedHourlyProduction.hlpInSetupTime += rest;
                                    if (SelectedHourlyProduction.hlpInSetupTime < 0)
                                    {
                                        SelectedHourlyProduction.hlpInSetupTime = 0;
                                    }

                                    SelectedOrderInfo.PlannedSetupTime -= rest;
                                }
                                else
                                {
                                    SelectedHourlyProduction.hlpOutSetupTime += (acurVal - aorigVal);
                                }
                            }

                            var sumSetups = COSContext.Current.HourlyProductions.Where(a => a.ProductionOrder == SelectedHourlyProduction.ProductionOrder && a.ID_WorkCenter == SelectedHourlyProduction.ID_WorkCenter && a.ID_WorkGroup == SelectedHourlyProduction.ID_WorkGroup && a.ID != SelectedHourlyProduction.ID);

                            int sums = 0;
                            if (sumSetups.Count() > 0)
                            {
                                sums = sumSetups.Sum(a => a.hlpOutSetupTime + a.hlpInSetupTime);
                                sums += SelectedHourlyProduction.hlpOutSetupTime + SelectedHourlyProduction.hlpInSetupTime;

                                var allrest = SelectedOrderInfo.SetupTime - sums;

                                if (allrest > 0)
                                {
                                    SelectedHourlyProduction.hlpOutSetupTime += allrest;
                                }
                            }

                        }




                        //dopočítávání hodnot - počítá se se vzorci z databáze
                        //nastavování proměných pro příslušné vzorce
                        Dictionary<string, double> values = new Dictionary<string, double>();

                        values.Add("A", 1.0);
                        values.Add("ActualTime", SelectedHourlyProduction.ActualTime_min);
                        values.Add("StdPcsHour", (double)PcsPerHour);
                        values.Add("RunFactor", (double)SelectedWorkCenter.RunFactor_Perc);
                        values.Add("TotalLabour", (double)(SelectedHourlyProduction.LabourOwn + SelectedHourlyProduction.LabourTemp));

                        SelectedHourlyProduction.LabourTotal = SelectedHourlyProduction.LabourOwn + SelectedHourlyProduction.LabourTemp;

                        SelectedHourlyProduction.DowntimeTime_min = ProductionAssets.Where(a => a.Downtime.IsLabourAffection == false).Sum(a => a.Time_min) + SelectedHourlyProduction.hlpOutSetupTime;
                        SelectedHourlyProduction.HrDowntimeTime_min = ProductionAssets.Where(a => a.Downtime.IsLabourAffection == true).Sum(a => a.Time_min) + SelectedHourlyProduction.hlpInSetupTime;
                        SelectedHourlyProduction.StdPiecesPerHour = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("StdPiecesPerHour", values), 2);

                        values.Add("hrDowntime", (double)SelectedHourlyProduction.HrDowntimeTime_min);
                        values.Add("Downtime", (double)SelectedHourlyProduction.DowntimeTime_min);
                        SelectedHourlyProduction.ActOperationalTime_min = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("ActOperationalTime_min", values), 2);

                        values.Add("StdPcsHourCalculated", (double)SelectedHourlyProduction.StdPiecesPerHour);
                        SelectedHourlyProduction.ActIdealTaktTime_min = (decimal)COS.Common.WPF.Helpers.CalculateFunction("ActIdealTaktTime_min", values);

                        values.Add("ProducedPcs", (double)SelectedHourlyProduction.ProducedPieces);
                        values.Add("OperationalTime", (double)SelectedHourlyProduction.ActOperationalTime_min);
                        SelectedHourlyProduction.ActPiecesPerHeadHour = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("ActPiecesPerHeadHour", values), 2);

                        values.Add("StdLabour", stdLabour);
                        SelectedHourlyProduction.StdPiecesPerHeadHour = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("StdPiecesPerHeadHour", values), 2);

                        values.Add("ScrapPcs", SelectedHourlyProduction.ScrapPieces);
                        SelectedHourlyProduction.KpiQuality = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiQuality", values), 2);

                        values.Add("actIdealTime", (double)SelectedHourlyProduction.ActIdealTaktTime_min);
                        SelectedHourlyProduction.KpiPerformance = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiPerformance", values), 2);

                        SelectedHourlyProduction.KpiAvailability = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiAvailability", values), 2);

                        SelectedHourlyProduction.KpiProductivity = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("KpiProductivity", values), 2);

                        values.Add("KpiProductivity", (double)SelectedHourlyProduction.KpiProductivity);
                        SelectedHourlyProduction.hlpHrProductivity = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("hlpHrProductivity", values), 2);

                        values.Add("KpiPerformance", (double)SelectedHourlyProduction.KpiPerformance);
                        SelectedHourlyProduction.hlpPerformance = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("hlpPerformance", values), 2);

                        values.Add("WeighPcs", (double)weighPcs);
                        SelectedHourlyProduction.ScrapCountedWeigh = Math.Round((decimal)COS.Common.WPF.Helpers.CalculateFunction("ScrapWeigh", values), 2);


                        //ukládání/mazaní aktuálních operátorů a downtimů
                        foreach (var oper in ProductionHRResources)
                        {
                            if (oper.ID == 0)
                            {
                                oper.ID_HP = SelectedHourlyProduction.ID_HP;
                                COSContext.Current.ProductionHRResources.AddObject(oper);
                            }
                        }

                        List<ProductionHRResource> prodHRToDelete = new List<ProductionHRResource>();

                        foreach (var prodhr in COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP))
                        {
                            if (ProductionHRResources.Where(a => a.ID == prodhr.ID).Count() == 0)
                                prodHRToDelete.Add(prodhr);
                        }
                        foreach (var todel in prodHRToDelete)
                        {
                            COSContext.Current.ProductionHRResources.DeleteObject(todel);
                        }


                        foreach (var asset in ProductionAssets)
                        {
                            if (asset.ID == 0)
                            {
                                asset.ID_HP = SelectedHourlyProduction.ID_HP;
                                COSContext.Current.ProductionAssets.AddObject(asset);
                            }
                        }

                        List<ProductionAsset> assetsToDelete = new List<ProductionAsset>();

                        foreach (var assethr in COSContext.Current.ProductionAssets.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP))
                        {
                            if (ProductionAssets.Where(a => a.ID == assethr.ID).Count() == 0)
                                assetsToDelete.Add(assethr);
                        }
                        foreach (var todel in assetsToDelete)
                        {
                            COSContext.Current.ProductionAssets.DeleteObject(todel);
                        }


                        SelectedHourlyProduction.hlp_ID_Standard = SelectedWorkCenter.Value + SelectedHourlyProduction.ItemNumber.Trim();

                        if (SelectedOrderInfo != null)
                        {
                            if (SelectedOrderInfo.ID == 0)
                                COSContext.Current.ProductionOrderInfos.AddObject(SelectedOrderInfo);

                        }

                        if (SelectedHourlyProduction.ID == 0)
                        {
                            COSContext.Current.HourlyProductions.AddObject(SelectedHourlyProduction);
                            COS.Application.Shared.TM.Helper.ProcessTool(Shared.TM.Helper.ProcessType.Insert, SelectedHourlyProduction);
                        }
                        else
                        {
                            COS.Application.Shared.TM.Helper.ProcessTool(Shared.TM.Helper.ProcessType.Update, SelectedHourlyProduction);
                        }

                        SelectedHourlyProduction.Changed = true;
                        SelectedHourlyProduction.ChangedDate = COSContext.Current.DateTimeServer;

                        using (TransactionScope trans = new TransactionScope())
                        {
                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            trans.Complete();
                        }

                        if (refresh)
                        {
                            //COSContext.Current.Refresh(System.Data.Objects.RefreshMode.StoreWins, COSContext.Current.ProductionOrderInfos);

                            Guid gid = SelectedHourlyProduction.ID_HP;
                            LocalHourlyProductions = COSContext.Current.HourlyProductions.Where(a => a.ID_ShiftType == SelectedShiftType.ID && a.ID_Shift == SelectedShift.ID &&
                                a.Date == SelectedDate && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ID_WorkCenter == SelectedWorkCenter.ID).OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).ToList();


                            if (LocalHourlyProductions.Count > 0)
                            {
                                SelectedHourlyProduction = LocalHourlyProductions.FirstOrDefault(a => a.ID_HP == gid);
                                //SelectedHourlyProduction = LocalHourlyProductions.OrderByDescending(a=> a.ID_Hour).OrderByDescending(a =>a.ActualTime_min).FirstOrDefault();
                                //ProductionAssets = COSContext.Current.ProductionAssets.Where(a=> a.ID_HP ==SelectedHourlyProduction.ID_HP).ToList();
                                //ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a=> a.ID_HP ==SelectedHourlyProduction.ID_HP).ToList();
                            }
                        }
                        result = true;

                        OnPropertyChanged("Saved");
                    }
                    else
                    {
                        LocalError = err;
                    }
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail, null);
                    COSContext.Current.RejectChanges();

                    ClearAll();

                    LocalError = ResourceHelper.GetResource<string>("m_Body_PROD00000001");
                }
            }

            return result;


        }

        private bool ValivHP(out string err)
        {
            bool result = false;
            err = "";

            if (SelectedHourlyProduction != null)
            {
                result = true;
                if (SelectedHourlyProduction.ID_Hour < 1)
                {
                    result = false;
                    err += ResourceHelper.GetResource<string>("m_Body_PROD00000002");
                    err += Environment.NewLine;
                }

                if (string.IsNullOrEmpty(SelectedHourlyProduction.ProductionOrder))
                {
                    result = false;
                    err += ResourceHelper.GetResource<string>("m_Body_PROD00000003");
                    err += Environment.NewLine;
                }

                if (string.IsNullOrEmpty(SelectedHourlyProduction.ItemNumber))
                {
                    result = false;
                    err += ResourceHelper.GetResource<string>("m_Body_PROD00000004");
                    err += Environment.NewLine;
                }

                //if (SelectedOrderInfo != null && SelectedOrderInfo.PlannedPieces < 1)
                //{
                //    result = false;
                //    err += ResourceHelper.GetResource<string>("50ErrorCont");
                //    err += Environment.NewLine;
                //}
            }
            return result;
        }

        private void AddNewHourlyProduction()
        {
            COSContext.Current.RejectChanges();

            ItemDescription = null;
            ProductionAssets = null;
            ProductionHRResources = null;
            SelectedOrderInfo = null;

            SelectedHourlyProduction = COSContext.Current.HourlyProductions.CreateObject();
            COSContext.Current.HourlyProductions.AddObject(SelectedHourlyProduction);

            //SelectedHourlyProduction.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectedHourlyProduction_PropertyChanged);

            SelectedHourlyProduction.WorkCenter = SelectedWorkCenter;
            SelectedHourlyProduction.WorkGroup = SelectedWorkGroup;
            SelectedHourlyProduction.Division = SelectedDivision;
            SelectedHourlyProduction.Date = SelectedDate;
            SelectedHourlyProduction.Shift = SelectedShift;
            SelectedHourlyProduction.ShiftType = SelectedShiftType;


            CultureInfo myCI = new CultureInfo("cs-CZ");
            Calendar myCal = myCI.Calendar;

            SelectedHourlyProduction.Week = myCal.GetWeekOfYear(SelectedHourlyProduction.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            SelectedHourlyProduction.ID_User = COSContext.Current.CurrentUser.ID;
            SelectedHourlyProduction.ID_HP = Guid.NewGuid();

            ProductionAssets = new List<ProductionAsset>();
            ProductionHRResources = new List<ProductionHRResource>();

            OnPropertyChanged("AddNew");
        }



        private bool _isSet = false;
        public bool IsSet
        {
            set
            {
                _isSet = value;
                OnPropertyChanged("IsSet");
            }
            get
            {
                return _isSet;
            }
        }
        private string _isSetText = ResourceHelper.GetResource<string>("prod_SetButton");
        public string IsSetText
        {
            set
            {
                _isSetText = value;
                OnPropertyChanged("IsSetText");
            }
            get
            {
                return _isSetText;
            }
        }

        private string _itemDescription = "";
        public string ItemDescription
        {
            set
            {
                _itemDescription = value;
                OnPropertyChanged("ItemDescription");
            }
            get
            {
                return _itemDescription;
            }
        }

        private int _OrderInfoPlannedPieces = 0;
        public int OrderInfoPlannedPieces
        {
            set
            {
                _OrderInfoPlannedPieces = value;
                OnPropertyChanged("OrderInfoPlannedPieces");
            }
            get
            {
                return _OrderInfoPlannedPieces;
            }
        }



        public void SetUnset()
        {
            if (!IsSet)
            {

                IsSetText = ResourceHelper.GetResource<string>("prod_SetButton");

                SelectedOrderInfo = null;
                LocalHourlyProductions = null;
                LocalShiftPatterns = null;
                LocalWorkCenters = null;
                LocalWorkGroups = null;
                SelectedDivision = null;
                SelectedShift = null;
                SelectedShiftType = null;
                ItemDescription = null;
                ProductionAssets = null;
                ProductionHRResources = null;
                LocalDowntimes = null;
                SelectedHourlyProduction = null;

                EditingMode = EditMode.View;
            }
            else
            {
                string err;
                if (ValidSet(out err))
                {
                    IsSetText = ResourceHelper.GetResource<string>("prod_UnsetButton");
                    LocalShiftPatterns = COSContext.Current.ShiftPatterns.Where(a => a.ID_ShiftType == SelectedShiftType.ID).ToList();
                }
                else
                {
                    LocalError = err;
                    IsSet = false;
                }
            }
        }

        private string _localError = "";
        public string LocalError
        {
            set
            {
                _localError = value;
                OnPropertyChanged("LocalError");
            }
            get
            {
                return _localError;
            }
        }

        private bool ValidSet(out string error)
        {
            bool result = true;

            string err = null;
            if (SelectedDivision == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000004");
                err += Environment.NewLine;
                result = false;
            }

            if (SelectedShift == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000005");
                err += Environment.NewLine;
                result = false;
            }

            if (SelectedShiftType == null)
            {
                err += ResourceHelper.GetResource<string>("m_Body_PROD00000006");
                err += Environment.NewLine;
                result = false;
            }

            error = err;

            return result;
        }


        public ICommand Cycle1
        {
            get
            {
                return new RelayCommand(param => this.CycleHP1());
            }
        }

        private void CycleHP1()
        {
            if (SaveHP(true))
            {
                var prod = LocalHourlyProductions.OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).FirstOrDefault();

                AddNewHourlyProduction();

                SelectedHourlyProduction.ProductionOrder = prod.ProductionOrder;
                SelectedHourlyProduction.ItemNumber = prod.ItemNumber;
                SelectedHourlyProduction.IsConfig = prod.IsConfig;
                SelectedHourlyProduction.LabourOwn = prod.LabourOwn;
                SelectedHourlyProduction.LabourTemp = prod.LabourTemp;
                SelectedHourlyProduction.hlp_ID_Standard = prod.hlp_ID_Standard;

                //logika poslední hodiny
                
                var curHour = LocalShiftPatterns.FirstOrDefault(a=>a.ID == prod.ID_Hour);
                var nextHour = LocalShiftPatterns.FirstOrDefault(a => a.Hour_24 == curHour.Hour_24 + 1);

                if (nextHour != null)
                    SelectedHourlyProduction.ID_Hour = nextHour.ID;
                else
                    SelectedHourlyProduction.ID_Hour = prod.ID_Hour;


                List<ProductionHRResource> templist = new List<ProductionHRResource>();
                foreach (var itm in COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == prod.ID_HP).ToList())
                {
                    ProductionHRResource res = new ProductionHRResource();
                    res.ID_HP = SelectedHourlyProduction.ID_HP;
                    res.ID_HR = itm.ID_HR;
                    templist.Add(res);
                }

                ProductionHRResources = templist;
                OnPropertyChanged("Cycle1");
            }
        }

        public ICommand Cycle2
        {
            get
            {
                return new RelayCommand(param => this.CycleHP2());
            }
        }

        public ICommand OneHourNPCommand
        {
            get
            {
                return new RelayCommand(param => this.OneHourNP());
            }
        }

        private void OneHourNP()
        {
            if (SelectedHourlyProduction != null)
            {
                SelectedHourlyProduction.ProductionOrder = "NP";
                SelectedHourlyProduction.ItemNumber = "NP";
                SelectedHourlyProduction.ActualTime_min = 0;
                                
                this.ProductionHRResources.Clear();
            }
        }

        public ICommand OneHourPMCommand
        {
            get
            {
                return new RelayCommand(param => this.OneHourPM());
            }
        }

        private void OneHourPM()
        {
            if (SelectedHourlyProduction != null)
            {
                SelectedHourlyProduction.ProductionOrder = "PM";
                SelectedHourlyProduction.ItemNumber = "PM";
                SelectedHourlyProduction.ActualTime_min = 0;

                this.ProductionHRResources.Clear();
            }
        }

        public ICommand ShiftNPCommand
        {
            get
            {
                return new RelayCommand(param => this.ShiftNP());
            }
        }

        private void ShiftNP()
        {
            if (LocalHourlyProductions != null && LocalHourlyProductions.Count > 0 || SelectedHourlyProduction != null)
            {

                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_PROD00000007") + Environment.NewLine + ResourceHelper.GetResource<string>("m_Body_PROD00000008"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
            else
            {
                if (IsSet && SelectedShiftType != null)
                {
                    var hours = COSContext.Current.ShiftPatterns.Where(a => a.ID_ShiftType == SelectedShiftType.ID);

                    foreach (var itm in hours)
                    {
                        HourlyProduction prod = new HourlyProduction();
                        //SelectedHourlyProduction.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectedHourlyProduction_PropertyChanged);

                        prod.WorkCenter = SelectedWorkCenter;
                        prod.WorkGroup = SelectedWorkGroup;
                        prod.Division = SelectedDivision;
                        prod.Date = SelectedDate;
                        prod.Shift = SelectedShift;
                        prod.ShiftType = SelectedShiftType;
                        prod.ProductionOrder = "NP";
                        prod.ItemNumber = "NP";
                        prod.ActualTime_min = 0;

                        prod.hlp_ID_Standard = SelectedWorkCenter.Value + prod.ItemNumber;

                        prod.Week = COSContext.Current.Week;

                        prod.ID_User = COSContext.Current.CurrentUser.ID;
                        prod.ID_HP = Guid.NewGuid();

                        prod.ID_Hour = itm.ID;

                        COSContext.Current.HourlyProductions.AddObject(prod);
                    }

                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                    LocalHourlyProductions = COSContext.Current.HourlyProductions.Where(a => a.ID_ShiftType == SelectedShiftType.ID && a.ID_Shift == SelectedShift.ID &&
                       a.Date == SelectedDate && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ID_WorkCenter == SelectedWorkCenter.ID).ToList();

                    if (LocalHourlyProductions != null && LocalHourlyProductions.Count > 0)
                    {
                        SelectedHourlyProduction = LocalHourlyProductions.OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).FirstOrDefault();
                        ProductionAssets = COSContext.Current.ProductionAssets.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                        ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                    }
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_PROD00000009"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
        }

        public ICommand ShiftPMCommand
        {
            get
            {
                return new RelayCommand(param => this.ShiftPM());
            }
        }

        private void ShiftPM()
        {
            if (LocalHourlyProductions != null && LocalHourlyProductions.Count > 0 || SelectedHourlyProduction != null)
            {
                RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_PROD00000007") + Environment.NewLine + ResourceHelper.GetResource<string>("m_Body_PROD00000008"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
            else
            {
                if (IsSet && SelectedShiftType != null)
                {
                    var hours = COSContext.Current.ShiftPatterns.Where(a => a.ID_ShiftType == SelectedShiftType.ID);

                    foreach (var itm in hours)
                    {
                        HourlyProduction prod = new HourlyProduction();
                        //SelectedHourlyProduction.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectedHourlyProduction_PropertyChanged);

                        prod.WorkCenter = SelectedWorkCenter;
                        prod.WorkGroup = SelectedWorkGroup;
                        prod.Division = SelectedDivision;
                        prod.Date = SelectedDate;
                        prod.Shift = SelectedShift;
                        prod.ShiftType = SelectedShiftType;
                        prod.ProductionOrder = "PM";
                        prod.ItemNumber = "PM";
                        prod.ActualTime_min = 0;

                        prod.hlp_ID_Standard = SelectedWorkCenter.Value + prod.ItemNumber;

                        prod.Week = COSContext.Current.Week;

                        prod.ID_User = COSContext.Current.CurrentUser.ID;
                        prod.ID_HP = Guid.NewGuid();

                        prod.ID_Hour = itm.ID;

                        COSContext.Current.HourlyProductions.AddObject(prod);
                    }

                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

                    LocalHourlyProductions = COSContext.Current.HourlyProductions.Where(a => a.ID_ShiftType == SelectedShiftType.ID && a.ID_Shift == SelectedShift.ID &&
                       a.Date == SelectedDate && a.ID_WorkGroup == SelectedWorkGroup.ID && a.ID_WorkCenter == SelectedWorkCenter.ID).ToList();

                    if (LocalHourlyProductions != null && LocalHourlyProductions.Count > 0)
                    {
                        SelectedHourlyProduction = LocalHourlyProductions.OrderByDescending(a => a.ActualTime_min).OrderByDescending(a => a.ID_Hour).FirstOrDefault();
                        ProductionAssets = COSContext.Current.ProductionAssets.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                        ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == SelectedHourlyProduction.ID_HP).ToList();
                    }
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_Body_PROD00000009"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }
        }




        private void CycleHP2()
        {
            if (SaveHP(true))
            {
                var prod = LocalHourlyProductions.FirstOrDefault(a => a.ID == SelectedHourlyProduction.ID);

                AddNewHourlyProduction();

                SelectedHourlyProduction.ProductionOrder = prod.ProductionOrder;
                SelectedHourlyProduction.ItemNumber = prod.ItemNumber;
                SelectedHourlyProduction.IsConfig = prod.IsConfig;
                SelectedHourlyProduction.LabourOwn = prod.LabourOwn;
                SelectedHourlyProduction.LabourTemp = prod.LabourTemp;
                SelectedHourlyProduction.hlp_ID_Standard = prod.hlp_ID_Standard;
                SelectedHourlyProduction.ID_Hour = prod.ID_Hour;


                List<ProductionHRResource> templist = new List<ProductionHRResource>();
                foreach (var itm in COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == prod.ID_HP).ToList())
                {
                    ProductionHRResource res = new ProductionHRResource();
                    res.ID_HP = SelectedHourlyProduction.ID_HP;
                    res.ID_HR = itm.ID_HR;
                    templist.Add(res);
                }

                ProductionHRResources = templist;
                OnPropertyChanged("Cycle2");
            }
        }

        public ICommand Cycle3
        {
            get
            {
                return new RelayCommand(param => this.CycleHP3());
            }
        }

        private void CycleHP3()
        {
            if (SaveHP(true))
            {
                var prod = LocalHourlyProductions.FirstOrDefault(a => a.ID == SelectedHourlyProduction.ID);

                AddNewHourlyProduction();

                SelectedHourlyProduction.ProductionOrder = prod.ProductionOrder;
                SelectedHourlyProduction.ItemNumber = prod.ItemNumber;
                SelectedHourlyProduction.IsConfig = prod.IsConfig;
                SelectedHourlyProduction.LabourOwn = prod.LabourOwn;
                SelectedHourlyProduction.LabourTemp = prod.LabourTemp;
                SelectedHourlyProduction.hlp_ID_Standard = prod.hlp_ID_Standard;


                var curHour = LocalShiftPatterns.FirstOrDefault(a => a.ID == prod.ID_Hour);
                var nextHour = LocalShiftPatterns.FirstOrDefault(a => a.Hour_24 == curHour.Hour_24 + 1);

                if (nextHour != null)
                    SelectedHourlyProduction.ID_Hour = nextHour.ID;
                else
                    SelectedHourlyProduction.ID_Hour = prod.ID_Hour;

                List<ProductionHRResource> templist = new List<ProductionHRResource>();
                foreach (var itm in COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == prod.ID_HP).ToList())
                {
                    ProductionHRResource res = new ProductionHRResource();
                    res.ID_HP = SelectedHourlyProduction.ID_HP;
                    res.ID_HR = itm.ID_HR;
                    templist.Add(res);
                }

                ProductionHRResources = templist;
                OnPropertyChanged("Cycle3");
            }
        }

        private HourlyProduction _selectedHourlyProduction = null;
        public HourlyProduction SelectedHourlyProduction
        {
            set
            {
                _selectedHourlyProduction = value;

                if (_selectedHourlyProduction != null)
                {
                    EditingMode = EditMode.Edit;
                    _selectedHourlyProduction.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectedHourlyProduction_PropertyChanged);

                    if (_selectedHourlyProduction.ID > 0)
                    {
                        string itemnumber = _selectedHourlyProduction.ItemNumber.Trim();
                        string workcentervalue = SelectedWorkCenter.Value;

                        string inwc = workcentervalue + itemnumber;

                        var stand = COSContext.Current.Standards.Where(a => a.ID_Standard == inwc).FirstOrDefault();

                        if (stand != null)
                        {
                            ItemDescription = stand.ItemDescription;
                        }
                        else ItemDescription = "";

                        SelectedOrderInfo = COSContext.Current.ProductionOrderInfos.FirstOrDefault(a => a.ID_WorkCenter == _selectedHourlyProduction.ID_WorkCenter && a.ID_WorkGroup == _selectedHourlyProduction.ID_WorkGroup && a.ProductionOrder == _selectedHourlyProduction.ProductionOrder && a.ItemNumber == _selectedHourlyProduction.ItemNumber);

                        if (SelectedOrderInfo != null)
                        {
                            OrderInfoPlannedPieces = SelectedOrderInfo.PlannedPieces;
                        }

                        ProductionAssets = COSContext.Current.ProductionAssets.Where(a => a.ID_HP == _selectedHourlyProduction.ID_HP).ToList();
                        ProductionHRResources = COSContext.Current.ProductionHRResources.Where(a => a.ID_HP == _selectedHourlyProduction.ID_HP).ToList();
                    }
                    else
                    {

                    }

                }
                else
                    EditingMode = EditMode.New;

                OnPropertyChanged("SelectedHourlyProduction");
            }
            get
            {
                return _selectedHourlyProduction;
            }
        }

        private ProductionOrderInfo _selectedOrderInfo = null;
        public ProductionOrderInfo SelectedOrderInfo
        {
            set
            {
                _selectedOrderInfo = value;
                OnPropertyChanged("SelectedOrderInfo");
            }
            get
            {
                return _selectedOrderInfo;
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

        private DateTime _selectedDate = COSContext.Current.DateTimeServer.Date;
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

        private List<HourlyProduction> _localHourlyProductions = null;
        public List<HourlyProduction> LocalHourlyProductions
        {
            set
            {
                COSContext.Current.SelectedHourlyProductions = value;
                _localHourlyProductions = value;
                OnPropertyChanged("LocalHourlyProductions");
            }
            get
            {
                if (_localHourlyProductions == null)
                    _localHourlyProductions = new List<HourlyProduction>();
                return _localHourlyProductions;
            }
        }

        private List<ShiftPattern> _localShiftPatterns = null;
        public List<ShiftPattern> LocalShiftPatterns
        {
            set
            {
                _localShiftPatterns = value;
                OnPropertyChanged("LocalShiftPatterns");
            }
            get
            {
                return _localShiftPatterns;
            }
        }

        private List<ProductionAsset> _productionAssets = null;
        public List<ProductionAsset> ProductionAssets
        {
            set
            {
                _productionAssets = value;
                OnPropertyChanged("ProductionAssets");
            }
            get
            {
                return _productionAssets;
            }
        }

        private List<ProductionHRResource> _productionHRResources = null;
        public List<ProductionHRResource> ProductionHRResources
        {
            set
            {
                _productionHRResources = value;
                OnPropertyChanged("ProductionHRResources");
            }
            get
            {
                return _productionHRResources;
            }
        }


        private List<Employee> _localOperators = null;
        public List<Employee> LocalOperators
        {
            set
            {
                _localOperators = value;
                OnPropertyChanged("LocalOperators");
            }
            get
            {
                return _localOperators;
            }
        }

      
        private List<Downtime> _localDowntimes = null;
        public List<Downtime> LocalDowntimes
        {
            set
            {
                _localDowntimes = value;
                OnPropertyChanged("LocalDowntimes");
            }
            get
            {
                return _localDowntimes;
            }
        }

        //private EditModeHPConvertor1 _editModeHPConvertor1 = null;
        //public EditModeHPConvertor1 EditModeHPConvertor1
        //{
        //    set
        //    {
        //        _editModeHPConvertor1 = value;
        //        OnPropertyChanged("EditModeHPConvertor1");
        //    }
        //    get
        //    {
        //        return _editModeHPConvertor1;
        //    }
        //}

        //private EditModeHPConvertor2 _editModeHPConvertor2 = null;
        //public EditModeHPConvertor2 EditModeHPConvertor2
        //{
        //    set
        //    {
        //        _editModeHPConvertor2 = value;
        //        OnPropertyChanged("EditModeHPConvertor2");
        //    }
        //    get
        //    {
        //        return _editModeHPConvertor2;
        //    }
        //}

        //private EditModeHPConvertor3 _editModeHPConvertor3 = null;
        //public EditModeHPConvertor3 EditModeHPConvertor3
        //{
        //    set
        //    {
        //        _editModeHPConvertor3 = value;
        //        OnPropertyChanged("EditModeHPConvertor3");
        //    }
        //    get
        //    {
        //        return _editModeHPConvertor3;
        //    }
        //}


    }

    public class EditModeHPConvertor1 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.Edit)
            {

                if (parameter != null)
                {
                    string param = parameter.ToString();

                    if (param == "Save")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Update");

                        return res;
                    }
                    else if (param == "Delete")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Delete");

                        return res;
                    }
                    else if (param == "Setting1h")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Update");

                        return res;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else if (mode == EditMode.New)
            {
                return false;
            }
            else if (mode == EditMode.View)
            {
                return false;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class EditModeHPConvertor2 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.Edit || mode == EditMode.New)
            {

                if (parameter != null)
                {
                    string param = parameter.ToString();

                    if (param == "AddNew")
                    {
                        bool res = false;

                        res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Insert");

                        return res;
                    }
                    else if (param == "ShiftNP" || param == "ShiftPM")
                    {
                        if (COSContext.Current.SelectedHourlyProductions != null && COSContext.Current.SelectedHourlyProductions.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            bool res = false;

                            res = COS.Common.WPF.Helpers.HasRightForOperation("HourlyProduction", "Insert");

                            return res;
                        }

                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }

            }
            else if (mode == EditMode.View)
            {
                return false;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class EditModeHPConvertor3 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.Edit)
            {
                return true;
            }
            else if (mode == EditMode.New)
            {
                return true;
            }
            else if (mode == EditMode.View)
            {
                return true;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

}