using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TileFallEffect : MonoBehaviour
{
    [SerializeField] private Image _tileImg;

    public void Init(TileColor tileColor, float duration, Vector3 scale, Vector3 pos)
    {
        transform.position = pos;
        transform.localScale = scale;
        _tileImg.sprite = tileColor.Tile;
        // StartFallDown(duration);
        Bomb();
    }
    public void Init(Sprite sprite, float duration, Vector3 scale, Vector3 pos)
    {
        _tileImg.DOFade(1f, 0.0f);
        transform.position = pos;
        transform.localScale = scale;
        _tileImg.sprite = sprite;
        // StartFallDown(duration);
        Bomb();
    }

    private void StartFallDown(float duration)
    {
        transform.DOKill();

        Vector3 startPos = transform.position;

        float jumpHeight = Random.Range(GameConfig.Ins.EffectConfig.MinJumpHeight, GameConfig.Ins.EffectConfig.MaxJumpHeight);
        float fallDistance = GameConfig.Ins.EffectConfig.FallDistance;

        float upDuration = GameConfig.Ins.EffectConfig.TimeJump;
        upDuration = Random.Range(upDuration * 0.8f, upDuration * 1.2f);
        float downDuration = GameConfig.Ins.EffectConfig.TimeFall;
        downDuration = Random.Range(downDuration * 0.8f, downDuration * 1.2f);

        float totalDuration = upDuration + downDuration;

        float randomRotation = Random.Range(-180f, 180f);

        float randomX = Random.Range(-50f, 50f);
        Sequence seq = DOTween.Sequence();

        // 1. Giật lên
        seq.Append(
            transform.DOMoveY(startPos.y + jumpHeight, upDuration)
                .SetEase(Ease.OutQuad)
        );
        seq.Join(
            transform.DOMoveX(startPos.x + randomX, upDuration)
                            .SetEase(Ease.OutQuad)
        );

        // 2. Rơi xuống
        seq.Append(
            transform.DOMoveY(startPos.y - fallDistance, downDuration)
                .SetEase(Ease.InQuad)
        );

        // 3. XOAY NGAY TỪ ĐẦU (song song toàn bộ sequence)
        seq.Join(
            transform.DORotate(
                new Vector3(0, 0, randomRotation),
                downDuration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.OutCubic)
        );
        seq.Join(_tileImg.DOFade(0f, totalDuration));
    }
    private void Bomb()
    {
        Vector3 startPos = transform.position;

        Vector2 dir = Random.insideUnitCircle.normalized;
        dir.y = Mathf.Abs(dir.y);

        float distance = Random.Range(6f, 12f);
        float jumpPower = Random.Range(GameConfig.Ins.EffectConfig.MinJumpHeight, GameConfig.Ins.EffectConfig.MaxJumpHeight);
        float duration = Random.Range(0.7f, 1.2f);

        Vector3 endPos = startPos + (Vector3)(dir * distance);
        endPos.y -= GameConfig.Ins.EffectConfig.FallDistance;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            transform.DOJump(
                endPos,
                jumpPower,   // độ cao
                1,           // số lần jump
                duration
            ).SetEase(Ease.Linear) // rất quan trọng
        );

        seq.Join(transform.DORotate(
            new Vector3(0, 0, Random.Range(-360, 360)),
            duration,
            RotateMode.FastBeyond360
        ));

        seq.Join(_tileImg.DOFade(0f, duration));
    }

}
