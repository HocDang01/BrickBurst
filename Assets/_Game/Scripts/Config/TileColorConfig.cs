using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class TileColorConfig
{
    public List<TileColor> ColorSprites = new();
    public List<RainBowColor> RainBowHorizontal = new();
    public List<RainBowColor> RainBowVertical = new();
    public Sprite RainbowOutlineHorizontal;
    public Sprite RainbowOutlineVertical;

    public Sprite MoneyTile;

    public List<TileGoalItem> TileGoalItems = new();
}

[Serializable]
public class TileColor
{
    public Sprite Tile;
    public Sprite TileBreak;
    public Sprite TileHightLight;
    public Sprite FBXTile;
    public Sprite LikeIcon;
    public Sprite OutlineVertical;
    public Sprite OutlineHorizontal;
    public GameObject FinishFXVertical;
    public GameObject FinishFXHorizontal;

}

[Serializable]
public class RainBowColor
{
    public Sprite Tile;
}

[Serializable]
public class TileGoalItem
{
    public GoalItemType GoalItemType;
    public Sprite Tile;
    public Sprite Icon;
}
