using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody _rb;
    private PlayerMovement _pm;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("CameraEffects")]
    public PlayerCam cam;
    public float dashFov;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    private float _dashCdTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(dashKey))
            Dash();

        if (_dashCdTimer > 0)
            _dashCdTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        if (_dashCdTimer > 0) return;
        _dashCdTimer = dashCd;

        _pm.dashing = true;
        _pm.maxYSpeed = maxDashYSpeed;

        cam.DoFov(dashFov);

        Transform forwardT;

        if (useCameraForward)
            forwardT = playerCam;
        else
            forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            _rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if (resetVel)
            _rb.velocity = Vector3.zero;

        _rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        _pm.dashing = false;
        _pm.maxYSpeed = 0;

        cam.DoFov(60f);

        if (disableGravity)
            _rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }
}
