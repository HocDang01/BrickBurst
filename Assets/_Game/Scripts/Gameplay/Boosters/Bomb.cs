using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bomb : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Vector2 offset = new Vector2(0f, 0);
    [SerializeField] private float _dragScalae = 1.3f;
    private Camera _cam;
    private Canvas _canvas;
    private Vector3 _initPosition;
    private Vector3 _initBombScale;
    private Booster _host;
    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _initPosition = transform.localPosition;
        _initBombScale = transform.localScale;
    }
    void OnEnable()
    {
        _cam = Camera.main;
    }
    public void SetHost(Booster booster)
    {
        _host = booster;
    }
    public void BackToInitPos()
    {
        transform.localPosition = _initPosition;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false || _host.Amount <= 0)
        {
            return;
        }
        if (GameplayManager.Ins.DraggingShape != null) return;
        transform.DOKill();
        transform.localScale = Vector3.one;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                eventData.position, _cam, out Vector2 pos);
        transform.localPosition = pos + offset;
        transform.localScale = _initBombScale * _dragScalae;
        BoosterManager.Ins.BombDragging = this;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.Bomb;
        _host.OnUsing();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false || _host.Amount <= 0)
        {
            return;
        }
        if (BoosterManager.Ins.BombDragging != null && BoosterManager.Ins.BombDragging != this) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                eventData.position, _cam, out Vector2 pos);
        Vector3 targetPos = pos + offset;
        transform.localPosition = Vector3.Lerp(
                    transform.localPosition,
                    targetPos,
                    25f * Time.deltaTime
                );
        transform.localScale = _initBombScale * _dragScalae;
        BoosterManager.Ins.BombDragging = this;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.Bomb;
        _host.OnUsing();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameplayManager.Ins.IsInGame == false || _host.Amount <= 0)
        {
            return;
        }
        if (BoosterManager.Ins.BombDragging != null && BoosterManager.Ins.BombDragging != this) return;
        BoosterManager.Ins.CheckBombPlace();
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
        transform.localScale = _initBombScale;
        _host.OnEndUsing();
    }
}
