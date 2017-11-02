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

        public static int gSize = 4; // Текущий размер поля
        public static int gChangedSize = 4; // Текущий размер поля

        public Cell[,] gCell = new Cell[gSize, gSize]; // Игровое поле
        public Element[,] gElement = new Element[gSize, gSize]; // Клетки

        public double CellWidth; // Длина Клетки
        public double CellHeight; // Ширина Клетки
        public double CellOffset = 4; // Сдвиг клетки относительно левого верхнего угла
        private static int numAnims = 0; // Количество проигрывающихся анимаций в данный момент

        public static int Score = 0; // Счёт
        public static int PreviousScore = 0; // Счёт, используемый для анимации
        public static int BestScore = 0; // Лучший счёт

        MainWindow mainWindow; // Главное окно
        public bool IsInterfaceLocked = false; // Блокировка интерфейса

        public Game(MainWindow window)
        {
            InitializeComponent();

            mainWindow = window;
            mainWindow.Width = 55 * gSize + 10;
            mainWindow.Height = 55 * gSize + 90;
            mainWindow.UpdateLayout();

            Score = 0;
            PreviousScore = 0;

            UpdateScore();
        }

        private void FieldCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(mainWindow.curScore, mainWindow.scoreCanvas.ActualWidth * 0.3);
            Canvas.SetLeft(mainWindow.bestScore, mainWindow.scoreCanvas.ActualWidth * 0.8);
            mainWindow.UpdateLayout();

            CellWidth = fieldCanvas.ActualWidth / gSize;
            CellHeight = fieldCanvas.ActualHeight / gSize;

            for (int i = 0; i < gSize; i++)
            {
                for (int j = 0; j < gSize; j++)
                {
                    gCell[i, j] = new Cell(new Point(CellWidth * j + CellOffset, CellHeight * i + CellOffset));

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
            Grid.SetColumnSpan(fieldCanvas, 3);

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

        /// <summary>
        /// Обработка хода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    for (int i = 0; i < gSize; i++)  // Элемент Столбца
                    {
                        int[] arrValues = new int[gSize];
                        for (int j = 0; j < gSize; j++)
                        {
                            if (gCell[i, j].IsFree)
                                arrValues[j] = 0;
                            else
                                arrValues[j] = gElement[i, j].Value;
                        }
                        for (int k = 0; k < gSize; k++)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int j = k; j < gSize; j++)
                            {
                                if (!gCell[i, j].IsFree)
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
                                if (gCell[i, j].IsFree)
                                    FreeCell--;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int j = FromCell + 1; j < gSize; j++)
                            {
                                if (!gCell[i, j].IsFree)
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
                                    MoveCell(ref gElement[i, FromCell], gCell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref gElement[i, FromCell], ref gElement[i, MergeCell],
                                        gCell[i, FreeCell].startPoint, Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref gElement[i, MergeCell], gCell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Up:
                    for (int j = 0; j < gSize; j++)  // Элемент Строки
                    {
                        int[] arrValues = new int[gSize];
                        for (int i = 0; i < gSize; i++)
                        {
                            if (gCell[i, j].IsFree)
                                arrValues[i] = 0;
                            else
                                arrValues[i] = gElement[i, j].Value;
                        }
                        for (int k = 0; k < gSize; k++)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int i = k; i < gSize; i++)
                            {
                                if (!gCell[i, j].IsFree)
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
                                if (gCell[i, j].IsFree)
                                    FreeCell--;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int i = FromCell + 1; i < gSize; i++)
                            {
                                if (!gCell[i, j].IsFree)
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
                                    MoveCell(ref gElement[FromCell, j], gCell[FreeCell, j].startPoint,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref gElement[FromCell, j], ref gElement[MergeCell, j],
                                        gCell[FreeCell, j].startPoint, Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref gElement[MergeCell, j], gCell[FreeCell, j].startPoint,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Right:
                    for (int i = 0; i < gSize; i++)  // Элемент Столбца
                    {
                        int[] arrValues = new int[gSize];
                        for (int j = 0; j < gSize; j++)
                        {
                            if (gCell[i, j].IsFree)
                                arrValues[j] = 0;
                            else
                                arrValues[j] = gElement[i, j].Value;
                        }
                        for (int k = gSize - 1; k >= 0; k--)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int j = k; j >= 0; j--)
                            {
                                if (!gCell[i, j].IsFree)
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
                            for (int j = FromCell + 1; j < gSize; j++)
                            {
                                if (gCell[i, j].IsFree)
                                    FreeCell++;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int j = FromCell - 1; j >= 0; j--)
                            {
                                if (!gCell[i, j].IsFree)
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
                                    MoveCell(ref gElement[i, FromCell], gCell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref gElement[i, FromCell], ref gElement[i, MergeCell],
                                        gCell[i, FreeCell].startPoint, Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref gElement[i, MergeCell], gCell[i, FreeCell].startPoint,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Down:
                    for (int j = 0; j < gSize; j++)  // Элемент Строки
                    {
                        int[] arrValues = new int[gSize];
                        for (int i = 0; i < gSize; i++)
                        {
                            if (gCell[i, j].IsFree)
                                arrValues[i] = 0;
                            else
                                arrValues[i] = gElement[i, j].Value;
                        }

                        for (int k = gSize - 1; k >= 0; k--)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int i = k; i >= 0; i--)
                            {
                                if (!gCell[i, j].IsFree)
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
                            for (int i = FromCell + 1; i < gSize; i++)
                            {
                                if (gCell[i, j].IsFree)
                                    FreeCell++;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int i = FromCell - 1; i >= 0; i--)
                            {
                                if (!gCell[i, j].IsFree)
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
                                    MoveCell(ref gElement[FromCell, j], gCell[FreeCell, j].startPoint,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref gElement[FromCell, j], ref gElement[MergeCell, j],
                                        gCell[FreeCell, j].startPoint, Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать одни элемент
                                {
                                    MergeCells(ref gElement[MergeCell, j], gCell[FreeCell, j].startPoint,
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

            gCell[From.row, From.column].IsFree = true;
            gCell[(int)(To.Y / CellHeight), (int)(To.X / CellWidth)].IsFree = false;
        }


        /// <summary>
        /// Одновление информации после хода
        /// </summary>
        /// <param name="xFrom">Столбец объекта</param>
        /// <param name="yFrom">Строка объекта</param>
        /// <param name="xTo">Столбец точки перемещения</param>
        /// <param name="yTo">Строка точки перемещения</param>
        public void UpdateInfo(int xFrom, int yFrom, int xTo, int yTo)
        {
            DeleteElement(ref gElement[xTo, yTo]);
            gElement[xTo, yTo] = gElement[xFrom, yFrom];
            fieldCanvas.UpdateLayout();

            DeleteElement(ref gElement[xFrom, yFrom]);

            fieldCanvas.Children.Remove(gElement[xTo, yTo]);
            fieldCanvas.Children.Add(gElement[xTo, yTo]);

            gElement[xTo, yTo].Visibility = Visibility.Visible;
            gElement[xTo, yTo].row = xTo;
            gElement[xTo, yTo].column = yTo;
            fieldCanvas.UpdateLayout();

            numAnims--;
            if (numAnims == 0)
            {
                SpawnRandomElement();
                UpdateScore();
            }
        }

        /// <summary>
        /// Удаление клетки
        /// </summary>
        /// <param name="element">Объект</param>
        private void DeleteElement(ref Element element)
        {
            fieldCanvas.Children.Remove(element);
            fieldCanvas.UpdateLayout();
            element = null;
        }

        /// <summary>
        /// Спавн клетки в случайном месте
        /// </summary>
        public void SpawnRandomElement()
        {
            Random rnd = new Random();
            while (true)
            {
                int x = rnd.Next(0, gSize);
                int y = rnd.Next(0, gSize);

                if (gCell[y, x].IsFree)
                {
                    gElement[y, x] = SpawnElement(y, x);

                    gCell[y, x].IsFree = false;
                    break;
                }
            }

            CheckGameStatus();
        }

        /// <summary>
        /// Проверка игры на победу / поражение.
        /// </summary>
        public void CheckGameStatus()
        {
            bool HasFree = false;
            foreach (var cell in gCell)
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
                for (int i = 0; i < gSize; i++)
                {
                    for (int j = 0; j < gSize - 1; j++)
                    {
                        if (gElement[i, j].Value == gElement[i, j + 1].Value)
                            IsLose = false;
                    }
                }
                for (int i = 0; i < gSize - 1; i++)
                {
                    for (int j = 0; j < gSize; j++)
                    {
                        if (gElement[i, j].Value == gElement[i + 1, j].Value)
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
                foreach (var element in gElement)
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

        /// <summary>
        /// Обновление очков
        /// </summary>
        public void UpdateScore()
        {
            if(Score > PreviousScore)
            {
                mainWindow.curScore.Text = "+" + (Score - PreviousScore).ToString();
                Animations.SetScoreAnimation(mainWindow.curScore);
            }
            PreviousScore = Score;

            if (Score > BestScore)
            {
                mainWindow.bestScore.Text = "+" + (Score - BestScore).ToString();
                Animations.SetScoreAnimation(mainWindow.bestScore);

                BestScore = Score;
            }

            mainWindow.blockScore.Text = Score.ToString();
            mainWindow.blockBestScore.Text = BestScore.ToString();
        }
    }
}
