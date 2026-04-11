using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoardOutlineRenderer : MonoBehaviour
{
    [SerializeField] private Image _outlineScoreVerticalEffectPrefab;
    [SerializeField] private Image _outlineScoreHorizontalEffectPrefab;
    public BoardScoreService _boardScoreService;
    private BoardContext _boardContext;
    public void Init(BoardContext boardContext, BoardScoreService boardScoreService)
    {
        _boardContext = boardContext;
        _boardScoreService = boardScoreService;
        SpawnOutlineEffects();
    }

    #region SpawnOutLine
    public void SpawnOutlineEffects()
    {
        SpawnRowOutlines();
        SpawnColumnOutlines();
    }
    public void SpawnColumnOutlines()
    {
        for (int col = 0; col < _boardContext.ColumnsCount; col++)
        {
            Image outline = Instantiate(_outlineScoreVerticalEffectPrefab, transform);
            outline.gameObject.name = $"ColumnOutline_{col}";
            outline.gameObject.SetActive(false);

            RectTransform rt = outline.rectTransform;

            float padding = 50f;


            float left = _boardContext.LineColumnPositions[col];
            float right = _boardContext.LineColumnPositions[col + 1];

            left -= padding;
            right += padding;

            float width = right - left;
            float centerX = (left + right) * 0.5f;

            float top = _boardContext.LineRowPositions[0];
            float bottom = _boardContext.LineRowPositions[^1];

            top += padding;
            bottom -= padding;

            float height = top - bottom;

            rt.anchoredPosition = new Vector2(centerX, (top + bottom) * 0.5f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            _boardContext.ColumnOutlines[col] = outline;
        }
    }
    public void SpawnRowOutlines()
    {
        for (int row = 0; row < _boardContext.RowsCount; row++)
        {
            Image outline = Instantiate(_outlineScoreHorizontalEffectPrefab, transform);
            outline.gameObject.name = $"RowOutline_{row}";
            outline.gameObject.SetActive(false);

            RectTransform rt = outline.rectTransform;
            float padding = 50f;

            float top = _boardContext.LineRowPositions[row];
            float bottom = _boardContext.LineRowPositions[row + 1];

            top += padding;
            bottom -= padding;

            float height = top - bottom;
            float centerY = (top + bottom) * 0.5f;

            float left = _boardContext.LineColumnPositions[0];
            float right = _boardContext.LineColumnPositions[^1];

            left -= padding;
            right += padding;

            float width = right - left;

            rt.anchoredPosition = new Vector2((left + right) * 0.5f, centerY);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            _boardContext.RowOutlines[row] = outline;
        }
    }
    #endregion

    #region OutLineScore
    public void HighlightOutlineByScore(int totalRowCol)
    {
        if (GameplayManager.Ins.DraggingShape == null)
        {
            HideAllOutlines();
            return;
        }
        TileColor tileColor = GameplayManager.Ins.DraggingShape.CurrentTileColor;
        Sprite rowLight = totalRowCol >= GameConfig.Ins.GameplayConfig.CountStartRainbow ?
                                            GameConfig.Ins.TileColorConfig.RainbowOutlineHorizontal : tileColor.OutlineHorizontal;
        Sprite colLight = totalRowCol >= GameConfig.Ins.GameplayConfig.CountStartRainbow ?
                                            GameConfig.Ins.TileColorConfig.RainbowOutlineVertical : tileColor.OutlineVertical;

        // ===== ROW =====
        HashSet<int> scoredRows = new HashSet<int>();
        foreach (var tile in _boardScoreService.rowsCanScore)
            scoredRows.Add(tile.RowIndex);

        foreach (var row in scoredRows)
            ShowRowOutline(row, rowLight);

        // Hide rowOutline not score
        foreach (var kvp in _boardContext.RowOutlines)
        {
            int rowIndex = kvp.Key;
            Image img = kvp.Value;

            if (!scoredRows.Contains(rowIndex))
            {
                img.gameObject.SetActive(false);
            }
        }
        // ===== COLUMN =====
        HashSet<int> scoredColumns = new HashSet<int>();
        foreach (var tile in _boardScoreService.columnsCanScore)
            scoredColumns.Add(tile.ColumnIndex);

        foreach (var col in scoredColumns)
            ShowColumnOutline(col, colLight);


        // Hide columnOutline not score
        foreach (var kvp in _boardContext.ColumnOutlines)
        {
            int columnIndex = kvp.Key;
            Image img = kvp.Value;

            if (!scoredColumns.Contains(columnIndex))
            {
                img.gameObject.SetActive(false);
            }
        }
    }
    public void HideAllOutlines()
    {
        foreach (var r in _boardContext.RowOutlines.Values)
        {
            r.gameObject.SetActive(false);
        }

        foreach (var c in _boardContext.ColumnOutlines.Values)
        {
            c.gameObject.SetActive(false);
        }
    }
    public void ShowRowOutline(int row, Sprite sprite)
    {
        if (_boardContext.RowOutlines.TryGetValue(row, out var img))
        {
            img.gameObject.SetActive(true);
            img.sprite = sprite;
        }
    }
    public void ShowColumnOutline(int col, Sprite sprite)
    {
        if (_boardContext.ColumnOutlines.TryGetValue(col, out var img))
        {
            img.gameObject.SetActive(true);
            img.sprite = sprite;
        }
    }
    #endregion

}
