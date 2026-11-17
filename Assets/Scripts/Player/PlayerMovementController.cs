using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    #region Attributes
    [Header("Movement Settings")]
    [field:SerializeField] public float WalkSpeed { get; private set; } = 3.0f;
    [field: SerializeField] public float CrouchSpeed { get; private set; } = 0f;
    [field: SerializeField] public float MaxFallSpeed { get; private set; } = 20f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask _groundLayer;

    [Header("Wall Check")]
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.05f, 0.5f);
    [SerializeField] private LayerMask _wallLayer;

    public Rigidbody2D PlayerRB2D {get; private set;}
    public Animator PlayerAnimator { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsTouchingWall { get; private set; }
    public bool IsMoving { get; set; }
    public bool IsWalking { get; set; }
    public bool IsCrouching { get; set; }
    public bool IsDashing { get; set; }
    public bool IsJumping { get; set; }
    public bool isFalling { get; set; }
    public float CurrentSpeed { get; set; }

    private PlayerInputHandler _inputHandler;
    private PlayerJump _playerJump;
    private PlayerDash _playerDash;
    private bool _subscribedToInputEvents;
    private float _facing = 1f; // 1 for right, -1 for left
    #endregion Attributes

    #region MonoBehaviour Methods
    private void Awake()
    {
        PlayerRB2D = GetComponent<Rigidbody2D>();
        PlayerAnimator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputHandler = PlayerInputHandler.Instance;
        TryGetMovementComponent<PlayerJump>(out _playerJump);
        TryGetMovementComponent<PlayerDash>(out _playerDash);
        Subscribe();
        GameManager.Instance.RegisterPlayer(gameObject);
        CurrentSpeed = WalkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        _playerJump?.UpdateJump(IsGrounded);
        _playerDash?.UpdateDash();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        _playerJump?.UpdateGravity();
        PlayerRB2D.linearVelocity = new Vector2(HandleXAxisMovement(), HandleYAxisMovement());
    }
    #endregion MonoBehaviour Methods

    #region Checks
    private void GroundCheck()
    {
        IsGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0f, _groundLayer);
    }

    private void WallCheck()
    {
        IsTouchingWall = Physics2D.OverlapBox(_wallCheck.position, _wallCheckSize, 0f, _wallLayer);
    }
    #endregion Checks

    #region Movement Handling
    private float HandleXAxisMovement()
    {
        Vector2 movementInput = _inputHandler.GetMovementInput();
        _facing = movementInput.x != 0 ? Mathf.Sign(movementInput.x) : _facing;
        transform.localScale = new Vector3(_facing, 1f, 1f); // Flip the player sprite based on movement direction
        WallCheck();
        if (IsTouchingWall && !IsGrounded)
        {
            return 0f; // Prevent horizontal movement when touching a wall in mid-air
        }
        return movementInput.x * CurrentSpeed;
    }

    private float HandleYAxisMovement()
    {
        float velocity_Y = Mathf.Clamp(PlayerRB2D.linearVelocityY, -MaxFallSpeed, float.MaxValue);
        if (IsDashing)
        {
            velocity_Y = 0f; // Prevent vertical movement during dash
        }
        return velocity_Y;
    }
    #endregion Movement Handling

    #region Other Methods
    private void UpdateAnimator()
    {
        PlayerAnimator.SetBool("IsGrounded", IsGrounded);
        PlayerAnimator.SetFloat("SpeedX", Mathf.Abs(PlayerRB2D.linearVelocityX));
        PlayerAnimator.SetFloat("SpeedY", PlayerRB2D.linearVelocityY);
    }

    private bool TryGetMovementComponent<T>(out T component) where T : MonoBehaviour, IPlayerMovementComponent
    {
        component = GetComponent<T>();
        if (component == null)
        {
            Debug.LogWarning($"{typeof(T).Name} component not found on this GameObject.");
            return false;
        }
        component.Initialize(this);
        return true;
    }
    #endregion Other Methods

    #region Event Subscription
    private void Subscribe(bool subscribe=true)
    {
        if (subscribe)
        {
            if (_subscribedToInputEvents) return;
            if (_playerJump != null)
            {
                PlayerInputHandler.OnJumpPressed += _playerJump.OnJumpStarted;
                PlayerInputHandler.OnJumpReleased += _playerJump.OnJumpCanceled;
            }
            if (_playerDash != null)
            {
                PlayerInputHandler.OnDashPressed += _playerDash.OnDash;
            }
            _subscribedToInputEvents = subscribe;
        }
        else
        {
            if (_playerJump != null)
            {
                PlayerInputHandler.OnJumpPressed -= _playerJump.OnJumpStarted;
                PlayerInputHandler.OnJumpReleased -= _playerJump.OnJumpCanceled;
            }
            if (_playerDash != null)
            {
                PlayerInputHandler.OnDashPressed -= _playerDash.OnDash;
            }
            _subscribedToInputEvents = subscribe;
        }
    }

    private void OnDisable()
    {
        if (_subscribedToInputEvents)
        {
            Subscribe(false);
        }
    }

    private void OnDestroy()
    {
        if(_subscribedToInputEvents)
        {
            Subscribe(false);
        }
        GameManager.Instance.UnregisterPlayer();
    }
    #endregion Event Subscription

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (IsGrounded)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        }

        if (IsTouchingWall)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_wallCheck.position, _wallCheckSize);
        }
    }
    #endregion Gizmos
}
