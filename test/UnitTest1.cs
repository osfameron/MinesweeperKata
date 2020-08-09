using NUnit.Framework;
using Minesweeper;
using System;
using Sprache;

namespace MinesweeperTests
{
    public class Tests
    {
        private const string pic1     = "..*.\n....\n.*..\n....";
        private const string pic1left = ".*..\n....\n*...\n....";

        [Test]
        public void TestInvalidArguments()
        {
            Assert.Throws<ArgumentException>(() => Game.FromPicture(""));
            Assert.Throws<ArgumentException>(() => Game.FromPicture("short\nverylongline"));
            Assert.Throws<ArgumentException>(EmptyGrid);
            Assert.Throws<ArgumentException>(EmptyRow);
        }

        public void EmptyGrid()
        {
            char[][] x = {};
            var g = new Game(x);
        }
        public void EmptyRow()
        {
            char[] row = {};
            char[][] x = {row};
            var g = new Game(x);
        }

        [Test]
        public void TestBasic()
        {
            var grid = Game.FromPicture(pic1);
            Assert.AreEqual(4, grid.Y);
            Assert.AreEqual(4, grid.X);
            Assert.AreEqual(pic1, grid.ToString());
        }

        [Test]
        public void TestOutput()
        {
            var g = Game.FromPicture("*.*\n.*.\n*.*");
            var expected = "*3*\n3*3\n*3*";
            Assert.AreEqual(expected, g.Output());
        }

        [Test]
        public void TestParseGrid()
        {
            Game g = Game.Parsed("2 2\n.*\n*.\n");
            Assert.AreEqual(2, g.Y);
            Assert.AreEqual(2, g.X);
        }

        [Test]
        public void TestIndexGrid()
        {
            Grid<int> g = new Grid<int>(new int[][] { new int[] {1, 2}, new int[] {3, 4} });
            Assert.AreEqual(1, g[0,0]);
            Assert.AreEqual(2, g[0,1]);
            Assert.AreEqual(3, g[1,0]);
            Assert.AreEqual(4, g[1,1]);
        }
    }
}
