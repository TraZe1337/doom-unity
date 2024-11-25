using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovement pm;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float _climbTimer;

    private bool _climbing;

    [Header("ClimbJumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    private int _climbJumpsLeft;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float _wallLookAngle;

    private RaycastHit _frontWallHit;
    private bool _wallFront;

    private Transform _lastWall;
    private Vector3 _lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float _exitWallTimer;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (_climbing && !exitingWall) ClimbingMovement();
    }

    private void StateMachine()
    {
        // State 1 - Climbing
        if (_wallFront && Input.GetKey(KeyCode.W) && _wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!_climbing && _climbTimer > 0) StartClimbing();

            // timer
            if (_climbTimer > 0) _climbTimer -= Time.deltaTime;
            if (_climbTimer < 0) StopClimbing();
        }

        // State 2 - Exiting
        else if (exitingWall)
        {
            if (_climbing) StopClimbing();

            if (_exitWallTimer > 0) _exitWallTimer -= Time.deltaTime;
            if (_exitWallTimer < 0) exitingWall = false;
        }

        // State 3 - None
        else
        {
            if (_climbing) StopClimbing();
        }

        if (_wallFront && Input.GetKeyDown(jumpKey) && _climbJumpsLeft > 0) ClimbJump();
    }

    private void WallCheck()
    {
        _wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out _frontWallHit, detectionLength, whatIsWall);
        _wallLookAngle = Vector3.Angle(orientation.forward, -_frontWallHit.normal);

        bool newWall = _frontWallHit.transform != _lastWall || Mathf.Abs(Vector3.Angle(_lastWallNormal, _frontWallHit.normal)) > minWallNormalAngleChange;

        if ((_wallFront && newWall) || pm.grounded)
        {
            _climbTimer = maxClimbTime;
            _climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        _climbing = true;
        pm.climbing = true;

        _lastWall = _frontWallHit.transform;
        _lastWallNormal = _frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        _climbing = false;
        pm.climbing = false;
    }

    private void ClimbJump()
    {
        exitingWall = true;
        _exitWallTimer = exitWallTime;

        Vector3 forceToApply = transform.up * climbJumpUpForce + _frontWallHit.normal * climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        _climbJumpsLeft--;
    }
}
