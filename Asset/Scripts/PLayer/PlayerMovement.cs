using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData Data;

    public bool mobile;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    public PlayerAnimator AnimHandler { get; private set; }
    #endregion

    #region STATE PARAMETERS

    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }

    public float LastOnGroundTime { get; private set; }

    public float strengBuff { get; private set; }

    private bool _isJumpCut;
    private bool _isJumpFalling;
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;
    private int _jumpsLeft;  // Track the number of jumps left
    public float _pauseTimeRemaining { get; private set; }

    #endregion

    #region INPUT PARAMETERS
    [HideInInspector] public Vector2 _moveInput;
    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [HideInInspector] public bool isOnPlatform;
    [HideInInspector] public Rigidbody2D platformRb;
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        platformRb = GetComponent<Rigidbody2D>();
        AnimHandler = GetComponent<PlayerAnimator>();
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;

        // Giảm thời gian tạm dừng
        if (_pauseTimeRemaining > 0)
        {
            _pauseTimeRemaining -= Time.deltaTime;
            RB.velocity = Vector2.zero;
            StopMoving();
            return;
        }
        #endregion


        #region INPUT HANDLER
        _moveInput.y = Input.GetAxisRaw("Vertical");
        if (!mobile)
        {
            _moveInput.x = Input.GetAxisRaw("Horizontal");
        }

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
        {
            OnJumpUpInput();
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsJumping)
        {
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                if (LastOnGroundTime < -0.1f)
                {
                    AnimHandler.justLanded = true;
                }

                LastOnGroundTime = Data.coyoteTime;
                _jumpsLeft = Data.jumpAmount;  // Reset jumps when grounded
            }
        }
        #endregion

        #region JUMP CHECKS
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;
            _isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
            _isJumpFalling = false;
        }

        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();

            AnimHandler.startedJumping = true;
        }
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            SetGravityScale(0);

            if (RB.velocity.y < 0 && _moveInput.y < 0)
            {
                //Much higher gravity if holding down
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
            }
            else if (_isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.velocity.y < 0)
            {
                //Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            //No gravity when dashing (returns to normal once initial dashAttack phase over)
            SetGravityScale(0);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        if (_pauseTimeRemaining > 0)
        {
            // Không thực hiện di chuyển nếu đang trong thời gian tạm dừng
            return;
        }

        Run(1);
    }

    // Phương thức gọi khi LoadLevel
    public void OnLoadLevel()
    {
        _pauseTimeRemaining = 0.5f; // Tạm dừng trong 1 giây sau khi LoadLevel
    }

    #region INPUT CALLBACKS
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
            _isJumpCut = true;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    public void MoveRight()
    {
        _moveInput.x = 1;
    }

    public void MoveLeft()
    {
        _moveInput.x = -1;
    }
    public void StopMoving()
    {
        _moveInput.x = 0;
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        if (isOnPlatform)
        {
            // Khi người chơi ở trên moving platform, tốc độ người chơi sẽ bằng với nó
            targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed + platformRb.velocity.x, lerpAmount);
        }
        else
        {
            targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);
        }

        #region Calculate AccelRate
        float accelRate;

        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion
        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion

        float speedDif = targetSpeed - RB.velocity.x;
        float movement = speedDif * accelRate;

        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    public void Jump()
    {
        AudioManager.instance.PlaySFX(Random.Range(1, 2));

        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        _jumpsLeft--;  // Decrease the number of jumps left

        float force = Data.jumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    public void JumpBuff()
    {
        AudioManager.instance.PlaySFX(Random.Range(1, 2));

        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        _jumpsLeft--;  // Decrease the number of jumps left

        float force = Data.jumpForce * 1.5f;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    public bool CanJump()
    {
        // Kiểm tra nếu thời gian Coyote Time đã kết thúc và nhân vật không còn số lần nhảy còn lại
        return _jumpsLeft > 0 && LastOnGroundTime > 0;
    }


    public bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    public void ResetJumps()
    {
        LastOnGroundTime = Data.coyoteTime;
        _jumpsLeft = Data.jumpAmount;
        IsJumping = false;
        _isJumpFalling = false;
        _isJumpCut = false;
    }

    public void OnSpringJump(float force)
    {
        // Reset các tham số liên quan đến nhảy
        IsJumping = true;
        _isJumpCut = false;
        _isJumpFalling = false;

        LastOnGroundTime = 0;
        _jumpsLeft--;  // Giảm số lần nhảy còn lại nếu cần thiết

        // Áp dụng lực đẩy lên
        RB.velocity = new Vector2(RB.velocity.x, 0);  // Đặt lại vận tốc y hiện tại
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion

    #region CHECK METHODS
    private void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    public bool IsCrushed()
    {
        // Tạo một box collider để kiểm tra va chạm với ground từ phía trên và phía dưới
        Bounds bounds = GetComponent<Collider2D>().bounds;
        Vector2 topCenter = new Vector2(bounds.center.x, bounds.max.y);
        Vector2 bottomCenter = new Vector2(bounds.center.x, bounds.min.y);

        float checkRadius = 0.1f;

        bool topCheck = Physics2D.OverlapCircle(topCenter, checkRadius, _groundLayer);
        bool bottomCheck = Physics2D.OverlapCircle(bottomCenter, checkRadius, _groundLayer);

        return topCheck && bottomCheck;
    }
    #endregion
}
