using System.Collections.Generic;
using CoreDang;
using DangExtension;
using UnityEngine;
public class BoardSaveData : BaseUserData
{
    public static BoardSaveData Ins => LocalData.Get<BoardSaveData>();
    public int PlayCount;           // PlayCount of BrickBurst

    // Progress for new milestone
    public bool HasProgress;
    public Dictionary<BoosterType, int> BoosterAmounts;
    // Board Data
    public BoardData ClassicSave;
    public BoardData AdventureSave;
    protected override void OnInit()
    {
        PlayCount = 0;
        base.OnInit();
    }

    public void SaveProgress()
    {
        HasProgress = true;
        BoosterAmounts = BoosterManager.Ins.BoosterAmounts.Clone();
        dirty = true;
    }
    public void ClearProgress()
    {
        HasProgress = false;
        if (BoosterAmounts != null) BoosterAmounts.Clear();
        dirty = true;
    }


    // ================== SAVE ==================
    public void SaveClassic(BoardManager tileManager)
    {
        ClassicSave = BuildBoardData(tileManager);
        dirty = true;
    }

    public void SaveAdventure(BoardManager tileManager)
    {
        AdventureSave = BuildBoardData(tileManager);
        dirty = true;
    }

    // ================== LOAD ==================
    public BoardData GetClassic()
    {
        return ClassicSave;
    }

    public BoardData GetAdventure()
    {
        return AdventureSave;
    }

    public bool HasClassic() => ClassicSave != null && ClassicSave.tiles != null && ClassicSave.tiles.Count > 0;
    public bool HasAdventure() => AdventureSave != null && AdventureSave.tiles != null && AdventureSave.tiles.Count > 0;

    // ================== CLEAR ==================
    public void ClearClassic()
    {
        ClassicSave = null;
        Save();

    }

    public void ClearAdventure()
    {
        AdventureSave = null;
        Save();
    }

    public void ClearAll()
    {
        ClassicSave = null;
        AdventureSave = null;
        dirty = true;
    }

    // ================== CORE ==================
    private BoardData BuildBoardData(BoardManager tileManager)
    {
        List<int> shapeIndexs = new();
        List<ShapeType> shapeTypes = new();
        List<GoalItemType> goalItemTypes = new();
        List<List<int>> goalItemIndex = new();
        // List<List<int>> moneyIndex = new();

        List<Shape> shapes = BoardManager.Ins.Shapes;
        foreach (Shape shape in shapes)
        {
            if (shape.CurrentShape == null || shape.CurrentShape.Count <= 0 || shape.CurrentShapeData == null)
            {
                shapeTypes.Add(ShapeType.None);
                shapeIndexs.Add(-1);
                goalItemTypes.Add(GoalItemType.None);
                goalItemIndex.Add(null);
                // moneyIndex.Add(null);
                continue;
            }

            bool found = false;

            foreach (var pool in GameConfig.Ins.ShapeSpawnerConfig.PoolShapeConfigs)
            {
                for (int i = 0; i < pool.ShapeRatios.Count; i++)
                {
                    if (pool.ShapeRatios[i].ShapeData == shape.CurrentShapeData)
                    {
                        shapeTypes.Add(pool.ShapeType);
                        shapeIndexs.Add(i);
                        goalItemTypes.Add(shape.GoalItemType);
                        goalItemIndex.Add(shape.GetGoalItemIndex());
                        // moneyIndex.Add(shape.GetMoneyIndex());
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            if (!found)
            {
                Debug.LogWarning(
                    $"[ShapeConfigMissing] ShapeData '{shape.CurrentShapeData.name}' not found in PoolShapeConfigs");

                shapeTypes.Add(ShapeType.None);
                shapeIndexs.Add(-1);
                goalItemTypes.Add(GoalItemType.None);
                goalItemIndex.Add(null);
                // moneyIndex.Add(null);
            }
        }
        int levelSave = 0;
        if (BBManager.EnableCheat)
            levelSave = GameConfig.Ins.GameplayConfig.Level;
        // else
        //     levelSave = UserProperty.BrickBurstLevel;
        BoardData data = new BoardData
        {
            level = levelSave,
            rows = BoardManager.Ins.RowCount,
            columns = BoardManager.Ins.ColumnCount,
            goalItems = new Dictionary<GoalItemType, int>(ScoreManager.Ins.GoalItemTargets),
            currentScore = ScoreManager.Ins.TotalScore,
            canWatchAds = GameplayManager.Ins.CanWatchAds,
        };
        for (int i = 0; i < 3; i++)
        {
            data.shapesSaveData.Add(new ShapeSaveData
            {
                shapeType = shapeTypes[i],
                shapeIndex = shapeIndexs[i],
                goalItemType = goalItemTypes[i],
                itemIndexList = goalItemIndex[i],
                // moneyIndexList = moneyIndex[i],
            });
        }

        foreach (var tile in tileManager.Board)
        {
            if (!tile.SquareOccupied && tile.GoalItemType == GoalItemType.None)
                continue;

            data.tiles.Add(new BoardTileSaveData
            {
                row = tile.RowIndex,
                column = tile.ColumnIndex,
                occupied = tile.SquareOccupied,
                goalItemType = (int)tile.GoalItemType,
                hasMoney = tile.HasMoney,
                colorIndex = tile.SquareOccupied
                    ? GameConfig.Ins.TileColorConfig.ColorSprites
                        .FindIndex(x => x.Tile == tile.GetSpriteActive())
                    : -1
            });
        }

        return data;
    }
}
[System.Serializable]
public class BoardData
{
    //@TODO: Save combocount, bonusCount
    public int level;
    public int rows;
    public int columns;
    public int currentScore;
    public bool canWatchAds;
    // 3 shape
    public List<ShapeSaveData> shapesSaveData = new();
    // Use for adventure
    public Dictionary<GoalItemType, int> goalItems = new();
    public List<BoardTileSaveData> tiles = new();
}

[System.Serializable]
public class ShapeSaveData
{
    public ShapeType shapeType;
    public int shapeIndex;
    public GoalItemType goalItemType;
    public List<int> itemIndexList;
    // public List<int> moneyIndexList;
}

[System.Serializable]
public class BoardTileSaveData
{
    public int row;
    public int column;
    public int colorIndex;
    public bool occupied;
    public int goalItemType;
    public bool hasMoney;
}


