using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BigComboEffect : MonoBehaviour
{
    [SerializeField] private float _duration;

    [Header("Ref UI Anim")]
    [SerializeField] private Transform _comboText;
    [SerializeField] private Transform _comboNumber;
    [SerializeField] private GameObject _light;

    [Header("Ref Number")]
    [SerializeField] private Image _num1;
    [SerializeField] private Image _num2;
    [SerializeField] private Image _num3;

    [SerializeField] private List<Sprite> _numSprites;

    private Sequence _sequence;


    void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
    public void Play(int combo)
    {
        if (combo < 0 || combo >= 1000) return;
        CalculateCombo(combo);
        gameObject.SetActive(true);
        PlayAnim();
    }

    private void PlayAnim()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _comboText.localScale = Vector3.zero;
        _comboNumber.localScale = Vector3.zero;

        // 1️⃣ POP mạnh ngay khi xuất hiện
        _sequence.Append(_comboText
            .DOScale(1f, 0.2f)
            .SetEase(Ease.OutBack));

        _sequence.Join(_comboNumber
            .DOScale(1.4f, 0.12f)
            .SetEase(Ease.OutBack));

        // 2️⃣ Giật ngược về size chuẩn
        _sequence.Append(_comboNumber
            .DOScale(1f, 0.1f)
            .SetEase(Ease.InOutSine));

        // 3️⃣ Punch giật mạnh (cảm giác impact)
        _sequence.Append(_comboNumber
            .DOPunchScale(Vector3.one * 0.3f, 0.25f, 8, 0.8f));

        _sequence.Join(_comboText
            .DOPunchScale(Vector3.one * 0.15f, 0.25f, 6, 0.6f));

        _sequence.Join(_comboNumber
            .DOShakePosition(0.25f, 10f, 15));

        // 4️⃣ Giữ lại 1 nhịp cho người nhìn
        // _sequence.AppendInterval(0.15f);

        // 5️⃣ Tắt nhanh – gọn – dứt khoát
        _sequence.Append(_comboText
            .DOScale(0f, 0.12f)
            .SetEase(Ease.InBack));

        _sequence.Join(_comboNumber
            .DOScale(0f, 0.12f)
            .SetEase(Ease.InBack));

        _sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }


    private void CalculateCombo(int combo)
    {
        _num1.transform.localScale = Vector3.one;
        _num2.transform.localScale = Vector3.one;
        _num3.transform.localScale = Vector3.one;
        if (combo >= 100)
        {
            int x3 = combo % 10;
            int x2 = Mathf.FloorToInt(combo / 10) % 10;
            int x1 = Mathf.FloorToInt(combo / 100) % 10;

            _num1.sprite = _numSprites[x1];
            _num2.sprite = _numSprites[x2];
            _num3.sprite = _numSprites[x3];

            _num1.gameObject.SetActive(true);
            _num2.gameObject.SetActive(true);
            _num3.gameObject.SetActive(true);

            _light.transform.localPosition = _num2.transform.localPosition;
        }
        else if (combo >= 10 && combo <= 99)
        {
            int x2 = combo % 10;
            int x1 = Mathf.FloorToInt(combo / 10) % 10;

            _num1.sprite = _numSprites[x1];
            _num2.sprite = _numSprites[x2];

            _num1.gameObject.SetActive(true);
            _num2.gameObject.SetActive(true);
            _num3.gameObject.SetActive(false);

            _light.transform.localPosition = (_num2.transform.localPosition + _num1.transform.localPosition) / 2;
        }
        else if (combo >= 0 && combo <= 9)
        {
            _num1.sprite = _numSprites[combo];

            _num1.gameObject.SetActive(true);
            _num2.gameObject.SetActive(false);
            _num3.gameObject.SetActive(false);

            _light.transform.localPosition = _num1.transform.localPosition;
        }
    }
}
