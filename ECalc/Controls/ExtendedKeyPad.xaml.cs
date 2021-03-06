﻿using ECalc.Classes;
using System.Windows;
using System.Windows.Controls;

namespace ECalc.Controls
{
    /// <summary>
    /// Interaction logic for ExtendedKeyPad.xaml
    /// </summary>
    internal partial class ExtendedKeyPad : UserControl
    {
        public ExtendedKeyPad()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler ExecuteClicked;

        public event RoutedEventHandler BackClicked;

        public event StringEventHandler ButtonClicked;

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (BackClicked != null)
            {
                BackClicked(sender, e);
            }
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (ExecuteClicked != null)
            {
                ExecuteClicked(sender, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonClicked != null)
            {
                var content = ((Button)sender).Content.ToString();
                switch (content)
                {
                    case "Roman":
                        ButtonClicked(sender, new StringEventArgs("Rx"));
                        break;
                    case "BIN":
                        ButtonClicked(sender, new StringEventArgs("Bx"));
                        break;
                    case "OCT":
                        ButtonClicked(sender, new StringEventArgs("Ox"));
                        break;
                    case "HEX":
                        ButtonClicked(sender, new StringEventArgs("Hx"));
                        break;
                    default:
                        ButtonClicked(sender, new StringEventArgs(content));
                        break;
                }
            }
        }
    }
}
