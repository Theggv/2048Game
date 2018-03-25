using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для WinState.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private MainWindow _MainWindow;
        private ScoreTable _ScoreTable;

        public MainMenu(MainWindow window)
        {
            InitializeComponent();

            _MainWindow = window;
        }

        private void buttonNewGame_Click(object sender, RoutedEventArgs e)
        {
            if (Game.Score > 0)
            {
                MainWindow.ScoreBase.AddScore(new UserInfo("Player", Game.Score));
            }

            Animations.OpacityAnimation(this, 1, 0, 0.3, _MainWindow);
            _MainWindow.GameStart();
        }

        private void buttonContinue_Click(object sender, RoutedEventArgs e)
        {
            _MainWindow.CheckContinue(this);
        }

        private void BestScores_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.ScoreBase.IsHasScores())
            {
                _ScoreTable = new ScoreTable(this);

                Grid.SetColumnSpan(_ScoreTable, 3);
                Grid.SetRowSpan(_ScoreTable, 6);

                MenuGrid.Children.Add(_ScoreTable);

                Animations.OpacityAnimation(_ScoreTable, 0, 1, 0.3);
            }
        }

        public void RemoveStateForm(UIElement uIElement)
        {
            MenuGrid.Children.Remove(uIElement);
            uIElement = null;
        }

        private void buttonContinue_Loaded(object sender, RoutedEventArgs e)
        {
            if (Game.GetGameState == Game.GameState.Not_Started || Game.GetGameState == Game.GameState.Lose)
            {
                buttonContinue.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
            }
            else
            {
                buttonContinue.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 250, 238));
            }
        }

        private void BestScores_Loaded(object sender, System.EventArgs e)
        {
            if (MainWindow.ScoreBase.IsHasScores())
            {
                BestScores.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 250, 238));
            }
            else
            {
                BestScores.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
            }
        }
    }
}
