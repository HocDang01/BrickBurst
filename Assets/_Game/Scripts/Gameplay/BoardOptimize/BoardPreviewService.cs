using System.Collections.Generic;
using UnityEngine;
public class BoardPreviewService
{
    public List<RainBowColor> _rainBowHorizontalColors;
    public List<RainBowColor> _rainBowVerticalColors;

    private BoardScoreService _boardScoreService;
    private BoardOutlineRenderer _boardOutlineRenderer;
    private BoardPlacementService _boardPlacementService;
    private BoardContext _boardContext;

    public BoardPreviewService(BoardContext boardContext, BoardScoreService boardScoreService, BoardOutlineRenderer boardOutlineRenderer, BoardPlacementService boardPlacementService)
    {
        _boardContext = boardContext;
        _boardScoreService = boardScoreService;
        _boardOutlineRenderer = boardOutlineRenderer;
        _boardPlacementService = boardPlacementService;
        _rainBowHorizontalColors = GameConfig.Ins.TileColorConfig.RainBowHorizontal;
        _rainBowVerticalColors = GameConfig.Ins.TileColorConfig.RainBowVertical;
    }

    #region UpdateFunc
    public void CheckHover()
    {
        if (GameplayManager.Ins.DraggingShape == null || GameplayManager.Ins.DraggingShape.CurrentShape == null)
        {
            foreach (var cell in _boardContext.Board)
            {
                cell.SetHover(false);
                cell.UnSetVirtualScore();
            }
            return;
        }
        var listSquare = GameplayManager.Ins.DraggingShape.CurrentShape;
        if (_boardPlacementService.CanPlace(listSquare))
        {
            var listNowShadow = new List<Tile>();
            foreach (var square in listSquare)
            {
                int indexRow = _boardPlacementService.GetIndex(square, "Row");
                int indexColumn = _boardPlacementService.GetIndex(square, "Column");
                if (indexRow >= 0 && indexRow < _boardContext.RowsCount && indexColumn >= 0 && indexColumn < _boardContext.ColumnsCount)
                {
                    square.RowIndex = indexRow;
                    square.ColumnIndex = indexColumn;
                    if (square.GoalItemType != GoalItemType.None)
                    {
                        _boardContext.Board[indexRow, indexColumn].SetHover(square.GetCurrentSprite(), true);
                    }
                    else
                    {
                        if (square.HasMoney)
                        {
                            _boardContext.Board[indexRow, indexColumn].SetHoverMoney();
                        }
                        else
                        {
                            _boardContext.Board[indexRow, indexColumn].SetHover(GameplayManager.Ins.DraggingShape.CurrentTileColor.Tile, true);
                        }
                    }
                    listNowShadow.Add(_boardContext.Board[indexRow, indexColumn]);
                }
            }
            foreach (var cell in _boardContext.Board)
            {
                if (!listNowShadow.Exists(x => x == cell))
                {
                    cell.SetHover(false);
                }
            }
        }
        else
        {
            foreach (var cell in _boardContext.Board) cell.SetHover(false);
            foreach (var square in listSquare)
            {
                square.RowIndex = -1;
                square.ColumnIndex = -1;
            }
        }
    }
    public void CheckVirtual()
    {
        _boardScoreService.ClearList();
        _boardScoreService.rowsCanScore = _boardScoreService.GetScoredList(_boardContext.Rows);
        _boardScoreService.columnsCanScore = _boardScoreService.GetScoredList(_boardContext.Columns);
        if (_boardScoreService.rowsCanScore.Count > 0 || _boardScoreService.columnsCanScore.Count > 0)
        {
            _boardScoreService.allCanScore = _boardScoreService.GetAllScoredList();
            _boardOutlineRenderer.HighlightOutlineByScore(_boardScoreService.rowsCanScore.Count / _boardContext.RowsCount + _boardScoreService.columnsCanScore.Count / _boardContext.ColumnsCount);
        }
        else
        {
            _boardOutlineRenderer.HideAllOutlines();
        }
    }
    public void SetVirtualScore()
    {
        if (GameplayManager.Ins.DraggingShape == null || _boardScoreService.allCanScore == null || _boardScoreService.rowsCanScore == null || _boardScoreService.columnsCanScore == null) return;
        int row = _boardScoreService.rowsCanScore.Count / _boardContext.RowsCount;
        int column = _boardScoreService.columnsCanScore.Count / _boardContext.ColumnsCount;
        if (row + column >= GameConfig.Ins.GameplayConfig.CountStartRainbow)
        {
            SetVirtualScoreRainBow();
        }
        else
        {
            SetVirtualScoreNormal();
        }
        var listShapeSquare = GameplayManager.Ins.DraggingShape.CurrentShape;
        foreach (var shapeSquare in listShapeSquare)
        {
            shapeSquare.SetUnVirtual();
            foreach (var tile in _boardScoreService.allCanScore)
            {
                if (shapeSquare.RowIndex == tile.RowIndex && shapeSquare.ColumnIndex == tile.ColumnIndex)
                {
                    // Just hightlight square if this doesn't have GoalItem
                    if (shapeSquare.GoalItemType == GoalItemType.None && !shapeSquare.HasMoney)
                    {
                        shapeSquare.SetVirtual(tile.GetSpriteVirtual());
                    }
                    else
                    {
                        tile.SetVirtualScore(shapeSquare.GetCurrentSprite());
                    }
                    break;
                }
            }
        }
    }
    private void SetVirtualScoreNormal()
    {
        // if (GameplayManager.Ins.DraggingShape == null || _boardScoreService.allCanScore == null) return;
        foreach (var tile in _boardScoreService.allCanScore)
        {
            tile.SetVirtualScore(GameplayManager.Ins.DraggingShape.CurrentTileColor);
        }
        foreach (var cell in _boardContext.Board)
        {
            if (!_boardScoreService.allCanScore.Exists(x => x == cell))
            {
                cell.UnSetVirtualScore();
            }
        }
    }
    private void SetVirtualScoreRainBow()
    {
        // if (GameplayManager.Ins.DraggingShape == null || _boardScoreService.allCanScore == null || _boardScoreService.rowsCanScore == null || _boardScoreService.columnsCanScore == null) return;
        int idxRainBow = 0;
        for (int i = 0; i < _boardScoreService.rowsCanScore.Count; i++)
        {
            _boardScoreService.rowsCanScore[i].SetVirtualScore(_rainBowHorizontalColors[idxRainBow]);
            idxRainBow = (idxRainBow + 1) % _rainBowHorizontalColors.Count;
        }
        idxRainBow = 0;
        for (int i = 0; i < _boardScoreService.columnsCanScore.Count; i++)
        {
            _boardScoreService.columnsCanScore[i].SetVirtualScore(_rainBowVerticalColors[idxRainBow]);
            idxRainBow = (idxRainBow + 1) % _rainBowHorizontalColors.Count;
        }
        foreach (var cell in _boardContext.Board)
        {
            if (!_boardScoreService.allCanScore.Exists(x => x == cell))
            {
                cell.UnSetVirtualScore();
            }
        }
    }
    #endregion
}
