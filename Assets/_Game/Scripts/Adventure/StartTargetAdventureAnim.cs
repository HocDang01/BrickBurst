using System;
using System.Collections;
using System.Collections.Generic;
using DangExtension;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class StartTargetAdventureAnim : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject _overlay;
    [SerializeField] private TextMeshProUGUI _titleText;
    [Header("Score")]
    [SerializeField] private GameObject _scoreMove;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _scoreTarget;

    [Header("Item Goal")]
    [SerializeField] private GameObject _itemTarget;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayout;
    [SerializeField] private List<UIInitialItem> _uIItemTargets;
    [SerializeField] private UIItemTargetManager _uIItemTargetManager;

    private List<UIInitialItem> _currentUIItemTarget;
    private Vector3 _scoreMoveOriginPos;

    void Awake()
    {
        _scoreMoveOriginPos = _scoreMove.transform.localPosition;
        _currentUIItemTarget = new();
    }
    public void StartAnim(TargetData targetData)
    {
        if (GameplayManager.Ins.PlayMode == PlayMode.Classic || targetData == null)
        {
            _overlay.SetActive(false);
            _scoreMove.SetActive(false);
            _itemTarget.SetActive(false);
            return;
        }
        switch ((TargetType)targetData.TargetType)
        {
            case TargetType.Score:
                ScoreMode(targetData);
                break;
            case TargetType.GoalIem:
                ItemTargetMode(targetData);
                break;
            case TargetType.Both:
                break;
        }
    }

    private void ItemTargetMode(TargetData targetData)
    {
        _horizontalLayout.enabled = true;
        _titleText.text = "Target Collection";
        _overlay.SetActive(true);
        _itemTarget.SetActive(true);
        _scoreMove.SetActive(false);

        var goalItemTargets = Utility.GoalItemListToDict(targetData.goalItems);
        SetCurrentGoalItem(goalItemTargets);
        foreach (var itemMove in _currentUIItemTarget)
        {
            _uIItemTargetManager.GetItemTarget(itemMove.GoalItemType).SetActiveTargetObject(false);
        }
        _overlay.transform.localScale = Vector3.one;
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.8f, () =>
            {
                StartCoroutine(ItemMoveToTargetAnim());
            });
        });
    }
    private IEnumerator ItemMoveToTargetAnim()
    {
        _horizontalLayout.enabled = false;
        foreach (UIInitialItem itemMove in _currentUIItemTarget)
        {
            UIItemTarget target = _uIItemTargetManager.GetItemTarget(itemMove.GoalItemType);
            target.transform.localScale = Vector3.one;
            itemMove.SetActiveCounttext(false);
            FlyToTargetAnim(
                itemMove.transform,
                target.transform,
                0.6f,
                5f,
                () =>
                {
                    itemMove.gameObject.SetActive(false);
                    target.gameObject.SetActive(true);
                    target.SetActiveTargetObject(true);
                    target.transform.localScale = Vector3.one;
                    target.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f, 8, 0.8f).OnComplete(() =>
                    {
                        target.transform.localScale = Vector3.one;
                    });
                }
            );
            yield return WaitTimeCache.Wait0_1;
        }
        _horizontalLayout.enabled = true;
        yield return WaitTimeCache.Wait0_5;
        _overlay.transform.DOScale(0f, 0.2f).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    _overlay.SetActive(false);
                });
    }
    private void SetCurrentGoalItem(Dictionary<GoalItemType, int> goalItemTargets)
    {
        _currentUIItemTarget.Clear();
        foreach (var goal in goalItemTargets)
        {
            var item = _uIItemTargets.Find(e => e.GoalItemType == goal.Key);
            if (goal.Value > 0)
            {
                item.gameObject.SetActive(true);
                item.Init();
                item.SetTargetCount(goal.Value);
                _currentUIItemTarget.Add(item);
                item.transform.localScale = Vector3.one;
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
        foreach (var item in _uIItemTargets)
        {
            if (!goalItemTargets.ContainsKey(item.GoalItemType))
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    #region ScoreMode
    private void ScoreMode(TargetData targetData)
    {
        _titleText.text = "Target Score";
        _overlay.SetActive(true);
        _scoreMove.SetActive(true);
        _scoreTarget.SetActive(false);
        _itemTarget.SetActive(false);

        _scoreText.text = targetData.score.ToString();
        _scoreMove.transform.localPosition = _scoreMoveOriginPos;
        _overlay.transform.localScale = Vector3.one;
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            ScoreMoveToTargetAnim();
        });
    }
    private void ScoreMoveToTargetAnim()
    {
        _scoreMove.SetActive(true);
        _scoreTarget.SetActive(false);

        FlyToTargetAnim(
            _scoreMove.transform,
            _scoreTarget.transform,
            0.6f,
            7f,
            () =>
            {
                _scoreMove.SetActive(false);
                _scoreTarget.SetActive(true);
                _overlay.SetActive(false);

                _scoreTarget.transform
                    .DOPunchScale(Vector3.one * 0.25f, 0.25f, 8, 0.8f);
                _overlay.transform.DOScale(0f, 0.2f).SetEase(Ease.OutSine).OnComplete(() =>
               {
                   _overlay.SetActive(false);
               });
            }
        );
    }
    #endregion

    #region AnimFly
    private void FlyToTargetAnim(Transform moveTf, Transform targetTf, float duration = 0.6f, float arcDepth = 50f, Action onComplete = null)
    {
        if (moveTf == null || targetTf == null) return;

        moveTf.gameObject.SetActive(true);

        Vector3 startPos = moveTf.position;
        Vector3 endPos = targetTf.position;

        // Mid point tạo vòng cung
        Vector3 midPos = (startPos + endPos) * 0.5f;
        midPos.y -= arcDepth;

        Sequence seq = DOTween.Sequence();

        // Bay lên nhẹ
        seq.Append(
            moveTf.DOMove(midPos, duration * 0.45f)
                  .SetEase(Ease.OutQuad)
        );

        // Lao mạnh xuống target
        seq.Append(
            moveTf.DOMove(endPos, duration * 0.55f)
                  .SetEase(Ease.InBack)
        );

        // Scale tăng lực
        seq.Join(
            moveTf.DOScale(1.3f, duration * 0.3f)
                  .SetEase(Ease.OutBack)
        );

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    #endregion
}