using System;
using System.Windows;

namespace LabyrinthGame
{
    static class Direction
    {
        public static Point Left { get { return new Point(-1, 0); } }
        public static Point Right { get { return new Point(1, 0); } }
        public static Point Up { get { return new Point(0, -1); } }
        public static Point Down { get { return new Point(0, 1); } }
    }
}
