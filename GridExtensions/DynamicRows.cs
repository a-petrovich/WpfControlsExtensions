using System;
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
        private struct RowColumnDefinition
        {
            public RowColumnDefinition(int row, int column)
            {
                Row = row;
                Column = column;
            }

            public int Row { get; }
            public int Column { get; }
        }

        #region Changes user rows and columns definitions for childrens

        private static readonly Dictionary<object, RowColumnDefinition> ChildrenRowDefinitions = new Dictionary<object, RowColumnDefinition>();

        #endregion

        #region SplitterColor

        /// <summary>
        /// Set color of splitters
        /// </summary>
        public static readonly DependencyProperty SplitterBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "SplitterBackground", typeof(System.Windows.Media.Brush), typeof(DynamicGrid),
                new PropertyMetadata(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black), SplitterBackgroundChanged));

        private static void SplitterBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
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

        public static System.Windows.Media.Brush GetSplitterBackground(DependencyObject obj)
        {
            return (System.Windows.Media.Brush)obj.GetValue(SplitterBackgroundProperty);
        }

        public static void SetSplitterBackground(DependencyObject obj, System.Windows.Media.Brush value)
        {
            obj.SetValue(SplitterBackgroundProperty, value);
        }

        #endregion

        #region SplitterWidth

        public static readonly DependencyProperty SplitterWidthProperty =
            DependencyProperty.RegisterAttached(
                "SplitterWidth", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(1, SplitterWidthPropertyChanged));

        private static void SplitterWidthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid grid) || !(e.NewValue is int newWidth))
                return;

            foreach (var spl in grid.Children)
            {
                if (spl is VerticalSplitter vspl)
                {
                    vspl.Width = newWidth;
                    continue;
                }
                if (spl is HorizontalSplitter hspl)
                {
                    hspl.Height = newWidth;
                    continue;
                }
            }
        }

        public static int GetSplitterWidth(DependencyObject obj)
        {
            return (int)obj.GetValue(SplitterWidthProperty);
        }

        public static void SetSplitterWidth(DependencyObject obj, int newWidth)
        {
            obj.SetValue(SplitterWidthProperty, newWidth);
        }

        #endregion

        #region DoNeedSplitter

        /// <summary>
        /// Set true if you need splitters
        /// </summary>
        public static readonly DependencyProperty AddVerticalSplittersProperty = 
            DependencyProperty.RegisterAttached(
                "AddVerticalSplitters", typeof(bool), typeof(DynamicGrid),
                new PropertyMetadata(true, AddVerticalSplittersChanged));

        private static void AddVerticalSplittersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
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

            var newValue = needToAddSeparators ? GetColumnCount(obj) * 2 - 1 : GetColumnCount(obj);

            for (int i = 0; i < newValue; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            PositionChildrens(obj);

            SetStarColumns(obj, needToAddSeparators, GetSplitterBackground(obj), GetVerticalSplitterSpan(obj));
        }

        public static bool GetAddVerticalSplitters(DependencyObject obj)
        {
            return (bool)obj.GetValue(AddVerticalSplittersProperty);
        }

        public static void SetAddVerticalSplitters(DependencyObject obj, bool value)
        {
            obj.SetValue(AddVerticalSplittersProperty, value);
        }

        /// <summary>
        /// Set true if you need separators
        /// </summary>
        public static readonly DependencyProperty AddHorizontalSplittersProperty =
            DependencyProperty.RegisterAttached(
                "AddHorizontalSplitters", typeof(bool), typeof(DynamicGrid),
                new PropertyMetadata(true, AddHorizontalSplittersChanged));

        private static void AddHorizontalSplittersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
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

            var newValue = needToAddSeparators ? GetRowCount(obj) * 2 - 1 : GetRowCount(obj);

            for (int i = 0; i < newValue; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            PositionChildrens(obj);

            SetStarRows(obj, needToAddSeparators, GetSplitterBackground(obj), GetHorizontalSplitterSpan(obj));
        }

        public static bool GetAddHorizontalSplitters(DependencyObject obj)
        {
            return (bool)obj.GetValue(AddHorizontalSplittersProperty);
        }

        public static void SetAddHorizontalSplitters(DependencyObject obj, bool value)
        {
            obj.SetValue(AddHorizontalSplittersProperty, value);
        }

        #endregion

        #region HorizontalSplitterSpan

        /// <summary>
        /// Set columnspan for Splitter
        /// </summary>
        public static readonly DependencyProperty HorizontalSplitterSpanProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalSplitterSpan", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(1, HorizontalSplitterSpanChanged));

        private static void HorizontalSplitterSpanChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || !(e.NewValue is int colSpan))
                return;

            if (colSpan < 1)
            {
                SetHorizontalSplitterSpan(obj, 1);
                return;
            }

            Grid grid = (Grid)obj;

            SetSplitterSpan(grid, GetAddHorizontalSplitters(obj) ? colSpan*2-1 : colSpan);
        }

        private static void SetSplitterSpan(Grid grid, int span)
        {
            foreach (var child in grid.Children)
            {
                if (child is HorizontalSplitter spl)
                {
                    spl.SetValue(Grid.ColumnSpanProperty, span);
                }
            }
        }

        public static int GetHorizontalSplitterSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(HorizontalSplitterSpanProperty);
        }

        public static void SetHorizontalSplitterSpan(DependencyObject obj, int value)
        {
            obj.SetValue(HorizontalSplitterSpanProperty, value);
        }
        #endregion

        #region VerticalSplitterSpan

        /// <summary>
        /// Set true if you need Splitters
        /// </summary>
        public static readonly DependencyProperty VerticalSplitterSpanProperty =
            DependencyProperty.RegisterAttached(
                "VerticalSplitterSpan", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(1, VerticalSplitterSpanChanged));

        private static void VerticalSplitterSpanChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid grid) || !(e.NewValue is int rowSpan))
                return;
            if (rowSpan < 1)
            {
                SetVerticalSplitterSpan(obj, 1);
                return;
            }
            SetVerticalSpan(grid, GetAddVerticalSplitters(obj) ? rowSpan*2-1 : rowSpan);
        }

        public static int GetVerticalSplitterSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(VerticalSplitterSpanProperty);
        }

        public static void SetVerticalSplitterSpan(DependencyObject obj, int value)
        {
            obj.SetValue(VerticalSplitterSpanProperty, value);
        }

        private static void SetVerticalSpan(Grid grid, int span)
        {
            foreach (var child in grid.Children)
            {
                if (child is VerticalSplitter spl)
                {
                    spl.SetValue(Grid.RowSpanProperty, span);
                }
            }
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

            var newCount = GetAddHorizontalSplitters(obj) ? (int)e.NewValue * 2 - 1 : (int)e.NewValue;

            for (int i = 0; i < newCount; i++)
                grid.RowDefinitions.Add(
                    new RowDefinition() { Height = GridLength.Auto });

            PositionChildrens(obj);

            SetStarRows(obj, GetAddHorizontalSplitters(obj), GetSplitterBackground(obj), GetHorizontalSplitterSpan(obj));
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

            var newCount = GetAddVerticalSplitters(obj) ? (int)e.NewValue * 2 - 1 : (int)e.NewValue;

            for (int i = 0; i < newCount; i++)
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = GridLength.Auto });

            PositionChildrens(obj);

            SetStarColumns(obj, GetAddVerticalSplitters(obj), GetSplitterBackground(obj), GetVerticalSplitterSpan(obj));
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

            SetStarRows(obj, GetAddHorizontalSplitters(obj), GetSplitterBackground(obj), GetHorizontalSplitterSpan(obj));
        }

        private static void SetStarRows(DependencyObject obj, bool needToAddSeparators, System.Windows.Media.Brush brush, int? colSpan)
        {
            if (!(obj is Grid grid))
                return;
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
                        gs.SetValue(Grid.ColumnSpanProperty, colSpan.HasValue ? colSpan.Value * 2 - 1 : grid.ColumnDefinitions.Count);
                        gs.Background = brush;
                        gs.ResizeBehavior = GridResizeBehavior.PreviousAndNext;
                        gs.Height = GetSplitterWidth(obj);
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

            SetStarColumns(obj, GetAddVerticalSplitters(obj), GetSplitterBackground(obj), GetVerticalSplitterSpan(obj));
        }

        private static void SetStarColumns(DependencyObject obj, bool needToAddSeparators, System.Windows.Media.Brush brush, int? rowSpan)
        {
            if (!(obj is Grid grid))
                return;

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
                        gs.SetValue(Grid.RowSpanProperty, rowSpan.HasValue ? rowSpan.Value * 2 - 1 : grid.RowDefinitions.Count);
                        gs.ResizeBehavior = GridResizeBehavior.BasedOnAlignment;
                        gs.Width = GetSplitterWidth(obj);
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

        #endregion

        private static void PositionChildrens(DependencyObject obj)
        {
            if (!(obj is Grid grid))
                return;

            bool AddingVertical = GetAddVerticalSplitters(obj);
            bool AddingHorizontal = GetAddHorizontalSplitters(obj);

            foreach (var child in grid.Children)
            {
                if (!(child is DependencyObject dobj))
                    return;

                if (child is VerticalSplitter || child is HorizontalSplitter)
                    continue;
                
                lock (ChildrenRowDefinitions)
                {
                    if (!ChildrenRowDefinitions.ContainsKey(child)) // if it's not in list - add it
                        ChildrenRowDefinitions[child] = new RowColumnDefinition(
                            (int)(child as DependencyObject).GetValue(Grid.RowProperty), 
                            (int)(child as DependencyObject).GetValue(Grid.ColumnProperty));
                }

                // If we add splitters, we need to move all user controls with row % 2 = 0 to the next grid column
                dobj.SetValue(Grid.ColumnProperty, AddingVertical ? ChildrenRowDefinitions[child].Column * 2 : ChildrenRowDefinitions[child].Column);
                dobj.SetValue(Grid.RowProperty, AddingHorizontal ? ChildrenRowDefinitions[child].Row * 2 : ChildrenRowDefinitions[child].Row);
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
