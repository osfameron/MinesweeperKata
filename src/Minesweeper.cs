using System;
using System.Linq;

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
            x = rows[0].Length;
            y = rows.Length;

            #region preconditions
            if (x == 0)
            {
                throw new ArgumentException("Can't initialize an empty grid");
            }
            if (rows.Any(r => r.Length != x))
            {
                throw new ArgumentException("Row size mismatch");
            }
            #endregion
        }
        public static Grid FromPicture(string picture)
        {
            var rows = (from l in picture.Split("\n")
                        select l.ToCharArray())
                       .ToArray();
            return new Grid(rows);
        }

        public string Output()
        {
            return
                ReinsertMines(ProximityGrid())
                .ToString();
        }

        public override string ToString()
        {
            return (from r in grid
                    select new string(r))
                   .Aggregate((a, b) => $"{a}\n{b}");
        }

        public TOut[][] ZipOverGrid<TOut, TIn1, TIn2>(
                                            TIn1[][] acc,
                                            TIn2[][] grid,
                                            Func<TIn1, TIn2, TOut> f)
        {
            return acc.Zip(grid,
                (ar, gr) =>
                    ar.Zip(gr, f)
                .ToArray()).ToArray();
        }

        private static int Add(int a, char g)
        {
            return g == MINE ? a + 1 : a;
        }

        private Grid ReinsertMines(int[][] pg)
        {
            var final = ZipOverGrid(pg,
                                    grid,
                                    (p, g) => g == MINE ? MINE : p.ToString()[0]);
            return new Grid(final);
        }

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

        private char[] EmptyRow()
        {
            return Enumerable.Repeat(EMPTY, x).ToArray();
        }

        private char[][] Left()
        {
            return
                (from r in grid
                 select r
                    .Skip(1)
                    .Append(EMPTY)
                    .ToArray())
                 .ToArray();
        }

        private char[][] Right()
        {
            return
                (from r in grid
                 select r
                    .SkipLast(1)
                    .Prepend(EMPTY)
                    .ToArray())
                 .ToArray();
        }

        private char[][] Up(char[][] grid)
        {
            return
                grid.Skip(1).Append(EmptyRow()).ToArray();
        }
        private char[][] Down(char[][] grid)
        {
            return
                grid.SkipLast(1).Prepend(EmptyRow()).ToArray();
        }

        private int[][] Zeros()
        {
            return
                (from row in grid
                 select
                  (from c in row
                   select 0).ToArray()).ToArray();
        }

    }
}
