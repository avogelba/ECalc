﻿using ECalc.IronPythonEngine.Types;
using ECalc.Maths;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ECalc.Controls
{
    /// <summary>
    /// Interaction logic for EditNewVariableDialog.xaml
    /// </summary>
    public partial class EditNewVariableDialog : CustomDialog
    {
        public EditNewVariableDialog()
        {
            InitializeComponent();
            Index = -1;
            _cantrigger = true;
        }

        public RoutedEventHandler SaveClicked;
        public RoutedEventHandler SaveInsertClicked;

        private bool _cantrigger { get; set; }

        public int Index
        {
            get;
            private set;
        }

        public bool IsEditDialog
        {
            get;
            set;
        }

        #region Double Editor
        public double Double
        {
            get { return Convert.ToDouble(TbDoubleValue.Text); }
            set
            {
                TbDoubleValue.Text = value.ToString();
                Dispatcher.Invoke(() => { TabTypeSelector.SelectedIndex = 0; });
            }
        }
        #endregion

        #region Complex Editor
        public Complex Complex
        {
            get
            {
                var r = Convert.ToDouble(TbRealValue.Text);
                var i = Convert.ToDouble(TbImaginaryValue.Text);
                return new Complex(r, i);
            }
            set
            {
                TbRealValue.Text = value.Real.ToString();
                TbImaginaryValue.Text = value.Imaginary.ToString();
                Dispatcher.Invoke(() => { TabTypeSelector.SelectedIndex = 1; });
            }
        }

        private void ComplexEdit(object sender, TextChangedEventArgs e)
        {
            if (!_cantrigger) return;
            try
            {
                var tb = sender as TextBox;
                switch (tb.Name)
                {
                    case "TbRealValue":
                    case "TbImaginaryValue":
                        var cmp = Complex;
                        _cantrigger = false;
                        TbAbs.Text = cmp.Magnitude.ToString();
                        TbAngle.Text = TrigFunctions.ArcTan(cmp.Imaginary / cmp.Real).ToString();
                        _cantrigger = true;
                        break;
                    case "TbAbs":
                    case "TbAngle":
                        var cmplx = TypeFunctions.CplxPolar(Convert.ToDouble(TbAbs.Text), Convert.ToDouble(TbAngle.Text));
                        _cantrigger = false;
                        TbRealValue.Text = cmplx.Real.ToString();
                        TbImaginaryValue.Text = cmplx.Imaginary.ToString();
                        _cantrigger = true;
                        break;
                }
            }
            catch (Exception) { _cantrigger = true; }
        }
        #endregion

        #region Fraction Editor
        public Fraction Fraction
        {
            get
            {
                var n = Convert.ToInt64(TbNumeratorValue.Text);
                var d = Convert.ToInt64(TbDenumeratorValue.Text);
                return new Fraction(n, d);
            }
            set
            {
                TbDenumeratorValue.Text = value.Denominator.ToString();
                TbNumeratorValue.Text = value.Numerator.ToString();
                Dispatcher.Invoke(() => { TabTypeSelector.SelectedIndex = 2; });
            }
        }
        #endregion

        #region Vector Editor
        public IronPythonEngine.Types.Vector Vector
        {
            get
            {
                var x = Convert.ToDouble(TbXValue.Text);
                var y = Convert.ToDouble(TbYValue.Text);
                if (Cb3D.IsChecked == true)
                {
                    var z = Convert.ToDouble(TbZValue.Text);
                    return new IronPythonEngine.Types.Vector(x, y, z);
                }
                else return new IronPythonEngine.Types.Vector(x, y);
            }
            set
            {
                TbXValue.Text = value.X.ToString();
                TbYValue.Text = value.Y.ToString();
                if (value.Dimensions == 2)
                {
                    Cb3D.IsChecked = false;
                    TbZValue.Text = "";
                }
                else
                {
                    TbZValue.Text = value.Z.ToString();
                    Cb3D.IsChecked = true;
                }
            }
        }
        #endregion

        #region Set
        public IronPythonEngine.Types.Set Set
        {
            get
            {
                Set ret = new Set(TbSet.LineCount);
                foreach (var line in TbSet.Text.Split('\n'))
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    ret.Add(Convert.ToDouble(line));
                }
                return ret;
            }
            set
            {
                StringBuilder buffer = new StringBuilder();
                foreach (var item in value)
                {
                    buffer.Append(item);
                    buffer.Append("\r\n");
                }
                TbSet.Text = buffer.ToString();
            }
        }
        #endregion

        #region Matrix Editor

        /// <summary>
        /// Renders the editor
        /// </summary>
        /// <param name="rows">Number of desired rows</param>
        /// <param name="columns">Number of desired columns</param>
        private void RenderEditor(int rows, int columns)
        {
            string[,] copy = null;

            if (Editor.Children.Count > 0)
            {
                int r = Editor.RowDefinitions.Count;
                int c = Editor.ColumnDefinitions.Count;
                copy = new string[r, c];
                foreach (TextBox tx in Editor.Children)
                {
                    r = Grid.GetRow(tx);
                    c = Grid.GetColumn(tx);
                    if (!string.IsNullOrEmpty(tx.Text)) copy[r, c] = tx.Text;
                }
            }

            Editor.Children.Clear();
            Editor.ColumnDefinitions.Clear();
            Editor.RowDefinitions.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            for (int i = 0; i < rows; i++) Editor.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < columns; i++) Editor.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var tx = new TextBox();
                    tx.MinWidth = 60;
                    tx.Margin = new Thickness(2);
                    Grid.SetRow(tx, i);
                    Grid.SetColumn(tx, j);

                    if (copy != null)
                    {
                        if (i < copy.GetLength(0) && j < copy.GetLength(1)) tx.Text = copy[i, j];
                        else tx.Text = "0";
                    }
                    else tx.Text = "0";
                    Editor.Children.Add(tx);
                }
            }
        }

        public Matrix Matrix
        {
            get
            {
                var m = new Matrix(Editor.RowDefinitions.Count, Editor.ColumnDefinitions.Count);
                int i = 0;
                int j = 0;
                foreach (TextBox tx in Editor.Children)
                {
                    i = Grid.GetRow(tx);
                    j = Grid.GetColumn(tx);
                    m[i, j] = Convert.ToDouble(tx.Text);
                }
                return m;
            }
            set
            {
                Matrix matrix = value;
                Editor.Children.Clear();
                Editor.RowDefinitions.Clear();
                Editor.ColumnDefinitions.Clear();

                for (int i = 0; i < matrix.Rows; i++) Editor.RowDefinitions.Add(new RowDefinition());
                for (int i = 0; i < matrix.Columns; i++) Editor.ColumnDefinitions.Add(new ColumnDefinition());

                for (int i = 0; i < matrix.Rows; i++)
                {
                    for (int j = 0; j < matrix.Columns; j++)
                    {
                        var tx = new TextBox();
                        tx.MinWidth = 60;
                        tx.Margin = new Thickness(2);
                        Grid.SetRow(tx, i);
                        Grid.SetColumn(tx, j);
                        tx.Text = matrix[i, j].ToString();
                        Editor.Children.Add(tx);
                    }
                }
                Dispatcher.Invoke(() => { TabTypeSelector.SelectedIndex = 4; });
            }
        }

        private void BtnSetSize_Click(object sender, RoutedEventArgs e)
        {
            RenderEditor((int)Rows.Value, (int)Columns.Value);
        }

        #endregion

        #region Time Editor
        public Time Time
        {
            get { return new Time(Timepicker.SelectedTime.Value.TotalSeconds); }
            set
            {
                var Ts = TimeSpan.FromSeconds(value.TotalSeconds);
                Timepicker.SelectedTime = Ts;
            }
        }
        #endregion

        private async void PART_NegativeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Index = -1;
            var main = (MainWindow)Application.Current.MainWindow;
            await main.HideMetroDialogAsync(this);
        }

        private void SaveCheck()
        {
            Index = TabTypeSelector.SelectedIndex;

            try
            {
                object test = null;
                switch (Index)
                {
                    case 0:
                        test = this.Double;
                        break;
                    case 1:
                        test = this.Complex;
                        break;
                    case 2:
                        test = this.Fraction;
                        break;
                    case 3:
                        test = this.Vector;
                        break;
                    case 4:
                        test = this.Matrix;
                        break;
                    case 5:
                        test = this.Set;
                        break;
                    case 6:
                        test = this.Time;
                        break;
                }
            }
            catch (Exception)
            {
                TbError.Visibility = Visibility.Visible;
                return;
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCheck();
            SaveClicked?.Invoke(sender, e);
            var main = (MainWindow)Application.Current.MainWindow;
            await main.HideMetroDialogAsync(this);
        }

        private async void BtnSave2_Click(object sender, RoutedEventArgs e)
        {
            SaveCheck();
            SaveInsertClicked?.Invoke(sender, e);
            var main = (MainWindow)Application.Current.MainWindow;
            await main.HideMetroDialogAsync(this);
        }

        private void TabTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource == TabTypeSelector)
            {
                if (IsEditDialog)
                {
                    e.Handled = true;
                    Dispatcher.Invoke(() => { TabTypeSelector.SelectedIndex = Index; });
                }
                else
                {
                    Index = TabTypeSelector.SelectedIndex;
                }
            }
        }

        protected override void OnShown()
        {
            base.OnShown();
            Dispatcher.Invoke(() =>
            {
                if (TbError.Visibility == Visibility.Visible) TbError.Visibility = Visibility.Collapsed;
                if (this.IsEditDialog) return;
                TbDenumeratorValue.Text = "";
                TbNumeratorValue.Text = "";
                TbRealValue.Text = "";
                TbImaginaryValue.Text = "";
                TbDoubleValue.Text = "";
                TbXValue.Text = "";
                TbYValue.Text = "";
                TbZValue.Text = "";
                Cb3D.IsChecked = false;
                Editor.Children.Clear();
                Rows.Value = 2;
                Columns.Value = 2;
                TbSet.Text = "";
            });
        }
    }
}
