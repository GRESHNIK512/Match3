using System.Collections.Generic;
using UnityEngine;

public class GameFieldModel
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int CellSize { get; private set; }
    public int TotalSize { get; private set; }

    private Dictionary<string, FieldCellModel> _fieldCells = new();
    private List<CellInfo> _horizontalCells = new();
    private List<CellInfo> _verticalCells = new();
    private List<CellInfo> _allcellinfo = new();

    private Dictionary<string, CrystalModel> _crystals = new();

    private GameFieldView _gameFieldView;
    private GameSettings _gameSettings;

    private List<int> _colors = new();
    private FieldCellModel _firstCell;
    private FieldCellModel _secondCell;
    private int _busySwapCount = 0;

    private List<int> _indexHorizontalLineChek = new();
    private List<int> _indexVerticalLineChek = new();

    private List<FieldCellModel> _lineCellWithSwipeColor = new();
    private List<CrystalModel> _destroyedCrystals = new();

    public GameFieldModel(GameFieldView fieldView, GameSettings gameSettings)
    {
        _gameFieldView = fieldView;
        _gameSettings = gameSettings;
    }

    public void InitializeData()
    {
        Width = _gameSettings.Width;
        Height = _gameSettings.Height;

        // Получаем размеры экрана в мировых координатах
        var screenWidth = (int)_gameFieldView.CanvasRectTransform.rect.width;  // Ширина экрана в пикселях 
        var screenHeight = (int)_gameFieldView.CanvasRectTransform.rect.height; // Высота экрана в пикселях 

        // Рассчитываем максимальные размеры поля в пикселях
        float maxFieldWidth = screenWidth * _gameSettings.PercentCanvas / 100;
        float maxFieldHeight = screenHeight * _gameSettings.PercentCanvas / 100;

        // Рассчитываем размер одной клетки
        var cellWidth = (int)maxFieldWidth / Width;
        var cellHeight = (int)maxFieldHeight / Height;

        // Учитываем отступы между клетками
        CellSize = Mathf.Min(cellWidth, cellHeight) - _gameSettings.Spacing;
        TotalSize = CellSize + _gameSettings.Spacing;

        // Рассчитываем общий размер поля с учетом отступов
        var totalWidth = Width * TotalSize;
        var totalHeight = Height * TotalSize;

        // Начальная позиция для первой клетки (верхний левый угол)
        var startPosition = new Vector2(-totalWidth / 2 + CellSize / 2, totalHeight / 2 - CellSize / 2);

        for (int i = 0; i < Height; i++)
            _horizontalCells.Add(new CellInfo());

        for (int j = 0; j < Width; j++)
            _verticalCells.Add(new CellInfo());

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var pos = startPosition + new Vector2(x * TotalSize, -y * TotalSize);
                var fieldModel = new FieldCellModel(x, y, pos);
                _fieldCells.Add($"{x}{y}", fieldModel);

                _horizontalCells[y].FieldCells.Add(fieldModel);
                _verticalCells[x].FieldCells.Add(fieldModel);

                _crystals.Add($"{x}{y}", new CrystalModel(x, y, pos));
            }
        }
    }

    public void AttachCrystalViewToModel(string key, CrystalView crystalView)
    {
        _crystals[key].SetView(crystalView);
    }

    public void AttachFieldViewToModel(string key, FieldCellView fieldCellView)
    {
        _fieldCells[key].SetView(fieldCellView);
    }

    public void SetColorCrystalsOnStart()
    {
        int countIgnore, x, y, crystalLeftColor, crystalUpColor;

        for (int i = 0; i < _gameSettings.UseMaxColors; i++)
        {
            _colors.Add(i);
        }

        foreach (var crystal in _crystals)
        {
            countIgnore = 0;
            x = crystal.Value.X;
            y = crystal.Value.Y;

            if (x >= 2)
            {
                crystalLeftColor = GetCrystal(x - 1, y).ColorNumber;

                if (crystalLeftColor == GetCrystal(x - 2, y).ColorNumber)
                {
                    ++countIgnore;
                    MoveIndexColorToEnd(crystalLeftColor);
                }
            }

            if (crystal.Value.Y >= 2)
            {
                crystalUpColor = GetCrystal(x, y - 1).ColorNumber;

                if (crystalUpColor == GetCrystal(x, y - 2).ColorNumber)
                {
                    ++countIgnore;
                    MoveIndexColorToEnd(crystalUpColor);
                }
            }

            crystal.Value.UpdateColor(_colors[Random.Range(0, _colors.Count - countIgnore)]);
        }
    }

    public Vector2 GetFieldPosition(int x, int y)
    {
        return _fieldCells[$"{x}{y}"].Position;
    }

    private CrystalModel GetCrystal(FieldCellModel fieldcell)
    {
        return _crystals[$"{fieldcell.X}{fieldcell.Y}"];
    }

    private CrystalModel GetCrystal(int x, int y)
    {
        return _crystals[$"{x}{y}"];
    }

    private void MoveIndexColorToEnd(int colorNumber)
    {
        _colors.Remove(colorNumber);
        _colors.Add(colorNumber);
    }

    public void AddSelectedCell(FieldCellView fieldCellView)
    {
        var selectedFieldCell = _fieldCells[fieldCellView.name];

        if (_firstCell == selectedFieldCell || _busySwapCount > 0)
        {
            ResetSelection();
            return;
        }

        if (_firstCell == null)
            _firstCell = selectedFieldCell;
        else if (_secondCell == null)
        {
            _secondCell = selectedFieldCell;
            CheckAndSwapCells();
        }
    }

    public void ResetSelection()
    {
        _firstCell = null;
        _secondCell = null;
    }

    private void CheckAndSwapCells()
    {
        if (FieldCellsIsNeighbors())
        {
            if (HaveMatch())
                SwapCrystal();
            else DoFakeAnimation();
        }
    }

    private bool FieldCellsIsNeighbors()
    {
        return Mathf.Abs(_firstCell.X - _secondCell.X) + Mathf.Abs(_firstCell.Y - _secondCell.Y) == 1;
    }

    private bool HaveMatch()
    {
        bool match = false;

        if (GetCrystal(_firstCell).ColorNumber == GetCrystal(_secondCell).ColorNumber)
            return false;

        _indexHorizontalLineChek.Clear();
        _indexVerticalLineChek.Clear();

        if (_firstCell.X == _secondCell.X)  //verticalSwipe
        {
            _indexHorizontalLineChek.Add(_firstCell.Y);
            _indexHorizontalLineChek.Add(_secondCell.Y);
            _indexVerticalLineChek.Add(_firstCell.X);
        }
        else
        {
            _indexHorizontalLineChek.Add(_firstCell.Y);
            _indexVerticalLineChek.Add(_firstCell.X);
            _indexVerticalLineChek.Add(_secondCell.X);
        }

        match = SmartCheck();

        return match;
    }

    private bool SmartCheck()
    {
        bool haveMatch = false;

        foreach (var index in _indexHorizontalLineChek)
        {
            haveMatch = CheckForThreeInARow(_horizontalCells[index].FieldCells);
            if (haveMatch) return true;
        }

        foreach (var index in _indexVerticalLineChek)
        {
            haveMatch = CheckForThreeInARow(_verticalCells[index].FieldCells);
            if (haveMatch) return true;
        }

        return haveMatch;
    }

    private bool CheckForThreeInARow(List<FieldCellModel> cells)
    {
        _lineCellWithSwipeColor = new List<FieldCellModel>(cells);

        // Находим индексы элементов
        int indexFirstCell = _lineCellWithSwipeColor.IndexOf(_firstCell);
        int indexSecondCell = _lineCellWithSwipeColor.IndexOf(_secondCell);

        // Если первый элемент найден, а второй нет, заменяем первый на второй (если второй существует)
        if (indexFirstCell != -1 && indexSecondCell == -1)
            _lineCellWithSwipeColor[indexFirstCell] = _secondCell;

        // Если второй элемент найден, а первый нет, заменяем второй на первый
        else if (indexSecondCell != -1 && indexFirstCell == -1)
            _lineCellWithSwipeColor[indexSecondCell] = _firstCell;

        // Если оба элемента найдены, меняем их местами
        else if (indexFirstCell != -1 && indexSecondCell != -1)
        {
            FieldCellModel temp = _lineCellWithSwipeColor[indexFirstCell];
            _lineCellWithSwipeColor[indexFirstCell] = _lineCellWithSwipeColor[indexSecondCell];
            _lineCellWithSwipeColor[indexSecondCell] = temp;
        }

        for (int i = 0; i <= _lineCellWithSwipeColor.Count - 3; i++)
        {
            int targetColorNumber = GetCrystal(_lineCellWithSwipeColor[i]).ColorNumber;
            if (targetColorNumber == GetCrystal(_lineCellWithSwipeColor[i + 1]).ColorNumber &&
                targetColorNumber == GetCrystal(_lineCellWithSwipeColor[i + 2]).ColorNumber)
                return true;
        }

        return false;
    }

    private void SwapCrystal()
    {
        _busySwapCount += 2;

        var firstCrystal = GetCrystal(_firstCell);
        var secondCrystal = GetCrystal(_secondCell);

        firstCrystal.ChangePositionWithAnimation(secondCrystal.Position);
        secondCrystal.ChangePositionWithAnimation(firstCrystal.Position);

        // Обновляем значения в словаре
        SwipeInfoCrystals(firstCrystal, secondCrystal);

        ResetSelection();  // Сбрасываем выбор
    }

    private void DoFakeAnimation()
    {
        _busySwapCount += 2;

        GetCrystal(_firstCell).FakeSwap(_secondCell.Position);
        GetCrystal(_secondCell).FakeSwap(_firstCell.Position);
    }

    public void MoveComplete(bool searchmatch)
    {
        --_busySwapCount;

        if (searchmatch && _busySwapCount == 0)
            SearchCellToMatch();
    }

    private void SearchCellToMatch()
    {
        if (_allcellinfo.Count == 0)
        {
            _allcellinfo.AddRange(_horizontalCells);
            _allcellinfo.AddRange(_verticalCells);
        }

        int matchcount;
        int lenghtcount;

        foreach (var line in _allcellinfo)
        {
            matchcount = 1;
            lenghtcount = line.FieldCells.Count - 1;

            for (int i = 1; i < line.FieldCells.Count; i++)
            {
                if (GetCrystal(line.FieldCells[i]).ColorNumber ==
                    GetCrystal(line.FieldCells[i - 1]).ColorNumber)
                {
                    matchcount++;
                }

                else
                {
                    //если найдено 3 или более одинаковых элементов подряд
                    if (matchcount >= 3)
                    {
                        // вызываем puff() для всех элементов в последовательности
                        for (int j = i - 1; j >= i - matchcount; j--)
                            _crystals[$"{line.FieldCells[j].X}{line.FieldCells[j].Y}"].Puff();
                    }

                    matchcount = 1;
                }

                //если последовательность закончилась последним элементом
                if (i == lenghtcount && matchcount >= 3)
                {
                    for (int j = lenghtcount; j > lenghtcount - matchcount; j--)
                        _crystals[$"{line.FieldCells[j].X}{line.FieldCells[j].Y}"].Puff();
                }
            }
        }

        CellMoveDown();
    }

    private void CellMoveDown()
    {
        foreach (var line in _verticalCells)
        {
            _destroyedCrystals.Clear();

            // проходим по ячейкам снизу вверх
            for (int i = line.FieldCells.Count - 1; i >= 0; i--)
            {
                var crystal = GetCrystal(line.FieldCells[i]);

                if (crystal.HasPuffed)
                {
                    _destroyedCrystals.Add(crystal);
                }
                else if (_destroyedCrystals.Count > 0) //смещаем живые кристаллы вниз
                {
                    var targetCrystal = GetCrystal(line.FieldCells[i + _destroyedCrystals.Count]);
                    ++_busySwapCount;
                    crystal.ChangePositionWithAnimation(targetCrystal.Position);
                    SwipeInfoCrystals(targetCrystal, crystal);
                }
            }

            //выводим убитые кристаллы вверх и смещаем вниз
            if (_destroyedCrystals.Count > 0)
            {
                foreach (var crystal in _destroyedCrystals)
                {
                    crystal.HasPuffed = false;
                    crystal.TeleportCrystalPosition(crystal.Position + new Vector2(0, _destroyedCrystals.Count * CellSize));
                    crystal.UpdateColor(_colors[Random.Range(0, _colors.Count)]);
                    ++_busySwapCount;
                    crystal.ChangePositionWithAnimation(crystal.Position);
                }
            }
        }
    }

    private void SwipeInfoCrystals(CrystalModel firstCrystal, CrystalModel secondCrystal)
    {
        var temX = firstCrystal.X;
        var temY = firstCrystal.Y;
        var tempPos = firstCrystal.Position;

        _crystals[$"{firstCrystal.X}{firstCrystal.Y}"] = secondCrystal;
        _crystals[$"{secondCrystal.X}{secondCrystal.Y}"] = firstCrystal;

        firstCrystal.SetCoordinateAndPosition(secondCrystal.X, secondCrystal.Y, secondCrystal.Position);
        secondCrystal.SetCoordinateAndPosition(temX, temY, tempPos);
    }
}

[System.Serializable]
public class CellInfo
{
    public List<FieldCellModel> FieldCells = new();
}