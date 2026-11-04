using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class PlayerActionsController : MonoBehaviour
{
    [field:SerializeField] public Transform ShootingPoint { get; set; }
    private PlayerInputHandler _inputHandler;
    private PlayerReticle _playerReticle;
    private PlayerEnergyGun _playerEnergyGun;


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
            Debug.Log("EnergyGun component found and initialized.");
        }
        LockCursor();
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
}
