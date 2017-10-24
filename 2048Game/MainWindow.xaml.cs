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
            Game.gSize = Game.gChangedSize;
            game = new Game(this);

            Grid.SetRow(game, 3);
            Grid.SetColumnSpan(game, 3);

            mainGrid.Children.Add(game);
            game.UpdateLayout();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (game != null)
                game.Game_KeyDown(sender, e);
        }

        public void Lose()
        {
            var loseState = new LoseState(this);

            Grid.SetColumn(loseState, 0);
            Grid.SetRow(loseState, 0);
            Grid.SetColumnSpan(loseState, 3);
            Grid.SetRowSpan(loseState, 4);

            mainGrid.Children.Add(loseState);
            Animations.OpacityAnimation(loseState);
        }

        public void Win()
        {
            var winState = new WinState(this);

            Grid.SetColumn(winState, 0);
            Grid.SetRow(winState, 0);
            Grid.SetColumnSpan(winState, 3);
            Grid.SetRowSpan(winState, 4);

            mainGrid.Children.Add(winState);
            Animations.OpacityAnimation(winState);
        }

        public void RemoveStateForm(UIElement uIElement)
        {
            mainGrid.Children.Remove(uIElement);
            uIElement = null;
            game.IsInterfaceLocked = false;
        }

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
