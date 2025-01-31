using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Application.Shared;
using System.ComponentModel;
using COS.Common;
using System.Transactions;

namespace COS.Notification.TM
{
    public class NotificationController
    {
        private static BackgroundWorker worker = new BackgroundWorker();

        private static List<HourlyProduction> requestsList = new List<HourlyProduction>();

        private static BackgroundWorker worker2 = new BackgroundWorker();

        private static List<HourlyProduction> requestsList2 = new List<HourlyProduction>();

        public static void ControlLimitNotification(HourlyProduction production, bool doAsync)
        {
            if (doAsync)
            {
                if (!worker.IsBusy)
                {
                    worker.DoWork -= worker_DoWork;
                    worker.DoWork += new DoWorkEventHandler(worker_DoWork);

                    worker.RunWorkerCompleted -= worker_RunWorkerCompleted;
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

                    worker.RunWorkerAsync(production);
                }
                else
                {
                    requestsList.Add(production);
                }
            }
            else
            {
                ControlLimit(production);
            }

        }

        static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (requestsList.Count > 0)
            {
                var item = requestsList.Last();

                worker.RunWorkerAsync(item);

                requestsList.RemoveAt(requestsList.Count - 1);
            }
        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ControlLimit((HourlyProduction)e.Argument);
        }

        private static void ControlLimit(HourlyProduction production)
        {

            ToolNotification toolNotifyP = COSContext.Current.ToolNotifications.FirstOrDefault(a => a.Notification.NotificationType.Name == NotificationConstants.LimitToolNotification);

            decimal limitPerc = 15;

            if (toolNotifyP != null)            
            {
                limitPerc = toolNotifyP.LimitPerc;
            }            
            
            var toolitm = COSContext.Current.ToolItemNumbers.FirstOrDefault(a => a.ItemNumber == production.ItemNumber);

            if (toolitm != null)
            {
                var tool = toolitm.Tool;
                if (tool != null)
                {
                    var wctool = tool.ToolWCs.FirstOrDefault(a => a.ID_workCenter == production.ID_WorkCenter && a.ID_tool == tool.ID);
                    if (wctool != null)
                    {
                        //když je překročen limit procent tak pustíme notifikaci
                        if (wctool.ActualPcs / tool.ServiceLifePcs * 100 > limitPerc)
                        {
                            if (!wctool.IsLimitNotifySend)
                            {
                                Console.WriteLine("Limit překročen");

                                string msg = "Byl překročen limit u přípravku: " + tool.Description;
                                msg += Environment.NewLine;
                                msg += "WC: " + wctool.WorkCenter.Value;
                                msg += Environment.NewLine;
                                msg += "limit: " + limitPerc + "%";
                                msg += Environment.NewLine;
                                msg += "životnost: " + tool.ServiceLifePcs;
                                msg += Environment.NewLine;
                                msg += "aktuální stav: " + wctool.ActualPcs;

                                try
                                {
                                    var notf = COSContext.Current.SysNotifications.FirstOrDefault(a=>a.NotificationType.Name == NotificationConstants.LimitToolNotification);
                                    EmailNotification.SendEmail("LIMIT TOOL notification", msg, notf);
                                }
                                catch { }

                                wctool.IsLimitNotifySend = true;
                                using (TransactionScope trans = new TransactionScope())
                                {
                                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                                    trans.Complete();
                                }
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    Console.WriteLine("OK");
            //}


        }

        public static void ControlOverflowNotification(HourlyProduction production, bool doAsync)
        {

            if (doAsync)
            {
                if (!worker2.IsBusy)
                {
                    worker2.DoWork -= worker2_DoWork;
                    worker2.DoWork += new DoWorkEventHandler(worker2_DoWork);

                    worker2.RunWorkerCompleted -= worker2_RunWorkerCompleted;
                    worker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker2_RunWorkerCompleted);

                    worker2.RunWorkerAsync(production);
                }
                else
                {
                    requestsList2.Add(production);
                }
            }
            else
            {
                ControlOverflow(production);
            }


        }

        static void worker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (requestsList2.Count > 0)
            {
                var item = requestsList2.Last();

                worker2.RunWorkerAsync(item);

                requestsList2.RemoveAt(requestsList2.Count - 1);
            }
        }

        static void worker2_DoWork(object sender, DoWorkEventArgs e)
        {
            ControlOverflow((HourlyProduction)e.Argument);
        }

        private static void ControlOverflow(HourlyProduction production)
        {

            var notify = COSContext.Current.SysNotifications.FirstOrDefault(a => a.NotificationType.Name == NotificationConstants.OverflowToolNotification);

            var toolitm = COSContext.Current.ToolItemNumbers.FirstOrDefault(a => a.ItemNumber == production.ItemNumber);

            if (toolitm != null)
            {
                var tool = toolitm.Tool;
                if (tool != null)
                {
                    var wctool = COSContext.Current.TmToolWCs.FirstOrDefault(a => a.ID_workCenter == production.ID_WorkCenter && a.ID_tool == tool.ID);
                    if (wctool != null)
                    {
                        //když je překročen limit procent tak pustíme notifikaci
                        if (wctool.ActualPcs > tool.ServiceLifePcs)
                        {
                            if (!wctool.IsOverflowNotifySend)
                            {
                                Console.WriteLine("Životnost překročena");

                                string msg = "Byla překročena životnost přípravku: " + tool.Description;
                                msg += Environment.NewLine;
                                msg += "WC: " + wctool.WorkCenter.Value;
                                msg += Environment.NewLine;
                                msg += "životnost: " + tool.ServiceLifePcs;
                                msg += Environment.NewLine;
                                msg += "aktuální stav: " + wctool.ActualPcs;

                                try
                                {
                                    EmailNotification.SendEmail("OVERFLOW TOOL notification", msg, notify);
                                }
                                catch { }
                                wctool.IsOverflowNotifySend = true;

                                using (TransactionScope trans = new TransactionScope())
                                {
                                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                                    trans.Complete();
                                }
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    Console.WriteLine("OK");
            //}
        }



    }
}
