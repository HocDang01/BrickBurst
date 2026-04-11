using UnityEngine;

public class BoosterBomb : Booster
{
    [SerializeField] private Bomb _bomb;

    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        _bomb.SetHost(this);
    }
}
