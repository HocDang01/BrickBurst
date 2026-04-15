using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEffectSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem _effect;
    [SerializeField] private Camera _mainCam;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 0f;
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(pos);

            _effect.transform.position = worldPos;
            _effect.Play();
            // if (EventSystem.current.IsPointerOverGameObject())
            // {
            // }

        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 pos = Input.GetTouch(0).position;
            pos.z = 0f;

            Vector3 worldPos = _mainCam.ScreenToWorldPoint(pos);
            _effect.transform.position = worldPos;
            _effect.Play();
        }
    }
}