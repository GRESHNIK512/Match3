using UnityEngine;

public class CrystalModel
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int ColorNumber { get; private set; }
    public bool HasPuffed { get { return _hasPuffed; } set { _hasPuffed = value; _crystalView.ChangeCrystalVisible(!_hasPuffed); } }
    private bool _hasPuffed;

    public Vector2 Position { get; private set; }

    private CrystalView _crystalView;

    public CrystalModel(int x, int y, Vector2 pos)
    {
        SetCoordinateAndPosition(x, y, pos);
    }

    public void SetCoordinateAndPosition(int x, int y, Vector2 pos)
    {
        X = x;
        Y = y;
        Position = pos;
    }

    public void SetView(CrystalView view)
    {
        _crystalView = view;
    }

    public void ChangePositionWithAnimation(Vector2 newPos)
    {
        _crystalView.MoveToPositionWithAnimation(newPos);
    }

    public void UpdateColor(int colorNubber)
    {
        ColorNumber = colorNubber;
        _crystalView.ReplaceColor(colorNubber);
    }

    public void FakeSwap(Vector2 fakePos)
    {
        _crystalView.PlayFakeAnimation(Position, fakePos);
    }

    public void Puff()
    {
        if (!HasPuffed)
            HasPuffed = true;
    }

    public void TeleportCrystalPosition(Vector2 pos)
    {
        _crystalView.TeleportToPosition(pos);
    }
}