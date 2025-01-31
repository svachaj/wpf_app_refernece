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
using System.Deployment.Application;
using System.Threading;
using System.ComponentModel;

namespace COS.Application
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UpdateWindow_Loaded);
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            InstallUpdateSyncWithInfo();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AppStartClass.Current.ResultStartCode = error;
        }

        int error = 0;

        BackgroundWorker worker = null;

        void UpdateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();               
        }

        private void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    error = 3;
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    error = 1;
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    error = 1;
                    return;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();
                            error = 2;
                            return;
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            error = 1;
                            return;
                        }
                    }
                }


                error = 3;
            }
            else
                error = 3;
        }
    }
}
