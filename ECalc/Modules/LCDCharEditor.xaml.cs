﻿using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ECalc.Modules
{
    /// <summary>
    /// Interaction logic for LCDCharEditor.xaml
    /// </summary>
    public partial class LCDCharEditor : UserControl
    {
        public LCDCharEditor()
        {
            InitializeComponent();
        }

        private bool isSegmentOn(Rectangle segment)
        {
            SolidColorBrush segmentfill = (SolidColorBrush)segment.Fill;
            if (segmentfill.Color == Colors.Black) return false;
            else return true;
        }

        private Rectangle GetRectangle(Grid grid, int row, int column)
        {
            return grid.Children.Cast<Rectangle>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);
        }


        private void Seg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle segment = (Rectangle)sender;
            SolidColorBrush segmentfill = (SolidColorBrush)segment.Fill;
            if (segmentfill.Color == Colors.Black) segment.Fill = new SolidColorBrush(Colors.Red);
            else segment.Fill = new SolidColorBrush(Colors.Black);
        }

        private void FormatOutput(int[] array)
        {
            StringBuilder sb = new StringBuilder("const char char[] = {");
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i].ToString());
                if (i != array.Length - 1) sb.Append(", ");
            }
            sb.Append("};");
            TbOutput.Text = sb.ToString();
        }

        private void Clear()
        {
            int rowlimit = 0;
            if (Type5x7.IsChecked == true) rowlimit = 7;
            else rowlimit = 10;
            for (int i = 0; i < rowlimit; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Rectangle r = GetRectangle(Matrix, i, j);
                    r.Fill = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void calculate(int rownum)
        {
            int[] rows = new int[rownum];
            Rectangle r;
            for (int i = 0; i < rownum; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    r = GetRectangle(Matrix, i, j);
                    if (isSegmentOn(r))
                    {
                        rows[i] += 1 << (4 - j);
                    }
                }
            }
            FormatOutput(rows);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Type5x7.IsChecked == true) calculate(7);
            else calculate(10);
        }


        private void ShowFull(bool value)
        {
            Visibility v = Visibility.Collapsed;
            if (value) v = Visibility.Visible;
            for (int i = 7; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Rectangle r = GetRectangle(Matrix, i, j);
                    r.Visibility = v;
                }
            }
        }

        private void SaveBmp(string target)
        {
            System.Drawing.Bitmap bmp;
            int rowlimit = 0;
            if (Type5x7.IsChecked == true)
            {
                bmp = new System.Drawing.Bitmap(5, 7, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                rowlimit = 7;
            }
            else
            {
                bmp = new System.Drawing.Bitmap(5, 10, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                rowlimit = 10;
            }
            for (int i = 0; i < rowlimit; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var r = GetRectangle(Matrix, i, j);
                    if (isSegmentOn(r)) bmp.SetPixel(j, i, System.Drawing.Color.Black);
                    else bmp.SetPixel(j, i, System.Drawing.Color.White);
                }
            }
            bmp.Save(target);
            bmp = null;
        }

        private void LoadBmp(string src)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(src))
            {
                if (bmp.Height == 7) ShowFull(false);
                else ShowFull(true);
                Clear();
                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        var c = bmp.GetPixel(j, i);
                        var r = GetRectangle(Matrix, i, j);
                        if (c.B == 0 && c.R == 0 && c.G == 0) r.Fill = new SolidColorBrush(Colors.Red);
                        else r.Fill = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void BtnLoadBMP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.Filter = "Bitmap files | *.bmp";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadBmp(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                MainWindow.ErrorDialog(ex.Message);
            }
        }

        private void BtnSaveBmp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.DefaultExt = "*.bmp";
                sfd.Filter = "Bitmap files | *.bmp";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SaveBmp(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MainWindow.ErrorDialog(ex.Message);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Type5x7.IsChecked == true) ShowFull(false);
            else ShowFull(true);
        }

    }
}
