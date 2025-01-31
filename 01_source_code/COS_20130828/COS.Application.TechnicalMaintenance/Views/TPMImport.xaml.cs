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
using Microsoft.Win32;
using COS.Application.Shared;
using System.IO;
using COS.Application.TechnicalMaintenance.Models;

namespace COS.Application.TechnicalMaintenance.Views
{
    /// <summary>
    /// Interaction logic for TPMImport.xaml
    /// </summary>
    public partial class TPMImport : Window
    {
        public TPMImport()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();

                ofd.ShowDialog();

                var eqps = COSContext.Current.Equipments.ToList();
                var lists = COSContext.Current.TpmCheckLists.Where(a => a.IsPreImage == true).ToList();
                var priorities = COSContext.Current.TpmPriorities.ToList();
                var status = COSContext.Current.TpmStatuses.FirstOrDefault(a => a.Code == "P");

                if (!string.IsNullOrEmpty(ofd.FileName))
                {
                    using (COS.Excel.COSExcel excel = new Excel.COSExcel(ofd.FileName))
                    {
                        try
                        {
                            int tpmCount = 0;
                            int eqCount = 0;
                            int totalCountCreated = 0;
                            List<string> notFounded = new List<string>();

                            TpmPlan plan;
                            TpmRecurrencePattern recurr;


                            for (int i = 3; i < 600; i++)
                            {
                                TimeSpan timeFrom;
                                if (TimeSpan.TryParse(excel.GetDataText(1, i, 5).ToString().Trim(), out timeFrom))
                                {

                                    var eqp = excel.GetDataText(1, i, 1);
                                    var chlist = excel.GetDataText(1, i, 2);

                                    Equipment eqpControl = null;
                                    TpmCheckList chL;
                                    TpmPriority prio;

                                    if ((eqp != null && !string.IsNullOrEmpty(eqp.ToString())))
                                    {

                                        var prioX = excel.GetData(1, i, 3);

                                        var timeTo = TimeSpan.Parse(excel.GetDataText(1, i, 6).ToString().Trim());

                                        if (timeTo > timeFrom)
                                        {
                                            var eachWeekNum = int.Parse(excel.GetData(1, i, 8).ToString().Trim());
                                            var dayOfWekk = excel.GetData(1, i, 9).ToString().Trim().ToLower();
                                            DateTime dateStart;

                                            if (DateTime.TryParse(excel.GetDataText(1, i, 10).ToString().Trim(), out dateStart))
                                            {
                                                DateTime dateTo;
                                                if (DateTime.TryParse(excel.GetDataText(1, i, 11).ToString().Trim(), out dateTo))
                                                {


                                                    tpmCount++;
                                                    eqpControl = eqps.FirstOrDefault(a => a.EquipmentName.ToLower().Trim() == eqp.ToString().ToLower().Trim());
                                                   
                                                    chL = lists.FirstOrDefault(a => a.CheckListName.ToLower().Trim() == chlist.ToString().ToLower().Trim());
                                                    prio = priorities.FirstOrDefault(a => a.Code == prioX.ToString().Trim());

                                                    if (chL != null)
                                                        eqCount++;
                                                    else
                                                    {
                                                        notFounded.Add(chlist.ToString());
                                                        excel.HighliteData(1, i);
                                                    }

                                                    if (eqpControl != null)
                                                    {
                                                        eqps.Remove(eqpControl);
                                                        eqCount++;
                                                    }
                                                    else
                                                    {
                                                        notFounded.Add(eqp.ToString());
                                                        excel.HighliteData(1, i);
                                                    }

                                                    if (eqpControl != null && chL != null)
                                                    {
                                                        TpmPlanDetailViewModel model = new TpmPlanDetailViewModel();
                                                        TpmPlan item = new TpmPlan();


                                                        item.TimePlanMode = 1;

                                                        item.TpmStartDateTime = DateTime.Now.Date;
                                                        item.TpmEndDatetime = DateTime.Now.Date;

                                                        model.SelectedItem = item;
                                                        model.EditingMode = Common.EditMode.New;
                                                        model.IsRecurrencyUpdating = null;

                                                        item.Equipment = eqpControl;
                                                        item.CheckList = chL;
                                                        item.TpmStatus = status;
                                                        item.TpmPriority = prio;

                                                        item.TimePlanMode = 2;

                                                        item.Recurrence.isDaily = false;
                                                        item.Recurrence.isWeekly = true;
                                                        item.Recurrence.StartTime = timeFrom;
                                                        item.Recurrence.EndTime = timeTo;
                                                        item.Recurrence.isWeeklyRecursEveryWeek = eachWeekNum;
                                                        item.Recurrence.RangeOfReccStartDate = dateStart;
                                                        item.Recurrence.RangeOfReccEndDate = dateTo;

                                                        switch (dayOfWekk)
                                                        {
                                                            case "pondělí":
                                                                item.Recurrence.isWeeklyMonday = true;
                                                                break;
                                                            case "úterý":
                                                                item.Recurrence.isWeeklyTuesday = true;
                                                                break;
                                                            case "středa":
                                                                item.Recurrence.isWeeklyWednesday = true;
                                                                break;
                                                            case "čtvrtek":
                                                                item.Recurrence.isWeeklyThursday = true;
                                                                break;
                                                            case "pátek":
                                                                item.Recurrence.isWeeklyFriday = true;
                                                                break;
                                                            case "sobota":
                                                                item.Recurrence.isWeeklySaturday = true;
                                                                break;
                                                            case "neděle":
                                                                item.Recurrence.isWeeklySunday = true;
                                                                break;
                                                        }

                                                        model.Save(false);
                                                        totalCountCreated++;

                                                    }
                                                }
                                                else
                                                {
                                                    //špatné datum do
                                                    excel.HighliteData(1, i);
                                                }
                                            }
                                            else
                                            {
                                                //špatně datum od
                                                excel.HighliteData(1, i);
                                            }
                                        }
                                        else
                                        {
                                           // MessageBox.Show("špatný čas: " + eqpControl.DisplayName);
                                            excel.HighliteData(1, i);
                                        }
                                    }
                                }

                            }
                            FileInfo finfo = new FileInfo(ofd.FileName);
                            excel.Save(finfo.FullName.Replace(finfo.Name, finfo.Name + "_errors." + finfo.Extension));
                           
                            MessageBox.Show("Hotovo. Vytvořeno: " + totalCountCreated.ToString() + " plánu s rekurencí z (" + tpmCount.ToString() + ")");
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

        }
    }
}
