using System.Collections.Generic;
using UnityEngine;

public class UILevelAdventureContainer : MonoBehaviour
{
    [SerializeField] private List<UILevelAdventure> _uILevelAdventures;

    [Header("Color")]
    [SerializeField] private Color _activateTextColor;
    [SerializeField] private Color _deactivateTextColor;
    [Header("Sprite")]
    [SerializeField] private Sprite _notVisitedSprite;
    [SerializeField] private Sprite _visitedSprite;
    [SerializeField] private Sprite _currentVisitSprite;

    public void Init(int levelStart)
    {

        int level = levelStart;
        for (int i = _uILevelAdventures.Count - 1; i >= 0; i--)
        {
            bool isCurrentLevel = false;
            bool isOvercome = false;
            if (BBManager.EnableCheat)
            {
                isCurrentLevel = (level) == GameConfig.Ins.GameplayConfig.Level;
                isOvercome = level < GameConfig.Ins.GameplayConfig.Level;
            }
            else
            {
                isCurrentLevel = (level) == BBSaveData.Ins.Level;
                isOvercome = level < BBSaveData.Ins.Level;
            }

            Color color = isCurrentLevel ? _activateTextColor : _deactivateTextColor;
            _uILevelAdventures[i].SetData(level, color, isOvercome, isCurrentLevel);
            if (isCurrentLevel)
            {
                _uILevelAdventures[i].SetSprite(_currentVisitSprite);
            }
            else if (isOvercome)
            {
                _uILevelAdventures[i].SetSprite(_visitedSprite);
            }
            else if (!isOvercome)
            {
                _uILevelAdventures[i].SetSprite(_notVisitedSprite);
            }
            level++;
        }
    }
    public void Init(int levelStart, int realLevel, int maxLevel)
    {
        int loopIndex = (realLevel - 1) / maxLevel;
        int loopOffset = loopIndex * maxLevel;

        int level = levelStart;

        for (int i = _uILevelAdventures.Count - 1; i >= 0; i--)
        {
            int absoluteLevel = level + loopOffset;

            bool isCurrentLevel = absoluteLevel == realLevel;
            bool isOvercome = absoluteLevel < realLevel;

            Color color = isCurrentLevel ? _activateTextColor : _deactivateTextColor;

            _uILevelAdventures[i].SetData(
                absoluteLevel,   // hiển thị: 271, 272, 273...
                color,
                isOvercome,
                isCurrentLevel
            );

            if (isCurrentLevel)
                _uILevelAdventures[i].SetSprite(_currentVisitSprite);
            else if (isOvercome)
                _uILevelAdventures[i].SetSprite(_visitedSprite);
            else
                _uILevelAdventures[i].SetSprite(_notVisitedSprite);

            level++;
        }
    }

    public int GetCountLevel()
    {
        return _uILevelAdventures == null ? 0 : _uILevelAdventures.Count;
    }

}
