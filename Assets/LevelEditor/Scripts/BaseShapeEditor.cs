using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseShapeEditor : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    public LevelEditorManager _levelEditorManager;
    [SerializeField] private ShapeSquare _squareShapeImage;

    [HideInInspector]
    public ShapeData CurrentShapeData;

    private List<ShapeSquare> _currentShape = new();
    private Sprite _currentSprite;
    public List<ShapeSquare> CurrentShape { get => _currentShape; set => _currentShape = value; }
    public Sprite CurrentSprite { get => _currentSprite; set => _currentSprite = value; }

    #region Create Shape
    public void RequestNewShape(ShapeData shapeData)
    {
        CreateShape(shapeData);
    }
    public void RequestNewShape(ShapeData shapeData, Sprite sprite)
    {
        CreateShape(shapeData);
        SetSpriteShape(sprite);
    }
    public void CreateShape(ShapeData shapeData)
    {
        if (shapeData == null || shapeData.GetCountNode() <= 0)
        {
            Debug.LogError("data error");
            return;
        }
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

        float cellWidth = GetComponent<RectTransform>().rect.width / shapeData.columns;
        float cellHeight = GetComponent<RectTransform>().rect.height / shapeData.rows;
        float realSize = Mathf.Min(cellHeight, cellWidth);
        foreach (var square in CurrentShape)
        {
            var rect = square.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(realSize, realSize);
        }

        var squareRect = CurrentShape[0].GetComponent<RectTransform>();
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
    private void SetSpriteShape(Sprite sprite)
    {
        CurrentSprite = sprite;
        if (CurrentShape == null) return;
        foreach (var square in CurrentShape)
        {
            square.SetSprite(sprite);
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

    #region Event Mouse
    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentShapeData == null)
        {
            Debug.LogError("Why null?");
            return;
        }
        LevelEditorManager.OnClickBaseShape?.Invoke(CurrentShapeData);
        Debug.Log("Click " + CurrentShapeData.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }


    #endregion

}


