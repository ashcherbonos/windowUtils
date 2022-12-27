using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Utils.Display
{
    /// Display utils class
    public class DisplayUtils : IDisplayUtils
    {
        private readonly double _edgeZoneSize;
        private readonly Visual _visual;

        /// Constructor
        public DisplayUtils(double edgeZoneSize, Visual visual)
        {
            _edgeZoneSize = edgeZoneSize;
            _visual = visual;
        }

        /// <inheritdoc/>
        public Edge GetCurrentEdge(Window window)
        {
            var (dpiX, _) = GetDpi();
            return GetCurrentEdgeImpl(Screen.AllScreens.Select(s => s.Bounds).ToArray(), GetWindowCenter(window), _edgeZoneSize, dpiX);
        }

        /// Testable implementation of GetCurrentEdge method
        public static Edge GetCurrentEdgeImpl(Rectangle[] allScreensBounds, System.Drawing.Point position, double edgeZoneSize, double dpiX)
        {
            var index = GetCurrentScreenIdImpl(allScreensBounds, position);
            if (index < 0) return Edge.NotAtEdge;
            var bound = allScreensBounds[index];
            if (position.X * dpiX <= bound.X + edgeZoneSize)
            {
                return Edge.Left;
            }
            else if (position.X * dpiX >= bound.X + bound.Width - edgeZoneSize)
            {
                return Edge.Right;
            }
            else
            {
                return Edge.NotAtEdge;
            }
        }

        /// Get Window Position Relative to current display
        public (int, double, double) GetRelativeWindowPosition(Window window)
        {
            var allScreensBounds = Screen.AllScreens.Select(s => s.Bounds).ToArray();
            var WindowCenter = GetWindowCenter(window);

            var screenIndex = GetCurrentScreenIdImpl(allScreensBounds, WindowCenter);

            var bound = allScreensBounds[screenIndex];
            var (dpiX, dpiY) = GetDpi();

            var x = (WindowCenter.X * dpiX - bound.X) / (bound.Width);
            var y = (WindowCenter.Y * dpiY - bound.Y) / (bound.Height);
            return (screenIndex, x, y);
        }

        /// Calculate Window Position from Relative
        public void SetWindowPositionFromRelative(Window window, int screenIndex, double relativeX, double relativeY)
        {
            var allScreensBounds = Screen.AllScreens.Select(s => s.Bounds).ToArray();
            if (screenIndex >= allScreensBounds.Length)
            {
                screenIndex = 0;
                relativeX = relativeY = 0.5;
            }

            var screen = allScreensBounds[screenIndex];

            // Set window to proper screen first, to calculate this screen dpi:
            window.Left = screen.X + 1;
            window.Top = screen.Y + 1;
            var (dpiX, dpiY) = GetDpi();

            var centerX = screen.X + screen.Width * relativeX;
            var centerY = screen.Y + screen.Height * relativeY;

            var left = centerX / dpiX - Constants.WindowCenterX;
            var top = centerY / dpiY - Constants.WindowCenterY;

            // set window position:
            window.Left = left;
            window.Top = top;
        }

        /// <inheritdoc cref="IDisplayUtils" />
        public string GetDeviceName(Window window)
        {
            var screen = Screen.FromHandle(
                new System.Windows.Interop.WindowInteropHelper(window).Handle);

            return screen.DeviceName;
        }

        /// <inheritdoc cref="IDisplayUtils" />
        public bool DisplayExist(string deviceName)
        {
            var exist = Screen.AllScreens.Any(s => s.DeviceName == deviceName);

            return exist;
        }

        /// <inheritdoc cref="IDisplayUtils" />
        public void RecenterWindowBasedEdge(Window window, Edge edge)
        {
            if (edge == Edge.NotAtEdge)
                RecenterWindowCenter(window);

            if (edge == Edge.Right)
                RecenterWindowRight(window);

            if (edge == Edge.Left)
                RecenterWindowLeft(window);
        }

        /// <inheritdoc/>
        public Rectangle GetMainScreenBounds()
        {
            var allScreensBounds = Screen.AllScreens.Select(s => s.Bounds).ToArray();
            var (dpiX, dpiY) = GetDpi();
            var r = allScreensBounds[0];
            return new Rectangle(
                (int)(r.X / dpiX),
                (int)(r.Y / dpiY),
                (int)(r.Width / dpiX),
                (int)(r.Height / dpiY));
        }

        /// <inheritdoc/>
        public Rectangle GetCurrentScreenBounds(Window window)
        {
            var allScreensBounds = Screen.AllScreens.Select(s => s.Bounds).ToArray();
            var (dpiX, dpiY) = GetDpi();
            var r = allScreensBounds[GetCurrentScreenIdImpl(allScreensBounds, GetWindowCenter(window))];
            return new Rectangle(
                (int)(r.X / dpiX),
                (int)(r.Y / dpiY),
                (int)(r.Width / dpiX),
                (int)(r.Height / dpiY));
        }

        /// <inheritdoc/>
        public int GetCurrentScreenId(Window window)
        {
            var allScreensBounds = Screen.AllScreens.Select(s => s.Bounds).ToArray();
            var position = GetWindowCenter(window);
            return GetCurrentScreenIdImpl(allScreensBounds, position);
        }

        /// Testable implementation of GetCurrentScreenId method
        public static int GetCurrentScreenIdImpl(Rectangle[] allScreensBounds, System.Drawing.Point position)
        {
            for (var i = 0; i < allScreensBounds.Length; i++)
            {
                var bound = allScreensBounds[i];
                if (position.X >= bound.X &&
                    position.X <= bound.X + bound.Width &&
                    position.Y >= bound.Y &&
                    position.Y <= bound.Y + bound.Height)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <inheritdoc/>
        public (double, double) GetDpi()
        {
            var source = PresentationSource.FromVisual(_visual);

            if (source is null)
            {
                return (1, 1);
            }

            var dpiX = source.CompositionTarget.TransformToDevice.M11;
            var dpiY = source.CompositionTarget.TransformToDevice.M22;

            return (dpiX, dpiY);
        }

        private static System.Drawing.Point GetWindowCenter(Window window)
        {
            return new System.Drawing.Point((int)(window.Left + Constants.WindowCenterX), (int)(window.Top + Constants.WindowCenterY));
        }

        private void RecenterWindowCenter(Window window)
        {
            var bounds = GetMainScreenBounds();
            window.Top = bounds.Y + bounds.Height / 2 - Constants.WindowCenterY;
            window.Left = bounds.X + bounds.Width / 2 - Constants.WindowCenterX;
        }

        private void RecenterWindowRight(Window window)
        {
            var bounds = GetMainScreenBounds();
            window.Top = bounds.Y + bounds.Height / 2 - Constants.WindowCenterY;
            window.Left = bounds.Width - (window.Width / 2 + Constants.WindowWidth / 2);
        }

        private void RecenterWindowLeft(Window window)
        {
            var bounds = GetMainScreenBounds();
            window.Top = bounds.Y + bounds.Height / 2 - Constants.WindowCenterY;
            window.Left = -(window.Width / 2) + Constants.WindowWidth / 2;
        }
    }
}
