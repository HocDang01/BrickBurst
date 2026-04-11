using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TileBreakEffect : MonoBehaviour
{
    [SerializeField] private Image _light;
    [SerializeField] private Image _tileBreakImg;

    public void Init(Sprite sprite, Vector3 scale, float duration, Vector3 pos)
    {
        transform.position = pos;
        _tileBreakImg.sprite = sprite;
        transform.localScale = scale;
        StartAnim(duration);
        _light.gameObject.SetActive(false);
    }


    private void StartAnim(float duration)
    {
        // Sequence sequence = DOTween.Sequence();
        // _light.transform.localScale = Vector3.zero;
        _light.DOFade(0f, 0.0f);
        _tileBreakImg.DOFade(1f, 0f);
        // sequence.Append(_light.transform.DOScale(1.2f, duration * 0.3f));
        // sequence.Join(_light.DOFade(1f, duration * 0.3f));
        // sequence.Append(_light.transform.DOScale(1f, duration * 0.5f));
        // sequence.Join(_light.DOFade(0f, duration * 0.8f));
        // sequence.Join(_tileBreakImg.DOFade(0f, duration * 0.8f));

        _tileBreakImg.DOFade(0f, duration * 0.8f);

    }
    // private void LightAnim()
    // {
    //     _light.Do
    // }
}
