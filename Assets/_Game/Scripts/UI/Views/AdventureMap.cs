using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventureMap : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private List<UILevelAdventureContainer> _uILevelAdventureContainers;
    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _startGameBtn;
    [SerializeField] private Image _startBtnImg;
    [SerializeField] private Sprite _enableBtnSprite;
    [SerializeField] private Sprite _disableBtnSprite;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _trophyFillImage;
    [SerializeField] private Image _trophyEmptyImage;
    [Header("Color")]
    [SerializeField] private Color _activateTextColor;
    [SerializeField] private Color _deactivateTextColor;
    [Header("Sprite")]
    [SerializeField] private Sprite _notVisitedSprite;
    [SerializeField] private Sprite _visitedSprite;
    [SerializeField] private Sprite _currentVisitSprite;

    [SerializeField] private List<Sprite> _fullTrophySprites;
    [SerializeField] private List<Sprite> _emptyTrophySprites;

    protected void Awake()
    {
        _startGameBtn.onClick.AddListener(() =>
        {
            GameplayManager.Ins.CanWatchAds = true;
            GameEvents.OnEnterGameplay?.Invoke();
            MainMenu.Hide();
            GameplayView.Show();
        });
        _backBtn.onClick.AddListener(OnClickBack);
    }


    void OnEnable()
    {
        SetDataUINew();
        CheckLevel();
    }

    private void OnClickBack()
    {
        GameEvents.OnBackMainMenu?.Invoke(true);
    }

    private void SetDataUINew()
    {
        int level = 0;
        if (BBManager.EnableCheat)
        {
            level = GameConfig.Ins.GameplayConfig.Level;
        }
        else
        {
            level = UserProperty.BrickBurstLevel;
        }
        int realLevel = level; // giữ nguyên level thật

        int maxLevel = GameConfig.Ins.GameplayConfig.MaxLevel;
        // level dùng để chọn container
        int displayLevel = ((realLevel - 1) % maxLevel) + 1;
        int levelStart = 1;
        int levelEnd = 0;
        int idx = -1;
        for (int i = 0; i < _uILevelAdventureContainers.Count; i++)
        {
            if (i == 0)
            {
                levelStart = 1;
            }
            else
            {

                levelStart = levelStart + _uILevelAdventureContainers[i - 1].GetCountLevel();
            }
            levelEnd = levelStart + _uILevelAdventureContainers[i].GetCountLevel() - 1;
            Debug.Log("LevelStart: " + levelStart);
            Debug.Log("LevelEnd: " + levelEnd);
            if (displayLevel <= levelEnd && displayLevel >= levelStart)
            {
                idx = i;
                _uILevelAdventureContainers[i].gameObject.SetActive(true);
                _uILevelAdventureContainers[i].Init(levelStart, realLevel, maxLevel);
                _trophyFillImage.fillAmount = (float)(displayLevel - levelStart) / (float)(levelEnd - levelStart + 1);
            }
            else
            {
                _uILevelAdventureContainers[i].gameObject.SetActive(false);
            }
        }
        if (idx < 0)
        {
            _trophyEmptyImage.sprite = _emptyTrophySprites[0];
            _trophyFillImage.sprite = _fullTrophySprites[0];
            return;
        }
        if (idx >= _emptyTrophySprites.Count)
        {
            _trophyEmptyImage.sprite = _emptyTrophySprites[^1];
            _trophyFillImage.sprite = _fullTrophySprites[^1];
            return;
        }
        _trophyEmptyImage.sprite = _emptyTrophySprites[idx];
        _trophyFillImage.sprite = _fullTrophySprites[idx];
    }
    private void CheckLevel()
    {
        if (BBManager.EnableCheat)
        {
            _levelText.text = $"Level {GameConfig.Ins.GameplayConfig.Level}";
            _startGameBtn.enabled = true;
            _startBtnImg.sprite = _enableBtnSprite;
            // }
        }
        else
        {
            _levelText.text = $"Level {UserProperty.BrickBurstLevel}";
            _startGameBtn.enabled = true;
            _startBtnImg.sprite = _enableBtnSprite;
        }
    }
    // private void CheckTrophy()
    // {
    //     int level = 0;
    //     if (BBManager.EnableCheat)
    //     {
    //         level = GameConfig.Ins.GameplayConfig.Level;
    //     }
    //     else
    //     {
    //         level = UserProperty.BrickBurstLevel;
    //     }
    //     var config = GameConfig.Ins.MainMenuConfig.AdventureLevelConfigs;
    //     int idx = -1;
    //     for (int i = 0; i < config.Count; i++)
    //     {
    //         if (level >= config[i].MinLevel && level <= config[i].MaxLevel)
    //         {
    //             idx = i;
    //             break;
    //         }
    //     }
    //     if (idx < 0) return;
    //     _trophyFillImage.fillAmount = (float)(level - config[idx].MinLevel) / (float)(config[idx].MaxLevel - config[idx].MinLevel + 1);
    // }
    private void CheckMaxLevel()
    {
        int level = 0;
        if (BBManager.EnableCheat)
        {
            level = GameConfig.Ins.GameplayConfig.Level;
            if (level > 24)
            {
                GameConfig.Ins.GameplayConfig.Level = 1;
            }
        }
        else
        {
            level = UserProperty.BrickBurstLevel;
            if (level > 24)
            {
                UserProperty.BrickBurstLevel = 1;
            }
        }
    }
}

