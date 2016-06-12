using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.IO;
using System.Xml.Serialization;

namespace LabyrinthGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private DispatcherTimer coinsTimer;
        private Labyrinth labyrinth;
        private Game game;
        private Player player = new Player();
        private List<Zombie> zombies = new List<Zombie>();
        private List<Coin> coins = new List<Coin>();
        private bool gameOver;
        private CellType[,] labMatrix;
        private const string xmlFilePath = "Results.xml";
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(GameSerializer));
        private GameSerializer gsOut = new GameSerializer();
        private GameSerializer gsIn;

        #region CTOR
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            coinsTimer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            coinsTimer.Interval = new TimeSpan(0, 0, 0, 5);
            timer.Tick += timer_Tick;
            coinsTimer.Tick += coinsTimer_Tick;
            gameCanvas.KeyDown += GameCanvas_KeyDown;
            menuStackPanel.Visibility = Visibility.Visible;
            resStackPanel.Visibility = Visibility.Collapsed;
            gameStackPanel.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region LABYRINTH
        private void RenderLabyrinth()
        {
            labMatrix = labyrinth.Generate();
            BitmapImage wallImg = new BitmapImage(labyrinth.WallImageUri);
            BitmapImage cellImg = new BitmapImage(labyrinth.CellImageUri);
            for (int i = 0; i < Labyrinth.Width; i++)
                for (int j = 0; j < Labyrinth.Height; j++)
                {
                    Rectangle cell = new Rectangle();
                    cell.Width = cell.Height = (int)Labyrinth.CellLength;
                    cell.StrokeThickness = 0;
                    cell.Stroke = cell.Fill = (labMatrix[i, j] == (int)CellType.Wall)
                        ? new ImageBrush(wallImg) : new ImageBrush(cellImg);
                    Canvas.SetLeft(cell, i * Labyrinth.CellLength);
                    Canvas.SetTop(cell, j * (Labyrinth.CellLength));
                    gameCanvas.Children.Add(cell);
                }
        }
        #endregion

        #region PLAYER
        private void RenderPlayer()
        {
            Canvas.SetLeft(player.Rect, Labyrinth.CellLength + (Labyrinth.CellLength - player.Rect.Width) / 2);
            Canvas.SetTop(player.Rect, Labyrinth.CellLength + (Labyrinth.CellLength - player.Rect.Height) / 2);
            gameCanvas.Children.Add(player.Rect);
        }

        private void UpdatePlayer()
        {
            Point corner1;
            Point corner2;
            corner1 = corner2 = new Point(player.X, player.Y);
            if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.W))
            {
                corner1.Offset(0, 0);
                corner2.Offset(player.Rect.Width, 0);
                Canvas.SetTop(player.Rect, player.Y - player.GetDistance(labMatrix, Labyrinth.CellLength, corner1, corner2, Direction.Up));
                player.NextImage(Direction.Up);
            }
            else if (Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.D))
            {
                corner1.Offset(player.Rect.Width, 0);
                corner2.Offset(player.Rect.Width, player.Rect.Height);
                Canvas.SetLeft(player.Rect, player.X + player.GetDistance(labMatrix, Labyrinth.CellLength, corner1, corner2, Direction.Right));
                player.NextImage(Direction.Right);
            }
            else if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.S))
            {
                corner1.Offset(0, player.Rect.Height);
                corner2.Offset(player.Rect.Width, player.Rect.Height);
                Canvas.SetTop(player.Rect, player.Y + player.GetDistance(labMatrix, Labyrinth.CellLength, corner1, corner2, Direction.Down));
                player.NextImage(Direction.Down);
            }
            else if (Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.A))
            {
                corner1.Offset(0, 0);
                corner2.Offset(0, player.Rect.Height);
                Canvas.SetLeft(player.Rect, player.X - player.GetDistance(labMatrix, Labyrinth.CellLength, corner1, corner2, Direction.Left));
                player.NextImage(Direction.Left);
            }
        }
        #endregion

        #region ZOMBIE
        private void RenderZombie()
        {
            Zombie z = new Zombie();
            zombies.Add(z);
            Canvas.SetLeft(z.Rect, Labyrinth.CellLength * (Labyrinth.Width - 2) + (Labyrinth.CellLength - z.Rect.Width) / 2);
            Canvas.SetTop(z.Rect, Labyrinth.CellLength * (Labyrinth.Height - 2) + (Labyrinth.CellLength - z.Rect.Height) / 2);
            gameCanvas.Children.Add(z.Rect);
        }

        private void UpdateZombies()
        {
            foreach (Zombie zombie in zombies)
            {
                zombie.NextImage();
                if (zombie.HitMode)
                    continue;
                if (zombie.PassedDist >= Labyrinth.CellLength)
                {
                    if (Zombie.PursuitMode)
                        zombie.PursuitWalk(labMatrix, GetCenter(new Point(player.X, player.Y), new Size(player.Rect.Width, player.Rect.Height)));
                    else
                        zombie.RandomWalk(labMatrix);
                    zombie.PassedDist = 0;
                }
                Canvas.SetLeft(zombie.Rect, zombie.X + zombie.Distance * zombie.DirectionStep.X);
                Canvas.SetTop(zombie.Rect, zombie.Y + zombie.Distance * zombie.DirectionStep.Y);
                if (zombie.PassedDist + zombie.Distance >= Labyrinth.CellLength)
                {
                    zombie.PassedDist = Labyrinth.CellLength;
                    Point cellCenter = Cell.GetCellCenter(new Point(zombie.X + zombie.Rect.Width / 2, zombie.Y + zombie.Rect.Height / 2));
                    Canvas.SetLeft(zombie.Rect, cellCenter.X - zombie.Rect.Width / 2);
                    Canvas.SetTop(zombie.Rect, cellCenter.Y - zombie.Rect.Height / 2);
                }
                else
                    zombie.PassedDist += zombie.Distance;
            }
        }
        #endregion

        #region COIN
        private void RenderCoin()
        {
            if (coins.Count == 10)
                return;
            Coin c = new Coin();
            Random rand = new Random();
            Cell cell = new Cell(-1, -1);
            while (cell.X == -1 || cell.Y == -1)
            {
                int x = rand.Next(0, Labyrinth.Width);
                int y = rand.Next(0, Labyrinth.Height);
                if (labMatrix[x, y] == CellType.Visited)
                    cell = new Cell(x, y);
                Point cellCenter = Cell.GetCellCenter(new Point(cell.X * Labyrinth.CellLength, cell.Y * Labyrinth.CellLength));
                Canvas.SetLeft(c.Rect, cellCenter.X - c.Rect.Width / 2);
                Canvas.SetTop(c.Rect, cellCenter.Y - c.Rect.Height / 2);
                foreach (Coin coin in coins)
                    if (c.X == coin.X && c.Y == coin.Y)
                        cell = new Cell(-1, -1);
            }
            coins.Add(c);
            gameCanvas.Children.Add(c.Rect);
        }

        private void UpdateCoins()
        {
            if (DateTime.Now.Ticks % 4 == 0)
                foreach (Coin coin in coins)
                    coin.NextImage();
        }
        #endregion

        #region TIMERS
        private void timer_Tick(object sender, EventArgs e)
        {
            timeTextBlock.Text = game.TimeString;
            Point playerCenter = GetCenter(new Point(player.X, player.Y), new Size(player.Rect.Width, player.Rect.Height));
            Rect playerRect = new Rect(player.X - 4, player.Y, player.Rect.Width + 16, player.Rect.Height);
            foreach (Zombie zombie in zombies)
            {
                if (zombie.HitMode)
                {
                    UpdateZombies();
                    if (zombie.AnimationEnded)
                        EndGame(true);
                    return;
                }
                Rect zombieRect = new Rect(zombie.X, zombie.Y, zombie.Rect.Width, zombie.Rect.Height);
                if (playerRect.IntersectsWith(zombieRect))
                {
                    zombie.SetHitMode();
                    return;
                }
            }
            List<Coin> tempCoins = new List<Coin>();
            foreach (Coin coin in coins)
            {
                Rect coinRect = new Rect(coin.X, coin.Y, coin.Rect.Width, coin.Rect.Height);
                if (playerRect.IntersectsWith(coinRect))
                {
                    gameCanvas.Children.Remove(coin.Rect);
                    game.CoinsCount++;
                    coinsCountTextBlock.Text = "Монет: " + game.CoinsCount.ToString();
                    if (game.CoinsCount == 10 && zombies.Count < 2)
                        RenderZombie();
                    else if (game.CoinsCount == 30)
                        Zombie.PursuitMode = true;
                    Zombie.SpeedUp();
                    continue;
                }
                tempCoins.Add(coin);
            }
            coins = tempCoins;
            UpdatePlayer();
            UpdateZombies();
            UpdateCoins();
        }

        private void coinsTimer_Tick(object sender, EventArgs e)
        {
            RenderCoin();
        }
        #endregion

        #region EVENT_HANDLERS
        private void GameCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                EndGame(false);
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void resultsButton_Click(object sender, RoutedEventArgs e)
        {
            menuStackPanel.Visibility = Visibility.Collapsed;
            resStackPanel.Visibility = Visibility.Visible;
            resultsListView.ItemsSource = Deserialize(xmlFilePath);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            menuStackPanel.Visibility = Visibility.Visible;
            resStackPanel.Visibility = Visibility.Collapsed;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(xmlFilePath);
            resultsListView.ItemsSource = null;
        }

        private void toMenuButton_Click(object sender, RoutedEventArgs e)
        {
            EndGame(false);
            menuStackPanel.Visibility = Visibility.Visible;
            gameStackPanel.Visibility = Visibility.Collapsed;
        }

        private void anewButton_Click(object sender, RoutedEventArgs e)
        {
            EndGame(false);
            StartGame();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EndGame(false);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (this.IsLoaded && game != null && !gameOver)
            {
                game.StartTimer();
                timer.Start();
                coinsTimer.Start();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (game != null)
            {
                game.PauseTimer();
                timer.Stop();
                coinsTimer.Stop();
            }
        }
        #endregion

        #region METHODS
        private Point GetCenter(Point leftTopPoint, Size size)
        {
            return new Point(leftTopPoint.X + size.Width / 2,
                leftTopPoint.Y + size.Height / 2);
        }

        private void StartGame()
        {
            gameCanvas.Children.Clear();
            menuStackPanel.Visibility = Visibility.Collapsed;
            gameStackPanel.Visibility = Visibility.Visible;
            labyrinth = new Labyrinth();
            player = new Player();
            zombies = new List<Zombie>();
            Zombie.PursuitMode = false;
            coins = new List<Coin>();
            gameOver = false;
            gameCanvas.Width = Labyrinth.CellLength * Labyrinth.Width;
            gameCanvas.Height = Labyrinth.CellLength * Labyrinth.Height;
            game = new Game(playerNameTextBox.Text);
            playerNameTextBlock.Text = game.PlayerName;
            RenderLabyrinth();
            RenderPlayer();
            RenderZombie();
            RenderCoin();
            gameCanvas.Focus();
            timer.Start();
            coinsTimer.Start();
            coinsCountTextBlock.Text = "Монет: 0";
            gameInfoTextBlock.Text = "Соберите как можно больше монет, не попадитесь зомби!";
            playerNameTextBlock.Foreground = coinsCountTextBlock.Foreground = gameInfoTextBlock.Foreground
                = timeTextBlock.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void EndGame(bool gameFailed)
        {
            if (game == null || gameOver)
                return;
            gameOver = true;
            timer.Stop();
            coinsTimer.Stop();
            game.Time = game.TimeString;
            game.Date = game.DateString;
            game.EndingReason = gameFailed ? "Гибель персонажа" : "Выход";
            List<Game> loadedResults = Deserialize(xmlFilePath);
            Serialize(loadedResults);
            gameInfoTextBlock.Text = "ВЫ ПРОИГРАЛИ";
            playerNameTextBlock.Foreground = coinsCountTextBlock.Foreground = gameInfoTextBlock.Foreground
                = timeTextBlock.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void Serialize(List<Game> loadedResults)
        {
            try
            {
                using (FileStream streamOut = new FileStream(xmlFilePath, FileMode.Create, FileAccess.Write))
                {
                    loadedResults.Reverse();
                    loadedResults.Add(game);
                    gsOut.Games = loadedResults;
                    serializer.Serialize(streamOut, gsOut);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<Game> Deserialize(string xmlFile)
        {
            List<Game> loadedResults = new List<Game>();
            try
            {
                if (File.Exists(xmlFile))
                {
                    using (FileStream streamIn = new FileStream(xmlFile, FileMode.Open, FileAccess.Read))
                    {
                        gsIn = serializer.Deserialize(streamIn) as GameSerializer;
                        loadedResults = gsIn.Games;
                        loadedResults.Reverse();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return loadedResults;
        }
        #endregion
    }
}
