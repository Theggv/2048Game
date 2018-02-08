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
    /// Логика взаимодействия для WinState.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        MainWindow mainWindow;

        public MainMenu(MainWindow window)
        {
            InitializeComponent();

            mainWindow = window;
        }

        private void buttonNewGame_Click(object sender, RoutedEventArgs e)
        {
            //mainWindow.RemoveStateForm(this);

            Animations.OpacityAnimation(this, 1, 0, 0.3, mainWindow);
            mainWindow.GameStart();
        }

        private void buttonContinue_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.CheckContinue(this);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
