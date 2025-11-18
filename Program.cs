using Assistance;
using Forms;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Main
{
    internal static class Program
    {
        public static string SessionID = string.Empty;

        [STAThread]
        public static void Main()
        {
            try
            {
                var dllDirectory = @"C:/lib";
                if (!Directory.Exists(dllDirectory))
                {
                    Directory.CreateDirectory(dllDirectory, Functions.directorySecurityRules());
                }

                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory);
                bool restartNeeded = false;
                if (!File.Exists("InfiniteOW.exe.config") || Functions.isExternalNewer(Functions.hostUrl + "/InfiniteOW/InfiniteOW.exe.config", "InfiniteOW.exe.config"))
                {
                    while (!Functions.downloadFile(Functions.hostUrl + "/InfiniteOW/InfiniteOW.exe.config", "InfiniteOW.exe.config"))
                    {
                        if (!Functions.yesNoMessage("Error downloading required file", "InfiniteOW.exe.config\n\nDo you want to try again?"))
                        {
                            break;
                        }
                    }

                    restartNeeded = true;
                }
                if (!File.Exists(dllDirectory + "/Newtonsoft.Json.dll"))
                {
                    while (!Functions.downloadFile(Functions.hostUrl + "/InfiniteOW/lib/Newtonsoft.Json.dll", dllDirectory + "/Newtonsoft.Json.dll"))
                    {
                        if (!Functions.yesNoMessage("Error downloading required file", "Newtonsoft.Json.dll\n\nDo you want to try again?"))
                        {
                            break;
                        }
                    }

                    restartNeeded = true;
                }
                if (restartNeeded)
                {
                    Functions.restart();
                }

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.CurrentDomain_AssemblyResolve);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)new StartUpForm());
            }
            catch (Exception ex)
            {
                Functions.showOKMessage("Error starting program InfiniteOW", ex, false);
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(Functions.getErrorTrace(e.ExceptionObject as Exception), "Unhandled Exception Event");
        }

        public static Assembly CurrentDomain_AssemblyResolve(
          object sender,
          ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

    }
}
