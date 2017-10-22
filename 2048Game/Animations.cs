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
        private MainWindow mainWindow;
        private Storyboard storyboard;
        private bool isMultiplyValue;

        public Animations(MainWindow window)
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

        private void Animation_Completed(object sender, EventArgs e)
        {
            if (isMultiplyValue)
            {
                curTarget.Value *= 2;
                curTarget.UpdateColor();
                MainWindow.Score += curTarget.Value;
            }
            mainWindow.UpdateInfo(curTarget.row, curTarget.column,
                (int)(curDest.Y / mainWindow.CellHeigth), (int)(curDest.X / mainWindow.CellWidth));
        }
    }
}
