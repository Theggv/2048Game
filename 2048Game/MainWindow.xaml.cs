using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game _Game;
        private MainMenu _MainMenu;

        public MainWindow()
        {
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
            Game.gSize = Game.gChangedSize;
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
                        //RemoveStateForm(_MainMenu);
                    }
                    else
                    {
                        _MainMenu = new MainMenu(this);

                        Grid.SetColumnSpan(_MainMenu, 3);
                        Grid.SetRowSpan(_MainMenu, 4);

                        mainGrid.Children.Add(_MainMenu);
                        
                        Animations.OpacityAnimation(_MainMenu, 0, 1, 0.3);

                        _Game.IsInterfaceLocked = true;
                    }
                }
                else
                    _Game.Game_KeyDown(sender, e);
            }
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
                Animations.OpacityAnimation(uIElement, 1, 0, 0.3, this);
                _Game.IsInterfaceLocked = false;
            }
        }

        /// <summary>
        /// Рестарт игры при нажатии Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            GameStart();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Game.gChangedSize = (int)slider.Value;
        }
    }
}