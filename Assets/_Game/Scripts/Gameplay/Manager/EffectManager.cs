using System.Collections;
using System.Collections.Generic;
using CoreDang;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class EffectManager : MonoBehaviour
{
    [SerializeField] private TileBreakEffect _tileBreakEffect;
    [SerializeField] private TileFallEffect _tileFallDownEffect;
    [SerializeField] private LikeEffect _likeEffect;
    [SerializeField] private Image _gemFlyEffect;
    [SerializeField] private NotifyTextEffect _notifyTextEffect;

    [Header("--------------------VFX---------------------")]
    [SerializeField] private GameObject _confettiBlastPrefab;
    [SerializeField] private GameObject _confettiDirectionalPrefab;
    [SerializeField] private GameObject _flashEllow;
    [SerializeField] private GameObject _flashEllowPink;
    [SerializeField] private float _minX = -400f;
    [SerializeField] private float _maxX = 400f;
    [SerializeField] private float _minY = -700f;
    [SerializeField] private float _maxY = 700f;

    [Header("----------------Effect Victory------------------")]
    [SerializeField] private VictoryEffect _victoryEffect;
    [SerializeField] private WelldoneEffect _welldoneEffect;

    [Header("--------------Ref Board HightLight-----------------")]
    [SerializeField] private Image _hightLightBoardImg;
    [SerializeField] private Image _boardComboImg;
    [SerializeField] private Sprite _hightLightBoard1;
    [SerializeField] private Sprite _hightLightBoard2;

    [Header("----------------Ref GoalItem Target------------------")]
    [SerializeField] private List<UIItemTarget> _itemTargets;

    [Header("--------------Start Target Adenture Anim------------------")]
    [SerializeField] private StartTargetAdventureAnim _startTargetAdventureAnim;
    [Header("-----------------Milestone Gameplay-----------------")]
    [SerializeField] private Transform _centerTransform;

    private Dictionary<GoalItemType, Sprite> _itemGoalDict;
    private WaitForSeconds _wait0_3;
    public static EffectManager Ins;
    void Awake()
    {
        Ins = this;
        _itemGoalDict = new();
        _wait0_3 = new(0.3f);
        EffectPoolInstalling();
    }
    void Start()
    {
        _victoryEffect.gameObject.SetActive(false);
        _welldoneEffect.gameObject.SetActive(false);
        _startTargetAdventureAnim.gameObject.SetActive(false);
    }
    private void EffectPoolInstalling()
    {
        foreach (var tileGoalItem in GameConfig.Ins.TileColorConfig.TileGoalItems)
        {
            _itemGoalDict[tileGoalItem.GoalItemType] = tileGoalItem.Icon;
        }
    }
    #region BreakTile
    public void RequireBreakTile(TileColor tileColor, Vector3 pos)
    {
        var effect = PoolSystem.RequireObject(_tileBreakEffect);
        effect.transform.SetParent(transform);
        effect.Init(tileColor.TileBreak, BoardManager.Ins.SquareScale, 0.8f, pos);
        DOVirtual.DelayedCall(0.8f, () =>
        {
            PoolSystem.ReturnObject(effect);
        });

        var tile = PoolSystem.RequireObject(_tileFallDownEffect);
        tile.transform.SetParent(transform);
        tile.Init(tileColor, 2f, BoardManager.Ins.SquareScale, pos);
        DOVirtual.DelayedCall(2f, () =>
        {
            PoolSystem.ReturnObject(tile);
        });
    }
    public void RequireBreakTile(Sprite sprite, Vector3 pos)
    {
        var tile = PoolSystem.RequireObject(_tileFallDownEffect);
        tile.transform.SetParent(transform);
        tile.Init(sprite, 2f, BoardManager.Ins.SquareScale, pos);
        DOVirtual.DelayedCall(2f, () =>
        {
            PoolSystem.ReturnObject(tile);
        });
    }
    public void RequireLikeEffect(TileColor tileColor, Vector3 pos)
    {
        var effect = PoolSystem.RequireObject(_likeEffect);
        effect.transform.SetParent(transform);
        effect.Init(tileColor, BoardManager.Ins.SquareScale * 0.9f, pos);
        DOVirtual.DelayedCall(0.8f, () =>
        {
            PoolSystem.ReturnObject(effect);
        });
    }
    #endregion

    #region WinAdventure
    public void WinAdventure(TargetData targetData)
    {
        float time = _victoryEffect.GetTime();
        BBCanvasTop.Ins.PlayAnimVictory();

        DOVirtual.DelayedCall(0.2f, () =>
        {
            StartCoroutine(SpawnFlash());
            StartCoroutine(SpawnConfetti());
        });
        time += 0.3f;
        DOVirtual.DelayedCall(time, () =>
        {
            BBCanvasTop.Ins.PlayWellDone(targetData);
        });
    }
    // public void WinAdventure(TargetData targetData)
    // {
    //     float time = _victoryEffect.GetTime();
    //     _victoryEffect.Play();
    //     DOVirtual.DelayedCall(0.2f, () =>
    //     {
    //         StartCoroutine(SpawnFlash());
    //         StartCoroutine(SpawnConfetti());
    //     });
    //     time += 0.3f;
    //     DOVirtual.DelayedCall(time, () =>
    //     {
    //         _welldoneEffect.Play(targetData);
    //     });
    // }
    public float GetTimeWinAdventure()
    {
        float time = _victoryEffect.GetTime();
        time += 0.3f;
        time += _welldoneEffect.GetTime();
        time += 0.2f;
        return time;
    }
    private IEnumerator SpawnFlash()
    {
        Vector3 scale = Vector3.one * 40f;
        for (int i = 0; i < 4; i++)
        {
            var flash = Random.Range(0, 2) == 1 ? PoolSystem.RequireObject(_flashEllow) :
                                                PoolSystem.RequireObject(_flashEllowPink);
            flash.transform.SetParent(transform);
            flash.transform.localPosition = new Vector3(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY), 10f);
            flash.transform.localScale = scale;
            DOVirtual.DelayedCall(2f, () =>
            {
                PoolSystem.ReturnObject(flash);
            });
            yield return _wait0_3;
        }
    }
    private IEnumerator SpawnConfetti()
    {
        Vector3 scale = Vector3.one * 60f;
        for (int i = 0; i < 4; i++)
        {
            var confetti = Random.Range(0, 2) == 1 ? PoolSystem.RequireObject(_confettiBlastPrefab) :
                                                PoolSystem.RequireObject(_confettiDirectionalPrefab);
            confetti.transform.SetParent(transform);
            confetti.transform.localPosition = new Vector3(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY), 10f);
            confetti.transform.localScale = scale;
            DOVirtual.DelayedCall(2f, () =>
            {
                PoolSystem.ReturnObject(confetti);
            });
            yield return _wait0_3;
        }
    }
    #endregion

    #region Clear Row/Col Effect
    public void PlayRowClearEffect(TileColor currentTileColor, Vector3 pos)
    {
        var effect = PoolSystem.RequireObject(currentTileColor.FinishFXHorizontal);
        effect.transform.position = pos;
        DOVirtual.DelayedCall(2f, () =>
        {
            PoolSystem.ReturnObject(effect);
        });
    }
    public void PlayColumnClearEffect(TileColor currentTileColor, Vector3 pos)
    {
        var effect = PoolSystem.RequireObject(currentTileColor.FinishFXVertical);
        effect.transform.position = pos;
        DOVirtual.DelayedCall(2f, () =>
        {
            PoolSystem.ReturnObject(effect);
        });
    }
    #endregion

    #region StartTargetAdventureAnim
    public void PlayAnimStartTarget(TargetData targetData)
    {
        _startTargetAdventureAnim.gameObject.SetActive(true);
        _startTargetAdventureAnim.StartAnim(targetData);
    }
    #endregion

    #region Board Effect
    public void HightLightBoard(bool isCombo)
    {
        EndComboBoard();

        _hightLightBoardImg.gameObject.SetActive(true);
        Vector3 initScale = _hightLightBoardImg.transform.localScale;
        _hightLightBoardImg.sprite = _hightLightBoard1;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
                        _hightLightBoardImg.transform.DOScale(initScale * 1.05f, 0.2f)
                                        .SetEase(Ease.OutBack)
                                        .OnComplete(() =>
                                        {
                                            _hightLightBoardImg.sprite = _hightLightBoard2;

                                        })
                        );
        sequence.Append(
        _hightLightBoardImg.transform.DOScale(initScale, 0.2f)
                        .SetEase(Ease.InOutQuad)
                        .OnComplete(() =>
                        {
                            _hightLightBoardImg.gameObject.SetActive(false);
                            if (isCombo)
                                StartComboBoard();
                        })
                        );
    }
    public void StartComboBoard()
    {
        _boardComboImg.gameObject.SetActive(true);
        _boardComboImg.DOFade(0.5f, 0f);
        _boardComboImg.transform.localScale = Vector3.one * 1.1f;

        _boardComboImg.transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        _boardComboImg.DOFade(0.8f, 0.3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
    public void EndComboBoard()
    {
        _boardComboImg.DOKill();
        _boardComboImg.transform.DOKill();
        _boardComboImg.gameObject.SetActive(false);
    }
    #endregion

    #region GoalItem
    public void PlayAnimGoalItemFly(Vector3 position, GoalItemType goalItemType, int newCount)
    {
        var gem = PoolSystem.RequireObject(_gemFlyEffect);
        gem.transform.SetParent(transform);
        gem.transform.position = position;
        gem.sprite = _itemGoalDict[goalItemType];

        Transform gemTf = gem.transform;
        gemTf.localScale = Vector3.zero;

        UIItemTarget target = _itemTargets.Find(e => e.GoalItemType == goalItemType);
        if (target == null) return;

        Vector3 startPos = gemTf.position;
        Vector3 targetPos = target.transform.position;

        float screenMidX = Screen.width * 0.5f;
        bool isLeftScreen = position.x < screenMidX;

        // === Config ===
        float scaleDuration = 0.3f;
        float delayAfterScale = 0.1f;
        float downDistance = 5f;
        float sideDistance = 3f;
        float moveDownDuration = 0.2f;
        float moveToTargetDuration = 0.5f;

        // === Tính điểm rơi xuống + lệch ===
        Vector3 downPos = startPos;
        downPos.y -= downDistance;
        downPos.x += isLeftScreen ? -sideDistance : sideDistance;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);

        // Scale lên 1
        seq.Append(gemTf.DOScale(1f, scaleDuration));

        // Delay
        seq.AppendInterval(delayAfterScale);

        // Bay xuống + lệch
        seq.Append(
            gemTf.DOMove(downPos, moveDownDuration)
                 .SetEase(Ease.InQuad)
        );

        // Bay về target
        seq.Append(
            gemTf.DOMove(targetPos, moveToTargetDuration)
                 .SetEase(Ease.InCubic)
        );

        seq.OnComplete(() =>
        {
            PoolSystem.ReturnObject(gem);

            // punch nhẹ target cho đã
            target.SetTargetCountAnim(newCount);

        });
    }

    #endregion

}
