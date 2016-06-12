using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LabyrinthGame
{
    class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Cell GetCell(Point currentPoint, double cellLength)
        {
            return new Cell((int)(currentPoint.X / cellLength),
                (int)(currentPoint.Y / cellLength));
        }

        public static Point GetCellCenter(Point point)
        {
            int x = (int)(point.X / Labyrinth.CellLength);
            int y = (int)(point.Y / Labyrinth.CellLength);
            return new Point(x * Labyrinth.CellLength + Labyrinth.CellLength / 2,
                y * Labyrinth.CellLength + Labyrinth.CellLength / 2);
        }

        public static List<Cell> GetNeighbors(CellType[,] labyrinth, Cell currCell, CellType neighborType, int intervalToNeighbourCell)
        {
            Cell[] allNeighbors = new Cell[] { new Cell(currCell.X - intervalToNeighbourCell, currCell.Y),
                                            new Cell(currCell.X + intervalToNeighbourCell, currCell.Y),
                                            new Cell(currCell.X, currCell.Y - intervalToNeighbourCell),
                                            new Cell(currCell.X, currCell.Y + intervalToNeighbourCell) };
            List<Cell> neighbors = new List<Cell>();
            foreach (Cell c in allNeighbors)
                if (c.X >= 0 && c.X < Labyrinth.Width && c.Y >= 0 && c.Y < Labyrinth.Height
                    && labyrinth[c.X, c.Y] == neighborType)
                    neighbors.Add(c);
            return neighbors;
        }
    }
}