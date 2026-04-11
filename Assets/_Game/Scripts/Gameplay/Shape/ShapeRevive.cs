using System.Collections.Generic;
using UnityEngine;

public class ShapeRevive : MonoBehaviour
{
    [SerializeField] private ShapeSquare _squareShapeImage;
    [SerializeField] private Vector2 _shapeSelectedScale;
    [SerializeField] private Vector2 offset = new Vector2(0f, 0);

    // [HideInInspector]
    public ShapeData CurrentShapeData;

    private List<ShapeSquare> _currentShape = new();
    private TileColor _currentTileColor;
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private Vector3 _initPos;
    public List<ShapeSquare> CurrentShape { get => _currentShape; set => _currentShape = value; }
    public TileColor CurrentTileColor { get => _currentTileColor; set => _currentTileColor = value; }
    public GoalItemType GoalItemType;

    void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _initPos = transform.localPosition;
        GoalItemType = GoalItemType.None;
    }

    #region Create Shape
    public void RequestNewShape(ShapeData shapeData)
    {
        CreateShape(shapeData);
    }
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor)
    {
        CreateShape(shapeData);
        SetTileColor(tileColor);
    }
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor, GoalItemType goalItemType)
    {
        GoalItemType = goalItemType;
        RequestNewShape(shapeData, tileColor);
        SetGoalItem(goalItemType);
    }
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor, GoalItemType goalItemType, List<int> itemIndex)
    {
        GoalItemType = goalItemType;
        RequestNewShape(shapeData, tileColor);
        SetGoalItem(goalItemType, itemIndex);
    }
    private void SetGoalItem(GoalItemType goalItemType)
    {
        if (CurrentShape == null || CurrentShape.Count <= 0 || goalItemType == GoalItemType.None)
            return;

        int count = Random.Range(1, CurrentShape.Count + 1);

        // 1. Create a copy list
        List<ShapeSquare> shuffled = new List<ShapeSquare>(CurrentShape);

        // 2. Shuffle
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rnd = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rnd]) = (shuffled[rnd], shuffled[i]);
        }

        // 3. Take exactly 'count' items
        for (int i = 0; i < count; i++)
        {
            shuffled[i].SetGoalItem(goalItemType);
        }
    }
    private void SetGoalItem(GoalItemType goalItemType, List<int> itemIndex)
    {
        if (CurrentShape == null || CurrentShape.Count <= 0 || goalItemType == GoalItemType.None)
            return;
        if (itemIndex.Count > CurrentShape.Count) return;
        for (int i = 0; i < itemIndex.Count; i++)
        {
            CurrentShape[itemIndex[i]].SetGoalItem(goalItemType);
        }
    }
    private void CreateShape(ShapeData shapeData)
    {
        _shapeSelectedScale = BoardManager.Ins.SquareScale;
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
        float tileWidth = squareRect.rect.width * squareRect.localScale.x;
        float tileHeight = squareRect.rect.height * squareRect.localScale.y;

        // SPACING
        float spacing = GameConfig.Ins.GameplayConfig.EverySquareOffset;

        float moveX = tileWidth + spacing;
        float moveY = tileHeight + spacing;

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
    private void SetTileColor(TileColor tileColor, GoalItemType goalItemType = GoalItemType.None)
    {
        CurrentTileColor = tileColor;
        if (CurrentShape == null) return;
        for (int i = 0; i < CurrentShape.Count; i++)
        {
            CurrentShape[i].SetSprite(CurrentTileColor.Tile);
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
    public List<int> GetGoalItemIndex()
    {
        List<int> indexList = new();
        for (int i = 0; i < CurrentShape.Count; i++)
        {
            if (CurrentShape[i].GoalItemType != GoalItemType.None)
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }
    #endregion

    #region SetPos
    public void DestroyShape()
    {
        GoalItemType = GoalItemType.None;
        CurrentShape.Clear();
        transform.RemoveAllChildren();
        CurrentShapeData = null;
        _transform.localScale = _shapeStartScale;
        transform.localPosition = _initPos;
    }
    #endregion
}
