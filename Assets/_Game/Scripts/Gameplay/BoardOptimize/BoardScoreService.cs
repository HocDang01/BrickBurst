using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardScoreService
{
    [Header("Score")]
    public List<Tile> rowsCanScore = new();
    public List<Tile> columnsCanScore = new();
    public List<Tile> allCanScore = new();
    private BoardContext _boardContext;

    public BoardScoreService(BoardContext boardContext)
    {
        _boardContext = boardContext;
        rowsCanScore = new();
        columnsCanScore = new();
        allCanScore = new();
    }
    public void TryPlayScoreEffect()
    {
        if (allCanScore == null || allCanScore.Count <= 0) return;
        int row = rowsCanScore.Count / _boardContext.RowsCount;
        int column = columnsCanScore.Count / _boardContext.ColumnsCount;
        foreach (Tile tile in allCanScore)
        {
            if (row + column >= GameConfig.Ins.GameplayConfig.CountStartRainbow)
            {
                EffectManager.Ins.RequireBreakTile(tile.GetSpriteVirtual(), tile.transform.position);
            }
            // else
            // {
            //     EffectManager.Ins.RequireBreakTile(GameplayManager.Ins.DraggingShape.CurrentTileColor, tile.transform.position);
            // }
            tile.ResetSquare();
        }
        if (row + column < GameConfig.Ins.GameplayConfig.CountStartRainbow)
        {
            if (row > 0)
            {
                var rowCenters = GetRowCenters();
                foreach (var pos in rowCenters)
                {
                    EffectManager.Ins.PlayRowClearEffect(GameplayManager.Ins.DraggingShape.CurrentTileColor, pos);
                }
            }
            if (column > 0)
            {
                var columnCenters = GetColumnCenters();
                foreach (var pos in columnCenters)
                {
                    EffectManager.Ins.PlayColumnClearEffect(GameplayManager.Ins.DraggingShape.CurrentTileColor, pos);
                }
            }
        }
    }
    public Vector3 GetCenterPosition(List<Tile> tiles)
    {
        Vector3 sum = Vector3.zero;
        foreach (var t in tiles)
            sum += t.transform.position;

        return sum / tiles.Count;
    }
    public List<Vector3> GetRowCenters()
    {
        return rowsCanScore
            .GroupBy(t => t.RowIndex)
            .Select(g => GetCenterPosition(g.ToList()))
            .ToList();
    }
    public List<Vector3> GetColumnCenters()
    {
        return columnsCanScore
            .GroupBy(t => t.ColumnIndex)
            .Select(g => GetCenterPosition(g.ToList()))
            .ToList();
    }
    public List<Tile> GetScoredList(List<Tile>[] dic)
    {
        var returnList = new List<Tile>();
        for (int i = 0; i < dic.Length; i++)
        {
            List<Tile> temp = dic[i];
            int count = temp.Count(cell => cell.SquareOccupied || cell.IsPlacedVirtual);
            if (count == _boardContext.RowsCount) returnList.AddRange(temp);
        }
        return returnList;
    }
    public void ClearList()
    {
        rowsCanScore.Clear();
        columnsCanScore.Clear();
        allCanScore.Clear();
    }
    public List<Tile> GetAllScoredList()
    {
        var temp = new List<Tile>();
        foreach (var row in rowsCanScore) if (!temp.Contains(row)) temp.Add(row);
        foreach (var column in columnsCanScore) if (!temp.Contains(column)) temp.Add(column);
        return temp;
    }
}
