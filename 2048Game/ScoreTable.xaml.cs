using System.Windows;
using System.Windows.Controls;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для ScoreTable.xaml
    /// </summary>
    public partial class ScoreTable : UserControl
    {
        private ScoreBase _ScoreBase;
        public ScoreTable(ScoreBase scoreBase)
        {
            _ScoreBase = scoreBase;
            InitializeComponent();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Results.ItemsSource = _ScoreBase.Scores;
        }
    }
}
