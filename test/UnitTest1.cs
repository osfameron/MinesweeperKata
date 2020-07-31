using NUnit.Framework;
using Minesweeper;
using System.Linq;
using System

;

namespace MinesweeperTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            grid = new Grid(grid1);
        }
        private Grid grid;
        private const string grid1 = @"*...
....
.*..
....";

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            string[] expected = { "*...", "....", ".*..", "...." };
            Assert.AreEqual(expected, grid1.Split("\n"));

            char[] exp2 = { 'f', 'o', 'o' };
            Assert.AreEqual(exp2, "foo".ToCharArray());

            var result =
                (from line in "fo\nba".Split("\n")
                 select line.ToCharArray())
                .ToArray();
            var lenFirst = result[0].Length;
            Assert.True(result.All(s => s.Length == lenFirst));

            Console.WriteLine(result);

            Assert.AreEqual('f', result[0][0]);
            Assert.AreEqual('o', result[0][1]);
            Assert.AreEqual('b', result[1][0]);
            Assert.AreEqual('a', result[1][1]);

        }

    }
}
