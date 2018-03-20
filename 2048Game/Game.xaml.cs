using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _2048Game
{
    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        public enum GameState // Состояние игры
        {
            Not_Started = 0,
            Started,
            Win,
            Lose
        }

        private static GameState _GameState = GameState.Not_Started;
        private MainWindow mainWindow; // Главное окно

        private static int _FieldSize = 4;
        private static int _FieldChangedSize = _FieldSize;

        private const double CELL_SIZE = 60; // Размер клетки
        private double _CellWidth; // Длина Клетки
        private double _CellHeight; // Ширина Клетки
        private double _CellOffset = 4; // Сдвиг клетки относительно левого верхнего угла
        private static int _NumAnims = 0; // Количество проигрывающихся анимаций в данный момент

        private static int _Score = 0; // Счёт
        private static int _PreviousScore = 0; // Счёт, используемый для анимации
        private static int _BestScore = MainWindow.ScoreBase.GetBestScore(); // Лучший счёт

        private bool _IsInterfaceLocked = false; // Блокировка интерфейса

        private LogicalCell[,] _LogCell = new LogicalCell[_FieldSize, _FieldSize]; // Игровое поле
        private PhysicalCell[,] _PhyCell = new PhysicalCell[_FieldSize, _FieldSize]; // Клетки

        public static int G_Size { get { return _FieldSize; } set { _FieldSize = value; } }
        public static int G_ChangedSize { get { return _FieldChangedSize; } set { _FieldChangedSize = value; } }
        public double CellWidth { get { return _CellWidth; } }
        public double CellHeight { get { return _CellHeight; } }
        public static int Score { get { return _Score; } set { _Score = value; } }
        public static GameState GetGameState { get { return _GameState; } }
        public bool IsInterfaceLocked { get { return _IsInterfaceLocked; } set { _IsInterfaceLocked = value; } }

        public Game(MainWindow window)
        {
            InitializeComponent();

            // Задание размеров окна
            mainWindow = window;
            mainWindow.Width = CELL_SIZE * _FieldSize + 10;
            mainWindow.Height = CELL_SIZE * _FieldSize + 90;
            mainWindow.UpdateLayout();

            // Обнуление очков
            _Score = 0;
            _PreviousScore = 0;

            UpdateScore();
        }

        // Инициализация поля
        private void FieldCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _GameState = GameState.Started;

            Canvas.SetLeft(mainWindow.curScore, mainWindow.scoreCanvas.ActualWidth * 0.3);
            Canvas.SetLeft(mainWindow.bestScore, mainWindow.scoreCanvas.ActualWidth * 0.8);
            mainWindow.UpdateLayout();

            // Задание размеров ячейки
            _CellWidth = fieldCanvas.ActualWidth / _FieldSize;
            _CellHeight = fieldCanvas.ActualHeight / _FieldSize;

            // Отрисовка визуального поля
            for (int i = 0; i < _FieldSize; i++)
            {
                for (int j = 0; j < _FieldSize; j++)
                {
                    _LogCell[i, j] = new LogicalCell(new Point(_CellWidth * j + _CellOffset, _CellHeight * i + _CellOffset));

                    var border = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                        CornerRadius = new CornerRadius(5),
                        Width = _CellWidth - 2 * _CellOffset,
                        Height = _CellHeight - 2 * _CellOffset
                    };
                    Canvas.SetLeft(border, _CellWidth * j + _CellOffset);
                    Canvas.SetTop(border, _CellHeight * i + _CellOffset);

                    fieldCanvas.Children.Add(border);
                    fieldCanvas.UpdateLayout();
                }
            }
            
            // Спавним 2 элемента на поле
            SpawnRandomElement();
            SpawnRandomElement();
        }
    
        /// <summary>
        /// Спавн элемента по заданным координатам
        /// </summary>
        /// <param name="x">Столбец</param>
        /// <param name="y">Строка</param>
        /// <returns>Элемент</returns>
        private PhysicalCell SpawnElement(int x, int y)
        {
            var obj = new PhysicalCell
            {
                Height = _CellHeight - 2 * _CellOffset,
                Width = _CellWidth - 2 * _CellOffset,
                Row = x,
                Column = y
            };

            Canvas.SetLeft(obj, _CellWidth * y + _CellOffset);
            Canvas.SetTop(obj, _CellHeight * x + _CellOffset);

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
            if (_NumAnims != 0 || _IsInterfaceLocked)
                return;

            switch (e.Key)
            {
                case Key.Enter:
                    mainWindow.GameRestart();
                    break;
                case Key.Left:
                    for (int i = 0; i < _FieldSize; i++)  // Элемент Столбца
                    {
                        int[] arrValues = new int[_FieldSize];
                        for (int j = 0; j < _FieldSize; j++)
                        {
                            if (_LogCell[i, j].IsCellFree)
                                arrValues[j] = 0;
                            else
                                arrValues[j] = _PhyCell[i, j].Value;
                        }
                        for (int k = 0; k < _FieldSize; k++)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int j = k; j < _FieldSize; j++)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                                if (_LogCell[i, j].IsCellFree)
                                    FreeCell--;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int j = FromCell + 1; j < _FieldSize; j++)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                                    MoveCell(ref _PhyCell[i, FromCell], _LogCell[i, FreeCell].Coordinates,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref _PhyCell[i, FromCell], ref _PhyCell[i, MergeCell],
                                        _LogCell[i, FreeCell].Coordinates, Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать один элемент
                                {
                                    MergeCells(ref _PhyCell[i, MergeCell], _LogCell[i, FreeCell].Coordinates,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Up:
                    for (int j = 0; j < _FieldSize; j++)  // Элемент Строки
                    {
                        int[] arrValues = new int[_FieldSize];
                        for (int i = 0; i < _FieldSize; i++)
                        {
                            if (_LogCell[i, j].IsCellFree)
                                arrValues[i] = 0;
                            else
                                arrValues[i] = _PhyCell[i, j].Value;
                        }
                        for (int k = 0; k < _FieldSize; k++)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int i = k; i < _FieldSize; i++)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                                if (_LogCell[i, j].IsCellFree)
                                    FreeCell--;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int i = FromCell + 1; i < _FieldSize; i++)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                                    MoveCell(ref _PhyCell[FromCell, j], _LogCell[FreeCell, j].Coordinates,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref _PhyCell[FromCell, j], ref _PhyCell[MergeCell, j],
                                        _LogCell[FreeCell, j].Coordinates, Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать один элемент
                                {
                                    MergeCells(ref _PhyCell[MergeCell, j], _LogCell[FreeCell, j].Coordinates,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Right:
                    for (int i = 0; i < _FieldSize; i++)  // Элемент Столбца
                    {
                        int[] arrValues = new int[_FieldSize];
                        for (int j = 0; j < _FieldSize; j++)
                        {
                            if (_LogCell[i, j].IsCellFree)
                                arrValues[j] = 0;
                            else
                                arrValues[j] = _PhyCell[i, j].Value;
                        }
                        for (int k = _FieldSize - 1; k >= 0; k--)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int j = k; j >= 0; j--)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                            for (int j = FromCell + 1; j < _FieldSize; j++)
                            {
                                if (_LogCell[i, j].IsCellFree)
                                    FreeCell++;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int j = FromCell - 1; j >= 0; j--)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                                    MoveCell(ref _PhyCell[i, FromCell], _LogCell[i, FreeCell].Coordinates,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref _PhyCell[i, FromCell], ref _PhyCell[i, MergeCell],
                                        _LogCell[i, FreeCell].Coordinates, Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать один элемент
                                {
                                    MergeCells(ref _PhyCell[i, MergeCell], _LogCell[i, FreeCell].Coordinates,
                                        Animations.Direction.Left);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[MergeCell] = 0;
                                }
                            }
                        }
                    }
                    break;
                case Key.Down:
                    for (int j = 0; j < _FieldSize; j++)  // Элемент Строки
                    {
                        int[] arrValues = new int[_FieldSize];
                        for (int i = 0; i < _FieldSize; i++)
                        {
                            if (_LogCell[i, j].IsCellFree)
                                arrValues[i] = 0;
                            else
                                arrValues[i] = _PhyCell[i, j].Value;
                        }

                        for (int k = _FieldSize - 1; k >= 0; k--)
                        {
                            #region Поиск Первого ненулевого элемента
                            int FromCell = -1; // Первый ненулевой элемент
                            for (int i = k; i >= 0; i--)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                            for (int i = FromCell + 1; i < _FieldSize; i++)
                            {
                                if (_LogCell[i, j].IsCellFree)
                                    FreeCell++;
                                else
                                    break;
                            }
                            #endregion

                            #region Поиск Стакующегося элемента
                            int MergeCell = FromCell; // Второй элемент со значением FromCell
                            for (int i = FromCell - 1; i >= 0; i--)
                            {
                                if (!_LogCell[i, j].IsCellFree)
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
                                    MoveCell(ref _PhyCell[FromCell, j], _LogCell[FreeCell, j].Coordinates,
                                        Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell];
                                    arrValues[FromCell] = 0;
                                }
                            }
                            else // Если есть второй элемент
                            {
                                if (FromCell != FreeCell) // Если нужно двигать оба элемента
                                {
                                    MoveAndMergeCells(ref _PhyCell[FromCell, j], ref _PhyCell[MergeCell, j],
                                        _LogCell[FreeCell, j].Coordinates, Animations.Direction.Up);

                                    arrValues[FreeCell] = arrValues[FromCell] * 2;
                                    arrValues[FromCell] = 0;
                                    arrValues[MergeCell] = 0;
                                }
                                else // Если нужно двигать один элемент
                                {
                                    MergeCells(ref _PhyCell[MergeCell, j], _LogCell[FreeCell, j].Coordinates,
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

        /// <summary>
        /// Передвижение и объединение двух элементов
        /// </summary>
        /// <param name="From">Первый элемент</param>
        /// <param name="Dest">Второй элемент</param>
        /// <param name="To">Место перемещения</param>
        /// <param name="d">Направвление</param>
        public void MoveAndMergeCells(ref PhysicalCell From, ref PhysicalCell Dest, Point To, Animations.Direction d)
        {
            MoveCell(ref From, To, d);
            MoveCell(ref Dest, To, d, true);
        }

        /// <summary>
        /// Объединение двух элементов
        /// </summary>
        /// <param name="Dest">Элемент</param>
        /// <param name="To">Место перемещения</param>
        /// <param name="d">Направление</param>
        public void MergeCells(ref PhysicalCell Dest, Point To, Animations.Direction d)
        {
            MoveCell(ref Dest, To, d, true);
        }

        /// <summary>
        /// Перемещение элемента
        /// </summary>
        /// <param name="From">Элемент</param>
        /// <param name="To">Точка перемещения</param>
        /// <param name="d">Направление</param>
        /// <param name="IsMultiply">Увеличивать ли кол-во очков</param>
        public void MoveCell(ref PhysicalCell From, Point To, Animations.Direction d, bool IsMultiply = false)
        {
            _NumAnims++;
            var anim = new Animations(this);
            anim.SetMoveAnimation(From, To, d, IsMultiply);

            _LogCell[From.Row, From.Column].IsCellFree = true;
            _LogCell[(int)(To.Y / _CellHeight), (int)(To.X / _CellWidth)].IsCellFree = false;
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
            // Удаляем элемент в точке перемещения и присваиваем ему элемент, который мы перемещали
            DeleteElement(ref _PhyCell[xTo, yTo]);
            _PhyCell[xTo, yTo] = _PhyCell[xFrom, yFrom];
            fieldCanvas.UpdateLayout();

            // Удаляем элемент, который перемещали
            DeleteElement(ref _PhyCell[xFrom, yFrom]);

            // Пересоздаем элемент в точке перемещения
            fieldCanvas.Children.Remove(_PhyCell[xTo, yTo]);
            fieldCanvas.Children.Add(_PhyCell[xTo, yTo]);

            // Присваиваем свойства
            _PhyCell[xTo, yTo].Visibility = Visibility.Visible;
            _PhyCell[xTo, yTo].Row = xTo;
            _PhyCell[xTo, yTo].Column = yTo;
            fieldCanvas.UpdateLayout();

            _NumAnims--;
            // Если все анимации закончились, то спавним новый элемент в случайной позиции
            if (_NumAnims == 0)
            {
                SpawnRandomElement();
                UpdateScore();
            }
        }

        /// <summary>
        /// Удаление клетки
        /// </summary>
        /// <param name="element">Объект</param>
        private void DeleteElement(ref PhysicalCell element)
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
                int x = rnd.Next(0, _FieldSize);
                int y = rnd.Next(0, _FieldSize);

                if (_LogCell[y, x].IsCellFree)
                {
                    _PhyCell[y, x] = SpawnElement(y, x);

                    _LogCell[y, x].IsCellFree = false;
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
            // Проверка на свободность поля
            foreach (var cell in _LogCell)
            {
                if (cell.IsCellFree)
                {
                    HasFree = true;
                    break;
                }
            }
            if (!HasFree)
            {
                bool IsLose = true;
                // Проверка строк на наличие ходов
                for (int i = 0; i < _FieldSize; i++)
                {
                    for (int j = 0; j < _FieldSize - 1; j++)
                    {
                        if (_PhyCell[i, j].Value == _PhyCell[i, j + 1].Value)
                            IsLose = false;
                    }
                }
                // Проверка столбцов на наличие ходов
                for (int i = 0; i < _FieldSize - 1; i++)
                {
                    for (int j = 0; j < _FieldSize; j++)
                    {
                        if (_PhyCell[i, j].Value == _PhyCell[i + 1, j].Value)
                            IsLose = false;
                    }
                }
                // Если ходов нет, то блокируем ввод, выводим поражение
                if (IsLose)
                {
                    _IsInterfaceLocked = true;
                    _GameState = GameState.Lose;

                    mainWindow.Lose(Score);
                    return;
                }
            }

            // Если игра ещё не выйграна, проверка на победу
            if (_GameState != GameState.Win)
            {
                // Проверка на наличие клетки со значением 2048
                foreach (var element in _PhyCell)
                {
                    if (element != null && element.Value == 2048)
                    {
                        _GameState = GameState.Win;
                        break;
                    }
                }
                // Если такая клетка есть, вывод победы
                if (_GameState == GameState.Win)
                {
                    _IsInterfaceLocked = true;
                    mainWindow.Win();
                }
            }
        }

        /// <summary>
        /// Обновление очков
        /// </summary>
        public void UpdateScore()
        {
            // Обновление анимации текущего результата
            if(_Score > _PreviousScore)
            {
                mainWindow.curScore.Text = "+" + (_Score - _PreviousScore).ToString();
                Animations.SetScoreAnimation(mainWindow.curScore);
            }
            _PreviousScore = _Score;

            // Если Текущий результат лучше предыдущего
            if (_Score > _BestScore)
            {
                mainWindow.bestScore.Text = "+" + (_Score - _BestScore).ToString();
                Animations.SetScoreAnimation(mainWindow.bestScore);

                _BestScore = _Score;
            }

            mainWindow.blockScore.Text = _Score.ToString();
            mainWindow.blockBestScore.Text = _BestScore.ToString();
        }
    }
}
