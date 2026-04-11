using System.Collections.Generic;
using UnityEngine;

public class BoosterOneTille : Booster
{
    [SerializeField] private ShapeData _shapeData;
    [SerializeField] private ShapeBooster _shape;

    private List<TileColor> _tileColors;
    protected override void Awake()
    {
        base.Awake();
        _tileColors = GameConfig.Ins.TileColorConfig.ColorSprites;
    }
    void Start()
    {
        _shape.SetHost(this);
        CheckReCreateTile();
    }
    public void CheckReCreateTile()
    {
        if (_shape.CurrentShape == null || _shape.CurrentShape.Count <= 0)
        {
            int colorIndex = Random.Range(0, _tileColors.Count);
            _shape.RequestNewShape(_shapeData, _tileColors[colorIndex]);
        }
    }
}
