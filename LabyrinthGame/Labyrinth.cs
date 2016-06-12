using System;
using System.Collections.Generic;

namespace LabyrinthGame
{
    class Labyrinth
    {
        public const double CellLength = 90;
        public const int Width = 17;
        public const int Height = 9;
        public readonly Uri WallImageUri = new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Wall.png", UriKind.Absolute);
        public readonly Uri CellImageUri = new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Ground.png", UriKind.Absolute);

        public CellType[,] Generate()
        {
            CellType[,] labyrinth = new CellType[Width, Height];
            //every odd cell marks like unvisited cell
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (i % 2 != 0 && j % 2 != 0)
                        labyrinth[i, j] = CellType.Unvisited;

            Stack<Cell> visitedCells = new Stack<Cell>();
            List<Cell> neighbors = new List<Cell>();
            Random rand = new Random();
            Cell currentCell = new Cell(1, 1);
            Cell neighborCell;
            visitedCells.Push(currentCell);
            do
            {
                labyrinth[currentCell.X, currentCell.Y] = CellType.Visited;
                neighbors = Cell.GetNeighbors(labyrinth, currentCell, CellType.Unvisited, 2);
                //if cell has unvisited neighbors, push random neighbor to stack 
                //and mark the Wall in front of it like Visited
                if (neighbors.Count > 0)
                {
                    neighborCell = neighbors[rand.Next(0, neighbors.Count)];
                    RemoveWall(labyrinth, currentCell, neighborCell);
                    currentCell = neighborCell;
                    visitedCells.Push(currentCell);
                }
                //if cell has only visited neighbors, mark the Wall (with 90% chance)
                //in front of it like Visited
                else
                {
                    neighbors = Cell.GetNeighbors(labyrinth, currentCell, CellType.Visited, 2);
                    neighborCell = (rand.Next(0, 10) == 0) ? currentCell : neighbors[rand.Next(0, neighbors.Count)];
                    RemoveWall(labyrinth, currentCell, neighborCell);
                    currentCell = visitedCells.Pop();
                }
            }
            while (visitedCells.Count > 0);
            return labyrinth;
        }

        private void RemoveWall(CellType[,] labyrinth, Cell firstCell, Cell secondCell)
        {
            Cell middleCell = new Cell((firstCell.X + secondCell.X) / 2, (firstCell.Y + secondCell.Y) / 2);
            labyrinth[middleCell.X, middleCell.Y] = CellType.Visited;
        }
    }
}
