using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
    public int BusySwapCount { get; set; }

    [SerializeField] private List<Color> _availableColors;
    [SerializeField] private Transform _animationCanvas;

    private List<int> _colors = new List<int>();

    private List<Cell> _cells = new();

    private List<CellInfo> _allCellInfo = new();
    private List<CellInfo> _horizontalCells = new();
    private List<CellInfo> _verticalCells = new();

    private Cell _firstCell;
    private Cell _secondCell;

    private List<int> _indexHorizontalLineChek = new();
    private List<int> _indexVerticalLineChek = new();

    private List<Cell> _cellsReColor = new();
    private List<Image> destroyedImages = new(); 

    public void Init(int line�ount, int columnCout)
    {
        for (int k = 0; k < _availableColors.Count; k++)
            _colors.Add(k);

        for (int i = 0; i < columnCout; i++)
            _horizontalCells.Add(new CellInfo());

        for (int j = 0; j < line�ount; j++)
            _verticalCells.Add(new CellInfo());
    }

    public void AddCell(Cell cell)
    {
        _cells.Add(cell);

        _horizontalCells[cell.PosY].Cells.Add(cell);
        _verticalCells[cell.PosX].Cells.Add(cell);

        cell.SetColor(GetRandomColorWithoutRepetition(cell.PosX, cell.PosY), _availableColors);
    }

    public void OnCellClicked(Cell cell)
    {
        if (_firstCell == cell || BusySwapCount > 0)
        {
            ResetSelection();
            return;
        }

        if (_firstCell == null)
            _firstCell = cell;
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
                SwapCells(_firstCell, _secondCell);
            else
                DoFakeAnimation();
        }
    }

    private bool CellsIsNeighbors(Cell cell1, Cell cell2)
    {
        return Mathf.Abs(cell1.PosX - cell2.PosX) + Mathf.Abs(cell1.PosY - cell2.PosY) == 1;
    }

    private void SwapCells(Cell cell1, Cell cell2)
    {
        var tempImageCell1 = cell1.FigureImage;
        var tempColorCell1 = cell1.ColorNumber;

        BusySwapCount += 2;

        cell1.FigureImage = cell2.FigureImage;
        cell1.SetColor(cell2.ColorNumber, _availableColors);
        cell1.MoveFigureImageToOriginalPosition(0.25f, false);

        cell2.FigureImage = tempImageCell1;
        cell2.SetColor(tempColorCell1, _availableColors);
        cell2.MoveFigureImageToOriginalPosition(0.25f, true);

        ResetSelection();  // ���������� �����
    }

    public void ResetSelection()
    {
        _firstCell = null;
        _secondCell = null;
    }

    private void DoFakeAnimation()
    {
        BusySwapCount += 2;
        _firstCell.FakeSwapAnimation(_secondCell.GetAnchoredImagePosition(), 0.25f);
        _secondCell.FakeSwapAnimation(_firstCell.GetAnchoredImagePosition(), 0.25f);
    }
    public void OnMoveComplete(bool searchMatch)
    {
        --BusySwapCount;
        if (searchMatch)
            SearchCellToMatch();
    }

    public void RemoveCellsParent()
    {
        foreach (var cell in _cells)
            cell.SetParentForImage(_animationCanvas);
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
        _cellsReColor = new List<Cell>(cells);

        // ������� ������� ���������
        int indexFirstCell = _cellsReColor.IndexOf(_firstCell);
        int indexSecondCell = _cellsReColor.IndexOf(_secondCell);

        // ���� ������ ������� ������, � ������ ���, �������� ������ �� ������ (���� ������ ����������)
        if (indexFirstCell != -1 && indexSecondCell == -1)
            _cellsReColor[indexFirstCell] = _secondCell;

        // ���� ������ ������� ������, � ������ ���, �������� ������ �� ������
        else if (indexSecondCell != -1 && indexFirstCell == -1)
            _cellsReColor[indexSecondCell] = _firstCell;

        // ���� ��� �������� �������, ������ �� �������
        else if (indexFirstCell != -1 && indexSecondCell != -1)
        {
            Cell temp = _cellsReColor[indexFirstCell];
            _cellsReColor[indexFirstCell] = _cellsReColor[indexSecondCell];
            _cellsReColor[indexSecondCell] = temp;
        }

        for (int i = 0; i <= _cellsReColor.Count - 3; i++)
        {
            if (_cellsReColor[i].ColorNumber == _cellsReColor[i + 1].ColorNumber &&
                _cellsReColor[i].ColorNumber == _cellsReColor[i + 2].ColorNumber)
                return true; 
        }

        return false;
    }

    public void SearchCellToMatch()
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
                        matchCount++;
                    else
                    {
                        // ���� ������� 3 ��� ����� ���������� ��������� ������
                        if (matchCount >= 3)
                        {
                            // �������� Puff() ��� ���� ��������� � ������������������
                            for (int j = i - 1; j >= i - matchCount; j--)
                                line.Cells[j].Puff();
                        }

                        matchCount = 1;
                    }

                    // ���� ������������������ ����������� ��������� ���������
                    if (i == lenghtCount && matchCount >= 3)
                    {
                        for (int j = lenghtCount; j > lenghtCount - matchCount; j--)
                            line.Cells[j].Puff();
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

            // �������� �� ������� ����� �����
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
                    ++BusySwapCount;
                    targeCell.MoveFigureImageToOriginalPosition(0.8f, true);
                }
            }

            if (destroyedImages.Count > 0)
            {
                var j = destroyedImages.Count - 1;

                foreach (var cell in line.Cells)
                {
                    cell.FigureImage = destroyedImages[j];
                    cell.SetRandomColor(_availableColors);
                    cell.SetImageFigurePosition(line.Cells[0].GetOriginalPosition() + new Vector2(0, (j + 1) * cell.RecTranform.sizeDelta.y));
                    ++BusySwapCount;
                    cell.MoveFigureImageToOriginalPosition(0.8f, true);

                    if (--j < 0) break;
                }
            }
        }
    }

    private int GetRandomColorWithoutRepetition(int x, int y)
    {
        var countIgnore = 0;

        if (x >= 2)
        {
            int left1 = GetColorByCoordinate(x - 1, y);

            if (left1 == GetColorByCoordinate(x - 2, y))
            {
                ++countIgnore;
                MoveToEnd(left1);
            }
        }

        // �������� ������������ ����� (2 ���������� ��������)
        if (y >= 2)
        {
            int up1 = GetColorByCoordinate(x, y - 1);

            if (up1 == GetColorByCoordinate(x, y - 2))
            {
                ++countIgnore;
                MoveToEnd(up1);
            }
        }

        int randomIndex = Random.Range(0, _colors.Count - countIgnore);

        return _colors[randomIndex];
    }

    private void MoveToEnd(int colorNumber)
    {
        _colors.Remove(colorNumber);
        _colors.Add(colorNumber);
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