using System.Collections;
using System.Collections.Generic;
using DangExtension;
using DG.Tweening;
using TMPro;
using UnityEngine;
public class WelldoneEffect : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject _overlay;
    [SerializeField] private GameObject _welldone;

    [Header("GoalItem")]
    [SerializeField] private GameObject _goalItems;
    [SerializeField] private List<UIInitialItem> _uIInitialItems;

    [Header("Score")]
    [SerializeField] private GameObject _scoreObject;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [Header("Effect")]
    [SerializeField] private GameObject _hitEffectPrefab;

    public Transform MoveTransform;


    private Vector3 _welldoneOriginPos;
    private Vector3 _scoreOriginScale;

    private void Awake()
    {
        _welldoneOriginPos = _welldone.transform.localPosition;
        _scoreOriginScale = _scoreObject.transform.localScale;
    }

    public void Play(TargetData targetData)
    {
        if (targetData == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        StopAllCoroutines();

        _overlay.SetActive(true);
        _welldone.SetActive(false);
        _goalItems.SetActive(false);
        _scoreObject.SetActive(false);

        switch ((TargetType)targetData.TargetType)
        {
            case TargetType.Score:
                StartCoroutine(ScoreFlowRoutine(targetData.score));
                break;
            case TargetType.GoalIem:
                StartCoroutine(ItemFlowRoutine(targetData.goalItems));
                break;
        }


    }
    public float GetTime()
    {
        float time = 0f;

        // 1. Overlay delay
        time += 0.25f;

        // 2. Score pop in
        time += 0.25f;
        time += 0.2f;

        // 3. Increase score
        time += 0.4f;

        // 4. Impact score
        time += 0.16f + 0.12f + 0.12f;

        // 5. Delay trước welldone
        time += 0.2f;

        // 6. Welldone pop + wave
        time += 0.5f;
        time += 0.25f;
        time += 0.2f;
        time += 0.2f;
        time += 0.3f;

        return time; // ~2.6s
    }

    #region ItemMode
    private IEnumerator ItemFlowRoutine(List<GoalItemEntry> goalItemEntrys)
    {
        // 1. Overlay lên trước
        yield return WaitTimeCache.Wait0_25; // 0.2 -> 0.5

        _goalItems.SetActive(true);
        _goalItems.transform.localScale = Vector3.zero;
        _goalItems.transform.DOScale(_scoreOriginScale, 0.25f).SetEase(Ease.OutBack); // 0.25
        foreach (var goalItem in goalItemEntrys)
        {
            var initItem = _uIInitialItems.Find(e => e.GoalItemType == (GoalItemType)goalItem.goalItem);
            if (goalItem.count <= 0)
            {
                initItem.gameObject.SetActive(false);
                continue;
            }
            initItem.gameObject.SetActive(true);
            initItem.SetDone();
        }

        // 5. Delay trước welldone
        yield return WaitTimeCache.Wait0_5;
        PlayNewWelldoneWave();
    }
    #endregion


    #region ScoreMode
    private IEnumerator ScoreFlowRoutine(int targetScore)
    {
        // 1. Overlay lên trước
        yield return WaitTimeCache.Wait0_25; // 0.2 -> 0.5

        // 2. Show score
        _scoreObject.SetActive(true);
        _scoreObject.transform.localScale = Vector3.zero;
        _scoreObject.transform.DOScale(_scoreOriginScale, 0.25f).SetEase(Ease.OutBack); // 0.25

        _scoreText.text = "0";
        yield return WaitTimeCache.Wait0_2;

        // 3. Chạy số
        yield return StartCoroutine(IncreaseScoreRoutine(targetScore));

        // 4. Nhún mạnh + spawn effect
        yield return PlayScoreImpact();

        // 5. Delay trước welldone
        yield return WaitTimeCache.Wait0_2;
        PlayNewWelldoneWave();
    }


    private IEnumerator IncreaseScoreRoutine(int target)
    {
        int score = 0;

        float duration = 0.4f; // tổng thời gian chạy số
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / duration);
            score = Mathf.RoundToInt(Mathf.Lerp(0, target, percent));
            _scoreText.text = score.ToString();
            yield return null;
        }

        _scoreText.text = target.ToString();
    }

    private IEnumerator PlayScoreImpact()
    {
        Transform tf = _scoreObject.transform;

        Sequence seq = DOTween.Sequence();
        seq.Append(tf.DOScale(1.3f, 0.16f).SetEase(Ease.OutQuad)); // 0.08 -> 0.16
        seq.Append(tf.DOScale(0.88f, 0.12f).SetEase(Ease.InQuad)); // 0.06 -> 0.12
        seq.Append(tf.DOScale(1f, 0.12f).SetEase(Ease.OutQuad));  // 0.05 -> 0.12

        if (_hitEffectPrefab != null)
        {
            Instantiate(_hitEffectPrefab, tf.position, Quaternion.identity, tf.parent);
        }

        yield return seq.WaitForCompletion();
    }


    private void PlayWelldoneWave()
    {
        _welldone.SetActive(true);
        _welldone.transform.DOKill();

        _welldone.transform.position = _welldoneOriginPos;
        _welldone.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        // pop vào (chậm hơn, nặng hơn)
        seq.Append(_welldone.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        // gợn sóng (wave)
        seq.Append(_welldone.transform.DOScaleX(1.12f, 0.25f).SetEase(Ease.OutQuad));
        seq.Join(_welldone.transform.DOScaleY(0.9f, 0.25f).SetEase(Ease.OutQuad));

        seq.Append(_welldone.transform.DOScaleX(0.95f, 0.2f).SetEase(Ease.InOutQuad));
        seq.Join(_welldone.transform.DOScaleY(1.05f, 0.2f).SetEase(Ease.InOutQuad));

        seq.Append(_welldone.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad));
        seq.AppendInterval(0.8f);

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    #endregion

    #region NewWelldone
    private void PlayNewWelldoneWave()
    {
        _welldone.SetActive(true);
        _welldone.transform.DOKill();

        _welldone.transform.localPosition = _welldoneOriginPos;
        _welldone.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        // pop vào (chậm hơn, nặng hơn)
        seq.Append(_welldone.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        // gợn sóng (wave)
        seq.Append(_welldone.transform.DOScaleX(1.12f, 0.25f).SetEase(Ease.OutQuad));
        seq.Join(_welldone.transform.DOScaleY(0.9f, 0.25f).SetEase(Ease.OutQuad));

        seq.Append(_welldone.transform.DOScaleX(0.95f, 0.2f).SetEase(Ease.InOutQuad));
        seq.Join(_welldone.transform.DOScaleY(1.05f, 0.2f).SetEase(Ease.InOutQuad));

        seq.Append(_welldone.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad));
        seq.AppendInterval(0.4f);
        seq.Append(_welldone.transform.DOScale(0f, 0.4f));

        seq.OnComplete(() =>
        {
            _overlay.SetActive(false);
            _welldone.SetActive(false);
        });
    }
    #endregion
    public void DisableAllEffect()
    {
        StopAllCoroutines();
        _welldone.SetActive(false);
        gameObject.SetActive(false);
    }
}
