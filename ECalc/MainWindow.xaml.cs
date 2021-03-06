﻿using AppLib.Common.Console;
using AppLib.WPF;
using ECalc.Classes;
using ECalc.Pages;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ECalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Switches to another user control
        /// </summary>
        /// <param name="control">A user control to show</param>
        public static void SwithToControl(UserControl control)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            if (main.TransitionControl.Content is Calculator)
            {
                (main.TransitionControl.Content as Calculator).SaveMemSession();
                ConfigFileHelpers.SerializeFunctionUsageStats();
                ConfigFileHelpers.SerializeConstantUsageStats();
                Properties.Settings.Default.Save();
            }
            var dispose = main.TransitionControl.Content as IDisposable;
            if (dispose!=null)
            {
                dispose.Dispose();
            }
            main.TransitionControl.Content = null;
            foreach (Flyout flyout in main.Flyouts.Items) flyout.IsOpen = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            main.TransitionControl.Content = control;
        }

        /// <summary>
        /// Display an error dialog
        /// </summary>
        /// <param name="error">error text</param>
        public static async void ErrorDialog(string error)
        {
            try
            {
                var main = (MainWindow)Application.Current.MainWindow;
                await main.ShowMessageAsync("Error", error, MessageDialogStyle.Affirmative);
            }
            catch (Exception)
            {
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Display a generic dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="text">Dialog text</param>
        /// <param name="style">Dialog style</param>
        public static async void ShowDialog(string title, string text, MessageDialogStyle style)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            await main.ShowMessageAsync(title, text, style);
        }

        /// <summary>
        /// Shows a custom dialog
        /// </summary>
        /// <param name="dialog">dialog to display</param>
        public static async void ShowDialog(CustomDialog dialog)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            await main.ShowMetroDialogAsync(dialog);
        }

        public static void SetTitle(string titletext)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.Title = titletext;
        }

        private void WindowCommandCalculatorChooser_Click(object sender, RoutedEventArgs e)
        {
            MainMenuFlyOut.IsOpen = false;
            CalculatorChooserFlyOut.IsOpen = !CalculatorChooserFlyOut.IsOpen;
        }

        private void WindowCommandMenu_Click(object sender, RoutedEventArgs e)
        {
            CalculatorChooserFlyOut.IsOpen = false;
            MainMenuFlyOut.IsOpen = !MainMenuFlyOut.IsOpen;
        }

        private void WindowCommandSize_Click(object sender, RoutedEventArgs e)
        {
            var ws = WindowCommandSize.IsChecked.Value == true ? WindowSizes.Large : WindowSizes.Normal;
            WindowManager.ResizeWindows(ws);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !WindowManager.CloseAll();
            if (TransitionControl.Content is Calculator)
            {
                ConfigFileHelpers.SerializeFunctionUsageStats();
                ConfigFileHelpers.SerializeConstantUsageStats();
                
            }
            App.SaveSettings();
            Properties.Settings.Default.Save();
        }

        private void ThumbMenu_Click(object sender, EventArgs e)
        {
            WindowCommandMenu_Click(sender, null);
            WindowManager.BringToFront(this);
        }

        private void ThumbCalculators_Click(object sender, EventArgs e)
        {
            WindowCommandCalculatorChooser_Click(sender, null);
            WindowManager.BringToFront(this);
        }

        private void ThumbWinManager_Click(object sender, EventArgs e)
        {
            SwithToControl(new WindowsManager());
        }

        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            App.Splash.Close();
            HandleArguments();

        }

        private void MainWin_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var calc = TransitionControl.Content as Calculator;
            if (calc == null) return;

            if (CalculatorChooserFlyOut.IsOpen) return;
            else calc.FocusInput();
        }

        private void HandleArguments()
        {
            var pp = new ParametersParser(false);
            if (pp.HasKey("/update")) Updater.DoUpdate();
            else if (pp.HasKey("/firstmonitor"))
            {
                ScreenHelper.MoveToDefaultScreen(this);
                var pid = Process.GetCurrentProcess().Id;
                var others = from i in Process.GetProcesses()
                             where i.Id != pid && i.ProcessName.Contains("ECalc")
                             select i;

                foreach (var process in others)
                    process.CloseMainWindow();
            }
        }
    }
}
