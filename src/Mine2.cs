using System;
using System.Linq;
using System.Collections.Generic;

using static Mine2.Rose;
using static Mine2.Rose.Direction;

namespace Mine2
{
    public enum Piece { Empty, Mine };

    public static class Extensions
    {
        public static Func<Cell<Piece>, string> PieceOut = p => (p.Value == Piece.Empty) ? "." : "*";
    }
    
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

    public class Cell<T>
    {

        public T Value { get; set; }
        public Dictionary<Direction, Cell<T>> Neighbours { get; }

        public Cell(T value)
        {
            Value = value;
            Neighbours = new Dictionary<Direction, Cell<T>> {};
        }

        public Cell<T>? this[Direction dir] => Neighbours[dir];

        public void Connect(Direction dir, Cell<T> other)
        {
            Neighbours.Add(dir, other);
            other.Neighbours.Add(Opposite(dir), this);
        }

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

        public IEnumerable<Cell<T>> Traverse(Direction dir)
        {
            var c = this;
            while (c != null)
            {
                yield return c;
                c = c.Neighbours.GetValueOrDefault(dir, null);
            }
        }

        public override string ToString() => Value.ToString();

        public string ToGridString() => ToGridString(c => c.ToString());
        public string ToGridString(Func<Cell<T>, string> f) =>
            String.Join('\n',
                from row in Traverse(S)
                select String.Concat(
                    (from cell in row.Traverse(E)
                    select f(cell))));

        public Cell<T> Grow(Direction dir)
        {
            AssertCardinal(dir);

            var other = Grow_(dir);
            Grow(dir, Rotate(dir, 2));
            Grow(dir, Rotate(dir, -2));

            return other;
        }

        private Cell<T> Grow_(Direction dir)
        {
            var other = new Cell<T>(Value);
            Connect(dir, other);
            return other;
        }

        private void Grow(Direction dir, Direction perp)
        {
            Cell<T> p = Neighbours.GetValueOrDefault(perp, null);
            if (p is null) return;

            Cell<T> q = p.Grow_(dir);
            Cell<T> r = Neighbours[dir];

            Connect(Mid(dir, perp), q);
            p.Connect(Mid(dir, Opposite(perp)), r);
            r.Connect(perp, q);

            p.Grow(dir, perp);
        }
    }
}
