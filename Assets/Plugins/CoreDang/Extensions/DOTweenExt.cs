using DG.Tweening;
using UnityEngine;

public static class DOTweenExt
{
    public static Tween DOJump3D(this Transform transform, Vector3 target, float jumpPower, float duration)
    {
        Vector3 middlePos = (transform.position + target) / 2;
        middlePos.y += jumpPower;

        Vector3[] wayPoint = new Vector3[] { transform.position, middlePos, target };
        return transform.DOPath(wayPoint, duration, PathType.CatmullRom, PathMode.Full3D);
    }

    public static Tween DOJump3D(this Transform transform, Vector3 target, float duration, Vector3 midPointOffset)
    {
        Vector3 middlePos = (transform.position + target) / 2;
        middlePos += midPointOffset;

        Vector3[] wayPoint = new Vector3[] { transform.position, middlePos, target };
        return transform.DOPath(wayPoint, duration, PathType.CatmullRom, PathMode.Full3D);
    }

    public static Tween DOJump3D(this Transform transform, Vector3 target, float duration, Vector3 midPointOffset1, Vector3 midPointOffset2)
    {
        Vector3 sum = transform.position + target;
        Vector3 point1 = sum / 3 + midPointOffset1;
        Vector3 point2 = sum * 2 / 3 + midPointOffset2;

        Vector3[] wayPoint = new Vector3[] { transform.position, point1, point2, target };
        return transform.DOPath(wayPoint, duration, PathType.CatmullRom, PathMode.Full3D);
    }
}