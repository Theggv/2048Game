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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для LoseState.xaml
    /// </summary>
    public partial class LoseState : UserControl
    {
        private MainWindow _MainWindow;
        private int _ScoreIndex;

        public LoseState(MainWindow window, int index)
        {
            InitializeComponent();

            _MainWindow = window;

            ScoreStr.BorderThickness = new Thickness(4);
            ScoreStr.curUserName.Text = MainWindow.ScoreBase[index].UserName;
            ScoreStr.curScore.Text = MainWindow.ScoreBase[index].Score.ToString();
            _ScoreIndex = index;
        }

        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            Animations.OpacityAnimation(this, 1, 0, 0.3, _MainWindow);
            _MainWindow.GameStart();
        }

        private void BackMenu_Click(object sender, RoutedEventArgs e)
        {
            Animations.OpacityAnimation(this, 1, 0, 0.3, _MainWindow);

            _MainWindow.ShowMainMenu();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.ScoreBase[_ScoreIndex].UserName = ScoreStr.curUserName.Text;
            MainWindow.ScoreBase[_ScoreIndex].Score = int.Parse(ScoreStr.curScore.Text);
        }
    }
}
