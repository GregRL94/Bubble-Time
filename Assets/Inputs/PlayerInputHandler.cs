using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static Action OnAttackPressed;
    public static Action OnAttackReleased;
    public static Action OnInteractPressed;
    public static Action OnCrouchPressed;
    public static Action OnCrouchReleased;
    public static Action OnJumpPressed;
    public static Action OnJumpReleased;
    public static Action OnDashPressed;
    public static Action<Vector2> OnLookDelta;
    public static Action<int> OnWeaponSwitch;

    public static PlayerInputHandler Instance { get; private set; }
    private InputSystem_Actions _inputActions;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        _inputActions = new InputSystem_Actions();
    }

    /// <summary>
    /// Gets the current movement input.
    /// </summary>
    /// <returns>The current movement input as a two-dimensional vector.</returns>
    public Vector2 GetMovementInput()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }

    /// <summary>
    /// Gets the current look input.
    /// </summary>
    /// <returns>The current look input as a two-dimensional vector.</returns>
    public Vector2 GetLookInput()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }

    /// <summary>
    /// Gets a boolean indicating whether the crouch input is pressed.
    /// </summary>
    /// <returns>true if the crouch input is pressed; otherwise, false.</returns>
    public bool CrouchPressed()
    {
        return _inputActions.Player.Crouch.ReadValue<bool>();
    }

    /// <summary>
    /// Gets a value indicating whether the jump action is pressed.
    /// </summary>
    /// <returns>true if the jump action is pressed; otherwise, false.</returns>
    public bool JumpPressed()
    {
        return _inputActions.Player.Jump.ReadValue<bool>();
    }

    /// <summary>
    /// Raises OnAttackPressed event when attack input is pressed, OnAttackReleased when it is released.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnAttackPressed?.Invoke();
        }
        else if (context.canceled)
        {
            OnAttackReleased?.Invoke();
        }
    }

    /// <summary>
    /// Handles interaction input and invokes the interaction event when the input starts.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInteractPressed?.Invoke();
        }
    }

    /// <summary>
    /// Raises the crouch-pressed or crouch-released notification based on the input action phase.
    /// </summary>
    /// <remarks>Raises the crouch-pressed notification when the action starts and the crouch-released
    /// notification when the action is canceled.</remarks>
    /// <param name="context">The input action callback context.</param>
    private void OnCrouchInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnCrouchPressed?.Invoke();
        }
        else if (context.canceled)
        {
            OnCrouchReleased?.Invoke();
        }
    }

    /// <summary>
    /// Invokes the corresponding callback when jump input is pressed or released.
    /// </summary>
    /// <param name="context">The input action callback context containing the current input phase.</param>
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnJumpPressed?.Invoke();
        }
        else if (context.canceled)
        {
            OnJumpReleased?.Invoke();
        }
    }

    /// <summary>
    /// Handles dash input events.
    /// </summary>
    /// <remarks>Invokes the dash-pressed event when the input action starts.</remarks>
    /// <param name="context">The callback context containing the dash input state.</param>
    private void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnDashPressed?.Invoke();
        }
    }

    /// <summary>
    /// Handles the next-input action by requesting a switch to weapon n+1 when the action starts.
    /// </summary>
    /// <param name="context">The callback context for the input action.</param>
    private void OnNextInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnWeaponSwitch?.Invoke(1);
        }
    }

    /// <summary>
    /// Handles the previous-input action.
    /// </summary>
    /// <remarks>Invokes the weapon switch notification with a previous-selection value when the action
    /// starts.</remarks>
    /// <param name="context">The input action callback context.</param>
    private void OnPreviousInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnWeaponSwitch?.Invoke(-1);
        }
    }

    /// <summary>
    /// Subscribes to or unsubscribes from player input actions.
    /// </summary>
    /// <param name="subscribe">true to subscribe to input actions; false to unsubscribe.</param>
    private void Subscribe(bool subscribe=true)
    {
        if (subscribe)
        {
            _inputActions.Player.Attack.started += OnAttackInput;
            _inputActions.Player.Attack.canceled += OnAttackInput;
            _inputActions.Player.Interact.started += OnInteractInput;
            _inputActions.Player.Crouch.started += OnCrouchInput;
            _inputActions.Player.Crouch.canceled += OnCrouchInput;
            _inputActions.Player.Jump.started += OnJumpInput;
            _inputActions.Player.Jump.canceled += OnJumpInput;
            _inputActions.Player.Dash.started += OnDashInput;
            _inputActions.Player.Look.started += ctx => OnLookDelta?.Invoke(ctx.ReadValue<Vector2>());
            _inputActions.Player.Next.started += OnNextInput;
            _inputActions.Player.Previous.started += OnPreviousInput;
        }
        else
        {
            _inputActions.Player.Attack.started -= OnAttackInput;
            _inputActions.Player.Attack.canceled -= OnAttackInput;
            _inputActions.Player.Interact.started -= OnInteractInput;
            _inputActions.Player.Crouch.started -= OnCrouchInput;
            _inputActions.Player.Crouch.canceled -= OnCrouchInput;
            _inputActions.Player.Jump.started -= OnJumpInput;
            _inputActions.Player.Jump.canceled -= OnJumpInput;
            _inputActions.Player.Dash.started -= OnDashInput;
            _inputActions.Player.Look.started -= ctx => OnLookDelta?.Invoke(ctx.ReadValue<Vector2>());
            _inputActions.Player.Next.started -= OnNextInput;
            _inputActions.Player.Previous.started -= OnPreviousInput;
        }
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        Subscribe();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        Subscribe(false);
    }
}
