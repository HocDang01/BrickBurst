using System;
using System.Collections;
using CoreDang;
using DangExtension;
using DG.Tweening;
using UnityEngine;

public class GameplayManager : SingletonMono<GameplayManager>
{
    [Header("----------Level Editor-----------")]
    public bool _levelEditor;
    // public UIMoneyMoveAnim UIMoneyMoveAnim;
    public Shape DraggingShape;

    // Param Board
    public int CurrentShapeCount;

    // Param Gameplay
    public bool IsInGame = true;
    public PlayMode PlayMode;
    public bool IsHardTurn = false;         // this 3 shape are hard
    public bool CanWatchAds = true;
    public int MilestoneIndexPrev;
    public int PlayTime;


    private Coroutine _countTimeRoutine;

    protected override void Awake()
    {
        base.Awake();
        Input.multiTouchEnabled = false;
        CurrentShapeCount = 0;
        PlayMode = PlayMode.Adventure;
        CanWatchAds = true;
        _countTimeRoutine = null;

    }

    #region PublicMethod
    public void OnPlaceShapeSuccess()
    {
        // If dragging is booster one tile -> don't --
        if (DraggingShape is not ShapeBooster)
            CurrentShapeCount--;
        else
        {
            if (BoosterManager.Ins)
                BoosterManager.Ins.UseBooster(BoosterType.OneTile);
        }
        if (DraggingShape)
        {
            DraggingShape.DestroyShape();
        }
        DraggingShape = null;
        // Check Re-create shape booster
        if (BoosterManager.Ins)
            BoosterManager.Ins.CheckReCreateShape();
        if (CurrentShapeCount <= 0)
        {
            OnNeedCreateShapesEvents();
        }
    }
    public void OnEraseShape()
    {
        CurrentShapeCount--;
        if (CurrentShapeCount <= 0)
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                OnNeedCreateShapesEvents();
            });
        }
    }
    private void OnNeedCreateShapesEvents()
    {
        float occupied = CalculateOccupiedOfBoard();
        if (occupied < GameConfig.Ins.ShapeSpawnerConfig.MinOccupiedOfferOneBeauty)
        {
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.Normal);
        }
        else if (occupied >= GameConfig.Ins.ShapeSpawnerConfig.MinOccupiedOfferOneBeauty && occupied < GameConfig.Ins.ShapeSpawnerConfig.MaxOccupiedOfferOneBeauty)
        {
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.OneBeauty);
        }
        else if (occupied >= GameConfig.Ins.ShapeSpawnerConfig.MinOccupiedOfferTwoBeauty && occupied < GameConfig.Ins.ShapeSpawnerConfig.MaxOccupiedOfferTwoBeauty)
        {
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.TwoBeauty);
        }
        else if (occupied >= GameConfig.Ins.ShapeSpawnerConfig.MinOccupiedOfferThreeBeauty && occupied <= GameConfig.Ins.ShapeSpawnerConfig.MaxOccupiedOfferThreeBeauty)
        {
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.ThreeBeauty);
        }
        else
        {
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.OneBeauty);
        }
    }

    public void RestartGame()
    {
        if (_countTimeRoutine != null)
        {
            StopCoroutine(_countTimeRoutine);
            _countTimeRoutine = null;
        }
        GameplayView.Ins.ModifyBackBtn(true);
        PlayTime = 0;
        _countTimeRoutine = StartCoroutine(CountPlayTime());
        IsInGame = true;
        IsHardTurn = false;
        CurrentShapeCount = 0;
        if (DraggingShape)
            DraggingShape.DestroyShape();
        DraggingShape = null;
        GameEvents.OnStartGame?.Invoke();
    }


    public void OnLoseGame(Action replay = null, Action nextLevel = null)
    {
        SoundManager.Ins.PlaySFX(SoundManager.Ins.failPlayFX);
        if (PlayMode == PlayMode.Classic)
        {
            BoardSaveData.Ins.ClearClassic();
        }
        else if (PlayMode == PlayMode.Adventure)
        {
            BoardSaveData.Ins.ClearAdventure();
        }
        IsInGame = false;
        GameplayView.Ins.ModifyBackBtn(false);
        float timeWait = GameConfig.Ins.EffectConfig.EndColumnDelay * 8
                             + GameConfig.Ins.EffectConfig.EndRowDelay * 8
                             + GameConfig.Ins.EffectConfig.EndTileHide
                             + 0.5f;
        if (CanWatchAds)
        {
            CanWatchAds = false;
            var shapes = ShapeSpawner.Ins.GetShapeWatchAds();
            DOVirtual.DelayedCall(timeWait, (TweenCallback)(() =>
            {
                PopupWatchAds.Show();
                var popupWatchAds = PopupWatchAds.Ins;
                DOVirtual.DelayedCall(0.1f, (TweenCallback)(() =>
                {
                    popupWatchAds.SpawnShape(shapes);
                    popupWatchAds.SetAction(
                        continueAction: (Action)(() =>
                        {
                            GameplayView.Ins.ModifyBackBtn(true);
                            IsInGame = true;
                            ShapeSpawner.Ins.SpawnShapeWatchAds(shapes);
                            DOVirtual.DelayedCall(0.01f, (TweenCallback)(() =>
                            {
                                switch (PlayMode)
                                {
                                    case PlayMode.Adventure:
                                        BoardSaveData.Ins.SaveAdventure((BoardManager)BoardManager.Ins);
                                        break;
                                    case PlayMode.Classic:
                                        BoardSaveData.Ins.SaveClassic((BoardManager)BoardManager.Ins);
                                        break;
                                }
                            }));
                        }),
                        loseGameAction: () =>
                        {
                            CanWatchAds = true;
                            LoseGameLogic(replay, nextLevel, false);
                        }
                    );
                }));
            }));
        }
        else
        {
            LoseGameLogic(replay, nextLevel, true);
            CanWatchAds = true;
        }
    }
    private void LoseGameLogic(Action replay = null, Action nextLevel = null, bool wait = true)
    {
        SendTracking(false);
        SoundManager.Ins.PlaySFX(SoundManager.Ins.failPlayFX);
        GameEvents.OnEndGame?.Invoke();

        GameplayView.Ins.ModifyBackBtn(false);
        IsInGame = false;
        Debug.Log("Lose");
        float timeWait = 0.1f;
        if (wait)
        {
            timeWait = GameConfig.Ins.EffectConfig.EndColumnDelay * 8
                             + GameConfig.Ins.EffectConfig.EndRowDelay * 8
                             + GameConfig.Ins.EffectConfig.EndTileHide
                             + 0.5f;
        }
        if (PlayMode == PlayMode.Classic)
        {
            BoardSaveData.Ins.ClearClassic();
            DOVirtual.DelayedCall(timeWait, () =>
            {
                if (CheckInterAds())
                {
                    // Ads.ShowInterstitial("inter_level_restart", OnCompleteInter, "brickburst_inter_loss");
                }
                else
                {
                    OnCompleteInter();
                }
                void OnCompleteInter(bool yes = true)
                {
                    ResetTimescaleAfterInter(yes);
                    PopupEndClassic.Show();
                    var popup = PopupEndClassic.Ins;
                    popup.SetScore(ScoreManager.Ins.TotalScore);
                    popup.SetTitle(ScoreManager.Ins.IsNewScore);
                    popup.SetActionOnHide(replay);
                }
            });
        }
        else if (PlayMode == PlayMode.Adventure)
        {
            BoardSaveData.Ins.ClearAdventure();
            DOVirtual.DelayedCall(timeWait, () =>
            {
                if (CheckInterAds())
                {
                    // Ads.ShowInterstitial("inter_level_restart", OnCompleteInter, "brickburst_inter_loss");
                }
                else
                {
                    OnCompleteInter();
                }
                void OnCompleteInter(bool yes = true)
                {
                    ResetTimescaleAfterInter(yes);
                    PopupEndAdventure.Show();
                    var popup = PopupEndAdventure.Ins;
                    popup.SetResult(ScoreManager.Ins.CurrentTargetData, ScoreManager.Ins.TotalScore, ScoreManager.Ins.GoalItemTargets);
                    popup.SetAction(replay, nextLevel);
                    popup.SetWin(false);

                }
            });
        }
    }
    public void OnWinGame(TargetData targetData)
    {
        SendTracking(true);
        IsInGame = false;
        GameplayView.Ins.ModifyBackBtn(false);
        GameEvents.OnEndGame?.Invoke();
        SoundManager.Ins.PlaySFX(SoundManager.Ins.winFX);
        if (BBManager.EnableCheat)
        {
            GameConfig.Ins.GameplayConfig.Level++;
        }
        else
        {
            BBSaveData.Ins.Level++;
            BBSaveData.Ins.dirty = true;
        }
        float waitTime = BBCanvasTop.Ins.GetTimeVictoryEffect();
        BoardSaveData.Ins.ClearAdventure();
        DOVirtual.DelayedCall((TargetType)ScoreManager.Ins.CurrentTargetData.TargetType == TargetType.Score ? 0.2f : 1f, (TweenCallback)(() =>
        {
            EffectManager.Ins.WinAdventure(targetData);
            DOVirtual.DelayedCall(waitTime, () =>
            {
                if (CheckInterAds())
                {
                    // Ads.ShowInterstitial("inter_level_completed", OnCompleteInter, "brickburst_inter_win");
                }
                else
                {
                    OnCompleteInter();
                }

                void OnCompleteInter(bool yes = true)
                {
                    ResetTimescaleAfterInter(yes);
                    PopupEndAdventure.Show();
                    var popupfake = PopupEndAdventure.Ins;
                    popupfake.SetResult(ScoreManager.Ins.CurrentTargetData, ScoreManager.Ins.TotalScore, ScoreManager.Ins.GoalItemTargets);
                    popupfake.SetAction((Action)(_levelEditor ? null : BoardManager.Ins.RestartLevel), (Action)(_levelEditor ? null : BoardManager.Ins.NextLevel));
                    popupfake.SetWin(true);
                }
            });
        }));
        Debug.Log("Clear Adventure");
    }

    #region MICS
    // can show ads => return true
    private bool CheckInterAds()
    {
        return false;
        // bool isNoAdsByMode = MoneyModeEnum == MoneyModeEnum.M5M || MoneyModeEnum == MoneyModeEnum.M10M;
        // int gap = GameConfig.Ins.MainMenuConfig.GetAdsGapByMoneyMode(MoneyModeEnum);
        // if (gap <= 0)
        // {
        //     isNoAdsByMode = true;
        // }
        // else
        // {
        //     isNoAdsByMode = (MilestoneIndexPrev + 1) % gap != 0;
        // }
        // Debug.Log($"MilestoneIndex: {MilestoneIndexPrev + 1}");
        // Debug.Log($"Gap: {gap}");
        // Debug.Log($"isNoAdsByMode: {isNoAdsByMode}");
        // return !isNoAdsByMode && BBSaveData.Ins.Vip == 0 && Ads.IsAdsEnabled(AdsType.INTERSTITIAL) && Ads.IsDisplayable(AdsType.INTERSTITIAL) && Ads.IsAvailable(AdsType.INTERSTITIAL);
    }
    private void SendTracking(bool win)
    {
        if (_countTimeRoutine != null)
        {
            StopCoroutine(_countTimeRoutine);
            _countTimeRoutine = null;
        }
        if (PlayMode == PlayMode.Classic)
        {
            TrackingHandler.OnClassicEnd(BBSaveData.Ins.BBClassicPlayCount, ScoreManager.Ins.TotalScore, PlayTime, BBSaveData.Ins.BBClassicContinue, ScoreManager.Ins.TotalScore >= 500);
            BBSaveData.Ins.BBClassicContinue = 0;
            BBSaveData.Ins.dirty = true;
        }
        else if (PlayMode == PlayMode.Adventure)
        {
            BBSaveData.Ins.BBStartLevelCount += 1;
            TrackingHandler.OnAdventureEnd(BBSaveData.Ins.BBStartLevelCount, ScoreManager.Ins.TotalScore, PlayTime, BBSaveData.Ins.BBContinueLevelCount, win);
            BBSaveData.Ins.BBContinueLevelCount = 0;
            if (win)
            {
                BBSaveData.Ins.BBStartLevelCount = 0;
            }
            BBSaveData.Ins.dirty = true;
        }
    }

    public void LoadProgressData(BoardData boardData)
    {
        if (boardData == null || boardData.tiles == null || boardData.tiles.Count <= 0) return;
        CurrentShapeCount = 0;
        for (int i = 0; i < 3; i++)
        {
            if (boardData.shapesSaveData[i].shapeType != ShapeType.None && boardData.shapesSaveData[i].shapeIndex >= 0)
            {
                CurrentShapeCount++;
            }
        }
        if (CurrentShapeCount <= 0)
        {
            Debug.Log("Dont have shape in save data!");
            GameEvents.OnNeedCreateShapes?.Invoke(ShapeSpawnType.Normal);
        }
    }
    #endregion
    private void ResetTimescaleAfterInter(bool yes)
    {
        StartCoroutine(WaitOneFrameToTimeScale1());
    }
    private IEnumerator WaitOneFrameToTimeScale1()
    {
        yield return null;
        Time.timeScale = 1f;
    }

    private IEnumerator CountPlayTime()
    {
        while (true)
        {
            yield return WaitTimeCache.Wait1;
            PlayTime++;
        }
    }

    private float CalculateOccupiedOfBoard()   // return occupied / total tile
    {
        float occupied = 0;
        foreach (var tile in BoardManager.Ins.Board)
        {
            if (tile.SquareOccupied)
                occupied++;
        }
        return occupied / (BoardManager.Ins.RowCount * BoardManager.Ins.ColumnCount);
    }

    #endregion

}

