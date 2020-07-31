using System;
using System.Linq;

namespace Minesweeper
{
    public class Grid
    {
        public const char EMPTY = '.';
        public const char MINE = '*';

        private char[][] grid;
        public readonly int y;
        public readonly int x;

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

        public int[][] ProximityGrid()
        {
            char[][][] layers = {Left(), Right(), Up(), Down()};
            return layers.Aggregate(
                        Zeros(),
                        Add);
        }

        public char[][] Left()
        {
            return
                (from r in grid
                 select r
                    .Skip(1)
                    .Append(EMPTY)
                    .ToArray())
                 .ToArray();
        }

        public char[][] Right()
        {
            return
                (from r in grid
                 select r
                    .SkipLast(1)
                    .Prepend(EMPTY)
                    .ToArray())
                 .ToArray();
        }
        
        private char[] EmptyRow() {
            return Enumerable.Repeat(EMPTY, x).ToArray();
        }

        public char[][] Up()
        {
            return
                grid.Skip(1).Append(EmptyRow()).ToArray();
        }
        public char[][] Down()
        {
            return
                grid.SkipLast(1).Prepend(EmptyRow()).ToArray();
        }

        public int[][] Zeros()
        {
            return
                (from row in grid
                select
                 (from c in row
                 select 0).ToArray()).ToArray();
        }

        public static int[][] Add(int[][] acc, char[][] grid)
        {
            return acc.Zip(grid,
                (ar,gr) =>
                    ar.Zip(gr,
                        (a,g) => g == MINE ? a + 1 : a)
                .ToArray()).ToArray();
        }

        public override string ToString()
        {
            return (from r in grid
                    select new string(r))
                   .Aggregate((a,b) => $"{a}\n{b}");
        }
    }
}
