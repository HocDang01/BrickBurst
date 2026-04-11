using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script will work like Shape.cs but we just using for booster one tile.
/// </summary>
public class ShapeBooster : Shape
{
    private Booster _host;
    public void SetHost(Booster booster)
    {
        _host = booster;
    }
    public override void DestroyShape()
    {
        base.DestroyShape();
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
    }
    public override void BackToPos()
    {
        base.BackToPos();
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_host.Amount <= 0) return;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
        base.OnPointerClick(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (_host.Amount <= 0) return;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
        base.OnPointerUp(eventData);
        _host.OnEndUsing();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (_host.Amount <= 0) return;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.OneTile;
        base.OnPointerDown(eventData);
        _host.OnUsing();
        transform.localScale = Vector3.one / _host.UsingScale;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (_host.Amount <= 0) return;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.OneTile;
        base.OnBeginDrag(eventData);
        _host.OnUsing();
        transform.localScale = Vector3.one / _host.UsingScale;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (_host.Amount <= 0) return;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.OneTile;
        base.OnDrag(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (_host.Amount <= 0) return;
        BoosterManager.Ins.BoosterTypeUsing = BoosterType.None;
        _host.OnEndUsing();
        base.OnEndDrag(eventData);
    }
}
