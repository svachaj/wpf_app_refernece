using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using COS.Application.Shared;
using System.Threading;
using System.Transactions;
using COS.Common;

namespace COS.NotificationServer
{
    class Program
    {
        static System.Threading.Timer tmr;

        //kolik minut má trvat interval timeru
        static int minuteInterval = 60;

        static int timerInterval = 60000;

        static void Main(string[] args)
        {
            string cmd = "";

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["notifyMinuteInterval"]))
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["notifyMinuteInterval"], out minuteInterval);

            timerInterval = minuteInterval * 1000 * 60;

            COSContext.Init(System.Configuration.ConfigurationManager.ConnectionStrings["COSDataModelContainer"].ConnectionString);


            ControlPrivate();

            tmr = new System.Threading.Timer(new System.Threading.TimerCallback(ControlNotify), null, timerInterval, 0);

            while (cmd != "exit")
            {
                cmd = Console.ReadLine();
            }
        }

        static void ControlNotify(object o)
        {
            tmr.Change(int.MaxValue, 0);

            ControlPrivate();

            tmr.Change(timerInterval, 0);

        }

        private static void ControlPrivate()
        {
            Console.WriteLine("Zahájení kontroly notifikací...");
            Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            var dateto = COSContext.Current.DateTimeServer.AddMonths(-6);
            var productions = COSContext.Current.HourlyProductions.Where(a => a.Changed == true && a.ChangedDate != null && a.ChangedDate >= dateto).ToList();
            
            Console.WriteLine("Počet záznamů ke kontrole: " + productions.Count.ToString());
          
            foreach (var itm in productions)
            {
                COS.Notification.TM.NotificationController.ControlLimitNotification(itm, false);
                COS.Notification.TM.NotificationController.ControlOverflowNotification(itm, false);
                itm.Changed = false;
            }

            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    trans.Complete();
                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, LogType.ToFileAndEmail);
                }
            }


            Console.WriteLine("Konec kontroly notifikací...");
            Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
        }

        
    }
}
