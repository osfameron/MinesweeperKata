using System;
using System.Collections.Generic;

namespace Mine2
{
    public class Rose
    {
        /// <summary> Compass Rose of all 8 directions, in Clockwise order </summary>
        public enum Direction { N, NE, E, SE, S, SW, W, NW };
        private const int DIR = 8;

        /// <summary> Actual mathematical modulus rather than remainder </summary>
        private static int Mod (int x, int m) => (x % m + m) % m;

        public static Direction Rotate(Direction r, int c) =>
            (Direction) Mod((int) r + c, DIR);

        public static Direction Opposite(Direction r) =>
            Rotate(r, 4);
    }

    public class Cell
    {
        public Dictionary<Rose.Direction, Cell> Neighbours { get; }

        public Cell()
        {
            Neighbours = new Dictionary<Rose.Direction, Cell> {};
        }

        public void Connect(Rose.Direction dir, Cell other)
        {
            Neighbours[dir] = other;
            other.Neighbours[Rose.Opposite(dir)] = this;
        }
    }
}
