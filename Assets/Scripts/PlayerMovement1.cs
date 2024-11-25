using UnityEngine;
using System.Collections;



public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] private float _moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float dashSpeedChangeFactor;
    public float dashSpeed;
    public float maxYSpeed;
    public float swingSpeed;

    private float _desiredMoveSpeed;
    private float _lastDesiredMoveSpeed;
    private MovementState _lastState;
    private bool _keepMomentum;

    [Header("Settings")] public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Jumping")] public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool _readyToJump;
    private int _jumpCount;
    public int maxJumps = 1;

    [Header("Climbing")] public float climbSpeed;
    public Climbing climbingScript;

    [Header("Crouching")] public float crouchSpeed;
    public float crouchYScale;
    private float _startYScale;

    [Header("Keybinds")] public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")] public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")] public float maxSlopeAngle;
    private RaycastHit _slopeHit;
    private bool _exitingSlope;

    [Header("Camera Effects")] public PlayerCam cam;
    public float grappleFov = 75;

    [Header("Guns")] 
    public GameObject grappleGun;
    public GameObject shootingGun;
    public RaycastWeapon weapon;

    public bool grappleGunActive = true;


    public Transform orientation;

    float _horizontalInput;
    float _verticalInput;

    Vector3 _moveDirection;

    Rigidbody _rb;

    public MovementState state;

    public bool climbing;
    public bool crouching;
    public bool dashing;
    public bool freeze;
    public bool activeGrapple;
    public bool swinging;

    public enum MovementState
    {
        walking,
        sprinting,
        swinging,
        crouching,
        freeze,
        climbing,
        dashing,
        air
    }

    private void Start()
    {
        climbingScript = GetComponent<Climbing>();
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _readyToJump = true;

        _startYScale = transform.localScale.y;
        _jumpCount = maxJumps;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        Debug.Log("grounded: " + grounded);

        MyInput();
        SpeedControl();
        StateHandler();


        // handle drag
        if ((state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching) && !activeGrapple)
        {
            _jumpCount = maxJumps;
            _rb.drag = groundDrag;
        }
        else
            _rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        

        if (Input.GetKeyDown(KeyCode.Q))
        {
            grappleGunActive = !grappleGunActive;
            grappleGun.SetActive(grappleGunActive);
            shootingGun.SetActive(!grappleGunActive);
        }

        // when to jump
        if (Input.GetKey(jumpKey) && _readyToJump &&
            ((state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching) ||
             _jumpCount > 0))
        {
            _readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey) && _horizontalInput == 0 && _verticalInput == 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            crouching = true;
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
            crouching = false;
        }
        
        if (Input.GetButtonDown("Fire1") && !grappleGunActive)
        {
            weapon.StartFiring();
        }
        
        if(weapon.isFiring && !grappleGunActive)
        {
            weapon.UpdateFiring(Time.deltaTime);
        }
        
        weapon.UpdateBullets(Time.deltaTime);
        
        if(Input.GetButtonUp("Fire1") && !grappleGunActive)
        {
            weapon.StopFiring();
        }
    }

    private void StateHandler()
    {
        
        //Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            _desiredMoveSpeed = 0;
            _rb.velocity = Vector3.zero;
        }
        
        // Mode - Dashing
        else if (dashing)
        {
            state = MovementState.dashing;
            _desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        
        // Mode - Swinging
        else if(swinging)
        {
            state = MovementState.swinging;
            _desiredMoveSpeed = swingSpeed;
        }
        
        // Mode - Climbing
        else if (climbing)
        {
            
            state = MovementState.climbing;
            _desiredMoveSpeed = climbSpeed;
        }

        // Mode - Crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            _desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            _desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            _desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            _desiredMoveSpeed = _desiredMoveSpeed < sprintSpeed ? walkSpeed : sprintSpeed;
        }
        
        bool desiredMoveSpeedHasChanged = _desiredMoveSpeed != _lastDesiredMoveSpeed;
        if (_lastState == MovementState.dashing) _keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (_keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                _moveSpeed = _desiredMoveSpeed;
            }
        }
        
        _lastDesiredMoveSpeed = _desiredMoveSpeed;
        _lastState = state;
    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(_desiredMoveSpeed - _moveSpeed);
        float startValue = _moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            _moveSpeed = Mathf.Lerp(startValue, _desiredMoveSpeed, time / difference);
            
            time += Time.deltaTime * boostFactor;

            // if (OnSlope())
            // {
            //     float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //     float slopeAngleIncrease = 1 + (slopeAngle / 90f);
            //
            //     time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            // }
            // else
            //     time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        _moveSpeed = _desiredMoveSpeed;
        speedChangeFactor = 1f;
        _keepMomentum = false;
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;
        if (swinging) return;
        if (climbingScript.exitingWall) return;
        if (state == MovementState.dashing) return;
        // calculate movement direction
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        // on slope
        if (OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection() * _moveSpeed * 20f, ForceMode.Force);

            if (_rb.velocity.y > 0)
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // on ground
        else if (grounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);

            // Reduce velocity to stop sliding
            if (_rb.velocity.magnitude > _moveSpeed)
            {
                _rb.velocity = new Vector3(_rb.velocity.x * 0.5f, _rb.velocity.y, _rb.velocity.z * 0.5f);
            }
        }
        // in air
        else if (!grounded)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // turn gravity off while on slope
        _rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(activeGrapple) return;
        // limiting speed on slope
        if (OnSlope() && !_exitingSlope)
        {
            if (_rb.velocity.magnitude > _moveSpeed)
                _rb.velocity = _rb.velocity.normalized * _moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }
        
        if(maxYSpeed != 0 && _rb.velocity.y > maxYSpeed)
            _rb.velocity = new Vector3(_rb.velocity.x, maxYSpeed, _rb.velocity.z);
    }

    private void Jump()
    {
        _exitingSlope = true;
        if (_jumpCount > 0)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        Debug.Log("Jump count:   " + _jumpCount);
        _jumpCount--;
    }

    private void ResetJump()
    {
        _readyToJump = true;

        _exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }
    
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
                                               + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
    
    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        _rb.velocity = velocityToSet;

        cam.DoFov(grappleFov);
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
        cam.DoFov(60f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }
    
    private bool enableMovementOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }
}