using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    private Camera _mainCamera;
    private Ray _ray;
    private RaycastHit _hitInfo;
    
    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        _ray.origin = _mainCamera.transform.position;
        _ray.direction = _mainCamera.transform.forward;
        Physics.Raycast(_ray, out _hitInfo);
        transform.position = _hitInfo.point;
    }
}
