using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

    public class StarScoreEffect : MonoBehaviour
    {
        [SerializeField] private Image _mainStarImg;
        [SerializeField] private Image _roundStarImg;
        [SerializeField] private List<Image> _smallStars;
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private float _initScale = 1f;
        [SerializeField] private float _targetScale = 1.2f;

        private float _startScale = 0f;
        private Vector3 _originScale;

        void Awake()
        {
            _startScale = 0f;
            _originScale = _roundStarImg.transform.localScale;
            _mainStarImg.gameObject.SetActive(false);
            _roundStarImg.gameObject.SetActive(false);
            foreach (var a in _smallStars)
            {
                a.gameObject.SetActive(false);
            }
        }
        private Tween _loopTween;

        public void StartCombo()
        {
            _mainStarImg.gameObject.SetActive(true);
            _roundStarImg.gameObject.SetActive(false);

            // Kill loop cũ nếu có
            _loopTween?.Kill();

            // Reset trạng thái
            _mainStarImg.transform.localScale = Vector3.zero;
            _mainStarImg.color = new Color(1, 1, 1, 0);

            Sequence intro = DOTween.Sequence();
            intro.SetUpdate(true);

            // =========================
            // 1️⃣ INTRO: 0 → target
            // =========================
            intro.Append(
                _mainStarImg.transform
                    .DOScale(_targetScale, _duration)
                    .SetEase(Ease.OutBack)
            );

            intro.Join(
                _mainStarImg
                    .DOFade(1f, _duration)
                    .SetEase(Ease.InOutSine)
            );

            // =========================
            // 2️⃣ START LOOP SAU INTRO
            // =========================
            intro.OnComplete(StartLoop);
        }
        private void StartLoop()
        {
            _loopTween = DOTween.Sequence()
                .Append(
                    _mainStarImg.transform
                        .DOScale(_initScale, _duration)
                        .SetEase(Ease.InOutSine)
                )
                .Join(
                    _mainStarImg
                        .DOFade(0.7f, _duration)
                        .SetEase(Ease.InOutSine)
                )
                .Append(
                    _mainStarImg.transform
                        .DOScale(_targetScale, _duration)
                        .SetEase(Ease.InOutSine)
                )
                .Join(
                    _mainStarImg
                        .DOFade(1f, _duration)
                        .SetEase(Ease.InOutSine)
                )
                .SetLoops(-1)
                .SetUpdate(true);
        }


        public void EndCombo()
        {
            ResetDOTWeen();
            _roundStarImg.gameObject.SetActive(false);
            _mainStarImg.transform.DOScale(0f, _duration * 0.5f).SetEase(Ease.OutBack);
            _mainStarImg.DOFade(0f, _duration * 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _mainStarImg.gameObject.SetActive(false);
            });
        }

        public void PlayStrongEffect(float strongDuration)
        {
            ResetDOTWeen();

            _mainStarImg.gameObject.SetActive(true);
            _roundStarImg.gameObject.SetActive(true);

            // =========================
            // RESET
            // =========================
            _mainStarImg.transform.localScale = Vector3.zero;
            _roundStarImg.transform.localScale = Vector3.zero;

            _mainStarImg.color = new Color(1, 1, 1, 1f);
            _roundStarImg.color = new Color(1, 1, 1, 0.8f);

            float hitTime = strongDuration * 0.35f; // đập nhanh
            float settleTime = strongDuration * 0.15f; // về form

            PlaySmallStarsBurst(strongDuration);
            // =========================
            // MAIN STAR – ĐẬP PHÁT ĂN NGAY
            // =========================
            Sequence mainSeq = DOTween.Sequence();

            mainSeq.Append(
                _mainStarImg.transform
                    .DOScale(_targetScale * 1.25f, hitTime)
                    .SetEase(Ease.OutBack, 3.5f)   // gắt hơn
            );

            mainSeq.Append(
                _mainStarImg.transform
                    .DOScale(_targetScale, settleTime)
                    .SetEase(Ease.OutSine)
            );

            // punch thêm cho đã tay
            mainSeq.Append(
                _mainStarImg.transform
                    .DOPunchScale(Vector3.one * 0.15f, 0.15f, 8, 1)
            );

            // =========================
            // ROUND STAR – BÙM RỘNG + BIẾN MẤT NHANH
            // =========================
            Sequence roundSeq = DOTween.Sequence();

            roundSeq.Append(
                _roundStarImg.transform
                    .DOScale(_targetScale * 3.5f, hitTime)
                    .SetEase(Ease.OutExpo)
            );

            roundSeq.Join(
                _roundStarImg
                    .DOFade(0f, hitTime)
            );

            roundSeq.Join(
                _roundStarImg.transform
                    .DORotate(new Vector3(0, 0, Random.Range(-30f, 30f)), hitTime, RotateMode.FastBeyond360)
            );

            roundSeq.OnComplete(() =>
            {
                _roundStarImg.gameObject.SetActive(false);
                _roundStarImg.transform.localScale = _originScale;
                _roundStarImg.transform.rotation = Quaternion.identity;
            });

            // =========================
            // SYNC + COMBO
            // =========================
            DOTween.Sequence()
                .Append(mainSeq)
                .Join(roundSeq)
                .OnComplete(StartCombo);
        }


        private void PlaySmallStarsBurst(float hitTime)
        {
            foreach (var star in _smallStars)
            {
                star.gameObject.SetActive(true);

                // Reset
                star.transform.localPosition = Vector3.zero;
                star.transform.localScale = Vector3.one;
                star.color = new Color(1, 1, 1, 1f);

                // Random hướng
                Vector2 dir = Random.insideUnitCircle.normalized;
                float distance = Random.Range(80f, 140f); // chỉnh độ xa gần
                Vector3 targetPos = dir * distance;

                float moveTime = hitTime * Random.Range(0.8f, 1.1f);

                Sequence seq = DOTween.Sequence();

                // bay ra
                seq.Append(
                    star.transform
                        .DOLocalMove(targetPos, moveTime)
                        .SetEase(Ease.OutCubic)
                );

                // nhỏ dần
                seq.Join(
                    star.transform
                        .DOScale(0.3f, moveTime)
                        .SetEase(Ease.InQuad)
                );

                // fade dần
                seq.Join(
                    star
                        .DOFade(0f, moveTime)
                        .SetEase(Ease.InQuad)
                );

                seq.OnComplete(() =>
                {
                    star.gameObject.SetActive(false);
                });
            }
        }

        private void ResetDOTWeen()
        {
            _mainStarImg.transform.DOKill();
            _mainStarImg.DOKill();
            _roundStarImg.transform.DOKill();
            _roundStarImg.DOKill();
            _loopTween?.Kill();
        }
    }
