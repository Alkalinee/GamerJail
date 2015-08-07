using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Native;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This custom popup is used by the validation error template.
    /// It provides some additional nice features:
    ///     - repositioning if host-window size or location changed
    ///     - repositioning if host-window gets maximized and vice versa
    ///     - it's only topmost if the host-window is activated
    /// </summary>
    public class CustomValidationPopup : Popup
    {
        private Window hostWindow;

        public CustomValidationPopup()
        {
            Loaded += CustomValidationPopup_Loaded;
            Opened += CustomValidationPopup_Opened;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsOpen = false;
        }

        private void CustomValidationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var target = PlacementTarget as FrameworkElement;
            if (target == null)
            {
                return;
            }

            hostWindow = Window.GetWindow(target);
            if (hostWindow == null)
            {
                return;
            }

            hostWindow.LocationChanged -= hostWindow_SizeOrLocationChanged;
            hostWindow.LocationChanged += hostWindow_SizeOrLocationChanged;
            hostWindow.SizeChanged -= hostWindow_SizeOrLocationChanged;
            hostWindow.SizeChanged += hostWindow_SizeOrLocationChanged;
            target.SizeChanged -= hostWindow_SizeOrLocationChanged;
            target.SizeChanged += hostWindow_SizeOrLocationChanged;
            hostWindow.StateChanged -= hostWindow_StateChanged;
            hostWindow.StateChanged += hostWindow_StateChanged;
            hostWindow.Activated -= hostWindow_Activated;
            hostWindow.Activated += hostWindow_Activated;
            hostWindow.Deactivated -= hostWindow_Deactivated;
            hostWindow.Deactivated += hostWindow_Deactivated;

            Unloaded -= CustomValidationPopup_Unloaded;
            Unloaded += CustomValidationPopup_Unloaded;
        }

        private void CustomValidationPopup_Opened(object sender, EventArgs e)
        {
            SetTopmostState(true);
        }

        private void hostWindow_Activated(object sender, EventArgs e)
        {
            SetTopmostState(true);
        }

        private void hostWindow_Deactivated(object sender, EventArgs e)
        {
            SetTopmostState(false);
        }

        private void CustomValidationPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            var target = PlacementTarget as FrameworkElement;
            if (target != null)
            {
                target.SizeChanged -= hostWindow_SizeOrLocationChanged;
            }
            if (hostWindow != null)
            {
                hostWindow.LocationChanged -= hostWindow_SizeOrLocationChanged;
                hostWindow.SizeChanged -= hostWindow_SizeOrLocationChanged;
                hostWindow.StateChanged -= hostWindow_StateChanged;
                hostWindow.Activated -= hostWindow_Activated;
                hostWindow.Deactivated -= hostWindow_Deactivated;
            }
            Unloaded -= CustomValidationPopup_Unloaded;
            Opened -= CustomValidationPopup_Opened;
            hostWindow = null;
        }

        private void hostWindow_StateChanged(object sender, EventArgs e)
        {
            if (hostWindow != null && hostWindow.WindowState != WindowState.Minimized)
            {
                var target = PlacementTarget as FrameworkElement;
                var holder = target != null ? target.DataContext as AdornedElementPlaceholder : null;
                if (holder != null && holder.AdornedElement != null)
                {
                    PopupAnimation = PopupAnimation.None;
                    IsOpen = false;
                    var errorTemplate = holder.AdornedElement.GetValue(Validation.ErrorTemplateProperty);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
                }
            }
        }

        private void hostWindow_SizeOrLocationChanged(object sender, EventArgs e)
        {
            var offset = HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            HorizontalOffset = offset + 1;
            HorizontalOffset = offset;
        }

        private bool? appliedTopMost;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private void SetTopmostState(bool isTop)
        {
            // Don’t apply state if it’s the same as incoming state
            if (appliedTopMost.HasValue && appliedTopMost == isTop)
            {
                return;
            }

            if (Child == null)
            {
                return;
            }

            var hwndSource = (PresentationSource.FromVisual(Child)) as HwndSource;

            if (hwndSource == null)
            {
                return;
            }
            var hwnd = hwndSource.Handle;

            RECT rect;
            if (!UnsafeNativeMethods.GetWindowRect(hwnd, out rect))
            {
                return;
            }
            //Debug.WriteLine("setting z-order " + isTop);

            var left = rect.left;
            var top = rect.top;
            var width = rect.Width;
            var height = rect.Height;
            if (isTop)
            {
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_TOPMOST, left, top, width, height, Constants.TOPMOST_FLAGS);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_BOTTOM, left, top, width, height, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_TOP, left, top, width, height, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_NOTOPMOST, left, top, width, height, Constants.TOPMOST_FLAGS);
            }

            appliedTopMost = isTop;
        }
    }
}