using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LabyrinthGame
{
    class Coin
    {
        public Rectangle Rect { get; set; }
        public double X { get { return Canvas.GetLeft(Rect); } }
        public double Y { get { return Canvas.GetTop(Rect); } }
        private int spriteIndex = 0;
        private int spriteCount = 4;
        private int imgDir = 1;
        private Uri[] imageUris = new Uri[] { new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Coin/Coin1.png"), 
                                               new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Coin/Coin2.png"),
                                               new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Coin/Coin3.png"),
                                               new Uri(@"pack://application:,,,/LabyrinthGame;component/Images/Coin/Coin4.png")};

        public Coin()
        {
            Rect = new Rectangle();           
            NextImage();
        }

        public void NextImage()
        {
            spriteIndex += imgDir;
            if (spriteIndex == 0)
                imgDir = 1;
            else if (spriteIndex == spriteCount - 1)
                //set reverse order of loading images
                imgDir = -1;
            BitmapSource image = new BitmapImage(imageUris[spriteIndex]);
            Rect.Fill = new ImageBrush(image);
            Rect.Width = image.Width;
            Rect.Height = image.Height;
        }
    }
}
