using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class LikeEffect : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private float _duration;

    public void Init(Sprite sprite, Vector3 scale)
    {
        _icon.sprite = sprite;
        transform.DOKill();
        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(scale * 1.1f, _duration * 0.8f));
        sequence.Append(transform.DOScale(scale, _duration * 0.2f));
        sequence.AppendInterval(0.2f);
        sequence.AppendCallback(() =>
        {
            gameObject.SetActive(false);
        });
    }
    public void Init(TileColor tileColor, Vector3 scale, Vector3 pos)
    {
        transform.position = pos;
        // RectTransform rect = transform as RectTransform;

        // rect.sizeDelta = new Vector2(width, height);
        Init(tileColor.LikeIcon, scale);
    }

}
