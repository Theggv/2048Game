using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace _2048Game
{
    public class Animations
    {
        public enum Direction
        {
            Left = 0,
            Up,
            Right,
            Down
        }

        public int AnimationTimeMS = 200;

        private Element curTarget;
        private Point curDest;
        private Game mainWindow;
        private Storyboard storyboard;
        private bool isMultiplyValue;

        public Animations(Game window)
        {
            mainWindow = window;
        }

        public DoubleAnimation SetMoveAnimX(Element from, Point to)
        {
            return new DoubleAnimation
            {
                From = Canvas.GetLeft(from),
                To = to.X,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS),

            };
        }

        public DoubleAnimation SetMoveAnimY(Element from, Point to)
        {
            return new DoubleAnimation
            {
                From = Canvas.GetTop(from),
                To = to.Y,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS)
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
                    animation = SetMoveAnimX(Target, Destination);
                    Storyboard.SetTarget(animation, Target);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
                    break;
                case Direction.Up:
                    animation = SetMoveAnimY(Target, Destination);
                    Storyboard.SetTarget(animation, Target);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
                    break;
                case Direction.Right:
                    animation = SetMoveAnimX(Target, Destination);
                    Storyboard.SetTarget(animation, Target);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
                    break;
                case Direction.Down:
                    animation = SetMoveAnimY(Target, Destination);
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

            var animationX = new DoubleAnimation
            {
                From = 0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4)
            };
            var animationY = new DoubleAnimation
            {
                From = 0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4)
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);

            var offsetTranform = new TranslateTransform();
            Target.RenderTransform = offsetTranform;

            var offsetX = new DoubleAnimation
            {
                From = mainWindow.CellWidth / 2,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4)
            };
            var offsetY = new DoubleAnimation
            {
                From = mainWindow.CellHeight / 2,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4)
            };

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

            var animationX = new DoubleAnimation
            {
                From = 1.0,
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4),
                AutoReverse = true
            };
            var animationY = new DoubleAnimation
            {
                From = 1.0,
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4),
                AutoReverse = true
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);

            var offsetTranform = new TranslateTransform();
            Target.RenderTransform = offsetTranform;

            var offsetX = new DoubleAnimation
            {
                From = 0,
                To = -mainWindow.CellWidth * 0.1,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4),
                AutoReverse = true
            };
            var offsetY = new DoubleAnimation
            {
                From = 0,
                To = -mainWindow.CellHeight * 0.1,
                Duration = TimeSpan.FromMilliseconds(AnimationTimeMS / 4),
                AutoReverse = true
            };

            offsetTranform.BeginAnimation(TranslateTransform.XProperty, offsetX);
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

        public static void OpacityAnimation(UIElement Target)
        {
            var opacityAnim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1.5)
            };

            Target.BeginAnimation(UIElement.OpacityProperty, opacityAnim);
        }
    }
}
