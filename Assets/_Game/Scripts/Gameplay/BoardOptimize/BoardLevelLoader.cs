using UnityEngine;

public class BoardLevelLoader
{
    public LevelJsonData CurrentLevelData;
    private BoardContext _boardContext;
    public BoardLevelLoader(BoardContext boardContext)
    {
        _boardContext = boardContext;
    }

    public TargetData LoadLevelByIndex(int level)
    {
        level = level % GameConfig.Ins.GameplayConfig.LevelToLoop;
        level = level <= 0 ? GameConfig.Ins.GameplayConfig.LevelToLoop : level;
        CurrentLevelData = LevelJsonSystem.LoadLevelJson(level);

        if (CurrentLevelData == null)
        {
            Debug.LogError($"❌ Cannot load Level_{level}.json");
            return null;
        }

        return CurrentLevelData.target;
    }
    public void LoadBoardBaseOnLevel()
    {
        if (CurrentLevelData == null)
        {
            foreach (var tile in _boardContext.Board)
                tile.ResetSquare();
            return;
        }

        int index = 0;

        for (int row = 0; row < _boardContext.RowsCount; row++)
        {
            for (int col = 0; col < _boardContext.ColumnsCount; col++)
            {
                if (index >= CurrentLevelData.tiles.Count) continue;
                Tile tile = _boardContext.Board[row, col];
                TileJsonData data = CurrentLevelData.tiles[index++];

                tile.ResetSquare();
                tile.GoalItemType = (GoalItemType)data.goalItemType;

                if (data.occupied)
                {
                    if ((GoalItemType)data.goalItemType != GoalItemType.None)
                    {
                        tile.ActivateSquareGoalItem((GoalItemType)data.goalItemType);
                    }
                    else if (data.colorIndex >= 0 && data.colorIndex < GameConfig.Ins.TileColorConfig.ColorSprites.Count)
                    {
                        TileColor tileColor =
                            GameConfig.Ins.TileColorConfig.ColorSprites[data.colorIndex];

                        tile.ActivateSquare(tileColor);
                    }
                }
            }
        }
    }
}
