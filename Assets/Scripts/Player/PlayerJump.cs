using UnityEngine;

public class PlayerJump : MonoBehaviour, IPlayerMovementComponent
{
    [Header("Jump")]
    [SerializeField] private float _jumpHeight = 3.5f;          // in world units
    [SerializeField] private float _timeToApex = 0.35f;         // seconds to reach the top

    [Header ("Double Jump")]
    [SerializeField] private bool _allowDoubleJump = true;       // whether the player can perform a second jump while in the air
    [SerializeField] private int _maxJumps = 2;                   // maximum number of jumps allowed

    [Header("Gravity Feel")]
    [SerializeField] private float _fallGravityMultiplier = 1.8f; // heavier on the way down
    [SerializeField] private float _cutGravityMultiplier = 3f;    // applied when jump is released early

    [Header("Forgiveness")]
    [SerializeField] private float _coyoteTime = 0.1f;          // can still jump just after leaving a ledge
    [SerializeField] private float _jumpBufferTime = 0.12f;     // jump pressed slightly before landing still counts

    private Rigidbody2D _targetRb2D;
    private float _baseGravity;
    private float _jumpVelocity;
    private float _coyoteCounter;
    private float _bufferCounter;
    private int _jumpCount;
    private bool jumpHeld;

    public void Initialize(PlayerMovementController movementController)
    {
        _targetRb2D = movementController.PlayerRB2D;
        CalculateJump();
    }

    /// <summary>
    /// Updates the character's jump state and applies a jump velocity when jump conditions are met.
    /// </summary>
    /// <param name="grounded">Whether the character is currently grounded.</param>
    public void UpdateJump(bool grounded)
    {
        if (!grounded && _allowDoubleJump && _jumpCount < _maxJumps && _bufferCounter > 0f)
        {
            _targetRb2D.linearVelocity = new Vector2(_targetRb2D.linearVelocity.x, _jumpVelocity);
            _jumpCount++;
            _bufferCounter = 0f;
            return;
        }

        if (grounded) { _jumpCount = 0; }

        _coyoteCounter = grounded ? _coyoteTime : _coyoteCounter - Time.deltaTime;
        _bufferCounter -= Time.deltaTime;

        if (_bufferCounter > 0f && _coyoteCounter > 0f && _jumpCount < _maxJumps)
        {
            _targetRb2D.linearVelocity = new Vector2(_targetRb2D.linearVelocity.x, _jumpVelocity);
            _coyoteCounter = 0f;
            _bufferCounter = 0f;
        }
    }

    /// <summary>
    /// Updates the target Rigidbody2D's gravity scale based on vertical velocity and jump input.
    /// </summary>
    /// <remarks>Applies increased gravity while falling, reduced gravity when the jump is released during
    /// ascent, and the base gravity otherwise.</remarks>
    public void UpdateGravity()
    {
        Vector2 v = _targetRb2D.linearVelocity;

        if (v.y < 0f)
            _targetRb2D.gravityScale = _baseGravity * _fallGravityMultiplier;   // snappier fall
        else if (v.y > 0f && !jumpHeld)
            _targetRb2D.gravityScale = _baseGravity * _cutGravityMultiplier;    // short hop when released early
        else
            _targetRb2D.gravityScale = _baseGravity;
    }

    /// <summary>
    /// Calculates the gravity and jump velocity required to reach the configured jump height and time to apex.
    /// </summary>
    private void CalculateJump()
    {
        float gravity = (2f * _jumpHeight) / (_timeToApex * _timeToApex);
        _baseGravity = gravity / Physics2D.gravity.magnitude;   // scale relative to project gravity
        _jumpVelocity = gravity * _timeToApex;
        _targetRb2D.gravityScale = _baseGravity;
    }

    public void OnJumpStarted()
    {
        _bufferCounter = _jumpBufferTime;
        jumpHeld = true;
    }

    public void OnJumpCanceled()
    {
        jumpHeld = false;
    }

}