public enum PlayMode
{
    Classic,
    Adventure,
}



// public void OnWinGame(TargetData targetData)
//         {
//             SendTracking(true);
//             IsInGame = false;
//             GameplayView.Ins.ModifyBackBtn(false);
//             GameEvents.OnEndGame?.Invoke();
//             SoundManager.Ins.PlaySFX(SoundManager.Ins.winFX);
//             if (BBManager.NewAdventure)
//             {
//                 MilestoneIndexPrev = BBSaveData.Ins.BBMilestoneIndex;
//                 BBSaveData.Ins.BBMilestoneIndex = (BBSaveData.Ins.BBMilestoneIndex + 1) % GameConfig.Ins.MoneyAdventureConfig.MilestoneDatas.Count; GameConfig.Ins.GameplayConfig.Level++;
//             }
//             if (BBManager.EnableCheat)
//             {
//                 GameConfig.Ins.GameplayConfig.Level++;
//             }
//             else
//             {
//                 BBSaveData.Ins.BrickBurstLevel++;
//             }
//             float waitTime = EffectManager.Ins.GetTimeWinAdventure();
//             BoardSaveData.Ins.ClearAdventure();
//             DOVirtual.DelayedCall((TargetType)ScoreManager.Ins.CurrentTargetData.TargetType == TargetType.Score ? 0.2f : 1f, (TweenCallback)(() =>
//             {
//                 EffectManager.Ins.WinAdventure(targetData);
//                 DOVirtual.DelayedCall(waitTime, () =>
//                 {
//                     if (CheckInterAds())
//                     {
//                         Ads.ShowInterstitial("inter_level_completed", OnCompleteInter, "brickburst_inter_win");
//                     }
//                     else
//                     {
//                         OnCompleteInter();
//                     }

