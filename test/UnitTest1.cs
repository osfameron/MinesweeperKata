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
        private const string pic1 = @"*...
....
.*..
....";

        [Test]
        public void TestInvalidArguments()
        {
            Assert.Throws<ArgumentException>(() => new Grid(""));
            Assert.Throws<ArgumentException>(() => new Grid("short\nverylongline"));
        }

        [Test]
        public void TestBasic()
        {
            var grid = new Grid(pic1);
            Assert.IsNotNull(grid);
            Assert.AreEqual(pic1, grid.ToString());
        }
    }
}
