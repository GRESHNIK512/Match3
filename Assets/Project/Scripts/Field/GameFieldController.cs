
public class GameFieldController
{
    private GameFieldModel _fieldModel;
    private GameFieldView _fieldView;

    public GameFieldController(GameFieldModel model, GameFieldView view)
    {
        _fieldModel = model;
        _fieldView = view;
    }

    public void Initialize(GameSettings gameSettings)
    {
        _fieldModel.InitializeData();
        _fieldView.DrawField(this, _fieldModel, gameSettings);
        _fieldModel.SetColorCrystalsOnStart();
    }

    public void CellEnter(FieldCellView fieldCellView)
    {
        _fieldModel.AddSelectedCell(fieldCellView);
    }
}