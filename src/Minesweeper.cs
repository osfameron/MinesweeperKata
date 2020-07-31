using System;
using System.Linq;

namespace Minesweeper
{
    public class Grid
    {
        private char[][] grid;
        public readonly int y;
        public readonly int x;
        public Grid(string picture)
        {
            #region unpack and preconditions
            // TODO: find standard idiom for this: Contract.Requires crashes `dotnet`
            if (picture.Length == 0) {
                throw new ArgumentException("Must pass in non-zero-length String");
            }
            string[] lines = picture.Split("\n");
            var rows = from l in lines
                       select l.ToCharArray();
            x = lines[0].Length;
            y = lines.Length;
            if (rows.Any(r => r.Length != x))
            {
                throw new ArgumentException("Row size mismatch");
            }
            #endregion

            grid = rows.ToArray();
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
