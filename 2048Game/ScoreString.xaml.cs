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
    /// Логика взаимодействия для ScoreString.xaml
    /// </summary>
    public partial class ScoreString : UserControl
    {
        public new Thickness BorderThickness
        {
            get { return borderUserName.BorderThickness; }
            set
            {
                borderUserName.BorderThickness = value;
                borderScore.BorderThickness = value;
                mainBorderUN.Margin = new Thickness(0, 0, -value.Top / 2, 0);
                mainBorderScore.Margin = new Thickness(-value.Top / 2, 0, 0, 0);
            }
        }

        public ScoreString()
        {
            InitializeComponent();
        }
    }
}
