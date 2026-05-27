using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PopupEndAdventure : PopupEndGame<PopupEndAdventure>
{
    [SerializeField] private Transform _goalTransform;

    [SerializeField] private Button _nextLevelBtn;
    [SerializeField] private Button _homeBtn;

    [Header("----------------------GoalItem----------------------")]
    [SerializeField] private GameObject _goalItems;
    [SerializeField] private List<UIInitialItem> _uIInitialItems;

    [Header("----------------------Score----------------------")]
    [SerializeField] private GameObject _score;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _targetScoreText;
    [SerializeField] private Slider _scoreSlider;



    private Transform _victory;
    private Transform _welldone;
    private Vector3 _goalPos;
    private Action _replayAction;
    private Action _nextLevelAction;
    private bool _isClicked;

    protected override void Awake()
    {
        base.Awake();
        _isClicked = false;
        _replayBtn.onClick.AddListener(Replay);
        _nextLevelBtn.onClick.AddListener(NextLevel);
        _homeBtn.onClick.AddListener(OnClickHome);
        _goalPos = _goalTransform.localPosition;
    }
    protected override void Start()
    {
        base.Start();
    }
    private void OnClickHome()
    {
        MainMenu.Show();
        GameEvents.OnBackMainMenu?.Invoke(true);
        Hide();
    }
    protected override void Replay()
    {
        base.Replay();
        if (_replayAction != null)
        {
            _replayAction?.Invoke();
        }
        Hide();
    }

    private void NextLevel()
    {
        if (_isClicked) return;
        _isClicked = true;
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
            _victory.gameObject.SetActive(false);
            _welldone.gameObject.SetActive(false);
            if (_nextLevelAction != null)
            {
                _nextLevelAction?.Invoke();
            }

            Hide();
        }
    }
    public void SetResult(TargetData targetData, int score, Dictionary<GoalItemType, int> goalItemTargets)
    {
        if (targetData == null)
        {
            Debug.LogError("Could not find targetData!!");
            return;
        }
        switch ((TargetType)targetData.TargetType)
        {
            case TargetType.Score:
                SetScore(targetData.score, score);
                break;
            case TargetType.GoalIem:
                SetGoalItem(targetData.goalItems, goalItemTargets);
                break;
        }
    }
    #region GoalItem
    private void SetGoalItem(List<GoalItemEntry> goalItemEntries, Dictionary<GoalItemType, int> GoalItemCollected)
    {
        _goalItems.SetActive(true);
        _score.SetActive(false);

        SetCurrentGoalItem(goalItemEntries);
        foreach (var goalItem in GoalItemCollected)
        {
            var item = _uIInitialItems.Find(e => e.GoalItemType == goalItem.Key);
            item.SetTargetCount(goalItem.Value);
        }
    }
    private void SetCurrentGoalItem(List<GoalItemEntry> goalItemTargets)
    {
        foreach (var goal in goalItemTargets)
        {
            var item = _uIInitialItems.Find(e => e.GoalItemType == (GoalItemType)goal.goalItem);
            if (goal.count > 0)
            {
                item.gameObject.SetActive(true);
                item.Init();
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
        foreach (var item in _uIInitialItems)
        {
            if (!goalItemTargets.Exists(e => e.goalItem == (int)item.GoalItemType))
            {
                item.gameObject.SetActive(false);
            }
        }
    }
    #endregion
    #region Score
    private void SetScore(int targetScore, int score)
    {
        _goalItems.SetActive(false);
        _score.SetActive(true);
        _scoreSlider.value = 0f;
        _scoreText.text = Utility.FormatNumber(score);
        _targetScoreText.text = Utility.FormatNumber(targetScore);
        StartCoroutine(IncreaseSlider(0.5f, score, targetScore));
    }
    private IEnumerator IncreaseSlider(float duration, float score, float target)
    {
        if (target <= 0)
            yield break;

        float startValue = _scoreSlider.value;
        float targetValue = score / target;

        targetValue = Mathf.Clamp01(targetValue);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (_scoreSlider.value >= 1) yield break;
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            _scoreSlider.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        _scoreSlider.value = targetValue; // đảm bảo chạm đúng
    }
    #endregion
    public void SetAction(Action replay, Action nextLevel)
    {
        _replayAction = replay;
        _nextLevelAction = nextLevel;
    }

    public void SetWin(bool isWin)
    {
        _nextLevelBtn.gameObject.SetActive(isWin);
        _replayBtn.gameObject.SetActive(!isWin);
        _titleImg.sprite = isWin ? _winSprite : _loseSprite;

        if (isWin)
        {
            StartAnimationWin();
        }
        else
        {
            StartAnimationLose();
        }

    }
    protected void StartAnimationLose()
    {
        _titleImg.gameObject.SetActive(true);
        // Reset states
        _titleRect.position = _centerPoint.position;
        // Reset scale
        _titleRect.localScale = Vector3.zero;
        _content.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence
            .SetUpdate(true)
            .Append(_titleRect.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack))
            .Append(_titleRect.DOScale(1f, 0.3f))
            .Append(_titleRect.DOLocalMove(_originPosTitle, 0.8f).SetEase(Ease.OutQuad))
            .Append(_content.DOScale(1f, 0.6f).SetEase(Ease.OutBack));
    }
    protected void StartAnimationWin()
    {
        _victory = BBCanvasTop.Ins.GetImageVictory();
        _welldone = BBCanvasTop.Ins.GetWellDone();
        _score.SetActive(false);
        _goalItems.SetActive(false);
        _titleImg.gameObject.SetActive(false);

        // Reset states
        // _titleRect.position = _centerPoint.position;
        // Reset scale
        // _titleRect.localScale = Vector3.zero;

        _content.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence
            .SetUpdate(true)
            .Append(_victory.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack))
            .Join(_welldone.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack))
            .Append(_victory.transform.DOScale(1f, 0.3f))
            .Join(_welldone.transform.DOScale(1f, 0.3f))
            .Append(_victory.transform.DOLocalMove(_originPosTitle, 0.8f).SetEase(Ease.OutQuad))
            .Join(_welldone.transform.DOLocalMove(_goalPos, 0.8f).SetEase(Ease.OutQuad))

            .AppendCallback(() =>
            {
                //@TODO: Spawn VFX Win
            })
            .Append(_content.DOScale(1f, 0.6f).SetEase(Ease.OutBack))
            .AppendInterval(0.3f)
            .AppendInterval(0.3f)

            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                
            }
            );
    }
    private bool CheckInterAds()
    {
        return false;
        // var gameplay = GameplayManager.Ins;
        // bool isNoAdsByMode = gameplay.MoneyModeEnum == MoneyModeEnum.M5M || gameplay.MoneyModeEnum == MoneyModeEnum.M10M;
        // int gap = BrickBurstConfig.Ins.MainMenuConfig.GetAdsGapByMoneyMode(gameplay.MoneyModeEnum);
        // if (gap <= 0)
        // {
        //     isNoAdsByMode = true;
        // }
        // else
        // {
        //     isNoAdsByMode = (gameplay.MilestoneIndexPrev + 1) % gap != 0;
        // }
        // Debug.Log($"MilestoneIndex: {gameplay.MilestoneIndexPrev + 1}");
        // Debug.Log($"Gap: {gap}");
        // Debug.Log($"isNoAdsByMode: {isNoAdsByMode}");
        // return !isNoAdsByMode && UserProperty.Vip == 0 && Ads.IsAdsEnabled(AdsType.INTERSTITIAL) && Ads.IsDisplayable(AdsType.INTERSTITIAL) && Ads.IsAvailable(AdsType.INTERSTITIAL);
    }
}
