using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;
using Standard;
using RECT = MahApps.Metro.Native.RECT;
using WM = MahApps.Metro.Models.Win32.WM;

namespace MahApps.Metro.Behaviours
{
    public class GlowWindowBehavior : Behavior<Window>
    {
        private static readonly TimeSpan GlowTimerDelay = TimeSpan.FromMilliseconds(200); //200 ms delay, the same as VS2013
        private GlowWindow left, right, top, bottom;
        private DispatcherTimer makeGlowVisibleTimer;
        private IntPtr handle;

        private bool IsGlowDisabled
        {
            get
            {
                var metroWindow = AssociatedObject as MetroWindow;
                return metroWindow != null && (metroWindow.UseNoneWindowStyle || metroWindow.GlowBrush == null);
            }
        }

        private bool IsWindowTransitionsEnabled
        {
            get
            {
                var metroWindow = AssociatedObject as MetroWindow;
                return metroWindow != null && metroWindow.WindowTransitionsEnabled;
            }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SourceInitialized += (o, args) => {
                // No glow effect if UseNoneWindowStyle is true or GlowBrush not set.
                if (IsGlowDisabled)
                {
                    return;
                }
                handle = new WindowInteropHelper(AssociatedObject).Handle;
                var hwndSource = HwndSource.FromHwnd(handle);
                if (hwndSource != null)
                {
                    hwndSource.AddHook(AssociatedObjectWindowProc);
                }
            };
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            AssociatedObject.Unloaded += AssociatedObjectUnloaded;
        }

        void AssociatedObjectStateChanged(object sender, EventArgs e)
        {
            if (makeGlowVisibleTimer != null)
            {
                makeGlowVisibleTimer.Stop();
            }
            if(AssociatedObject.WindowState != WindowState.Minimized)
            {
                var metroWindow = AssociatedObject as MetroWindow;
                var ignoreTaskBar = metroWindow != null && metroWindow.IgnoreTaskbarOnMaximize;
                if (makeGlowVisibleTimer != null && SystemParameters.MinimizeAnimation && !ignoreTaskBar)
                {
                    makeGlowVisibleTimer.Start();
                }
                else
                {
                    RestoreGlow();
                }
            }
            else
            {
                HideGlow();
            }
        }

        void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            if(makeGlowVisibleTimer != null)
            {
                makeGlowVisibleTimer.Stop();
                makeGlowVisibleTimer.Tick -= makeGlowVisibleTimer_Tick;
                makeGlowVisibleTimer = null;
            }
        }

        private void makeGlowVisibleTimer_Tick(object sender, EventArgs e)
        {
            if(makeGlowVisibleTimer != null)
            {
                makeGlowVisibleTimer.Stop();
            }
            RestoreGlow();
        }

        private void RestoreGlow()
        {
            if (left != null) left.IsGlowing = true;
            if (top != null) top.IsGlowing = true;
            if (right != null) right.IsGlowing = true;
            if (bottom != null) bottom.IsGlowing = true;
            Update();
        }

        private void HideGlow()
        {
            if (left != null) left.IsGlowing = false;
            if (top != null) top.IsGlowing = false;
            if (right != null) right.IsGlowing = false;
            if (bottom != null) bottom.IsGlowing = false;
            Update();
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // No glow effect if UseNoneWindowStyle is true or GlowBrush not set.
            if (IsGlowDisabled)
            {
                return;
            }

            AssociatedObject.StateChanged -= AssociatedObjectStateChanged;
            AssociatedObject.StateChanged += AssociatedObjectStateChanged;

            if (makeGlowVisibleTimer == null)
            {
                makeGlowVisibleTimer = new DispatcherTimer { Interval = GlowTimerDelay };
                makeGlowVisibleTimer.Tick += makeGlowVisibleTimer_Tick;
            }

            left = new GlowWindow(AssociatedObject, GlowDirection.Left);
            right = new GlowWindow(AssociatedObject, GlowDirection.Right);
            top = new GlowWindow(AssociatedObject, GlowDirection.Top);
            bottom = new GlowWindow(AssociatedObject, GlowDirection.Bottom);

            Show();
            Update();

            if (!IsWindowTransitionsEnabled)
            {
                // no storyboard so set opacity to 1
                AssociatedObject.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => SetOpacityTo(1)));
            }
            else
            {
                // start the opacity storyboard 0->1
                StartOpacityStoryboard();
                // hide the glows if window get invisible state
                AssociatedObject.IsVisibleChanged += AssociatedObjectIsVisibleChanged;
                // closing always handled
                AssociatedObject.Closing += (o, args) =>
                {
                    if (!args.Cancel)
                    {
                        AssociatedObject.IsVisibleChanged -= AssociatedObjectIsVisibleChanged;
                    }
                };
            }
        }

        private WINDOWPOS _previousWP;

        private IntPtr AssociatedObjectWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WM)msg)
            {
                case WM.WINDOWPOSCHANGED:
                case WM.WINDOWPOSCHANGING:
                    Assert.IsNotDefault(lParam);
                    var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    if (!wp.Equals(_previousWP))
                    {
                        UpdateCore();
                    }
                    _previousWP = wp;
                    break;
                case WM.SIZE:
                case WM.SIZING:
                    UpdateCore();
                    break;
            }
            return IntPtr.Zero;
        }

        private void AssociatedObjectIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!AssociatedObject.IsVisible)
            {
                // the associated owner got invisible so set opacity to 0 to start the storyboard by 0 for the next visible state
                SetOpacityTo(0);
            }
            else
            {
                StartOpacityStoryboard();
            }
        }

        /// <summary>
        /// Updates all glow windows (visible, hidden, collapsed)
        /// </summary>
        private void Update()
        {
            if (left != null) left.Update();
            if (right != null) right.Update();
            if (top != null) top.Update();
            if (bottom != null) bottom.Update();
        }

        private void UpdateCore()
        {
            RECT rect;
            if (handle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(handle, out rect))
            {
                if (left != null) left.UpdateCore(rect);
                if (right != null) right.UpdateCore(rect);
                if (top != null) top.UpdateCore(rect);
                if (bottom != null) bottom.UpdateCore(rect);
            }
        }

        /// <summary>
        /// Sets the opacity to all glow windows
        /// </summary>
        private void SetOpacityTo(double newOpacity)
        {
            if (left != null) left.Opacity = newOpacity;
            if (right != null) right.Opacity = newOpacity;
            if (top != null) top.Opacity = newOpacity;
            if (bottom != null) bottom.Opacity = newOpacity;
        }

        /// <summary>
        /// Starts the opacity storyboard 0 -> 1
        /// </summary>
        private void StartOpacityStoryboard()
        {
            if (left != null && left.OpacityStoryboard != null) left.BeginStoryboard(left.OpacityStoryboard);
            if (right != null && right.OpacityStoryboard != null) right.BeginStoryboard(right.OpacityStoryboard);
            if (top != null && top.OpacityStoryboard != null) top.BeginStoryboard(top.OpacityStoryboard);
            if (bottom != null && bottom.OpacityStoryboard != null) bottom.BeginStoryboard(bottom.OpacityStoryboard);
        }

        /// <summary>
        /// Shows all glow windows
        /// </summary>
        private void Show()
        {
            if (left != null) left.Show();
            if (right != null) right.Show();
            if (top != null) top.Show();
            if (bottom != null) bottom.Show();
        }
    }
}
