using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace _2048Game
{
    public class Cell
    {
        public bool IsFree = true;
        public Point startPoint;
        public Point endPoint;

        public Cell(Point start, Point end, bool isFree = true)
        {
            startPoint = start;
            endPoint = end;
            IsFree = isFree;
        }

        public Cell(bool isFree = true)
        {
            startPoint = new Point(0, 0);
            endPoint = new Point(0, 0);
            IsFree = isFree;
        }
    }

    public partial class Element : Border
    {
        public TextBlock textBlock;
        public int Value
        {
            get { return int.Parse(textBlock.Text); }
            set => textBlock.Text = value.ToString();
        }

        /// <summary>
        /// Текущий столбец
        /// </summary>
        public int row;

        /// <summary>
        /// Текущая строка
        /// </summary>
        public int column;

        public Element()
        {
            this.BorderBrush = Brushes.Black;
            this.BorderThickness = new Thickness(1);
            this.CornerRadius = new CornerRadius(12);

            textBlock = new TextBlock
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Effect = new DropShadowEffect
                {
                    BlurRadius = 30,
                    RenderingBias = RenderingBias.Quality,
                    ShadowDepth = 1
                },
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White
            };
            this.Child = textBlock;

            this.Value = new Random().Next(0, 11) != 10 ? 2 : 4;
            UpdateColor();
        }

        public void UpdateColor()
        {
            if(this.Value == 2)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            }
            else if(this.Value == 4 || this.Value == 8)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(255, 200, 125));
            }
            else if (this.Value == 16 || this.Value == 32)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(255, 150, 80));
            }
            else if (this.Value == 64 || this.Value == 128)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(255, 128, 70));
            }
            else if (this.Value == 256 || this.Value == 512)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(255, 128, 0));
            }
            else if (this.Value == 1024 || this.Value == 2048)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            }
            else if (this.Value == 4096 || this.Value == 8192)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else if (this.Value == 16384 || this.Value == 32768)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }
    }
}
