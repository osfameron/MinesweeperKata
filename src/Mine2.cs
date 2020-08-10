using System;
using System.Collections.Generic;

namespace Mine2
{
    public class Rose
    {
        /// <summary> Compass Rose of all 8 directions, in Clockwise order </summary>
        public enum Direction { N, NE, E, SE, S, SW, W, NW };
        private const int DIR = 8;

        public static void AssertCardinal(Direction r)
        {
            if ((int) r % 2 != 0)
            {
                throw new ArgumentException($"Direction {r} is not cardinal");
            }
        }

        /// <summary> Actual mathematical modulus rather than remainder </summary>
        private static int Mod (int x, int m) => (x % m + m) % m;

        public static Direction Rotate(Direction r, int c) =>
            (Direction) Mod((int) r + c, DIR);

        public static Direction Opposite(Direction r) =>
            Rotate(r, 4);

        public static Direction Mid(Direction dir, Direction perp) =>
            (Direction) (((int) dir + (int) perp) / 2);
    }

    public class Cell
    {
        public Dictionary<Rose.Direction, Cell> Neighbours { get; }

        public Cell()
        {
            Neighbours = new Dictionary<Rose.Direction, Cell> {};
        }

        public Cell? this[Rose.Direction dir] => Neighbours[dir];

        public void Connect(Rose.Direction dir, Cell other)
        {
            Neighbours.Add(dir, other);
            other.Neighbours.Add(Rose.Opposite(dir), this);
        }

        public Cell Grow(Rose.Direction dir)
        {
            Rose.AssertCardinal(dir);

            Cell other = Grow_(dir);
            Grow(dir, Rose.Rotate(dir, 2));
            Grow(dir, Rose.Rotate(dir, -2));

            return other;
        }

        private Cell Grow_(Rose.Direction dir)
        {
            Cell other = new Cell();
            Connect(dir, other);
            return other;
        }

        private void Grow(Rose.Direction dir, Rose.Direction perp)
        {
            Cell? p = Neighbours.GetValueOrDefault(perp, null);
            if (p is null) return;

            Cell q = p.Grow_(dir);
            Cell r = Neighbours[dir];

            Connect(Rose.Mid(dir, perp), q);
            p.Connect(Rose.Mid(dir, Rose.Opposite(perp)), r);
            r.Connect(perp, q);

            p.Grow(dir, perp);
        }
    }
}
