using System.Windows;
using System.Windows.Controls;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для ScoreTable.xaml
    /// </summary>
    public partial class ScoreTable : UserControl
    {
        private MainMenu _MainMenu;

        public ScoreTable(MainMenu mainMenu)
        {
            InitializeComponent();

            _MainMenu = mainMenu;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Results.ItemsSource = MainWindow.ScoreBase.Scores;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Animations.OpacityAnimation(this, 1, 0, 0.3, _MainMenu);
        }
    }
}
