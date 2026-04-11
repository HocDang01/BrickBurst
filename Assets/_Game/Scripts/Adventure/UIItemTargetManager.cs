using System.Collections.Generic;
using UnityEngine;
public class UIItemTargetManager : MonoBehaviour
{
    [SerializeField] private List<UIItemTarget> _itemTargets;

    /// <summary>
    /// Get Target from level
    /// </summary>
    /// <param name="goalItemTargets"></param>
    public void InitItemTarget(Dictionary<GoalItemType, int> goalItemTargets)
    {
        foreach (var goal in goalItemTargets)
        {
            var item = _itemTargets.Find(e => e.GoalItemType == goal.Key);
            if (goal.Value > 0)
            {
                item.gameObject.SetActive(true);
                item.Init();
                item.SetTargetCount(goal.Value);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
        foreach (var item in _itemTargets)
        {
            if (!goalItemTargets.ContainsKey(item.GoalItemType))
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    public void LoadItemTarget(Dictionary<GoalItemType, int> goalItemTargets)
    {
        foreach (var item in goalItemTargets)
        {
            UpdateItemTarget(item.Key, item.Value);
        }
    }

    public void UpdateItemTarget(GoalItemType goalItemType, int newCount)
    {
        var item = GetItemTarget(goalItemType);
        item.PlayAnimCollect(newCount);
    }
    public UIItemTarget GetItemTarget(GoalItemType goalItemType)
    {
        return _itemTargets.Find(e => e.GoalItemType == goalItemType);
    }
}
