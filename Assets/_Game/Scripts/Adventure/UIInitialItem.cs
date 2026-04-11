
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInitialItem : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _tickObject;
    public GoalItemType GoalItemType;


    void Awake()
    {
        var a = GameConfig.Ins.TileColorConfig.TileGoalItems.Find(e => e.GoalItemType == GoalItemType);
        if (a != null && a.Icon != null)
        {
            _icon.sprite = a.Icon;
        }
    }

    public void Init()
    {
        _countText.gameObject.SetActive(false);
        _tickObject.SetActive(false);
    }
    public void SetActiveCounttext(bool enabled)
    {
        _countText.gameObject.SetActive(enabled);
    }
    #region Set Immediate
    public void SetTargetCount(int count)
    {
        if (count <= 0)
        {
            SetDone();
            return;
        }
        _tickObject.SetActive(false);
        _countText.text = count.ToString();
        _countText.gameObject.SetActive(true);
    }
    public void SetDone()
    {
        _tickObject.SetActive(true);
        _countText.gameObject.SetActive(false);
    }
    #endregion

    #region Anim
    public void AnimVictory()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutSine);
    }
    #endregion

}
