using System;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeLevelEditor : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public LevelEditorManager _levelEditorManager;
    [SerializeField] private ShapeSquare _squareShapeImage;
    [SerializeField] private Vector2 _shapeSelectedScale;
    [SerializeField] private Vector2 offset = new Vector2(0f, 0);

    [HideInInspector]
    public ShapeData CurrentShapeData;

    private List<ShapeSquare> _currentShape = new();
    private TileColor _tileColor;
    private Vector3 _shapeStartScale;
    private Vector3 _initPos;
    private RectTransform _transform;
    private Canvas _canvas;
    public List<ShapeSquare> CurrentShape { get => _currentShape; set => _currentShape = value; }
    public TileColor CurrentTileColor { get => _tileColor; set => _tileColor = value; }

    void Awake()
    {
        // _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _initPos = transform.position;
    }
    void Start()
    {
        transform.localScale = _levelEditorManager.ScaleSquare;
        _shapeSelectedScale = _levelEditorManager.ScaleSquare;
    }

    #region Create Shape
    public void RequestNewShape(ShapeData shapeData)
    {
        DestroyShape();
        CreateShape(shapeData);
    }
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor)
    {
        DestroyShape();
        CreateShape(shapeData);
        SetSpriteShape(tileColor);
    }
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor, GoalItemType goalItemType)
    {
        RequestNewShape(shapeData, tileColor);
        SetGoalItem(goalItemType);
    }
    private void SetGoalItem(GoalItemType goalItemType)
    {
        if (CurrentShape == null || CurrentShape.Count <= 0 || goalItemType == GoalItemType.None)
            return;

        int count = UnityEngine.Random.Range(1, CurrentShape.Count + 1);

        // 1. Create a copy list
        List<ShapeSquare> shuffled = new List<ShapeSquare>(CurrentShape);

        // 2. Shuffle
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rnd]) = (shuffled[rnd], shuffled[i]);
        }

        // 3. Take exactly 'count' items
        for (int i = 0; i < count; i++)
        {
            shuffled[i].SetGoalItem(goalItemType);
        }
    }
    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        var totalSquareNumber = GetNumberOfSquare(shapeData);

        while (CurrentShape.Count < totalSquareNumber)
        {
            CurrentShape.Add(Instantiate(_squareShapeImage, transform));
        }

        foreach (var square in CurrentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = _squareShapeImage.GetComponent<RectTransform>();
        float moveX = squareRect.rect.width * squareRect.localScale.x;
        float moveY = squareRect.rect.height * squareRect.localScale.y;

        int currentIndexInList = 0;


        //set position to form final shape
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    CurrentShape[currentIndexInList].gameObject.SetActive(true);
                    float x = GetCenteredOffset(column, shapeData.columns, moveX);
                    float y = GetCenteredOffset((shapeData.rows - 1 - row), shapeData.rows, moveY);
                    CurrentShape[currentIndexInList].GetComponent<RectTransform>().localPosition = new Vector2(x, y);

                    currentIndexInList++;
                }
            }
        }

    }
    private float GetCenteredOffset(int index, int total, float move)
    {
        return (index - (total - 1) * 0.5f) * move;
    }
    private void SetSpriteShape(TileColor tileColor)
    {
        if (tileColor == null) return;
        CurrentTileColor = tileColor;
        foreach (var square in CurrentShape)
        {
            square.SetSprite(tileColor.Tile);
        }
    }

    #endregion

    #region Helper

    private int GetNumberOfSquare(ShapeData shapeData)
    {
        int number = 0;
        for (int i = 0; i < shapeData.rows; i++)
        {
            for (int j = 0; j < shapeData.columns; j++)
            {
                if (shapeData[i, j]) number++;
            }
        }
        return number;
    }
    public int GetNumberOfSquare()
    {
        if (CurrentShapeData == null) return -1;
        int number = 0;
        foreach (var rowData in CurrentShapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active) number++;
            }
        }
        return number;
    }

    #endregion

    #region SetPos
    public void DestroyShape()
    {
        CurrentShape.Clear();
        transform.RemoveAllChildren();
        CurrentShapeData = null;
    }
    #endregion

    #region Event Mouse
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_levelEditorManager.DraggingShape != null) return;
        _transform.localScale = _shapeSelectedScale;
        _levelEditorManager.DraggingShape = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_levelEditorManager.DraggingShape != null && _levelEditorManager.DraggingShape != this) return;
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
        _levelEditorManager.DraggingShape = this;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_levelEditorManager.DraggingShape != null && _levelEditorManager.DraggingShape != this) return;
        GameEvents.CheckIfShapeCanPlaced?.Invoke();
    }
    #endregion

    #region Reset
    public void ResetShape()
    {
        DestroyShape();
    }
    public void BackToPos()
    {
        transform.position = _initPos;
    }

    public void SetColor(TileColor color)
    {
        CurrentTileColor = color;
        foreach (var square in CurrentShape)
        {
            if (square.GoalItemType != GoalItemType.None) return;
            square.SetSprite(color.Tile);
        }
    }
    #endregion
}
