﻿using MahApps.Metro.Controls;
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

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            var title = ((Tile)sender).Title;
            UserControl control = null;

            switch (title)
            {
                case "Calculator":
                    control = new Calculator();
                    break;
                case "Statistics":
                    control = new Stat();
                    break;
                case "Unit Converter":
                    control = new UnitConverterPage();
                    break;
                case "Equation System Solver":
                    control = new EquationSystemSolver();
                    break;
                case "Logic Minimizer":
                    control = new LogicFunctionMinimizer();
                    break;
                case "IP Subnet Calculator":
                    control = new SubnetCalculator();
                    break;
                case "Hash Calculator":
                    control = new HashCalculators();
                    break;
                case "About":
                    control = new About();
                    break;
                case "Resistor Color":
                    control = new ResistorColorDecoder();
                    break;
                case "Currency Converter":
                    control = new CurrencyConverter();
                    break;
                case "Voltage & Current divider":
                    control = new VoltageCurrentDivider();
                    break;
                case "LED Calculator":
                    control = new LEDCalculator();
                    break;
                case "Resistor value solver":
                    control = new ResistorSolver();
                    break;
                case "OpAmp Calculator":
                    control = new OpAmp();
                    break;
                case "Number System converter":
                    control = new NumberSystems();
                    break;
                default:
                    return;
            }

            MainWindow.SwithToControl(control);
            MainWindow.SetTitle(title);
        }
    }
}
