using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardBuilder : MonoBehaviour
{
    [SerializeField] protected Tile _gridSquare;
    [SerializeField] protected Vector2 _startPosition = new Vector2(0.0f, 0.0f);
    public Vector2 _squareScale;

    [Header("Dev")]
    [SerializeField] protected List<RectTransform> _lineColumn;
    [SerializeField] protected List<RectTransform> _lineRow;
    protected float _squaresGap = 0.0f;
    protected float _everySquareOffset = 0.5f;  // khoang cach giua moi tile
    protected List<Tile> _gridSquares = new List<Tile>();

    private BoardContext _boardContext;

    public void Init(BoardContext boardContext)
    {
        _boardContext = boardContext;
        _lineColumn = new();
        _lineRow = new();
        _everySquareOffset = GameConfig.Ins.GameplayConfig.EverySquareOffset;
        CreateBoard();
    }

    #region Create board

    protected void CreateBoard()
    {
        CalculateTileScale();       // Calculate scale of each square
        SpawnGridSquare();          // Spawn
        SetGridSquaresPositions();  // Set pos for each square
        SetupRowColumn();           // Set Dictionary for each rows and columns
        SetPosForRowAndColumn();    // Set pos foreach row and column limited by 2 head / tail (8x8 -> this have 9x9 pos)
                                    // SpawnOutlineEffects();      // Outline for hightlight when hover
    }
    protected void CalculateTileScale()
    {
        RectTransform gridRT = GetComponent<RectTransform>();
        float totalWidth = gridRT.rect.width;
        float totalHeight = gridRT.rect.height;

        int gapCols = (_boardContext.ColumnsCount - 1) / 3;
        int gapRows = (_boardContext.RowsCount - 1) / 3;

        // Lấy tile gốc
        RectTransform tileRT = _gridSquare.GetComponent<RectTransform>();

        float rawTileWidth =
            (totalWidth - (_boardContext.ColumnsCount - 1) * _everySquareOffset - gapCols * _squaresGap)
            / _boardContext.ColumnsCount;

        float rawTileHeight =
            (totalHeight - (_boardContext.RowsCount - 1) * _everySquareOffset - gapRows * _squaresGap)
            / _boardContext.RowsCount;

        float scaleX = rawTileWidth / tileRT.rect.width;
        float scaleY = rawTileHeight / tileRT.rect.height;

        _squareScale = new Vector2(scaleX, scaleY);
        // if (GameplayManager.Ins)
        // {
        //     GameplayManager.Ins.ScaleSquare = _squareScale;
        // }
    }
    protected void SpawnGridSquare()
    {
        // 0,1,2,3,4
        // 5,6,7,8,9

        int square_index = 0;
        for (var row = _boardContext.RowsCount - 1; row >= 0; row--)
        {
            for (var column = 0; column < _boardContext.ColumnsCount; column++)
            {
                var grid = Instantiate(_gridSquare);
                _gridSquares.Add(grid);
                _boardContext.Board[row, column] = grid;
                grid.transform.SetParent(this.transform);
                grid.gameObject.name = "Tile_" + row + "_" + column;
                grid.transform.localScale = new Vector2(_squareScale.x, _squareScale.y);
                grid.GetComponent<Tile>().SetImage(square_index % 2 == 0);
                grid.ResetSquare();
                square_index++;

                grid.RowIndex = row;
                grid.ColumnIndex = column;

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
    protected void SetGridSquaresPositions()
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
            (_boardContext.RowsCount - 1) * offset.y +
            ((_boardContext.RowsCount - 1) / 3) * _squaresGap;

        // ✅ TÍNH TỔNG CHIỀU RỘNG GRID
        float totalWidth =
            (_boardContext.ColumnsCount - 1) * offset.x +
            ((_boardContext.ColumnsCount - 1) / 3) * _squaresGap;

        foreach (Tile square in _gridSquares)
        {
            if (column_number + 1 > _boardContext.ColumnsCount)
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
    protected void SetupRowColumn()
    {
        for (int i = 0; i < _boardContext.RowsCount; i++)
        {
            List<Tile> rows = new();
            for (int j = 0; j < _boardContext.ColumnsCount; j++)
            {
                rows.Add(_boardContext.Board[i, j]);
            }
            _boardContext.Rows[i] = rows;
        }

        for (int j = 0; j < _boardContext.ColumnsCount; j++)
        {
            List<Tile> columns = new();
            for (int i = 0; i < _boardContext.RowsCount; i++)
            {
                columns.Add(_boardContext.Board[i, j]);
            }
            _boardContext.Columns[j] = columns;
        }
    }
    protected void SetPosForRowAndColumn()
    {
        if (_lineColumn == null || _lineColumn.Count <= 0 ||
            _lineRow == null || _lineRow.Count <= 0) return;

        float stepX =
            _lineColumn[0].rect.width * _squareScale.x +
            _everySquareOffset;

        float stepY =
            _lineRow[0].rect.height * _squareScale.y +
            _everySquareOffset;

        // ========== COLUMN ==========
        Vector3 colWorld = _lineColumn[0].position;
        Vector3 colLocal = transform.InverseTransformPoint(colWorld);

        _boardContext.LineColumnPositions[0] =
            colLocal.x - stepX * 0.5f;

        for (int i = 1; i <= _boardContext.ColumnsCount; i++)
        {
            _boardContext.LineColumnPositions[i] =
                _boardContext.LineColumnPositions[i - 1] + stepX;
        }

        // ========== ROW ==========
        Vector3 rowWorld = _lineRow[0].position;
        Vector3 rowLocal = transform.InverseTransformPoint(rowWorld);

        _boardContext.LineRowPositions[0] =
            rowLocal.y + stepY * 0.5f;

        for (int i = 1; i <= _boardContext.RowsCount; i++)
        {
            _boardContext.LineRowPositions[i] =
                _boardContext.LineRowPositions[i - 1] - stepY;
        }
    }

    #endregion

}
