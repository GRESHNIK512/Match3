using System;
using UnityEngine;
using UnityEngine.UI;

public class FieldGenerator : MonoBehaviour
{
    [Min(3)]
    [SerializeField] private int _lengthField;

    [Min(3)]
    [SerializeField] private int _heightField;

    [SerializeField] private float _cellSpacing = 1.1f;

    [Range(0.1f, 1f)]
    [SerializeField] private float _maxWidthPercentage = 0.8f; // ������������ ������ ���� � ��������� �� ������ ������

    [Range(0.1f, 1f)]
    [SerializeField] private float _maxHeightPercentage = 0.8f; // ������������ ������ ���� � ��������� �� ������ ������

    [SerializeField] private RectTransform _canvasRectTranform;

    [SerializeField] private Cell _fieldCellPrefab; // ������ ������ 

    [SerializeField] private CellController _cellController;

    [SerializeField] private RectTransform _backGroundMaskTransform;

    [SerializeField] private Transform _cells;

    private void Start()
    {
        _cellController.Init(_lengthField, _heightField);
        GenerateField();
    }

    private void GenerateField()
    {
        // �������� ������� ������ � ������� �����������
        float screenWidth = _canvasRectTranform.rect.width;  // ������ ������ � ��������
        float screenHeight = _canvasRectTranform.rect.height; // ������ ������ � �������� 

        // ������������ ������������ ������� ���� � ��������
        float maxFieldWidth = screenWidth * _maxWidthPercentage;
        float maxFieldHeight = screenHeight * _maxHeightPercentage;

        // ������������ ������ ����� ������
        float cellWidth = maxFieldWidth / _lengthField;
        float cellHeight = maxFieldHeight / _heightField;

        // ��������� ������� ����� ��������
        cellWidth = Mathf.Min(cellWidth, cellHeight) - _cellSpacing;
        cellHeight = cellWidth; // ������ ������ ���������� 

        // ������������ ����� ������ ���� � ������ ��������
        float totalWidth = _lengthField * (cellWidth + _cellSpacing);
        float totalHeight = _heightField * (cellHeight + _cellSpacing);

        _backGroundMaskTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

        // ��������� ������� ��� ������ ������ (������� ����� ����)
        Vector2 startPosition = new Vector2(-totalWidth / 2 + cellWidth / 2, totalHeight / 2 - cellHeight / 2);

        // ��������� ������ ����� ������� � ������ ����
        for (int y = 0; y < _heightField; y++) // ������ (������ ����)
        {
            for (int x = 0; x < _lengthField; x++) // ������ � ������ (����� �������)
            {
                // ������������ ������� ��� ������ ������
                Vector2 cellPosition = startPosition + new Vector2(x * (cellWidth + _cellSpacing), -y * (cellHeight + _cellSpacing));
                var cell = Instantiate(_fieldCellPrefab, _cells);

                // ������������� ������� � ������ ������ 
                cell.SetNewName($"Cell_({x};{y})");
                cell.SetCellCoordinate(x, y);
                cell.SetStartPositonAndSize(cellPosition, new Vector2(cellWidth, cellHeight));
                _cellController.AddCell(cell);
            }
        }
        _cellController.RemoveCellsParent();
    }
}