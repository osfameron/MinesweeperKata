#define CONTRACTS_FULL

using System;
using System.Linq;

namespace Minesweeper
{
    public class Grid
    {
        private char[][] grid;
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
            var lenFirst = lines[0].Length;
            if (rows.Any(r => r.Length != lenFirst))
            {
                throw new ArgumentException("Row size mismatch");
            }
            #endregion

            grid = rows.ToArray();
        }

        public string ToString()
        {
            return (from r in grid
                    select new string(r))
                   .Aggregate((a,b) => $"{a}\n{b}");
        }
    }
}
