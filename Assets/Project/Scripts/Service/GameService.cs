using UnityEngine;
using Zenject;

public class GameService : MonoBehaviour
{
    [SerializeField] private GameFieldView _gameFieldView;
    [SerializeField] private InputService _inputService;

    private GameSettings _gameSettings;

    [Inject]
    private void Construct(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    private void Start()
    {
        GameFieldModel fieldModel = new GameFieldModel(_gameFieldView, _gameSettings);
        _inputService.OnMouseUpEvent += fieldModel.ResetSelection;

        GameFieldController fieldController = new GameFieldController(fieldModel, _gameFieldView);
        fieldController.Initialize(_gameSettings);
    }
}