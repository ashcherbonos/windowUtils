using System.Drawing;
using System.Threading;
using System.Windows;

using Moq;

using NUnit.Framework;

using Point = System.Drawing.Point;
using Visual = System.Windows.Media.Visual;

namespace UtilsTest
{
    [Apartment(ApartmentState.STA)]
    public class DisplayUtilsTests
    {
        [Test]
        public void ControlTest_ShouldBe_True()
        {
            // arrange
            // act 
            // assert
            Assert.True(true);
        }

        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 0, 1000, 1000 }, new int[] { 0, 0 }, ExpectedResult = 0)]
        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 0, 1000, 1000 }, new int[] { 1000, 0 }, ExpectedResult = 0)]
        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 0, 1000, 1000 }, new int[] { 1001, 0 }, ExpectedResult = 1)]
        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { -1000, 0, 1000, 1000 }, new int[] { -1, 0 }, ExpectedResult = 1)]
        [TestCase(new int[] { 0, 0, 500, 500 }, new int[] { 500, 0, 1000, 1000 }, new int[] { 600, 0 }, ExpectedResult = 1)]
        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 0, 1000, 1000, 1000 }, new int[] { 500, 1500 }, ExpectedResult = 1)]
        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 1000, 1000, 1000 }, new int[] { 1500, 1500 }, ExpectedResult = 1)]
        public int Should_FindCurrentScreen_From2Screens(int[] border1, int[] border2, int[] cursor)
        {
            // arrange            
            var screen1 = new Rectangle(border1[0], border1[1], border1[2], border1[3]);
            var screen2 = new Rectangle(border2[0], border2[1], border2[2], border2[3]);
            var cursorPosition = new Point(cursor[0], cursor[1]);
            // act
            return DisplayManager.GetCurrentScreenIdImpl(new Rectangle[] { screen1, screen2 }, cursorPosition);
        }

        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 0, 1000, 1000 }, new int[] { 2000, 0, 1000, 1000 }, new int[] { 2500, 0 }, ExpectedResult = 2)]
        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 0, 1000, 1000, 1000 }, new int[] { 0, 2000, 1000, 1000 }, new int[] { 0, 2500 }, ExpectedResult = 2)]
        [TestCase(new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 1000, 1000, 1000 }, new int[] { 2000, 2000, 1000, 1000 }, new int[] { 2500, 2500 }, ExpectedResult = 2)]
        public int Should_FindCurrentScreen_From3Screens(int[] border1, int[] border2, int[] border3, int[] cursor)
        {
            // arrange            
            var screen1 = new Rectangle(border1[0], border1[1], border1[2], border1[3]);
            var screen2 = new Rectangle(border2[0], border2[1], border2[2], border2[3]);
            var screen3 = new Rectangle(border3[0], border3[1], border3[2], border3[3]);
            var cursorPosition = new Point(cursor[0], cursor[1]);
            // act
            return DisplayManager.GetCurrentScreenIdImpl(new Rectangle[] { screen1, screen2, screen3 }, cursorPosition);
        }

        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 0, 0 }, ExpectedResult = Edge.Left)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 100, 0 }, ExpectedResult = Edge.Left)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 0, 1000 }, ExpectedResult = Edge.Left)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 999, 0 }, ExpectedResult = Edge.Right)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 999, 0 }, ExpectedResult = Edge.Right)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 999, 999 }, ExpectedResult = Edge.Right)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 500, 500 }, ExpectedResult = Edge.NotAtEdge)]
        public Edge Should_FindCurrentEdge_From1Screens(double edgeSize, int[] border1, int[] cursor)
        {
            // arrange            
            var screen1 = new Rectangle(border1[0], border1[1], border1[2], border1[3]);
            var cursorPosition = new Point(cursor[0], cursor[1]);
            // act
            return DisplayManager.GetCurrentEdgeImpl(new Rectangle[] { screen1 }, cursorPosition, edgeSize, 1);
        }

        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 0, 1000, 1000 }, new int[] { 1100, 0 }, ExpectedResult = Edge.Left)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { 1000, 0, 1000, 1000 }, new int[] { 1900, 0 }, ExpectedResult = Edge.Right)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { -1000, 0, 1000, 1000 }, new int[] { -100, 0 }, ExpectedResult = Edge.Right)]
        [TestCase(200, new int[] { 0, 0, 1000, 1000 }, new int[] { -1000, 0, 1000, 1000 }, new int[] { -900, 0 }, ExpectedResult = Edge.Left)]
        public Edge Should_FindCurrentEdge_From2Screens(double edgeSize, int[] border1, int[] border2, int[] cursor)
        {
            // arrange            
            var screen1 = new Rectangle(border1[0], border1[1], border1[2], border1[3]);
            var screen2 = new Rectangle(border2[0], border2[1], border2[2], border2[3]);
            var cursorPosition = new Point(cursor[0], cursor[1]);
            // act 
            return DisplayManager.GetCurrentEdgeImpl(new Rectangle[] { screen1, screen2 }, cursorPosition, edgeSize, 1);
        }


        [Test]
        public void Should_Recenter_Window_Based_Edge()
        {
            // arrange  
            var visual = new Mock<Visual>();
            var window = new Mock<Window>();
            var displayManager = new DisplayManager(0, visual.Object);
            var screenBounds = displayManager.GetMainScreenBounds();
            // act 
            displayManager.RecenterWindowBasedEdge(window.Object, Edge.NotAtEdge);

            // assert
            Assert.AreEqual(window.Object.Top, (screenBounds.Height / 2) - 72);
            Assert.AreEqual(window.Object.Left, (screenBounds.Width / 2) - 240);
        }
    }


}

