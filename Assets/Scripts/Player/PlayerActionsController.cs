using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

/* TODO: Consider overall simplification by binding weapons directly to input events, eliminating the need for a centralized controller.
 * OR by passing events from the input handler through the controller, and handling them in the individual components. */

public class PlayerActionsController : MonoBehaviour
{
    public Animator PlayerAnimator { get; private set; }
    private PlayerInputHandler _inputHandler;
    private PlayerReticle _playerReticle;
    private PlayerEnergyGun _playerEnergyGun;
    private bool _subscridedToInputEvents;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputHandler = PlayerInputHandler.Instance;
        if (TryGetActionComponent<PlayerReticle>(out _playerReticle))
        {
            Debug.Log("Reticle component found and initialized.");
        }
        if (TryGetActionComponent<PlayerEnergyGun>(out _playerEnergyGun))
        {
            Debug.Log("Energy Gun component found and initialized.");
        }
        LockCursor();
        Subscribe();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 aimInput = _inputHandler.GetLookInput();
        _playerReticle?.Aim(aimInput, _playerEnergyGun.ShootingPoint);
    }

    private void LateUpdate()
    {        
        _playerReticle?.KeepReticleInScreen();
    }

    private bool TryGetActionComponent<T>(out T component) where T : MonoBehaviour, IPlayerActionComponent
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

    private void LockCursor(bool locked = true)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void Subscribe(bool subscribe = true)
    {
        if (subscribe)
        {
            if (_subscridedToInputEvents) return;
            if (_playerEnergyGun != null)
            {
                PlayerInputHandler.OnAttackPressed += _playerEnergyGun.StartShooting;
                PlayerInputHandler.OnAttackReleased += _playerEnergyGun.StopShooting;
            }
            _subscridedToInputEvents = subscribe;
        }
        else
        {
            if (_playerEnergyGun != null)
            {
                PlayerInputHandler.OnAttackPressed -= _playerEnergyGun.StartShooting;
                PlayerInputHandler.OnAttackReleased -= _playerEnergyGun.StopShooting;
            }
            _subscridedToInputEvents = subscribe;
        }
    }
}
