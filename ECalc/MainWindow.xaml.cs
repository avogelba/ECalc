﻿using ECalc.Classes;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

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
            MainWindow main = (MainWindow)App.Current.MainWindow;
            main.TransitionControl.Content = null;
            foreach (Flyout flyout in main.Flyouts.Items) flyout.IsOpen = false;
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
            MainWindow main = (MainWindow)App.Current.MainWindow;
            await main.ShowMessageAsync("Error", error, MessageDialogStyle.Affirmative);
        }

        /// <summary>
        /// Display a generic dialog
        /// </summary>
        /// <param name="error">error text</param>
        public static async void ShowDialog(string title, string text, MessageDialogStyle style)
        {
            MainWindow main = (MainWindow)App.Current.MainWindow;
            await main.ShowMessageAsync(title, text, style);
        }

        /// <summary>
        /// Show a custom dialog
        /// </summary>
        /// <param name="dialog">dialog to display</param>
        public static async void ShowDialog(CustomDialog dialog)
        {
            MainWindow main = (MainWindow)App.Current.MainWindow;
            await main.ShowMetroDialogAsync(dialog);
        }

        private void WindowCommandMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyOut.IsOpen = !MenuFlyOut.IsOpen;
        }

        private void WindowCommandKeyboard_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "osk.exe";
            p.StartInfo.UseShellExecute = true;
            p.Start();
        }
    }
}
