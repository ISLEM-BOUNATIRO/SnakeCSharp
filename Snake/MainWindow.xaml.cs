using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images. Empty },
            {GridValue.Snake, Images. Body },
            {GridValue.Food, Images. Food }
        };
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        private readonly int rows = 15, cols =15 ; //16 24
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private bool gamepaused = false;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            SoundPlayer gameover = new("gameover.wav");
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            gameover.Play();
            await DrawDeadSnake();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Space:
                    gamepaused = !gamepaused;
                    break;
            }
        }
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                while (gamepaused)
                    await Task.Delay(1);
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }
        class MyImage :Image
        {
            public int col{ get; set; }
            public int row { get; set; }
        }
        private Image[,] SetupGrid()
        {
            MyImage[,] images = new MyImage[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    MyImage image = new()
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    image.col = c;
                    image.row = r;
                    images[r, c] = image;
                    //images[r, c].MouseLeftButtonUp += Image_MouseLeftButtonUp;
                    GameGrid.Children.Add(image);


                }
            }
            return images;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyImage m = (MyImage)sender;
            //m.Source= Images.Food;

            gameState.Grid[m.row, m.col] = GridValue.Food;
            //ScoreText.Text =m.row.ToString();
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }
        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head; int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }
        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }
        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }
        private async Task ShowGameOver()
        {
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START PRESS SPACE TO PAUSE";
        }
    }
}