//                     void OnCompleteInter(bool yes = true)
//                     {
//                         ResetTimescaleAfterInter(yes);
//                         var milestoneData = GameConfig.Ins.MoneyAdventureConfig.GetMilestoneData(MoneyModeEnum);
//                         if (BBManager.NewAdventure &&
//                             MilestoneIndexPrev >= milestoneData.Count - 1 ||
//                             BBSaveData.Ins.BBMilestoneIndex >= milestoneData.Count)
//                         {
//                             int cashValue = milestoneData[MilestoneIndexPrev].Money;
//                             var popupCollectMoney = PopupCollectMoney.Show();
//                             popupCollectMoney.SetCash(cashValue, milestoneData[MilestoneIndexPrev].IsAnchorPoint);

//                             return;
//                         }
//                         var popupfake = PopupEndAdventure.Show();
//                         popupfake.SetResult(ScoreManager.Ins.CurrentTargetData, ScoreManager.Ins.TotalScore, ScoreManager.Ins.GoalItemTargets);
//                         popupfake.SetAction((Action)(_levelEditor ? null : BoardManager.Ins.RestartLevel), (Action)(_levelEditor ? null : BoardManager.Ins.NextLevel));
//                         popupfake.SetWin(true);
//                         popupfake.SetCash(ScoreManager.Ins.CashTemp);
//                     }
//                 });
//             }));
//             Debug.Log("Clear Adventure");
//         }