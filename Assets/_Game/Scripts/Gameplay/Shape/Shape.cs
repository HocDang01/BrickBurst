using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected ShapeSquare _squareShapeImage;
    [SerializeField] protected Vector2 _shapeSelectedScale;
    [SerializeField] protected Vector2 offset = new Vector2(0f, 0);

    // [HideInInspector]
    protected ShapeData _currentShapeData;
    // public ShapeData CurrentShapeData => CurrentShapeData1;

    protected List<ShapeSquare> _currentShape = new();
    protected Camera _cam;
    protected TileColor _currentTileColor;
    protected Vector3 _shapeStartScale;
    protected RectTransform _transform;
    protected Canvas _canvas;
    protected Vector3 _initPos;
    public List<ShapeSquare> CurrentShape { get => _currentShape; set => _currentShape = value; }
    public TileColor CurrentTileColor { get => _currentTileColor; set => _currentTileColor = value; }
    public ShapeData CurrentShapeData { get => _currentShapeData; set => _currentShapeData = value; }

    public GoalItemType GoalItemType;
    public bool HasMoney;


    protected virtual void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _initPos = transform.localPosition;
        GoalItemType = GoalItemType.None;
        HasMoney = false;

    }
    protected virtual void OnEnable()
    {
        _cam = Camera.main;
    }

    #region Create Shape
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor)
    {
        CreateShape(shapeData);
        SetTileColor(tileColor);
    }
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor, GoalItemType goalItemType)
    {
        GoalItemType = goalItemType;
        CreateShape(shapeData);
        SetTileColor(tileColor);
        SetGoalItem(goalItemType);
    }
    public void RequestNewShape(ShapeData shapeData, TileColor tileColor, GoalItemType goalItemType, List<int> itemIndex)
    {
        GoalItemType = goalItemType;
        CreateShape(shapeData);
        SetTileColor(tileColor);
        SetGoalItem(goalItemType, itemIndex);
    }
    protected void SetGoalItem(GoalItemType goalItemType)
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
    protected void SetGoalItem(GoalItemType goalItemType, List<int> itemIndex)
    {
        if (CurrentShape == null || CurrentShape.Count <= 0 || goalItemType == GoalItemType.None)
            return;
        if (itemIndex.Count > CurrentShape.Count) return;
        for (int i = 0; i < itemIndex.Count; i++)
        {
            CurrentShape[itemIndex[i]].SetGoalItem(goalItemType);
        }
    }
    protected void CreateShape(ShapeData shapeData)
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
            square.transform.localScale = _shapeSelectedScale;
        }

        var squareRect = _squareShapeImage.GetComponent<RectTransform>();
        // float tileWidth = squareRect.rect.width * squareRect.localScale.x;
        // float tileHeight = squareRect.rect.height * squareRect.localScale.y;
        float tileWidth = squareRect.rect.width * _shapeSelectedScale.x;
        float tileHeight = squareRect.rect.height * _shapeSelectedScale.y;

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
                    var rect = CurrentShape[currentIndexInList].GetComponent<RectTransform>();
                    rect.localPosition = new Vector2(x, y);

                    currentIndexInList++;
                }
            }
        }
        transform.localScale = Vector3.zero;
        transform.DOScale(_shapeStartScale, 0.2f);
    }
    protected float GetCenteredOffset(int index, int total, float move)
    {
        return (index - (total - 1) * 0.5f) * move;
    }
    protected void SetTileColor(TileColor tileColor, GoalItemType goalItemType = GoalItemType.None)
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

    protected int GetNumberOfSquare(ShapeData shapeData)
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
    public List<int> GetMoneyIndex()
    {
        List<int> indexList = new();
        for (int i = 0; i < CurrentShape.Count; i++)
        {
            if (CurrentShape[i].HasMoney)
            {
                indexList.Add(i);
            }
        }
        return indexList;
    }
    #endregion

    #region Event Mouse
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false)
        {
            return;
        }
        if (this is not ShapeBooster && BoosterManager.Ins && BoosterManager.Ins.BoosterTypeUsing == BoosterType.EraseShape)
        {
            if (CurrentShape == null || CurrentShape.Count <= 0) return;
            // SoundManager.Ins.breakTile.Play();
            var boosterManager = BoosterManager.Ins;
            boosterManager.BoosterTypeUsing = BoosterType.None;
            boosterManager.DisableEraseShape();
            boosterManager.UseBooster(BoosterType.EraseShape);

            foreach (ShapeSquare block in CurrentShape)
            {
                EffectManager.Ins.RequireBreakTile(block.GetCurrentSprite(), block.transform.position);
            }
            DestroyShape();
            GameplayManager.Ins.OnEraseShape();
            return;
        }
        // SoundManager.Ins.pickupTileFx.Play();
        // // SoundManager.Ins.liftTilesFx.Play();
        // // _transform.localScale = _shapeSelectedScale;
        // _transform.localScale = Vector3.one;

        // Vector2 pos;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     _canvas.transform as RectTransform,
        //     eventData.position,
        //     Camera.main,
        //     out pos
        // );

        // _transform.localPosition = pos + offset;
        // GameplayManager.Ins.DraggingShape = this;
        // // BackToPos();
        // Debug.Log("OnClick Shape");
        // GameEvents.CheckIfShapeCanPlaced?.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false)
        {
            return;
        }
        if (BoosterManager.Ins && BoosterManager.Ins.BoosterTypeUsing == BoosterType.EraseShape && this is not ShapeBooster)
        {
            return;
        }
        Debug.Log("OnPointerUp");
        GameEvents.CheckIfShapeCanPlaced?.Invoke();


        // _transform.DOKill();
        // _transform.DOScale(_shapeStartScale, 0.4f);
        // _transform.DOLocalMove(_initPos, 0.5f);
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false)
        {
            return;
        }
        if (GameplayManager.Ins.DraggingShape != null) return;
        if (BoosterManager.Ins && BoosterManager.Ins.BoosterTypeUsing == BoosterType.EraseShape && this is not ShapeBooster)
        {
            return;
        }
        // SoundManager.Ins.pickupTileFx.Play();

        _transform.DOKill();
        // SoundManager.Ins.liftTilesFx.Play();
        // _transform.localScale = _shapeSelectedScale;
        _transform.localScale = Vector3.one;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            Camera.main,
            out pos
        );

        _transform.localPosition = pos + offset;
        GameplayManager.Ins.DraggingShape = this;
    }


    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false)
        {
            return;
        }
        if (GameplayManager.Ins.DraggingShape != null) return;
        if (BoosterManager.Ins && BoosterManager.Ins.BoosterTypeUsing == BoosterType.EraseShape && this is not ShapeBooster)
        {
            return;
        }
        // SoundManager.Ins.liftTilesFx.Play();
        _transform.DOKill();
        // _transform.localScale = _shapeSelectedScale;
        _transform.localScale = Vector3.one;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                eventData.position, _cam, out pos);
        _transform.localPosition = pos + offset;
        GameplayManager.Ins.DraggingShape = this;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false)
        {
            return;
        }
        if (GameplayManager.Ins.DraggingShape != null && GameplayManager.Ins.DraggingShape != this) return;
        if (BoosterManager.Ins && BoosterManager.Ins.BoosterTypeUsing == BoosterType.EraseShape && this is not ShapeBooster)
        {
            return;
        }
        Vector2 pos;
        // _transform.localScale = _shapeSelectedScale;
        // _transform.localScale = Vector3.one;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                eventData.position, _cam, out pos);
        // _transform.localPosition = pos + offset;
        Vector3 targetPos = pos + offset;
        _transform.localPosition = Vector3.Lerp(
                    _transform.localPosition,
                    targetPos,
                    40f * Time.deltaTime
                );
        GameplayManager.Ins.DraggingShape = this;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false)
        {
            return;
        }
        if (GameplayManager.Ins.DraggingShape != null && GameplayManager.Ins.DraggingShape != this) return;
        if (BoosterManager.Ins && BoosterManager.Ins.BoosterTypeUsing == BoosterType.EraseShape && this is not ShapeBooster)
        {
            return;
        }
        GameEvents.CheckIfShapeCanPlaced?.Invoke();
    }
    #endregion

    #region SetPos
    public virtual void BackToPos()
    {
        GameplayManager.Ins.DraggingShape = null;
        transform.DOKill();
        transform.DOScale(_shapeStartScale, 0.4f);
        transform.DOLocalMove(_initPos, 0.5f);
        // _transform.localScale = _shapeStartScale;
        // transform.position = _initPos;
    }
    #endregion
    #region Reset
    public virtual void DestroyShape()
    {
        GoalItemType = GoalItemType.None;
        transform.DOKill();
        CurrentShape.Clear();
        transform.RemoveAllChildren();
        CurrentShapeData = null;
        _transform.localScale = _shapeStartScale;
        _transform.localPosition = _initPos;
    }
    public void BackToScale()
    {
        transform.DOKill();
        _transform.localScale = _shapeStartScale;
    }
    #endregion
}
