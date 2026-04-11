using System.Collections.Generic;
using DangExtension;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("---------Image---------")]
    [SerializeField] private Image _introImage;
    [SerializeField] private Image _normalImage;
    [SerializeField] private Image _hoverImage;
    [SerializeField] private Image _activeImage;
    [SerializeField] private Image _hightLightScore;
    [SerializeField] private Image _virtualBombImage;
    public List<Sprite> normalImages = new();
    // [Header("---------Color--------")]
    // [SerializeField] private Color _normalColor;
    // [SerializeField] private Color _bombColor;

    public GoalItemType GoalItemType;
    public bool HasMoney;

    public int RowIndex;
    public int ColumnIndex;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }
    public bool IsPlacedVirtual { get; set; }

    void Awake()
    {
        _introImage.gameObject.SetActive(false);
        _hoverImage.gameObject.SetActive(false);
        _activeImage.gameObject.SetActive(false);
        _hightLightScore.gameObject.SetActive(false);
        HasMoney = false;
    }

    public bool CanUseThisSquare()
    {
        return _hoverImage.gameObject.activeSelf;
    }

    #region ActiveSuqare
    public void ActivateSquare()
    {
        _hoverImage.gameObject.SetActive(false);
        _activeImage.gameObject.SetActive(true);
        _hightLightScore.gameObject.SetActive(false);
        Selected = true;
        SquareOccupied = true;
    }
    public void ActivateSquare(Sprite sprite)
    {
        GoalItemType = GoalItemType.None;
        _activeImage.sprite = sprite;
        ActivateSquare();
    }
    public void ActivateSquare(TileColor tileColor, bool hasLike = false)
    {
        if (tileColor == null) return;
        _activeImage.sprite = tileColor.Tile;
        GoalItemType = GoalItemType.None;
        ActivateSquare();
    }
    public void ActivateSquareMoney()
    {
        GoalItemType = GoalItemType.None;
        HasMoney = true;
        _activeImage.sprite = GameConfig.Ins.TileColorConfig.MoneyTile;
        ActivateSquare();
    }
    public void ActivateSquareGoalItem(GoalItemType goalItemType)
    {
        GoalItemType = goalItemType;
        if (goalItemType == GoalItemType.None) return;
        var a = GameConfig.Ins.TileColorConfig.TileGoalItems.Find(e => e.GoalItemType == goalItemType);
        if (a != null)
        {
            _activeImage.sprite = a.Tile;
        }
        ActivateSquare();

    }
    public void ActivateForLevelEditor(Sprite sprite)
    {
        _activeImage.sprite = sprite;
    }
    public void ActivateForLevelEditor(TileColor tileColor)
    {
        _activeImage.sprite = tileColor.Tile;
        ActivateSquare();
    }
    #endregion

    #region Reset
    public void ResetSquare()
    {
        _introImage.gameObject.SetActive(false);
        _hoverImage.gameObject.SetActive(false);
        _activeImage.gameObject.SetActive(false);
        _hightLightScore.gameObject.SetActive(false);
        _virtualBombImage.gameObject.SetActive(false);
        GoalItemType = GoalItemType.None;
        Selected = false;
        IsPlacedVirtual = false;
        SquareOccupied = false;
        HasMoney = false;
        _hightLightScore.transform.localScale = Vector3.one;
    }
    #endregion
    public void SetImage(bool setFirstImage)
    {
        _normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    #region Hover
    public void SetHover(bool isHover)
    {
        _hoverImage.gameObject.SetActive(isHover);
        IsPlacedVirtual = isHover;
    }
    public void SetHover(Sprite sprite, bool isHover)
    {
        _hoverImage.sprite = sprite;
        SetHover(isHover);
    }
    public void SetHoverMoney()
    {
        _hoverImage.sprite = GameConfig.Ins.TileColorConfig.MoneyTile;
        SetHover(true);
    }
    public void SetHoverForLevelEditor(bool isHover)
    {
        _hoverImage.gameObject.SetActive(isHover);
        SquareOccupied = isHover;
    }
    public void SetHoverForLevelEditor(Sprite sprite, bool isHover)
    {
        _hoverImage.sprite = sprite;
        _hoverImage.gameObject.SetActive(isHover);
        SquareOccupied = isHover;
    }
    public void SetHoverForLevelEditor(TileColor tileColor, bool isHover)
    {
        _hoverImage.sprite = tileColor.Tile;
        _hoverImage.gameObject.SetActive(isHover);
        SquareOccupied = isHover;
    }
    #endregion

    #region VirtualScore
    public void SetVirtualScore(TileColor currentTileColor)
    {
        if (GoalItemType != GoalItemType.None || HasMoney) return;
        _hightLightScore.gameObject.SetActive(true);
        _hightLightScore.sprite = currentTileColor.Tile;
    }
    public void SetVirtualScore(RainBowColor rainBowColor)
    {
        if (GoalItemType != GoalItemType.None || HasMoney) return;
        _hightLightScore.gameObject.SetActive(true);
        _hightLightScore.sprite = rainBowColor.Tile;
    }
    public void SetVirtualScore(Sprite sprite)
    {
        if (GoalItemType != GoalItemType.None || HasMoney) return;
        _hightLightScore.gameObject.SetActive(true);
        _hightLightScore.sprite = sprite;
    }
    public void UnSetVirtualScore()
    {
        _hightLightScore.gameObject.SetActive(false);
    }
    #endregion

    #region VirtualBomb
    public void SetVirtualBomb(bool enabled)
    {
        // if (SquareOccupied && enabled)
        // {
        //     // _activeImage.color = _bombColor;
        //     _virtualBombImage.gameObject.SetActive(true);
        // }
        // else if(!enabled)
        // {
        //     // _activeImage.color = _normalColor;
        //     _virtualBombImage.gameObject.SetActive(false);
        // }
        _virtualBombImage.gameObject.SetActive(enabled);
    }
    #endregion

    #region Getters
    public Sprite GetSpriteActive()
    {
        if (!SquareOccupied) return null;
        return _activeImage.sprite;
    }

    public Sprite GetSpriteVirtual()
    {
        if (GoalItemType != GoalItemType.None)
        {
            return GameConfig.Ins.TileColorConfig.TileGoalItems.Find(e => e.GoalItemType == GoalItemType).Tile;
        }
        if (HasMoney) return GameConfig.Ins.TileColorConfig.MoneyTile;
        return _hightLightScore.sprite;
    }
    public Sprite GetHoverSprite()
    {
        return _hoverImage.sprite;
    }
    #endregion

    public void SetIntro(Sprite sprite)
    {
        _introImage.gameObject.SetActive(true);
        _introImage.sprite = sprite;
        this.WaitThenExecute(GameConfig.Ins.EffectConfig.IntroTileHide, () =>
        {
            _introImage.gameObject.SetActive(false);
        });
    }
    public void SetLose(Sprite sprite)
    {
        _introImage.gameObject.SetActive(true);
        _introImage.sprite = sprite;
        this.WaitThenExecute(GameConfig.Ins.EffectConfig.EndTileHide, () =>
        {
            _introImage.gameObject.SetActive(false);
        });
    }
}

public enum GoalItemType : byte
{
    None,
    GemBlue,
    GemPink,
    GemGreen,
    GemRed,
    GemYellow,
}