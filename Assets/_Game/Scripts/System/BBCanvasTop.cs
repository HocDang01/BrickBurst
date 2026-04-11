using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

    public class BBCanvasTop : MonoBehaviour
    {
        [SerializeField] private Transform _victoryTopPos;
        [SerializeField] private Image _victory;
        [SerializeField] private WelldoneEffect _welldoneEffect;

        private Vector3 _startVictoryPos;
        public static BBCanvasTop Ins;
        private void Awake()
        {
            Ins = this;
            _startVictoryPos = _victory.transform.localPosition;
            _victory.gameObject.SetActive(false);
        }
        void Start()
        {
            _welldoneEffect.gameObject.SetActive(false);
        }

        #region Victory
        public Transform GetImageVictory() => _victory.transform;
        public void PlayAnimVictory()
        {
            _victory.gameObject.SetActive(true);
            PlayAnim();
        }
        public float GetTimeVictoryEffect()
        {
            return 0.4f     // rơi + scale
                 + 0.18f
                 + 0.14f
                 + 0.14f
                + 0.6f + _welldoneEffect.GetTime();
        }
        private void PlayAnim()
        {
            _victory.transform.DOKill();
            _victory.DOFade(1f, 0f);

            // reset
            _victory.transform.localScale = Vector3.one * 3.5f;
            _victory.transform.localPosition = _startVictoryPos + Vector3.up * 600f;

            Sequence seq = DOTween.Sequence();

            seq.Append(_victory.transform.DOLocalMoveY(_startVictoryPos.y, 0.4f)   
                .SetEase(Ease.InQuad))

               .Join(_victory.transform.DOScale(1f, 0.4f)
                .SetEase(Ease.InQuad))

               // đập mạnh + nảy (cũng kéo dài hơn)
               .Append(_victory.transform.DOScale(1.25f, 0.18f).SetEase(Ease.OutQuad)) 
               .Append(_victory.transform.DOScale(0.9f, 0.14f).SetEase(Ease.InQuad))   
               .Append(_victory.transform.DOScale(1f, 0.14f).SetEase(Ease.OutQuad))    
                .AppendCallback(() =>
                {
                    // pulse scale
                    _victory.transform.DOScale(1.05f, 0.4f)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);

                    // float lên xuống
                    _victory.transform.DOLocalMoveY(_startVictoryPos.y + 20f, 0.8f)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                })
                .AppendInterval(0.6f)
                .AppendCallback(() =>
                {
                    _victory.transform.DOKill(); // stop pulse + float
                })
                .Append(_victory.transform.DOMove(_victoryTopPos.position, 0.6f).SetEase(Ease.OutQuad));
        }

        
        #endregion
        #region WellDone
        public Transform GetWellDone() => _welldoneEffect.MoveTransform;
        public void PlayWellDone(TargetData targetData)
        {
            _welldoneEffect.Play(targetData);
        }

        public void DisableWinEffect()
        {
            _victory.transform.DOKill();
            _victory.gameObject.SetActive(false);
            _welldoneEffect.DisableAllEffect();
            _welldoneEffect.gameObject.SetActive(false);
        }
        #endregion
    }
