using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{

    public float sensX;
    public float sensY;

    public Transform orientation;

    float _xRotation;
    float _yRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.fixedDeltaTime;
        
        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
    }
    
    public void DoFov(float targetFov)
    {
        GetComponent<Camera>().DOFieldOfView(targetFov, 0.25f);
    }
}
