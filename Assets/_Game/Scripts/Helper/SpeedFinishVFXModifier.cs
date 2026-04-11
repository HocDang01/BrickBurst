using UnityEngine;


public class SpeedFinishVFXModifier : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private float _simulationSpeed = 1.5f;

    [SerializeField] private float _childScale = 5f;

    // [ContextMenu("Change Speed Children")]
    // [Button]
    // public void ChangeSpeedChildren()
    // {
    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         var child = transform.GetChild(i);

    //         if (child.TryGetComponent<ParticleSystem>(out var ps))
    //         {
    //             var main = ps.main;
    //             main.simulationSpeed = _simulationSpeed;
    //         }
    //     }
    // }

    [ContextMenu("Change Scale Children")]
    public void ChangeScaleChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            child.transform.localScale = Vector3.one * _childScale;
        }
    }
#endif
}
