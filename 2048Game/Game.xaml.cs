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
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        enum GameState
        {
            Started = 0,
            Win,
            Lose
        }

        GameState gameState = GameState.Started;

        const int Size = 4;
        public Cell[,] cell = new Cell[Size, Size];
        public Element[,] element = new Element[Size, Size];

        public double CellWidth; // Длина Клетки
        public double CellHeight; // Ширина Клетки
        public double CellOffset = 4; // Сдвиг клетки относительно левого верхнего угла
        private static int numAnims = 0;

        public static int Score = 0;
        public static int PreviousScore = 0;

        MainWindow mainWindow;
        public bool IsInterfaceLocked = false;

        public Game(MainWindow window)
        {
            InitializeComponent();

            mainWindow = window;
            Score = 0;
            PreviousScore = 0;

            UpdateScore();
        }

        private void FieldCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            CellWidth = fieldCanvas.ActualWidth / Size;
            CellHeight = fieldCanvas.ActualHeight / Size;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    cell[i, j] = new Cell(new Point(CellWidth * j + CellOffset, CellHeight * i + CellOffset),
                        new Point(CellWidth * (j + 1) - CellOffset, CellHeight * (i + 1) - CellOffset));

                    var border = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                        CornerRadius = new CornerRadius(5),
                        Width = CellWidth - 2 * CellOffset,
                        Height = CellHeight - 2 * CellOffset
                    };
                    Canvas.SetLeft(border, CellWidth * j + CellOffset);
                    Canvas.SetTop(border, CellHeight * i + CellOffset);

                    fieldCanvas.Children.Add(border);
                    fieldCanvas.UpdateLayout();
                }
            }

            SpawnRandomElement();
            SpawnRandomElement();
        }

        private void GameRestart()
        {
            fieldCanvas = new Canvas
            {
                Background = new SolidColorBrush(Color.FromRgb(145, 145, 145))
            };

            Grid.SetRow(fieldCanvas, 1);
            Grid.SetColumnSpan(fieldCanvas, 2);

            fieldCanvas.Loaded += FieldCanvas_Loaded;
            fieldCanvas.UpdateLayout();
        }

        private Element SpawnElement(int x, int y)
        {
            var obj = new Element
            {
                Height = CellHeight - 2 * CellOffset,
                Width = CellWidth - 2 * CellOffset,
                row = x,
                column = y
            };

            Canvas.SetLeft(obj, CellWidth * y + CellOffset);
            Canvas.SetTop(obj, CellHeight * x + CellOffset);

            fieldCanvas.Children.Add(obj);
            fieldCanvas.UpdateLayout();

            new Animations(this).SetSpawnAnimation(obj);

            return obj;
        }

        public void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (numAnims != 0 || IsInterfaceLocked)
                return;

            switch (e.Key)
            {
                case Key.Enter:
                    GameRestart();
                    break;
                case Key.Left:
                    for (int i = 0; i < Size; i++)  // Элемент Столбца
                    {
                        int[] arrValues = new int[Size];
                        for (int j = 0; j < Size; j++)
                        {
                            if (cell[i, j].IsFree)
                                arrValues[j] = 0;
                            else
                                arrValues[j] = element[i, j].Value;
                        }
                        for (int k = 0; k < Size; k++)
                        {
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
                                    if (arrValues[FromCell] == arrValues[j])
                                    {
                                        MergeCell = j;
                                        break;
                                    }
                                    else if (arrValues[j] == 0)
                                        continue;
                                    else
                                        break;
                                }
                            }
                            #endregion

                            if (FromCell == MergeCell) // Если нет второго элемента
                            {
                                if (FromCell != FreeCell)
                                {
                                    MoveCell(ref element[i, FromCell], cell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref element[i, FromCell], ref element[i, MergeCell],
                                        cell[i, FreeCell].startPoint, Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref element[i, MergeCell], cell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Up:
                    for (int j = 0; j < Size; j++)  // Элемент Строки
                    {
                        int[] arrValues = new int[Size];
                        for (int i = 0; i < Size; i++)
                        {
                            if (cell[i, j].IsFree)
                                arrValues[i] = 0;
                            else
                                arrValues[i] = element[i, j].Value;
                        }
                        for (int k = 0; k < Size; k++)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int i = k; i < Size; i++)
                            {
                                if (!cell[i, j].IsFree)
                                {
                                    FromCell = i;
                                    break;
                                }
                            }
                            #endregion

                            // Если строка пустая, то идем дальше.
                            if (FromCell == -1)
                                break;

                            #region Поиск Первого нулевого элемента
                            int FreeCell = FromCell; // Первая свободная клетка
                            for (int i = FromCell - 1; i >= 0; i--)
                            {
                                if (cell[i, j].IsFree)
                                    FreeCell--;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int i = FromCell + 1; i < Size; i++)
                            {
                                if (!cell[i, j].IsFree)
                                {
                                    if (arrValues[FromCell] == arrValues[i])
                                    {
                                        MergeCell = i;
                                        break;
                                    }
                                    else if (arrValues[i] == 0)
                                        continue;
                                    else
                                        break;
                                }
                            }
                            #endregion

                            if (FromCell == MergeCell) // Если нет второго элемента
                            {
                                if (FromCell != FreeCell)
                                {
                                    MoveCell(ref element[FromCell, j], cell[FreeCell, j].startPoint,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref element[FromCell, j], ref element[MergeCell, j],
                                        cell[FreeCell, j].startPoint, Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref element[MergeCell, j], cell[FreeCell, j].startPoint,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Right:
                    for (int i = 0; i < Size; i++)  // Элемент Столбца
                    {
                        int[] arrValues = new int[Size];
                        for (int j = 0; j < Size; j++)
                        {
                            if (cell[i, j].IsFree)
                                arrValues[j] = 0;
                            else
                                arrValues[j] = element[i, j].Value;
                        }
                        for (int k = Size - 1; k >= 0; k--)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int j = k; j >= 0; j--)
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
                            for (int j = FromCell + 1; j < Size; j++)
                            {
                                if (cell[i, j].IsFree)
                                    FreeCell++;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int j = FromCell - 1; j >= 0; j--)
                            {
                                if (!cell[i, j].IsFree)
                                {
                                    if (arrValues[FromCell] == arrValues[j])
                                    {
                                        MergeCell = j;
                                        break;
                                    }
                                    else if (arrValues[j] == 0)
                                        continue;
                                    else
                                        break;
                                }
                            }
                            #endregion

                            if (FromCell == MergeCell) // Если нет второго элемента
                            {
                                if (FromCell != FreeCell)
                                {
                                    MoveCell(ref element[i, FromCell], cell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref element[i, FromCell], ref element[i, MergeCell],
                                        cell[i, FreeCell].startPoint, Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref element[i, MergeCell], cell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Down:
                    for (int j = 0; j < Size; j++)  // Элемент Строки
                    {
                        int[] arrValues = new int[Size];
                        for (int i = 0; i < Size; i++)
                        {
                            if (cell[i, j].IsFree)
                                arrValues[i] = 0;
                            else
                                arrValues[i] = element[i, j].Value;
                        }

                        for (int k = Size - 1; k >= 0; k--)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int i = k; i >= 0; i--)
                            {
                                if (!cell[i, j].IsFree)
                                {
                                    FromCell = i;
                                    break;
                                }
                            }
                            #endregion

                            // Если строка пустая, то идем дальше.
                            if (FromCell == -1)
                                break;

                            #region Поиск Первого нулевого элемента
                            int FreeCell = FromCell; // Первая свободная клетка
                            for (int i = FromCell + 1; i < Size; i++)
                            {
                                if (cell[i, j].IsFree)
                                    FreeCell++;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int i = FromCell - 1; i >= 0; i--)
                            {
                                if (!cell[i, j].IsFree)
                                {
                                    if (arrValues[FromCell] == arrValues[i])
                                    {
                                        MergeCell = i;
                                        break;
                                    }
                                    else if (arrValues[i] == 0)
                                        continue;
                                    else
                                        break;
                                }
                            }
                            #endregion

                            if (FromCell == MergeCell) // Если нет второго элемента
                            {
                                if (FromCell != FreeCell)
                                {
                                    MoveCell(ref element[FromCell, j], cell[FreeCell, j].startPoint,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref element[FromCell, j], ref element[MergeCell, j],
                                        cell[FreeCell, j].startPoint, Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref element[MergeCell, j], cell[FreeCell, j].startPoint,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public void MoveAndMergeCells(ref Element From, ref Element Dest, Point To, Animations.Direction d)
        {
            MoveCell(ref From, To, d);
            MoveCell(ref Dest, To, d, true);
        }

        public void MergeCells(ref Element Dest, Point To, Animations.Direction d)
        {
            MoveCell(ref Dest, To, d, true);
        }

        public void MoveCell(ref Element From, Point To, Animations.Direction d, bool IsMultiply = false)
        {
            numAnims++;
            var anim = new Animations(this);
            anim.SetMoveAnimation(From, To, d, IsMultiply);

            cell[From.row, From.column].IsFree = true;
            cell[(int)(To.Y / CellHeight), (int)(To.X / CellWidth)].IsFree = false;
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

            numAnims--;
            if (numAnims == 0)
            {
                SpawnRandomElement();
                UpdateScore();
            }
        }

        private void DeleteElement(ref Element element)
        {
            fieldCanvas.Children.Remove(element);
            fieldCanvas.UpdateLayout();
            element = null;
        }

        public void SpawnRandomElement()
        {
            Random rnd = new Random();
            while (true)
            {
                int x = rnd.Next(0, Size);
                int y = rnd.Next(0, Size);

                if (cell[y, x].IsFree)
                {
                    element[y, x] = SpawnElement(y, x);

                    cell[y, x].IsFree = false;
                    break;
                }
            }

            bool HasFree = false;
            foreach(var cell in cell)
            {
                if (cell.IsFree)
                {
                    HasFree = true;
                    break;
                }
            }
            if (!HasFree)
            {
                bool IsLose = true;
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size - 1; j++)
                    {
                        if (element[i, j].Value == element[i, j + 1].Value)
                            IsLose = false;
                    }
                }
                for (int i = 0; i < Size - 1; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (element[i, j].Value == element[i + 1, j].Value)
                            IsLose = false;
                    }
                }
                if (IsLose)
                {
                    IsInterfaceLocked = true;
                    gameState = GameState.Lose;
                    mainWindow.Lose();
                    return;
                }
            }

            if (gameState != GameState.Win)
            {
                foreach (var element in element)
                {
                    if (element != null && element.Value == 2048)
                    {
                        gameState = GameState.Win;
                        break;
                    }
                }
                if (gameState == GameState.Win)
                {
                    IsInterfaceLocked = true;
                    mainWindow.Win();
                }
            }
        }

        public void UpdateScore()
        {
            PreviousScore = Score;
            mainWindow.blockScore.Text = String.Format("Счёт: {0}", Score);
        }
    }
}
