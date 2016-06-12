using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LabyrinthGame
{
    class Zombie
    {
        public Rectangle Rect { get; set; }
        public double Distance { get { return step / time; } }
        public double PassedDist = Labyrinth.CellLength;
        public static bool PursuitMode = false;
        public Point DirectionStep = new Point(0, -1);
        public double X { get { return Canvas.GetLeft(Rect); } }
        public double Y { get { return Canvas.GetTop(Rect); } }
        public bool HitMode;
        public bool AnimationEnded { get { return (spriteIndex == spriteCount - 1) ? true : false; } }
        private int spriteIndex = -1;
        private int spriteCount { get { return (HitMode) ? imageUrisHit.Length : imageUrisGo.Length; } }
        private Stack<Point> directionsToPlayer = new Stack<Point>();
        private static double startStep = 1;
        private static double step = 1;
        private static double time = 0.9;
        private Random rand = new Random();
        private Point currDir;        
        private Uri[] imageUris { get { return HitMode ? imageUrisHit : imageUrisGo; } }
        private Uri[] imageUrisGo = new Uri[] { 
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_1.png"), 
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_2.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_3.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_4.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_5.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_6.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_7.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_8.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_9.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/go_10.png")};
        private Uri[] imageUrisHit = new Uri[] { 
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/hit_1.png"), 
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/hit_2.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/hit_3.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/hit_4.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/hit_5.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/hit_6.png"),
                                                new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Zombie/hit_7.png")};

        public Zombie()
        {
            Rect = new Rectangle();
            NextImage();
        }

        public void NextImage()
        {
            spriteIndex++;
            if (spriteIndex >= spriteCount)
                spriteIndex = 0;
            BitmapSource image = new BitmapImage(imageUris[spriteIndex]);
            currDir = (DirectionStep == Direction.Left || DirectionStep == Direction.Right) ? DirectionStep : currDir;
            if (currDir == Direction.Right)
                image = new TransformedBitmap(image, new ScaleTransform(-1, 1, 0, 0));
            Rect.Fill = new ImageBrush(image);
            Rect.Width = image.Width;
            Rect.Height = image.Height;
        }

        public void SetHitMode()
        {
            HitMode = true;
            spriteIndex = 0;
        }

        public static void SpeedUp()
        {
            step += startStep * 0.05;
        }

        public void RandomWalk(CellType[,] labMatrix)
        {
            Point cellCenter = Cell.GetCellCenter(new Point(X + Rect.Width / 2, Y + Rect.Height / 2));
            Cell currentCell = Cell.GetCell(cellCenter, Labyrinth.CellLength);
            //get empty neighbor cells
            List<Cell> cellNeighbors = Cell.GetNeighbors(labMatrix, currentCell, CellType.Visited, 1);
            List<Point> directions = new List<Point>();
            //add possible directions
            foreach (Cell c in cellNeighbors)
            {
                if (c.X == currentCell.X)
                    directions.Add(c.Y > currentCell.Y ? Direction.Down : Direction.Up);
                else
                    directions.Add(c.X > currentCell.X ? Direction.Right : Direction.Left);
            }
            //if zombie at the crossroad/deadlock OR in the corner
            if (directions.Count != 2 || (directions[0].X != directions[1].X && directions[0].Y != directions[1].Y))
            {
                //if zombie is not in deadlock, remove direction where he came from
                if (directions.Count != 1)
                    directions.Remove(new Point(DirectionStep.X * -1, DirectionStep.Y * -1));
                DirectionStep = directions[rand.Next(0, directions.Count)];
            }
        }

        public void PursuitWalk(CellType[,] labMatrix, Point playerPos)
        {
            Cell startCell = Cell.GetCell(new Point(X + Rect.Width / 2, Y + Rect.Height / 2), Labyrinth.CellLength);
            Cell finishCell = Cell.GetCell(new Point(playerPos.X, playerPos.Y), Labyrinth.CellLength);

            const int blank = -1;
            const int wall = -2;
            int currIndex = 0;
            bool notAllCellsMarked;
            //mark all cells like blank or wall constants depending on its CellType
            int[,] zombieLabMatrix = new int[Labyrinth.Width, Labyrinth.Height];
            for (int i = 0; i < Labyrinth.Width; i++)
                for (int j = 0; j < Labyrinth.Height; j++)
                    zombieLabMatrix[i, j] = labMatrix[i, j] == CellType.Wall ? wall : blank;
            zombieLabMatrix[startCell.X, startCell.Y] = 0;
            //wave expansion of marked cells from startCell 
            do
            {
                notAllCellsMarked = true;
                for (int x = 0; x < Labyrinth.Width; x++)
                    for (int y = 0; y < Labyrinth.Height; y++)
                        if (zombieLabMatrix[x, y] == currIndex)
                        {
                            List<Cell> cellNeighbors = Cell.GetNeighbors(labMatrix, new Cell(x, y), CellType.Visited, 1);
                            foreach (Cell c in cellNeighbors)
                                if (zombieLabMatrix[c.X, c.Y] == blank)
                                {
                                    notAllCellsMarked = false;
                                    zombieLabMatrix[c.X, c.Y] = currIndex + 1;
                                }
                        }
                currIndex++;
            } while (!notAllCellsMarked && zombieLabMatrix[finishCell.X, finishCell.Y] == blank);
            //find the shortest chain of indexes from startCell to finishCell in matrix
            currIndex = zombieLabMatrix[finishCell.X, finishCell.Y];
            Cell currCell = new Cell(finishCell.X, finishCell.Y);
            while (currIndex > zombieLabMatrix[startCell.X, startCell.Y])
            {
                currIndex--;
                List<Cell> cellNeighbors = Cell.GetNeighbors(labMatrix, currCell, CellType.Visited, 1);
                foreach (Cell c in cellNeighbors)
                    if (zombieLabMatrix[c.X, c.Y] == currIndex)
                    {
                        directionsToPlayer.Push(new Point(currCell.X - c.X, currCell.Y - c.Y));
                        currCell = c;
                    }
            }
            DirectionStep = directionsToPlayer.Pop();
        }
    }
}
