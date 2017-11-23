using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _2048Game
{
    /// <summary>
    /// Вспомогательный класс для хранения данных о клетке
    /// </summary>
    public class Cell
    {
        public bool IsFree = true; // Свободна ли клетка
        public Point coordinates; // координаты клетки

        public Cell(Point coord, bool isFree = true)
        {
            coordinates = coord;
            IsFree = isFree;
        }
    }

    /// <summary>
    /// Класс для обозначения элемента
    /// </summary>
    public partial class Element : Border
    {
        public TextBlock textBlock;
        public int Value // Значение элемента
        {
            get { return int.Parse(textBlock.Text); }
            set { textBlock.Text = value.ToString(); }
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
            BorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            CornerRadius = new CornerRadius(5);

            textBlock = new TextBlock
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = 18,
                FontWeight = FontWeights.DemiBold,
                FontFamily = new FontFamily("Verdana"),
                VerticalAlignment = VerticalAlignment.Center,
            };
            Child = textBlock;

            Value = new Random().Next(0, 11) != 10 ? 2 : 4;
            UpdateColor();
        }

        /// <summary>
        /// Обновление цвета элемента
        /// </summary>
        public void UpdateColor()
        {
            if (Value <= 4)
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            else
            {
                textBlock.Foreground = Brushes.White;
                if (Value >= 1024)
                    textBlock.FontSize = 16;
            }

            if (Value == 2)
                Background = new SolidColorBrush(Color.FromRgb(240, 230, 215));
            else if (Value == 4)
                Background = new SolidColorBrush(Color.FromRgb(235, 225, 200));
            else if (Value == 8)
                Background = new SolidColorBrush(Color.FromRgb(240, 170, 120));
            else if (Value == 16)
                Background = new SolidColorBrush(Color.FromRgb(250, 150, 100));
            else if (Value == 32)
                Background = new SolidColorBrush(Color.FromRgb(245, 124, 95));
            else if (Value == 64)
                Background = new SolidColorBrush(Color.FromRgb(245, 95, 60));
            else if (Value == 128)
                Background = new SolidColorBrush(Color.FromRgb(240, 205, 115));
            else if (Value == 256)
                Background = new SolidColorBrush(Color.FromRgb(235, 200, 100));
            else if (Value == 512)
                Background = new SolidColorBrush(Color.FromRgb(240, 200, 80));
            else if (Value == 1024)
                Background = new SolidColorBrush(Color.FromRgb(235, 195, 65));
            else if (Value == 2048)
                Background = new SolidColorBrush(Color.FromRgb(240, 195, 45));
            else if (Value >= 4096)
                Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }
    }
}
