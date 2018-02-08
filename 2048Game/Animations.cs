using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace _2048Game
{
    public class Animations
    {
        public enum Direction // Направление
        {
            Left = 0,
            Up = 1,
            Right = 0,
            Down = 1
        }

        private Element curTarget; // Текущий объект
        private Point curDest; // Текущая точка перемещения
        private Game mainWindow; // Текущее окно
        private Storyboard storyboard; // Массив анимаций
        private bool isMultiplyValue; // Увеличивать ли значение элемента

        private int AnimationTimeMS = 200; // Время анимации

        public Animations(Game window)
        {
            mainWindow = window;
        }

        /// <summary>
        /// Анимация изменения прозрачности объекта
        /// </summary>
        /// <param name="Target">Объект</param>
        public static void OpacityAnimation(UIElement Target, double From, double To, double Sec)
        {
            var opacityAnim = SetDoubleAnimation(From, To, Sec * 1000);

            Target.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
        }

        public static void OpacityAnimation(UIElement Target, double From, double To, double Sec, MainWindow mainWindow)
        {
            var opacityAnim = SetDoubleAnimation(From, To, Sec * 1000);
            opacityAnim.Completed += (s, e) => mainWindow.RemoveStateForm(Target);

            Target.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
        }

        /// <summary>
        /// Анимация
        /// </summary>
        /// <param name="Target">Объект</param>
        /// <param name="From">Начальное Значение</param>
        /// <param name="To">Конечное Значение</param>
        /// <param name="Sec">Время действия</param>
        /// <param name="IsReverse">Реверсировать?</param>
        public static DoubleAnimation SetDoubleAnimation(double From, double To, double Millisec, bool IsReverse = false)
        {
            return new DoubleAnimation
            {
                From = From,
                To = To,
                Duration = TimeSpan.FromMilliseconds(Millisec),
                AutoReverse = IsReverse
            };
        }

        /// <summary>
        /// Анимация движения клетки
        /// </summary>
        /// <param name="Target">Объект</param>
        /// <param name="Destination">Точка назначения</param>
        /// <param name="direction">Направление</param>
        /// <param name="isMult">Удваивать значение клетки</param>
        public void SetMoveAnimation(Element Target, Point Destination, Direction direction, bool isMult = false)
        {
            curTarget = Target;
            curDest = Destination;
            isMultiplyValue = isMult;

            var animation = new DoubleAnimation();
            storyboard = new Storyboard();

            switch (direction)
            {
                case Direction.Left:
                    animation = SetDoubleAnimation(Canvas.GetLeft(Target),
                        Destination.X, AnimationTimeMS);

                    Storyboard.SetTarget(animation, Target);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
                    break;
                case Direction.Up:
                    animation = SetDoubleAnimation(Canvas.GetTop(Target),
                       Destination.Y, AnimationTimeMS);

                    Storyboard.SetTarget(animation, Target);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
                    break;
            }

            animation.Completed += new EventHandler(Animation_Completed);

            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        /// <summary>
        /// Анимация появления клетки
        /// </summary>
        /// <param name="Target">Объект</param>
        public void SetSpawnAnimation(Element Target)
        {
            var scaleTransform = new ScaleTransform();
            Target.LayoutTransform = scaleTransform;

            var animationX = SetDoubleAnimation(0, 1.0, AnimationTimeMS / 4);
            var animationY = SetDoubleAnimation(0, 1.0, AnimationTimeMS / 4);

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);

            var offsetTranform = new TranslateTransform();
            Target.RenderTransform = offsetTranform;

            var offsetX = SetDoubleAnimation(mainWindow.CellWidth / 2, 0, AnimationTimeMS / 4);
            var offsetY = SetDoubleAnimation(mainWindow.CellHeight / 2, 0, AnimationTimeMS / 4);

            offsetTranform.BeginAnimation(TranslateTransform.XProperty, offsetX);
            offsetTranform.BeginAnimation(TranslateTransform.YProperty, offsetY);
        }

        /// <summary>
        /// Анимация объединения клеток
        /// </summary>
        /// <param name="Target">Объект</param>
        public void SetMergeAnimation(Element Target)
        {
            var scaleTransform = new ScaleTransform();
            Target.LayoutTransform = scaleTransform;

            var animationX = SetDoubleAnimation(1.0, 1.2, AnimationTimeMS / 4, true);
            var animationY = SetDoubleAnimation(1.0, 1.2, AnimationTimeMS / 4, true);

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);

            var offsetTranform = new TranslateTransform();
            Target.RenderTransform = offsetTranform;

            var offsetX = SetDoubleAnimation(0, -mainWindow.CellWidth * 0.1, AnimationTimeMS / 4, true);
            var offsetY = SetDoubleAnimation(0, -mainWindow.CellWidth * 0.1, AnimationTimeMS / 4, true);

            offsetTranform.BeginAnimation(TranslateTransform.XProperty, offsetX);
            offsetTranform.BeginAnimation(TranslateTransform.YProperty, offsetY);
        }

        public static void SetScoreAnimation(TextBlock Target)
        {
            OpacityAnimation(Target, 1, 0, 1.5);

            Canvas.SetTop(Target, 25);

            var offsetTranform = new TranslateTransform();
            Target.RenderTransform = offsetTranform;
            
            var offsetY = SetDoubleAnimation(0, -20, 1500);
            
            offsetTranform.BeginAnimation(TranslateTransform.YProperty, offsetY);
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            if (isMultiplyValue)
            {
                curTarget.Value *= 2;
                curTarget.UpdateColor();
                Game.Score += curTarget.Value;

                SetMergeAnimation(curTarget);
            }
            mainWindow.UpdateInfo(curTarget.row, curTarget.column,
                (int)(curDest.Y / mainWindow.CellHeight), (int)(curDest.X / mainWindow.CellWidth));
        }
    }
}
