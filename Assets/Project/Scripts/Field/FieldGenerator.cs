using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRectTranform;
    [Min(3)]
    [SerializeField] private int _lengthField;

    [Min(3)]
    [SerializeField] private int _heightField;

    [SerializeField] private Cell _fieldCellPrefab; // ������ ������ 
    [SerializeField] private float _cellSpacing = 1.1f;

    [Range(0.1f, 0.99f)]
    [SerializeField] private float _maxWidthPercentage = 0.8f; // ������������ ������ ���� � ��������� �� ������ ������

    [Range(0.1f, 0.99f)]
    [SerializeField] private float _maxHeightPercentage = 0.8f; // ������������ ������ ���� � ��������� �� ������ ������

    [SerializeField] private CellController _cellController;

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

        // ��������� ������� ��� ������ ������ (������� ����� ����)
        Vector2 startPosition = new Vector2(-totalWidth / 2 + cellWidth / 2, totalHeight / 2 - cellHeight / 2);

        // ��������� ������ ����� ������� � ������ ����
        for (int y = 0; y < _heightField; y++) // ������ (������ ����)
        {
            for (int x = 0; x < _lengthField; x++) // ������ � ������ (����� �������)
            {
                // ������������ ������� ��� ������ ������
                Vector2 cellPosition = startPosition + new Vector2(x * (cellWidth + _cellSpacing), -y * (cellHeight + _cellSpacing));
                var cell = Instantiate(_fieldCellPrefab, transform);

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