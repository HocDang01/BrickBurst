using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemTarget : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Transform _targetObject;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _tickObject;
    public GoalItemType GoalItemType;

    private Vector3 _originScale;

    void Awake()
    {
        _originScale = _targetObject.localScale;
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
        _targetObject.gameObject.SetActive(false);
    }
    #region Set Immediate
    public void SetTargetCount(int count)
    {
        _tickObject.SetActive(false);
        _countText.gameObject.SetActive(true);
        _countText.text = count.ToString();
        _targetObject.DOKill();
        _targetObject.localScale = _originScale;
    }
    public void SetActiveTargetObject(bool active)
    {
        _targetObject.gameObject.SetActive(active);
    }
    #endregion

    #region Anim
    public void SetTargetCountAnim(int count)
    {
        if (count <= 0)
        {
            _countText.gameObject.SetActive(false);
            _tickObject.SetActive(true);
        }
        else
        {
            _countText.text = count.ToString();
            _countText.gameObject.SetActive(true);
        }
        _targetObject.DOKill();
        _targetObject.DOPunchScale(_originScale * 1.3f, 0.25f, 8, 0.8f);
    }

    public void PlayAnimCollect(int count)
    {
        if (count > 0)
        {
            AnimCollect(count);
        }
        else
        {
            AnimComplete();
        }
    }
    private void AnimCollect(int count)
    {
        _targetObject.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(_targetObject
            .DOScale(_originScale * 1.3f, 0.05f)
            .SetEase(Ease.OutBack));

        seq.AppendCallback(() =>
        {
            _countText.text = count.ToString();
        });

        seq.Append(_targetObject
            .DOScale(_originScale, 0.1f)
            .SetEase(Ease.OutBack));
    }
    private void AnimComplete()
    {
        _countText.gameObject.SetActive(false);
        _targetObject.DOKill();
        _targetObject.localScale = _originScale * 1.3f;
        _tickObject.SetActive(true);

        _targetObject.DOScale(_originScale, 0.1f).SetEase(Ease.OutBack);
    }
    #endregion

}
