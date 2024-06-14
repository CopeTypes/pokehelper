using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokeHelper.config;
using PokeHelper.ui;
using PokeHelper.util;

namespace PokeHelper
{
    static class Program
    {
        [DllImport( "kernel32.dll" )]
        private static extern bool AttachConsole( int dwProcessId );
    
        public static string WorkDir;
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static async Task Main()
        {
            AttachConsole(-1);
            var wd = GetCurrentDir();
            WorkDir = wd ?? throw new Exception("Unable to get execution directory.");
            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());


            var helper = new PogoHelper();
            var _adb = helper.GetAdbHelper();
            var _pgs = helper.GetPgsHelper();
            
            new DebugUI(helper).ShowDialog();
        }
        
        
        private static string GetCurrentDir()
        {
            var exePath = Assembly.GetEntryAssembly()?.Location;
            return Path.GetDirectoryName(exePath);
        }
    }
}