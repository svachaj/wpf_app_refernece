using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.IO;

namespace COS.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        int prcsCount = 0;
        int prcsTick = 0;
        string startFileapp = "";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

                                   
            if (e.Args.Length > 1)
            {

                try
                {
                    startFileapp = e.Args[0];
                    string procesNameToKill = e.Args[1];

                    string userName = e.Args.Length > 2 ? e.Args[2] : "";

                    var prcs = Process.GetProcesses().Where(a => a.ProcessName == procesNameToKill);
                    prcsCount = prcs.Count();

                    foreach (var itm in Process.GetProcesses().Where(a => a.ProcessName == procesNameToKill))
                    {
                        itm.Kill();
                    }

                    startFileapp = startFileapp.Replace("$$", " ");

                    if (File.Exists(startFileapp))
                    {
                        Process.Start(startFileapp, userName);
                    }
                    else 
                    {
                        Process.Start("COS.Application.exe");
                    }
                }
                catch
                {
                    this.Shutdown();
                }

            }


            this.Shutdown();


        }

        //void itm_Exited(object sender, EventArgs e)
        //{
        //    prcsTick++;

        //    if (prcsTick == prcsCount)
        //    {
        //        try
        //        {
        //            Process.Start(startFileapp);
        //        }
        //        catch
        //        {
        //            this.Shutdown();
        //        }
        //        finally
        //        {
        //            this.Shutdown();
        //        }
        //    }
        //}
    }
}
