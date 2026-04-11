using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileEditor : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _normalImage;
    [SerializeField] private Image _hoverImage;
    [SerializeField] private Image _activeImage;
    [SerializeField] private Image _hightLightScore;
    public List<Sprite> normalImages = new();

    public GoalItemType GoalItemType;

    public int RowIndex;
    public int ColumnIndex;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }
    public bool IsPlacedVirtual { get; set; }

    void Awake()
    {
        _hoverImage.gameObject.SetActive(false);
        _activeImage.gameObject.SetActive(false);
        _hightLightScore.gameObject.SetActive(false);
    }

    public bool CanUseThisSquare()
    {
        return _hoverImage.gameObject.activeSelf;
    }
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
        _activeImage.sprite = sprite;
        ActivateSquare();
    }
    public void ActivateSquare(TileColor tileColor, bool hasLike = false)
    {
        _activeImage.sprite = tileColor.Tile;
        ActivateSquare();
    }

    internal void ActivateSquareGoalItem(GoalItemType goalItemType)
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
    public void ResetSquare()
    {
        _hoverImage.gameObject.SetActive(false);
        _activeImage.gameObject.SetActive(false);
        _hightLightScore.gameObject.SetActive(false);
        GoalItemType = GoalItemType.None;
        Selected = false;
        IsPlacedVirtual = false;
        SquareOccupied = false;
        _hightLightScore.transform.localScale = Vector3.one;
    }
    public void SetImage(bool setFirstImage)
    {
        _normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

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

    public void SetVirtualScore(TileColor currentTileColor)
    {
        _hightLightScore.gameObject.SetActive(true);
        _hightLightScore.sprite = currentTileColor.Tile;
    }
    public void SetVirtualScore(RainBowColor rainBowColor)
    {
        _hightLightScore.gameObject.SetActive(true);
        _hightLightScore.sprite = rainBowColor.Tile;
    }
    public void UnSetVirtualScore()
    {
        _hightLightScore.gameObject.SetActive(false);
    }

    public Sprite GetSpriteActive()
    {
        if (!SquareOccupied) return null;
        return _activeImage.sprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left mouse click");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ResetSquare();
            Debug.Log("Right mouse click");
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Debug.Log("Middle mouse click");
        }
    }

}
