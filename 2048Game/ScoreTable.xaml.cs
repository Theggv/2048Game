using System.Windows;
using System.Windows.Media;
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

        private void Results_Loaded(object sender, RoutedEventArgs e)
        {
            Header.curUserName.Text = "Имя";
            Header.curUserName.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Header.curUserName.IsReadOnly = true;
            Header.curUserName.Focusable = false;
            Header.curScore.Text = "Счёт";
            Header.curScore.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Header.BorderThickness = new Thickness(4);

            foreach (var elem in MainWindow.ScoreBase.Scores)
            {
                ScoreString scoreString = new ScoreString
                {
                    Height = 30
                };
                scoreString.curUserName.IsReadOnly = true;
                scoreString.curUserName.Text = elem.UserName;
                scoreString.curUserName.FontSize = 12;
                scoreString.curUserName.Focusable = false;
                scoreString.curScore.Text = elem.Score.ToString();
                scoreString.curScore.FontSize = 12;

                Results.Items.Add(scoreString);
                Results.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                Results.VerticalContentAlignment = VerticalAlignment.Stretch;
                Results.Focusable = false;
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Animations.OpacityAnimation(this, 1, 0, 0.3, _MainMenu);
        }
    }
}
