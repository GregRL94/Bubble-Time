using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3.0f;
    [SerializeField] private float _crouchSpeed = 0f;
    [SerializeField] private float _maxFallSpeed = 20f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask _groundLayer;

    private PlayerInputHandler _inputHandler;
    private PlayerJump _playerJump;
    private Rigidbody2D _rb2D;
    private float _currentSpeed;
    private bool _isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputHandler = PlayerInputHandler.Instance;
        _rb2D = GetComponent<Rigidbody2D>();
        if (!TryGetComponent<PlayerJump>(out _playerJump))
        {
            Debug.LogWarning("PlayerJump component not found on this GameObject.");
        }
        _playerJump.Initialize(_rb2D);
        _currentSpeed = _walkSpeed;
        Subscribe();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        _playerJump?.UpdateJump(_isGrounded);
    }

    private void FixedUpdate()
    {
        _playerJump?.UpdateGravity();
        Vector2 movement = new Vector2(HandleXAxisMovement(), HandleYAxisMovement());
        _rb2D.linearVelocity = movement;
    }

    private void GroundCheck()
    {
        _isGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0f, _groundLayer);
    }

    private float HandleXAxisMovement()
    {
        Vector2 movementInput = _inputHandler.GetMovementInput();
        return movementInput.x * _currentSpeed;
    }

    private float HandleYAxisMovement()
    {
        float velocity_Y = _rb2D.linearVelocityY;

        if (_rb2D.linearVelocityY < -_maxFallSpeed)
        {
            velocity_Y = -_maxFallSpeed;
        }

        return velocity_Y;
    }

    private void Subscribe()
    {
        if (_playerJump != null)
        {
            PlayerInputHandler.OnJumpPressed += _playerJump.OnJumpStarted;
            PlayerInputHandler.OnJumpReleased += _playerJump.OnJumpCanceled;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_isGrounded) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
    }
}
