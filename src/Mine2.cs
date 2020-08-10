using System;

namespace Mine2
{
    public class Rose
    {
        public enum Direction { N, NE, E, SE, S, SW, W, NW };
        public static Direction Rotate(Direction r, int c)
        {
            return (Direction) ((int) r + c);
        }
    }

    public class Cell
    {

    }
}
