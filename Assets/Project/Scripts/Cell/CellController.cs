using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
    [SerializeField] public List<Color> _availableColors;
    private List<int> _colors = new List<int>();

    private List<Cell> _cells = new();

    [SerializeField] private Cell _firstCell;
    [SerializeField] private Cell _secondCell;

    private List<CellInfo> _horizontalCells = new();
    private List<CellInfo> _verticalCells = new();
    private List<CellInfo> _allCellInfo = new();

    [SerializeField] private List<int> _indexHorizontalLineChek = new();
    [SerializeField] private List<int> _indexVerticalLineChek = new();
    private List<Cell> _cellsReColor = new();
    private List<Image> destroyedImages = new();

    public int BusySwapCount { get; set; }

    int _countMiss;


    private void Update()
    { 
        if (Input.GetMouseButtonUp(0)) Invoke(nameof(ResetSelection), 0.1f);
    }

    public void Init(int lineСount, int columnCout)
    {
        for (int k = 0; k < _availableColors.Count; k++)
        {
            _colors.Add(k);
        }

        for (int i = 0; i < columnCout; i++)
        {
            _horizontalCells.Add(new CellInfo());
        }

        for (int j = 0; j < lineСount; j++)
        {
            _verticalCells.Add(new CellInfo());
        }
    }

    public void AddCell(Cell cell)
    {
        _cells.Add(cell);

        cell.SetColor(GetRandomColorWithoutRepetition(cell.PosX, cell.PosY));

        _horizontalCells[cell.PosY].Cells.Add(cell);
        _verticalCells[cell.PosX].Cells.Add(cell);
    }

    public void OnCellClicked(Cell cell)
    {
        if (_firstCell == cell || BusySwapCount > 0) { ResetSelection(); return; }

        if (_firstCell == null) _firstCell = cell;

        else if (_secondCell == null)
        {
            _secondCell = cell;
            CheckAndSwapCells();
        }
    }

    private void CheckAndSwapCells()
    {
        if (CellsIsNeighbors(_firstCell, _secondCell))
        {
            if (HaveMatch())
            {
                SwapCells(_firstCell, _secondCell);
            }
            else DoFakeAnimation();
        }
    }

    private bool CellsIsNeighbors(Cell cell1, Cell cell2)
    {
        return Mathf.Abs(cell1.PosX - cell2.PosX) + Mathf.Abs(cell1.PosY - cell2.PosY) == 1;
    }

    private void SwapCells(Cell cell1, Cell cell2)
    {
        var imageCell1 = cell1.FigureImage;
        var colorCell1 = cell1.ColorNumber;

        cell1.FigureImage = cell2.FigureImage;
        cell1.SetColor(cell2.ColorNumber);
        cell1.MoveFigureImageToOriginalPosition(10, true);

        cell2.FigureImage = imageCell1;
        cell2.SetColor(colorCell1);
        cell2.MoveFigureImageToOriginalPosition(10, true);

        ResetSelection();  // Сбрасываем выбор
    }

    private void ResetSelection()
    {
        _firstCell = null;
        _secondCell = null;
    }

    private void DoFakeAnimation()
    {
        _firstCell.FakeSwapAnimation(_secondCell.GetAnchoredImagePosition(), 0.25f);
        _secondCell.FakeSwapAnimation(_firstCell.GetAnchoredImagePosition(), 0.25f);
    }

    public void RemoveCellsParent()
    {
        foreach (var cell in _cells)
        {
            cell.RemoveParent();
        }
    }

    private bool HaveMatch()
    {
        bool match = false;

        if (_firstCell.ColorNumber == _secondCell.ColorNumber) return false;

        _indexHorizontalLineChek.Clear();
        _indexVerticalLineChek.Clear();

        if (_firstCell.PosX == _secondCell.PosX)  //verticalSwipe
        {
            _indexHorizontalLineChek.Add(_firstCell.PosY);
            _indexHorizontalLineChek.Add(_secondCell.PosY);
            _indexVerticalLineChek.Add(_firstCell.PosX);
        }
        else
        {
            _indexHorizontalLineChek.Add(_firstCell.PosY);
            _indexVerticalLineChek.Add(_firstCell.PosX);
            _indexVerticalLineChek.Add(_secondCell.PosX);
        }

        match = SmartCheck();

        return match;
    }

    private bool SmartCheck()
    {
        bool haveMatch = false;

        foreach (var index in _indexHorizontalLineChek)
        {
            haveMatch = CheckForThreeInARow(_horizontalCells[index].Cells);
            if (haveMatch) return true;
        }

        foreach (var index in _indexVerticalLineChek)
        {

            haveMatch = CheckForThreeInARow(_verticalCells[index].Cells);
            if (haveMatch) return true;
        }

        return haveMatch;
    }

    private bool CheckForThreeInARow(List<Cell> cells)
    {
        _cellsReColor.Clear();
        _cellsReColor = new List<Cell>(cells);

        // Находим индексы элементов
        int indexFirstCell = _cellsReColor.IndexOf(_firstCell);
        int indexSecondCell = _cellsReColor.IndexOf(_secondCell);

        // Если первый элемент найден, а второй нет, заменяем первый на второй (если второй существует)
        if (indexFirstCell != -1 && indexSecondCell == -1)
        {
            // Если второго элемента нет, просто заменяем первый на новый (например, _secondCell)
            _cellsReColor[indexFirstCell] = _secondCell;
        }
        // Если второй элемент найден, а первый нет, заменяем второй на первый
        else if (indexSecondCell != -1 && indexFirstCell == -1)
        {
            _cellsReColor[indexSecondCell] = _firstCell;
        }
        // Если оба элемента найдены, меняем их местами
        else if (indexFirstCell != -1 && indexSecondCell != -1)
        {
            Cell temp = _cellsReColor[indexFirstCell];
            _cellsReColor[indexFirstCell] = _cellsReColor[indexSecondCell];
            _cellsReColor[indexSecondCell] = temp;
        }

        for (int i = 0; i <= _cellsReColor.Count - 3; i++)
        {
            if (_cellsReColor[i].ColorNumber == _cellsReColor[i + 1].ColorNumber && _cellsReColor[i].ColorNumber == _cellsReColor[i + 2].ColorNumber)
            {
                return true;
            }
        }

        return false;
    }

    public void TryCellDestroy()
    {
        if (BusySwapCount == 0)
        {
            if (_allCellInfo.Count == 0)
            {
                _allCellInfo.AddRange(_horizontalCells);
                _allCellInfo.AddRange(_verticalCells);
            }

            int matchCount;
            int lenghtCount;

            foreach (var line in _allCellInfo)
            {
                matchCount = 1;
                lenghtCount = line.Cells.Count - 1;

                for (int i = 1; i < line.Cells.Count; i++)
                {
                    if (line.Cells[i].ColorNumber == line.Cells[i - 1].ColorNumber)
                    {
                        matchCount++;
                    }
                    else
                    {
                        // Если найдено 3 или более одинаковых элементов подряд
                        if (matchCount >= 3)
                        {
                            // Вызываем Puff() для всех элементов в последовательности
                            for (int j = i - 1; j >= i - matchCount; j--)
                            {
                                line.Cells[j].Puff();
                            }
                        }
                        matchCount = 1;
                    }

                    if (i == lenghtCount && matchCount >= 3)
                    {
                        for (int j = lenghtCount; j > lenghtCount - matchCount; j--)
                        {
                            line.Cells[j].Puff();
                        }
                    }
                }
            }

            CellMoveDown();
        }
    }

    private void CellMoveDown()
    {
        foreach (var line in _verticalCells)
        {
            destroyedImages.Clear();

            // Проходим по ячейкам снизу вверх
            for (int i = line.Cells.Count - 1; i >= 0; i--)
            {
                if (line.Cells[i].HasPuffed)
                {
                    destroyedImages.Add(line.Cells[i].FigureImage);
                    line.Cells[i].HasPuffed = false;
                }
                else if (destroyedImages.Count > 0)
                {
                    var targeCell = line.Cells[i + destroyedImages.Count];

                    targeCell.FigureImage = line.Cells[i].FigureImage;
                    targeCell.ColorNumber = line.Cells[i].ColorNumber;
                    targeCell.MoveFigureImageToOriginalPosition(10, true);
                }
            }

            if (destroyedImages.Count > 0)
            {
                var j = destroyedImages.Count - 1;

                foreach (var cell in line.Cells)
                {
                    cell.FigureImage = destroyedImages[j];
                    cell.SetRandomColor();

                    cell.SetImageFigurePosition(line.Cells[0].GetOriginalPosition() + new Vector2(0, (j + 1) * cell.RecTranform.sizeDelta.y));
                    cell.MoveFigureImageToOriginalPosition(10, true);
                    --j;
                    if (j < 0) break;
                }
            }
        }
    }

    private int GetRandomColorWithoutRepetition(int x, int y)
    {
        _countMiss = 0;

        if (x >= 2)
        {
            int left1 = GetColorByCoordinate(x - 1, y);
            int left2 = GetColorByCoordinate(x - 2, y);
           
            if (left1 == left2)
            {
                // Перемещаем "запрещенный" цвет в конец списка
                MoveToEnd(_colors, left1);
            }
        }

        // Проверка вертикальной линии (2 предыдущих элемента)
        if (y >= 2)
        {
            int up1 = GetColorByCoordinate(x, y - 1);
            int up2 = GetColorByCoordinate(x, y - 2);
          
            if (up1 == up2)
            {
                // Перемещаем "запрещенный" цвет в конец списка
                MoveToEnd(_colors, up1);
            }
        }

        int randomIndex = Random.Range(0, _colors.Count - _countMiss);

        return _colors[randomIndex];
    }

    private void MoveToEnd(List<int> list, int colorNumber)
    {
        ++_countMiss;
        list.Remove(colorNumber);
        list.Add(colorNumber);
    }

    private int GetColorByCoordinate(int x, int y)
    {
        int color = 0;
      
        foreach (var cell in _cells)
        {
            if (cell.PosX == x && cell.PosY == y)
            {
                color = cell.ColorNumber;
                break;
            }
        }
        return color;
    }
}

[System.Serializable]
public class CellInfo
{
    public List<Cell> Cells = new();
}