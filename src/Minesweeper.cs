using System;
using System.Linq;
using Sprache;

namespace Minesweeper
{
    public class Grid
    {
        public const char EMPTY = '.';
        public const char MINE = '*';
        public readonly int y;
        public readonly int x;

        private char[][] grid;

        public Grid(char[][] rows)
        {
            grid = rows;
            y = rows.Length;

            #region preconditions
            if (y == 0)
            {
                throw new ArgumentException("Can't initialize an empty grid");
            }
            x = rows[0].Length;
            if (x == 0)
            {
                throw new ArgumentException("Can't initialize a grid with empty rows");
            }
            if (rows.Any(r => r.Length != x))
            {
                throw new ArgumentException("Row size mismatch");
            }
            #endregion
        }

        public static Grid FromPicture(string picture) =>
                new Grid(
                    (from l in picture.Split("\n")
                    select l.ToCharArray())
                    .ToArray());

        public static Parser<(int,int)> sizeParser =>
                from ys in Parse.Digit.AtLeastOnce().Text().Token()
                from xs in Parse.Digit.AtLeastOnce().Text()
                let y = Int32.Parse(ys)
                let x = Int32.Parse(xs)
                from _lineEnd in Parse.LineEnd
                select (y, x);
        
        public static Parser<Grid> gridParser =>
                from size in sizeParser
                from rows in GridParser(size)
                select new Grid(rows);
        
        public static Parser<char[]> RowParser(int x) =>
                from cs in Parse.Chars(new [] {EMPTY, MINE}).Repeat(x)
                let chars = cs.ToArray()
                select chars;

        public static Parser<char[][]> GridParser((int y, int x) size) =>
                from rs in RowParser(size.x).DelimitedBy(Parse.LineEnd)
                let rows = rs.ToArray()
                where rows.Length == size.y
                select rows;

        public string Output() =>
                ReinsertMines(ProximityGrid())
                .ToString();

        public override string ToString() =>
                (from r in grid
                 select new string(r))
                .Aggregate((a, b) => $"{a}\n{b}");

        public TOut[][] ZipOverGrid<TOut, TIn1, TIn2>
                    (TIn1[][] acc,
                     TIn2[][] grid,
                     Func<TIn1, TIn2, TOut> f) =>
                acc.Zip(
                    grid,
                    (ar, gr) =>
                        ar.Zip(gr, f)
                        .ToArray()).ToArray();

        private static int Add(int a, char g) =>
                g == MINE ? a + 1 : a;

        private static char Reinsert(int a, char g) =>
                g == MINE ? MINE : a.ToString()[0];

        private Grid ReinsertMines(int[][] pg) =>
                new Grid(ZipOverGrid(pg, grid, Reinsert));

        private int[][] ProximityGrid()
        {
            char[][][] center = { Left(), grid, Right() };
            var up = (from g in center select Up(g)).ToArray();
            var down = (from g in center select Down(g)).ToArray();
            var layers = up.Concat(center).Concat(down);

            return layers.Aggregate(
                        Zeros(),
                        (acc, grid) => ZipOverGrid(acc, grid, Add));
        }

        private char[] EmptyRow() =>
                Enumerable.Repeat(EMPTY, x).ToArray();

        private char[][] Left() =>
                (from r in grid
                 select r
                    .Skip(1)
                    .Append(EMPTY)
                    .ToArray())
                 .ToArray();

        private char[][] Right() =>
                (from r in grid
                 select r
                    .SkipLast(1)
                    .Prepend(EMPTY)
                    .ToArray())
                 .ToArray();

        private char[][] Up(char[][] grid) =>
                grid.Skip(1)
                    .Append(EmptyRow())
                    .ToArray();

        private char[][] Down(char[][] grid) =>
                grid.SkipLast(1)
                    .Prepend(EmptyRow())
                    .ToArray();

        private int[][] Zeros() =>
                (from row in grid
                 select
                  (from c in row
                   select 0).ToArray()).ToArray();
    }
}
