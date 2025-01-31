using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace COS.Common
{
    public class IOPaths
    {

        private static List<UpdateFileInfo> _filesToUpdate = null;

        public static List<UpdateFileInfo> FilesToUpdate
        {
            get
            {
                if (_filesToUpdate == null)
                {
                    _filesToUpdate = new List<UpdateFileInfo>();
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.exe", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Splash.exe", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Administration.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Engeneering.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.HumanResources.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Production.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Reporting.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Shared.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.TechnicalMaintenance.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Common.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Common.WPF.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Security.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COSResources.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Data.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Docking.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.GridView.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.ImageEditor.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Input.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Navigation.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.RibbonView.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Data.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Documents.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.exe.config", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Splash.exe.Config", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.DataVisualization.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Charting.dll", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe", null));
                    _filesToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe.config", null));



                }

                return _filesToUpdate;
            }
        }

        private static List<UpdateFileInfo> _filesToUpdate2 = null;

        public static List<UpdateFileInfo> FilesToUpdate2
        {
            get
            {
                if (_filesToUpdate2 == null)
                {
                    _filesToUpdate2 = new List<UpdateFileInfo>();
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe.config", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Administration.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Engeneering.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.HumanResources.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Production.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Reporting.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Shared.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.TechnicalMaintenance.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Common.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Common.WPF.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Security.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COSResources.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Data.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Docking.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.GridView.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.ImageEditor.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Input.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Navigation.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.RibbonView.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Data.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Documents.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.DataVisualization.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Charting.dll", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.exe.config", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Splash.exe.Config", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.exe", null));
                    _filesToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Splash.exe", null));

                }

                return _filesToUpdate2;
            }
        }


        private static List<UpdateFileInfo> _filesVisualsToUpdate = null;

        public static List<UpdateFileInfo> FilesVisualsToUpdate
        {
            get
            {
                if (_filesVisualsToUpdate == null)
                {
                    _filesVisualsToUpdate = new List<UpdateFileInfo>();
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.exe", null));
                    //_filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Splash.exe", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Administration.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Engeneering.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.HumanResources.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Production.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Reporting.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.Shared.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.TechnicalMaintenance.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Common.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Common.WPF.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Security.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COSResources.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\System.ComponentModel.Composition.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Data.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Docking.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.GridView.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.ImageEditor.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Input.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.Navigation.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Controls.RibbonView.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Data.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\Telerik.Windows.Documents.dll", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Application.exe.config", null));
                    //_filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe.config", null));
                    _filesVisualsToUpdate.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Splash.exe.Config", null));


                    //_filesVisualsToUpdate.Add(new UpdateFileInfo(@"c:\test\index.htm", @"c:\test2\index.htm"));
                }

                return _filesVisualsToUpdate;
            }
        }

        private static List<UpdateFileInfo> _filesVisualsToUpdate2 = null;

        public static List<UpdateFileInfo> FilesVisualsToUpdate2
        {
            get
            {
                if (_filesVisualsToUpdate2 == null)
                {
                    _filesVisualsToUpdate2 = new List<UpdateFileInfo>();
                    _filesVisualsToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe", null));
                    _filesVisualsToUpdate2.Add(new UpdateFileInfo(ServerPaths.PrimaryServerPath + @"\COS.Launcher.exe.config", null));
                    //_filesToUpdate2.Add(new UpdateFileInfo(@"c:\test\Optim.SL.Wustenrot.FinancialPlan.dll", @"c:\test2\Optim.SL.Wustenrot.FinancialPlan.dll"));
                    //_filesToUpdate2.Add(new UpdateFileInfo(@"c:\test\index.htm", @"c:\test2\index.htm"));
                }

                return _filesVisualsToUpdate2;
            }
        }
    }

    public static class ServerPaths
    {
        public static string PrimaryServerPath
        {
            get
            {
                string cryptname = System.Configuration.ConfigurationManager.AppSettings["primaryServerPath"];

                if (!string.IsNullOrEmpty(cryptname))
                {
                    return Crypto.DecryptString(cryptname, COS.Security.SecurityHelper.SecurityKey);
                }
                else
                    return null;
            }
        }

        public static string SecondaryServerPath
        {
            get
            {
                string cryptname = System.Configuration.ConfigurationManager.AppSettings["secondaryServerPath"];

                if (!string.IsNullOrEmpty(cryptname))
                {
                    return Crypto.DecryptString(cryptname, COS.Security.SecurityHelper.SecurityKey);
                }
                else
                    return null;
            }
        }
    }

    public static class LocalPaths
    {
        public static string LocalAssembliesPath
        {
            get
            {
                return "";
            }
        }
    }



    public class UpdateFileInfo
    {
        public UpdateFileInfo()
        {

        }

        public UpdateFileInfo(string sourceFilePath)
        {
            SourceFilePath = sourceFilePath;
        }

        public UpdateFileInfo(string sourceFilePath, string sourceFilePathAlternate)
        {
            SourceFilePath = sourceFilePath;
            SourceFilePathAlternate = sourceFilePathAlternate;
        }

        public string SourceFilePath { set; get; }

        public string SourceFilePathAlternate { set; get; }

        public FileInfo SourceFile
        {
            get
            {
                FileInfo info = null;

                try
                {
                    if (File.Exists(SourceFilePath))
                        info = new FileInfo(SourceFilePath);
                }
                catch
                {
                    info = null;
                }

                return info;
            }
        }

        public FileInfo SourceFileAlternate
        {
            get
            {
                FileInfo info = null;

                try
                {
                    if (File.Exists(SourceFilePathAlternate))
                        info = new FileInfo(SourceFilePathAlternate);
                }
                catch
                {
                    info = null;
                }

                return info;
            }
        }

        public bool IsFileAssembly
        {
            get
            {
                return false;
                bool result = false;

                if (SourceFile != null)
                {
                    if (SourceFile.Extension.Contains("dll") || SourceFile.Extension.Contains("exe"))
                    {
                        result = true;
                    }
                    else
                        result = false;
                }
                else
                    result = false;

                return result;
            }
        }

        public bool IsFileAlternateAssembly
        {
            get
            {
                return false;
                bool result = false;

                if (SourceFile != null)
                {
                    if (SourceFile.Extension.Contains("dll") || SourceFile.Extension.Contains("exe"))
                    {
                        result = true;
                    }
                    else
                        result = false;
                }
                else
                    result = false;

                return result;
            }
        }


        public Assembly SourceFileAssembly
        {
            get
            {
                Assembly asm = null;

                if (IsFileAssembly)
                {
                    asm = Assembly.LoadFile(SourceFilePath);

                }

                return asm;
            }
        }

        public Assembly SourceFileAlternateAssembly
        {
            get
            {
                Assembly asm = null;

                if (IsFileAssembly)
                {

                    asm = Assembly.LoadFile(SourceFilePathAlternate);

                }

                return asm;
            }
        }
    }
}
