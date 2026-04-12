using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Advantage")]
    [SerializeField] private GameObject _adventure;
    [SerializeField] private GameObject _scoreAdventure;
    [SerializeField] private ScoreTextAnim _scoreAdventureText;
    [SerializeField] private TextMeshProUGUI _targetScoreText;
    [SerializeField] private Slider _targetSlider;

    [SerializeField] private GameObject _itemAdventure;
    [SerializeField] private UIItemTargetManager _itemTargetManager;

    [Header("Score Classic")]
    [SerializeField] private GameObject _classic;
    [SerializeField] private ScoreTextAnim _scoreClassicTextAnim;
    [SerializeField] private ScoreTextAnim _bestScoreTextAnim;

    [Header("Ref Star Score")]
    [SerializeField] private StarScoreEffect _starScoreEffect;
    [Header("Dev")]

    private int _targetScore;
    private int _totalScore;
    private int _rowCanScore;
    private int _columnCanScore;
    private bool _canNotiNewScore = true;
    private float _gapTimeToShowNewScoreEffect = 0f;
    private bool _isCombo;

    [SerializeField] private Dictionary<GoalItemType, int> _goalItemTargets;
    private TargetData _currentTargetData;


    public int TotalScore { get => _totalScore; private set => _totalScore = value; }
    public int TargetScore { get => _targetScore; set => _targetScore = value; }
    public bool IsNewScore => !_canNotiNewScore;

    public int ComboCount { get => _comboCount; private set => _comboCount = value; }
    public Dictionary<GoalItemType, int> GoalItemTargets
    { get => _goalItemTargets; private set => _goalItemTargets = value; }
    public TargetData CurrentTargetData { get => _currentTargetData; private set => _currentTargetData = value; }

    public static ScoreManager Ins;
    void Awake()
    {
        GoalItemTargets = new();

        Ins = this;
        GameEvents.OnStartGame += StartGame;
    }
    void OnEnable()
    {
        _bestScoreTextAnim.SetScoreImmediate(BBSaveData.Ins.BestScore);
        GameEvents.OnPlaceOnGrid += OnGetScore;
    }
    void OnDisable()
    {
        GameEvents.OnPlaceOnGrid -= OnGetScore;
    }
    void OnDestroy()
    {
        GameEvents.OnStartGame -= StartGame;
    }
    #region StartGame
    private void StartGame()
    {
        Ins = this;
        _lastScoredTurn = -999;
        _comboSoundIdx = -1;
        TotalScore = 0;
        ComboCount = 0;
        _nowTurn = 0;
        _gapTimeToShowNewScoreEffect = 0f;
        _starScoreEffect.EndCombo();
        _isCombo = false;
        GoalItemTargets.Clear();
        EffectManager.Ins.EndComboBoard();
        SwitchMode();

        _bestScoreTextAnim.SetScoreImmediate(BBSaveData.Ins.BestScore);
        _scoreClassicTextAnim.RefreshScore();
        _scoreAdventureText.RefreshScore();
        _targetSlider.value = 0f;
    }
    private void SwitchMode()
    {
        _scoreAdventure.SetActive(false);
        _itemAdventure.SetActive(false);
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Adventure:
                _canNotiNewScore = false;
                _adventure.SetActive(true);
                _classic.SetActive(false);
                break;
            case PlayMode.Classic:
                if (BBSaveData.Ins.BestScore > 0)     // Just notify if had been played
                {
                    _canNotiNewScore = true;
                }
                else
                {
                    _canNotiNewScore = false;
                }
                _adventure.SetActive(false);
                _classic.SetActive(true);
                break;
        }
    }
    public void SetTarget(TargetData targetData)
    {
        if (GameplayManager.Ins.PlayMode != PlayMode.Adventure) return;
        CurrentTargetData = targetData;
        Debug.Log("Target Type: " + (TargetType)targetData.TargetType);
        switch ((TargetType)targetData.TargetType)
        {
            case TargetType.Score:
                TargetScore = targetData.score;
                _scoreAdventure.SetActive(true);
                _itemAdventure.SetActive(false);
                _classic.SetActive(false);
                break;
            case TargetType.GoalIem:
                _scoreAdventure.SetActive(false);
                _itemAdventure.SetActive(true);
                _classic.SetActive(false);
                TargetScore = 0;
                GoalItemTargets = Utility.GoalItemListToDict(targetData.goalItems);
                _itemTargetManager.InitItemTarget(GoalItemTargets);     // udpate UI
                break;
            case TargetType.Both:
                _scoreAdventure.SetActive(true);
                _itemAdventure.SetActive(true);
                _classic.SetActive(false);
                TargetScore = targetData.score;
                GoalItemTargets.Clear();
                GoalItemTargets = Utility.GoalItemListToDict(targetData.goalItems);
                _itemTargetManager.InitItemTarget(GoalItemTargets);     // udpate UI
                break;
        }
        _targetScoreText.text = targetData.score.ToString();
    }
    #endregion
    #region Load Data
    public void LoadScore(BoardData data)
    {
        //@TODO: Save combocount, bonusCount
        TotalScore = data.currentScore;
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Classic:
                _scoreClassicTextAnim.SetScoreImmediate(TotalScore);
                if (TotalScore >= BBSaveData.Ins.BestScore)
                {
                    _canNotiNewScore = false;
                }
                break;
            case PlayMode.Adventure:
                //@TODO: add condition when target not only score
                switch ((TargetType)CurrentTargetData.TargetType)
                {
                    case TargetType.Score:
                        break;
                    case TargetType.GoalIem:
                        break;
                    case TargetType.Both:
                        break;
                }
                if (data.goalItems != null && data.goalItems.Count > 0)
                {
                    GoalItemTargets = data.goalItems;
                    _itemTargetManager.LoadItemTarget(GoalItemTargets);
                }
                // StartCoroutine(IncreaseSlider(0.2f));
                _targetSlider.value = TotalScore / (float)TargetScore;
                _scoreAdventureText.SetScoreImmediate(TotalScore);
                break;
        }
    }
    #endregion

    #region GetScore
    // This function will be invoked before decrease CurrentShapeCount 
    private void OnGetScore()
    {
        if (GameplayManager.Ins.DraggingShape == null || GameplayManager.Ins.DraggingShape.CurrentShape == null) return;
        _nowTurn++;
        _rowCanScore = BoardManager.Ins.rowsCanScore.Count;
        _columnCanScore = BoardManager.Ins.columnsCanScore.Count;

        // ---------Caluclate param-------------
        int scoreShape = GameplayManager.Ins.DraggingShape.CurrentShape.Count;
        int scorePerRowCol = GameConfig.Ins.ScoreConfig.ScorePerRowCol;
        int rowScore = _rowCanScore / BoardManager.Ins.RowCount * scorePerRowCol;
        int colScore = _columnCanScore / BoardManager.Ins.ColumnCount * scorePerRowCol;
        if (rowScore > 0 || colScore > 0)
        {
            _lastScoredTurn = _nowTurn;
        }
        bool isScoredThisTurn = (rowScore > 0 || colScore > 0);
        int bonusCount = CalculateBonusCount();
        int comboCount = CalculateComboCount(isScoredThisTurn, bonusCount);
        // ---------------END-------------------
        if (bonusCount >= 2 && comboCount <= 0)
        {
            _gapTimeToShowNewScoreEffect = 0.6f;
        }
        else if (isScoredThisTurn && GameplayManager.Ins.IsHardTurn && GameplayManager.Ins.CurrentShapeCount <= 1)
        {
            _gapTimeToShowNewScoreEffect = 0.2f;
        }
        else
        {
            _gapTimeToShowNewScoreEffect = 0.1f;
        }

        // Get Score
        TotalScore += GetScoreAddition(isScoredThisTurn, bonusCount, comboCount, rowScore, colScore);
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Classic:
                UpdateClassicScore();
                break;
            case PlayMode.Adventure:
                UpdateAdventureScore(BoardManager.Ins.allCanScore);
                break;
        }
        CheckAmazing();
        // Wait 0.2s to complete board
        DOVirtual.DelayedCall(0.4f, (TweenCallback)(() =>
        {
            CheckAllClear();
            CheckEndGame();
        }));
    }
    private void CheckEndGame()
    {
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Classic:
                if (GameplayManager.Ins.IsInGame)
                    BoardSaveData.Ins.SaveClassic(BoardManager.Ins);
                break;
            case PlayMode.Adventure:
                switch ((TargetType)CurrentTargetData.TargetType)
                {
                    case TargetType.Score:
                        if (TotalScore >= TargetScore)
                        {
                            GameplayManager.Ins.OnWinGame(CurrentTargetData);
                        }
                        else
                        {
                            if (GameplayManager.Ins.IsInGame)
                            {
                                BoardSaveData.Ins.SaveProgress();
                                BoardSaveData.Ins.SaveAdventure(BoardManager.Ins);
                            }
                        }
                        break;
                    case TargetType.GoalIem:
                        if (GoalItemTargets == null || GoalItemTargets.Count <= 0 || GoalItemTargets.Sum(e => e.Value) <= 0)
                        {

                            GameplayManager.Ins.OnWinGame(CurrentTargetData);
                        }
                        else
                        {
                            if (GameplayManager.Ins.IsInGame)
                            {
                                BoardSaveData.Ins.SaveProgress();
                                BoardSaveData.Ins.SaveAdventure(BoardManager.Ins);
                            }
                        }
                        break;
                    case TargetType.Both:
                        if (TotalScore >= TargetScore && GoalItemTargets == null || GoalItemTargets.Count <= 0 || GoalItemTargets.Sum(e => e.Value) <= 0)
                        {
                            GameplayManager.Ins.OnWinGame(CurrentTargetData);
                        }
                        else
                        {
                            if (GameplayManager.Ins.IsInGame)
                            {
                                BoardSaveData.Ins.SaveProgress();
                                BoardSaveData.Ins.SaveAdventure(BoardManager.Ins);
                            }
                        }
                        break;
                }
                break;
        }
        if (GameplayManager.Ins.IsInGame)
        {
            BoardManager.Ins.CheckLoseGame();
        }
    }
    private void GetGoalItem(List<Tile> allCanScore)
    {
        if (allCanScore == null) return;
        foreach (var tile in allCanScore)
        {
            if (tile.GoalItemType != GoalItemType.None)
            {
                // Calculate remaining GoalItemTarget
                if (GoalItemTargets.Keys.Contains(tile.GoalItemType) && GoalItemTargets[tile.GoalItemType] > 0)
                {
                    GoalItemTargets[tile.GoalItemType]--;
                }
                //play anim Get GoalItem
                EffectManager.Ins.PlayAnimGoalItemFly(tile.transform.position, tile.GoalItemType, GoalItemTargets[tile.GoalItemType]);
            }
        }
    }
    private int GetScoreAddition(bool isScoredThisTurn, int bonusCount, int comboCount, int rowScore, int colScore)
    {
        if (GameplayManager.Ins.DraggingShape == null || GameplayManager.Ins.DraggingShape.CurrentShape == null) return 0;
        int scoreShape = GameplayManager.Ins.DraggingShape.CurrentShape.Count;

        int bonusScore = 0;
        int comboScore = 0;

        // Start Play Effect
        if (isScoredThisTurn)
        {
            PlayEffect(bonusCount, comboCount);
            bonusScore = GetBonusScore(bonusCount);
            comboScore = GetComboScore(comboCount);
            // VibrationManager.Play(VibrateType.Medium);
            Vibrate.Play(VibrateType.Selection);
        }
        else if (comboCount <= 0)
        {
            _starScoreEffect.EndCombo();
            _isCombo = false;

            EffectManager.Ins.EndComboBoard();
        }
        if (!isScoredThisTurn)
        {
            SoundManager.Ins.PlaySFX(SoundManager.Ins.putTilesFx);
            Vibrate.Play(VibrateType.Selection);
        }
        return scoreShape + rowScore + colScore + bonusScore + comboScore;
    }
    #endregion

    #region Adventure
    private void UpdateAdventureScore(List<Tile> allCanScore)
    {
        _scoreAdventureText.SetScore(TotalScore);
        StartCoroutine(IncreaseSlider(0.3f));
        if (CurrentTargetData != null && (TargetType)CurrentTargetData.TargetType != TargetType.Score)
        {
            GetGoalItem(allCanScore);
        }
    }
    private IEnumerator IncreaseSlider(float duration)
    {
        if (TargetScore <= 0)
            yield break;

        float startValue = _targetSlider.value;
        float targetValue = TotalScore / (float)TargetScore;

        targetValue = Mathf.Clamp01(targetValue);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (_targetSlider.value >= 1) yield break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            _targetSlider.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        _targetSlider.value = targetValue; // đảm bảo chạm đúng
    }

    #endregion

    #region Classic
    private void UpdateClassicScore()
    {
        _scoreClassicTextAnim.SetScore(TotalScore);
        if (TotalScore > BBSaveData.Ins.BestScore)
        {
            BBSaveData.Ins.BestScore = TotalScore;
            BBSaveData.Ins.dirty = true;
            _bestScoreTextAnim.SetScore(BBSaveData.Ins.BestScore);
            if (_canNotiNewScore)
            {
                _canNotiNewScore = false;
                DOVirtual.DelayedCall(_gapTimeToShowNewScoreEffect, () =>
                {
                    GameEvents.NewScoreEffect?.Invoke();
                });
            }
        }
    }
    #endregion

    #region PlayEffect
    private void PlayEffect(int bonusCount, int comboCount)
    {
        // Play Sound
        if (bonusCount == 1 && comboCount <= 0)         // 0 combo, 1 row/column
        {
            SoundManager.Ins.PlaySFX(SoundManager.Ins.normalScoredFx);
        }
        else if (bonusCount == 2 || bonusCount == 3)    // 2,3 row/column
        {
            SoundManager.Ins.PlaySFX(SoundManager.Ins.columns23);
        }
        else if (bonusCount == 4)                       // 4 row/column
        {
            SoundManager.Ins.PlaySFX(SoundManager.Ins.columns4);
        }
        else if (bonusCount >= 5)                       // >= 5 row/column
        {
            SoundManager.Ins.PlaySFX(SoundManager.Ins.columns5);
        }
        else if (_comboSoundIdx >= 0 && _comboSoundIdx < SoundManager.Ins.comboScoreFXs.Count)    // 1 row/column, combo > 1
        {
            SoundManager.Ins.PlaySFX(SoundManager.Ins.comboScoreFXs[_comboSoundIdx]);
        }

        // Start Effect Combo
        if (comboCount >= 2)
        {
            _isCombo = true;
            _starScoreEffect.StartCombo();
            EffectManager.Ins.StartComboBoard();
        }

        // 1. Shake Cam
        if (comboCount > (GameConfig.Ins.GameplayConfig.ComboCountShake - 1))
        {
            Debug.Log("ShakeCam param: " + (comboCount - (GameConfig.Ins.GameplayConfig.ComboCountShake - 1)));
            ShakeParam shakeParam = new((comboCount - (GameConfig.Ins.GameplayConfig.ComboCountShake - 1)) * GameConfig.Ins.GameplayConfig.InitSeverity,
                                    _columnCanScore > 0, _rowCanScore > 0);
            GameEvents.ShakeCam?.Invoke(shakeParam);
            // play effect at score board
            _starScoreEffect.PlayStrongEffect(GameConfig.Ins.GameplayConfig.Duration * 2f);
            EffectManager.Ins.HightLightBoard(_isCombo);
        }
        else if (bonusCount >= 2)
        {
            ShakeParam shakeParam = new((bonusCount - 1) * GameConfig.Ins.GameplayConfig.InitSeverity,
                                    _columnCanScore > 0, _rowCanScore > 0);
            GameEvents.ShakeCam?.Invoke(shakeParam);
            EffectManager.Ins.HightLightBoard(_isCombo);
        }

        // 2. Call Combo count (streak)
        if (comboCount > 0)     // Wait show Big Combo Effect
        {
            DOVirtual.DelayedCall(_gapTimeToShowNewScoreEffect, () =>
            {
                // this will place 0.7s
                GameEvents.ComboEfect?.Invoke(comboCount);
            });
            _gapTimeToShowNewScoreEffect += 0.7f;
        }

        // 3. Call Emotion
        if (bonusCount >= 2)
        {
            DOVirtual.DelayedCall(_gapTimeToShowNewScoreEffect, () =>
            {
                // this will place 1.0s
                if (bonusCount == 2)
                {
                    GameEvents.GoodEffect?.Invoke();
                }
                else if (bonusCount == 3)
                {
                    GameEvents.GreatEffect?.Invoke();
                }
                else if (bonusCount == 4)
                {
                    GameEvents.ExcellentEffect?.Invoke();
                }
                else if (bonusCount >= 5)
                {
                    GameEvents.PerfectEffect?.Invoke();
                }
            });
            _gapTimeToShowNewScoreEffect += 1.0f;
        }

    }

    // This function will be invoked before decrease CurrentShapeCount 
    private void CheckAmazing()
    {
        if (!GameplayManager.Ins.IsHardTurn || GameplayManager.Ins.CurrentShapeCount > 1)
        {
            return;
        }
        DOVirtual.DelayedCall(_gapTimeToShowNewScoreEffect, () =>
        {
            GameEvents.AmazingEffect?.Invoke();
        });
        _gapTimeToShowNewScoreEffect += 1.3f;
    }
    private void CheckAllClear()
    {
        foreach (var tile in BoardManager.Ins.Board)
        {
            if (tile.SquareOccupied) return;
        }
        _gapTimeToShowNewScoreEffect -= 0.4f;
        DOVirtual.DelayedCall(_gapTimeToShowNewScoreEffect, () =>
        {
            GameEvents.AllClearEffect?.Invoke();
        });

        _gapTimeToShowNewScoreEffect += 1.5f;
    }
    #endregion

    #region Bonus+Combo
    private int GetBonusScore(int bonus)
    {
        Debug.Log("Bonus count " + bonus);
        int bonusScore = GameConfig.Ins.ScoreConfig.GetBonusScore(bonus);
        return bonusScore;
    }
    private int GetComboScore(int combo)
    {
        Debug.Log("Streak Count" + combo);
        int comboScore = GameConfig.Ins.ScoreConfig.GetComboScore(combo);
        return comboScore;
    }


    private int _comboCount;
    public int _nowTurn;
    private int _lastScoredTurn = -999;
    private int _comboSoundIdx = -1;  // use for play sound  (increase one by one)

    private int CalculateBonusCount() => _rowCanScore / BoardManager.Ins.RowCount + _columnCanScore / BoardManager.Ins.ColumnCount;
    // private int ComboCount() => streakCount = (listScoredTurn.Count <= 1) ? 1 : (listScoredTurn[listScoredTurn.Count - 2] == nowTurn - 1) ? streakCount + 1 : 1;
    private int CalculateComboCount(bool isScoredThisTurn, int bonusCount)
    {
        int interval = GameConfig.Ins.ScoreConfig.IntervalCombo;
        // nếu lượt này không ăn
        if (!isScoredThisTurn)
        {
            // nếu đã quá 3 lượt không ăn → reset
            if (_nowTurn - _lastScoredTurn > interval - 1)
            {
                ComboCount = -1;
                _comboSoundIdx = -1;
            }

            // KHÔNG tăng combo
            return ComboCount;
        }

        // lượt này có ăn
        if (_nowTurn - _lastScoredTurn <= interval - 1)
        {
            if (ComboCount < 0)
            {
                ComboCount = 0;
            }
            else if (ComboCount == 0)
            {
                ComboCount = 1;
                _comboSoundIdx = 0;
            }
            else
            {
                ComboCount += bonusCount;
                _comboSoundIdx = Mathf.Min(_comboSoundIdx + 1, SoundManager.Ins.comboScoreFXs.Count - 1);
            }
        }
        else
        {
            ComboCount = 0;
            _comboSoundIdx = -1;
        }

        return ComboCount;
    }
    public TargetData LoadLevelByIndex(int level)
    {
        level = level % GameConfig.Ins.GameplayConfig.LevelToLoop;
        level = level <= 0 ? GameConfig.Ins.GameplayConfig.LevelToLoop : level;
        var currentLevelData = LevelJsonSystem.LoadLevelJson(level);

        if (currentLevelData == null)
        {
            Debug.LogError($"❌ Cannot load Level_{level}.json");
            return null;
        }

        return currentLevelData.target;
    }

    #endregion

    #region Cheat
    public void CheatWin()
    {
        if (GameplayManager.Ins.PlayMode == PlayMode.Classic || !GameplayManager.Ins.IsInGame) return;
        GameplayManager.Ins.OnWinGame(CurrentTargetData);
    }
    public void CheatAddScore(int score)
    {
        TotalScore += score;
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Classic:
                UpdateClassicScore();
                break;
            case PlayMode.Adventure:
                UpdateAdventureScore(null);
                break;
        }
    }
    #endregion

    #region Bomb
    public void OnBombUsed(List<Tile> tiles)
    {
        if (tiles == null) return;
        int comboCount = CalculateComboCount(true, 1);
        TotalScore += GetScoreBomb(tiles.Count, comboCount);
        switch (GameplayManager.Ins.PlayMode)
        {
            case PlayMode.Classic:
                UpdateClassicScore();
                break;
            case PlayMode.Adventure:
                UpdateAdventureScore(tiles);
                break;
        }
    }
    private int GetScoreBomb(int shapeCount, int comboCount)
    {
        // Start Play Effect
        PlayEffect(1, comboCount);
        int bonusScore = GetBonusScore(1);
        int comboScore = GetComboScore(1);
        Vibrate.Play(VibrateType.Selection);

        return 5 * shapeCount + bonusScore + comboScore;

    }
    #endregion
}

public enum TargetType
{
    Score,
    GoalIem,
    Both,
}
