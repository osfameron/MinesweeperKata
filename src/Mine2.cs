using System;
using System.Linq;
using System.Collections.Generic;

using static Mine2.Rose;
using static Mine2.Rose.Direction;

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

        public Dictionary<Direction, Cell> Neighbours { get; }

        public Cell()
        {
            Neighbours = new Dictionary<Direction, Cell> {};
        }

        public Cell? this[Direction dir] => Neighbours[dir];

        public void Connect(Direction dir, Cell other)
        {
            Neighbours.Add(dir, other);
            other.Neighbours.Add(Opposite(dir), this);
        }

        public static Cell Lattice(int y, int x)
        {
            Cell c = new Cell();
            foreach (var _ in Enumerable.Range(1, y - 1)) {
                c = c.Grow(N);
            }
            foreach (var _ in Enumerable.Range(1, x - 1)) {
                c = c.Grow(W);
            }
            return c;
        }

        public IEnumerable<Cell> Traverse(Direction dir)
        {
            Cell c = this;
            while (c != null)
            {
                yield return c;
                c = c.Neighbours.GetValueOrDefault(dir, null);
            }
        }

        public Cell Grow(Direction dir)
        {
            AssertCardinal(dir);

            Cell other = Grow_(dir);
            Grow(dir, Rotate(dir, 2));
            Grow(dir, Rotate(dir, -2));

            return other;
        }

        private Cell Grow_(Direction dir)
        {
            Cell other = new Cell();
            Connect(dir, other);
            return other;
        }

        private void Grow(Direction dir, Direction perp)
        {
            Cell? p = Neighbours.GetValueOrDefault(perp, null);
            if (p is null) return;

            Cell q = p.Grow_(dir);
            Cell r = Neighbours[dir];

            Connect(Mid(dir, perp), q);
            p.Connect(Mid(dir, Opposite(perp)), r);
            r.Connect(perp, q);

            p.Grow(dir, perp);
        }
    }
}
