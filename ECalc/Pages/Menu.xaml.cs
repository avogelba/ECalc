﻿using ECalc.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ECalc.Pages
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog w = new AboutDialog();
            MainWindow.ShowDialog(w);
        }

        private void BtnIssue_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "https://github.com/webmaster442/ECalc/issues";
            p.Start();
        }

        private void BtnScreenKeyboard_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "osk.exe";
            p.StartInfo.UseShellExecute = true;
            p.Start();
        }
    }
}
