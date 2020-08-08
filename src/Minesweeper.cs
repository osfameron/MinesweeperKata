using System;
using System.Linq;
using Sprache;

namespace Minesweeper
{
    public class Game
    {
        public const char EMPTY = '.';
        public const char MINE  = '*';

        private Grid<char> grid;
        public int Y { get => grid.Y; }
        public int X { get => grid.X; }

        public Game(Grid<char> g)
        {
            grid = g;
        }

        public Game(char[][] rows)
        {
            grid = new Grid<char>(rows);
        }

        public static Game FromPicture(string picture) =>
                new Game(
                    (from l in picture.Split("\n")
                    select l.ToCharArray())
                    .ToArray());

        public static Parser<(int,int)> sizeParser =>
                from ys in Parse.Digit.AtLeastOnce().Text().Token()
                from xs in Parse.Digit.AtLeastOnce().Text()
                let y = Int32.Parse(ys)
                let x = Int32.Parse(xs)
                from _ in Parse.LineEnd
                select (y, x);
        
        public static Parser<Game> gridParser =>
                from size in sizeParser
                from rows in GridParser(size)
                select new Game(rows);
        
        public static Parser<char[]> RowParser(int x) =>
                from cs in Parse.Chars(new [] {EMPTY, MINE})
                           .Repeat(x)
                let chars = cs.ToArray()
                select chars;

        public static Parser<char[][]> GridParser((int y, int x) size) =>
                from rs in RowParser(size.x)
                           .DelimitedBy(Parse.LineEnd)
                let rows = rs.ToArray()
                where rows.Length == size.y
                select rows;

        public string Output() =>
                ReinsertMines(ProximityGrid)
                .ToString();

        public override string ToString() =>
                grid.Comprehend(r => new string(r),
                                rs => rs.Aggregate((a,b) => $"{a}\n{b}"));

        private static int Add(int a, char g) =>
                g == MINE ? a + 1 : a;

        private static char Reinsert(int a, char g) =>
                g == MINE ? MINE : a.ToString()[0];

        private Game ReinsertMines(Grid<int> pg) =>
                new Game(pg.Zip(grid, Reinsert));

        private Grid<char>[] ProximityLayers()
        {
            Grid<char>[] center = { grid.Left(EMPTY), grid, grid.Right(EMPTY) };
            Grid<char>[] up   = center.Select(g => g.Up(EMPTY)).ToArray();
            Grid<char>[] down = center.Select(g => g.Down(EMPTY)).ToArray();
            return up.Concat(center).Concat(down).ToArray();
        }

        private Grid<int> Zeros =>
                grid.Map(_ => 0);

        private Grid<int> ProximityGrid =>
                ProximityLayers()
                    .Aggregate(
                        Zeros,
                        (acc, grid) => acc.Zip(grid, Add));
    }

    public class Grid<T>
    {
        public T[][] Data { get; }
        public int Y { get; }
        public int X { get; }

        public Grid (T[][] rows) {
            Data = rows;
            Y = rows.Length;

            if (Y == 0) {
                throw new ArgumentException("Can't initialize an empty grid");
            }
            X = rows[0].Length;
            if (X == 0) {
                throw new ArgumentException("Can't initialize a grid with empty rows");
            }
            if (rows.Any(r => r.Length != X)) {
                throw new ArgumentException("Row size mismatch");
            }
        }
        
        public Grid<TOut> Zip<TOut, TIn> (Grid<TIn> g2, Func<T, TIn, TOut> f) =>
                new Grid<TOut>(
                    Data.Zip(g2.Data, (r1, r2) => r1.Zip(r2, f).ToArray())
                    .ToArray());

        public Grid<TOut> Map<TOut> (Func<T, TOut> f) =>
                new Grid<TOut> (
                    Data.Select(row => row.Select(f).ToArray())
                        .ToArray());

        public TOut Comprehend<TOut, TRow> (Func<T[], TRow> aggregateRow, Func<TRow[], TOut> aggregateRows) =>
                aggregateRows(
                    Data
                        .Select(row => aggregateRow(row))
                        .ToArray());

        // this is basically a delegate for Grid<T>, is there a builtin way of doing this in C#?
        private Grid<T> Reform(T[][] rows) => new Grid<T> (rows);

        private T[] Row(T r) =>
                Enumerable.Repeat(r, X).ToArray();

        public Grid<T> Left(T zero) =>
                Comprehend(
                    r => r.Skip(1).Append(zero).ToArray(),
                    Reform);

        public Grid<T> Right(T zero) =>
                Comprehend(
                    r => r.SkipLast(1).Prepend(zero).ToArray(),
                    Reform);

        public Grid<T> Up(T zero) =>
                Reform(
                    Data.Skip(1)
                    .Append(Row(zero))
                    .ToArray());
 
        public Grid<T> Down(T zero) =>
                Reform(
                    Data.SkipLast(1)
                    .Prepend(Row(zero))
                    .ToArray());
    }
}
