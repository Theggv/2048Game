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
    public static class Animations
    {
        public enum Direction
        {
            Left = 0,
            Up,
            Right,
            Down
        }

        private static Element curTarget;
        private static Point curDest;
        private static MainWindow mainWindow;
        public static Storyboard storyboard = new Storyboard();

        public static void GetWindow(MainWindow window)
        {
            mainWindow = window;
        }

        public static DoubleAnimation SetMoveAnimX(Element from, Point to)
        {
            return new DoubleAnimation
            {
                From = Canvas.GetLeft(from),
                To = to.X,
                Duration = TimeSpan.FromMilliseconds(300),

            };
        }

        public static DoubleAnimation SetMoveAnimY(Element from, Point to)
        {
            return new DoubleAnimation
            {
                From = Canvas.GetTop(from),
                To = to.Y,
                Duration = TimeSpan.FromMilliseconds(300)
            };
        }

        public static void SetMoveAnimation(ref Element Target, Point Destination, Direction direction)
        {
            curTarget = Target;
            curDest = Destination;

            var animation = new DoubleAnimation();

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
            //storyboard.Completed += new EventHandler(Animation_Completed);
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        public static void StartAnimation()
        {
            storyboard.Begin();
        }

        private static void Animation_Completed(object sender, EventArgs e)
        {
            mainWindow.UpdateInfo(curTarget.row, curTarget.column,
                (int)(curDest.Y / mainWindow.CellHeigth), (int)(curDest.X / mainWindow.CellWidth));
        }
    }
}
