using NUnit.Framework;
using Minesweeper;
using System.Linq;
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
            Assert.Throws<ArgumentException>(() => Grid.FromPicture(""));
            Assert.Throws<ArgumentException>(() => Grid.FromPicture("short\nverylongline"));
            Assert.Throws<ArgumentException>(EmptyGrid);
            Assert.Throws<ArgumentException>(EmptyRow);
        }

        public void EmptyGrid()
        {
            char[][] x = {};
            var g = new Grid(x);
        }
        public void EmptyRow()
        {
            char[] row = {};
            char[][] x = {row};
            var g = new Grid(x);
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
        public void TestOutput()
        {
            var g = Grid.FromPicture("*.*\n.*.\n*.*");
            var expected = "*3*\n3*3\n*3*";
            Assert.AreEqual(expected, g.Output());
        }

        [Test]
        public void TestParseSize()
        {
            Parser<(int,int)> p = Grid.sizeParser;
            (int,int) size = p.Parse("4 4");
            Assert.AreEqual((4,4), size);
        }
    }
}
