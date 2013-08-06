using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace signed_hasher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
            Application.ThreadException += NBug.Handler.ThreadException;
            NBug.Settings.ReleaseMode = true;
            NBug.Settings.UIMode = NBug.Enums.UIMode.Full;
            StreamWriter writer=null;

            //gdrive.init("872915088368.apps.googleusercontent.com", "LLM9uPE-wq4aXnFNba4a0DNb");
            //gdrive.listfiles("PCD");
            //gdrive.insertFile("hasherdump","filehasherdb",mimetype.plaintext,DateTime.Now.ToShortDateString() + ".sql","hasherdb");

            if (args.Length == 0)
            {
                #if !DEBUG
                try
                {
                    writer = new StreamWriter("log/" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + ".log", true);
                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory("log");
                    writer = new StreamWriter("log/" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + ".log", true);
                }
                    writer.AutoFlush = true;
                    Console.SetOut(writer);
                #endif

                //autorun(); // causes error in WINPE 4 NUllReferenceException
                //create_shortcut();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new hasher()); //blocking call
                
                #if !DEBUG
                    writer.Close();
                #endif
            }
            else
            {
                Console.WriteLine("hasher mode certificate [password] file");
                Console.WriteLine("Mode: create/verify");
            }
        }

        private static void autorun()
        {
            RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            Key.SetValue("File Hasher", System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public static void create_shortcut()
        { // code from codeproject
            IWshRuntimeLibrary.WshShellClass wshShell = new IWshRuntimeLibrary.WshShellClass();
            IWshRuntimeLibrary.IWshShortcut shortcut;
            string startUpFolderPath =
              Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            // Create the shortcut
            shortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(startUpFolderPath + "\\" + Application.ProductName + ".lnk");
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Description = "File Hasher";
            shortcut.Save();
        }
    }
}
