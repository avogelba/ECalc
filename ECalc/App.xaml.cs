﻿using ECalc.Api;
using MahApps.Metro;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ECalc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Accent[] _accents;
        private static Random _random;
        private static int _index;

        public static SplashScreen Splash { get; private set; }

        /// <summary>
        /// Property to acces application modules
        /// </summary>
        public static ModuleLoader Modules
        {
            get;
            private set;
        }

        public static void NextTheme()
        {
            int next = _index + 1;
            if (next > _accents.Length - 1) next = 0;
            _index = next;
            ThemeManager.ChangeAppStyle(Application.Current, _accents[_index], ThemeManager.GetAppTheme("BaseLight"));
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Splash = new SplashScreen();
            Splash.Show();

            var loadtask = new Task(() =>
            {
                //here to do long stuff;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                var theme = ThemeManager.DetectAppStyle(Application.Current);

                _accents = new Accent[]
                {
                ThemeManager.GetAccent("Red"), ThemeManager.GetAccent("Green"), ThemeManager.GetAccent("Blue"),
                ThemeManager.GetAccent("Purple"), ThemeManager.GetAccent("Orange"), ThemeManager.GetAccent("Lime"),
                ThemeManager.GetAccent("Emerald"), ThemeManager.GetAccent("Teal"), ThemeManager.GetAccent("Cyan"),
                ThemeManager.GetAccent("Cobalt"), ThemeManager.GetAccent("Indigo"), ThemeManager.GetAccent("Violet"),
                ThemeManager.GetAccent("Pink"), ThemeManager.GetAccent("Magenta"), ThemeManager.GetAccent("Crimson"),
                ThemeManager.GetAccent("Amber"), ThemeManager.GetAccent("Yellow"), ThemeManager.GetAccent("Brown"),
                ThemeManager.GetAccent("Olive"), ThemeManager.GetAccent("Steel"), ThemeManager.GetAccent("Mauve"),
                ThemeManager.GetAccent("Taupe"), ThemeManager.GetAccent("Sienna")
                };

                _random = new Random();
                _index = _random.Next(0, _accents.Length);
                ThemeManager.ChangeAppStyle(Application.Current, _accents[_index], ThemeManager.GetAppTheme("BaseLight"));

            });
            loadtask.ContinueWith(t =>
            {
                Modules = new ModuleLoader();
                Modules.LoadFromNameSpace("ECalc.Modules");
                MainWindow mw = new MainWindow();
                MainWindow = mw;
                mw.Show();

            }, TaskScheduler.FromCurrentSynchronizationContext());
            loadtask.Start();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception data = (Exception)e.ExceptionObject;
            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EcalcCrashLog.txt");
            using (var text = File.CreateText(filename))
            {
                text.WriteLine("--------------------------------------------");
                text.WriteLine("Ecalc has crashed. Sorry for the inconvenience");
                text.WriteLine("Please forward this log's contents to the projects issues section:");
                text.WriteLine("https://github.com/webmaster442/ECalc/issues");
                text.WriteLine("--------------------------------------------");
                text.WriteLine("Crash timestamp: {0}", DateTime.Now);
                text.WriteLine("OS Version: {0}", Environment.OSVersion);
                text.WriteLine("OS, Process is 64 bit: {0}, {1}", Environment.Is64BitOperatingSystem, Environment.Is64BitProcess);
                text.WriteLine("--------------------------------------------");
                text.WriteLine("Exception Message & Source:\r\n{0}\r\n\r\n{1}", data.Message, data.Source);
                text.WriteLine("--------------------------------------------");
                text.WriteLine("Target Site:\r\n{0}", data.TargetSite);
                text.WriteLine("--------------------------------------------");
                text.WriteLine("Stack Trace:\r\n{0}", data.StackTrace);
                text.WriteLine("--------------------------------------------");
            }
            Process p = new Process();
            p.StartInfo.FileName = filename;
            p.Start();
        }
    }
}
