using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Config/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public int PercentCanvas = 80;
    public int Spacing = 1;
    public int Width = 8;
    public int Height = 12;

    [Space(15)]
    public FieldCellView FieldCellView;
    public CrystalView CrystalView;

    [Space(15)]
    [Min(2)]
    public int UseMaxColors;
    public Color[] Colors;
} 