using UnityEngine;

public class GameFieldView : MonoBehaviour
{
    public RectTransform CanvasRectTransform => _canvasRectTranform;
    [SerializeField] private RectTransform _canvasRectTranform;

    [SerializeField] private Transform _fieldCellsTransform;
    [SerializeField] private RectTransform _backGroundMaskTransform;
    [SerializeField] private RectTransform _elementsRectTr;

    public void DrawField(GameFieldController gameFieldController, GameFieldModel gameFieldModel, GameSettings gameSettings)
    {
        var width = gameFieldModel.Width;
        var height = gameFieldModel.Height;

        FieldCellView fieldCellView = null;
        CrystalView crystalView = null;

        _backGroundMaskTransform.sizeDelta = new Vector2(width * gameFieldModel.TotalSize, height * gameFieldModel.TotalSize);

        for (int y = 0; y < height; y++) // Уровни (сверху вниз)
        {
            for (int x = 0; x < width; x++) // Клетки в уровне (слева направо)
            {
                // Рассчитываем позицию для каждой клетки 
                fieldCellView = Instantiate(gameSettings.FieldCellView, _fieldCellsTransform);

                var pos = gameFieldModel.GetFieldPosition(x, y);
                var size = gameFieldModel.CellSize;

                fieldCellView.OnMouseCellEnterEvent += gameFieldController.CellEnter;
                fieldCellView.ReplacePosition(pos);
                fieldCellView.ReplaceSize(size);
                fieldCellView.name = $"{x}{y}";
                gameFieldModel.AttachFieldViewToModel($"{x}{y}", fieldCellView);

                crystalView = Instantiate(gameSettings.CrystalView, _elementsRectTr);
                crystalView.OnEndAimationEvent += gameFieldModel.MoveComplete;
                crystalView.Init(gameSettings, pos, size);
                crystalView.name = $"CrystalView({x};{y})";
                gameFieldModel.AttachCrystalViewToModel($"{x}{y}", crystalView);
            }
        }
    }
}