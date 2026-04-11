using System.Collections.Generic;
using DangExtension;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class BoosterManager : MonoBehaviour
{
    [SerializeField] private List<Booster> _boosters;
    [SerializeField] private List<Shape> _shapes;

    [SerializeField] private GameObject _overlay;
    [SerializeField] private Transform _initParent;
    [SerializeField] private Transform _behindOverlay;

    // public Booster BombDragging;
    private Bomb _bombDragging;
    public BoosterType BoosterTypeUsing;
    public static BoosterManager Ins;
    public Tile[,] Board => BoardManager.Ins.Board;

    public Dictionary<BoosterType, int> BoosterAmounts { get => _boosterAmounts; set => _boosterAmounts = value; }
    public Bomb BombDragging { get => _bombDragging; set => _bombDragging = value; }

    private List<Tile> _lastHoverTiles = new List<Tile>();
    private List<Image> _shapeImages;
    private int _lastRow = -1;
    private int _lastCol = -1;
    private Dictionary<BoosterType, int> _boosterAmounts;
    void Awake()
    {
        Ins = this;
        BoosterTypeUsing = BoosterType.None;
        BoosterAmounts = new();
        _shapeImages = new();
        foreach (var shape in _shapes)
        {
            _shapeImages.Add(shape.GetComponent<Image>());
        }
        _overlay.SetActive(false);
        // transform.parent = _initParent;
    }
    void Update()
    {
        if (BoosterTypeUsing == BoosterType.Bomb)
        {
            CheckHoverBomb();
        }
    }
    // public void SetBoosterAmount(MoneyAdventureData moneyAdventureData)
    // {
    //     BoosterAmounts.Clear();
    //     BoosterAmounts.Add(BoosterType.EraseShape, moneyAdventureData.EraseAmount);
    //     BoosterAmounts.Add(BoosterType.Bomb, moneyAdventureData.BombAmount);
    //     BoosterAmounts.Add(BoosterType.OneTile, moneyAdventureData.OneTileAmount);
    //     BoosterAmounts.Add(BoosterType.Reroll, moneyAdventureData.RerollAmount);
    //     UpdateBooster();
    // }
    public void SetSavedBoosterAmount()
    {
        var save = BoardSaveData.Ins;
        Dictionary<BoosterType, int> boosterAmounts = save.BoosterAmounts == null ? null : save.BoosterAmounts.Clone();
        BoosterAmounts.Clear();
        if (boosterAmounts == null || boosterAmounts.Count <= 0)
        {
            BoosterAmounts.Add(BoosterType.EraseShape, 0);
            BoosterAmounts.Add(BoosterType.Bomb, 0);
            BoosterAmounts.Add(BoosterType.OneTile, 0);
            BoosterAmounts.Add(BoosterType.Reroll, 0);
        }
        else
        {
            BoosterAmounts = boosterAmounts;
        }
        UpdateBooster();
    }

    private void UpdateBooster()
    {
        foreach (var booster in _boosters)
        {
            int amount = 0;
            if (!BoosterAmounts.ContainsKey(booster.BoosterType))
            {
                BoosterAmounts.Add(booster.BoosterType, 0);
            }
            amount = BoosterAmounts[booster.BoosterType];
            booster.UpdateAmount(amount);
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            if (GameplayManager.Ins.IsInGame)
            {
                BoardSaveData.Ins.SaveAdventure(BoardManager.Ins);
                BoardSaveData.Ins.SaveProgress();
            }
        });
    }

    public void AddBooster(BoosterType boosterType)
    {
        if (!BoosterAmounts.ContainsKey(boosterType))
        {
            BoosterAmounts.Add(boosterType, 0);
        }
        BoosterAmounts[boosterType]++;
        UpdateBooster();
    }
    public void UseBooster(BoosterType boosterType)
    {
        if (!BoosterAmounts.ContainsKey(boosterType))
        {
            BoosterAmounts.Add(boosterType, 0);
        }
        else
        {
            BoosterAmounts[boosterType]--;
        }
        UpdateBooster();
    }
    #region OneTile
    public void CheckReCreateShape()
    {
        var oneTile = _boosters.Find(_ => _.BoosterType == BoosterType.OneTile);
        if (oneTile)
        {
            (oneTile as BoosterOneTille).CheckReCreateTile();
        }
    }
    #endregion

    #region EraseShape
    public void DisableEraseShape()
    {
        var erase = _boosters.Find(_ => _.BoosterType == BoosterType.EraseShape);
        if (erase)
        {
            (erase as BoosterEraseShape).BackToInitScale();
        }
    }
    public void EnableEraseShape(bool enabled)
    {
        if (enabled)
        {
            _overlay.SetActive(true);
            transform.SetParent(_behindOverlay, false);
            // transform.parent = _behindOverlay;
            foreach (var shape in _shapes)
            {
                shape.transform.DOKill();
                shape.transform.DOScale(shape.transform.localScale * 1.2f, 0.5f)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
            }
            foreach (var image in _shapeImages)
            {
                image.raycastTarget = false;
            }
        }
        else
        {
            _overlay.SetActive(false);
            transform.SetParent(_initParent, false);
            // transform.parent = _initParent;
            foreach (var shape in _shapes)
            {
                shape.transform.DOKill();
                shape.transform.rotation = Quaternion.identity;
                shape.BackToScale();
            }
            foreach (var image in _shapeImages)
            {
                image.raycastTarget = true;
            }
        }
    }
    #endregion

    #region Bomb
    // Use for Bomb
    public void CheckBombPlace()
    {
        if (!BombDragging) return;
        ClearLastHover();
        (int, int) indexRowCol = BoardManager.Ins.GetIndexRowColByTransform(BombDragging.transform);
        if (indexRowCol.Item1 < 0 || indexRowCol.Item2 < 0)
        {
            // Don't in the board => back
            BombDragging.BackToInitPos();
        }
        else
        {
            int row = indexRowCol.Item1;
            int col = indexRowCol.Item2;
            int maxRowIndex = BoardManager.Ins.RowCount - 1;
            int maxColIndex = BoardManager.Ins.ColumnCount - 1;
            // Erase 3x3 area centered at (row, col)
            List<Tile> tiles = new();
            for (int r = row - 1; r <= row + 1; r++)
            {
                for (int c = col - 1; c <= col + 1; c++)
                {
                    // check boundary
                    if (r < 0 || r > maxRowIndex || c < 0 || c > maxColIndex)
                        continue;
                    Tile tile = Board[r, c];
                    if (tile.SquareOccupied)
                    {
                        //@TODO: effect
                        EffectManager.Ins.RequireBreakTile(tile.GetSpriteActive(), tile.transform.position);
                        tiles.Add(tile);
                    }
                }
            }
            if (tiles.Count > 0)
            {
                UseBooster(BoosterType.Bomb);
                ScoreManager.Ins.OnBombUsed(tiles);
                // SoundManager.Ins.breakTile.Play();
                ShakeParam shakeParam = new(3 * GameConfig.Ins.GameplayConfig.InitSeverity,
                                    true, true);
                GameEvents.ShakeCam?.Invoke(shakeParam);
                foreach (var tile in tiles)
                {
                    tile.ResetSquare();
                }
            }
            else
            {

            }
            BombDragging.BackToInitPos();
        }
        BoosterTypeUsing = BoosterType.None;
        BombDragging = null;
        _lastRow = -1;
        _lastCol = -1;
    }

    private void CheckHoverBomb()
    {
        if (!BombDragging) return;
        (int row, int col) = BoardManager.Ins.GetIndexRowColByTransform(BombDragging.transform);
        if (row < 0 || col < 0)
        {
            ClearLastHover();
            return;
        }
        // ❗ Nếu vị trí không đổi → skip luôn
        if (row == _lastRow && col == _lastCol) return;
        int maxRowIndex = BoardManager.Ins.RowCount - 1;
        int maxColIndex = BoardManager.Ins.ColumnCount - 1;
        // 🔥 Clear vùng cũ
        ClearLastHover();

        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                // check boundary
                if (r < 0 || r > maxRowIndex || c < 0 || c > maxColIndex)
                    continue;
                var tile = Board[r, c];
                tile.SetVirtualBomb(true);
                _lastHoverTiles.Add(tile);
            }
        }
        _lastRow = row;
        _lastCol = col;
    }
    private void ClearLastHover()
    {
        foreach (var tile in _lastHoverTiles)
        {
            tile.SetVirtualBomb(false);
        }
        _lastHoverTiles.Clear();
    }
    #endregion

}