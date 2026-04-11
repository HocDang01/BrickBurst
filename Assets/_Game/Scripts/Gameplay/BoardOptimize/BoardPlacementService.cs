using System.Collections.Generic;
using UnityEngine;
public class BoardPlacementService
{
    private BoardContext _boardContext;
    private BoardManager _boardManager;
    public BoardPlacementService(BoardContext boardContext, BoardManager boardManager)
    {
        _boardContext = boardContext;
        _boardManager = boardManager;
    }


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
        Vector3 tileLocal = _boardManager.transform.InverseTransformPoint(tileWorld);
        switch (option)
        {
            case "Column":
                float tileX = tileLocal.x;

                if (tileX < _boardContext.LineColumnPositions[0] ||
                    tileX > _boardContext.LineColumnPositions[^1]) return -1;

                for (int i = 0; i < _boardContext.LineColumnPositions.Length - 1; i++)
                {
                    if (tileX >= _boardContext.LineColumnPositions[i] &&
                        tileX < _boardContext.LineColumnPositions[i + 1])
                    {
                        return i;
                    }
                }
                return -1;

            case "Row":
                float tileY = tileLocal.y;

                if (tileY > _boardContext.LineRowPositions[0] ||
                    tileY < _boardContext.LineRowPositions[^1]) return -1;

                for (int j = 0; j < _boardContext.LineRowPositions.Length - 1; j++)
                {
                    if (tileY <= _boardContext.LineRowPositions[j] &&
                        tileY > _boardContext.LineRowPositions[j + 1])
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
    public int GetIndex(Transform transform, string option)
    {
        RectTransform tileRect = transform.GetComponent<RectTransform>();
        if (tileRect == null) return -1;
        Vector3 tileWorld = tileRect.position;
        Vector3 tileLocal = _boardManager.transform.InverseTransformPoint(tileWorld);
        switch (option)
        {
            case "Column":
                float tileX = tileLocal.x;

                if (tileX < _boardContext.LineColumnPositions[0] ||
                    tileX > _boardContext.LineColumnPositions[^1]) return -1;

                for (int i = 0; i < _boardContext.LineColumnPositions.Length - 1; i++)
                {
                    if (tileX >= _boardContext.LineColumnPositions[i] &&
                        tileX < _boardContext.LineColumnPositions[i + 1])
                    {
                        return i;
                    }
                }
                return -1;

            case "Row":
                float tileY = tileLocal.y;

                if (tileY > _boardContext.LineRowPositions[0] ||
                    tileY < _boardContext.LineRowPositions[^1]) return -1;

                for (int j = 0; j < _boardContext.LineRowPositions.Length - 1; j++)
                {
                    if (tileY <= _boardContext.LineRowPositions[j] &&
                        tileY > _boardContext.LineRowPositions[j + 1])
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

        int boardRows = _boardContext.Board.GetLength(0);
        int boardCols = _boardContext.Board.GetLength(1);

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
                    if (_boardContext.Board[r, c].SquareOccupied)
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
                if (indexRow >= 0 && indexRow < _boardContext.RowsCount && indexColumn >= 0 && indexColumn < _boardContext.ColumnsCount) if (!_boardContext.Board[indexRow, indexColumn].SquareOccupied) count++;
            }
        }
        return (count == listTile.Count);
    }

    public (int, int) GetIndexRowColByTransform(Transform transform)
    {
        if (transform == null) return (-1, -1);
        int indexRow = GetIndex(transform, "Row");
        int indexColumn = GetIndex(transform, "Column");
        if (indexRow < 0 || indexRow >= _boardContext.RowsCount && indexColumn < 0 || indexColumn >= _boardContext.ColumnsCount)
            return (-1, -1);
        return (indexRow, indexColumn);
    }
    #endregion
}
