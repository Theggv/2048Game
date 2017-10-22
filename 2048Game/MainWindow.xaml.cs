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
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int Size = 4;
        public Cell[,] cell = new Cell[Size, Size];
        public Element[,] element = new Element[Size, Size];

        public double CellWidth;
        public double CellHeigth;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FieldCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            CellWidth = fieldCanvas.ActualWidth / Size;
            CellHeigth = fieldCanvas.ActualHeight / Size;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    cell[i, j] = new Cell(new Point((CellWidth + 1) * j, (CellHeigth + 1) * i),
                        new Point((CellWidth + 1) * (j + 1), (CellHeigth + 1) * (i + 1)));
                }
            }

            Random rnd = new Random();
            for (int i = 0; i < 2; i++)
            {
                int x = rnd.Next(0, 4);
                int y = rnd.Next(0, 4);
                y = 2;

                if (cell[y, x].IsFree)
                {
                    element[y, x] = SpawnElement(y, x);

                    cell[y, x].IsFree = false;
                }
                else
                    i--;
            }
        }

        private Element SpawnElement(int x, int y)
        {
            var obj = new Element
            {
                Height = CellHeigth - 1,
                Width = CellWidth - 1,
                row = x,
                column = y,
                Name = "cell" + x.ToString() + y.ToString()
            };

            Canvas.SetLeft(obj, (CellWidth + 1) * y);
            Canvas.SetTop(obj, (CellHeigth + 1) * x);

            fieldCanvas.Children.Add(obj);
            fieldCanvas.UpdateLayout();

            return obj;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    for (int i = 0; i < Size; i++)  // Элемент Столбца
                    {
                        Element[] tempArray = new Element[Size];
                        for(int j = 0; j < Size; j++)
                        {
                            tempArray[j] = element[i, j];
                        }

                        for (int k = 0; k < Size; k++)
                        {
                            int[] arrValues = new int[Size];

                            #region Запись значений строки в массив
                            for (int j = k; j < Size; j++)
                            {
                                if (cell[i, j].IsFree)
                                    arrValues[j] = 0;
                                else
                                    arrValues[j] = tempArray[j].Value;
                            }
                            #endregion

                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int j = k; j < Size; j++)
                            {
                                if (!cell[i, j].IsFree)
                                {
                                    FromCell = j;
                                    break;
                                }
                            }
                            #endregion

                            // Если строка пустая, то идем дальше.
                            if (FromCell == -1)
                                break;

                            #region Поиск Первого нулевого элемента
                            int FreeCell = FromCell; // Первая свободная клетка
                            for (int j = FromCell - 1; j >= 0; j--)
                            {
                                if (cell[i, j].IsFree)
                                    FreeCell--;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int j = FromCell + 1; j < Size; j++)
                            {
                                if (!cell[i, j].IsFree)
                                {
                                    if (tempArray[FromCell].Value == tempArray[j].Value)
                                    {
                                        MergeCell = j;
                                        break;
                                    }
                                }
                            }
                            #endregion

                            if (FromCell == MergeCell) // Если нет второго элемента
                            {
                                if (FromCell != FreeCell)
                                {
                                    MoveCell(element[i, FromCell], cell[i, FreeCell].startPoint);
                                    tempArray[FreeCell] = tempArray[FromCell];
                                    tempArray[FromCell] = null;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(element[i, FromCell], element[i, MergeCell],
                                        cell[i, FreeCell].startPoint);
                                    tempArray[FromCell].Value *= 2;
                                    tempArray[FreeCell] = tempArray[FromCell];
                                    tempArray[FromCell] = null;
                                    tempArray[MergeCell] = null;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(element[i, MergeCell], cell[i, FreeCell].startPoint);
                                    tempArray[FromCell].Value *= 2;
                                    tempArray[FreeCell] = tempArray[FromCell];
                                    tempArray[MergeCell] = null;
                                }
                            }
                        }
                    }
                    break;
                case Key.Up:
                    for (int j = 0; j < Size; j++) // Элемент Строки
                    {
                        for (int i = 0; i < Size - 1; i++) // Элемент Столбца
                        {
                            for (int k = i + 1; k < Size; k++)
                            {
                                if (cell[i, j].IsFree)
                                {
                                    if (!cell[k, j].IsFree)
                                    {
                                        Animations.GetWindow(this);
                                        Animations.SetMoveAnimation(ref element[k, j],
                                            cell[i, j].startPoint, Animations.Direction.Up);
                                        UpdateCells(k, j, i--, j);
                                        break;
                                    }
                                }
                                else
                                {
                                    if (cell[k, j].IsFree)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (element[i, j].Value != element[k, j].Value)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            element[k, j].Value *= 2;
                                            Animations.GetWindow(this);
                                            Animations.SetMoveAnimation(ref element[k, j],
                                                cell[i, j].startPoint, Animations.Direction.Up);
                                            UpdateCells(k, j, i, j);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Key.Right:
                    for (int i = 0; i < Size; i++) // Элемент Столбца
                    {
                        for (int j = Size - 1; j > 0; j--) // Элемент Строки
                        {
                            for (int k = j - 1; k >= 0; k--)
                            {
                                if (cell[i, j].IsFree)
                                {
                                    if (!cell[i, k].IsFree)
                                    {
                                        Animations.GetWindow(this);
                                        Animations.SetMoveAnimation(ref element[k, j],
                                            cell[i, j].startPoint, Animations.Direction.Up);
                                        UpdateCells(i, k, i, j++);
                                        break;
                                    }
                                }
                                else
                                {
                                    if (cell[i, k].IsFree)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (element[i, j].Value != element[i, k].Value)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Animations.GetWindow(this);
                                            Animations.SetMoveAnimation(ref element[i, k],
                                                cell[i, j].startPoint, Animations.Direction.Up);
                                            element[i, k].Value *= 2;
                                            UpdateCells(i, k, i, j);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Key.Down:
                    for (int j = 0; j < Size; j++) // Элемент Строки
                    {
                        for (int i = Size - 1; i > 0; i--) // Элемент Столбца
                        {
                            for (int k = i - 1; k >= 0; k--)
                            {
                                if (cell[i, j].IsFree)
                                {
                                    if (!cell[k, j].IsFree)
                                    {
                                        Animations.GetWindow(this);
                                        Animations.SetMoveAnimation(ref element[k, j],
                                            cell[i, j].startPoint, Animations.Direction.Up);
                                        UpdateCells(k, j, i++, j);
                                        break;
                                    }
                                }
                                else
                                {
                                    if (cell[k, j].IsFree)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (element[i, j].Value != element[k, j].Value)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Animations.GetWindow(this);
                                            Animations.SetMoveAnimation(ref element[k, j],
                                                cell[i, j].startPoint, Animations.Direction.Up);
                                            element[k, j].Value *= 2;
                                            UpdateCells(k, j, i, j);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            //Animations.StartAnimation();
        }

        public void MoveAndMergeCells(Element From, Element Dest, Point To)
        {
            Dest.Value *= 2;
            MoveCell(From, To);
            MoveCell(Dest, To);
        }

        public void MergeCells(Element Dest, Point To)
        {
            Dest.Value *= 2;
            MoveCell(Dest, To);
        }

        public void MoveCell(Element From, Point To)
        {
            Animations.GetWindow(this);
            Animations.SetMoveAnimation(ref From, To, Animations.Direction.Up);

            cell[From.row, From.column].IsFree = true;
            cell[(int)(To.Y / CellHeigth), (int)(To.X / CellWidth)].IsFree = false;
        }

        public void UpdateCells(int xFrom, int yFrom, int xTo, int yTo)
        {
            element[xTo, yTo] = element[xFrom, yFrom];
            element[xTo, yTo].Visibility = Visibility.Hidden;

            Random rnd = new Random();
            while (true)
            {
                bool HasFree = false;
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (cell[i, j].IsFree)
                        {
                            HasFree = true;
                            break;
                        }
                    }
                    if (HasFree)
                        break;
                }
                if (HasFree)
                {
                    int x = rnd.Next(0, 4);
                    int y = rnd.Next(0, 4);

                    if (cell[x, y].IsFree)
                    {
                        element[x, y] = SpawnElement(x, y);

                        cell[x, y].IsFree = false;
                        break;
                    }
                }
                else
                    break;
            }
        }

        public void UpdateInfo(int xFrom, int yFrom, int xTo, int yTo)
        {
            DeleteElement(ref element[xTo, yTo]);
            element[xTo, yTo] = element[xFrom, yFrom];
            fieldCanvas.UpdateLayout();

            DeleteElement(ref element[xFrom, yFrom]);

            fieldCanvas.Children.Remove(element[xTo, yTo]);
            fieldCanvas.Children.Add(element[xTo, yTo]);
            element[xTo, yTo].Visibility = Visibility.Visible;
            element[xTo, yTo].row = xTo;
            element[xTo, yTo].column = yTo;
            fieldCanvas.UpdateLayout();
        }

        private void DeleteElement(ref Element element)
        {
            fieldCanvas.Children.Remove(element);
            fieldCanvas.UpdateLayout();
            element = null;
        }
    }
}
