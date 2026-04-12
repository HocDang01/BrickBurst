using UnityEngine;


public class SpeedFinishVFXModifier : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private float _simulationSpeed = 1.5f;

    [SerializeField] private Vector3 _childScale;
    [SerializeField] private Vector3 _childRotation;

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

            child.transform.localScale = _childScale;
        }
    }
    [ContextMenu("Change Rotate Children")]
    public void ChangeRotationChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            child.transform.localRotation = Quaternion.Euler(_childRotation);
        }
    }
#endif
}
