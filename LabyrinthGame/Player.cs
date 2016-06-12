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
    class Player
    {
        public Rectangle Rect { get; set; }
        public double X { get { return Canvas.GetLeft(Rect); } }
        public double Y { get { return Canvas.GetTop(Rect); } }
        private readonly Uri imageUri = new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Player.png", UriKind.Absolute);
        private double step = 3;
        private double time = 1;
        private int spriteIndex = -1;
        private int spriteCount = 8;
        private Point currDir;

        public Player()
        {
            Rect = new Rectangle();
            NextImage(Direction.Right);
        }

        public void NextImage(Point direction)
        {
            spriteIndex++;
            if (spriteIndex >= spriteCount)
                spriteIndex = 0;
            BitmapSource image = new BitmapImage(imageUri);
            BitmapSource croppedImg = new CroppedBitmap(image, new Int32Rect(spriteIndex * ((int)image.Width / spriteCount),
                                                        0, (int)image.Width / spriteCount, 64));
            currDir = (direction == Direction.Left || direction == Direction.Right) ? direction : currDir;
            if (currDir == Direction.Left)
                croppedImg = new TransformedBitmap(croppedImg, new ScaleTransform(-1, 1, 0, 0));
            Rect.Fill = new ImageBrush(croppedImg);
            Rect.Width = croppedImg.Width;
            Rect.Height = croppedImg.Height;
        }

        public double GetDistance(CellType[,] labyrinth, double cellLength,
            Point currentCorner1, Point currentCorner2, Point direction)
        {
            double offset = -1;
            Cell cell1;
            Cell cell2;
            do
            {
                offset++;
                currentCorner1.Offset(direction.X, direction.Y);
                currentCorner2.Offset(direction.X, direction.Y);
                cell1 = Cell.GetCell(currentCorner1, cellLength);
                cell2 = Cell.GetCell(currentCorner2, cellLength);
            } while (offset <= step && labyrinth[cell1.X, cell1.Y] != CellType.Wall &&
                 labyrinth[cell2.X, cell2.Y] != CellType.Wall);
            return offset / time;
        }
    }
}
