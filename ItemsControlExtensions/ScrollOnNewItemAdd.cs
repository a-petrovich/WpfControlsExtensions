using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlExtensions.ItemsControlExtensions
{
    /// <summary>
    /// Add autoscroll in the specified direction, when new item added to the collection
    /// </summary>
    public class AutoScrollBehavior
    {
        /// <summary>
        /// Store the dictionarry of all ItemsControl that have AttachedBehavour attached.
        /// </summary>
        private static readonly Dictionary<ItemsControl, AutoScrollBehavour> AddedItemControlsDictionary = new Dictionary<ItemsControl, AutoScrollBehavour>();

        public static bool GetScrollOnNewItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollOnNewItemProperty);
        }

        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollOnNewItemProperty, value);
        }

        /// <summary>
        /// If true, ItemsControll will scroll to the ScrollOnNewItemDirection
        /// </summary>
        public static readonly DependencyProperty ScrollOnNewItemProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOnNewItem",
                typeof(bool),
                typeof(AutoScrollBehavior),
                new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

        /// <summary>
        /// Direction of auto-scroll
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnScrollOnNewItemChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl itemsControl)) return;
            bool oldValue = (bool)e.OldValue, newValue = (bool)e.NewValue;
            if (newValue == oldValue) return;

            if (!AddedItemControlsDictionary.ContainsKey(itemsControl))
            {
                itemsControl.Unloaded += AssociatedObjectOnUnloaded;

                AddedItemControlsDictionary[itemsControl] = new AutoScrollBehavour(itemsControl);
            }
            AddedItemControlsDictionary[itemsControl].SetAutoScroll(newValue);
        }

        /// <summary>
        /// Direction to scroll
        /// </summary>
        public static readonly DependencyProperty ScrollOnNewItemDirectionProperty =
                DependencyProperty.RegisterAttached(
        "ScrollOnNewItemDirection",
        typeof(ScrollOnNewItemDirectionType),
        typeof(AutoScrollBehavior),
        new UIPropertyMetadata(OnScrollOnNewItemDirectionChanged));

        /// <summary>
        /// Will attach ScrollOnNewItem if it was not, with thw default "false" value and set the direction of scroll.
        /// Or will just set the direction.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnScrollOnNewItemDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl itemsControl)) return;
            ScrollOnNewItemDirectionType oldValue = (ScrollOnNewItemDirectionType)e.OldValue, newValue = (ScrollOnNewItemDirectionType)e.NewValue;
            if (newValue == oldValue) return;

            if (!AddedItemControlsDictionary.ContainsKey(itemsControl))
            {
                itemsControl.Unloaded += AssociatedObjectOnUnloaded;

                AddedItemControlsDictionary[itemsControl] = new AutoScrollBehavour(itemsControl);
            }
            AddedItemControlsDictionary[itemsControl].SetScrollDirection(newValue);
        }

        /// <summary>
        /// When itemsControl.Unloaded fires, will dispose AutoScrollBehavour and remove itemsControl from AddedItemControlsDictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AssociatedObjectOnUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                if (AddedItemControlsDictionary.ContainsKey(itemsControl))
                {
                    itemsControl.Unloaded -= AssociatedObjectOnUnloaded;

                    AddedItemControlsDictionary[itemsControl].Dispose();
                    AddedItemControlsDictionary.Remove(itemsControl);
                }
            }
        }

        public static ScrollOnNewItemDirectionType GetScrollOnNewItemDirection(DependencyObject obj)
        {
            return (ScrollOnNewItemDirectionType)obj.GetValue(ScrollOnNewItemProperty);
        }

        public static void SetScrollOnNewItemDirection(DependencyObject obj, ScrollOnNewItemDirectionType value)
        {
            obj.SetValue(ScrollOnNewItemProperty, value);
        }
    }

    internal class AutoScrollBehavour : IDisposable
    {
        private ItemsControl AssociatedObject;
        private ScrollViewer scrollViewer;
        
        /// <summary>
        /// Internal autoscroll flag to prevent auto-scrolling when user interacts with ItemsControl
        /// </summary>
        private bool autoScroll = true;
        private bool justWheeled = false;
        private bool userInteracting = false;

        /// <summary>
        /// Auto-scroll flag that can be set to on/off.
        /// </summary>
        private bool autoscrollChecked = false;

        /// <summary>
        /// Auto-scroll direction.
        /// </summary>
        private ScrollOnNewItemDirectionType scrollDirection = ScrollOnNewItemDirectionType.Down;

        /// <summary>
        /// Will hold ref to control and do auto-scrolling.
        /// Disposable.
        /// </summary>
        /// <param name="control"></param>
        public AutoScrollBehavour(ItemsControl control)
        {
            AssociatedObject = control;

            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            AssociatedObject.Unloaded += AssociatedObjectOnUnloaded;

            if (AssociatedObject.IsLoaded)
            {
                AttachEvents();
            }
        }

        private void AssociatedObjectOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DetachEvents();
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            AttachEvents();
        }

        public void AttachEvents()
        {
            scrollViewer = GetScrollViewer(AssociatedObject);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;

                if (AssociatedObject is System.Windows.Controls.Primitives.Selector selector)
                {
                    selector.SelectionChanged += AssociatedObjectOnSelectionChanged;
                }
                AssociatedObject.ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorItemsChanged;
                AssociatedObject.GotMouseCapture += AssociatedObject_GotMouseCapture;
                AssociatedObject.LostMouseCapture += AssociatedObject_LostMouseCapture;
                AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
            }
        }

        public void DetachEvents()
        {
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged -= ScrollViewerOnScrollChanged;
            }
            if (AssociatedObject is System.Windows.Controls.Primitives.Selector selector)
            {
                selector.SelectionChanged -= AssociatedObjectOnSelectionChanged;
            }
            AssociatedObject.ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorItemsChanged;
            AssociatedObject.GotMouseCapture -= AssociatedObject_GotMouseCapture;
            AssociatedObject.LostMouseCapture -= AssociatedObject_LostMouseCapture;
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;

            scrollViewer = null;
        }

        private static ScrollViewer GetScrollViewer(DependencyObject root)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < childCount; ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                if (child is ScrollViewer sv)
                    return sv;
                if (i == childCount - 1)
                    return GetScrollViewer(child);
            }
            return null;
        }

        void AssociatedObject_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // User is actively interacting with listbox. Do not allow automatic scrolling to interfere with user experience.
            userInteracting = true;
            autoScroll = false;
        }

        void AssociatedObject_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // User is done interacting with control.
            userInteracting = false;
        }

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // diff is exactly zero if the last item in the list is visible. This can occur because of scroll-bar drag, mouse-wheel, or keyboard event.
            double diff = (scrollViewer.VerticalOffset - (scrollViewer.ExtentHeight - scrollViewer.ViewportHeight));

            // User just wheeled; this event is called immediately afterwards.
            if (justWheeled && diff != 0.0)
            {
                justWheeled = false;
                autoScroll = false;
                return;
            }

            if (diff == 0.0)
            {
                // then assume user has finished with interaction and has indicated through this action that scrolling should continue automatically.
                autoScroll = true;
            }
        }

        private void ItemContainerGeneratorItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                // An item was added to the listbox, or listbox was cleared.
                if (autoScroll && !userInteracting && autoscrollChecked)
                {
                    // If automatic scrolling is turned on, scroll to the bottom to bring new item into view.
                    // Do not do this if the user is actively interacting with the listbox.
                    switch (scrollDirection)
                    {
                        case ScrollOnNewItemDirectionType.Up:
                            scrollViewer.ScrollToTop();
                            break;
                        case ScrollOnNewItemDirectionType.Left:
                            scrollViewer.ScrollToLeftEnd();
                            break;
                        case ScrollOnNewItemDirectionType.Right:
                            scrollViewer.ScrollToRightEnd();
                            break;
                        case ScrollOnNewItemDirectionType.Down:
                            scrollViewer.ScrollToBottom();
                            break;
                        default:
                            scrollViewer.ScrollToBottom();
                            break;
                    }
                }
            }
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            // User selected (clicked) an item, or used the keyboard to select a different item. 
            // Turn off automatic scrolling.
            autoScroll = false;
        }

        void AssociatedObject_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // User wheeled the mouse. 
            // Cannot detect whether scroll viewer right at the bottom, because the scroll event has not occurred at this point.
            // Same for bubbling event.
            // Just indicated that the user mouse-wheeled, and that the scroll viewer should decide whether or not to stop autoscrolling.
            justWheeled = true;
        }

        public void SetAutoScroll(bool on)
        {
            autoscrollChecked = on;
        }

        public void SetScrollDirection(ScrollOnNewItemDirectionType direction)
        {
            scrollDirection = direction;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DetachEvents();
                    if (AssociatedObject != null)
                    {
                        AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
                        AssociatedObject.Unloaded -= AssociatedObjectOnUnloaded;
                        AssociatedObject = null;
                    }
                }
                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    public enum ScrollOnNewItemDirectionType
    {
        Down = 1,
        Up = 2,
        Right = 3,
        Left = 4
    }
}
