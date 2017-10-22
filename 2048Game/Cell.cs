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

    public class Element : Border
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
            this.Background = new SolidColorBrush(Color.FromRgb(255, 241, 159));
            this.BorderBrush = Brushes.Black;
            this.BorderThickness = new Thickness(1);
            this.CornerRadius = new CornerRadius(10);

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
                VerticalAlignment = VerticalAlignment.Center
            };
            this.Child = textBlock;
            this.Value = 2;
        }
    }
}
