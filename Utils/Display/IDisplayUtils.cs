using System.Drawing;

namespace Application.Displays
{
    /// Screen edges
    public enum Edge
    {
        /// At top edge
        Top,
        /// At Bottom edge
        Bottom,
        /// At Left edge
        Left,
        /// At Right edge
        Right,
        /// Not at the edge
        NotAtEdge
    }

    /// Display utils interface
    public interface IDisplayUtils
    {
        /// Returns the Window current position relative to the edge of the screen
        Edge GetCurrentEdge();

        /// Returns the primary screen bounds
        Rectangle GetMainScreenBounds();

        /// Returns the screen where the Window is currently located
        Rectangle GetCurrentScreenBounds();

        /// Return presentation-to-device scale coefficients
        (double, double) GetDpi();

        /// Returns id of the screen where the Window is currently located
        int GetCurrentScreenId();

        /// Get Window Position Relative to current display
        public (int, double, double) GetRelativeWindowPosition();

        /// Set Window Position from Relative
        public void SetWindowPositionFromRelative(int screenIndex, double relativeX, double relativeY);

        /// Returns current screen name
        public string GetDeviceName();

        /// Checks given display exist
        public bool DisplayExist(string deviceName);

        /// Recenter Window Based on the screen edge
        public void RecenterWindowBasedEdge(Edge edge);

        /// MainWindow Left position
        public double MainWindowLeft { get; }
        
        /// MainWindow Top position
        public double MainWindowTop { get; }
    }
}
