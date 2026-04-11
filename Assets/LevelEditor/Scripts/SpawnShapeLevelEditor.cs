using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShapeLevelEditor : MonoBehaviour
{
    [SerializeField] private LevelEditorManager _levelEditorManager;
    [SerializeField] private BaseShapeEditor _baseShapeEditorPrefab;
    [SerializeField] private ShapeColorLevelEditor _shapeColorEditorPrefab;
    [SerializeField] private ShapeLevelEditor _shapeLevelEditor;


    [SerializeField] private ShapeData _shapeData1x1;
    [SerializeField] private Transform _baseShapeStorage;
    [SerializeField] private Transform _colorStorage;

    private List<ShapeData> _shapeDatas;
    private TileColor _tileColor;

    void Awake()
    {
        _shapeDatas = new();
        foreach (var poolShapeType in GameConfig.Ins.ShapeSpawnerConfig.PoolShapeConfigs)
        {
            if (poolShapeType.ShapeRatios == null) continue;
            foreach (var shape in poolShapeType.ShapeRatios)
            {
                if (shape != null && shape.ShapeData != null)
                {
                    _shapeDatas.Add(shape.ShapeData);
                }
            }
        }
    }

    void OnEnable()
    {
        LevelEditorManager.OnClickBaseShape += SpawnShape;
        LevelEditorManager.OnClickGoalItem += SpawnShapeItemGoal;
        LevelEditorManager.OnClickColor += TrySetColor;
    }
    void OnDisable()
    {
        LevelEditorManager.OnClickBaseShape -= SpawnShape;
        LevelEditorManager.OnClickGoalItem -= SpawnShapeItemGoal;
        LevelEditorManager.OnClickColor -= TrySetColor;
    }

    void Start()
    {
        CreateBaseShape();
        CreateColor();
    }

    private void CreateBaseShape()
    {
        var shape1x1 = Instantiate(_baseShapeEditorPrefab, _baseShapeStorage);
        shape1x1.CreateShape(_shapeData1x1);
        foreach (var data in _shapeDatas)
        {
            var baseShape = Instantiate(_baseShapeEditorPrefab, _baseShapeStorage);
            baseShape.CreateShape(data);
        }
    }
    private void CreateColor()
    {
        foreach (var tilecolor in GameConfig.Ins.TileColorConfig.ColorSprites)
        {
            if (tilecolor == null) continue;
            var color = Instantiate(_shapeColorEditorPrefab, _colorStorage);
            color.CreatColor(tilecolor);
        }
    }

    private void TrySetColor(TileColor color)
    {
        if (_levelEditorManager.DraggingShape == null) return;
        _tileColor = color;
        _levelEditorManager.DraggingShape.SetColor(color);
    }

    private void SpawnShape(ShapeData data)
    {
        if (_levelEditorManager.DraggingShape == null)
        {
            if (_tileColor != null)
            {
                _shapeLevelEditor.RequestNewShape(data, _tileColor);
            }
            else
            {
                _shapeLevelEditor.RequestNewShape(data, GameConfig.Ins.TileColorConfig.ColorSprites[0]);
            }
            _levelEditorManager.DraggingShape = _shapeLevelEditor;
        }
        else
        {
            if (_tileColor != null)
            {
                _levelEditorManager.DraggingShape.RequestNewShape(data, _tileColor);
            }
            else
            {
                _levelEditorManager.DraggingShape.RequestNewShape(data);
            }
        }
    }

    private void SpawnShapeItemGoal(GoalItemType type)
    {
        if (_levelEditorManager.DraggingShape == null)
        {
            _shapeLevelEditor.RequestNewShape(_shapeData1x1, _tileColor, type);
            _levelEditorManager.DraggingShape = _shapeLevelEditor;
        }
        else
        {
            _levelEditorManager.DraggingShape.RequestNewShape(_shapeData1x1, _tileColor, type);
        }
    }


}
