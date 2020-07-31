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

        [Test]
        public void TestZeros()
        {
            var g = Grid.FromPicture("..\n..");
            var z = g.Zeros();
            Assert.AreEqual(0, z[0][0]);
            Assert.AreEqual(0, z[0][1]);
            Assert.AreEqual(0, z[1][0]);
            Assert.AreEqual(0, z[1][1]);
        }

        [Test]
        public void TestAdd()
        {
            var g = Grid.FromPicture(".*\n*.");
            var l = g.Left();
            var z = g.Zeros();
            var n = Grid.Add(z, l);
            Assert.AreEqual(1, n[0][0]);
            Assert.AreEqual(0, n[0][1]);
            Assert.AreEqual(0, n[1][0]);
            Assert.AreEqual(0, n[1][1]);
        }
    }
}
