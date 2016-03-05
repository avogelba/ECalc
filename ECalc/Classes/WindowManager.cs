﻿using ECalc.Api.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ECalc.Classes
{
    /// <summary>
    /// Window management class
    /// </summary>
    internal static class WindowManager
    {
        private static List<Window> _childs;

        static WindowManager()
        {
            _childs = new List<Window>();
        }

        /// <summary>
        /// Brings a window to the front
        /// </summary>
        /// <param name="w">Window to bring to front</param>
        public static void BringToFront(Window w)
        {
            if (w.WindowState == WindowState.Minimized)
                w.WindowState = WindowState.Normal;
            w.Activate();
        }

        /// <summary>
        /// Begins tracking of a child window
        /// </summary>
        /// <param name="w">Window to track</param>
        public static void RegisterChild(Window w)
        {
            _childs.Add(w);
        }

        /// <summary>
        /// Closes all child windows
        /// </summary>
        public static void CloseAll()
        {
            while (_childs.Count > 0)
            {
                var child = _childs[0];
                _childs.RemoveAt(0);
                child.Close();
                child = null;
            }
        }

        /// <summary>
        /// Stops tracking of a child window
        /// </summary>
        /// <param name="w"></param>
        public static void UnRegisterChild(Window w)
        {
            _childs.Remove(w);
        }

        /// <summary>
        /// Minimizes all tracked child windows
        /// </summary>
        public static void MinimizeAll()
        {
            foreach (var window in _childs)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        /// <summary>
        /// Restores all tracked child windows
        /// </summary>
        public static void RestoreAll()
        {
            foreach (var window in _childs)
            {
                window.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Returns a window based on it's index in the _childs collection
        /// </summary>
        /// <param name="index">index of the window</param>
        /// <returns>a Window instance</returns>
        public static Window GetWindow(int index)
        {
            return _childs[index];
        }

        /// <summary>
        /// Renders the previews for each child window 
        /// </summary>
        public static ObservableCollection<WindowData> Previews
        {
            get
            {
                var ret = new ObservableCollection<WindowData>();

                foreach (var window in _childs)
                {
                    ret.Add(new WindowData()
                    {
                        Image = window.Thumbnail(),
                        Title = window.Title
                    });
                }

                return ret;
            }
        }
    }

    internal class WindowData
    {
        public string Title { get; set; }
        public ImageSource Image { get; set; }
    }
}