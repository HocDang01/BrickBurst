using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _duration = 0.2f;
    private Vector3 _originalPosition;

    void Awake()
    {
        _originalPosition = Vector3.zero;
        _duration = GameConfig.Ins.GameplayConfig.Duration;
    }
    void OnEnable()
    {
        GameEvents.ShakeCam += ShakeCam;
    }
    void OnDisable()
    {
        GameEvents.ShakeCam -= ShakeCam;
    }

    public void ShakeCam(ShakeParam shakeParam)
    {
        StartCoroutine(ShakeRoutine(_duration, shakeParam));
    }

    private IEnumerator ShakeRoutine(float duration, ShakeParam shakeParam)
    {
        float newSeverity = Mathf.Min(shakeParam.severity, 30f);
        Debug.Log("Shake with " + newSeverity);
        transform.localPosition = _originalPosition;
        float time = duration;
        while (time > 0)
        {
            Vector3 shakeOffset = Vector3.zero;
            if (shakeParam.horizontal)
            {
                shakeOffset.x = Random.Range(-1f, 1f) * newSeverity;
            }
            if (shakeParam.vertical)
            {
                shakeOffset.y = Random.Range(-1f, 1f) * newSeverity;
            }
            transform.localPosition = _originalPosition + shakeOffset;
            time -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _originalPosition;
    }
}
public struct ShakeParam
{
    public float severity;
    public bool vertical;
    public bool horizontal;
    public ShakeParam(float severity, bool vertical, bool horizontal)
    {
        this.severity = severity;
        this.vertical = vertical;
        this.horizontal = horizontal;
    }
}
