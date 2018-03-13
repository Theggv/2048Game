using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _2048Game
{
    /// <summary>
    /// Вспомогательный класс для хранения данных о клетке
    /// </summary>
    public class LogicalCell
    {
        private bool _IsFree = true; // Свободна ли клетка
        private Point _Coordinates; // координаты клетки

        public bool IsCellFree { get { return _IsFree; } set { _IsFree = value; } }

        public Point Coordinates { get { return _Coordinates; } set { _Coordinates = value; } }

        public LogicalCell(Point coord, bool isFree = true)
        {
            _Coordinates = coord;
            _IsFree = isFree;
        }
    }

    /// <summary>
    /// Класс для отрисовки плитки
    /// </summary>
    public partial class PhysicalCell : Border
    {
        private int _Row;
        private int _Column;
        private TextBlock _TextBlock;

        public int Row { get { return _Row; } set { _Row = value; } }
        public int Column { get { return _Column; } set { _Column = value; } }
        public int Value { get { return int.Parse(_TextBlock.Text); } set { _TextBlock.Text = value.ToString(); } }

        public PhysicalCell()
        {
            BorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            CornerRadius = new CornerRadius(5);

            _TextBlock = new TextBlock
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = 18,
                FontWeight = FontWeights.DemiBold,
                FontFamily = new FontFamily("Verdana"),
                VerticalAlignment = VerticalAlignment.Center,
            };
            Child = _TextBlock;

            Value = new Random().Next(0, 11) != 10 ? 2 : 4;
            UpdateColor();
        }

        /// <summary>
        /// Обновление цвета плитки
        /// </summary>
        public void UpdateColor()
        {
            if (Value <= 4)
                _TextBlock.Foreground = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            else
            {
                _TextBlock.Foreground = Brushes.White;
                if (Value >= 1024)
                    _TextBlock.FontSize = 16;
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
