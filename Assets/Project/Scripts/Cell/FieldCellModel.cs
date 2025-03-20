using UnityEngine;

public class FieldCellModel
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Vector2 Position { get; private set; }

    private FieldCellView _fieldCellView;

    public FieldCellModel(int x, int y, Vector2 pos)
    {
        X = x;
        Y = y;
        Position = pos;
    }

    public void SetView(FieldCellView view)
    {
        _fieldCellView = view;
    }
}