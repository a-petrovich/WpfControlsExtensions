﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfControlExtensions.GridExtensions
{
    public class DynamicGrid
    {
        #region SeparatorColor

        /// <summary>
        /// Set color of separators
        /// </summary>
        public static readonly DependencyProperty SeparatorsBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "SeparatorsBackground", typeof(System.Windows.Media.Brush), typeof(DynamicGrid),
                new PropertyMetadata(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black), SeparatorsBackgroundChanged));

        private static void SeparatorsBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || !(e.NewValue is System.Windows.Media.Brush color))
                return;

            Grid grid = (Grid)obj;
            foreach (var child in grid.Children)
            {
                if (child is GridSplitter gs)
                {
                    gs.Background = color;
                }
            }
        }

        public static System.Windows.Media.Brush GetSeparatorsBackground(DependencyObject obj)
        {
            return (System.Windows.Media.Brush)obj.GetValue(SeparatorsBackgroundProperty);
        }

        public static void SetSeparatorsBackground(DependencyObject obj, System.Windows.Media.Brush value)
        {
            obj.SetValue(SeparatorsBackgroundProperty, value);
        }

        #endregion

        #region DoNeedSeparators

        /// <summary>
        /// Set true if you need separators
        /// </summary>
        public static readonly DependencyProperty AddVerticalSeparatorsProperty = 
            DependencyProperty.RegisterAttached(
                "AddVerticalSeparators", typeof(bool), typeof(DynamicGrid),
                new PropertyMetadata(true, AddVerticalSeparatorsChanged));

        private static void AddVerticalSeparatorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || !(e.NewValue is bool needToAddSeparators))
                return;

            Grid grid = (Grid)obj;

            List<VerticalSplitter> splitters = new List<VerticalSplitter>();

            foreach (var child in grid.Children)
            {
                if (child is VerticalSplitter spl)
                    splitters.Add(spl);
            }

            foreach (var child in splitters)
            {
                grid.Children.Remove(child);
            }

            splitters.Clear();
            
            grid.ColumnDefinitions.Clear();
            
            for (int i = 0; i < GetColumnCount(obj) * 2 - 1; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            SetStarColumns(grid, needToAddSeparators, GetSeparatorsBackground(obj));
        }

        public static bool GetAddVerticalSeparators(DependencyObject obj)
        {
            return (bool)obj.GetValue(AddVerticalSeparatorsProperty);
        }

        public static void SetAddVerticalSeparators(DependencyObject obj, bool value)
        {
            obj.SetValue(AddVerticalSeparatorsProperty, value);
        }

        /// <summary>
        /// Set true if you need separators
        /// </summary>
        public static readonly DependencyProperty AddHorizontalSeparatorsProperty =
            DependencyProperty.RegisterAttached(
                "AddHorizontalSeparators", typeof(bool), typeof(DynamicGrid),
                new PropertyMetadata(true, AddHorizontalSeparatorsChanged));

        private static void AddHorizontalSeparatorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || !(e.NewValue is bool needToAddSeparators))
                return;

            Grid grid = (Grid)obj;

            List<HorizontalSplitter> splitters = new List<HorizontalSplitter>();

            foreach (var child in grid.Children)
            {
                if (child is HorizontalSplitter spl)
                    splitters.Add(spl);
            }

            foreach (var child in splitters)
            {
                grid.Children.Remove(child);
            }

            splitters.Clear();

            grid.RowDefinitions.Clear();

            for (int i = 0; i < GetRowCount(obj) * 2 - 1; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            
            SetStarRows(grid, needToAddSeparators, GetSeparatorsBackground(obj));
        }

        public static bool GetAddHorizontalSeparators(DependencyObject obj)
        {
            return (bool)obj.GetValue(AddHorizontalSeparatorsProperty);
        }

        public static void SetAddHorizontalSeparators(DependencyObject obj, bool value)
        {
            obj.SetValue(AddHorizontalSeparatorsProperty, value);
        }

        #endregion

        #region RowCount Property

        /// <summary>
        /// Adds the specified number of Rows to RowDefinitions.
        /// Default Height is Auto
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached(
                "RowCount", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(1, RowCountChanged));

        public static int GetRowCount(DependencyObject obj)
        {
            return (int)obj.GetValue(RowCountProperty);
        }

        public static void SetRowCount(DependencyObject obj, int value)
        {
            obj.SetValue(RowCountProperty, value);
        }

        // Change Event - Adds the Rows
        public static void RowCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || (int)e.NewValue < 0)
                return;

            Grid grid = (Grid)obj;

            List<HorizontalSplitter> splitters = new List<HorizontalSplitter>();

            foreach (var child in grid.Children)
            {
                if (child is HorizontalSplitter spl)
                    splitters.Add(spl);
            }

            foreach (var child in splitters)
            {
                grid.Children.Remove(child);
            }

            grid.RowDefinitions.Clear();

            for (int i = 0; i < (int)e.NewValue * 2 - 1; i++)
                grid.RowDefinitions.Add(
                    new RowDefinition() { Height = GridLength.Auto });

            SetStarRows(grid, GetAddHorizontalSeparators(obj), GetSeparatorsBackground(obj));
        }

        #endregion

        #region ColumnCount Property

        /// <summary>
        /// Adds the specified number of Columns to ColumnDefinitions. 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.RegisterAttached(
                "ColumnCount", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(1, ColumnCountChanged));

        public static int GetColumnCount(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnCountProperty);
        }

        public static void SetColumnCount(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnCountProperty, value);
        }

        // Change Event - Add the Columns
        public static void ColumnCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || (int)e.NewValue < 0)
                return;

            Grid grid = (Grid)obj;

            List<VerticalSplitter> splitters = new List<VerticalSplitter>();

            foreach (var child in grid.Children)
            {
                if (child is VerticalSplitter spl)
                    splitters.Add(spl);
            }

            foreach (var child in splitters)
            {
                grid.Children.Remove(child);
            }

            splitters.Clear();

            grid.ColumnDefinitions.Clear();

            for (int i = 0; i < (int)e.NewValue * 2 - 1; i++)
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = GridLength.Auto });

            SetStarColumns(grid, GetAddVerticalSeparators(obj), GetSeparatorsBackground(obj));
        }

        #endregion

        #region StarRows Property

        /// <summary>
        /// Makes the specified Row's Height equal to Star. 
        /// Can set on multiple Rows
        /// </summary>
        public static readonly DependencyProperty StarRowsProperty =
            DependencyProperty.RegisterAttached(
                "StarRows", typeof(string), typeof(DynamicGrid),
                new PropertyMetadata(string.Empty, StarRowsChanged));

        public static string GetStarRows(DependencyObject obj)
        {
            return (string)obj.GetValue(StarRowsProperty);
        }

        public static void SetStarRows(DependencyObject obj, string value)
        {
            obj.SetValue(StarRowsProperty, value.ToString());
        }

        // Change Event - Makes specified Row's Height equal to Star
        public static void StarRowsChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
                return;

            SetStarRows((Grid)obj, GetAddHorizontalSeparators(obj), GetSeparatorsBackground(obj));
        }

        #endregion

        #region StarColumns Property

        /// <summary>
        /// Makes the specified Column's Width equal to Star. 
        /// Can set on multiple Columns
        /// </summary>
        public static readonly DependencyProperty StarColumnsProperty =
            DependencyProperty.RegisterAttached(
                "StarColumns", typeof(object), typeof(DynamicGrid),
                new PropertyMetadata(string.Empty, StarColumnsChanged));

        public static string GetStarColumns(DependencyObject obj)
        {
            return (string)obj.GetValue(StarColumnsProperty);
        }

        public static void SetStarColumns(DependencyObject obj, string value)
        {
            obj.SetValue(StarColumnsProperty, value.ToString());
        }

        // Change Event - Makes specified Column's Width equal to Star
        public static void StarColumnsChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
                return;

            SetStarColumns((Grid)obj, GetAddVerticalSeparators(obj), GetSeparatorsBackground(obj));
        }

        #endregion

        private static void SetStarColumns(Grid grid, bool needToAddSeparators, System.Windows.Media.Brush brush)
        {
            var compare = new StringTrimmedCompare();
            bool all = GetStarColumns(grid).Equals("All");
            string[] starColumns = GetStarColumns(grid).Split(',');

            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                if (needToAddSeparators)
                {
                    if (i % 2 != 0)
                    {
                        grid.ColumnDefinitions[i].Width = GridLength.Auto;

                        var gs = new VerticalSplitter
                        {
                            Background = brush
                        };
                        gs.SetValue(Grid.ColumnProperty, i);
                        gs.ResizeBehavior = GridResizeBehavior.PreviousAndNext;
                        gs.Width = 2;
                        gs.HorizontalAlignment = HorizontalAlignment.Stretch;
                        gs.VerticalAlignment = VerticalAlignment.Stretch;

                        grid.Children.Add(gs);
                    }
                    else
                    {
                        if (all || starColumns.Contains(i.ToString(), compare))
                            grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
                        else
                            grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Auto);
                    }
                }
                else
                {
                    if (all || starColumns.Contains(i.ToString(), compare))
                        grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
                    else
                        grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Auto);
                }
            }
        }

        private static void SetStarRows(Grid grid, bool needToAddSeparators, System.Windows.Media.Brush brush)
        {
            var compare = new StringTrimmedCompare();
            bool all = GetStarRows(grid).Equals("All");
            string[] starRows = GetStarRows(grid).Split(',');

            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                if (needToAddSeparators)
                {
                    if (i % 2 != 0)
                    {
                        grid.RowDefinitions[i].Height = GridLength.Auto;

                        var gs = new HorizontalSplitter
                        {
                            Background = brush
                        };
                        gs.SetValue(Grid.RowProperty, i);
                        gs.Background = brush;
                        gs.ResizeBehavior = GridResizeBehavior.PreviousAndNext;
                        gs.Height = 2;
                        gs.HorizontalAlignment = HorizontalAlignment.Stretch;
                        gs.VerticalAlignment = VerticalAlignment.Stretch;

                        grid.Children.Add(gs);
                    }
                    else
                    {
                        if (all || starRows.Contains(i.ToString(), compare))
                            grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
                        else
                            grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Auto);
                    }
                }
                else
                {
                    if (all || starRows.Contains(i.ToString(), compare))
                        grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
                    else
                        grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Auto);
                }
            }
        }

        class StringTrimmedCompare : EqualityComparer<string>
        {
            public override bool Equals(string s1, string s2)
            {
                return s1.Trim().Equals(s2.Trim());
            }

            public override int GetHashCode(string bx)
            {
                return bx.GetHashCode();
            }
        }
    }

    public class VerticalSplitter : GridSplitter
    { }
    public class HorizontalSplitter : GridSplitter
    { }
}
