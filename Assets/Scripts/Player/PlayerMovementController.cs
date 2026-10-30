using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [field:SerializeField] public float WalkSpeed { get; private set; } = 3.0f;
    [field: SerializeField] public float CrouchSpeed { get; private set; } = 0f;
    [field: SerializeField] public float MaxFallSpeed { get; private set; } = 20f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask _groundLayer;

    public Rigidbody2D PlayerRB2D {get; private set;}
    public bool IsGrounded { get; set; }
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputHandler = PlayerInputHandler.Instance;
        PlayerRB2D = GetComponent<Rigidbody2D>();

        TryGetMovementComponent<PlayerJump>(out _playerJump);
        TryGetMovementComponent<PlayerDash>(out _playerDash);
        Subscribe();
        CurrentSpeed = WalkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        _playerJump?.UpdateJump(IsGrounded);
        _playerDash?.UpdateDash();
    }

    private void FixedUpdate()
    {
        _playerJump?.UpdateGravity();
        PlayerRB2D.linearVelocity = new Vector2(HandleXAxisMovement(), HandleYAxisMovement());
    }

    private void GroundCheck()
    {
        IsGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0f, _groundLayer);
    }

    private float HandleXAxisMovement()
    {
        Vector2 movementInput = _inputHandler.GetMovementInput();
        return movementInput.x * CurrentSpeed;
    }

    private float HandleYAxisMovement()
    {
        float velocity_Y = Mathf.Clamp(PlayerRB2D.linearVelocityY, -MaxFallSpeed, float.MaxValue);
        return velocity_Y;
    }

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

    //private void OnEnable()
    //{
    //    Subscribe();
    //}

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
    }

    private void OnDrawGizmos()
    {
        if (!IsGrounded) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
    }
}
