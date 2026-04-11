using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileEditorManager : MonoBehaviour
{
    [SerializeField] private LevelEditorManager _levelEditorManager;
    [Header("Setup Board")]
    [SerializeField] private int _columnsCount = 8;
    [SerializeField] private int _rowsCount = 8;
    [SerializeField] private float _squaresGap = 0.0f;      // khoang canh lon giua 3 hang/cot
    [SerializeField] private TileEditor _gridSquare;
    [SerializeField] private Vector2 _startPosition = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 _squareScale;
    [SerializeField] private float _everySquareOffset = 0.05f;  // khoang cach giua moi TileEditor

    [Header("Score")]
    public List<TileEditor> rowsCanScore = new();
    public List<TileEditor> columnsCanScore = new();
    public List<TileEditor> allCanScore = new();

    private Dictionary<int, List<TileEditor>> _rows = new();
    private Dictionary<int, List<TileEditor>> _columns = new();

    [Header("Level")]
    [SerializeField] private LevelEditorSO _levelEditorSO;

    [Header("Dev")]
    [SerializeField] private List<RectTransform> _lineColumn;
    [SerializeField] private List<RectTransform> _lineRow;
    [SerializeField] private float[] _lineColumnPositions;
    [SerializeField] private float[] _lineRowPosition;
    private List<TileEditor> _gridSquares = new List<TileEditor>();
    public TileEditor[,] Board;

    private bool _canPlace;

    public LevelEditorSO LevelEditorSO { get => _levelEditorSO; set => _levelEditorSO = value; }

    void Awake()
    {
        _everySquareOffset = GameConfig.Ins.GameplayConfig.EverySquareOffset;
        StartGame();
    }
    void OnEnable()
    {
        GameEvents.CheckIfShapeCanPlaced += OnPlaceOnGrid;
    }
    void OnDisable()
    {
        GameEvents.CheckIfShapeCanPlaced -= OnPlaceOnGrid;
    }
    void Update()
    {
        CheckHover();
    }

    #region Start Level
    public void SaveToLevelSO(LevelEditorSO levelSO)
    {
        levelSO.SetSize(_rowsCount, _columnsCount);

        for (int i = 0; i < _rowsCount; i++)
        {
            for (int j = 0; j < _columnsCount; j++)
            {
                levelSO[i, j].Occupied = Board[i, j].SquareOccupied;
                levelSO[i, j].Sprite = Board[i, j].SquareOccupied ? Board[i, j].GetSpriteActive() : null;
            }
        }
    }

#if UNITY_EDITOR
    public int LoadLevelByIndex(int level)
    {
        string path = $"Assets/_Game/LevelEditor/ScriptableObjects/Level_{level}.asset";
        LevelEditorSO levelSO = AssetDatabase.LoadAssetAtPath<LevelEditorSO>(path);

        if (levelSO == null)
        {
            Debug.LogWarning($"❌ Level not found: {path}");
            return -1;
        }

        Debug.Log($"✅ Loaded level: {path}");

        LevelEditorSO = levelSO;
        return levelSO.ScoreTarget;
    }
#endif
    private void SetRowColumn()    // Just Load Scriptable Object and assign row and column Count
    {
        //@TODO: Load Level
        if (LevelEditorSO)
        {
            _rowsCount = LevelEditorSO.RowsCount;
            _columnsCount = LevelEditorSO.ColumnsCount;
        }
        else
        {
            _rowsCount = 8;
            _columnsCount = 8;
        }
        _levelEditorManager.CurrentRowCount = _rowsCount;
        _levelEditorManager.CurrentColumnCount = _columnsCount;
        // @TODO: Add Target Score
    }
    private void StartGame()
    {
        SetRowColumn();
        ClearList();
        InitList();     // Init Board, lineColumnPos, lineRowPos, lineColumn, lineRow, _rows, _columns
        _levelEditorManager.TileManager = this;
        CreateBoard();
        _levelEditorManager.RestartGame();
    }
    public void RestartLevel()
    {
        ClearList();
        foreach (var tile in Board)
        {
            tile.ResetSquare();
        }
        _levelEditorManager.RestartGame();
        LoadInitBoard();
    }
    private void NextLevel()
    {
        transform.RemoveAllChildren();
        StartGame();
    }
    private void InitList()
    {
        Board = new TileEditor[_rowsCount, _columnsCount];
        _lineColumnPositions = new float[_columnsCount + 1];
        _lineRowPosition = new float[_rowsCount + 1];
        _lineColumn = new();
        _lineRow = new();
        _rows = new();
        _columns = new();
    }

    #endregion

    #region Create board

    private void CreateBoard()
    {
        CalculateTileScale();       // Calculate scale of each square
        SpawnGridSquare();          // Spawn
        SetGridSquaresPositions();  // Set pos for each square
        SetupRowColumn();           // Set Dictionary for each rows and columns
        SetPosForRowAndColumn();    // Set pos foreach row and column limited by 2 head / tail (8x8 -> this have 9x9 pos)
        LoadInitBoard();             // Load Board from Level
    }
    private void CalculateTileScale()
    {
        RectTransform gridRT = GetComponent<RectTransform>();
        float totalWidth = gridRT.rect.width;
        float totalHeight = gridRT.rect.height;

        int gapCols = (_columnsCount - 1) / 3;
        int gapRows = (_rowsCount - 1) / 3;

        // Lấy tile gốc
        RectTransform tileRT = _gridSquare.GetComponent<RectTransform>();

        float rawTileWidth =
            (totalWidth - (_columnsCount - 1) * _everySquareOffset - gapCols * _squaresGap)
            / _columnsCount;

        float rawTileHeight =
            (totalHeight - (_rowsCount - 1) * _everySquareOffset - gapRows * _squaresGap)
            / _rowsCount;

        float scaleX = rawTileWidth / tileRT.rect.width;
        float scaleY = rawTileHeight / tileRT.rect.height;

        _squareScale = new Vector2(scaleX, scaleY);
        if (_levelEditorManager)
        {
            _levelEditorManager.ScaleSquare = _squareScale;
        }
    }
    private void SpawnGridSquare()
    {
        // 0,1,2,3,4
        // 5,6,7,8,9

        int square_index = 0;
        for (var row = _rowsCount - 1; row >= 0; row--)
        {
            for (var column = 0; column < _columnsCount; column++)
            {
                var grid = Instantiate(_gridSquare);
                _gridSquares.Add(grid);
                Board[row, column] = grid;
                grid.transform.SetParent(this.transform);
                grid.gameObject.name = "Tile_" + row + "_" + column;
                grid.transform.localScale = new Vector2(_squareScale.x, _squareScale.y);
                grid.GetComponent<TileEditor>().SetImage(square_index % 2 == 0);
                grid.ResetSquare();
                square_index++;

                if (column == 0)
                {
                    _lineRow.Add(grid.GetComponent<RectTransform>());
                }
                if (row == 0)
                {
                    _lineColumn.Add(grid.GetComponent<RectTransform>());
                }
            }
        }
        _lineRow.Reverse();
    }
    private void SetGridSquaresPositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        Vector2 offset = new Vector2(0.0f, 0.0f);
        offset.x = square_rect.rect.width * square_rect.transform.localScale.x + _everySquareOffset;
        offset.y = square_rect.rect.height * square_rect.transform.localScale.y + _everySquareOffset;

        // ✅ TÍNH TỔNG CHIỀU CAO GRID
        float totalHeight =
            (_rowsCount - 1) * offset.y +
            ((_rowsCount - 1) / 3) * _squaresGap;

        // ✅ TÍNH TỔNG CHIỀU RỘNG GRID
        float totalWidth =
            (_columnsCount - 1) * offset.x +
            ((_columnsCount - 1) / 3) * _squaresGap;

        foreach (TileEditor square in _gridSquares)
        {
            if (column_number + 1 > _columnsCount)
            {
                square_gap_number.x = 0;
                column_number = 0;
                row_number++;
                row_moved = true;
            }

            float pos_x_offset = offset.x * column_number + (square_gap_number.x * _squaresGap);
            float pos_y_offset = offset.y * row_number + (square_gap_number.y * _squaresGap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += _squaresGap;
            }

            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += _squaresGap;
            }

            // ✅ CĂN GIỮA CẢ X + Y
            float centeredX = _startPosition.x + pos_x_offset - totalWidth / 2f;
            float centeredY = _startPosition.y + pos_y_offset - totalHeight / 2f;

            RectTransform rect = square.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(centeredX, centeredY);
            rect.localPosition = new Vector3(centeredX, centeredY, 0.0f);

            column_number++;
        }
    }
    private void SetupRowColumn()
    {
        for (int i = 0; i < _rowsCount; i++)
        {
            List<TileEditor> rows = new();
            for (int j = 0; j < _columnsCount; j++)
            {
                rows.Add(Board[i, j]);
            }
            _rows.Add(i, rows);
        }

        for (int j = 0; j < _columnsCount; j++)
        {
            List<TileEditor> columns = new();
            for (int i = 0; i < _rowsCount; i++)
            {
                columns.Add(Board[i, j]);
            }
            _columns.Add(j, columns);
        }
    }
    private void SetPosForRowAndColumn()
    {
        if (_lineColumn == null || _lineColumn.Count <= 0 ||
            _lineRow == null || _lineRow.Count <= 0) return;

        // ========== COLUMN ==========
        Vector3 colWorld = _lineColumn[0].position;
        Vector3 colLocal = transform.InverseTransformPoint(colWorld);

        _lineColumnPositions[0] =
            colLocal.x - (_lineColumn[0].rect.width * _squareScale.x * 0.5f);

        for (int i = 1; i <= _lineColumn.Count; i++)
        {
            _lineColumnPositions[i] =
                _lineColumnPositions[i - 1] +
                (_lineColumn[i - 1].rect.width * _squareScale.x);
        }

        // ========== ROW ==========
        Vector3 rowWorld = _lineRow[0].position;
        Vector3 rowLocal = transform.InverseTransformPoint(rowWorld);

        _lineRowPosition[0] =
            rowLocal.y + (_lineRow[0].rect.height * _squareScale.y * 0.5f);

        for (int i = 1; i <= _lineRow.Count; i++)
        {
            _lineRowPosition[i] =
                _lineRowPosition[i - 1] -
                (_lineRow[i - 1].rect.height * _squareScale.y);
        }
    }
    private void LoadInitBoard()
    {
        if (LevelEditorSO == null) return;
        for (int i = 0; i < _rowsCount; i++)
        {
            for (int j = 0; j < _columnsCount; j++)
            {
                if (LevelEditorSO.GetCell(i, j).Occupied == true)
                {
                    Board[i, j].ActivateSquare(LevelEditorSO.GetCell(i, j).Sprite);
                }
            }
        }
    }

    #endregion

    #region CheckCanPlace
    private List<TileAddress> GetTilesAddressDic(int[,] tilesMatrix)
    {
        List<TileAddress> temp = new();
        for (int i = 0; i < tilesMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < tilesMatrix.GetLength(1); j++)
            {
                if (tilesMatrix[i, j] == 1)
                {
                    temp.Add(new TileAddress(i, j));
                }
            }
        }
        return temp;
    }
    public int GetIndex(ShapeSquare tile, string option)
    {
        RectTransform tileRect = tile.GetComponent<RectTransform>();
        if (tileRect == null) return -1;
        Vector3 tileWorld = tileRect.position;
        Vector3 tileLocal = transform.InverseTransformPoint(tileWorld);
        switch (option)
        {
            case "Column":
                float tileX = tileLocal.x;

                if (tileX < _lineColumnPositions[0] ||
                    tileX > _lineColumnPositions[^1]) return -1;

                for (int i = 0; i < _lineColumnPositions.Length - 1; i++)
                {
                    if (tileX >= _lineColumnPositions[i] &&
                        tileX < _lineColumnPositions[i + 1])
                    {
                        return i;
                    }
                }
                return -1;

            case "Row":
                float tileY = tileLocal.y;

                if (tileY > _lineRowPosition[0] ||
                    tileY < _lineRowPosition[^1]) return -1;

                for (int j = 0; j < _lineRowPosition.Length - 1; j++)
                {
                    if (tileY <= _lineRowPosition[j] &&
                        tileY > _lineRowPosition[j + 1])
                    {
                        return j;
                    }
                }
                return -1;

            default:
                Debug.Log("Wrong type option!");
                return -1;
        }
    }
    public bool CanPlaceInGrid(int[,] tilesMatrix)
    {
        // Debug.Log("Check CanPlace");
        if (tilesMatrix == null) return false;

        var listAddress = GetTilesAddressDic(tilesMatrix);

        int boardRows = Board.GetLength(0);
        int boardCols = Board.GetLength(1);

        for (int baseRow = 0; baseRow < boardRows; baseRow++)
        {
            for (int baseCol = 0; baseCol < boardCols; baseCol++)
            {
                bool canPlaceHere = true;

                for (int k = 0; k < listAddress.Count; k++)
                {
                    int r = baseRow + listAddress[k].rowIndex;
                    int c = baseCol + listAddress[k].columnIndex;

                    // ❌ VƯỢT BIÊN → FAIL NGAY
                    if (r < 0 || r >= boardRows || c < 0 || c >= boardCols)
                    {
                        canPlaceHere = false;
                        break;
                    }

                    // ❌ ĐÈ Ô ĐÃ CHIẾM → FAIL
                    if (Board[r, c].SquareOccupied)
                    {
                        canPlaceHere = false;
                        break;
                    }
                }

                // ✅ TÌM ĐƯỢC 1 VỊ TRÍ HỢP LỆ
                if (canPlaceHere)
                    return true;
            }
        }

        return false;
    }

    public bool CanPlaceByShapeData(ShapeData shapeData)
    {
        if (shapeData == null) return false;
        int[,] matrix = new int[shapeData.rows, shapeData.columns];

        for (int r = 0; r < shapeData.rows; r++)
        {
            for (int c = 0; c < shapeData.columns; c++)
            {
                matrix[r, c] = shapeData[r, c] ? 1 : 0;
                // Debug.Log(r + "," + c + ":" + matrix[r, c]);
            }
        }
        return CanPlaceInGrid(matrix);
    }

    public bool CanPlace(List<ShapeSquare> listTile)
    {
        if (listTile.Count == 0) return false;
        int count = 0;
        var listIndex = new List<TileAddress>();
        foreach (var tile in listTile)
        {
            int indexRow = GetIndex(tile, "Row");
            int indexColumn = GetIndex(tile, "Column");
            int duplicateCount = 0;
            foreach (var index in listIndex)
            {
                if (index.rowIndex == indexRow && index.columnIndex == indexColumn) duplicateCount++;

            }
            if (duplicateCount == 0)
            {
                listIndex.Add((new TileAddress(indexRow, indexColumn)));
                if (indexRow >= 0 && indexRow < _rowsCount && indexColumn >= 0 && indexColumn < _columnsCount) if (!Board[indexRow, indexColumn].SquareOccupied) count++;
            }
        }
        return (count == listTile.Count);
    }
    #endregion

    #region Update
    public void CheckHover()
    {
        if (_levelEditorManager.DraggingShape == null || _levelEditorManager.DraggingShape.CurrentShape == null) return;
        var listTile = _levelEditorManager.DraggingShape.CurrentShape;
        _canPlace = CanPlace(listTile);
        if (_canPlace)
        {
            var listNowShadow = new List<TileEditor>();
            foreach (var square in listTile)
            {
                int indexRow = GetIndex(square, "Row");
                int indexColumn = GetIndex(square, "Column");
                if (indexRow >= 0 && indexRow < _rowsCount && indexColumn >= 0 && indexColumn < _columnsCount)
                {
                    square.RowIndex = indexRow;
                    square.ColumnIndex = indexColumn;
                    if (square.GoalItemType != GoalItemType.None)
                    {
                        Board[indexRow, indexColumn].SetHover(square.GetCurrentSprite(), true);
                    }
                    else
                    {
                        Board[indexRow, indexColumn].SetHover(_levelEditorManager.DraggingShape.CurrentTileColor.Tile, true);
                    }
                    listNowShadow.Add(Board[indexRow, indexColumn]);
                }
            }
            foreach (var cell in Board)
            {
                if (!listNowShadow.Exists(x => x == cell))
                {
                    cell.SetHover(false);
                }
            }
        }
        else foreach (var cell in Board) cell.SetHover(false);
    }
    #endregion
    #region OnPlaceOnGrid
    public void OnPlaceOnGrid()
    {
        if (_levelEditorManager.DraggingShape == null)
        {
            Debug.Log("Null??");
            return;
        }
        var listTile = _levelEditorManager.DraggingShape.CurrentShape;
        if (_canPlace)
        {
            foreach (var tile in listTile)
            {
                int indexRow = GetIndex(tile, "Row");
                int indexColumn = GetIndex(tile, "Column");
                if (indexRow >= 0 && indexRow < _rowsCount && indexColumn >= 0 && indexColumn < _columnsCount)
                {
                    if (tile.GoalItemType == GoalItemType.None)
                    {
                        Board[indexRow, indexColumn].ActivateSquare(_levelEditorManager.DraggingShape.CurrentTileColor, false);
                    }
                    else
                    {
                        Board[indexRow, indexColumn].ActivateSquareGoalItem(tile.GoalItemType);
                    }
                    // _levelEditorManager.EffectManager.RequireLikeEffect(_levelEditorManager.DraggingShape.CurrentTileColor,  Board[indexRow, indexColumn].transform.position);
                }
            }

            // Return Dragging to null and Check to Spawn Shape
            _levelEditorManager.OnPlaceShapeSuccess();
        }
        else
        {
            // _levelEditorManager.DraggingShape.BackToPos();
        }
    }
    #endregion

    private void ClearList()
    {
        rowsCanScore.Clear();
        columnsCanScore.Clear();
        allCanScore.Clear();
    }

    #region Helper
    public void ApplyLevelFromJson(LevelJsonData data)
    {
        _rowsCount = 8;
        _columnsCount = 8;

        int index = 0;
        for (int row = 0; row < _rowsCount; row++)
        {
            for (int col = 0; col < _columnsCount; col++)
            {
                TileJsonData tile = data.tiles[index++];
                TileEditor boardTile = Board[row, col];

                boardTile.ResetSquare();
                boardTile.GoalItemType = (GoalItemType)tile.goalItemType;

                if (tile.occupied)
                {
                    if (boardTile.GoalItemType == GoalItemType.None)
                    {
                        TileColor tileColor = GameConfig.Ins.TileColorConfig.ColorSprites[tile.colorIndex];
                        boardTile.ActivateSquare(tileColor);
                    }
                    else
                    {
                        boardTile.ActivateSquareGoalItem(boardTile.GoalItemType);
                    }
                }
            }
        }
    }

    #endregion

}
