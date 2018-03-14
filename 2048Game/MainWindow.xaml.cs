using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game _Game;
        private MainMenu _MainMenu;
        private static ScoreBase _ScoreBase = new ScoreBase();

        public static ScoreBase ScoreBase { get { return _ScoreBase; } set { _ScoreBase = value; } }

        public MainWindow()
        {
            try
            {
                _ScoreBase = ScoreBase.ScoresLoad();
            }
            catch
            {
                _ScoreBase = new ScoreBase();
            }

            InitializeComponent();

            _MainMenu = new MainMenu(this);
            
            Grid.SetColumnSpan(_MainMenu, 3);
            Grid.SetRowSpan(_MainMenu, 4);

            mainGrid.Children.Add(_MainMenu);
        }

        /// <summary>
        /// Инициализация игрового поля
        /// </summary>
        public void GameStart()
        {
            Game.G_Size = Game.G_ChangedSize;
            _Game = new Game(this);

            Grid.SetRow(_Game, 3);
            Grid.SetColumnSpan(_Game, 3);

            mainGrid.Children.Add(_Game);
            _Game.UpdateLayout();
        }

        public void GameRestart()
        {
            if (Game.Score > 0)
            {
                ScoreBase.AddScore(new UserInfo("player", Game.Score));
            }

            Game.G_Size = Game.G_ChangedSize;
            _Game = new Game(this);

            Grid.SetRow(_Game, 3);
            Grid.SetColumnSpan(_Game, 3);

            mainGrid.Children.Add(_Game);
            _Game.UpdateLayout();
        }

        /// <summary>
        /// Обработка нажатий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_Game != null)
            {
                if (e.Key == Key.Escape)
                {
                    if (_Game.IsInterfaceLocked)
                    {
                        Animations.OpacityAnimation(_MainMenu, 1, 0, 0.3, this);
                    }
                    else
                    {
                        ShowMainMenu();
                    }
                }
                else
                    _Game.Game_KeyDown(sender, e);
            }
        }

        public void ShowMainMenu()
        {
            _MainMenu = new MainMenu(this);

            Grid.SetColumnSpan(_MainMenu, 3);
            Grid.SetRowSpan(_MainMenu, 4);

            mainGrid.Children.Add(_MainMenu);

            Animations.OpacityAnimation(_MainMenu, 0, 1, 0.3);

            _Game.IsInterfaceLocked = true;
        }

        /// <summary>
        /// Вывод поражения
        /// </summary>
        public void Lose()
        {
            var loseState = new LoseState(this);

            Grid.SetColumn(loseState, 0);
            Grid.SetRow(loseState, 0);
            Grid.SetColumnSpan(loseState, 3);
            Grid.SetRowSpan(loseState, 4);

            mainGrid.Children.Add(loseState);
            Animations.OpacityAnimation(loseState, 0, 1, 1.5);
        }

        /// <summary>
        /// Вывод победы
        /// </summary>
        public void Win()
        {
            var winState = new WinState(this);

            Grid.SetColumn(winState, 0);
            Grid.SetRow(winState, 0);
            Grid.SetColumnSpan(winState, 3);
            Grid.SetRowSpan(winState, 4);

            mainGrid.Children.Add(winState);
            Animations.OpacityAnimation(winState, 0, 1, 1.5);
        }

        /// <summary>
        /// Удаление сообщения победы или поражения
        /// </summary>
        /// <param name="uIElement"></param>
        public void RemoveStateForm(UIElement uIElement)
        {
            mainGrid.Children.Remove(uIElement);
            uIElement = null;

            if(_Game != null)
                _Game.IsInterfaceLocked = false;
        }

        public void CheckContinue(UIElement uIElement)
        {
            if (_Game != null)
            {
                if (Game.GetGameState == Game.GameState.Started || Game.GetGameState == Game.GameState.Win)
                {
                    Animations.OpacityAnimation(uIElement, 1, 0, 0.3, this);
                    _Game.IsInterfaceLocked = false;
                }
            }
        }

        /// <summary>
        /// Рестарт игры при нажатии Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            GameRestart();
        }

        /// <summary>
        /// Изменение размера поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Game.G_ChangedSize = (int)slider.Value;
        }

        /// <summary>
        /// Сохранение результатов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ScoreBase.ScoresSave(_ScoreBase);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}