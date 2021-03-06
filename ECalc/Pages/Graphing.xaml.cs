﻿using ECalc.Classes;
using ECalc.IronPythonEngine;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ECalc.Pages
{
    /// <summary>
    /// Interaction logic for Graphing.xaml
    /// </summary>
    public partial class Graphing : UserControl
    {
        private Engine _engine;
        private Point _selectionStart;
        private bool _selectionStarted;
        private FunctionPlotSource _source;

        public Graphing()
        {
            InitializeComponent();
            _engine = new Engine();
            _engine.MemoryManager = new SimpleMemmMan();
            _source = new FunctionPlotSource();
            FunctionTemplates.ItemsSource = _source;
        }

        private double CanvasWidth
        {
            get { return ((FrameworkElement)ScreenCanvas.Parent).ActualWidth; }
        }

        private double CanvasHeight
        {
            get { return ((FrameworkElement)ScreenCanvas.Parent).ActualHeight; }
        }

        private Size CanvasSize
        {
            get { return ((UIElement)ScreenCanvas.Parent).RenderSize; }
        }

        public string FunctionY
        {
            get { return TbYFunction.Text; }
            set { TbYFunction.Text = value; }
        }

        /// <summary>
        /// Clamp a point into the interval of the graph area
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>A clamped point</returns>
        private Point ClampedPoint(double x, double y)
        {
            if (double.IsPositiveInfinity(x) || x > CanvasWidth * 2) x = CanvasWidth * 2;
            else if (double.IsNegativeInfinity(x) || x < -CanvasWidth) x = -CanvasWidth;
            else if (double.IsNaN(x)) x = -CanvasWidth;
            if (double.IsPositiveInfinity(y) || y > CanvasHeight * 2) y = CanvasHeight * 2;
            else if (double.IsNegativeInfinity(y) || y < -CanvasHeight) y = -CanvasHeight;
            else if (double.IsNaN(x)) y = -CanvasHeight;
            return new Point(x, y);
        }

        /// <summary>
        /// Draw a line to the screen canvas
        /// </summary>
        /// <param name="x1">Start point x</param>
        /// <param name="y1">Start point y</param>
        /// <param name="x2">End point x</param>
        /// <param name="y2">End point y</param>
        /// <param name="dotted">if true, then the line is dotted</param>
        private void DrawLine(double x1, double y1, double x2, double y2, bool dotted)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Red,
                StrokeThickness = 1.0
            };
            if (dotted)
            {
                var collection = new DoubleCollection { 3, 3 };
                line.StrokeDashArray = collection;
            }
            ScreenCanvas.Children.Add(line);
        }

        /// <summary>
        /// Draw the axes
        /// </summary>
        /// <param name="xmin">x minimum</param>
        /// <param name="xmax">x maximum</param>
        /// <param name="ymin">y minimum</param>
        /// <param name="ymax">y maximum</param>
        public void DrawAxes(double? xmin, double? xmax, double? ymin, double? ymax)
        {
            var graphWidth = xmax - xmin;
            var graphHeight = ymax - ymin;
            var xOffset = (double)((xmin >= 0) ? 0.0 : (xmax <= 0) ? 1.0 : -xmin / graphWidth);
            var  yOffset = (double)((ymin >= 0) ? 1.0 : (ymax <= 0) ? 0.0 : ymax / graphHeight);
            xOffset = Math.Floor((double)(xOffset * (CanvasWidth - 0.0)));
            yOffset = Math.Floor((double)(yOffset * (CanvasHeight - 0.0)));

            // X axis
            DrawLine(xOffset, 0.0, xOffset, CanvasHeight, false);
            // Y axis
            DrawLine(0.0, yOffset, CanvasWidth, yOffset, false);
        }



        /// <summary>
        /// Draw a simple function
        /// </summary>
        private async void Draw()
        {
            try
            {
                if (string.IsNullOrEmpty(TbYFunction.Text))
                    throw new Exception("Invalid or empty function");

                var fncx = _engine.Compile(TbYFunction.Text);

                var width = CanvasWidth;
                var height = CanvasHeight;
                var offsetX = -MinX.Value;
                var offsetY = MaxY.Value;
                var graphToCanvasX = width / (MaxX.Value - MinX.Value);
                var graphToCanvasY = height / (MaxY.Value - MinY.Value);

                var points = new PointCollection();
                for (var x = MinX.Value; x < MaxX.Value; x += 1 / graphToCanvasX)
                {
                    _engine.MemoryManager.SetItem("x", x);
                    var xCanvas = (x + offsetX) * graphToCanvasX;
                    var y = await _engine.EvaluateCompiled(fncx);

                    if (!double.IsNaN(y))
                    {
                        var yCanvas = (offsetY - y) * graphToCanvasY;
                        points.Add(ClampedPoint((double)xCanvas, (double)yCanvas));
                    }
                }

                ScreenCanvas.Children.Clear();
                DrawAxes(MinX.Value, MaxX.Value, MinY.Value, MaxY.Value);
                var graphLine = new Polyline
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                    Points = points
                };
                ScreenCanvas.Children.Add(graphLine);
            }
            catch (Exception ex)
            {
                MainWindow.ErrorDialog(ex.Message);
            }
        }

        private async void Draw2D()
        {
            try
            {
                if (string.IsNullOrEmpty(Tb2DXFunction.Text))
                    throw new Exception("Invalid or empty function");

                if (string.IsNullOrEmpty(Tb2DYFunction.Text))
                    throw new Exception("Invalid or empty function");

                var fncx = _engine.Compile(Tb2DXFunction.Text);
                var fncy = _engine.Compile(Tb2DYFunction.Text);

                var width = CanvasWidth;
                var height = CanvasHeight;
                var graphToCanvasX = width / (Max2DX.Value - Min2DX.Value);
                var graphToCanvasY = height / (Max2DY.Value - Min2DY.Value);

                // distance from origin of graph to origin of canvas
                var offsetX = -Min2DX.Value;
                var offsetY = Max2DY.Value;

                var points = new PointCollection();
                for (var t = Min2Dt.Value; t <= Max2Dt.Value + 0.000001; t += Step2Dt.Value)
                {
                    _engine.MemoryManager.SetItem("t", t);
                    var x = await _engine.EvaluateCompiled(fncx);
                    var y = await _engine.EvaluateCompiled(fncy);

                    // Translate the origin based on the max/min parameters (y axis is flipped), then scale to canvas.
                    var xCanvas = (x + offsetX) * graphToCanvasX;
                    var yCanvas = (offsetY - y) * graphToCanvasY;

                    points.Add(ClampedPoint((double)xCanvas, (double)yCanvas));
                }

                ScreenCanvas.Children.Clear();
                DrawAxes(Min2DX.Value, Max2DX.Value, Min2DY.Value, Max2DY.Value);

                var polyLine = new Polyline
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                    Points = points
                };
                ScreenCanvas.Children.Add(polyLine);

            }
            catch (Exception ex)
            {
                MainWindow.ErrorDialog(ex.Message);
            }
        }

        #region UI
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            GridSettings.Visibility = Visibility.Collapsed;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            MinX.Value = -15;
            MinY.Value = -15;
            MaxX.Value = 15;
            MaxY.Value = 15;
            TbYFunction.Text = "";

            Min2DX.Value = -15;
            Min2DY.Value = -15;
            Max2DX.Value = 15;
            Max2DY.Value = 15;
            Min2Dt.Value = 0;
            Max2Dt.Value = 15;
            Step2Dt.Value = 1;
            Tb2DXFunction.Text = "";
            Tb2DYFunction.Text = "";
        }

        private void BtnPlot_Click(object sender, RoutedEventArgs e)
        {
            if (TabOptions.SelectedIndex == 0) Draw();
            else Draw2D();
            GridSettings.Visibility = Visibility.Collapsed;
        }

        private void BtnGraphOptions_Click(object sender, RoutedEventArgs e)
        {
            if (GridSettings.Visibility == Visibility.Visible) GridSettings.Visibility = Visibility.Collapsed;
            else GridSettings.Visibility = Visibility.Visible;
        }

        private void FunctionTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fnc = _source[FunctionTemplates.SelectedIndex];
            if (!fnc.Parametric)
            {
                Dispatcher.Invoke(() => { TabOptions.SelectedIndex = 0; });
                MinX.Value = fnc.XMin;
                MaxX.Value = fnc.XMax;
                MinY.Value = fnc.YMin;
                MaxY.Value = fnc.YMax;
                TbYFunction.Text = fnc.CodeY;
            }
            else
            {
                Dispatcher.Invoke(() => { TabOptions.SelectedIndex = 1; });
                Min2DX.Value = fnc.XMin;
                Max2DX.Value = fnc.XMax;
                Min2DY.Value = fnc.YMin;
                Max2DY.Value = fnc.YMax;
                Tb2DYFunction.Text = fnc.CodeY;
                Tb2DXFunction.Text = fnc.CodeX;
                Min2Dt.Value = fnc.tMin;
                Max2Dt.Value = fnc.tMax;
                Step2Dt.Value = fnc.tStep;
            }
        }
        #endregion

        #region zoom
        private void ScreenCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridSettings.Visibility == Visibility.Visible) return;
            switch (TabOptions.SelectedIndex)
            {
                case 0:
                case 1:
                    _selectionStarted = true;
                    _selectionStart = e.GetPosition(ScreenCanvas);
                    selection.Width = 0;
                    selection.Height = 0;
                    selection.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ScreenCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridSettings.Visibility == Visibility.Visible) return;
            if (_selectionStarted)
            {
                var zoomIn = new Rect(_selectionStart, e.GetPosition(ScreenCanvas));
                selection.Visibility = Visibility.Collapsed;
                if (zoomIn.Width <= 1 || zoomIn.Height <= 1) return;

                if (TabOptions.SelectedIndex == 0) ZoomViewportTo(zoomIn);
                else if (TabOptions.SelectedIndex == 1) ZoomViewport2DTo(zoomIn);

                _selectionStarted = false;
            }
        }

        private void ScreenCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (GridSettings.Visibility == Visibility.Visible) return;
            if (_selectionStarted)
            {
                var rect = new Rect(_selectionStart, e.GetPosition(ScreenCanvas));
                selection.RenderTransform = new TranslateTransform(rect.X, rect.Y);
                selection.Width = rect.Width;
                selection.Height = rect.Height;
            }
        }

        private void ScreenCanvas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridSettings.Visibility == Visibility.Visible) return;
            if (_selectionStarted)
            {
                _selectionStarted = false;
                selection.Visibility = Visibility.Collapsed;
            }
            else
            {
                var canvasSize = CanvasSize;
                var xBorder = canvasSize.Width / 2;
                var yBorder = canvasSize.Height / 2;

                var zoomOut = new Rect(-xBorder, -yBorder, xBorder * 4, yBorder * 4);

                switch (TabOptions.SelectedIndex)
                {
                    case 0:
                        ZoomViewportTo(zoomOut);
                        break;
                    case 1:
                        ZoomViewport2DTo(zoomOut);
                        break;
                }
            }
        }

        private void ZoomViewportTo(Rect canvasSelection)
        {
            var canvasSize = CanvasSize;
            var selectionOffset = new Vector(canvasSelection.X, canvasSelection.Y);
            var selectionScale = new Vector(canvasSelection.Width / canvasSize.Width,
                canvasSelection.Height / canvasSize.Height);

            var graphSize = new Size((double)(MaxX.Value - MinX.Value), (double)(MaxY.Value - MinY.Value));
            var canvasToGraphScale = new Vector(graphSize.Width / canvasSize.Width, graphSize.Height / canvasSize.Height);
            graphSize.Width *= selectionScale.X;
            graphSize.Height *= selectionScale.Y;
            var graphOffset = new Vector(selectionOffset.X * canvasToGraphScale.X, selectionOffset.Y * canvasToGraphScale.Y);
            var newViewport = new Rect((double)(MinX.Value + graphOffset.X), (double)(MaxY.Value - graphOffset.Y), graphSize.Width, graphSize.Height);

            MinX.Value = newViewport.Left;
            MaxX.Value = newViewport.Right;
            MinY.Value = newViewport.Top - graphSize.Height;
            MaxY.Value = newViewport.Top;

            this.Draw();
        }


        private void ZoomViewport2DTo(Rect canvasSelection)
        {
            var canvasSize = CanvasSize;
            var selectionOffset = new Vector(canvasSelection.X, canvasSelection.Y);
            var selectionScale = new Vector(canvasSelection.Width / canvasSize.Width,
                canvasSelection.Height / canvasSize.Height);

            var graphSize = new Size((double)(Max2DX.Value - Min2DX.Value), (double)(Max2DY.Value - Min2DY.Value));
            var canvasToGraphScale = new Vector(graphSize.Width / canvasSize.Width, graphSize.Height / canvasSize.Height);
            graphSize.Width *= selectionScale.X;
            graphSize.Height *= selectionScale.Y;
            var graphOffset = new Vector(selectionOffset.X * canvasToGraphScale.X, selectionOffset.Y * canvasToGraphScale.Y);
            var newViewport = new Rect((double)(Min2DX.Value + graphOffset.X), (double)(Max2DY.Value - graphOffset.Y), graphSize.Width, graphSize.Height);

            Min2DX.Value = newViewport.Left;
            Max2DX.Value = newViewport.Right;
            Min2DY.Value = newViewport.Top - graphSize.Height;
            Max2DY.Value = newViewport.Top;

            this.Draw2D();
        }

        #endregion
    }
}
