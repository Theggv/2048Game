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
        Game game;

        public MainWindow()
        {
            InitializeComponent();

            GameStart();
        }

        public void GameStart()
        {
            game = new Game(this);

            Grid.SetRow(game, 1);
            Grid.SetColumnSpan(game, 2);

            mainGrid.Children.Add(game);
            game.UpdateLayout();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GameStart();
                return;
            }
            if (game != null)
                game.Game_KeyDown(sender, e);
        }

        public void Lose()
        {
            var loseState = new LoseState(this);

            Grid.SetColumn(loseState, 0);
            Grid.SetRow(loseState, 0);
            Grid.SetColumnSpan(loseState, 2);
            Grid.SetRowSpan(loseState, 2);

            mainGrid.Children.Add(loseState);
            Animations.OpacityAnimation(loseState);
        }

        public void Win()
        {
            var winState = new WinState(this);

            Grid.SetColumn(winState, 0);
            Grid.SetRow(winState, 0);
            Grid.SetColumnSpan(winState, 2);
            Grid.SetRowSpan(winState, 2);

            mainGrid.Children.Add(winState);
            Animations.OpacityAnimation(winState);
        }

        public void RemoveStateForm(UIElement uIElement)
        {
            mainGrid.Children.Remove(uIElement);
            uIElement = null;
            game.IsInterfaceLocked = false;
        }
    }
}
