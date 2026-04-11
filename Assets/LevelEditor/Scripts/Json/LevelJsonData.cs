using System;
using System.Collections.Generic;

[Serializable]
public class LevelJsonData
{
    public int level;
    // public int rows;
    // public int columns;
    public TargetData target;
    public List<TileJsonData> tiles;
}

[Serializable]
public class TargetData
{
    public int TargetType;
    public int score;
    public List<GoalItemEntry> goalItems;
}

[Serializable]
public class TileJsonData
{
    public int rowIndex;
    public int columnIndex;
    public int colorIndex;
    public bool occupied;
    public int goalItemType;
}

[Serializable]
public class GoalItemEntry
{
    public int goalItem;
    public int count;
}
