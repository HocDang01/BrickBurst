using DG.Tweening;
using UnityEngine;
public class BoardProgressService
{
    private BoardContext _boardContext;

    public BoardProgressService(BoardContext boardContext)
    {
        _boardContext = boardContext;
    }

    public void LoadBoardSaved()
    {
        var gameplay = GameplayManager.Ins;
        var save = BoardSaveData.Ins;

        BoardData data = null;

        if (gameplay.PlayMode == PlayMode.Classic)
        {
            if (!save.HasClassic())
            {
                GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.Normal);
                TrackingHandler.OnClassicStart(UserProperty.BBClassicPlayCount, 0);
                UserProperty.BBClassicContinue = 0;
                return;
            }
            data = save.GetClassic();
            UserProperty.BBClassicContinue++;
            TrackingHandler.OnClassicContinue(data.currentScore, UserProperty.BBClassicContinue);
        }
        else if (gameplay.PlayMode == PlayMode.Adventure)
        {
            Debug.Log($"HasAdventure: {save.HasAdventure()}");
            // doesn't has board adventure
            if (!save.HasAdventure())
            {
                Debug.Log("Respawn");
                GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.Normal);
                StartNewAdventure();
                HandleBooster();
                return;
            }
            data = save.GetAdventure();

            int level = BBManager.EnableCheat ? GameConfig.Ins.GameplayConfig.Level : UserProperty.BrickBurstLevel;

            if (level != data.level)
            {
                Debug.Log("Respawn");
                GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.Normal);
                save.ClearAdventure();
                StartNewAdventure(level);
                HandleBooster();
                return;
            }
            UserProperty.BBContinueLevelCount++;
            TrackingHandler.OnAdventureContinue(data.currentScore, UserProperty.BBContinueLevelCount);
        }

        LoadBoardFromSave(data);
    }
    public void LoadBoardFromSave(BoardData data)
    {
        if (data == null || data.tiles == null || data.tiles.Count <= 0)
        {
            Debug.Log("Respawn");
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.Normal);
            return;
        }

        foreach (var tile in _boardContext.Board)
            tile.ResetSquare();

        foreach (var t in data.tiles)
        {
            if (t.row >= _boardContext.RowsCount || t.column >= _boardContext.ColumnsCount) continue;
            Tile tile = _boardContext.Board[t.row, t.column];

            tile.GoalItemType = (GoalItemType)t.goalItemType;

            if (t.occupied)
            {
                if ((GoalItemType)t.goalItemType != GoalItemType.None)
                {
                    tile.ActivateSquareGoalItem((GoalItemType)t.goalItemType);
                }
                else if (t.hasMoney)
                {
                    tile.ActivateSquareMoney();
                }
                else if (t.colorIndex >= 0)
                {
                    TileColor color =
                        GameConfig.Ins.TileColorConfig.ColorSprites[t.colorIndex];
                    tile.ActivateSquare(color);
                }

            }
        }
        GameplayManager.Ins.LoadProgressData(data);
        if (BoosterManager.Ins)
        {
            BoosterManager.Ins.SetSavedBoosterAmount();
        }

        // Wait to ScoreManager activate
        DOVirtual.DelayedCall(0.1f, () =>
        {
            GameplayManager.Ins.CanWatchAds = data.canWatchAds;
            ScoreManager.Ins.LoadScore(data);
            if (ShapeSpawner.Ins)
            {
                ShapeSpawner.Ins.ReSpawnFollowData(data);
            }
        });
    }
    private void StartNewAdventure(int level = -1)
    {
        // if (level < 0)
        //     level = UserProperty.BrickBurstLevel;

        // UserProperty.BBStartLevelCount++;
        // TrackingHandler.OnAdventureStart(level, UserProperty.BBStartLevelCount, 0);
    }
    private void HandleBooster()
    {
        // if (!BBManager.NewAdventure || BoosterManager.Ins == null)
        //     return;

        // var save = BoardSaveData.Ins;
        // Debug.Log($"HasProgress: {save.HasProgress}");
        // if (!save.HasProgress)
        // {
        //     var moneyAdventureData = GameConfig.Ins
        //         .MoneyAdventureConfig
        //         .GetMoneyAdventureData(GameplayManager.Ins.MoneyModeEnum);

        //     BoosterManager.Ins.SetBoosterAmount(moneyAdventureData);
        // }
        // else
        // {
        //     BoosterManager.Ins.SetSavedBoosterAmount();
        // }
    }

}
