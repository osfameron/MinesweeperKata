using System;
using System.Linq;
using System.Collections.Generic;

using static Mine2.Rose;
using static Mine2.Rose.Direction;

namespace Mine2
{
    /// <summary> Represents a Minesweeper piece on the grid </summary>
    public enum Piece { Empty, Mine };

    /// <summary> Formatters </summary>
    public static class Formatters
    {
        // should be a Formatter class rather than static lambdas?

        private static Func<Cell<Piece>, bool> IsMine = p => p.Value == Piece.Mine;
        /// <summary> Initial view with . or * </summary>
        public static Func<Cell<Piece>, string> PieceOut = p => IsMine(p) ? "*" : ".";

        /// <summary> Output view with adjacent mine count </summary>
        public static Func<Cell<Piece>, string> CountOut =
            p => IsMine(p)
                ? "*"
                : p.Neighbours
                   .Values
                   .Where(IsMine)
                   .Count()
                   .ToString();
    }
    

    /// <summary> Compass Rose </summary>
    public class Rose
    {
        /// <summary> Compass Rose of all 8 directions, in Clockwise order </summary>
        public enum Direction { N, NE, E, SE, S, SW, W, NW };
        private const int DIR = 8;

        /// <summary> Assert that a direction is N,S,E,W and not one of the diagonal directions </summary>
        public static void AssertCardinal(Direction r)
        {
            if ((int) r % 2 != 0)
            {
                throw new ArgumentException($"Direction {r} is not cardinal");
            }
        }

        /// <summary> Actual mathematical modulus rather than remainder </summary>
        private static int Mod (int x, int m) => (x % m + m) % m;

        /// <summary> Rotate direction by a number of 1/8-turns </summary>
        public static Direction Rotate(Direction r, int c) =>
            (Direction) Mod((int) r + c, DIR);

        /// <summary> The direction opposite </summary>
        public static Direction Opposite(Direction r) =>
            Rotate(r, DIR / 2);

        /// <summary> The direction midway between two </summary>
        public static Direction Mid(Direction dir, Direction perp) =>
            (Direction) (((int) dir + (int) perp) / 2);
    }

    /// <summary> Class representing a lattice grid of cells </summary>
    public class Cell<T>
    {

        /// <summary> Value of the cell </summary>
        public T Value { get; set; }

        /// <summary> Neighbours of the cell, indexed by the directions of the compass rose </summary>
        public Dictionary<Direction, Cell<T>> Neighbours { get; }

        /// <summary> Constructor </summary>
        public Cell(T value)
        {
            Value = value;
            Neighbours = new Dictionary<Direction, Cell<T>> {};
        }

        /// <summary> Indexer to traverse cell by direction <eg> e.g. <c>cell[N]</c></eg> </summary>
        public Cell<T> this[Direction dir] =>
            Neighbours[dir];
 
        /// <summary> Indexer to get/set value at cell traversed to by [y,x] from the current location at NW.</summary>
        public T this[int y, int x] {
            get => Traverse(E)
                   .ElementAt(x)
                   .Traverse(S)
                   .ElementAt(y)
                   .Value;
            set => Traverse(E)
                  .ElementAt(x)
                  .Traverse(S)
                  .ElementAt(y)
                  .Value = value;
        }

        /// <summary> Connect two cells via a direction (automatically creating the reciprocal) </summary>
        public void Connect(Direction dir, Cell<T> other)
        {
            Neighbours.Add(dir, other);
            other.Neighbours.Add(Opposite(dir), this);
        }

        /// <summary> Static factory to create a lattice of value {T}. The cell returned is at NW corner.</summary>
        public static Cell<T> Lattice(int y, int x, T value)
        {
            var c = new Cell<T>(value);
            foreach (var _ in Enumerable.Range(1, y - 1)) {
                c = c.Grow(N);
            }
            foreach (var _ in Enumerable.Range(1, x - 1)) {
                c = c.Grow(W);
            }
            return c;
        }

        /// <summary> Travel in a given direction </summary>
        public IEnumerable<Cell<T>> Traverse(Direction dir)
        {
            var c = this;
            while (c != null)
            {
                yield return c;
                c = c.Neighbours.GetValueOrDefault(dir, null);
            }
        }

        /// <summary> ToString method delegates to the {T} value</summary>
        public override string ToString() => Value.ToString();

        /// <summary> Show the whole grid (assuming current cell at NW) </summary>
        public string ToGridString() => ToGridString(c => c.ToString());

        /// <summary> Show the whole grid (assuming current cell at NW), passing in a custom formatter function </summary>
        public string ToGridString(Func<Cell<T>, string> f) =>
            String.Join('\n',
                from row in Traverse(S)
                select String.Concat(
                    (from cell in row.Traverse(E)
                    select f(cell))));

        /// <summary> Expand the lattice in a cardinal direction. </summary>
        public Cell<T> Grow(Direction dir)
        {
            AssertCardinal(dir);

            var other = Grow_(dir);
            Grow_(dir, Rotate(dir, 2));
            Grow_(dir, Rotate(dir, -2));

            return other;
        }

        private Cell<T> Grow_(Direction dir)
        {
            var other = new Cell<T>(Value);
            Connect(dir, other);
            return other;
        }

        private void Grow_(Direction dir, Direction perp)
        {
            Cell<T> p = Neighbours.GetValueOrDefault(perp, null);
            if (p is null) return;

            Cell<T> q = p.Grow_(dir);
            Cell<T> r = Neighbours[dir];

            Connect(Mid(dir, perp), q);
            p.Connect(Mid(dir, Opposite(perp)), r);
            r.Connect(perp, q);

            p.Grow_(dir, perp);
        }
    }
}
