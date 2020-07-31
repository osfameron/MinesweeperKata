using NUnit.Framework;
using Minesweeper;
using System.Linq;
using System

;

namespace MinesweeperTests
{
    public class Tests
    {
        private Grid grid;
        private const string pic1 = @"..*.
....
.*..
....";

        private const string pic1left = @".*..
....
*...
....";

        [Test]
        public void TestInvalidArguments()
        {
            Assert.Throws<ArgumentException>(() => Grid.FromPicture(""));
            Assert.Throws<ArgumentException>(() => Grid.FromPicture("short\nverylongline"));
        }

        [Test]
        public void TestBasic()
        {
            var grid = Grid.FromPicture(pic1);
            Assert.AreEqual(4, grid.y);
            Assert.AreEqual(4, grid.x);
            Assert.AreEqual(pic1, grid.ToString());
        }

        [Test]
        public void TestLeftRight()
        {
            var grid = Grid.FromPicture(pic1);

            char[][] left = grid.Left();
            var gLeft = new Grid(left);
            Assert.AreEqual(pic1left, gLeft.ToString());

            char[][] right = gLeft.Right();
            var gRight = new Grid(right);
            Assert.AreEqual(pic1, gRight.ToString());
        }
    }
}
