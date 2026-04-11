using UnityEngine;

public class OutlineFadeController : MonoBehaviour
{
    public static OutlineFadeController Ins;

    [SerializeField] private float _minAlpha = 0.1f;
    [SerializeField] private float _maxAlpha = 0.55f;

    private float _speed = 7f;
    private float _time;
    public float CurrentAlpha { get; private set; }

    private void Awake()
    {
        Ins = this;
        _speed = GameConfig.Ins.EffectConfig.OutlineFadeSpeed;
    }

    private void Update()
    {
        _time += Time.deltaTime * _speed;
        if (_time > Mathf.PI * 2f)
            _time -= Mathf.PI * 2f;
        float t = (Mathf.Sin(_time) + 1f) * 0.5f; // 0..1
        CurrentAlpha = Mathf.Lerp(_minAlpha, _maxAlpha, t);
    }
}
