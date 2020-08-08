using System;
using System.Linq;
using Sprache;

namespace Minesweeper
{
    /** <summary>
        Class <c>Game</c> represents the game grid for the Minesweeper Kata.
        </summary>
    */
    public class Game
    {
        /// <summary> Empty square on the grid </summary>
        public const char EMPTY = '.';

        /// <summary> Mine on the grid </summary>
        public const char MINE  = '*';

        /// <summary> The game grid is represented as a <c>Grid{T}</c> of <c>char</c>.</summary>
        private Grid<char> grid;

        /// <summary> Vertical size of grid </summary>
        public int Y { get => grid.Y; }

        /// <summary> Horizontal size of grid </summary>
        public int X { get => grid.X; }

        /** <summary>
            Constructor. Accepts a grid of characters.
            Usually this includes just <c>.</c> (empty) and <c>*</c> (mine)
            but this isn't enforced.
            </summary>
        */
        public Game(Grid<char> g) => grid = g;

        /// <summary> Constructor, passing in nested grid, for convenience. </summary>
        public Game(char[][] rows) => grid = new Grid<char>(rows);

        /** <summary>
            Static factory, creating a <c>Game</c> from a string picture.
            This method currently does no validation of the input grid.
            
            See <c>gridParser</c> for a better approach.
            </summary>
        */
        public static Game FromPicture(string picture) =>
                new Game(
                    (from l in picture.Split("\n")
                    select l.ToCharArray())
                    .ToArray());

        /** <summary>
            Static factory, creating a <c>Game</c> from the Kata-specified
            format of y-size, x-size, and a grid picture.

            <example><code>
            2 3
            ..*
            .*.</code></example>
            </summary>
        */
        public static Parser<Game> gridParser =>
                from size in sizeParser
                from rows in GridParser(size)
                select new Game(rows);

        /// <summary> Parser for the <c>y x</c> format to declare size</summary>
        public static Parser<(int,int)> sizeParser =>
                from ys in Parse.Digit.AtLeastOnce().Text().Token()
                from xs in Parse.Digit.AtLeastOnce().Text()
                let y = Int32.Parse(ys)
                let x = Int32.Parse(xs)
                from _ in Parse.LineEnd
                select (y, x);
        
        /// <summary> Parser for a game row full of <c>.</c> and <c>*</c> characters </summary>
        public static Parser<char[]> RowParser(int x) =>
                from cs in Parse.Chars(new [] {EMPTY, MINE})
                           .Repeat(x)
                let chars = cs.ToArray()
                select chars;

        /// <summary> Parser for a game grid of a specific size </summary>
        public static Parser<char[][]> GridParser((int y, int x) size) =>
                from rs in RowParser(size.x)
                           .DelimitedBy(Parse.LineEnd)
                let rows = rs.ToArray()
                where rows.Length == size.y
                select rows;

        /// <summary> Delegate ToString to the underlying grid </summary>?
        public override string ToString() =>
                grid.ToString();

        /// <summary> Calculate the output, with proximity counts </summary>
        public string Output() =>
                ReinsertMines(ProximityGrid)
                .ToString();

        private static int Add(int a, char g) =>
                g == MINE ? a + 1 : a;

        /// <summary> 9 copies of the grid, the original + shifted in all cardinal and diagonal directions </summary>
        private Grid<char>[] ProximityLayers()
        {
            Grid<char>[] center = { grid.Left(EMPTY), grid, grid.Right(EMPTY) };
            Grid<char>[] up   = center.Select(g => g.Up(EMPTY)).ToArray();
            Grid<char>[] down = center.Select(g => g.Down(EMPTY)).ToArray();
            return up.Concat(center).Concat(down).ToArray();
        }

        /// <summary> A grid containing just the number 0 </summary>
        private Grid<int> Zeros =>
                grid.Map(_ => 0);

        /// <summary> Aggregate the proximity layers, adding 1 for each mine found </summary>
        private Grid<int> ProximityGrid =>
                ProximityLayers()
                    .Aggregate(
                        Zeros,
                        (acc, grid) => acc.Zip(grid, Add));
    
        private static char Reinsert(int a, char g) =>
                g == MINE ? MINE : a.ToString()[0];

        /// <summary> Add the original mine <c>*</c> back onto the numerical proximity grid </summary>
        private Grid<char> ReinsertMines(Grid<int> pg) =>
                pg.Zip(grid, Reinsert);
    
    }

    /** <summary>
        Generic class representing a Grid as a nested array.
        This refactor simplifies a lot of the type signatures
        (nobody wants to read <c>char[][][]</c>) and separates
        the Game logic from basic data-structure handling.
        </summary>
    */
    public class Grid<T>
    {
        public T[][] Data { get; }
        public int Y { get; }
        public int X { get; }

        /// <summary> The constructor takes a nested array, and ensures it's well formed as a grid </summary>
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
        
        /** <summary>
            The <c>Zip</c> method takes a second grid and a function, returning
            a 3rd grid, with the function applied over each cell, in the manner
            of Linq's <c>Enumerable.Zip</c> but over 2 dimensions.
            </summary>
        */
        public Grid<TOut> Zip<TOut, TIn> (Grid<TIn> g2, Func<T, TIn, TOut> f) =>
                new Grid<TOut>(
                    Data.Zip(g2.Data, (r1, r2) => r1.Zip(r2, f).ToArray())
                    .ToArray());

        /// <summary> Map a function over the grid </summary>
        public Grid<TOut> Map<TOut> (Func<T, TOut> f) =>
                new Grid<TOut> (
                    Data.Select(row => row.Select(f).ToArray())
                        .ToArray());

        /** <summary>
            Utility method, taking a pair of functions which summarize
            <list>
                <item><description> a row </description></item>
                <item><description> an array of row summaries </description></item>
            </list>
            </summary>
        */
        public TOut Comprehend<TOut, TRow> (Func<T[], TRow> aggregateRow, Func<TRow[], TOut> aggregateRows) =>
                aggregateRows(
                    Data
                        .Select(row => aggregateRow(row))
                        .ToArray());

        // this is basically a delegate for Grid<T>, is there a builtin way of doing this in C#?
        private Grid<T> Reform(T[][] rows) => new Grid<T> (rows);

        // A Row of the given element.
        private T[] RowOf(T r) =>
                Enumerable.Repeat(r, X).ToArray();

        /// <summary> Shift a grid to the left </summary>
        public Grid<T> Left(T zero) =>
                Comprehend(
                    r => r.Skip(1).Append(zero).ToArray(),
                    Reform);

        /// <summary> Shift a grid to the right </summary>
        public Grid<T> Right(T zero) =>
                Comprehend(
                    r => r.SkipLast(1).Prepend(zero).ToArray(),
                    Reform);

        /// <summary> Shift a grid up </summary>
        public Grid<T> Up(T zero) =>
                Reform(
                    Data.Skip(1)
                    .Append(RowOf(zero))
                    .ToArray());
 
        /// <summary> Shift a grid down </summary>
        public Grid<T> Down(T zero) =>
                Reform(
                    Data.SkipLast(1)
                    .Prepend(RowOf(zero))
                    .ToArray());

        /// <summary> ToString method, requires the underlying {T} to have a sensible ToString </summary>
        public override string ToString() =>
                Comprehend(r => String.Concat(r.Select(c => c.ToString()).ToArray()),
                           rs => rs.Aggregate((a,b) => $"{a}\n{b}"));
    }
}
