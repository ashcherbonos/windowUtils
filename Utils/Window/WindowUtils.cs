using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Utils.Window
{
    /// <summary>
    /// Window utils
    /// Expose some hidden Windows OS functionality to work with windows
    /// </summary>
    public class WindowUtils
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private const int GwlExstyle = -20;
        private const int WsExNoactivate = 0x08000000;
        private const int WsExToolWindow = 0x00000080;

        [DllImport("user32.dll")] private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")] private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true)] private static extern int GetWindowRect(IntPtr hwnd, out Rect rc);

        /// Set window not focusable
        public static void SetNonFocusable(Window window)
        {
            var helper = new WindowInteropHelper(window);
            SetWindowLong(helper.Handle, GwlExstyle,
                GetWindowLong(helper.Handle, GwlExstyle) | WsExNoactivate);
        }

        /// Hide in AltTab menu
        public static void HideAltTab(Window window)
        {
            var windowInterop = new WindowInteropHelper(window);
            var exStyle = GetWindowLong(windowInterop.Handle, GwlExstyle);
            exStyle |= WsExToolWindow;
            SetWindowLong(windowInterop.Handle, GwlExstyle, exStyle);
        }

        /// Set window not focusable
        public static void SetFocusable(Window window)
        {
            var helper = new WindowInteropHelper(window);
            SetWindowLong(helper.Handle, GwlExstyle,
                GetWindowLong(helper.Handle, GwlExstyle) & ~WsExNoactivate);
            HideAltTab(window);
        }

        /// Check for games in full screen mode
        public static bool IsForegroundWindowInFullscreenMode()
        {
            var hWnd = GetForegroundWindow();
            if (!hWnd.Equals(IntPtr.Zero))
            {
                var desktopHandle = GetDesktopWindow();
                var shellHandle = GetShellWindow();
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle)))
                {
                    Rect appBounds;
                    var succeeds = GetWindowRect(hWnd, out appBounds);
                    if (succeeds == 0) return false;
                    var screenBounds = Screen.FromHandle(hWnd).Bounds;
                    return (appBounds.Bottom - appBounds.Top) == screenBounds.Height 
                        && (appBounds.Right - appBounds.Left) == screenBounds.Width;
                }
            }
            return false;
        }

    }
}
