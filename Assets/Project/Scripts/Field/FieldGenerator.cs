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
    [SerializeField] private float _maxWidthPercentage = 0.8f; // Максимальная ширина поля в процентах от ширины экрана

    [Range(0.1f, 1f)]
    [SerializeField] private float _maxHeightPercentage = 0.8f; // Максимальная высота поля в процентах от высоты экрана

    [SerializeField] private RectTransform _canvasRectTranform;

    [SerializeField] private Cell _fieldCellPrefab; // Префаб клетки 

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
        // Получаем размеры экрана в мировых координатах
        float screenWidth = _canvasRectTranform.rect.width;  // Ширина экрана в пикселях
        float screenHeight = _canvasRectTranform.rect.height; // Высота экрана в пикселях 

        // Рассчитываем максимальные размеры поля в пикселях
        float maxFieldWidth = screenWidth * _maxWidthPercentage;
        float maxFieldHeight = screenHeight * _maxHeightPercentage;

        // Рассчитываем размер одной клетки
        float cellWidth = maxFieldWidth / _lengthField;
        float cellHeight = maxFieldHeight / _heightField;

        // Учитываем отступы между клетками
        cellWidth = Mathf.Min(cellWidth, cellHeight) - _cellSpacing;
        cellHeight = cellWidth; // Делаем клетку квадратной 

        // Рассчитываем общий размер поля с учетом отступов
        float totalWidth = _lengthField * (cellWidth + _cellSpacing);
        float totalHeight = _heightField * (cellHeight + _cellSpacing);

        _backGroundMaskTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

        // Начальная позиция для первой клетки (верхний левый угол)
        Vector2 startPosition = new Vector2(-totalWidth / 2 + cellWidth / 2, totalHeight / 2 - cellHeight / 2);

        // Генерация клеток слева направо и сверху вниз
        for (int y = 0; y < _heightField; y++) // Уровни (сверху вниз)
        {
            for (int x = 0; x < _lengthField; x++) // Клетки в уровне (слева направо)
            {
                // Рассчитываем позицию для каждой клетки
                Vector2 cellPosition = startPosition + new Vector2(x * (cellWidth + _cellSpacing), -y * (cellHeight + _cellSpacing));
                var cell = Instantiate(_fieldCellPrefab, _cells);

                // Устанавливаем позицию и размер клетки 
                cell.SetNewName($"Cell_({x};{y})");
                cell.SetCellCoordinate(x, y);
                cell.SetStartPositonAndSize(cellPosition, new Vector2(cellWidth, cellHeight));
                _cellController.AddCell(cell);
            }
        }
        _cellController.RemoveCellsParent();
    }
}