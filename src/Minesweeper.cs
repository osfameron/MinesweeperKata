using System;
using System.Linq;

namespace Minesweeper
{
    public class Grid
    {
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

        public char[][] Left()
        {
            return grid; // stub
        }

        public override string ToString()
        {
            return (from r in grid
                    select new string(r))
                   .Aggregate((a,b) => $"{a}\n{b}");
        }
    }
}
